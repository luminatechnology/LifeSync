using LUMProductionEventFixed.Descriptor;
using PX.Data;
using PX.Data.BQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAMS.AM
{
    public class AMProdEvntExtension : PXCacheExtension<AMProdEvnt>
    {
        protected int _lineNbr;
        [PXDBInt(IsKey = true)]
        [PXDefault]
        //[PXLineNbr(typeof(AMProdItem.lineCntrEvnt))]
        [LumValidIndex(typeof(AMProdItem.lineCntrEvnt))]
        [PXUIField(DisplayName = "Event Line Number", Enabled = false)]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : BqlType<IBqlInt, int>.Field<lineNbr> { }
    }
}
