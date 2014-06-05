﻿using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SKBKontur.Catalogue.ExcelFileGenerator
{
    internal interface IExcelDocumentStyle
    {
        void Save();
        uint SaveStyle(ExcelCellStyle style);
    }

    internal class ExcelDocumentStyle : IExcelDocumentStyle
    {
        public ExcelDocumentStyle(Stylesheet stylesheet)
        {
            this.stylesheet = stylesheet;
            numberingFormats = new ExcelDocumentNumberingFormats(stylesheet);
            fillStyles = new ExcelDocumentFillStyles(stylesheet);
            bordersStyles = new ExcelDocumentBordersStyles(stylesheet);
        }

        public void Save()
        {
            stylesheet.Save();
        }

        public uint SaveStyle(ExcelCellStyle style)
        {
            var fillId = fillStyles.AddStyle(style.FillStyle);
            var borderId = bordersStyles.AddStyle(style.BordersStyle);
            var numberFormatId = numberingFormats.AddFormat(style.NumberingFormat);
            var styleFormatId = stylesheet.CellFormats.Count.Value;
            stylesheet.CellFormats.Count++;
            stylesheet.CellFormats.AppendChild(new CellFormat
                {
                    NumberFormatId = numberFormatId,
                    FormatId = 0,
                    FontId = 0,
                    FillId = fillId,
                    BorderId = borderId,
                    ApplyFill = fillId == 0 ? null : new BooleanValue(true),
                    ApplyBorder = borderId == 0 ? null : new BooleanValue(true),
                    ApplyNumberFormat = numberFormatId == 0 ? null : new BooleanValue(true)
                });
            return styleFormatId;
        }

        private readonly Stylesheet stylesheet;
        private readonly ExcelDocumentNumberingFormats numberingFormats;
        private readonly ExcelDocumentFillStyles fillStyles;
        private readonly ExcelDocumentBordersStyles bordersStyles;
    }
}