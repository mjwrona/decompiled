// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.HtmlFromXamlConverter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal static class HtmlFromXamlConverter
  {
    internal static string ConvertXamlToHtml(string xamlString)
    {
      string empty = string.Empty;
      using (XmlTextReader safeXmlTextReader = XmlUtility.CreateSafeXmlTextReader((TextReader) new StringReader(xamlString)))
      {
        StringBuilder sb = new StringBuilder();
        using (XmlTextWriter htmlWriter = new XmlTextWriter((TextWriter) new StringWriter(sb)))
        {
          if (!HtmlFromXamlConverter.WriteFlowDocument(safeXmlTextReader, htmlWriter))
            return string.Empty;
        }
        return sb.ToString();
      }
    }

    private static bool WriteFlowDocument(XmlTextReader xamlReader, XmlTextWriter htmlWriter)
    {
      if (!HtmlFromXamlConverter.ReadNextToken((XmlReader) xamlReader) || xamlReader.NodeType != XmlNodeType.Element || !string.Equals(xamlReader.Name, "FlowDocument", StringComparison.OrdinalIgnoreCase))
        return false;
      StringBuilder inlineStyle = new StringBuilder();
      htmlWriter.WriteStartElement("DIV");
      HtmlFromXamlConverter.WriteElementProperties(xamlReader, htmlWriter, inlineStyle);
      HtmlFromXamlConverter.WriteElementContent(xamlReader, htmlWriter, inlineStyle);
      htmlWriter.WriteEndElement();
      return true;
    }

    private static void WriteElementProperties(
      XmlTextReader xamlReader,
      XmlTextWriter htmlWriter,
      StringBuilder inlineStyle)
    {
      inlineStyle.Remove(0, inlineStyle.Length);
      if (!xamlReader.HasAttributes)
        return;
      bool flag = false;
      while (xamlReader.MoveToNextAttribute())
      {
        string str = (string) null;
        string name = xamlReader.Name;
        if (name != null)
        {
          switch (name.Length)
          {
            case 5:
              if (name == "Width")
              {
                str = "width:" + xamlReader.Value + ";";
                break;
              }
              break;
            case 6:
              if (name == "Margin")
              {
                str = "margin:" + HtmlFromXamlConverter.ParseXamlThickness(xamlReader.Value) + ";";
                break;
              }
              break;
            case 7:
              switch (name[0])
              {
                case 'P':
                  if (name == "Padding")
                  {
                    str = "padding:" + HtmlFromXamlConverter.ParseXamlThickness(xamlReader.Value) + ";";
                    break;
                  }
                  break;
                case 'R':
                  if (name == "RowSpan")
                  {
                    htmlWriter.WriteAttributeString("ROWSPAN", xamlReader.Value);
                    break;
                  }
                  break;
              }
              break;
            case 8:
              switch (name[6])
              {
                case 'i':
                  if (name == "Emphasis")
                    break;
                  break;
                case 'l':
                  if (name == "Capitals")
                    break;
                  break;
                case 'n':
                  if (name == "xml:lang")
                  {
                    htmlWriter.WriteAttributeString("xml:lang", xamlReader.Value);
                    break;
                  }
                  break;
                case 'o':
                  if (name == "Fraction")
                    break;
                  break;
                case 't':
                  if (name == "Variants")
                    break;
                  break;
                case 'z':
                  if (name == "FontSize")
                  {
                    str = !int.TryParse(xamlReader.Value.Trim(), out int _) ? "font-size:" + xamlReader.Value + ";" : "font-size:" + xamlReader.Value + "px;";
                    break;
                  }
                  break;
              }
              break;
            case 9:
              if (name == "FontStyle")
              {
                str = "font-style:" + xamlReader.Value.ToLowerInvariant() + ";";
                break;
              }
              break;
            case 10:
              switch (name[4])
              {
                case 'F':
                  if (name == "FontFamily")
                  {
                    str = "font-family:" + xamlReader.Value + ";";
                    break;
                  }
                  break;
                case 'H':
                  if (name == "LineHeight")
                    break;
                  break;
                case 'I':
                  if (name == "TextIndent")
                  {
                    str = "text-indent:" + xamlReader.Value + ";";
                    break;
                  }
                  break;
                case 'W':
                  if (name == "FontWeight")
                  {
                    str = "font-weight:" + xamlReader.Value.ToLowerInvariant() + ";";
                    break;
                  }
                  break;
                case 'g':
                  switch (name)
                  {
                    case "Background":
                      str = "background-color:" + HtmlFromXamlConverter.ParseXamlColor(xamlReader.Value) + ";";
                      break;
                    case "Foreground":
                      str = "color:" + HtmlFromXamlConverter.ParseXamlColor(xamlReader.Value) + ";";
                      break;
                  }
                  break;
                case 'm':
                  if (name == "ColumnSpan")
                  {
                    htmlWriter.WriteAttributeString("COLSPAN", xamlReader.Value);
                    break;
                  }
                  break;
              }
              break;
            case 11:
              switch (name[0])
              {
                case 'B':
                  if (name == "BorderBrush")
                  {
                    str = "border-color:" + HtmlFromXamlConverter.ParseXamlColor(xamlReader.Value) + ";";
                    flag = true;
                    break;
                  }
                  break;
                case 'F':
                  if (name == "FontStretch")
                    break;
                  break;
                case 'N':
                  if (name == "NavigateUri")
                  {
                    htmlWriter.WriteAttributeString("href", xamlReader.Value);
                    break;
                  }
                  break;
                case 'T':
                  if (name == "TextEffects")
                    break;
                  break;
              }
              break;
            case 13:
              switch (name[0])
              {
                case 'F':
                  if (name == "FlowDirection")
                  {
                    str = "dir:" + HtmlFromXamlConverter.GetFlowDirection(xamlReader.Value) + ";";
                    break;
                  }
                  break;
                case 'T':
                  if (name == "TextAlignment")
                  {
                    str = "text-align:" + xamlReader.Value + ";";
                    break;
                  }
                  break;
              }
              break;
            case 14:
              switch (name[6])
              {
                case 'T':
                  if (name == "IsKeptTogether")
                    break;
                  break;
                case 'W':
                  if (name == "IsKeptWithNext")
                    break;
                  break;
              }
              break;
            case 15:
              switch (name[0])
              {
                case 'B':
                  if (name == "BorderThickness")
                  {
                    str = "border-width:" + HtmlFromXamlConverter.ParseXamlThickness(xamlReader.Value) + ";";
                    flag = true;
                    break;
                  }
                  break;
                case 'P':
                  if (name == "PageBreakBefore")
                    break;
                  break;
                case 'T':
                  if (name == "TextDecorations")
                  {
                    str = "text-decoration:underline;";
                    break;
                  }
                  break;
              }
              break;
            case 17:
              switch (name[0])
              {
                case 'C':
                  if (name == "ColumnBreakBefore")
                    break;
                  break;
                case 'S':
                  if (name == "StandardLigatures")
                    break;
                  break;
              }
              break;
          }
        }
        if (str != null)
          inlineStyle.Append(str);
      }
      if (flag)
        inlineStyle.Append("border-style:solid;mso-element:para-border-div;");
      xamlReader.MoveToElement();
    }

    private static string GetFlowDirection(string flowDirection)
    {
      if (string.Equals(flowDirection, "RightToLeft", StringComparison.OrdinalIgnoreCase))
        return "rtl";
      string.Equals(flowDirection, "LeftToRight", StringComparison.OrdinalIgnoreCase);
      return "ltr";
    }

    private static string ParseXamlColor(string color)
    {
      if (color.StartsWith("#") && color.Length > 7)
        color = "#" + color.Substring(3);
      return color;
    }

    private static string ParseXamlThickness(string thickness)
    {
      string[] strArray = thickness.Split(',');
      for (int index = 0; index < strArray.Length; ++index)
      {
        double result;
        strArray[index] = !double.TryParse(strArray[index], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? "1" : Math.Ceiling(result).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
      string xamlThickness;
      switch (strArray.Length)
      {
        case 1:
          xamlThickness = thickness;
          break;
        case 2:
          xamlThickness = strArray[1] + " " + strArray[0];
          break;
        case 4:
          xamlThickness = strArray[1] + " " + strArray[2] + " " + strArray[3] + " " + strArray[0];
          break;
        default:
          xamlThickness = strArray[0];
          break;
      }
      return xamlThickness;
    }

    private static void WriteElementContent(
      XmlTextReader xamlReader,
      XmlTextWriter htmlWriter,
      StringBuilder inlineStyle)
    {
      bool flag1 = false;
      if (xamlReader.IsEmptyElement)
      {
        if (htmlWriter != null && !flag1 && inlineStyle.Length > 0)
        {
          htmlWriter.WriteAttributeString("STYLE", inlineStyle.ToString());
          inlineStyle.Remove(0, inlineStyle.Length);
        }
      }
      else
      {
        bool flag2 = false;
        while (flag2 || HtmlFromXamlConverter.ReadNextToken((XmlReader) xamlReader) && xamlReader.NodeType != XmlNodeType.EndElement)
        {
          flag2 = false;
          switch (xamlReader.NodeType)
          {
            case XmlNodeType.Element:
              if (xamlReader.Name.Contains("."))
              {
                HtmlFromXamlConverter.AddComplexProperty(xamlReader, inlineStyle);
                flag2 = true;
                continue;
              }
              if (htmlWriter != null && !flag1 && inlineStyle.Length > 0)
              {
                htmlWriter.WriteAttributeString("STYLE", inlineStyle.ToString());
                inlineStyle.Remove(0, inlineStyle.Length);
              }
              flag1 = true;
              HtmlFromXamlConverter.WriteElement(xamlReader, htmlWriter, inlineStyle);
              continue;
            case XmlNodeType.Text:
            case XmlNodeType.CDATA:
            case XmlNodeType.SignificantWhitespace:
              if (htmlWriter != null)
              {
                if (!flag1 && inlineStyle.Length > 0)
                  htmlWriter.WriteAttributeString("STYLE", inlineStyle.ToString());
                htmlWriter.WriteString(xamlReader.Value);
              }
              flag1 = true;
              continue;
            case XmlNodeType.Comment:
              if (htmlWriter != null)
              {
                if (!flag1 && inlineStyle.Length > 0)
                  htmlWriter.WriteAttributeString("STYLE", inlineStyle.ToString());
                htmlWriter.WriteComment(xamlReader.Value);
              }
              flag1 = true;
              continue;
            default:
              continue;
          }
        }
      }
    }

    private static void AddComplexProperty(XmlTextReader xamlReader, StringBuilder inlineStyle)
    {
      if (inlineStyle != null && xamlReader.Name.EndsWith(".TextDecorations"))
      {
        if (xamlReader.ReadInnerXml().IndexOf("Underline", 0, StringComparison.OrdinalIgnoreCase) < 0)
          return;
        inlineStyle.Append("text-decoration:underline;");
      }
      else
        xamlReader.ReadInnerXml();
    }

    private static void WriteElement(
      XmlTextReader xamlReader,
      XmlTextWriter htmlWriter,
      StringBuilder inlineStyle)
    {
      if (htmlWriter == null)
      {
        HtmlFromXamlConverter.WriteElementContent(xamlReader, (XmlTextWriter) null, (StringBuilder) null);
      }
      else
      {
        string name = xamlReader.Name;
        string localName;
        if (name != null)
        {
          switch (name.Length)
          {
            case 3:
              if (name == "Run")
                break;
              goto label_40;
            case 4:
              switch (name[0])
              {
                case 'B':
                  if (name == "Bold")
                  {
                    localName = "B";
                    goto label_41;
                  }
                  else
                    goto label_40;
                case 'L':
                  if (name == "List")
                  {
                    string attribute = xamlReader.GetAttribute("MarkerStyle");
                    localName = string.IsNullOrEmpty(attribute) || string.Equals(attribute, "None", StringComparison.OrdinalIgnoreCase) || string.Equals(attribute, "Disc", StringComparison.OrdinalIgnoreCase) || string.Equals(attribute, "Circle", StringComparison.OrdinalIgnoreCase) || string.Equals(attribute, "Square", StringComparison.OrdinalIgnoreCase) || string.Equals(attribute, "Box", StringComparison.OrdinalIgnoreCase) ? "UL" : "OL";
                    goto label_41;
                  }
                  else
                    goto label_40;
                case 'S':
                  if (name == "Span")
                    break;
                  goto label_40;
                default:
                  goto label_40;
              }
              break;
            case 5:
              if (name == "Table")
              {
                localName = "TABLE";
                goto label_41;
              }
              else
                goto label_40;
            case 6:
              if (name == "Italic")
              {
                localName = "I";
                goto label_41;
              }
              else
                goto label_40;
            case 7:
              if (name == "Section")
              {
                localName = "DIV";
                goto label_41;
              }
              else
                goto label_40;
            case 8:
              switch (name[0])
              {
                case 'L':
                  if (name == "ListItem")
                  {
                    localName = "LI";
                    goto label_41;
                  }
                  else
                    goto label_40;
                case 'T':
                  if (name == "TableRow")
                  {
                    localName = "TR";
                    goto label_41;
                  }
                  else
                    goto label_40;
                default:
                  goto label_40;
              }
            case 9:
              switch (name[0])
              {
                case 'H':
                  if (name == "Hyperlink")
                  {
                    localName = "A";
                    goto label_41;
                  }
                  else
                    goto label_40;
                case 'L':
                  if (name == "LineBreak")
                  {
                    localName = "BR";
                    goto label_41;
                  }
                  else
                    goto label_40;
                case 'P':
                  if (name == "Paragraph")
                  {
                    localName = "P";
                    goto label_41;
                  }
                  else
                    goto label_40;
                case 'T':
                  if (name == "TableCell")
                  {
                    localName = "TD";
                    goto label_41;
                  }
                  else
                    goto label_40;
                default:
                  goto label_40;
              }
            case 11:
              if (name == "TableColumn")
              {
                localName = "COL";
                goto label_41;
              }
              else
                goto label_40;
            case 13:
              if (name == "TableRowGroup")
              {
                localName = "TBODY";
                goto label_41;
              }
              else
                goto label_40;
            case 16:
              if (name == "BlockUIContainer")
              {
                localName = "DIV";
                goto label_41;
              }
              else
                goto label_40;
            case 17:
              if (name == "InlineUIContainer")
              {
                localName = "SPAN";
                goto label_41;
              }
              else
                goto label_40;
            default:
              goto label_40;
          }
          localName = "SPAN";
          goto label_41;
        }
label_40:
        localName = (string) null;
label_41:
        if (htmlWriter != null && localName != null)
        {
          htmlWriter.WriteStartElement(localName);
          HtmlFromXamlConverter.WriteElementProperties(xamlReader, htmlWriter, inlineStyle);
          HtmlFromXamlConverter.WriteElementContent(xamlReader, htmlWriter, inlineStyle);
          htmlWriter.WriteEndElement();
        }
        else
          HtmlFromXamlConverter.WriteElementContent(xamlReader, (XmlTextWriter) null, (StringBuilder) null);
      }
    }

    private static bool ReadNextToken(XmlReader xamlReader)
    {
      while (xamlReader.Read())
      {
        switch (xamlReader.NodeType)
        {
          case XmlNodeType.None:
          case XmlNodeType.Element:
          case XmlNodeType.Text:
          case XmlNodeType.CDATA:
          case XmlNodeType.SignificantWhitespace:
          case XmlNodeType.EndElement:
            return true;
          case XmlNodeType.Comment:
            return true;
          case XmlNodeType.Whitespace:
            if (xamlReader.XmlSpace == XmlSpace.Preserve)
              return true;
            continue;
          default:
            continue;
        }
      }
      return false;
    }
  }
}
