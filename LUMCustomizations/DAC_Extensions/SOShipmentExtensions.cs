using PX.Data;
using PX.Data.BQL;
using PX.Objects.CS;

namespace PX.Objects.SO
{
    public class SOShipmentExt : PXCacheExtension<SOShipment>
    {
        [LocationID(typeof(Where<PX.Objects.CR.Location.bAccountID, Equal<Current<SOShipment.customerID>>, 
                                 And<PX.Objects.CR.Location.isActive, Equal<True>, And<MatchWithBranch<PX.Objects.CR.Location.cBranchID>>>>), 
                    DescriptionField = typeof(PX.Objects.CR.Location.descr), DisplayName = "Ship To", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? UsrShipToID { get; set; }
        public abstract class usrShipToID : BqlType<IBqlInt, int>.Field<SOShipmentExt.usrShipToID> { }

        [PXString]
        [PXDependsOnFields(new System.Type[] { typeof(SOShipment.shipmentNbr) })]
        public virtual string UsrCombLotSerNbr => SOShipmentEntry_Extension.GetMultLotSerNbr(this.Base.ShipmentNbr);
        public abstract class usrCombLotSerNbr : BqlType<IBqlString, string>.Field<SOShipmentExt.usrCombLotSerNbr> { }
    }
}
