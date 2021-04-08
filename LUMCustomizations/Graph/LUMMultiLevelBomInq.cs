using System;
using System.Collections.Generic;
using JAMS.AM;
using JAMS.AM.Attributes;
using LumCustomizations.DAC;
using PX.Data;
using PX.Objects.IN;

namespace LUMCustomizations.Graph
{
    public class LUMMultiLevelBomInq : MultiLevelBomInq
    {
        #region Event Handler
        protected void _(Events.RowSelected<AMMultiLevelBomFilter> e)
        {
            var row = e.Row;

            if (row != null)
            {
                bool enabled = PXSelect<LifeSyncPreference>.Select(this).TopFirst.EnableProdCostAnlys ?? false;

                PXUIFieldAttribute.SetEnabled<AMMultiLevelBomFilterExt.usrEnblItemRoundUp>(e.Cache, null, enabled);
            }
        }
        #endregion

        #region Override Method
        /// <summary>
        /// Override the parent method to add additional conditions to control the quantity round-up logic.
        /// </summary>
        protected override AMMultiLevelBomData CreateDetailRow(AMBomMatl amBomMatl, AMBomOper amBomOper, AMBomItem amBomItem, InventoryItem inventoryItem, AMBomItem parentBomItem,
                                                               int lineID, int level, decimal totalQtyReq, AMMultiLevelBomFilter filter, string levelBomid, string levelRevisionID)
        {
            var itemExt = inventoryItem.GetExtension<JAMS.AEF.InventoryItemAMExtension>();

            var qtyRequired = amBomMatl.QtyReq.GetValueOrDefault() * (1 + amBomMatl.ScrapFactor.GetValueOrDefault()) *
                                  (amBomMatl.BatchSize.GetValueOrDefault() == 0m ? 1m : 1 / amBomMatl.BatchSize.GetValueOrDefault());

            var totalQtyRequired = amBomMatl.QtyReq.GetValueOrDefault() * (1 + amBomMatl.ScrapFactor.GetValueOrDefault()) *
                                      (amBomMatl.BatchSize.GetValueOrDefault() == 0m ? 1m : totalQtyReq / amBomMatl.BatchSize.GetValueOrDefault());

            var baseTotalQtyRequired = amBomMatl.BaseQty.GetValueOrDefault() * (1 + amBomMatl.ScrapFactor.GetValueOrDefault()) *
                                           (amBomMatl.BatchSize.GetValueOrDefault() == 0m ? 1m : totalQtyReq / amBomMatl.BatchSize.GetValueOrDefault());

            // Add one custom field to determine the round-up logic.
            AMMultiLevelBomFilterExt filterExt = filter.GetExtension<AMMultiLevelBomFilterExt>();

            if (filterExt.UsrEnblItemRoundUp == true)
            {
                qtyRequired = itemExt.AMQtyRoundUp == false ? qtyRequired : Math.Ceiling(qtyRequired);
                totalQtyRequired = itemExt.AMQtyRoundUp == false ? totalQtyRequired : Math.Ceiling(totalQtyRequired);
                baseTotalQtyRequired = itemExt.AMQtyRoundUp == false ? baseTotalQtyRequired : Math.Ceiling(baseTotalQtyRequired);
            }

            var row = new AMMultiLevelBomData
            {
                ParentBOMID = parentBomItem.BOMID,
                RevisionID = parentBomItem.RevisionID,
                LineID = lineID,
                Level = level,
                InventoryID = amBomMatl.InventoryID,
                Descr = amBomMatl.Descr,
                ParentInventoryID = parentBomItem.InventoryID,
                ParentSubItemID = parentBomItem.SubItemID,
                ParentDescr = parentBomItem.Descr,
                SubItemID = amBomMatl.SubItemID,
                UOM = amBomMatl.UOM,
                ScrapFactor = amBomMatl.ScrapFactor.GetValueOrDefault(),
                BatchSize = amBomMatl.BatchSize.GetValueOrDefault(),
                BOMQtyReq = amBomMatl.QtyReq.GetValueOrDefault(),
                BaseQtyReq = amBomMatl.BaseQty.GetValueOrDefault(),
                QtyReq = qtyRequired,
                TotalQtyReq = totalQtyRequired,
                BaseTotalQtyReq = baseTotalQtyRequired,
                UnitCost = amBomMatl.UnitCost.GetValueOrDefault(),
                ExtCost = qtyRequired * amBomMatl.UnitCost.GetValueOrDefault(),
                TotalExtCost = totalQtyRequired * amBomMatl.UnitCost.GetValueOrDefault(),
                LineBOMID = amBomMatl.BOMID,
                LineRevisionID = amBomMatl.RevisionID,
                OperationID = amBomOper.OperationID,
                OperationCD = amBomOper.OperationCD,
                EffStartDate = parentBomItem.EffStartDate,
                EffEndDate = parentBomItem.EffEndDate,
                SiteID = parentBomItem.SiteID,
                Status = parentBomItem.Status,
                LineStatus = amBomItem.Status,
                MaterialStatus = amBomItem.Status,
                OperationDescription = amBomOper.Descr,
                WcID = amBomOper.WcID,
                IsHeaderRecord = false,
                SortOrder = amBomMatl.SortOrder
            };

            var materialSiteID = amBomMatl.SiteID ?? amBomItem.SiteID;

            if (filter.IgnoreReplenishmentSettings.GetValueOrDefault()
                || JAMS.AM.InventoryHelper.GetReplenishmentSource(this, row.InventoryID, materialSiteID) == INReplenishmentSource.Manufactured)
            {
                levelBomid = amBomMatl.CompBOMID;
                levelRevisionID = amBomMatl.CompBOMRevisionID;

                if (!string.IsNullOrWhiteSpace(levelBomid) && !string.IsNullOrWhiteSpace(levelRevisionID))
                {
                    AMBomItem bomItem = null;

                    if (Filter.Current.IncludeBomsOnHold.GetValueOrDefault())
                    {
                        bomItem = PXSelect<AMBomItem, Where<AMBomItem.bOMID, Equal<Required<AMBomItem.bOMID>>,
                                                            And<AMBomItem.revisionID, Equal<Required<AMBomItem.revisionID>>,
                                                                And2<Where<AMBomItem.status, Equal<AMBomStatus.hold>,
                                                                           Or<AMBomItem.status, Equal<AMBomStatus.active>>>,
                                                                     And<Where<Required<AMBomItem.effStartDate>, Between<AMBomItem.effStartDate, AMBomItem.effEndDate>,
                                                                               Or<Where<AMBomItem.effStartDate, LessEqual<Required<AMBomItem.effStartDate>>,
                                                                                        And<AMBomItem.effEndDate, IsNull>>>>>>>>>
                                                      .Select(this, levelBomid, levelRevisionID, Filter.Current.BOMDate.GetValueOrDefault(), Filter.Current.BOMDate.GetValueOrDefault());
                    }
                    else
                    {
                        bomItem = PXSelect<AMBomItem, Where<AMBomItem.bOMID, Equal<Required<AMBomItem.bOMID>>,
                                                            And<AMBomItem.revisionID, Equal<Required<AMBomItem.revisionID>>,
                                                                And<AMBomItem.status, Equal<AMBomStatus.active>,
                                                                    And<Where<Required<AMBomItem.effStartDate>, Between<AMBomItem.effStartDate, AMBomItem.effEndDate>,
                                                                              Or<Where<AMBomItem.effStartDate, LessEqual<Required<AMBomItem.effStartDate>>,
                                                                                       And<AMBomItem.effEndDate, IsNull>>>>>>>>>
                                                      .Select(this, levelBomid, levelRevisionID, Filter.Current.BOMDate.GetValueOrDefault(), Filter.Current.BOMDate.GetValueOrDefault());
                    }

                    if (bomItem == null)
                    {
                        PXTrace.WriteWarning(JAMS.AM.Messages.GetLocal(JAMS.AM.Messages.ComponentBOMRevisionNotActive, levelBomid, levelRevisionID, inventoryItem.InventoryCD));
                        return row;
                    }
                }

                if (!string.IsNullOrWhiteSpace(levelBomid) && string.IsNullOrWhiteSpace(levelRevisionID))
                {
                    var compBomItem = filter.IncludeBomsOnHold == false
                                      ? PrimaryBomIDManager.GetActiveRevisionBomItemByDate(this, levelBomid, filter.BOMDate.GetValueOrDefault())
                                      : PrimaryBomIDManager.GetNotArchivedRevisionBomItemByDate(this, levelBomid, filter.BOMDate.GetValueOrDefault());

                    if (compBomItem == null)
                    {
                        PXTrace.WriteWarning(JAMS.AM.Messages.GetLocal(JAMS.AM.Messages.NoActiveRevisionForBom, parentBomItem.BOMID));
                        return row;
                    }

                    levelRevisionID = compBomItem.RevisionID;
                }

                if (string.IsNullOrWhiteSpace(levelBomid))
                {
                    var bomItem = filter.IncludeBomsOnHold == false 
                                  ? PrimaryBomIDManager.GetActiveRevisionBomItemByDate(this, new PrimaryBomIDManager(this).GetPrimaryAllLevels(row.InventoryID,
                                                                                       materialSiteID, row.SubItemID), filter.BOMDate.GetValueOrDefault()) 
                                  : PrimaryBomIDManager.GetNotArchivedRevisionBomItemByDate(this, new PrimaryBomIDManager(this).GetPrimaryAllLevels(row.InventoryID,
                                                                                                materialSiteID, row.SubItemID), filter.BOMDate.GetValueOrDefault());

                    if (bomItem == null)
                    {
                        PXTrace.WriteWarning(JAMS.AM.Messages.GetLocal(JAMS.AM.Messages.NoActiveRevisionForBom, parentBomItem.BOMID));
                        return row;
                    }

                    levelBomid = bomItem.BOMID;
                    levelRevisionID = bomItem.RevisionID;
                }

                row.ManufacturingBOMID = levelBomid;
                row.ManufacturingRevisionID = levelRevisionID;
            }

            return row;
        }

