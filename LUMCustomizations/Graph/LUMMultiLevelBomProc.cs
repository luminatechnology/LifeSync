using System;
using System.Collections.Generic;
using System.Linq;
using JAMS;
using JAMS.AM;
using JAMS.AM.Attributes;
using LumCustomizations.DAC;
using PX.Data;
using PX.Objects.IN;

namespace LUMCustomizations.Graph
{
    public class LUMMultiLevelBomProc : PXGraph<LUMMultiLevelBomProc>
    {
        #region Features & Select
        public PXCancel<AMMultiLevelBomFilter> Cancel;
        public PXFilter<AMMultiLevelBomFilter> Filter;

        [PXFilterable]
        public PXFilteredProcessing<LUMStdBomCost, AMMultiLevelBomFilter, Where<LUMStdBomCost.createdByID, Equal<Current<AccessInfo.userID>>>> Results;

        public PXSelect<LUMStdBomCost, Where<LUMStdBomCost.createdByID, Equal<Current<AccessInfo.userID>>>> BOMCost;
        #endregion

        #region Ctor
        public LUMMultiLevelBomProc()
        {
            Results.SetProcessVisible(false);
            Results.SetProcessAllCaption(PX.Objects.IN.Messages.Generate);

            if (BOMCost.Select().Count == 0) { InsertWrkTableRecs(this); }
        }
        #endregion

        #region Event Handler
        protected void _(Events.RowSelected<AMMultiLevelBomFilter> e)
        {
            AMMultiLevelBomFilter row = e.Row;

            if (row != null)
            {
                bool enabled = PXSelect<LifeSyncPreference>.Select(this).TopFirst.EnableProdCostAnlys ?? false;

                PXUIFieldAttribute.SetEnabled<AMMultiLevelBomFilterExt.usrEnblItemRoundUp>(e.Cache, row, enabled);
            }

            Results.SetProcessDelegate(delegate (List<LUMStdBomCost> lists)
            {
                try
                {
                    GenerateBOMCost(lists, row);

                    PXProcessing.SetProcessed();
                }
                catch (Exception ex)
                {
                    PXProcessing.SetError(ex);
                    throw;
                }
            });
        }
        #endregion

        #region Static Method
        public static void GenerateBOMCost(List<LUMStdBomCost> lists, AMMultiLevelBomFilter bomFilter)
        {
            var graph = CreateInstance<LUMMultiLevelBomProc>();

            graph.LoadAllData(bomFilter);
        }

        /// <summary>
        /// Delete database record by parameter of user.
        /// </summary>
        /// <param name="userID"></param>
        protected static void DeleteWrkTableRecs(Guid userID)
        {
            PXDatabase.Delete<LUMStdBomCost>(new PXDataFieldRestrict<LUMStdBomCost.createdByID>(userID));
        }

        /// <summary>
        /// If the table data is empty, the database record is inserted according to each user.
        /// </summary>
        /// <param name="graph"></param>
        protected static void InsertWrkTableRecs(PXGraph graph)
        {
            string screenIDWODot = graph.Accessinfo.ScreenID.ToString().Replace(".", "");

            PXDatabase.Insert<LUMStdBomCost>(new PXDataFieldAssign<LUMStdBomCost.createdByID>(graph.Accessinfo.UserID),
                                             new PXDataFieldAssign<LUMStdBomCost.createdByScreenID>(screenIDWODot),
                                             new PXDataFieldAssign<LUMStdBomCost.createdDateTime>(graph.Accessinfo.BusinessDate),
                                             new PXDataFieldAssign<LUMStdBomCost.lastModifiedByID>(graph.Accessinfo.UserID),
                                             new PXDataFieldAssign<LUMStdBomCost.lastModifiedByScreenID>(screenIDWODot),
                                             new PXDataFieldAssign<LUMStdBomCost.lastModifiedDateTime>(graph.Accessinfo.BusinessDate),
                                             new PXDataFieldAssign<LUMStdBomCost.noteID>(Guid.NewGuid()) );
        }
        #endregion

