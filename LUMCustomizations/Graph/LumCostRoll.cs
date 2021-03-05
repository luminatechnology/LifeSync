using JAMS;
using JAMS.AM;
using JAMS.AM.Attributes;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Force.DeepCloner;

namespace LUMCustomizations.Graph
{
    public class LumCostRoll : BOMCostRoll
    {
        public PXFilter<RollupSettings> rollsettings;

        public PXProcessingJoin<AMBomCost,
                InnerJoin<AMBomItem, On<AMBomCost.bOMID, Equal<AMBomItem.bOMID>,
                    And<AMBomCost.revisionID, Equal<AMBomItem.revisionID>>>>,
                Where<AMBomCost.userID, Equal<Current<AccessInfo.userID>>>> ProcBomCostRecs;

        public LumCostRoll()
        {
            ProcBomCostRecs.SetProcessVisible(false);
            ProcBomCostRecs.SetProcessAllCaption("Cost Roll");

            var _bomItem = new PXGraph().Select<AMBomItem>();
            if ((rollsettings.Current.LotSize == 0 || rollsettings.Current.LotSize == null) && rollsettings.Current.SnglMlti == "S")
            {
                rollsettings.Current.LotSize = 1000;
                rollsettings.Current.SnglMlti = "M";
            }


            if (ProcBomCostRecs.Select().Count == 0)
                BomCostRecs.Cache.Insert(DoingCostRoll(_bomItem.FirstOrDefault()));

            ProcBomCostRecs.SetProcessDelegate(delegate (List<AMBomCost> list)
            {
                rollsettings.Current.ApplyPend = false;
                rollsettings.Current.IncFixed = true;
                rollsettings.Current.IncMatScrp = true;
                rollsettings.Current.UpdateMaterial = false;
                rollsettings.Current.IsPersistMode = false;
                // Call the action to run and display the cost roll
                PXLongOperation.StartOperation(this, () => aMBomCostSummary(rollsettings.Current, _bomItem));
                rollsettings.ClearDialog();
            });
        }

        public IEnumerable aMBomCostSummary(RollupSettings _setting, IQueryable<AMBomItem> _bomItem)
        {
            // Delete All Data By User
            PXDatabase.Delete<AMBomCost>(new PXDataFieldRestrict<AMBomCost.userID>(Accessinfo.UserID));

            // Get All BOM Data
            int count = 0;
            string errorMsg = string.Empty;
            Settings.Current = _setting.DeepClone();
            List<object> result = new List<object>();
            foreach (var _bom in _bomItem)
            {
                try
                {
                    result.Add(DoingCostRoll(_bom));
                }
                catch (Exception ex)
                {
                    errorMsg += $"Error:{ex.Message} BOMID:{_bom.BOMID} Revision:{_bom.RevisionID}\n";
                }
            }
            // write Error Msg
            if (string.IsNullOrEmpty(errorMsg))
                PXProcessing.SetWarning(errorMsg);

            BomCostRecs.Cache.Clear();
            result.ForEach(x => { BomCostRecs.Cache.Insert(x); });
            Actions.PressSave();
            return null;
        }

        public Object DoingCostRoll(AMBomItem _bom)
        {
            Settings.Current.BOMID = _bom.BOMID; //Documents.Current.BOMID;
            Settings.Current.RevisionID = _bom.RevisionID; // Documents.Current.RevisionID;
            if (Settings.Current != null)
            {
                Settings.Current.EffectiveDate =
                    _bom.EffEndDate != null ? Settings.Current.EffectiveDate.LesserDateTime(_bom.EffEndDate)
                                            : Settings.Current.EffectiveDate;
                base.RollCosts();
            }
            return BomCostRecs.Cache.Current;
        }

        #region Overloading Method
        /// <summary>
        /// Roll Costs Call with BOM List.
        /// Copy standard method and add one more parameter to control qty round-up.
        /// </summary>
        public virtual bool RollCosts(RollBomList rollBomList, bool roundup = true)
        {
            var processedAll = true;

            base.BomCostRecs.Cache.Clear();

            using (new DisableSelectorValidationScope(BomCostRecs.Cache))
            {
                var orderedBomList = rollBomList.GetBomItemsByLevelDesc();
                foreach (var bomItem in orderedBomList)
                {
                    var levelDefault = rollBomList.GetLevelDefault(bomItem);
                    if (levelDefault == null)
                    {
                        if (!ProcessCost(bomItem, 0 , false, roundup))
                        {
                            processedAll = false;
                        }
                        continue;
                    }

                    if (!ProcessCost(bomItem, levelDefault.Item1, levelDefault.Item2, roundup))
                    {
                        processedAll = false;
                    }
                }
            }

            return processedAll;
        }

