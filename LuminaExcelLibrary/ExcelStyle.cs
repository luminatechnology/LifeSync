using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuminaExcelLibrary
{
    public class ExcelStyle
    {
        protected IWorkbook _workBook;
        protected ICellStyle _style;
        protected IFont _font;

        public ExcelStyle(XSSFWorkbook workBook)
        {
            this._workBook = workBook;
            this._style = this._workBook.CreateCellStyle();
            this._font = this._workBook.CreateFont();
        }

        public ExcelStyle(HSSFWorkbook workBook)
        {
            this._workBook = workBook;
            this._style = this._workBook.CreateCellStyle();
            this._font = this._workBook.CreateFont();
        }

        /// <summary>
        /// Alignment = HorizontalAlignment.Center;
        /// VerticalAlignment = VerticalAlignment.Center
        /// FontName = Calibri
        /// FontHeightInPoints = 20(Default)
        /// Boldweight = FontBoldWeight.Bold(Default: false)
        /// </summary>
        public virtual (ICellStyle ExcelStyle, IFont ExcelFont) HeaderStyle(int FontSize = 20, bool Bold = false, bool Border = false)
        {
            this._style.Alignment = HorizontalAlignment.Center;
            this._style.VerticalAlignment = VerticalAlignment.Center;
            this._font.FontName = "Calibri";
            this._font.FontHeightInPoints = FontSize;
            if (Bold)
                this._font.Boldweight = (short)FontBoldWeight.Bold;
            if (Border)
                this._style = AddBorder(_style);
            this._style.SetFont(this._font);
            return (_style, _font);
        }

        /// <summary>
        /// Alignment = HorizontalAlignment.Left
        /// VerticalAlignment = VerticalAlignment.Center
        /// FontName = "Calibri"
        /// FontHeightInPoints = 12(Default)
        /// </summary>
        public virtual (ICellStyle ExcelStyle, IFont ExcelFont) NormalStyle(int FontSize = 12, bool Bold = false, bool Border = false)
        {
            this._style.Alignment = HorizontalAlignment.Left;
            this._style.VerticalAlignment = VerticalAlignment.Center;
            this._font.FontName = "Calibri";
            this._font.FontHeightInPoints = FontSize;
            this._style.SetFont(this._font);
            if (Bold)
                this._font.Boldweight = (short)FontBoldWeight.Bold;
            if (Border)
                this._style = AddBorder(_style);
            return (_style, _font);
        }

        /// <summary>
        /// Alignment = HorizontalAlignment.Center
        /// VerticalAlignment = VerticalAlignment.Center
        /// FontName = "Calibri"
        /// FontHeightInPoints = 12(Default)
        /// </summary>
        public virtual (ICellStyle ExcelStyle, IFont ExcelFont) TableHeaderStyle(int FontSize = 12, bool Bold = false, bool Border = false)
        {
            this._style.Alignment = HorizontalAlignment.Center;
            this._style.VerticalAlignment = VerticalAlignment.Center;
            this._font.FontName = "Calibri";
            this._font.FontHeightInPoints = FontSize;
            this._style.SetFont(this._font);
            if (Bold)
                this._font.Boldweight = (short)FontBoldWeight.Bold;
            if (Border)
                this._style = AddBorder(_style);
            return (_style, _font);
        }

        /// <summary> Set Cell Color(IndexedColors.Color) </summary>
        public virtual (ICellStyle ExcelStyle, IFont ExcelFont) SetCellColor(short colorIndex, ICellStyle myStyle = null)
        {
            this._style = this._workBook.CreateCellStyle();
            if (myStyle != null)
                this._style  = myStyle;
            this._style.FillForegroundColor = colorIndex;
            this._style.FillPattern = FillPattern.SolidForeground;
            this._style.SetFont(this._font);
            return (_style, _font);
        }

        public ICellStyle AddBorder(ICellStyle style)
        {
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            return style;
        }

    }
}
