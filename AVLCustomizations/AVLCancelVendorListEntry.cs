using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.CS;
using PX.Objects.IN;

namespace AVLCustomizations
{
    public class AVLCancelVendorListEntry : PXGraph<AVLCancelVendorListEntry, AVLTable>
    {
        private const string currentPage = "Cancel";
        public class constCurrentPage : PX.Data.BQL.BqlString.Constant<constCurrentPage>
        {
            public constCurrentPage() : base(currentPage) { }
        }

        private const string statusIsApproved = "2";
        public class constStatusApproved : PX.Data.BQL.BqlString.Constant<constStatusApproved>
        {
            public constStatusApproved() : base(statusIsApproved) { }
        }


        public PXSetup<APSetup> apSetup;

        /* Declaration of the data view */
        public SelectFrom<AVLTable>.View AVLTableView;

        public PXFilter<ApprVendListsFilter> Filter;
        public SelectFrom<ApprovedVendLists>.Where<ApprovedVendLists.avlnbr.IsEqual<ApprVendListsFilter.avlnbr.FromCurrent>.
                                               And<ApprovedVendLists.aVLStatus.IsEqual<constStatusApproved>>>.View ApprovedVendListsDialogView;
        
        public SelectFrom<AVLLine>.
                 LeftJoin<ApprovedVendLists>.On<AVLLine.avlnbr.IsEqual<ApprovedVendLists.avlnbr>.
                      And<AVLLine.lineNbr.IsEqual<ApprovedVendLists.lineNbr>>>.
                 Where<ApprovedVendLists.selected.IsEqual<True>>.View AVLLineSelectorView;

        public SelectFrom<AVLLine>.Where<AVLLine.avlnbr.IsEqual<AVLTable.avlnbr.FromCurrent>>.View AVLLineView;

        public SelectFrom<AVLEvent>.Where<AVLEvent.avlnbr.IsEqual<AVLTable.avlnbr.FromCurrent>.
                                      And<AVLEvent.action.IsEqual<constCurrentPage>>>.
                                    OrderBy<Desc<AVLEvent.eventID>>.View EventHistoryView;

        /* Override SetNumberingId<> by event */
        public void _(Events.RowPersisting<AVLTable> eAVLTable)
        {
            AutoNumberAttribute.SetNumberingId<AVLTable.avlnbr>(eAVLTable.Cache, "AVL"); //AVL is the numbering sequence setting in system
        }

        public void Initialize()
        {
            ApproveAction.SetEnabled(false);
            RejectAction.SetEnabled(false);
        }

        #region Custom variables

        #region AVLTable Status
        Dictionary<string, string> dicAVLTableStatus = new Dictionary<string, string>()
        {
            { "OnHold", "0" },
            { "Submitted", "1" },
            { "Approved", "2" },
            { "Cancelled", "3" }
        };
        #endregion

        #endregion

        #region Custom Insert/Update Table Functions

        #region Insert AVLEvet Table
        private void insertAVLEvent(string aVLEventAction)
        {
            var curAVLTableCache = (AVLTable)this.Caches[typeof(AVLTable)].Current;

            var maint = PXGraph.CreateInstance<AVLCreateVendorListEntry>();
            maint.ProviderInsert<AVLEvent>(
                new PXDataFieldAssign("AVLNbr", curAVLTableCache.Avlnbr),
                //new PXDataFieldAssign("CompanyID", PX.Data.Update.PXInstanceHelper.CurrentCompany),
                new PXDataFieldAssign("Type", curAVLTableCache.AVLStatus),
                new PXDataFieldAssign("Action", aVLEventAction),
                new PXDataFieldAssign("CreatedByID", curAVLTableCache.CreatedByID),
                new PXDataFieldAssign("CreatedByScreenID", PXSiteMap.CurrentScreenID),
                new PXDataFieldAssign("CreatedDateTime", curAVLTableCache.CreatedDateTime),
                new PXDataFieldAssign("LastModifiedByID", curAVLTableCache.LastModifiedByID),
                new PXDataFieldAssign("LastModifiedByScreenID", PXSiteMap.CurrentScreenID),
                new PXDataFieldAssign("LastModifiedDateTime", curAVLTableCache.LastModifiedDateTime)
            );
        }
        #endregion