        /// <summary>
        /// Process Costs for a BOM.
        /// Copy standard method and add one more parameter to control qty round-up.
        /// </summary>
        protected virtual bool ProcessCost(AMBomItem bomItem, int level, bool isDefault, bool roundup = true)
        {
            var successful = true;

            if (bomItem?.BOMID == null)
            {
                return false;
            }

            var bomcostrec = new AMBomCost
            {
                InventoryID = bomItem.InventoryID,
                SubItemID = bomItem.SubItemID,
                BOMID = bomItem.BOMID,
                RevisionID = bomItem.RevisionID,
                SiteID = bomItem.SiteID,
                MultiLevelProcess = Settings.Current.SnglMlti == RollupSettings.SelectOptSM.Multi,
                UserID = this.Accessinfo.UserID,
                Level = level,
                // Might have to update later for subitem indication - currently only looks at INItemSite default BOM ID
                IsDefaultBom = isDefault
            };

            var invItem = JAMS.AM.InventoryHelper.CacheQueryInventoryItem(InvItemRecs.Cache, bomcostrec.InventoryID);

            bomcostrec.ItemClassID = invItem?.ItemClassID;

            //Set the Current and Pending cost from INItemSite
            var inItemSite = JAMS.AM.InventoryHelper.CacheQueryINItemSite(ItemSiteRecs.Cache, bomcostrec.InventoryID, bomcostrec.SiteID);

            bomcostrec.StdCost = inItemSite?.StdCost;
            bomcostrec.PendingStdCost = inItemSite?.PendingStdCost;

            // Set Lot Size based on Filter Settings
            if (Settings.Current.IgnoreMinMaxLotSizeValues == true)
            {
                bomcostrec.LotSize = 1;
            }
            else if (bomcostrec.BOMID == Settings.Current.BOMID && Settings.Current.LotSize.GetValueOrDefault() != 0
                     && Settings.Current.IgnoreMinMaxLotSizeValues == false)
            {
                bomcostrec.LotSize = Settings.Current.LotSize.GetValueOrDefault();
            }
            else
            {
                bomcostrec.LotSize = invItem == null 
                                     ? JAMS.AM.InventoryHelper.GetMfgReorderQty(this, bomcostrec.InventoryID, bomcostrec.SiteID) 
                                     : JAMS.AM.InventoryHelper.GetMfgReorderQty(this, invItem, inItemSite);
            }

            if (bomcostrec.LotSize.GetValueOrDefault() <= 0)
            {
                bomcostrec.LotSize = 1;
            }

            bomcostrec.FLaborCost = 0;
            bomcostrec.VLaborCost = 0;
            var laborCostAndHours = SetLaborCost(ref bomcostrec, Settings.Current?.IncFixed == true);

            bomcostrec.MachCost = GetMachineCost(bomcostrec);
            bomcostrec.ToolCost = GetToolCost(bomcostrec);

            var allMaterial = PXSelectReadonly2<AMBomMatl, InnerJoin<InventoryItem, On<AMBomMatl.inventoryID, Equal<InventoryItem.inventoryID>>,
                                                           LeftJoin<INItemSite, On<AMBomMatl.inventoryID, Equal<INItemSite.inventoryID>,
                                                                                   And<INItemSite.siteID, Equal<Required<INItemSite.siteID>>>>>>,
                                                           Where<AMBomMatl.bOMID, Equal<Required<AMBomMatl.bOMID>>,
                                                                 And<AMBomMatl.revisionID, Equal<Required<AMBomMatl.revisionID>>>>>.Select(this, bomcostrec.SiteID, bomcostrec.BOMID, bomcostrec.RevisionID);

            //Merge of Regular Material and Subcontract Material (excluding Reference/vendor supplied material)
            OperationCosts matlTotal = new OperationCosts();

            if (allMaterial.Count > 0)
            {
                var purchase     = new List<PXResult<AMBomMatl, InventoryItem>>();
                var manufactured = new List<PXResult<AMBomMatl, InventoryItem>>();
                var subcontract  = new List<PXResult<AMBomMatl, InventoryItem>>();
                var refMaterial  = new List<PXResult<AMBomMatl, InventoryItem>>();

                foreach (PXResult<AMBomMatl, InventoryItem, INItemSite> result in allMaterial)
                {
                    var bomMatl = (AMBomMatl)result;
                    if (bomMatl == null ||
                        (bomMatl.EffDate != null && bomMatl.EffDate > Accessinfo.BusinessDate) ||
                        (bomMatl.ExpDate != null && bomMatl.ExpDate <= Accessinfo.BusinessDate))
                    {
                        continue;
                    }

                    // Check for COMP BOMID, if exists, item is Manufactured
                    if (bomMatl.CompBOMID != null)
                    {
                        manufactured.Add(result);
                        continue;
                    }

                    if (bomMatl.MaterialType == AMMaterialType.Subcontract && bomMatl.SubcontractSource != AMSubcontractSource.VendorSupplied)
                    {
                        subcontract.Add(result);
                        continue;
                    }

                    if (bomMatl.MaterialType == AMMaterialType.Subcontract && bomMatl.SubcontractSource == AMSubcontractSource.VendorSupplied)
                    {
                        refMaterial.Add(result);
                        continue;
                    }

                    var replenishmentSource = JAMS.AM.InventoryHelper.GetReplenishmentSource((InventoryItem)result, (INItemSite)result);
                    if (replenishmentSource == INReplenishmentSource.Manufactured)
                    {
                        manufactured.Add(result);
                        continue;
                    }

                    if (replenishmentSource == INReplenishmentSource.Purchased)
                    {
                        purchase.Add(result);
                    }

                }

                var purchaseCost     = GetMaterialCost(bomcostrec, purchase, IsMultiLevel, out var purchaseMatlMessages, roundup);
                var manufacturedCost = GetMaterialCost(bomcostrec, manufactured, IsMultiLevel, out var manufacturedMatlMessages, roundup);
                var subcontractCost  = GetMaterialCost(bomcostrec, subcontract, IsMultiLevel, out var subContractMatlMessages, roundup);
                var refmaterialCost  = GetMaterialCost(bomcostrec, refMaterial, IsMultiLevel, out var refMaterialMatlMessages, roundup);

                if (purchaseMatlMessages != null)
                {
                    foreach (var matlMessage in purchaseMatlMessages)
                    {
                        successful = false;
                        PXTrace.WriteWarning(matlMessage);
                    }
                }

                if (manufacturedMatlMessages != null)
                {
                    foreach (var matlMessage in manufacturedMatlMessages)
                    {
                        successful = false;
                        PXTrace.WriteWarning(matlMessage);
                    }
                }

                if (subContractMatlMessages != null)
                {
                    foreach (var matlMessage in subContractMatlMessages)
                    {
                        successful = false;
                        PXTrace.WriteWarning(matlMessage);
                    }
                }

                if (refMaterialMatlMessages != null)
                {
                    foreach (var matlMessage in refMaterialMatlMessages)
                    {
                        successful = false;
                        PXTrace.WriteWarning(matlMessage);
                    }
                }

                bomcostrec.MatlManufacturedCost    = manufacturedCost?.TotalCost ?? 0m;
                bomcostrec.MatlNonManufacturedCost = purchaseCost?.TotalCost ?? 0m;
                bomcostrec.SubcontractMaterialCost = subcontractCost?.TotalCost ?? 0m;
                bomcostrec.ReferenceMaterialCost   = refmaterialCost?.TotalCost ?? 0m;

                matlTotal = new OperationCosts(manufacturedCost);
                matlTotal.Add(purchaseCost, true);
                matlTotal.Add(subcontractCost, true);
            }

            bomcostrec.FOvdCost = 0;
            bomcostrec.VOvdCost = 0;
            SetOverheadCosts(ref bomcostrec, Settings.Current.IncFixed.GetValueOrDefault(), matlTotal, laborCostAndHours.Item1, laborCostAndHours.Item2);

            bomcostrec.TotalCost = bomcostrec.FLaborCost.GetValueOrDefault()
                                   + bomcostrec.VLaborCost.GetValueOrDefault()
                                   + bomcostrec.MachCost.GetValueOrDefault()
                                   + bomcostrec.MatlManufacturedCost.GetValueOrDefault()
                                   + bomcostrec.MatlNonManufacturedCost.GetValueOrDefault()
                                   + bomcostrec.FOvdCost.GetValueOrDefault()
                                   + bomcostrec.VOvdCost.GetValueOrDefault()
                                   + bomcostrec.ToolCost.GetValueOrDefault()
                                   + bomcostrec.OutsideCost.GetValueOrDefault()
                                   + bomcostrec.DirectCost.GetValueOrDefault()
                                   + bomcostrec.SubcontractMaterialCost.GetValueOrDefault()
                                   + bomcostrec.ReferenceMaterialCost.GetValueOrDefault();

            bomcostrec.UnitCost = UomHelper.PriceCostRound(bomcostrec.TotalCost.GetValueOrDefault() / bomcostrec.LotSize.GetValueOrDefault());

            try
            {
                BomCostRecs.Insert(bomcostrec);
            }
            catch (Exception e)
            {
                if (e is PXOuterException)
                {
                    PXTraceHelper.PxTraceOuterException((PXOuterException)e, PXTraceHelper.ErrorLevel.Error);
                }

                InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, bomcostrec.InventoryID);

                if (item == null)
                {
                    PXTrace.WriteInformation(JAMS.AM.Messages.InvalidInventoryIDOnBOM, bomItem.BOMID);
                    successful = false;
                }
                else
                {
                    throw new PXException(JAMS.AM.Messages.GetLocal(JAMS.AM.Messages.UnableToSaveRecordForInventoryID), JAMS.AM.Messages.GetLocal(JAMS.AM.Messages.BOMCost), item.InventoryCD.Trim(), e.Message);
                }
            }

