// Decompiled with JetBrains decompiler
// Type: LumCustomizations.Descriptor.SOLineSelectorAttribute
// Assembly: LumCustomizations, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CAF5AA38-83F0-40FF-909D-BF37C3B18F4B
// Assembly location: C:\Program Files\Acumatica ERP\LifeSync\Bin\LumCustomizations.dll

using JAMS.AM;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.SO;
using System;

namespace LumCustomizations.Descriptor
{
    public class SOLineSelectorAttribute : PXAggregateAttribute
    {
        protected readonly int _SelAttrIndex;
        protected System.Type _inventoryType;

        public SOLineSelectorAttribute()
        {
        }

        public SOLineSelectorAttribute(System.Type inventorType)
        {
            this._inventoryType = inventorType;
            PXAggregateAttribute.AggregatedAttributesCollection attributes = this._Attributes;
            System.Type type = BqlCommand.Compose(typeof(Search2<,,>), typeof(SOLine.noteID), typeof(InnerJoin<,>), typeof(SOOrder), typeof(On<,,>), typeof(SOOrder.orderType), typeof(Equal<SOLine.orderType>), typeof(And<,>), typeof(SOOrder.orderNbr), typeof(Equal<SOLine.orderNbr>), typeof(Where<,>), typeof(SOLine.inventoryID), typeof(Equal<>), typeof(Optional<>), this._inventoryType);
            System.Type[] typeArray = new System.Type[6]
            {
        typeof (SOOrder.orderNbr),
        typeof (SOOrder.customerOrderNbr),
        typeof (SOLine.lineNbr),
        typeof (SOLine.inventoryID),
        typeof (SOLine.orderQty),
        typeof (SOLine.requestDate)
            };
            SOLineSelectorAttribute.SOLineSubstituteSelectorAttribute selectorAttribute1;
            PXSelectorAttribute selectorAttribute2 = (PXSelectorAttribute)(selectorAttribute1 = new SOLineSelectorAttribute.SOLineSubstituteSelectorAttribute(type, typeArray));
            attributes.Add((PXEventSubscriberAttribute)selectorAttribute1);
            this._SelAttrIndex = this._Attributes.Count - 1;
            selectorAttribute2.DescriptionField = typeof(SOLine.tranDesc);
            selectorAttribute2.ValidateValue = true;
            selectorAttribute2.CacheGlobal = true;
        }

        public class SOLineSubstituteSelectorAttribute : PXSelectorAttribute
        {
            public SOLineSubstituteSelectorAttribute(System.Type type, params System.Type[] fieldList)
              : base(type, fieldList)
            {
            }

            public override void SubstituteKeyFieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
            {
                if (e.NewValue == null || GUID.TryParse(e.NewValue.ToString(), out Guid _))
                    return;
                base.SubstituteKeyFieldUpdating(sender, e);
            }

            public override void SubstituteKeyFieldSelecting(PXCache sender, PXFieldSelectingEventArgs e) => base.SubstituteKeyFieldSelecting(sender, e);

            public override void DescriptionFieldSelecting(
              PXCache sender,
              PXFieldSelectingEventArgs e,
              string alias)
            {
                object obj = sender.GetValue(e.Row, this._FieldName);
                base.DescriptionFieldSelecting(sender, e, alias);
                if (obj == null || e.ReturnValue != null)
                    return;
                using (new PXReadDeletedScope())
                {
                    SOLine soLine = (SOLine)PXSelectBase<SOLine, PXSelect<SOLine, Where<SOLine.noteID, Equal<Required<SOLine.noteID>>>>.Config>.SelectSingleBound(sender.Graph, new object[0], obj);
                    e.ReturnValue = soLine != null ? (object)soLine.OrderNbr : obj;
                    e.ReturnState = (object)PXFieldState.CreateInstance(e.ReturnState, typeof(string), new bool?(false), new bool?(true), fieldName: alias, error: PXLocalizer.Localize("Sales Order line was not found.", typeof(PX.Objects.SO.Messages).FullName), errorLevel: PXErrorLevel.Warning, enabled: new bool?(false), visibility: PXUIVisibility.Visible);
                }
            }
        }
    }

    public class LumValidIndexAttribute : PXLineNbrAttribute, IPXRowPersistingSubscriber
    {
        protected readonly Type _sourceType;
        protected readonly string _sourceField;
        public LumValidIndexAttribute(Type sourceType) : base(sourceType)
        {
            if (typeof(IBqlField).IsAssignableFrom(sourceType) && sourceType.IsNested)
            {
                this._sourceType = BqlCommand.GetItemType(sourceType);
                this._sourceField = sourceType.Name;
            }
            else if (typeof(IBqlTable).IsAssignableFrom(sourceType))
            {
                this._sourceType = sourceType;
            }
            else
            {
                object[] args = new object[] { sourceType };
                throw new PXArgumentException("type", "A foreign key reference cannot be created from the type '{0}'.", args);
            }
        }

        public virtual void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            var amProdItemCache = sender.Graph.Caches[this._sourceType];
            if (amProdItemCache.Current != null)
            {
                var _fieldValue = amProdItemCache.GetValue(amProdItemCache.Current, _sourceField);
                var dbValue = SelectFrom<AMProdItem>
                              .Where<AMProdItem.orderType.IsEqual<@P.AsString>
                                .And<AMProdItem.prodOrdID.IsEqual<@P.AsString>>>
                              .View.Select(
                                sender.Graph,
                                amProdItemCache.GetValue<AMProdItem.orderType>(amProdItemCache.Current),
                                amProdItemCache.GetValue<AMProdItem.prodOrdID>(amProdItemCache.Current));
                if (((IComparable)_fieldValue).CompareTo((IComparable)dbValue.TopFirst.LineCntrEvnt) != 0)
                    throw new PXException("AAAAAAA");
            }
        }
    }
}
