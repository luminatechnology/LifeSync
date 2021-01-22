using JAMS.AM;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUMProductionEventFixed.Descriptor
{
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
            var tmp = sender.Graph.Caches[typeof(AMProdEvnt)];
            if (amProdItemCache.Current != null)
            {
                // AMProdEvnt LineNbr from Cache
                var _nowfieldValue = sender.GetValue(e.Row, base._FieldOrdinal);
                // AMProdItem LineCntrEvnt from DataBase
                var _dbLineCntrEvnt = SelectFrom<AMProdItem>
                              .Where<AMProdItem.orderType.IsEqual<P.AsString>
                                .And<AMProdItem.prodOrdID.IsEqual<P.AsString>>>
                              .View.Select(
                                sender.Graph,
                                amProdItemCache.GetValue<AMProdItem.orderType>(amProdItemCache.Current),
                                amProdItemCache.GetValue<AMProdItem.prodOrdID>(amProdItemCache.Current));
                if (((IComparable)_nowfieldValue).CompareTo((IComparable)_dbLineCntrEvnt.TopFirst.LineCntrEvnt) <= 0)
                    sender.SetValue(e.Row, base._FieldOrdinal, (_dbLineCntrEvnt.TopFirst.LineCntrEvnt ?? 0) + 1);
            }
        }
    }
}
