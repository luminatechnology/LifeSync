using PX.Data;
using PX.Data.BQL;
using System;
using System.Runtime.CompilerServices;

namespace LUMCustomizations.DAC
{
    [PXCacheName("LifeSyncPreference")]
    [Serializable]
    public class LifeSyncPreference : IBqlTable
    {
        [PXDBCreatedByID]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlString.Field<LifeSyncPreference.createdByID> { }

        [PXDBCreatedByScreenID]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<LifeSyncPreference.createdByScreenID> { }

        [PXDBCreatedDateTime]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlString.Field<LifeSyncPreference.createdDateTime> { }

        [PXDBLastModifiedByID]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlString.Field<LifeSyncPreference.lastModifiedByID> { }

        [PXDBLastModifiedByScreenID]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<LifeSyncPreference.lastModifiedByScreenID> { }

        [PXDBLastModifiedDateTime]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlString.Field<LifeSyncPreference.lastModifiedDateTime> { }

        [PXDBBool]
        [PXUIField(DisplayName = "Proforma Invoice Printing")]
        public virtual bool? ProformaInvoicePrinting { get; set; }
        public abstract class proformaInvoicePrinting : PX.Data.BQL.BqlBool.Field<LifeSyncPreference.proformaInvoicePrinting> { }
    }
}