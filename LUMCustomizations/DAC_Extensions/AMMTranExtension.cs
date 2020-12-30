using JAMS.AM;
using JAMS.AM.Attributes;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAMS.AM
{
    public class AMMTranExt : PXCacheExtension<AMMTran>
    {
        [PXString]
        [PXUIField(DisplayName ="Work center")]
        [PXDBScalar(typeof(SelectFrom<AMProdOper>.Where<AMProdOper.orderType.IsEqual<AMMTran.orderType>
                                                 .And<AMProdOper.prodOrdID.IsEqual<AMMTran.prodOrdID>>>.SearchFor<AMProdOper.wcID>))]
        public virtual string WcID { get; set; }
        public abstract class wcID : BqlType<IBqlGuid, string>.Field<AMMTranExt.wcID> { }
    }
}
