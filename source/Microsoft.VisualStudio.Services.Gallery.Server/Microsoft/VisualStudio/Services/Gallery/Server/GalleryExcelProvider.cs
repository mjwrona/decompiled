// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryExcelProvider
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Data;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class GalleryExcelProvider : IFileContentProvider
  {
    public byte[] GetFileContent(DataSet tableSet)
    {
      ArgumentUtility.CheckForNull<DataSet>(tableSet, nameof (tableSet));
      using (MemoryStream memoryStream = new MemoryStream())
      {
        SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create((Stream) memoryStream, (SpreadsheetDocumentType) 0);
        WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();
        ((OpenXmlPartContainer) workbookPart).AddNewPart<WorkbookStylesPart>();
        workbookPart.WorkbookStylesPart.Stylesheet = this.GetNewStylesheet();
        uint num = 1;
        spreadsheetDocument.WorkbookPart.Workbook.Sheets = new Sheets();
        Sheets firstChild = ((OpenXmlElement) spreadsheetDocument.WorkbookPart.Workbook).GetFirstChild<Sheets>();
        foreach (DataTable table in (InternalDataCollectionBase) tableSet.Tables)
        {
          WorksheetPart worksheetPart = ((OpenXmlPartContainer) workbookPart).AddNewPart<WorksheetPart>();
          Worksheet worksheet = new Worksheet();
          Sheet sheet = new Sheet()
          {
            Id = StringValue.op_Implicit(((OpenXmlPartContainer) spreadsheetDocument.WorkbookPart).GetIdOfPart((OpenXmlPart) worksheetPart)),
            SheetId = UInt32Value.op_Implicit(num),
            Name = StringValue.op_Implicit(table.TableName)
          };
          ((OpenXmlElement) firstChild).Append(new OpenXmlElement[1]
          {
            (OpenXmlElement) sheet
          });
          ((OpenXmlElement) worksheet).Append(new OpenXmlElement[1]
          {
            (OpenXmlElement) this.GetFixedSizedColumns(table)
          });
          SheetData dataFromTableData = this.GetSheetDataFromTableData(table);
          ((OpenXmlElement) worksheet).Append(new OpenXmlElement[1]
          {
            (OpenXmlElement) dataFromTableData
          });
          worksheetPart.Worksheet = worksheet;
          ++num;
        }
        ((OpenXmlPartRootElement) workbookPart.Workbook).Save();
        ((OpenXmlPackage) spreadsheetDocument).Close();
        return memoryStream.ToArray();
      }
    }

    private Stylesheet GetNewStylesheet()
    {
      Stylesheet newStylesheet = new Stylesheet();
      Font font = new Font()
      {
        FontSize = new FontSize()
        {
          Val = DoubleValue.op_Implicit(11.0)
        },
        FontName = new FontName()
        {
          Val = StringValue.op_Implicit("Calibri")
        },
        FontFamilyNumbering = new FontFamilyNumbering()
        {
          Val = Int32Value.op_Implicit(2)
        },
        FontScheme = new FontScheme()
        {
          Val = new EnumValue<FontSchemeValues>((FontSchemeValues) 2)
        }
      };
      ((OpenXmlElement) newStylesheet).Append(new OpenXmlElement[1]
      {
        (OpenXmlElement) new Fonts(new OpenXmlElement[1]
        {
          (OpenXmlElement) font
        })
      });
      ((OpenXmlElement) newStylesheet).Append(new OpenXmlElement[1]
      {
        (OpenXmlElement) new Fills(new OpenXmlElement[1]
        {
          (OpenXmlElement) new Fill()
          {
            PatternFill = new PatternFill()
            {
              PatternType = new EnumValue<PatternValues>((PatternValues) 0)
            }
          }
        })
      });
      Border border = new Border()
      {
        LeftBorder = new LeftBorder(),
        RightBorder = new RightBorder(),
        TopBorder = new TopBorder(),
        BottomBorder = new BottomBorder(),
        DiagonalBorder = new DiagonalBorder()
      };
      ((OpenXmlElement) newStylesheet).Append(new OpenXmlElement[1]
      {
        (OpenXmlElement) new Borders(new OpenXmlElement[1]
        {
          (OpenXmlElement) border
        })
      });
      CellFormats cellFormats = new CellFormats();
      CellFormat cellFormat1 = new CellFormat()
      {
        NumberFormatId = UInt32Value.op_Implicit(0U),
        FormatId = UInt32Value.op_Implicit(0U),
        FontId = UInt32Value.op_Implicit(0U),
        BorderId = UInt32Value.op_Implicit(0U),
        FillId = UInt32Value.op_Implicit(0U)
      };
      ((OpenXmlElement) cellFormats).Append(new OpenXmlElement[1]
      {
        (OpenXmlElement) cellFormat1
      });
      CellFormat cellFormat2 = new CellFormat()
      {
        NumberFormatId = UInt32Value.op_Implicit(14U),
        FormatId = UInt32Value.op_Implicit(0U),
        FontId = UInt32Value.op_Implicit(0U),
        BorderId = UInt32Value.op_Implicit(0U),
        FillId = UInt32Value.op_Implicit(0U),
        ApplyNumberFormat = BooleanValue.FromBoolean(true)
      };
      ((OpenXmlElement) cellFormats).Append(new OpenXmlElement[1]
      {
        (OpenXmlElement) cellFormat2
      });
      CellFormat cellFormat3 = new CellFormat()
      {
        NumberFormatId = UInt32Value.op_Implicit(22U),
        FormatId = UInt32Value.op_Implicit(0U),
        FontId = UInt32Value.op_Implicit(0U),
        BorderId = UInt32Value.op_Implicit(0U),
        FillId = UInt32Value.op_Implicit(0U),
        ApplyNumberFormat = BooleanValue.FromBoolean(true)
      };
      ((OpenXmlElement) cellFormats).Append(new OpenXmlElement[1]
      {
        (OpenXmlElement) cellFormat3
      });
      ((OpenXmlElement) newStylesheet).Append(new OpenXmlElement[1]
      {
        (OpenXmlElement) cellFormats
      });
      return newStylesheet;
    }

    private Columns GetFixedSizedColumns(DataTable table)
    {
      Columns fixedSizedColumns = new Columns();
      uint num = 1;
      foreach (DataColumn column1 in (InternalDataCollectionBase) table.Columns)
      {
        Column column2 = new Column()
        {
          BestFit = BooleanValue.op_Implicit(true),
          Min = UInt32Value.op_Implicit(num),
          Max = UInt32Value.op_Implicit(num),
          CustomWidth = BooleanValue.op_Implicit(true),
          Width = DoubleValue.op_Implicit(Convert.ToDouble(column1.ExtendedProperties[(object) "columnWidth"], (IFormatProvider) CultureInfo.InvariantCulture))
        };
        ((OpenXmlElement) fixedSizedColumns).Append(new OpenXmlElement[1]
        {
          (OpenXmlElement) column2
        });
        ++num;
      }
      return fixedSizedColumns;
    }

    private SheetData GetSheetDataFromTableData(DataTable table)
    {
      SheetData dataFromTableData = new SheetData();
      Row row1 = new Row();
      bool result = false;
      if (table.ExtendedProperties.ContainsKey((object) "useShortDateFormat"))
        bool.TryParse(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", table.ExtendedProperties[(object) "useShortDateFormat"]), out result);
      foreach (DataColumn column in (InternalDataCollectionBase) table.Columns)
      {
        Cell cell = new Cell();
        ((CellType) cell).DataType = EnumValue<CellValues>.op_Implicit((CellValues) 4);
        ((CellType) cell).CellValue = new CellValue(column.ColumnName);
        ((OpenXmlCompositeElement) row1).AppendChild<Cell>(cell);
      }
      ((OpenXmlCompositeElement) dataFromTableData).AppendChild<Row>(row1);
      foreach (DataRow row2 in (InternalDataCollectionBase) table.Rows)
      {
        Row row3 = new Row();
        foreach (DataColumn column in (InternalDataCollectionBase) table.Columns)
        {
          Cell cell = new Cell();
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", row2[column]);
          ((CellType) cell).DataType = this.GetCellDataType(column);
          if (EnumValue<CellValues>.op_Implicit(((CellType) cell).DataType) == 6)
          {
            DateTime dateTime = (DateTime) row2[column];
            if (dateTime != new DateTime())
            {
              str = dateTime.ToString("s");
              ((CellType) cell).StyleIndex = result ? UInt32Value.FromUInt32(1U) : UInt32Value.FromUInt32(2U);
            }
            else
              str = string.Empty;
          }
          ((CellType) cell).CellValue = new CellValue(str);
          ((OpenXmlCompositeElement) row3).AppendChild<Cell>(cell);
        }
        ((OpenXmlCompositeElement) dataFromTableData).AppendChild<Row>(row3);
      }
      return dataFromTableData;
    }

    private EnumValue<CellValues> GetCellDataType(DataColumn column)
    {
      CellValues cellValues = (CellValues) 4;
      Type dataType = column.DataType;
      if (dataType == typeof (byte) || dataType == typeof (short) || dataType == typeof (int) || dataType == typeof (long) || dataType == typeof (int) || dataType == typeof (long) || dataType == typeof (float) || dataType == typeof (double))
        cellValues = (CellValues) 1;
      else if (dataType == typeof (DateTime))
        cellValues = (CellValues) 6;
      return EnumValue<CellValues>.op_Implicit(cellValues);
    }
  }
}
