using LUMCustomizations.Library;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.FA
{
    public class TransactionEntry_Extension : PXGraphExtension<TransactionEntry>
    {
        /// <summary> 更新Description 如果資料來源為:FA502000 Origin: Depreciation </summary>
        public virtual void _ (Events.RowPersisting<FARegister> e)
        {
            var row = e.Row;
            if(new LumLibrary().GetJournalEnhance && 
               row.CreatedByScreenID == "FA502000" && 
               string.IsNullOrEmpty(row.DocDesc) &&
               row.Origin == "D")
                row.DocDesc = $"Depreciate {row.FinPeriodID.Substring(4,2)} / {row.FinPeriodID.Substring(0,4)}";
        }
    }
}
