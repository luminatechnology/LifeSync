using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace JAMS.AM
{
    public class MatlWizard2_Extension : PXGraphExtension<MatlWizard2>
    {
        public override void Initialize()
        {
            base.Initialize();
            Base.ProcessMatl.SetProcessDelegate(
                delegate (List<AMWrkMatl> list)
                {
                    try
                    {
                        MatlWizard2.BuildMaterialTransaction(list);
                    }
                    catch (PXRedirectRequiredException)
                    {
                        var grpSelectedData = list.GroupBy(x => new { x.ProdOrdID, x.GetExtension<AMWrkMatlExt>().UsrTempProdQty });
                        foreach (var item in grpSelectedData)
                        {
                            PXUpdate<Set<AMProdItemExt.usrQtyIssued, Required<AMProdItemExt.usrQtyIssued>>,
                                       AMProdItem,
                                       Where<AMProdItem.prodOrdID, Equal<Required<AMProdItem.prodOrdID>>>>.Update(Base, item.Key.UsrTempProdQty, item.Key.ProdOrdID);
                        }
                        throw;
                    }
                }
            );
        }
    }
}