        #region Methods
        public virtual void LoadAllData(AMMultiLevelBomFilter bomFilter)
        {
            DeleteWrkTableRecs(this.Accessinfo.UserID);

            Filter.Current = bomFilter;

            var multiLevelBomRecs = new List<LUMStdBomCost>();

            PXSelectBase<AMBomItem> cmdBOM = new PXSelect<AMBomItem>(this);

            if (bomFilter.BOMDate != null)
            {
                cmdBOM.WhereAnd<Where<Current<AMMultiLevelBomFilter.bOMDate>, Between<AMBomItem.effStartDate, AMBomItem.effEndDate>,
                                        Or<Where<AMBomItem.effStartDate, LessEqual<Current<AMMultiLevelBomFilter.bOMDate>>,
                                                And<AMBomItem.effEndDate, IsNull>>>>>();
            }

            if (bomFilter.BOMID != null)
            {
                cmdBOM.WhereAnd<Where<AMBomItem.bOMID, Equal<Current<AMMultiLevelBomFilter.bOMID>>>>();
            }

            if (bomFilter.InventoryID != null)
            {
                cmdBOM.WhereAnd<Where<AMBomItem.inventoryID, Equal<Current<AMMultiLevelBomFilter.inventoryID>>>>();
            }

            foreach (AMBomItem bomitem in cmdBOM.Select())
            {
                LoadDataRecords(bomitem.BOMID, bomitem.RevisionID, 0, 1, bomitem, multiLevelBomRecs, bomFilter);
            }

            if (bomFilter.RollCosts.GetValueOrDefault())
            {
                multiLevelBomRecs = RollCostUpdate(multiLevelBomRecs);
            }

            for (int i = 0; i < multiLevelBomRecs.Count; i++)
            {
                BOMCost.Insert(multiLevelBomRecs[i]);
            }

            this.Actions.PressSave();
        }

