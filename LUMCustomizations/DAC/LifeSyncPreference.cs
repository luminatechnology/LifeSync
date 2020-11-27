using PX.Data;
using PX.Data.BQL;
using System;
using System.Runtime.CompilerServices;

namespace LumCustomizations.DAC
{
    [PXCacheName("LifeSyncPreference")]
    [Serializable]
    public class LifeSyncPreference : IBqlTable
    {
        [PXDBCreatedByID]
        public virtual Guid? CreatedByID
        {
            get;
            set;
        }

        [PXDBCreatedByScreenID]
        public virtual string CreatedByScreenID
        {
            get;
            set;
        }

        [PXDBCreatedDateTime]
        public virtual DateTime? CreatedDateTime
        {
            get;
            set;
        }

        [PXDBLastModifiedByID]
        public virtual Guid? LastModifiedByID
        {
            get;
            set;
        }

        [PXDBLastModifiedByScreenID]
        public virtual string LastModifiedByScreenID
        {
            get;
            set;
        }

        [PXDBLastModifiedDateTime]
        public virtual DateTime? LastModifiedDateTime
        {
            get;
            set;
        }

        [PXDBBool]
        [PXUIField(DisplayName = "Proforma Invoice Printing")]
        public virtual bool? ProformaInvoicePrinting
        {
            get;
            set;
        }

        public abstract class createdByID : PX.Data.BQL.BqlString.Field<LifeSyncPreference.createdByID>
        {
            protected createdByID()
            {
            }
        }

        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<LifeSyncPreference.createdByScreenID>
        {
            protected createdByScreenID()
            {
            }
        }

        public abstract class createdDateTime : PX.Data.BQL.BqlString.Field< LifeSyncPreference.createdDateTime>
        {
            protected createdDateTime()
            {
            }
        }

        public abstract class lastModifiedByID : PX.Data.BQL.BqlString.Field<LifeSyncPreference.lastModifiedByID>
        {
            protected lastModifiedByID()
            {
            }
        }

        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<LifeSyncPreference.lastModifiedByScreenID>
        {
            protected lastModifiedByScreenID()
            {
            }
        }

        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlString.Field<LifeSyncPreference.lastModifiedDateTime>
        {
            protected lastModifiedDateTime()
            {
            }
        }

        public abstract class proformaInvoicePrinting : PX.Data.BQL.BqlString.Field<LifeSyncPreference.proformaInvoicePrinting>
        {
        }
    }
}