        #region Insert ApprovedVendLists Table
        private void insertApprovedVendLists(AVLTable curAVLTableCache, AVLLine curAVLLineCache)
        {
            if (curAVLTableCache == null || curAVLLineCache == null) return;

            var maint = PXGraph.CreateInstance<AVLCreateVendorListEntry>();
            maint.ProviderInsert<ApprovedVendLists>(
                new PXDataFieldAssign("AVLNbr", curAVLTableCache.Avlnbr),
                new PXDataFieldAssign("LineNbr", curAVLLineCache.LineNbr),
                new PXDataFieldAssign("AVLStatus", curAVLTableCache.AVLStatus), //Approved
                new PXDataFieldAssign("AVLDate", curAVLTableCache.Avldate),
                new PXDataFieldAssign("Descripton", curAVLTableCache.Descripton),
                new PXDataFieldAssign("VendorID", curAVLLineCache.VendorID),
                new PXDataFieldAssign("VendorName", curAVLLineCache.VendorName),
                new PXDataFieldAssign("InventoryID", curAVLLineCache.InventoryID),
                new PXDataFieldAssign("InventoryDesc", curAVLLineCache.InventoryDesc),
                new PXDataFieldAssign("Remark", curAVLLineCache.Remark),
                new PXDataFieldAssign("ApprovedByID", PXAccess.GetUserID()),
                new PXDataFieldAssign("ApprovedDateTime", DateTime.Now),
                new PXDataFieldAssign("CreatedByID", curAVLTableCache.CreatedByID),
                new PXDataFieldAssign("CreatedByScreenID", PXSiteMap.CurrentScreenID),
                new PXDataFieldAssign("CreatedDateTime", curAVLTableCache.CreatedDateTime),
                new PXDataFieldAssign("LastModifiedByID", curAVLTableCache.LastModifiedByID),
                new PXDataFieldAssign("LastModifiedByScreenID", PXSiteMap.CurrentScreenID),
                new PXDataFieldAssign("LastModifiedDateTime", curAVLTableCache.LastModifiedDateTime)
            );
        }
        #endregion

        #region Update ApprovedVendLists Table
        private void updateApprovedVendLists(AVLTable curAVLTableCache, AVLLine curAVLLineCache)
        {
            if (curAVLTableCache == null || curAVLLineCache == null) return;

            var maint = PXGraph.CreateInstance<AVLCreateVendorListEntry>();
            maint.ProviderUpdate<ApprovedVendLists>(
                new PXDataFieldAssign("AVLStatus", curAVLTableCache.AVLStatus),
                new PXDataFieldAssign("AVLDate", curAVLTableCache.Avldate),
                new PXDataFieldAssign("Descripton", curAVLTableCache.Descripton),
                //new PXDataFieldAssign("VendorID", curAVLLineCache.VendorID),
                //new PXDataFieldAssign("VendorName", curAVLLineCache.VendorName),
                //new PXDataFieldAssign("InventoryID", curAVLLineCache.InventoryID),
                //new PXDataFieldAssign("InventoryDesc", curAVLLineCache.InventoryDesc),
                //new PXDataFieldAssign("Remark", curAVLLineCache.Remark),
                new PXDataFieldAssign("ApprovedByID", PXAccess.GetUserID()),
                new PXDataFieldAssign("ApprovedDateTime", DateTime.Now),
                //new PXDataFieldAssign("CreatedByID", curAVLTableCache.CreatedByID),
                //new PXDataFieldAssign("CreatedDateTime", curAVLTableCache.CreatedDateTime),
                new PXDataFieldAssign("LastModifiedByID", curAVLTableCache.LastModifiedByID),
                new PXDataFieldAssign("LastModifiedDateTime", curAVLTableCache.LastModifiedDateTime),
                new PXDataFieldRestrict("AVLNbr", curAVLLineCache.OrigAVLNbr),
                new PXDataFieldRestrict("LineNbr", curAVLLineCache.LineNbr)
            );
        }
        #endregion

        #endregion

        #region  Actions

