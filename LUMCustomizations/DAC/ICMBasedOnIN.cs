using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace LUMCustomizations.DAC
{
    [Serializable]
    public class ICMBasedOnIN : IBqlTable
    {
        [PXInt]
        [PXUIField(DisplayName = "Item")]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }

        [PXString]
        [PXUIField(DisplayName = "BOM ID")]
        public virtual string BOMID { get; set; }
        public abstract class bOMID : PX.Data.BQL.BqlString.Field<bOMID> { }

        [PXString]
        [PXUIField(DisplayName = "Inventory ID")]
        public virtual string InventoryCD { get; set; }
        public abstract class inventoryCD : PX.Data.BQL.BqlString.Field<inventoryCD> { }

        [PXString]
        [PXUIField(DisplayName = "RevisionID")]
        public virtual string RevisionID { get; set; }
        public abstract class revisionID : PX.Data.BQL.BqlString.Field<revisionID> { }

        [PXString]
        [PXUIField(DisplayName = "Description")]
        public virtual string Descr { get; set; }
        public abstract class descr : PX.Data.BQL.BqlString.Field<descr> { }

        [PXInt]
        [PXUIField(DisplayName = "SubItem ID")]
        public virtual int? SubItemID { get; set; }
        public abstract class subItemID : PX.Data.BQL.BqlInt.Field<subItemID> { }
    }
}
