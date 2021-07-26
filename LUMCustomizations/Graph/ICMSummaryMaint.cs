using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAMS.AM;
using JAMS.AM.Attributes;
using LumCustomizations.DAC;
using LumCustomizations.Graph;
using LUMCustomizations.DAC;
using LUMCustomizations.Library;
using LuminaExcelLibrary;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using PX.CS;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.EP;
using PX.Objects.AP;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.TX;

namespace LUMCustomizations.Graph
{
    public class ICMSummaryMaint : PXGraph<ICMSummaryMaint>
    {
        public PXCancel<ICMFilter> Cancel;
        public PXFilter<ICMFilter> Filter;

        [PXFilterable]
        public PXFilteredProcessing<ICMSummary, ICMFilter, Where<ICMSummary.prodOrdID,
            Between<Current<ICMFilter.start_AMProdID>, Current<ICMFilter.end_AMProdID>>>> ICMList;

        public ICMSummaryMaint()
        {
            ICMList.SetProcessAllVisible(false);
            ICMList.SetProcessVisible(false);
        }

        public IEnumerable iCMList()
        {
            var amProdItem = SelectFrom<AMProdItem>
                .Where<AMProdItem.orderType.IsEqual<ICMType>>
                .View.Select(this).RowCast<AMProdItem>().ToList();
            var currFilter = this.Filter.Current;
            if (!string.IsNullOrEmpty(currFilter?.Start_AMProdID) && !string.IsNullOrEmpty(currFilter?.End_AMProdID))
                amProdItem = amProdItem.Where(
                    x => int.Parse(x.ProdOrdID.Substring(3)) >= int.Parse(currFilter.Start_AMProdID.Substring(3)) &&
                         int.Parse(x.ProdOrdID.Substring(3)) <= int.Parse(currFilter.End_AMProdID.Substring(3))).ToList();
            else
            {
                amProdItem = new List<AMProdItem>();
            }
            var icmGraph = PXGraph.CreateInstance<InternalCostModelMaint>();
            var inventoryData = SelectFrom<InventoryItem>.View.Select(this).RowCast<InventoryItem>().ToList();
            var initemXRef = SelectFrom<INItemXRef>.View.Select(this).RowCast<INItemXRef>().ToList();
            int lineNbr = 1;
            foreach (var item in amProdItem)
            {
                var summary = GetICMData(item, icmGraph);
                summary.LineNbr = lineNbr++;
                summary.ProdOrdID = item.ProdOrdID;
                summary.InventoryCD = inventoryData.FirstOrDefault(x => x.InventoryID == item.InventoryID).InventoryCD;
                summary.Description = inventoryData.FirstOrDefault(x => x.InventoryID == item.InventoryID).Descr;
                summary.CustomerPN = initemXRef.FirstOrDefault(x => x.InventoryID == item.InventoryID).AlternateID;
                yield return summary;
            }

            yield return null;
        }

