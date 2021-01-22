using LumCustomizations.Descriptor;
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
        [LumValidIndex(typeof(AMProdItem.lineCntrEvnt))]
        [PXUIField(DisplayName = "Event Line Number123456", Enabled = false)]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : BqlType<IBqlInt, int>.Field<lineNbr> { }
    }
}