        public virtual void LoadDataRecords(string levelBomid, string levelRevisionID, int? level, decimal? totalQtyReq, AMBomItem parentBomItem, List<LUMStdBomCost> multiLevelBomRecs, AMMultiLevelBomFilter bomFilter)
        {
            if (level == null || level > LowLevel.MaxLowLevel || string.IsNullOrWhiteSpace(levelBomid) || bomFilter == null) { return; }

            // We need a Header for each new level to add the costs from the Cost Roll and to Insert the record as material
            var headerRow = CreateHeaderRow(parentBomItem, multiLevelBomRecs.Count + 1, level.GetValueOrDefault(), totalQtyReq);
            
            if (headerRow == null) { return; }

            // Simulate report [AM614000] grouping conditions.
            if (multiLevelBomRecs.Exists(rec => rec.ManufBOMID == parentBomItem.BOMID && rec.ManufRevisionID == parentBomItem.RevisionID) == false && 
                parentBomItem.Status == AMBomStatus.Active)
            {
                multiLevelBomRecs.Add(headerRow);
            }

            var bomOpersWithoutMatl = new List<AMBomOper>();
            var includeOpersWithoutMatl = false;
            var includeOperations = bomFilter?.IncludeOperations == true;
            var lastOperationCD = string.Empty;
            if (includeOperations)
            {
                bomOpersWithoutMatl = GetOperationsWithoutMaterial(levelBomid, levelRevisionID).ToList();
            }
            includeOpersWithoutMatl = bomOpersWithoutMatl.Count > 0;

            foreach (PXResult<AMBomMatl, AMBomItem, AMBomOper, InventoryItem, INItemCost> result in PXSelectJoin<AMBomMatl, InnerJoin<AMBomItem, On<AMBomMatl.bOMID, Equal<AMBomItem.bOMID>,
                                                                                                                                      And<AMBomMatl.revisionID, Equal<AMBomItem.revisionID>>>,
                                                                                                                            InnerJoin<AMBomOper, On<AMBomMatl.bOMID, Equal<AMBomOper.bOMID>,
                                                                                                                                                    And<AMBomMatl.revisionID, Equal<AMBomOper.revisionID>,
                                                                                                                                                        And<AMBomMatl.operationID, Equal<AMBomOper.operationID>>>>,
                                                                                                                            InnerJoin<InventoryItem, On<AMBomMatl.inventoryID, Equal<InventoryItem.inventoryID>>,
                                                                                                                            LeftJoin<INItemCost, On<AMBomMatl.inventoryID, Equal<INItemCost.inventoryID>>>>>>,
                                                                                                                            Where<AMBomMatl.bOMID, Equal<Required<AMBomMatl.bOMID>>,
                                                                                                                                  And<AMBomMatl.revisionID, Equal<Required<AMBomMatl.revisionID>>,
                                                                                                                                      And2<Where<AMBomMatl.effDate, IsNull,
                                                                                                                                           Or<AMBomMatl.effDate, LessEqual<Current<AMMultiLevelBomFilter.bOMDate>>>>,
                                                                                                                                      And<Where<AMBomMatl.expDate, IsNull,
                                                                                                                                          Or<AMBomMatl.expDate, GreaterEqual<Current<AMMultiLevelBomFilter.bOMDate>>>>>>>>,
                                                                                                                            OrderBy<Asc<AMBomOper.operationCD,
                                                                                                                                        Asc<AMBomMatl.sortOrder,
                                                                                                                                            Asc<AMBomMatl.lineID>>>>>.Select(this, levelBomid, levelRevisionID))
            {
                var amBomMatl = (AMBomMatl)result;
                var amBomItem = (AMBomItem)result;
                var amBomOper = (AMBomOper)result;
                var invItem   = (InventoryItem)result;
                var itemCost  = (INItemCost)result;

                if (ExcludeMaterial(amBomMatl, invItem, amBomItem, amBomOper, bomFilter?.BOMDate ?? Accessinfo.BusinessDate.GetValueOrDefault()))
                {
                    continue;
                }

                var row = CreateDetailRow(amBomMatl, amBomOper, amBomItem, invItem, parentBomItem, multiLevelBomRecs.Count + 1, level.GetValueOrDefault(), 
                                          totalQtyReq.GetValueOrDefault(), bomFilter, levelBomid, levelRevisionID);
                
                if (row == null)
                {
                    continue;
                }

                if (itemCost != null && bomFilter.UseCurrentInventoryCost.GetValueOrDefault())
                {
                    row.UnitCost = BOMCostRoll.GetUnitCostFromINItemCostTable(itemCost) ?? amBomMatl.UnitCost.GetValueOrDefault();
                }

                row.CompExtCost = row.CompQtyReq * row.UnitCost;
                row.CompTotalExtCost = row.TotalQtyReq * row.UnitCost;

                if (includeOperations && !lastOperationCD.Equals(row.OperationCD))
                {
                    if (includeOpersWithoutMatl)
                    {
                        var indexes2Remove = new List<int>();
                        for (int i = 0; i < bomOpersWithoutMatl.Count; i++)
                        {
                            var op = bomOpersWithoutMatl[i];
                            if (OperationHelper.LessThan(op.OperationCD, row.OperationCD))
                            {
                                indexes2Remove.Add(i);
                                var operBomData = CreateOperationRow(op, parentBomItem, multiLevelBomRecs.Count + 1, level, 0);
                                multiLevelBomRecs.Add(operBomData);
                            }
                        }

                        foreach (var idx in indexes2Remove.OrderByDescending(x => x))
                        {
                            bomOpersWithoutMatl.RemoveAt(idx);
                        }
                    }

                    // include current operation as an entry
                    multiLevelBomRecs.Add(CreateOperationRow(amBomOper, parentBomItem, multiLevelBomRecs.Count + 1, level, 0));
                }

                row.LineID = multiLevelBomRecs.Count + 1;
                multiLevelBomRecs.Add(row);

                if (!string.IsNullOrWhiteSpace(row.ManufBOMID) &&
                    !string.IsNullOrWhiteSpace(row.ManufRevisionID))
                {
                    LoadDataRecords(row.ManufBOMID, row.ManufRevisionID, level + 1, row.TotalQtyReq.GetValueOrDefault(), parentBomItem, multiLevelBomRecs, bomFilter);
                }

                lastOperationCD = row.OperationCD;
            }

            if (includeOpersWithoutMatl)
            {
                foreach (var op in bomOpersWithoutMatl)
                {
                    var operBomData = CreateOperationRow(op, parentBomItem, multiLevelBomRecs.Count + 1, level, 0);
                    multiLevelBomRecs.Add(operBomData);
                }
            }
        }

