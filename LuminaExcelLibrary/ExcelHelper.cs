using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuminaExcelLibrary
{
    public class ExcelHelper
    {
        public void CreateBlankCell(ISheet sheet, int rowNum, int startCell, int endCell, ICellStyle style = null)
        {
            for (int i = startCell; i <= endCell; i++)
            {
                sheet.GetRow(rowNum).CreateCell(i);
                if (style != null)
                    sheet.GetRow(rowNum).GetCell(i).CellStyle = style;
            }
        }

        public void AutoSizeColumn(ISheet sheet, int rowOffset)
        {
            int numberOfColum = sheet.GetRow(rowOffset).PhysicalNumberOfCells;
            for(int i=1;i<= numberOfColum;i++)
            {
                sheet.AutoSizeColumn(i);
                GC.Collect();
            }
        }

    }
}
