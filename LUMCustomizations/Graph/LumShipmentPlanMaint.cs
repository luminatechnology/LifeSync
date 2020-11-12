// Decompiled with JetBrains decompiler
// Type: LumCustomizations.Graph.LumShipmentPlanMaint
// Assembly: LumCustomizations, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CAF5AA38-83F0-40FF-909D-BF37C3B18F4B
// Assembly location: C:\Program Files\Acumatica ERP\LifeSync\Bin\LumCustomizations.dll

using JAMS.AM;
using LumCustomizations.DAC;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.SO;

namespace LumCustomizations.Graph
{
  public class LumShipmentPlanMaint : PXGraph<LumShipmentPlanMaint, LumShipmentPlan>
  {
    public const string NotDeleteConfirmed = "The Shipment Plan [{0}] Had Confirmed And Can't Be Deleted.";
    [PXFilterable(new System.Type[] {})]
    public FbqlSelect<SelectFromBase<LumShipmentPlan, TypeArrayOf<IFbqlJoin>.Empty>, LumShipmentPlan>.View ShipPlan;
    public FbqlSelect<SelectFromBase<SOOrder, TypeArrayOf<IFbqlJoin>.Empty>.Where<BqlChainableConditionBase<TypeArrayOf<IBqlBinary>.FilledWith<And<Compare<SOOrder.orderType, Equal<LumShipmentPlan.orderType>>>>>.And<BqlOperand<SOOrder.orderNbr, IBqlString>.IsEqual<LumShipmentPlan.orderNbr>>>, SOOrder>.View Order;

    protected void _(PX.Data.Events.RowDeleting<LumShipmentPlan> e)
    {
      int num;
      if (e.Row != null)
      {
        bool? confirmed = e.Row.Confirmed;
        bool flag = true;
        num = confirmed.GetValueOrDefault() == flag & confirmed.HasValue ? 1 : 0;
      }
      else
        num = 0;
      if (num != 0)
        throw new PXSetPropertyException<LumShipmentPlan.confirmed>("The Shipment Plan [{0}] Had Confirmed And Can't Be Deleted.", new object[1]
        {
          (object) e.Row.ShipmentPlanID
        });
    }

    protected void _(PX.Data.Events.RowPersisting<LumShipmentPlan> e) => AutoNumberAttribute.SetNumberingId<LumShipmentPlan.shipmentPlanID>(e.Cache, "SHIPPLAN");

    protected void _(
      PX.Data.Events.FieldUpdated<LumShipmentPlan.sOLineNoteID> e)
    {
      if (!(e.Row is LumShipmentPlan row))
        return;
      SOLine soLine = (SOLine) PXSelectBase<SOLine, PXViewOf<SOLine>.BasedOn<SelectFromBase<SOLine, TypeArrayOf<IFbqlJoin>.Empty>.Where<BqlOperand<SOLine.noteID, IBqlGuid>.IsEqual<P.AsGuid>>>.Config>.Select((PXGraph) this, (object) row.SOLineNoteID);
      SOOrder soOrder = (SOOrder) PXSelectBase<SOOrder, PXViewOf<SOOrder>.BasedOn<SelectFromBase<SOOrder, TypeArrayOf<IFbqlJoin>.Empty>.Where<BqlChainableConditionBase<TypeArrayOf<IBqlBinary>.FilledWith<And<Compare<SOOrder.orderType, Equal<P.AsString>>>>>.And<BqlOperand<SOOrder.orderNbr, IBqlString>.IsEqual<P.AsString>>>>.Config>.SelectSingleBound((PXGraph) this, (object[]) null, (object) soLine.OrderType, (object) soLine.OrderNbr);
      PXFieldState valueExt = this.Order.Cache.GetValueExt((object) soOrder, "AttributeENDC") as PXFieldState;
      row.Customer = (string) valueExt.Value;
      row.OrderNbr = soOrder.OrderNbr;
      row.OrderType = soOrder.OrderType;
      row.CustomerLocationID = soOrder.CustomerLocationID;
      row.CustomerOrderNbr = soOrder.CustomerOrderNbr;
      row.OrderDate = soOrder.OrderDate;
      row.LineNbr = soLine.LineNbr;
      row.InventoryID = soLine.InventoryID;
      row.OpenQty = soLine.OpenQty;
      row.OrderQty = soLine.OrderQty;
      row.RequestDate = soLine.RequestDate;
    }

    protected void _(PX.Data.Events.FieldUpdated<LumShipmentPlan.prodOrdID> e)
    {
      if (!(e.Row is LumShipmentPlan row))
        return;
      AMProdItem amProdItem = (AMProdItem) PXSelectBase<AMProdItem, PXViewOf<AMProdItem>.BasedOn<SelectFromBase<AMProdItem, TypeArrayOf<IFbqlJoin>.Empty>.Where<BqlOperand<AMProdItem.prodOrdID, IBqlString>.IsEqual<P.AsString>>>.Config>.Select((PXGraph) this, (object) row.ProdOrdID);
      row.QtytoProd = amProdItem.QtytoProd;
      row.QtyComplete = amProdItem.QtyComplete;
      foreach (PXResult<AMProdAttribute> pxResult in PXSelectBase<AMProdAttribute, PXViewOf<AMProdAttribute>.BasedOn<SelectFromBase<AMProdAttribute, TypeArrayOf<IFbqlJoin>.Empty>.Where<BqlOperand<AMProdAttribute.prodOrdID, IBqlString>.IsEqual<P.AsString>>>.Config>.Select((PXGraph) this, (object) amProdItem.ProdOrdID))
      {
        AMProdAttribute amProdAttribute = (AMProdAttribute) pxResult;
        switch (amProdAttribute.AttributeID)
        {
          case "PRODLINE":
            row.ProdLine = amProdAttribute.Value;
            break;
          case "LOTNO":
            row.LotSerialNbr = amProdAttribute.Value;
            break;
          case "BR":
            row.BRNbr = amProdAttribute.Value;
            break;
        }
      }
    }
  }
}