        public virtual List<LUMStdBomCost> RollCostUpdate(List<LUMStdBomCost> multiLevelBomRecs)
        {
            var newMultiLevelBomRecs = new List<LUMStdBomCost>();
            var rollBomList = new RollBomList();
            var uniqueBoms = new HashSet<string>();
            AMBomItem bomItem;

            foreach (var multiLevelRecord in multiLevelBomRecs)
            {
                var bomKey = string.Join(":", multiLevelRecord.LineBOMID, multiLevelRecord.LineRevisionID);

                if (!uniqueBoms.Add(bomKey) || multiLevelRecord.IsHeaderRecord == true || multiLevelRecord.InventoryID == null)
                {
                    //Repeat bom/rev
                    continue;
                }

                bomItem = (AMBomItem)this.Caches<AMBomItem>().Locate(new AMBomItem { BOMID = multiLevelRecord.LineBOMID, RevisionID = multiLevelRecord.LineRevisionID });
                if (bomItem == null)
                {
                    bomItem = PXSelect<AMBomItem, Where<AMBomItem.bOMID, Equal<Required<AMBomItem.bOMID>>,
                                                        And<AMBomItem.revisionID, Equal<Required<AMBomItem.revisionID>>>>>.Select(this, multiLevelRecord.LineBOMID, multiLevelRecord.LineRevisionID);
                }

                rollBomList.Add(bomItem, multiLevelRecord.Level.GetValueOrDefault(), false);
            }

            var costRollGraph = CreateInstance<LumCostRoll>();
            var costRollFilter = new RollupSettings
            {
                SnglMlti = "M",
                SiteID = null,
                InventoryID = Filter.Current.InventoryID,
                SubItemID = null,
                BOMID = Filter.Current.BOMID,
                RevisionID = Filter.Current.RevisionID,
                EffectiveDate = Filter.Current.BOMDate,
                IncMatScrp = true,
                IncFixed = true,
                UpdateMaterial = false,
                UsePending = false,
                IgnoreMinMaxLotSizeValues = Filter.Current.IgnoreMinMaxLotSizeValues
            };

            costRollGraph.Settings.Current = costRollFilter;
            costRollGraph.RollCosts(rollBomList, Filter.Current.GetExtension<AMMultiLevelBomFilterExt>().UsrEnblItemRoundUp.GetValueOrDefault());

            foreach (var mutliLevelRecord in multiLevelBomRecs)
            {
                AMBomCost bomCostRec = null;

                if (!string.IsNullOrWhiteSpace(mutliLevelRecord.ManufBOMID) && !string.IsNullOrWhiteSpace(mutliLevelRecord.ManufRevisionID))
                {
                    bomCostRec = (AMBomCost)costRollGraph.Caches<AMBomCost>().Locate(new AMBomCost
                    {
                        BOMID = mutliLevelRecord.ManufBOMID,
                        RevisionID = mutliLevelRecord.ManufRevisionID,
                        UserID = costRollGraph.Accessinfo.UserID
                    });
                }

                var row = RollCostUpdate(mutliLevelRecord, bomCostRec);

                if (row == null)
                {
                    continue;
                }

                newMultiLevelBomRecs.Add(row);
            }
            return newMultiLevelBomRecs;
        }

