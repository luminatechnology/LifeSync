using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.GL;
using System;
using System.Runtime.CompilerServices;

namespace PX.Objects.SO
{
    public class SOOrderExt : PXCacheExtension<SOOrder>
    {
        [PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
        [PXSelector(typeof(Currency.curyID))]
        [PXUIField(DisplayName = "PI Currency", Visibility = PXUIVisibility.SelectorVisible, Required = false)]
        public virtual string UsrPICuryID { get; set; }
        public abstract class usrPICuryID : PX.Data.BQL.BqlString.Field<SOOrderExt.usrPICuryID> { }


        [CustomerActive(
            typeof(Search<Customer.bAccountID, 
                    Where<Customer.bAccountID, IsNotNull>>), 
            Visibility = PXUIVisibility.SelectorVisible, 
            DescriptionField = typeof(Customer.acctName), 
            Filterable = true, 
            DisplayName = "PI Customer", 
            Required = false)]
        [PXForeignReference(typeof(Field<SOOrder.customerID>.IsRelatedTo<BAccount.bAccountID>))]
        public virtual int? UsrPICustomerID { get; set; }
        public abstract class usrPICustomerID : PX.Data.BQL.BqlInt.Field<SOOrderExt.usrPICustomerID> { }
    }
}