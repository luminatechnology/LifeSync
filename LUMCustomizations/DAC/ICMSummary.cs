using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace LUMCustomizations.DAC
{
    [Serializable]
    public class ICMSummary : IBqlTable
    {
        [PXInt]
        [PXUIField(DisplayName = "Item")]
        public virtual int LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }

        [PXString]
        [PXUIField(DisplayName = "Production Order Nbr.")]
        public virtual string ProdOrdID { get; set; }
        public abstract class prodOrdID : PX.Data.BQL.BqlString.Field<prodOrdID> { }

        [PXString]
        [PXUIField(DisplayName = "P/N")]
        public virtual string InventoryCD { get; set; }
        public abstract class inventoryCD : PX.Data.BQL.BqlString.Field<inventoryCD> { }

        [PXString]
        [PXUIField(DisplayName = "Customer P/N")]
        public virtual string CustomerPN { get; set; }
        public abstract class customerPN : PX.Data.BQL.BqlString.Field<customerPN> { }

        [PXString]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }

        [PXDecimal(6)]
        [PXUIField(DisplayName =  "Standard time (Min)")]
        public virtual decimal? StandardTime { get; set; }
        public abstract class standardTime : PX.Data.BQL.BqlDecimal.Field<standardTime> { }

        [PXDecimal(6)]
        [PXUIField(DisplayName = "Material cost")]
        public virtual decimal? MaterialCost { get; set; }
        public abstract class materialCost : PX.Data.BQL.BqlDecimal.Field<materialCost> { }

        [PXDecimal(6)]
        [PXUIField(DisplayName = "Labour Cost")]
        public virtual decimal? LabourCost { get; set; }
        public abstract class labourCost : PX.Data.BQL.BqlDecimal.Field<labourCost> { }

        [PXDecimal(6)]
        [PXUIField(DisplayName = "Manufacture cost")]
        public virtual decimal? ManufactureCost { get; set; }
        public abstract class manufactureCost : PX.Data.BQL.BqlDecimal.Field<manufactureCost> { }

        [PXDecimal(6)]
        [PXUIField(DisplayName = "Overhead")]
        public virtual decimal? Overhead { get; set; }
        public abstract class overhead : PX.Data.BQL.BqlDecimal.Field<overhead> { }

        [PXDecimal(6)]
        [PXUIField(DisplayName = "Yield")]
        public virtual decimal? Lumyield { get; set; }
        public abstract class lumYield : PX.Data.BQL.BqlDecimal.Field<lumYield> { }

        [PXDecimal(6)]
        [PXUIField(DisplayName = "ABA DG total cost （USD）")]
        public virtual decimal? DGPrice { get; set; }
        public abstract class dgPrice : PX.Data.BQL.BqlDecimal.Field<dgPrice> { }

        [PXDecimal(6)]
        [PXUIField(DisplayName = "ABA HK overhead（USD）")]
        public virtual decimal? HKOverhead { get; set; }
        public abstract class hKoverhead : PX.Data.BQL.BqlDecimal.Field<hKoverhead> { }

        [PXDecimal(6)]
        [PXUIField(DisplayName = "ABA HK to ABI (USD)")]
        public virtual decimal? ABIPrice { get; set; }
        public abstract class abiPrice : PX.Data.BQL.BqlDecimal.Field<abiPrice> { }

        [PXDecimal(6)]
        [PXUIField(DisplayName = "ABA DG to ABA HK (USD)")]
        public virtual decimal? DGtoHKPrice { get; set; }
        public abstract class dgtoHKPrice : PX.Data.BQL.BqlDecimal.Field<dgtoHKPrice> { }

    }
}