        public virtual LUMStdBomCost RollCostUpdate(LUMStdBomCost multiLevelRecord, AMBomCost bomCostRec)
        {
            if (multiLevelRecord == null)
            {
                return null;
            }

            var row = multiLevelRecord.Copy<LUMStdBomCost>();

            row.HasCostRoll = false;

            if (bomCostRec?.BOMID == null)
            {
                return row;
            }

            row.HasCostRoll = true;
            row.LotSize = bomCostRec.LotSize;
            //row.FixedLaborTime = bomCostRec.FixedLaborTime;
            //row.VariableLaborTime = (int?)(bomCostRec.VariableLaborTime * multiLevelRecord.TotalQtyReq);
            //row.MachineTime = (int?)(bomCostRec.MachineTime * multiLevelRecord.TotalQtyReq);
            row.UnitCost = bomCostRec.UnitCost;
            row.ToolCost = bomCostRec.ToolCost * multiLevelRecord.TotalQtyReq;
            row.CompMatlCost = bomCostRec.MatlManufacturedCost * multiLevelRecord.TotalQtyReq;
            row.CompPurchMatl = bomCostRec.MatlNonManufacturedCost * multiLevelRecord.TotalQtyReq;
            row.FixedLaborCost = bomCostRec.FLaborCost;
            row.VariableLaborCost = bomCostRec.VLaborCost * multiLevelRecord.TotalQtyReq;
            row.FixedOvdCost = bomCostRec.FOvdCost;
            row.VariableOvdCost = bomCostRec.VOvdCost * multiLevelRecord.TotalQtyReq;
            row.MachineCost = bomCostRec.MachCost * multiLevelRecord.TotalQtyReq;
            row.CompExtCost = bomCostRec.UnitCost * multiLevelRecord.CompQtyReq;
            row.CompTotalExtCost = bomCostRec.UnitCost * multiLevelRecord.TotalQtyReq;
            row.TotalCost = bomCostRec.UnitCost * row.LotSize; // For MLB Report Header only
            row.SubcontractCost = bomCostRec.SubcontractMaterialCost;

            return row;
        }

        public virtual LUMStdBomCost CreateHeaderRow(AMBomItem parentBomItem, int lineID, int level, decimal? totalQtyReq)
        {
            return new LUMStdBomCost
            {
                BOMID = parentBomItem.BOMID,
                RevisionID = parentBomItem.RevisionID,
                LineID = lineID,
                Level = level,
                InventoryID = parentBomItem.InventoryID,
                SubItemID = parentBomItem.SubItemID,
                //ParentDescr = parentBomItem.Descr,
                EffStartDate = parentBomItem.EffStartDate,
                EffEndDate = parentBomItem.EffEndDate,
                SiteID = parentBomItem.SiteID,
                ManufBOMID = parentBomItem.BOMID,
                ManufRevisionID = parentBomItem.RevisionID,
                TotalQtyReq = totalQtyReq,
                IsHeaderRecord = true
            };
        }

