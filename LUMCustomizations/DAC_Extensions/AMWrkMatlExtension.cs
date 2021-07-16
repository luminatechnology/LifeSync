using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAMS.AM
{
    public class AMWrkMatlExt: PXCacheExtension<AMWrkMatl>
    {
        [PXDBDecimal]
        [PXUIField(DisplayName = "Wizard1 Input ProdQty(TempSave)", Visible = false, Enabled = false)]
        public virtual decimal? UsrTempProdQty { get; set; }
        public abstract class usrTempProdQty : PX.Data.BQL.BqlDecimal.Field<usrTempProdQty> { }
    }
}