        #region Submit Action
        public PXAction<AVLTable> SubmitAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Submit", Enabled = false)]
        protected void submitAction()
        {
            
            if ((AVLLine)this.Caches[typeof(AVLLine)].Current == null)
            {
                throw new PXException($"Please add a row in DOCUMENT DEATIAL at least.");
            }


            this.Save.Press();
            var curAVLTableCache = (AVLTable)this.Caches[typeof(AVLTable)].Current;
            string url = $"{PX.Common.PXUrl.SiteUrlWithPath()}/Main?ScreenId={PXSiteMap.CurrentScreenID}&{nameof(AVLTable.avlnbr)}={curAVLTableCache.Avlnbr}";
            
            doSubmit();
            PX.Data.Redirector.RefreshPage(System.Web.HttpContext.Current, url);

            #region backup solution
            //else
            //{
            //    var curAVLTableCache = (AVLTable)this.Caches[typeof(AVLTable)].Current;

            //    if (curAVLTableCache.Avlnbr == PX.Objects.CR.Messages.New)//(curAVLTableCache.Avlnbr.Contains("<") && curAVLTableCache.Avlnbr.Contains(">"))
            //        throw new PXException($"Please save the record first.");
            //    else
            //    {
            //        var avlnbrChecker = SelectFrom<AVLTable>.
            //                             Where<AVLTable.avlnbr.IsEqual<@P.AsString>>.View.
            //                             Select(this, curAVLTableCache.Avlnbr);

            //    if (avlnbrChecker.Count > 0)
            //        doSubmit();
            //    else
            //        throw new PXException($"Please save the record first.");
            //}
            //}
            #endregion
        }
        #endregion

        #region
        protected void doSubmit()
        {
            var curAVLTableCache = (AVLTable)this.Caches[typeof(AVLTable)].Current;
            if (curAVLTableCache.AVLStatus.Trim().Equals(dicAVLTableStatus["OnHold"]))
            {
                curAVLTableCache.AVLStatus = dicAVLTableStatus["Submitted"];
                AVLTableView.UpdateCurrent(); // reload the AVLTable Form
                Actions.PressSave();
                insertAVLEvent(currentPage);

                //Layout
                buttonEnable(curAVLTableCache.AVLStatus);
            }
        }
        #endregion

        #region Approve Action
        public PXAction<AVLTable> ApproveAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Approve", Enabled = false)]
        protected void approveAction()
        {
            var currentAVLTableCache = (AVLTable)this.Caches[typeof(AVLTable)].Current;
            if (currentAVLTableCache.AVLStatus.Trim().Equals(dicAVLTableStatus["Submitted"]))
            {
                currentAVLTableCache.AVLStatus = dicAVLTableStatus["Cancelled"];
                AVLTableView.UpdateCurrent(); // reload the AVLTable Form
                Actions.PressSave();
                insertAVLEvent(currentPage);

                foreach (AVLLine curAVLLineRow in this.AVLLineView.Select(currentAVLTableCache.Avlnbr))
                {
                    var curRecord = PXSelect<ApprovedVendLists, Where<ApprovedVendLists.avlnbr, Equal<Required<ApprovedVendLists.avlnbr>>,
                                                                And<ApprovedVendLists.lineNbr, Equal<Required<ApprovedVendLists.lineNbr>>>>>
                                                                .Select(this, curAVLLineRow.Avlnbr, curAVLLineRow.LineNbr);

                    if (curRecord.Count == 0)
                        insertApprovedVendLists(currentAVLTableCache, curAVLLineRow);
                    else
                        updateApprovedVendLists(currentAVLTableCache, curAVLLineRow);
                }

                //Layout
                buttonEnable(currentAVLTableCache.AVLStatus);
            }
            else return;
        }
        #endregion

        #region Reject Action
        public PXAction<AVLTable> RejectAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Reject", Enabled = false)]
        protected void rejectAction()
        {
            var currentCache = (AVLTable)this.Caches[typeof(AVLTable)].Current;
            if (currentCache.AVLStatus.Trim().Equals(dicAVLTableStatus["Submitted"]))
            {
                currentCache.AVLStatus = dicAVLTableStatus["OnHold"];
                AVLTableView.UpdateCurrent(); // reload the AVLTable Form
                Actions.PressSave();
                insertAVLEvent(currentPage);

                //Layout
                buttonEnable(currentCache.AVLStatus);
            }
            else return;
        }
        #endregion
        
        #region ADD button
        public PXAction<ApprovedVendLists> Add;
        [PXButton(CommitChanges = true)]
        protected void add()
        {
            foreach (ApprovedVendLists aVLrow in ApprovedVendListsDialogView.Cache.Updated)
            {
                if (!isDupilcateInAVLLineCache(aVLrow))
                {
                    var avllineCache = AVLLineView.Cache.CreateInstance() as AVLLine;
                    avllineCache.OrigAVLNbr = aVLrow.Avlnbr;
                    avllineCache.VendorID = aVLrow.VendorID;
                    avllineCache.VendorName = aVLrow.VendorName;
                    avllineCache.InventoryID = aVLrow.InventoryID;
                    avllineCache.InventoryDesc = aVLrow.InventoryDesc;
                    avllineCache.Remark = aVLrow.Remark;

                    AVLLineView.Cache.Insert(avllineCache);
                }
            }
        }
        #endregion

