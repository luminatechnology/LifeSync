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
            // Reset TimeStamp
            sender.Graph.SelectTimeStamp();
            var amProdItemCache = (PXCache<AMProdItem>)sender.Graph.Caches[this._sourceType];
            if (amProdItemCache.Current != null)
            {
                // AMProdItem lineCntrEvnt from Cache
                var _nowLineCntrEvnt = ((AMProdItem)amProdItemCache.Current).LineCntrEvnt;
                // AMProdItem LineCntrEvnt from DataBase
                var _dbLineCntrEvnt = SelectFrom<AMProdItem>
                              .Where<AMProdItem.orderType.IsEqual<@P.AsString>
                                .And<AMProdItem.prodOrdID.IsEqual<@P.AsString>>>
                              .View.Select(new PXGraph(),
                                    amProdItemCache.GetValue<AMProdItem.orderType>(amProdItemCache.Current),
                                    amProdItemCache.GetValue<AMProdItem.prodOrdID>(amProdItemCache.Current)).TopFirst;

                if (_nowLineCntrEvnt <= _dbLineCntrEvnt?.LineCntrEvnt)
                {
                    // reset AMProdItem LineCntrEvnt
                    (amProdItemCache.Current as AMProdItem).LineCntrEvnt = (_dbLineCntrEvnt.LineCntrEvnt ?? 0) + 1;
                    // reset Event LineNbr
                    sender.SetValue(e.Row, base._FieldOrdinal, (_dbLineCntrEvnt.LineCntrEvnt ?? 0) + 1);
                }
            }
        }
    }
}
