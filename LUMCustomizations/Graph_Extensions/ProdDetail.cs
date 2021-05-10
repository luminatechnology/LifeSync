using System;
using PX.Objects;
using PX.Data;
using JAMS.AM.Attributes;

namespace JAMS.AM
{
    public class ProdDetail_Extension : PXGraphExtension<ProdDetail>
    {
        #region Override ProdMatlRecords view
        [PXViewName("Material")]
        [PXImport(typeof(AMProdItem))]
        public AMOrderedMatlSelect<AMProdItem, AMProdMatl,
                    Where<AMProdMatl.orderType, Equal<Current<AMProdOper.orderType>>, And<AMProdMatl.prodOrdID, Equal<Current<AMProdOper.prodOrdID>>, And<AMProdMatl.operationID, Equal<Current<AMProdOper.operationID>>>>>,
                    OrderBy<Asc<AMProdMatl.inventoryID, Asc<AMProdMatl.sortOrder, Asc<AMProdMatl.lineID>>>>> ProdMatlRecords;
        #endregion

    }
}