        #endregion


        #region check dupilcate AVLLine record
        private bool isDupilcateInAVLLineCache(ApprovedVendLists aVLrow)
        {
            foreach (AVLLine row in AVLLineView.Cache.Cached)
            {
                if (row.OrigAVLNbr.Equals(aVLrow.Avlnbr))
                    return true;
            }
            return false;
        }
        #endregion


        #region Event Handlers

        protected void AVLTable_AVLAction_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
        {
            var row = (AVLTable)e.Row;
            row.AVLAction = "Cancel";
        }

        protected void AVLLine_VendorID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            if (e.Row == null) return;

            var row = (AVLLine)e.Row;
            var vendor = PXSelect<Vendor, Where<Vendor.bAccountID, Equal<Required<Vendor.bAccountID>>>>
                                          .Select(this, row.VendorID);
            row.VendorName = vendor.TopFirst.AcctName;
        }

        protected void AVLLine_InventoryID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            if (e.Row == null) return;

            var row = (AVLLine)e.Row;
            var inventoryItem = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
                                                        .Select(this, row.InventoryID);
            row.InventoryDesc = inventoryItem.TopFirst.Descr;
        }

        protected void ApprovedVendLists_Avlnbr_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            if (e.Row == null) return;

            var row = (ApprovedVendLists)e.Row;
            
        }

        #endregion

        #region Layout Handlers

        #region Base Table Function Control
        protected void _(Events.RowSelected<AVLTable> e)
        {
            //ApprovedVendListsSelectorView.AllowInsert = ApprovedVendListsSelectorView.AllowUpdate = ApprovedVendListsSelectorView.AllowDelete = false;

            if (e.Row.AVLStatus.Trim().Equals(dicAVLTableStatus["OnHold"]))
            {
                AVLTableView.AllowInsert = AVLTableView.AllowUpdate = AVLTableView.AllowDelete =
                   AVLLineView.AllowInsert = AVLLineView.AllowUpdate = AVLLineView.AllowDelete = true;
            }
            else
            {
                AVLTableView.AllowInsert = AVLTableView.AllowUpdate = AVLTableView.AllowDelete =
                   AVLLineView.AllowInsert = AVLLineView.AllowUpdate = AVLLineView.AllowDelete = false;
            }
            buttonEnable(e.Row.AVLStatus.Trim());
        }
        #endregion

        #region Button Control
        protected void buttonEnable(string aVLStatus)
        {
            if (aVLStatus.Trim().Equals(dicAVLTableStatus["OnHold"]))
            {
                SubmitAction.SetEnabled(true);
                ApproveAction.SetEnabled(false);
                RejectAction.SetEnabled(false);
            }
            else if (aVLStatus.Trim().Equals(dicAVLTableStatus["Submitted"]))
            {
                SubmitAction.SetEnabled(false);
                ApproveAction.SetEnabled(true);
                RejectAction.SetEnabled(true);
            }
            else if (aVLStatus.Trim().Equals(dicAVLTableStatus["Approved"]))
            {
                SubmitAction.SetEnabled(false);
                ApproveAction.SetEnabled(false);
                RejectAction.SetEnabled(false);
            }
            else if (aVLStatus.Trim().Equals(dicAVLTableStatus["Cancelled"]))
            {
                SubmitAction.SetEnabled(false);
                ApproveAction.SetEnabled(false);
                RejectAction.SetEnabled(false);
            }
        }
        #endregion

        #endregion
    }

    [Serializable]
    [PXCacheName("Approved Vendor Lists Filter")]
    public class ApprVendListsFilter : IBqlTable
    {
        #region Avlnbr
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "AVL Nbr")]
        [PXSelector(typeof(Search4<ApprovedVendLists.avlnbr, Where<ApprovedVendLists.aVLStatus, Equal<AVLCancelVendorListEntry.constStatusApproved>>, Aggregate<GroupBy<ApprovedVendLists.avlnbr>>>),
                    typeof(ApprovedVendLists.avlnbr))]
        public virtual string Avlnbr { get; set; }
        public abstract class avlnbr : PX.Data.BQL.BqlString.Field<avlnbr> { }
        #endregion
    }
}