        public virtual LUMStdBomCost CreateDetailRow(AMBomMatl amBomMatl, AMBomOper amBomOper, AMBomItem amBomItem, InventoryItem inventoryItem, AMBomItem parentBomItem, 
                                                     int lineID, int level, decimal totalQtyReq, AMMultiLevelBomFilter filter, string levelBomid, string levelRevisionID)
        {
            var itemExt = inventoryItem.GetExtension<JAMS.AEF.InventoryItemAMExtension>();

            var qtyRequired = amBomMatl.QtyReq.GetValueOrDefault() * (1 + amBomMatl.ScrapFactor.GetValueOrDefault()) *
                                  (amBomMatl.BatchSize.GetValueOrDefault() == 0m ? 1m : 1 / amBomMatl.BatchSize.GetValueOrDefault());

            var totalQtyRequired = amBomMatl.QtyReq.GetValueOrDefault() * (1 + amBomMatl.ScrapFactor.GetValueOrDefault()) *
                                      (amBomMatl.BatchSize.GetValueOrDefault() == 0m ? 1m : totalQtyReq / amBomMatl.BatchSize.GetValueOrDefault());

            var baseTotalQtyRequired = amBomMatl.BaseQty.GetValueOrDefault() * (1 + amBomMatl.ScrapFactor.GetValueOrDefault()) *
                                           (amBomMatl.BatchSize.GetValueOrDefault() == 0m ? 1m : totalQtyReq / amBomMatl.BatchSize.GetValueOrDefault());

            AMMultiLevelBomFilterExt filterExt = filter.GetExtension<AMMultiLevelBomFilterExt>();

            if (filterExt.UsrEnblItemRoundUp == true)
            {
                qtyRequired = itemExt.AMQtyRoundUp == false ? qtyRequired : Math.Ceiling(qtyRequired);
                totalQtyRequired = itemExt.AMQtyRoundUp == false ? totalQtyRequired : Math.Ceiling(totalQtyRequired);
                baseTotalQtyRequired = itemExt.AMQtyRoundUp == false ? baseTotalQtyRequired : Math.Ceiling(baseTotalQtyRequired);
            }

            var row = new LUMStdBomCost
            {
                BOMID = parentBomItem.BOMID,
                RevisionID = parentBomItem.RevisionID,
                LineID = lineID,
                Level = level,
                CompInventoryID = amBomMatl.InventoryID,
                //Descr = amBomMatl.Descr,
                InventoryID = parentBomItem.InventoryID,
                SubItemID = parentBomItem.SubItemID,
                //ParentDescr = parentBomItem.Descr,
                CompSubItemID = amBomMatl.SubItemID,
                UOM = amBomMatl.UOM,
                ScrapFactor = amBomMatl.ScrapFactor.GetValueOrDefault(),
                //BatchSize = amBomMatl.BatchSize.GetValueOrDefault(),
                //BOMQtyReq = amBomMatl.QtyReq.GetValueOrDefault(),
                //BaseQtyReq = amBomMatl.BaseQty.GetValueOrDefault(),
                CompQtyReq = qtyRequired,
                TotalQtyReq = totalQtyRequired,
                //BaseTotalQtyReq = baseTotalQtyRequired,
                CompUnitCost = amBomMatl.UnitCost.GetValueOrDefault(),
                CompExtCost = qtyRequired * amBomMatl.UnitCost.GetValueOrDefault(),
                CompTotalExtCost = totalQtyRequired * amBomMatl.UnitCost.GetValueOrDefault(),
                LineBOMID = amBomMatl.BOMID,
                LineRevisionID = amBomMatl.RevisionID,
                OperationID = amBomOper.OperationID,
                OperationCD = amBomOper.OperationCD,
                EffStartDate = parentBomItem.EffStartDate,
                EffEndDate = parentBomItem.EffEndDate,
                SiteID = parentBomItem.SiteID,
                //Status = parentBomItem.Status,
                //LineStatus = amBomItem.Status,
                //MaterialStatus = amBomItem.Status,
                //OperationDescription = amBomOper.Descr,
                WcID = amBomOper.WcID,
                IsHeaderRecord = false,
                SortOrder = amBomMatl.SortOrder
            };

            var materialSiteID = amBomMatl.SiteID ?? amBomItem.SiteID;

            if (filter.IgnoreReplenishmentSettings.GetValueOrDefault()
                || JAMS.AM.InventoryHelper.GetReplenishmentSource(this, row.CompInventoryID, materialSiteID) == INReplenishmentSource.Manufactured)
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
                                  ? PrimaryBomIDManager.GetActiveRevisionBomItemByDate(this, new PrimaryBomIDManager(this).GetPrimaryAllLevels(row.CompInventoryID,
                                                                                       materialSiteID, row.SubItemID), filter.BOMDate.GetValueOrDefault()) 
                                  : PrimaryBomIDManager.GetNotArchivedRevisionBomItemByDate(this, new PrimaryBomIDManager(this).GetPrimaryAllLevels(row.CompInventoryID,
                                                                                            materialSiteID, row.SubItemID), filter.BOMDate.GetValueOrDefault());

                    if (bomItem == null)
                    {
                        PXTrace.WriteWarning(JAMS.AM.Messages.GetLocal(JAMS.AM.Messages.NoActiveRevisionForBom, parentBomItem.BOMID));
                        return row;
                    }

                    levelBomid = bomItem.BOMID;
                    levelRevisionID = bomItem.RevisionID;
                }

