using System;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.CS;
using Location = PX.Objects.CR.Standalone.Location;

namespace PX.Objects.SO
{
    public class SOShipmentEntry_Extension : PXGraphExtension<SOShipmentEntry>
    {
        #region String  & Decimal Contants
        public const string CartonSize = "CARTONSIZE";
        public const string QtyCarton = "QTYCARTON";
        public const string GrsWeight = "GRSWEIGHT";

        public class CartonSizeAttr : PX.Data.BQL.BqlString.Constant<CartonSizeAttr>
        {
            public CartonSizeAttr() : base(CartonSize) { }
        }

        public class QtyCartonAttr : PX.Data.BQL.BqlString.Constant<QtyCartonAttr>
        {
            public QtyCartonAttr() : base(QtyCarton) { }
        }

        public class GrsWeightAttr : PX.Data.BQL.BqlString.Constant<GrsWeightAttr>
        {
            public GrsWeightAttr() : base(GrsWeight) { }
        }

        public class decimal5000 : PX.Data.BQL.BqlDecimal.Constant<decimal5000>
        {
            public decimal5000() : base(5000M) { }
        }
        #endregion

        #region Delegate Function        
        public delegate void DelegatePersist();
        [PXOverride]
        public void Persist(DelegatePersist baseMethod)
        {
            baseMethod();

            this.SetShipContactAndAddress();

            baseMethod();
        }
        #endregion

        protected void _(Events.FieldUpdated<SOShipLineExt.usrCartonQty> e)
        {
            SOShipLine row = e.Row as SOShipLine;
            SOShipLineExt rowExt = row.GetExtension<SOShipLineExt>();

            decimal? cartonQty = rowExt.UsrCartonQty;
            decimal? baseItemVol = rowExt.UsrBaseItemVolume;
            decimal oneHundK = 1000000;

            //Decimal? nullable2 = nullable1.HasValue & usrBaseItemVolume.HasValue ? new Decimal?(nullable1.GetValueOrDefault() * usrBaseItemVolume.GetValueOrDefault()) : new Decimal?();

            //Decimal? nullable3 = nullable2.HasValue ? new Decimal?(nullable2.GetValueOrDefault() * num1) : new Decimal?();
            //nullable2 = nullable3;
        //    Decimal num2 = 0M;
        //    Decimal? nullable4;
        //    if (!(nullable2.GetValueOrDefault() == num2 & nullable2.HasValue))
        //    {
        //        nullable2 = row.ShippedQty;
        //        Decimal num3 = 0M;
        //        if (!(nullable2.GetValueOrDefault() == num3 & nullable2.HasValue))
        //        {
        //            nullable2 = nullable3;
        //            Decimal? shippedQty = row.ShippedQty;
        //            if (!(nullable2.HasValue & shippedQty.HasValue))
        //            {
        //                nullable1 = new Decimal?();
        //                nullable4 = nullable1;
        //                goto label_6;
        //            }
        //            else
        //            {
        //                nullable4 = new Decimal?(nullable2.GetValueOrDefault() / shippedQty.GetValueOrDefault());
        //                goto label_6;
        //            }
        //        }
        //    }
        //    nullable4 = new Decimal?(0M);
        //label_6:
        //    soShipLineExt.UsrDimWeight = nullable4;
        }

        #region Static Method
        public static string GetMultLotSerNbr(string shipmentNbr)
        {
            string str = null;

            foreach (PXResult<SOShipLineSplit> result in SelectFrom<SOShipLineSplit>.Where<SOShipLineSplit.shipmentNbr.IsEqual<@P.AsString>>.View.ReadOnly
                                                         .Select(PXGraph.CreateInstance<SOShipmentEntry>(), shipmentNbr))
            {
                SOShipLineSplit shipLineSplit = result;

                str += string.Format("{0}/", shipLineSplit.LotSerialNbr);
            }

            return str == null ? string.Empty : str.Substring(0, str.Length - 1);
        }
        #endregion

        #region Method
        public virtual void SetShipContactAndAddress()
        {
            SOShipment current = Base.Document.Current;

            if (current == null || !current.GetExtension<SOShipmentExt>().UsrShipToID.HasValue) { return; }

            AddressAttribute addressAttribute = new SOShipmentAddressAttribute(typeof(Select2<Address, InnerJoin<Location, On<Location.bAccountID, Equal<Address.bAccountID>,
                                                                                                                              And<Address.addressID, Equal<Location.defAddressID>>>,
                                                                                                       LeftJoin<SOShipmentAddress, On<SOShipmentAddress.customerID, Equal<Address.bAccountID>,
                                                                                                                                      And<SOShipmentAddress.customerAddressID, Equal<Address.addressID>,
                                                                                                                                          And<SOShipmentAddress.revisionID, Equal<Address.revisionID>,
                                                                                                                                              And<SOShipmentAddress.isDefaultAddress, Equal<True>>>>>>>,
                                                                                                       Where<Location.bAccountID, Equal<Current<SOShipment.customerID>>,
                                                                                                             And<Location.locationID, Equal<Current<SOShipmentExt.usrShipToID>>>>>))
            {
                FieldName = "shipAddressID"
            };
            addressAttribute.DefaultAddress<SOShipmentAddress, SOShipmentAddress.addressID>(Base.Document.Cache, current, null);

            ContactAttribute contactAttribute = new SOShipmentContactAttribute(typeof(Select2<Contact, InnerJoin<Location, On<Location.bAccountID, Equal<Contact.bAccountID>,
                                                                                                                              And<Contact.contactID, Equal<Location.defContactID>>>,
                                                                                                       LeftJoin<SOShipmentContact, On<SOShipmentContact.customerID, Equal<Contact.bAccountID>,
                                                                                                                                      And<SOShipmentContact.customerContactID, Equal<Contact.contactID>,
                                                                                                                                          And<SOShipmentContact.revisionID, Equal<Contact.revisionID>,
                                                                                                                                              And<SOShipmentContact.isDefaultContact, Equal<True>>>>>>>,
                                                                                                       Where<Location.bAccountID, Equal<Current<SOShipment.customerID>>,
                                                                                                             And<Location.locationID, Equal<Current<SOShipmentExt.usrShipToID>>>>>))
            {
                FieldName = "shipContactID"
            };

            contactAttribute.DefaultContact<SOShipmentContact, SOShipmentContact.contactID>(Base.Document.Cache, (object)current, (object)null);

            Base.Document.Cache.Update((object)current);
        }
        #endregion
    }
}
