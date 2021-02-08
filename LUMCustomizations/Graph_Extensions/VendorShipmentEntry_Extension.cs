using JAMS.AM;
using LUMCustomizations.Library;
using PX.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUMCustomizations.Graph_Extensions
{
    public class VendorShipmentEntry_Extension: PXGraphExtension<VendorShipmentEntry>
    {
        public static bool IsActive()
        {
            //active customize button if current country ID is CN or HK
            return new LumLibrary().isCNorHK();
        }

        public override void Initialize()
        {
            base.Initialize();
            Base.report.AddMenuAction(VendorShipment);
        }

        #region Action
        public PXAction<AMVendorShipment> VendorShipment;
        [PXButton]
        [PXUIField(DisplayName = "Print Vendor Shipmetn Report", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable vendorShipment(PXAdapter adapter)
        {
            var _reportID = "lm611003";
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["ShipmentID"] = Base.Document.Current.ShipmentNbr;
                throw new PXReportRequiredException(parameters, _reportID, $"Report {_reportID}");
            }
            return adapter.Get();
        }
        #endregion

    }
}