                row.ManufBOMID = levelBomid;
                row.ManufRevisionID = levelRevisionID;
            }

            return row;
        }

        public virtual LUMStdBomCost CreateOperationRow(AMBomOper amBomOper, AMBomItem parentBomItem, int lineID, int? level, decimal totalQtyReq)
        {
            return new LUMStdBomCost
            {
                BOMID = parentBomItem.BOMID,
                RevisionID = parentBomItem.RevisionID,
                LineBOMID = amBomOper.BOMID,
                LineRevisionID = amBomOper.RevisionID,
                LineID = lineID,
                Level = level.GetValueOrDefault(),
                InventoryID = parentBomItem.InventoryID,
                SubItemID = parentBomItem.SubItemID,
                //ParentDescr = parentBomItem.Descr,
                EffStartDate = parentBomItem.EffStartDate,
                EffEndDate = parentBomItem.EffEndDate,
                SiteID = parentBomItem.SiteID,
                //Status = parentBomItem.Status,
                ManufBOMID = parentBomItem.BOMID,
                ManufRevisionID = parentBomItem.RevisionID,
                TotalQtyReq = totalQtyReq,
                IsHeaderRecord = false,
                OperationID = amBomOper.OperationID,
                OperationCD = amBomOper.OperationCD,
                //OperationDescription = amBomOper.Descr,
                WcID = amBomOper.WcID
            };
        }

        public virtual IEnumerable<AMBomOper> GetOperationsWithoutMaterial(string bomId, string revisionId)
        {
            foreach (AMBomOper bomOper in PXSelectReadonly2<AMBomOper, LeftJoin<AMBomMatl, On<AMBomOper.bOMID, Equal<AMBomMatl.bOMID>,
                                                                                And<AMBomOper.revisionID, Equal<AMBomMatl.revisionID>,
                                                                                    And<AMBomOper.operationID, Equal<AMBomMatl.operationID>>>>>,
                                                                       Where<AMBomOper.bOMID, Equal<Required<AMBomOper.bOMID>>,
                                                                             And<AMBomOper.revisionID, Equal<Required<AMBomOper.revisionID>>,
                                                                                 And<AMBomMatl.inventoryID, IsNull>>>>.Select(this, bomId, revisionId))
            {
                if (bomOper?.OperationID == null)
                {
                    continue;
                }

                yield return bomOper;
            }
        }

        public virtual bool ExcludeMaterial(AMBomMatl bomMatl, InventoryItem inventoryItem, AMBomItem bomItem, AMBomOper bomOper, DateTime parmDate)
        {
            if (bomMatl?.InventoryID == null || inventoryItem?.InventoryID == null || bomItem?.InventoryID == null)
            {
                return true;
            }

            if (bomMatl.ExpDate != null && bomMatl.ExpDate.GetValueOrDefault() <= parmDate)
            {
                return true;
            }

            if (bomMatl.EffDate != null && bomMatl.EffDate.GetValueOrDefault() > parmDate)
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}