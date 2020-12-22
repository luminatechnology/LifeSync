using PX.Data;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUMCustomizations.DAC
{
    [System.SerializableAttribute]
    public partial class PrintParameters : IBqlTable, PX.SM.IPrintable
    {
        #region PrintWithDeviceHub
        public abstract class printWithDeviceHub : IBqlField { }

        [PXDBBool]
        [PXDefault(typeof(FeatureInstalled<FeaturesSet.deviceHub>))]
        [PXUIField(DisplayName = "Print with DeviceHub")]
        public virtual bool? PrintWithDeviceHub { get; set; }
        #endregion

        #region DefinePrinterManually
        public abstract class definePrinterManually : IBqlField { }

        [PXDBBool]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Define Printer Manually")]
        public virtual bool? DefinePrinterManually { get; set; }
        #endregion

        #region Printer
        public abstract class printerName : PX.Data.IBqlField { }

        [PX.SM.PXPrinterSelector]
        public virtual string PrinterName { get; set; }
        #endregion

        public Guid? PrinterID { get; set; }
        public int? NumberOfCopies { get; set; }
    }
}
