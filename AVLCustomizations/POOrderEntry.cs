using PX.Data;
using AVLCustomizations;
using PX.Objects.IN;
using PX.Objects.AP;

namespace PX.Objects.PO
{
    public class POOrderEntry_Extension : PXGraphExtension<POOrderEntry>
    {
        private string statusIsApproved = "2";
        private string statusIsCancelled = "3";

        #region Event Handlers

        #region Base Table Function Control
        protected void _(Events.RowPersisting<POOrder> e) //try POLine
        {
            var curPOOrder = (POOrder)Base.Caches[typeof(POOrder)].Current;

            if (curPOOrder != null)
            {
                foreach (POLine curPOOrderLineRow in Base.Transactions.Cache.Cached)
                {
                    /**
                     * Rules
                     * <OK>
                     * 1. If inventory id is not existing on approvedvendorlist table, then validation is ok.
                     * 2. If inventory id and vendor can be found, also a field of status is 'approved'. It means ok. 
                     * 
                     * <NOT_OK>
                     * 1. If inventory id can be found, but vendor is different with this row. It cannot be passed.
                     * 2. If inventory id and vendor can be found, also a field of status is 'cancelled'. It means 
                     */

                    //Case I
                    var curAVLChecker_status = PXSelect<ApprovedVendLists,
                                                    Where<ApprovedVendLists.inventoryID, Equal<Required<ApprovedVendLists.inventoryID>>,
                                                    And<ApprovedVendLists.vendorid, Equal<Required<ApprovedVendLists.vendorid>>>>,
                                                    OrderBy<Desc<ApprovedVendLists.avlnbr>>>.
                                                    Select(Base, curPOOrderLineRow.InventoryID, curPOOrder.VendorID);

                    if (curAVLChecker_status.TopFirst != null && curAVLChecker_status.TopFirst.AVLStatus.Trim().Equals(statusIsCancelled))
                    {
                        var errorMsgInventory = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.
                                                           Select(Base, curPOOrderLineRow.InventoryID).TopFirst;

                        var errorMsgVendor = PXSelect<Vendor, Where<Vendor.bAccountID, Equal<Required<Vendor.bAccountID>>>>.
                                                           Select(Base, curPOOrder.VendorID).TopFirst;

                        //var errorMsgInventoryID = curPOOrderLineRow.InventoryID;
                        Base.Transactions.Cache.Delete(curPOOrderLineRow);
                        throw new PXException($"vendor ID {errorMsgVendor.AcctCD.Trim()} - {errorMsgVendor.AcctName.Trim()} is cancelled in approved vendor list for item {errorMsgInventory.InventoryCD.Trim()}.");
                    }


                    var curAVLChecker_inventoryID = PXSelect<ApprovedVendLists,
                                                             Where<ApprovedVendLists.inventoryID, Equal<Required<ApprovedVendLists.inventoryID>>>>.
                                                             Select(Base, curPOOrderLineRow.InventoryID);

                    if (curAVLChecker_inventoryID.Count > 0)
                    {
                        //Case II
                        var curAVLChecker_plusVendorID = PXSelect<ApprovedVendLists,
                                                              Where<ApprovedVendLists.inventoryID, Equal<Required<ApprovedVendLists.inventoryID>>,
                                                              And<ApprovedVendLists.vendorid, Equal<Required<ApprovedVendLists.vendorid>>>>>.
                                                              Select(Base, curPOOrderLineRow.InventoryID, curPOOrderLineRow.VendorID);
                        if (curAVLChecker_plusVendorID.Count == 0)
                        {
                            var errorMsgInventory = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.
                                                           Select(Base, curPOOrderLineRow.InventoryID).TopFirst;

                            var errorMsgVendor = PXSelect<Vendor, Where<Vendor.bAccountID, Equal<Required<Vendor.bAccountID>>>>.
                                                               Select(Base, curPOOrder.VendorID).TopFirst;

                            Base.Transactions.Cache.Delete(curPOOrderLineRow);
                            throw new PXException($"vendor ID {errorMsgVendor.AcctCD.Trim()} - {errorMsgVendor.AcctName.Trim()} is not defined in approved vendor list for item {errorMsgInventory.InventoryCD.Trim()}.");
                        }
                    }

                }
            }

        }
        #endregion

        #endregion
    }
}