        /// <summary>
        /// Override the parent method to add additional conditions to control the cost round-up logic.
        /// </summary>
        public override List<AMMultiLevelBomData> RollCostUpdate(List<AMMultiLevelBomData> multiLevelBomRecs)
        {
            List<AMMultiLevelBomData> list = new List<AMMultiLevelBomData>();
            RollBomList rollBomList = new RollBomList();
            HashSet<string> hashSet = new HashSet<string>();

            using (List<AMMultiLevelBomData>.Enumerator enumerator = multiLevelBomRecs.GetEnumerator())
            {
                for (; ; )
                {
                    AMMultiLevelBomData ammultiLevelBomData;
                    if (4 != 0)
                    {
                        if (!enumerator.MoveNext())
                        {
                            break;
                        }
                        ammultiLevelBomData = enumerator.Current;
                        string item = string.Join(":", new string[]
                        {
                            ammultiLevelBomData.LineBOMID,
                            ammultiLevelBomData.LineRevisionID
                        });
                        if (!hashSet.Add(item))
                        {
                            continue;
                        }
                    }
                    bool? isHeaderRecord = ammultiLevelBomData.IsHeaderRecord;
                    bool flag = true;
                    bool flag2 = isHeaderRecord.GetValueOrDefault() == flag & isHeaderRecord != null;
                    while (!flag2 && ammultiLevelBomData.InventoryID != null)
                    {
                        AMBomItem ambomItem = (AMBomItem)this.Caches<AMBomItem>().Locate(new AMBomItem
                        {
                            BOMID = ammultiLevelBomData.LineBOMID,
                            RevisionID = ammultiLevelBomData.LineRevisionID
                        });
                        if (ambomItem == null)
                        {
                            if (false)
                            {
                                goto IL_136;
                            }
                            ambomItem = PXSelect<AMBomItem, Where<AMBomItem.bOMID, Equal<Required<AMBomItem.bOMID>>, 
                                                                  And<AMBomItem.revisionID, Equal<Required<AMBomItem.revisionID>>>>>.Select(this, new object[] {
                                                                                                                                                                   ammultiLevelBomData.LineBOMID,
                                                                                                                                                                   ammultiLevelBomData.LineRevisionID
                                                                                                                                                               });
                        }
                        flag2 = rollBomList.Add(ambomItem, ammultiLevelBomData.Level.GetValueOrDefault(), false);
                        if (!false)
                        {
                            break;
                        }
                    }
                }
            IL_136:;
            }
            LumCostRoll bomcostRoll = PXGraph.CreateInstance<LumCostRoll>();

            RollupSettings value = new RollupSettings
            {
                SnglMlti = "M",
                SiteID = null,
                InventoryID = this.Filter.Current.InventoryID,
                SubItemID = null,
                BOMID = this.Filter.Current.BOMID,
                RevisionID = this.Filter.Current.RevisionID,
                EffectiveDate = this.Filter.Current.BOMDate,
                IncMatScrp = new bool?(true),
                IncFixed = new bool?(true),
                UpdateMaterial = new bool?(false),
                UsePending = new bool?(false),
                IgnoreMinMaxLotSizeValues = this.Filter.Current.IgnoreMinMaxLotSizeValues
            };
            bomcostRoll.Settings.Current = value;
            bomcostRoll.RollCosts(rollBomList, Filter.Current.GetExtension<AMMultiLevelBomFilterExt>().UsrEnblItemRoundUp.GetValueOrDefault());

            foreach (AMMultiLevelBomData ammultiLevelBomData2 in multiLevelBomRecs)
            {
                AMBomCost bomCostRec = null;
                if (!string.IsNullOrWhiteSpace(ammultiLevelBomData2.ManufacturingBOMID) && (6 == 0 || !string.IsNullOrWhiteSpace(ammultiLevelBomData2.ManufacturingRevisionID)))
                {
                    bomCostRec = (AMBomCost)bomcostRoll.Caches<AMBomCost>().Locate(new AMBomCost
                    {
                        BOMID = ammultiLevelBomData2.ManufacturingBOMID,
                        RevisionID = ammultiLevelBomData2.ManufacturingRevisionID,
                        UserID = new Guid?(bomcostRoll.Accessinfo.UserID)
                    });
                }
                AMMultiLevelBomData ammultiLevelBomData3 = this.RollCostUpdate(ammultiLevelBomData2, bomCostRec);

                if (ammultiLevelBomData3 != null)
                {
                    list.Add(ammultiLevelBomData3);
                }
            }
            return list;
        }
        #endregion
    }

    #region Extension DAC
    public class AMMultiLevelBomFilterExt : PXCacheExtension<AMMultiLevelBomFilter>
    {
        #region UsrEnblItemRoundUp
        [PXBool]
        [PXUnboundDefault(false)]
        [PXUIField(DisplayName = "Enable Item Round-Up")]
        public virtual bool? UsrEnblItemRoundUp { get; set; }
        public abstract class usrEnblItemRoundUp : PX.Data.BQL.BqlBool.Field<usrEnblItemRoundUp> { }
        #endregion
    }
    #endregion
}