            return successful;
        }

        // <summary>
        /// Get the total BOM/Operations material cost.
        /// Copy standard method and add one more parameter to control qty round-up.
        /// </summary>
        protected virtual OperationCosts GetMaterialCost(AMBomCost currentAMBomCost, IEnumerable<PXResult<AMBomMatl, InventoryItem>> material, bool isMultLevel, out List<string> materialMessage, bool roundup = true)
        {
            materialMessage = new List<string>();

            if (currentAMBomCost == null || string.IsNullOrWhiteSpace(currentAMBomCost.BOMID) || material == null) { return null; }

            var operationMaterialCosts = new OperationCosts();

            foreach (var result in material)
            {
                var matlRec = (AMBomMatl)result;
                var inventoryItem = (InventoryItem)result;

                if (matlRec?.BOMID == null || inventoryItem?.InventoryCD == null) { continue; }

                decimal? unitcost = null;
                var matlSiteID = matlRec.SiteID ?? currentAMBomCost.SiteID;
                if (isMultLevel)
                {
                    var bomManager = new PrimaryBomIDManager(this);
                    var bomItem = PrimaryBomIDManager.GetNotArchivedRevisionBomItem(this, bomManager.GetPrimaryAllLevels(inventoryItem, 
                                                                                                                         JAMS.AM.InventoryHelper.CacheQueryINItemSite(ItemSiteRecs.Cache, matlRec.InventoryID, matlSiteID), 
                                                                                                                         matlRec.SubItemID));
                    unitcost = GetCurrentBomCost(bomItem?.BOMID, bomItem?.RevisionID);
                }

                if (unitcost == null)
                {
                    unitcost = GetUnitCost(inventoryItem, matlSiteID);
                }

                var inUnit = (INUnit)PXSelectorAttribute.Select<AMBomMatl.uOM>(this.Caches<AMBomMatl>(), matlRec) ??
                             (INUnit)PXSelect<INUnit, Where<INUnit.inventoryID, Equal<Required<INUnit.inventoryID>>,
                                                            And<INUnit.fromUnit, Equal<Required<INUnit.fromUnit>>>>>.Select(this, matlRec.InventoryID, matlRec.UOM);

                if (inUnit == null)
                {
                    materialMessage.Add(JAMS.AM.Messages.GetLocal(JAMS.AM.Messages.InvalidUOMForMaterialonBOM, matlRec.UOM.TrimIfNotNullEmpty(), inventoryItem.InventoryCD, matlRec.BOMID, matlRec.RevisionID));
                    continue;
                }

                if (UomHelper.TryConvertToBaseCost<AMBomMatl.inventoryID>(BomMatlRecs.Cache, matlRec, matlRec.UOM, unitcost.GetValueOrDefault(), out var matlUnitCost))
                {
                    unitcost = matlUnitCost.GetValueOrDefault();
                }

                var itemExt = inventoryItem.GetExtension<JAMS.AEF.InventoryItemAMExtension>();

                var totalQtyRequired = matlRec.QtyReq.GetValueOrDefault() *
                                       (1 + (Settings.Current.IncMatScrp.GetValueOrDefault() ? matlRec.ScrapFactor.GetValueOrDefault() : 0m)) * (matlRec.BatchSize.GetValueOrDefault() == 0m 
                                       ? 1m 
                                       : currentAMBomCost.LotSize.GetValueOrDefault() / matlRec.BatchSize.GetValueOrDefault());

                // Use new parameter to determine the qty request round-up per Peter's request.
                if (roundup == true)
                {
                    totalQtyRequired = itemExt.AMQtyRoundUp == false ? totalQtyRequired : Math.Ceiling(totalQtyRequired);
                }

                var matlCost = totalQtyRequired * unitcost.GetValueOrDefault();

                operationMaterialCosts.Add(matlRec.OperationID, matlCost, true);

                if (Settings.Current.UpdateMaterial.GetValueOrDefault())
                {
                    UpdateMaterialUnitCost(matlRec, unitcost);
                }
            }

            return operationMaterialCosts;
        }

        #endregion
    }
}