        public ICMSummary GetICMData(AMProdItem row, InternalCostModelMaint icmGraph)
        {
            #region Varaible
            int rowNum = 0;
            decimal materialSum = 0;
          
            ICMSummary summaryResult = new ICMSummary();
            icmGraph.SettingCalaObject();
            // Currency Rate Type of ICM
            var graphProdDetail = PXGraph.CreateInstance<ProdDetail>();
            var iCMRateType = PXGraph.CreateInstance<InternalCostModelMaint>().Select<LifeSyncPreference>().Select(x => x.InternalCostModelRateType).FirstOrDefault();
            var aMProdAttribute = SelectFrom<AMProdAttribute>.Where<AMProdAttribute.orderType.IsEqual<P.AsString>
                .And<AMProdAttribute.prodOrdID.IsEqual<P.AsString>>>.View.Select(this, row.OrderType, row.ProdOrdID).RowCast<AMProdAttribute>().ToList();
            // Get Stock Item Vendor Details By LastModifierTime

            // Get All Material Data
            var materialData = from t in PXGraph.CreateInstance<InternalCostModelMaint>().Select<AMProdMatl>()
                               join i in icmGraph._inventoryItems on t.InventoryID equals i.InventoryID
                               where t.OrderType == row.OrderType && t.ProdOrdID == row.ProdOrdID
                               select new { material = t, inventoryCD = i.InventoryCD };
            var _AMProdOper = SelectFrom<AMProdOper>.Where<AMProdOper.orderType.IsEqual<P.AsString>
                .And<AMProdOper.prodOrdID.IsEqual<P.AsString>>>.View.Select(this, row.OrderType, row.ProdOrdID).RowCast<AMProdOper>().ToList();

            // Effect Curry Rate
            var _EffectCuryRate = new LumLibrary().GetCuryRateRecordEffData(this).Where(x => x.CuryRateType == iCMRateType).ToList();
            if (_EffectCuryRate.Count == 0)
                throw new PXException("Please Select ICM Rate Type !!");

            decimal _SetUpSum = 0;
            decimal _TotalCost = 0;
            var _StandardWorkingTime = (_AMProdOper.Sum(x => x.RunUnitTime) / _AMProdOper.Sum(x => x.RunUnits)).Value;

            #region AMProdAttribute 

            var _ENDC = aMProdAttribute.Where(x => x.AttributeID.Equals("ENDC")).FirstOrDefault()?.Value;
            var _EAU = aMProdAttribute.Where(x => x.AttributeID.Equals("EAU")).FirstOrDefault()?.Value;
            var _LBSCCost = aMProdAttribute.Where(x => x.AttributeID == "LBSC").FirstOrDefault()?.Value ?? "0";
            var _MFSCCost = aMProdAttribute.Where(x => x.AttributeID == "MFSC").FirstOrDefault()?.Value ?? "0";
            var _OHSCCost = aMProdAttribute.Where(x => x.AttributeID == "OHSC").FirstOrDefault()?.Value ?? "0";
            var _SETUPSCCost = aMProdAttribute.Where(x => x.AttributeID == "SETUPSC").FirstOrDefault()?.Value ?? "0";
            var _PRODYIELD = aMProdAttribute.Where(x => x.AttributeID == "PRODYIELD").FirstOrDefault()?.Value ?? "0";
            var _ABADGSELL = aMProdAttribute.Where(x => x.AttributeID == "ABADGSELL").FirstOrDefault()?.Value ?? "0";
            var _ABAHKSELL = aMProdAttribute.FirstOrDefault(x => x.AttributeID == "ABAHKSELL")?.Value ?? "0";
            var _HKOHSCCost = aMProdAttribute.Where(x => x.AttributeID == "HKOHSC").FirstOrDefault()?.Value ?? "0";
            var _ABISELLCost = aMProdAttribute.Where(x => x.AttributeID == "ABISELL").FirstOrDefault()?.Value ?? "0";

            #endregion

            #endregion

            #region 1.Material Cost Row(7~rowNum)

            foreach (var amMaterial in materialData.OrderBy(x => x.inventoryCD))
            {
                System.Collections.Stack visualBOM = new Stack();
                visualBOM = icmGraph.GetVisualBOM(visualBOM, amMaterial.material, 1);
                decimal? materailCost = 0;
                // Material Details
                foreach (InternalCostModelMaint.BomNode node in visualBOM)
                {
                    // Materail Cost(US$)
                    materialSum += node.Cost == null ? 0 : decimal.Parse(node.Cost?.ToString("N5"));
                }
            }

            // Materail Sum
            summaryResult.MaterialCost = materialSum;
            _SetUpSum += materialSum;
            #endregion

            #region 2.Labor Cost

            summaryResult.StandardTime = _StandardWorkingTime;
            summaryResult.LabourCost = (_StandardWorkingTime * decimal.Parse(_LBSCCost) *
                                        _EffectCuryRate.Where(x => x.FromCuryID == "USD").FirstOrDefault()
                                            .RateReciprocal).Value; ;
            // Sum
            _SetUpSum += (_StandardWorkingTime * decimal.Parse(_LBSCCost) * _EffectCuryRate.Where(x => x.FromCuryID == "USD").FirstOrDefault().RateReciprocal).Value;

            #endregion

            #region 3.Manufacture Cost
            summaryResult.ManufactureCost = (_StandardWorkingTime * decimal.Parse(_MFSCCost) / _EffectCuryRate.FirstOrDefault(x => x.FromCuryID == "USD").CuryRate.Value);
            _SetUpSum += (_StandardWorkingTime * decimal.Parse(_MFSCCost) * _EffectCuryRate.FirstOrDefault(x => x.FromCuryID == "USD").RateReciprocal).Value;
            #endregion

            #region 4.Overhead
            // Standard cost
            summaryResult.Overhead = (_StandardWorkingTime * decimal.Parse(_OHSCCost) /
                                     _EffectCuryRate.FirstOrDefault(x => x.FromCuryID == "USD").CuryRate.Value);
            _SetUpSum += (_StandardWorkingTime * decimal.Parse(_OHSCCost) / _EffectCuryRate.FirstOrDefault(x => x.FromCuryID == "USD").RateReciprocal).Value;
            #endregion

            #region 6.Production yield
            // Sum
            _TotalCost = (summaryResult.MaterialCost + summaryResult.LabourCost +
                          summaryResult.ManufactureCost + summaryResult.Overhead).Value /
                         (1 - decimal.Parse(_PRODYIELD) / 100);
            summaryResult.DGPrice = _TotalCost;
            summaryResult.Lumyield = _TotalCost - (summaryResult.MaterialCost + summaryResult.LabourCost +
                          summaryResult.ManufactureCost + summaryResult.Overhead).Value;
            #endregion

            #region 7.ABA DG Sell price
            // Sum
            var _abaDGPrice = _TotalCost + (_TotalCost * (decimal.Parse(_ABADGSELL) / 100));
            var _abaDGPrice_HKD = _abaDGPrice * _EffectCuryRate.Where(x => x.FromCuryID == "USD").FirstOrDefault()?.CuryRate * _EffectCuryRate.Where(x => x.FromCuryID == "HKD").FirstOrDefault()?.RateReciprocal;
            var temp = decimal.Parse(_ABADGSELL) + _TotalCost;
            summaryResult.DGtoHKPrice = temp;
            #endregion

            #region 8.HKOverhead
            // Sum
            summaryResult.HKOverhead = (decimal.Parse(_HKOHSCCost) * _StandardWorkingTime);

            #endregion

            #region 9.HK To ABI
            // Sum
            summaryResult.ABIPrice = summaryResult.DGtoHKPrice + (decimal.Parse(_ABAHKSELL) * summaryResult.HKOverhead) + summaryResult.HKOverhead;
            #endregion

            return summaryResult;
        }

    }

    public class ICMType : PX.Data.BQL.BqlString.Constant<ICMType>
    {
        public ICMType() : base("CM") { }
    }

    [Serializable]
    public class ICMFilter : IBqlTable
    {
        [PXDefault]
        [PXSelector(typeof(SearchFor<AMProdItem.prodOrdID>.Where<AMProdItem.orderType.IsEqual<ICMType>>))]
        [PXUIField(DisplayName = "Start Production Nbr.")]
        public virtual string Start_AMProdID { get; set; }
        public abstract class start_AMProdID : PX.Data.BQL.BqlString.Field<start_AMProdID> { }

        [PXDefault]
        [PXSelector(typeof(SearchFor<AMProdItem.prodOrdID>.Where<AMProdItem.orderType.IsEqual<ICMType>>))]
        [PXUIField(DisplayName = "End Production Nbr.")]
        public virtual string End_AMProdID { get; set; }
        public abstract class end_AMProdID : PX.Data.BQL.BqlString.Field<end_AMProdID> { }
    }
}
