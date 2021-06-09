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
        [PXDefault(false)]
        [PXUIField(DisplayName = "Proforma Invoice Printing")]
        public virtual bool? ProformaInvoicePrinting { get; set; }
        public abstract class proformaInvoicePrinting : PX.Data.BQL.BqlBool.Field<LifeSyncPreference.proformaInvoicePrinting> { }

        [PXDBBool]
        [PXUIField(DisplayName = "Enable Bubble Number Printing")]
        public virtual bool? BubbleNumberPrinting { get; set; }
        public abstract class bubbleNumberPrinting : PX.Data.BQL.BqlBool.Field<LifeSyncPreference.bubbleNumberPrinting> { }

        [PXDBString(6)]
        [PXStringList()]
        [PXUIField(DisplayName = "Rate Type for Internal Cost Model")]
        public virtual string InternalCostModelRateType { get; set; }
        public abstract class internalCostModelRateType : PX.Data.BQL.BqlString.Field<LifeSyncPreference.internalCostModelRateType> { }

        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Enable Cross Rate Override")]
        public virtual bool? CrossRateOverride { get; set; }
        public abstract class crossRateOverride : PX.Data.BQL.BqlBool.Field<LifeSyncPreference.crossRateOverride> { }

        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Enable Showing Total in Home Currency")]
        public virtual bool? ShowingTotalInHomeCurrency { get; set; }
        public abstract class showingTotalInHomeCurrency : PX.Data.BQL.BqlBool.Field<showingTotalInHomeCurrency> { }

        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Enable Journal transaction enhancement")]
        public virtual bool? EnableJournalEnhance { get; set; }
        public abstract class enableJournalEnhance : PX.Data.BQL.BqlBool.Field<enableJournalEnhance> { }

        #region EnableProdCostAnlys
        [PXDBBool]
        [PXDefault(false,PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Enable Production Cost Analysis")]
        public virtual bool? EnableProdCostAnlys { get; set; }
        public abstract class enableProdCostAnlys : PX.Data.BQL.BqlBool.Field<enableProdCostAnlys> { }
        #endregion

        #region MaxOverIssue
        [PXDBDecimal]
        [PXDefault(10.00,PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Max Over Issue Material %")]
        public virtual decimal? MaxOverIssue { get;set;}
        public abstract class maxOverIssue : PX.Data.BQL.BqlDecimal.Field<maxOverIssue> { }
        #endregion

    }
}