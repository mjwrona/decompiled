// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.HtmlToXamlConverter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal static class HtmlToXamlConverter
  {
    private static XmlElement InlineFragmentParentElement;
    public const string Xaml_FlowDocument = "FlowDocument";
    public const string Xaml_Run = "Run";
    public const string Xaml_Span = "Span";
    public const string Xaml_Hyperlink = "Hyperlink";
    public const string Xaml_Hyperlink_NavigateUri = "NavigateUri";
    public const string Xaml_Hyperlink_TargetName = "TargetName";
    public const string Xaml_Section = "Section";
    public const string Xaml_List = "List";
    public const string Xaml_List_MarkerStyle = "MarkerStyle";
    public const string Xaml_List_MarkerStyle_None = "None";
    public const string Xaml_List_MarkerStyle_Decimal = "Decimal";
    public const string Xaml_List_MarkerStyle_Disc = "Disc";
    public const string Xaml_List_MarkerStyle_Circle = "Circle";
    public const string Xaml_List_MarkerStyle_Square = "Square";
    public const string Xaml_List_MarkerStyle_Box = "Box";
    public const string Xaml_List_MarkerStyle_LowerLatin = "LowerLatin";
    public const string Xaml_List_MarkerStyle_UpperLatin = "UpperLatin";
    public const string Xaml_List_MarkerStyle_LowerRoman = "LowerRoman";
    public const string Xaml_List_MarkerStyle_UpperRoman = "UpperRoman";
    public const string Xaml_ListItem = "ListItem";
    public const string Xaml_LineBreak = "LineBreak";
    public const string Xaml_Paragraph = "Paragraph";
    public const string Xaml_Margin = "Margin";
    public const string Xaml_Padding = "Padding";
    public const string Xaml_BorderBrush = "BorderBrush";
    public const string Xaml_BorderThickness = "BorderThickness";
    public const string Xaml_Table = "Table";
    public const string Xaml_TableColumns = "Table.Columns";
    public const string Xaml_TableColumn = "TableColumn";
    public const string Xaml_TableRowGroup = "TableRowGroup";
    public const string Xaml_TableRow = "TableRow";
    public const string Xaml_TableCell = "TableCell";
    public const string Xaml_TableCell_BorderThickness = "BorderThickness";
    public const string Xaml_TableCell_BorderBrush = "BorderBrush";
    public const string Xaml_TableCell_ColumnSpan = "ColumnSpan";
    public const string Xaml_TableCell_RowSpan = "RowSpan";
    public const string Xaml_Width = "Width";
    public const string Xaml_Brushes_Black = "Black";
    public const string Xaml_FontFamily = "FontFamily";
    public const string Xaml_FontSize = "FontSize";
    public const string Xaml_FontSize_XXLarge = "22pt";
    public const string Xaml_FontSize_XLarge = "20pt";
    public const string Xaml_FontSize_Large = "18pt";
    public const string Xaml_FontSize_Medium = "16pt";
    public const string Xaml_FontSize_Small = "12pt";
    public const string Xaml_FontSize_XSmall = "10pt";
    public const string Xaml_FontSize_XXSmall = "8pt";
    public const string Xaml_FontWeight = "FontWeight";
    public const string Xaml_FontWeight_Bold = "Bold";
    public const string Xaml_FontStyle = "FontStyle";
    public const string Xaml_Foreground = "Foreground";
    public const string Xaml_Background = "Background";
    public const string Xaml_TextDecorations = "TextDecorations";
    public const string Xaml_TextDecorations_Underline = "Underline";
    public const string Xaml_TextIndent = "TextIndent";
    public const string Xaml_TextAlignment = "TextAlignment";
    public const string Xaml_FlowDirection = "FlowDirection";
    private static string _xamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

    public static string ConvertHtmlToXaml(string htmlString, bool asFlowDocument)
    {
      XmlElement html = HtmlParser.ParseHtml(htmlString);
      XmlElement xmlElement = new XmlDocument().CreateElement((string) null, asFlowDocument ? "FlowDocument" : "Section", HtmlToXamlConverter._xamlNamespace);
      CssStylesheet stylesheet = new CssStylesheet(html);
      List<XmlElement> sourceContext = new List<XmlElement>();
      HtmlToXamlConverter.InlineFragmentParentElement = (XmlElement) null;
      HtmlToXamlConverter.AddBlock(xmlElement, (XmlNode) html, new Hashtable(), stylesheet, sourceContext);
      if (!asFlowDocument)
        xmlElement = HtmlToXamlConverter.ExtractInlineFragment(xmlElement);
      xmlElement.SetAttribute("xml:space", "preserve");
      return xmlElement.OuterXml;
    }

    public static string GetAttribute(XmlElement element, string attributeName)
    {
      for (int i = 0; i < element.Attributes.Count; ++i)
      {
        if (string.Equals(element.Attributes[i].Name, attributeName, StringComparison.OrdinalIgnoreCase))
          return element.Attributes[i].Value;
      }
      return (string) null;
    }

    internal static string UnQuote(string value)
    {
      if (value.StartsWith("\"") && value.EndsWith("\"") || value.StartsWith("'") && value.EndsWith("'"))
        value = value.Substring(1, value.Length - 2).Trim();
      return value;
    }

    private static XmlNode AddBlock(
      XmlElement xamlParentElement,
      XmlNode htmlNode,
      Hashtable inheritedProperties,
      CssStylesheet stylesheet,
      List<XmlElement> sourceContext)
    {
      switch (htmlNode)
      {
        case XmlComment _:
          HtmlToXamlConverter.DefineInlineFragmentParent((XmlComment) htmlNode, (XmlElement) null);
          break;
        case XmlText _:
          htmlNode = HtmlToXamlConverter.AddImplicitParagraph(xamlParentElement, htmlNode, inheritedProperties, stylesheet, sourceContext);
          break;
        case XmlElement _:
          XmlElement xmlElement = (XmlElement) htmlNode;
          string localName = xmlElement.LocalName;
          if (xmlElement.NamespaceURI != "http://www.w3.org/1999/xhtml")
            return (XmlNode) xmlElement;
          sourceContext.Add(xmlElement);
          string lowerInvariant = localName.ToLowerInvariant();
          if (lowerInvariant != null)
          {
            switch (lowerInvariant.Length)
            {
              case 1:
                if (lowerInvariant == "p")
                  goto label_50;
                else
                  goto label_55;
              case 2:
                switch (lowerInvariant[1])
                {
                  case '1':
                    if (lowerInvariant == "h1")
                      goto label_50;
                    else
                      goto label_55;
                  case '2':
                    if (lowerInvariant == "h2")
                      goto label_50;
                    else
                      goto label_55;
                  case '3':
                    if (lowerInvariant == "h3")
                      goto label_50;
                    else
                      goto label_55;
                  case '4':
                    if (lowerInvariant == "h4")
                      goto label_50;
                    else
                      goto label_55;
                  case '5':
                    if (lowerInvariant == "h5")
                      goto label_50;
                    else
                      goto label_55;
                  case '6':
                    if (lowerInvariant == "h6")
                      goto label_50;
                    else
                      goto label_55;
                  case 'd':
                    switch (lowerInvariant)
                    {
                      case "dd":
                        goto label_50;
                      default:
                        goto label_55;
                    }
                  case 'h':
                    if (lowerInvariant == "th")
                      goto label_55;
                    else
                      goto label_55;
                  case 'i':
                    if (lowerInvariant == "li")
                    {
                      htmlNode = (XmlNode) HtmlToXamlConverter.AddOrphanListItems(xamlParentElement, xmlElement, inheritedProperties, stylesheet, sourceContext);
                      goto label_56;
                    }
                    else
                      goto label_55;
                  case 'l':
                    switch (lowerInvariant)
                    {
                      case "dl":
                        goto label_50;
                      case "ol":
                      case "ul":
                        goto label_51;
                      default:
                        goto label_55;
                    }
                  case 'r':
                    if (lowerInvariant == "tr")
                      goto label_55;
                    else
                      goto label_55;
                  case 't':
                    if (lowerInvariant == "dt" || lowerInvariant == "tt")
                      goto label_50;
                    else
                      goto label_55;
                  default:
                    goto label_55;
                }
              case 3:
                switch (lowerInvariant[2])
                {
                  case 'e':
                    if (lowerInvariant == "pre")
                      break;
                    goto label_55;
                  case 'g':
                    if (lowerInvariant == "img")
                    {
                      HtmlToXamlConverter.AddImage(xamlParentElement, xmlElement, inheritedProperties, stylesheet, sourceContext);
                      goto label_56;
                    }
                    else
                      goto label_55;
                  case 'r':
                    if (lowerInvariant == "dir")
                      goto label_51;
                    else
                      goto label_55;
                  case 'v':
                    if (lowerInvariant == "div")
                      break;
                    goto label_55;
                  default:
                    goto label_55;
                }
                break;
              case 4:
                switch (lowerInvariant[3])
                {
                  case 'a':
                    if (lowerInvariant == "meta")
                      goto label_56;
                    else
                      goto label_55;
                  case 'd':
                    if (lowerInvariant == "head")
                      goto label_56;
                    else
                      goto label_55;
                  case 'e':
                    if (lowerInvariant == "cite")
                      break;
                    goto label_55;
                  case 'l':
                    if (lowerInvariant == "html")
                      break;
                    goto label_55;
                  case 'm':
                    if (lowerInvariant == "form")
                      break;
                    goto label_55;
                  case 'u':
                    if (lowerInvariant == "menu")
                      goto label_51;
                    else
                      goto label_55;
                  case 'y':
                    if (lowerInvariant == "body")
                      break;
                    goto label_55;
                  default:
                    goto label_55;
                }
                break;
              case 5:
                switch (lowerInvariant[1])
                {
                  case 'a':
                    if (lowerInvariant == "table")
                    {
                      HtmlToXamlConverter.AddTable(xamlParentElement, xmlElement, inheritedProperties, stylesheet, sourceContext);
                      goto label_56;
                    }
                    else
                      goto label_55;
                  case 'b':
                    if (lowerInvariant == "tbody")
                      goto label_55;
                    else
                      goto label_55;
                  case 'f':
                    if (lowerInvariant == "tfoot")
                      goto label_55;
                    else
                      goto label_55;
                  case 'h':
                    if (lowerInvariant == "thead")
                      goto label_55;
                    else
                      goto label_55;
                  case 'i':
                    if (lowerInvariant == "title")
                      goto label_56;
                    else
                      goto label_55;
                  case 't':
                    if (lowerInvariant == "style")
                      goto label_56;
                    else
                      goto label_55;
                  default:
                    goto label_55;
                }
              case 6:
                switch (lowerInvariant[0])
                {
                  case 'c':
                    if (lowerInvariant == "center")
                      break;
                    goto label_55;
                  case 's':
                    if (lowerInvariant == "script")
                      goto label_56;
                    else
                      goto label_55;
                  default:
                    goto label_55;
                }
                break;
              case 7:
                if (lowerInvariant == "caption")
                  break;
                goto label_55;
              case 8:
                switch (lowerInvariant[0])
                {
                  case 'n':
                    if (lowerInvariant == "nsrtitle")
                      goto label_50;
                    else
                      goto label_55;
                  case 't':
                    if (lowerInvariant == "textarea")
                      goto label_50;
                    else
                      goto label_55;
                  default:
                    goto label_55;
                }
              case 10:
                if (lowerInvariant == "blockquote")
                  break;
                goto label_55;
              default:
                goto label_55;
            }
            HtmlToXamlConverter.AddSection(xamlParentElement, xmlElement, inheritedProperties, stylesheet, sourceContext);
            goto label_56;
label_50:
            HtmlToXamlConverter.AddParagraph(xamlParentElement, xmlElement, inheritedProperties, stylesheet, sourceContext);
            goto label_56;
label_51:
            HtmlToXamlConverter.AddList(xamlParentElement, xmlElement, inheritedProperties, stylesheet, sourceContext);
            goto label_56;
          }
label_55:
          htmlNode = HtmlToXamlConverter.AddImplicitParagraph(xamlParentElement, (XmlNode) xmlElement, inheritedProperties, stylesheet, sourceContext);
label_56:
          sourceContext.RemoveAt(sourceContext.Count - 1);
          break;
      }
      return htmlNode;
    }

    private static void AddBreak(XmlElement xamlParentElement, string htmlElementName)
    {
      XmlElement element1 = xamlParentElement.OwnerDocument.CreateElement((string) null, "LineBreak", HtmlToXamlConverter._xamlNamespace);
      xamlParentElement.AppendChild((XmlNode) element1);
      if (!string.Equals(htmlElementName, "hr", StringComparison.OrdinalIgnoreCase))
        return;
      XmlText textNode = xamlParentElement.OwnerDocument.CreateTextNode("----------------------");
      xamlParentElement.AppendChild((XmlNode) textNode);
      XmlElement element2 = xamlParentElement.OwnerDocument.CreateElement((string) null, "LineBreak", HtmlToXamlConverter._xamlNamespace);
      xamlParentElement.AppendChild((XmlNode) element2);
    }

    private static void AddSection(
      XmlElement xamlParentElement,
      XmlElement htmlElement,
      Hashtable inheritedProperties,
      CssStylesheet stylesheet,
      List<XmlElement> sourceContext)
    {
      bool flag = false;
      for (XmlNode xmlNode = htmlElement.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
      {
        if (xmlNode is XmlElement && HtmlSchema.IsBlockElement(xmlNode.LocalName.ToLowerInvariant()))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
      {
        HtmlToXamlConverter.AddParagraph(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
      }
      else
      {
        Hashtable localProperties;
        Hashtable elementProperties = HtmlToXamlConverter.GetElementProperties(htmlElement, inheritedProperties, out localProperties, stylesheet, sourceContext);
        XmlElement xmlElement = xamlParentElement.OwnerDocument.CreateElement((string) null, "Section", HtmlToXamlConverter._xamlNamespace);
        HtmlToXamlConverter.ApplyLocalProperties(xmlElement, localProperties, true);
        if (!xmlElement.HasAttributes)
          xmlElement = xamlParentElement;
        XmlNode htmlNode = htmlElement.FirstChild;
        while (htmlNode != null)
          htmlNode = HtmlToXamlConverter.AddBlock(xmlElement, htmlNode, elementProperties, stylesheet, sourceContext)?.NextSibling;
        if (xmlElement == xamlParentElement)
          return;
        xamlParentElement.AppendChild((XmlNode) xmlElement);
      }
    }

    private static void AddParagraph(
      XmlElement xamlParentElement,
      XmlElement htmlElement,
      Hashtable inheritedProperties,
      CssStylesheet stylesheet,
      List<XmlElement> sourceContext)
    {
      Hashtable localProperties;
      Hashtable elementProperties = HtmlToXamlConverter.GetElementProperties(htmlElement, inheritedProperties, out localProperties, stylesheet, sourceContext);
      XmlElement element = xamlParentElement.OwnerDocument.CreateElement((string) null, "Paragraph", HtmlToXamlConverter._xamlNamespace);
      HtmlToXamlConverter.ApplyLocalProperties(element, localProperties, true);
      for (XmlNode htmlNode = htmlElement.FirstChild; htmlNode != null; htmlNode = htmlNode.NextSibling)
        HtmlToXamlConverter.AddInline(element, htmlNode, elementProperties, stylesheet, sourceContext);
      xamlParentElement.AppendChild((XmlNode) element);
    }

    private static XmlNode AddImplicitParagraph(
      XmlElement xamlParentElement,
      XmlNode htmlNode,
      Hashtable inheritedProperties,
      CssStylesheet stylesheet,
      List<XmlElement> sourceContext)
    {
      XmlElement element = xamlParentElement.OwnerDocument.CreateElement((string) null, "Paragraph", HtmlToXamlConverter._xamlNamespace);
      HtmlToXamlConverter.ApplyLocalProperties(element, inheritedProperties, false);
      XmlNode xmlNode = (XmlNode) null;
      while (true)
      {
        switch (htmlNode)
        {
          case null:
            goto label_8;
          case XmlComment _:
            HtmlToXamlConverter.DefineInlineFragmentParent((XmlComment) htmlNode, (XmlElement) null);
            break;
          case XmlText _:
            if (htmlNode.Value.Trim().Length > 0)
            {
              HtmlToXamlConverter.AddTextRun(element, htmlNode.Value, inheritedProperties);
              break;
            }
            break;
          case XmlElement _:
            if (!HtmlSchema.IsBlockElement(htmlNode.LocalName.ToLowerInvariant()))
            {
              HtmlToXamlConverter.AddInline(element, htmlNode, inheritedProperties, stylesheet, sourceContext);
              break;
            }
            goto label_8;
        }
        xmlNode = htmlNode;
        htmlNode = htmlNode.NextSibling;
      }
label_8:
      if (element.FirstChild != null)
        xamlParentElement.AppendChild((XmlNode) element);
      return xmlNode;
    }

    private static void AddInline(
      XmlElement xamlParentElement,
      XmlNode htmlNode,
      Hashtable inheritedProperties,
      CssStylesheet stylesheet,
      List<XmlElement> sourceContext)
    {
      switch (htmlNode)
      {
        case XmlComment _:
          HtmlToXamlConverter.DefineInlineFragmentParent((XmlComment) htmlNode, xamlParentElement);
          break;
        case XmlText _:
          HtmlToXamlConverter.AddTextRun(xamlParentElement, htmlNode.Value, inheritedProperties);
          break;
        case XmlElement _:
          XmlElement htmlElement = (XmlElement) htmlNode;
          if (htmlElement.NamespaceURI != "http://www.w3.org/1999/xhtml")
            break;
          string lowerInvariant = htmlElement.LocalName.ToLowerInvariant();
          sourceContext.Add(htmlElement);
          switch (lowerInvariant)
          {
            case "a":
              HtmlToXamlConverter.AddHyperlink(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
              break;
            case "img":
              HtmlToXamlConverter.AddImage(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
              break;
            case "br":
            case "hr":
              HtmlToXamlConverter.AddBreak(xamlParentElement, lowerInvariant);
              break;
            default:
              if (HtmlSchema.IsInlineElement(lowerInvariant) || HtmlSchema.IsBlockElement(lowerInvariant))
              {
                HtmlToXamlConverter.AddSpanOrRun(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                break;
              }
              break;
          }
          sourceContext.RemoveAt(sourceContext.Count - 1);
          break;
      }
    }

    private static void AddSpanOrRun(
      XmlElement xamlParentElement,
      XmlElement htmlElement,
      Hashtable inheritedProperties,
      CssStylesheet stylesheet,
      List<XmlElement> sourceContext)
    {
      bool flag = false;
      for (XmlNode xmlNode = htmlElement.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
      {
        if (xmlNode is XmlElement)
        {
          string lowerInvariant = xmlNode.LocalName.ToLowerInvariant();
          if (HtmlSchema.IsInlineElement(lowerInvariant) || HtmlSchema.IsBlockElement(lowerInvariant) || string.Equals(lowerInvariant, "img", StringComparison.OrdinalIgnoreCase) || string.Equals(lowerInvariant, "br", StringComparison.OrdinalIgnoreCase) || string.Equals(lowerInvariant, "hr", StringComparison.OrdinalIgnoreCase))
          {
            flag = true;
            break;
          }
        }
      }
      string localName = flag ? "Span" : "Run";
      Hashtable localProperties;
      Hashtable elementProperties = HtmlToXamlConverter.GetElementProperties(htmlElement, inheritedProperties, out localProperties, stylesheet, sourceContext);
      XmlElement element = xamlParentElement.OwnerDocument.CreateElement((string) null, localName, HtmlToXamlConverter._xamlNamespace);
      HtmlToXamlConverter.ApplyLocalProperties(element, localProperties, false);
      for (XmlNode htmlNode = htmlElement.FirstChild; htmlNode != null; htmlNode = htmlNode.NextSibling)
        HtmlToXamlConverter.AddInline(element, htmlNode, elementProperties, stylesheet, sourceContext);
      xamlParentElement.AppendChild((XmlNode) element);
    }

    private static void AddTextRun(
      XmlElement xamlElement,
      string textData,
      Hashtable inheritedProperties)
    {
      for (int index = 0; index < textData.Length; ++index)
      {
        if (char.IsControl(textData[index]))
          textData = textData.Remove(index--, 1);
      }
      textData = textData.Replace(' ', ' ');
      if (textData.Length <= 0)
        return;
      xamlElement.AppendChild((XmlNode) xamlElement.OwnerDocument.CreateTextNode(textData));
    }

    private static void AddHyperlink(
      XmlElement xamlParentElement,
      XmlElement htmlElement,
      Hashtable inheritedProperties,
      CssStylesheet stylesheet,
      List<XmlElement> sourceContext)
    {
      string attribute = HtmlToXamlConverter.GetAttribute(htmlElement, "href");
      if (attribute == null)
      {
        HtmlToXamlConverter.AddSpanOrRun(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
      }
      else
      {
        Hashtable localProperties;
        Hashtable elementProperties = HtmlToXamlConverter.GetElementProperties(htmlElement, inheritedProperties, out localProperties, stylesheet, sourceContext);
        XmlElement element = xamlParentElement.OwnerDocument.CreateElement((string) null, "Hyperlink", HtmlToXamlConverter._xamlNamespace);
        HtmlToXamlConverter.ApplyLocalProperties(element, localProperties, false);
        string[] strArray = attribute.Split('#');
        if (strArray.Length != 0 && strArray[0].Trim().Length > 0)
          element.SetAttribute("NavigateUri", strArray[0].Trim());
        if (strArray.Length == 2 && strArray[1].Trim().Length > 0)
          element.SetAttribute("TargetName", strArray[1].Trim());
        for (XmlNode htmlNode = htmlElement.FirstChild; htmlNode != null; htmlNode = htmlNode.NextSibling)
          HtmlToXamlConverter.AddInline(element, htmlNode, elementProperties, stylesheet, sourceContext);
        xamlParentElement.AppendChild((XmlNode) element);
      }
    }

    private static void DefineInlineFragmentParent(
      XmlComment htmlComment,
      XmlElement xamlParentElement)
    {
      if (string.Equals(htmlComment.Value, "StartFragment", StringComparison.OrdinalIgnoreCase))
      {
        HtmlToXamlConverter.InlineFragmentParentElement = xamlParentElement;
      }
      else
      {
        if (!string.Equals(htmlComment.Value, "EndFragment", StringComparison.OrdinalIgnoreCase) || HtmlToXamlConverter.InlineFragmentParentElement != null || xamlParentElement == null)
          return;
        HtmlToXamlConverter.InlineFragmentParentElement = xamlParentElement;
      }
    }

    private static XmlElement ExtractInlineFragment(XmlElement xamlFlowDocumentElement)
    {
      if (HtmlToXamlConverter.InlineFragmentParentElement != null)
      {
        if (string.Equals(HtmlToXamlConverter.InlineFragmentParentElement.LocalName, "Span", StringComparison.OrdinalIgnoreCase))
        {
          xamlFlowDocumentElement = HtmlToXamlConverter.InlineFragmentParentElement;
        }
        else
        {
          xamlFlowDocumentElement = xamlFlowDocumentElement.OwnerDocument.CreateElement((string) null, "Span", HtmlToXamlConverter._xamlNamespace);
          while (HtmlToXamlConverter.InlineFragmentParentElement.FirstChild != null)
          {
            XmlNode firstChild = HtmlToXamlConverter.InlineFragmentParentElement.FirstChild;
            HtmlToXamlConverter.InlineFragmentParentElement.RemoveChild(firstChild);
            xamlFlowDocumentElement.AppendChild(firstChild);
          }
        }
      }
      return xamlFlowDocumentElement;
    }

    private static void AddImage(
      XmlElement xamlParentElement,
      XmlElement htmlElement,
      Hashtable inheritedProperties,
      CssStylesheet stylesheet,
      List<XmlElement> sourceContext)
    {
    }

    private static void AddList(
      XmlElement xamlParentElement,
      XmlElement htmlListElement,
      Hashtable inheritedProperties,
      CssStylesheet stylesheet,
      List<XmlElement> sourceContext)
    {
      string lowerInvariant = htmlListElement.LocalName.ToLowerInvariant();
      Hashtable localProperties;
      Hashtable elementProperties = HtmlToXamlConverter.GetElementProperties(htmlListElement, inheritedProperties, out localProperties, stylesheet, sourceContext);
      XmlElement element = xamlParentElement.OwnerDocument.CreateElement((string) null, "List", HtmlToXamlConverter._xamlNamespace);
      if (string.Equals(lowerInvariant, "ol", StringComparison.OrdinalIgnoreCase))
        element.SetAttribute("MarkerStyle", "Decimal");
      else
        element.SetAttribute("MarkerStyle", "Disc");
      HtmlToXamlConverter.ApplyLocalProperties(element, localProperties, true);
      for (XmlNode htmlLIElement = htmlListElement.FirstChild; htmlLIElement != null; htmlLIElement = htmlLIElement.NextSibling)
      {
        if (htmlLIElement is XmlElement && string.Equals(htmlLIElement.LocalName, "li", StringComparison.OrdinalIgnoreCase))
        {
          sourceContext.Add((XmlElement) htmlLIElement);
          HtmlToXamlConverter.AddListItem(element, (XmlElement) htmlLIElement, elementProperties, stylesheet, sourceContext);
          sourceContext.RemoveAt(sourceContext.Count - 1);
        }
      }
      if (!element.HasChildNodes)
        return;
      xamlParentElement.AppendChild((XmlNode) element);
    }

    private static XmlElement AddOrphanListItems(
      XmlElement xamlParentElement,
      XmlElement htmlLIElement,
      Hashtable inheritedProperties,
      CssStylesheet stylesheet,
      List<XmlElement> sourceContext)
    {
      XmlElement xmlElement1 = (XmlElement) null;
      XmlNode lastChild = xamlParentElement.LastChild;
      XmlElement xmlElement2;
      if (lastChild != null && string.Equals(lastChild.LocalName, "List", StringComparison.OrdinalIgnoreCase))
      {
        xmlElement2 = (XmlElement) lastChild;
      }
      else
      {
        xmlElement2 = xamlParentElement.OwnerDocument.CreateElement((string) null, "List", HtmlToXamlConverter._xamlNamespace);
        xamlParentElement.AppendChild((XmlNode) xmlElement2);
      }
      XmlNode htmlLIElement1 = (XmlNode) htmlLIElement;
      for (string localName = htmlLIElement1 == null ? (string) null : htmlLIElement1.LocalName; htmlLIElement1 != null && string.Equals(localName, "li", StringComparison.OrdinalIgnoreCase); localName = htmlLIElement1 == null ? (string) null : htmlLIElement1.LocalName)
      {
        HtmlToXamlConverter.AddListItem(xmlElement2, (XmlElement) htmlLIElement1, inheritedProperties, stylesheet, sourceContext);
        xmlElement1 = (XmlElement) htmlLIElement1;
        htmlLIElement1 = htmlLIElement1.NextSibling;
      }
      return xmlElement1;
    }

    private static void AddListItem(
      XmlElement xamlListElement,
      XmlElement htmlLIElement,
      Hashtable inheritedProperties,
      CssStylesheet stylesheet,
      List<XmlElement> sourceContext)
    {
      Hashtable elementProperties = HtmlToXamlConverter.GetElementProperties(htmlLIElement, inheritedProperties, out Hashtable _, stylesheet, sourceContext);
      XmlElement element = xamlListElement.OwnerDocument.CreateElement((string) null, "ListItem", HtmlToXamlConverter._xamlNamespace);
      XmlNode htmlNode = htmlLIElement.FirstChild;
      while (htmlNode != null)
        htmlNode = HtmlToXamlConverter.AddBlock(element, htmlNode, elementProperties, stylesheet, sourceContext)?.NextSibling;
      xamlListElement.AppendChild((XmlNode) element);
    }

    private static void AddTable(
      XmlElement xamlParentElement,
      XmlElement htmlTableElement,
      Hashtable inheritedProperties,
      CssStylesheet stylesheet,
      List<XmlElement> sourceContext)
    {
      Hashtable localProperties;
      Hashtable elementProperties1 = HtmlToXamlConverter.GetElementProperties(htmlTableElement, inheritedProperties, out localProperties, stylesheet, sourceContext);
      XmlElement fromSingleCellTable = HtmlToXamlConverter.GetCellFromSingleCellTable(htmlTableElement);
      if (fromSingleCellTable != null)
      {
        sourceContext.Add(fromSingleCellTable);
        Hashtable elementProperties2 = HtmlToXamlConverter.GetElementProperties(fromSingleCellTable, inheritedProperties, out localProperties, stylesheet, sourceContext);
        XmlNode htmlNode = fromSingleCellTable.FirstChild;
        while (htmlNode != null)
          htmlNode = HtmlToXamlConverter.AddBlock(xamlParentElement, htmlNode, elementProperties2, stylesheet, sourceContext)?.NextSibling;
        sourceContext.RemoveAt(sourceContext.Count - 1);
      }
      else
      {
        XmlElement element1 = xamlParentElement.OwnerDocument.CreateElement((string) null, "Table", HtmlToXamlConverter._xamlNamespace);
        ArrayList arrayList = HtmlToXamlConverter.AnalyzeTableStructure(htmlTableElement, stylesheet);
        HtmlToXamlConverter.AddColumnInformation(htmlTableElement, element1, arrayList, elementProperties1, stylesheet, sourceContext);
        XmlNode xmlNode = htmlTableElement.FirstChild;
        while (xmlNode != null)
        {
          string localName = xmlNode.LocalName;
          if (string.Equals(localName, "tbody", StringComparison.OrdinalIgnoreCase) || string.Equals(localName, "thead", StringComparison.OrdinalIgnoreCase) || string.Equals(localName, "tfoot", StringComparison.OrdinalIgnoreCase))
          {
            XmlElement element2 = element1.OwnerDocument.CreateElement((string) null, "TableRowGroup", HtmlToXamlConverter._xamlNamespace);
            element1.AppendChild((XmlNode) element2);
            sourceContext.Add((XmlElement) xmlNode);
            Hashtable elementProperties3 = HtmlToXamlConverter.GetElementProperties((XmlElement) xmlNode, elementProperties1, out Hashtable _, stylesheet, sourceContext);
            HtmlToXamlConverter.AddTableRowsToTableBody(element2, xmlNode.FirstChild, elementProperties3, arrayList, stylesheet, sourceContext);
            if (element2.HasChildNodes)
              element1.AppendChild((XmlNode) element2);
            sourceContext.RemoveAt(sourceContext.Count - 1);
            xmlNode = xmlNode.NextSibling;
          }
          else if (string.Equals(localName, "tr", StringComparison.OrdinalIgnoreCase))
          {
            XmlElement element3 = element1.OwnerDocument.CreateElement((string) null, "TableRowGroup", HtmlToXamlConverter._xamlNamespace);
            xmlNode = HtmlToXamlConverter.AddTableRowsToTableBody(element3, xmlNode, elementProperties1, arrayList, stylesheet, sourceContext);
            if (element3.HasChildNodes)
              element1.AppendChild((XmlNode) element3);
          }
          else
            xmlNode = xmlNode.NextSibling;
        }
        if (!element1.HasChildNodes)
          return;
        xamlParentElement.AppendChild((XmlNode) element1);
      }
    }

    private static XmlElement GetCellFromSingleCellTable(XmlElement htmlTableElement)
    {
      XmlElement fromSingleCellTable = (XmlElement) null;
      for (XmlNode xmlNode1 = htmlTableElement.FirstChild; xmlNode1 != null; xmlNode1 = xmlNode1.NextSibling)
      {
        string localName1 = xmlNode1.LocalName;
        if (string.Equals(localName1, "tbody", StringComparison.OrdinalIgnoreCase) || string.Equals(localName1, "thead", StringComparison.OrdinalIgnoreCase) || string.Equals(localName1, "tfoot", StringComparison.OrdinalIgnoreCase))
        {
          if (fromSingleCellTable != null)
            return (XmlElement) null;
          for (XmlNode xmlNode2 = xmlNode1.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
          {
            if (string.Equals(xmlNode2.LocalName, "tr", StringComparison.OrdinalIgnoreCase))
            {
              if (fromSingleCellTable != null)
                return (XmlElement) null;
              for (XmlNode xmlNode3 = xmlNode2.FirstChild; xmlNode3 != null; xmlNode3 = xmlNode3.NextSibling)
              {
                string localName2 = xmlNode3.LocalName;
                if (string.Equals(localName2, "td", StringComparison.OrdinalIgnoreCase) || string.Equals(localName2, "th", StringComparison.OrdinalIgnoreCase))
                {
                  if (fromSingleCellTable != null)
                    return (XmlElement) null;
                  fromSingleCellTable = (XmlElement) xmlNode3;
                }
              }
            }
          }
        }
        else if (string.Equals(xmlNode1.LocalName, "tr", StringComparison.OrdinalIgnoreCase))
        {
          if (fromSingleCellTable != null)
            return (XmlElement) null;
          for (XmlNode xmlNode4 = xmlNode1.FirstChild; xmlNode4 != null; xmlNode4 = xmlNode4.NextSibling)
          {
            string localName3 = xmlNode4.LocalName;
            if (string.Equals(localName3, "td", StringComparison.OrdinalIgnoreCase) || string.Equals(localName3, "th", StringComparison.OrdinalIgnoreCase))
            {
              if (fromSingleCellTable != null)
                return (XmlElement) null;
              fromSingleCellTable = (XmlElement) xmlNode4;
            }
          }
        }
      }
      return fromSingleCellTable;
    }

    private static void AddColumnInformation(
      XmlElement htmlTableElement,
      XmlElement xamlTableElement,
      ArrayList columnStartsAllRows,
      Hashtable currentProperties,
      CssStylesheet stylesheet,
      List<XmlElement> sourceContext)
    {
      XmlElement element1 = xamlTableElement.OwnerDocument.CreateElement((string) null, "Table.Columns", HtmlToXamlConverter._xamlNamespace);
      xamlTableElement.AppendChild((XmlNode) element1);
      if (columnStartsAllRows != null)
      {
        for (int index = 0; index < columnStartsAllRows.Count - 1; ++index)
        {
          XmlElement element2 = xamlTableElement.OwnerDocument.CreateElement((string) null, "TableColumn", HtmlToXamlConverter._xamlNamespace);
          element2.SetAttribute("Width", ((double) columnStartsAllRows[index + 1] - (double) columnStartsAllRows[index]).ToString((IFormatProvider) CultureInfo.InvariantCulture));
          element1.AppendChild((XmlNode) element2);
        }
      }
      else
      {
        for (XmlNode xmlNode = htmlTableElement.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
        {
          if (string.Equals(xmlNode.LocalName, "colgroup", StringComparison.OrdinalIgnoreCase))
            HtmlToXamlConverter.AddTableColumnGroup(element1, (XmlElement) xmlNode, currentProperties, stylesheet, sourceContext);
          else if (string.Equals(xmlNode.LocalName, "col", StringComparison.OrdinalIgnoreCase))
            HtmlToXamlConverter.AddTableColumn(element1, (XmlElement) xmlNode, currentProperties, stylesheet, sourceContext);
          else if (xmlNode is XmlElement)
            break;
        }
      }
    }

    private static void AddTableColumnGroup(
      XmlElement xamlTableElement,
      XmlElement htmlColgroupElement,
      Hashtable inheritedProperties,
      CssStylesheet stylesheet,
      List<XmlElement> sourceContext)
    {
      Hashtable elementProperties = HtmlToXamlConverter.GetElementProperties(htmlColgroupElement, inheritedProperties, out Hashtable _, stylesheet, sourceContext);
      for (XmlNode htmlColElement = htmlColgroupElement.FirstChild; htmlColElement != null; htmlColElement = htmlColElement.NextSibling)
      {
        if (htmlColElement is XmlElement && string.Equals(htmlColElement.LocalName, "col", StringComparison.OrdinalIgnoreCase))
          HtmlToXamlConverter.AddTableColumn(xamlTableElement, (XmlElement) htmlColElement, elementProperties, stylesheet, sourceContext);
      }
    }

    private static void AddTableColumn(
      XmlElement xamlTableElement,
      XmlElement htmlColElement,
      Hashtable inheritedProperties,
      CssStylesheet stylesheet,
      List<XmlElement> sourceContext)
    {
      HtmlToXamlConverter.GetElementProperties(htmlColElement, inheritedProperties, out Hashtable _, stylesheet, sourceContext);
      XmlElement element = xamlTableElement.OwnerDocument.CreateElement((string) null, "TableColumn", HtmlToXamlConverter._xamlNamespace);
      xamlTableElement.AppendChild((XmlNode) element);
    }

    private static XmlNode AddTableRowsToTableBody(
      XmlElement xamlTableBodyElement,
      XmlNode htmlTRStartNode,
      Hashtable currentProperties,
      ArrayList columnStarts,
      CssStylesheet stylesheet,
      List<XmlElement> sourceContext)
    {
      XmlNode tableBody = htmlTRStartNode;
      ArrayList activeRowSpans = (ArrayList) null;
      if (columnStarts != null)
      {
        activeRowSpans = new ArrayList();
        HtmlToXamlConverter.InitializeActiveRowSpans(activeRowSpans, columnStarts.Count);
      }
      while (tableBody != null && !string.Equals(tableBody.LocalName, "tbody", StringComparison.OrdinalIgnoreCase))
      {
        if (string.Equals(tableBody.LocalName, "tr", StringComparison.OrdinalIgnoreCase))
        {
          XmlElement element = xamlTableBodyElement.OwnerDocument.CreateElement((string) null, "TableRow", HtmlToXamlConverter._xamlNamespace);
          sourceContext.Add((XmlElement) tableBody);
          Hashtable elementProperties = HtmlToXamlConverter.GetElementProperties((XmlElement) tableBody, currentProperties, out Hashtable _, stylesheet, sourceContext);
          HtmlToXamlConverter.AddTableCellsToTableRow(element, tableBody.FirstChild, elementProperties, columnStarts, activeRowSpans, stylesheet, sourceContext);
          if (element.HasChildNodes)
            xamlTableBodyElement.AppendChild((XmlNode) element);
          sourceContext.RemoveAt(sourceContext.Count - 1);
          tableBody = tableBody.NextSibling;
        }
        else if (string.Equals(tableBody.LocalName, "td", StringComparison.OrdinalIgnoreCase))
        {
          XmlElement element = xamlTableBodyElement.OwnerDocument.CreateElement((string) null, "TableRow", HtmlToXamlConverter._xamlNamespace);
          tableBody = HtmlToXamlConverter.AddTableCellsToTableRow(element, tableBody, currentProperties, columnStarts, activeRowSpans, stylesheet, sourceContext);
          if (element.HasChildNodes)
            xamlTableBodyElement.AppendChild((XmlNode) element);
        }
        else
          tableBody = tableBody.NextSibling;
      }
      return tableBody;
    }

    private static XmlNode AddTableCellsToTableRow(
      XmlElement xamlTableRowElement,
      XmlNode htmlTDStartNode,
      Hashtable currentProperties,
      ArrayList columnStarts,
      ArrayList activeRowSpans,
      CssStylesheet stylesheet,
      List<XmlElement> sourceContext)
    {
      XmlNode tableRow = htmlTDStartNode;
      int num = 0;
      while (tableRow != null && !string.Equals(tableRow.LocalName, "tr", StringComparison.OrdinalIgnoreCase) && !string.Equals(tableRow.LocalName, "tbody", StringComparison.OrdinalIgnoreCase) && !string.Equals(tableRow.LocalName, "thead", StringComparison.OrdinalIgnoreCase) && !string.Equals(tableRow.LocalName, "tfoot", StringComparison.OrdinalIgnoreCase))
      {
        if (string.Equals(tableRow.LocalName, "td", StringComparison.OrdinalIgnoreCase) || string.Equals(tableRow.LocalName, "th", StringComparison.OrdinalIgnoreCase))
        {
          XmlElement element = xamlTableRowElement.OwnerDocument.CreateElement((string) null, "TableCell", HtmlToXamlConverter._xamlNamespace);
          sourceContext.Add((XmlElement) tableRow);
          Hashtable elementProperties = HtmlToXamlConverter.GetElementProperties((XmlElement) tableRow, currentProperties, out Hashtable _, stylesheet, sourceContext);
          HtmlToXamlConverter.ApplyPropertiesToTableCellElement((XmlElement) tableRow, element);
          if (columnStarts != null)
          {
            for (; num < activeRowSpans.Count && (int) activeRowSpans[num] > 0; ++num)
              activeRowSpans[num] = (object) ((int) activeRowSpans[num] - 1);
            double columnStart = (double) columnStarts[num];
            double columnWidth = HtmlToXamlConverter.GetColumnWidth((XmlElement) tableRow);
            int columnSpan = HtmlToXamlConverter.CalculateColumnSpan(num, columnWidth, columnStarts);
            int rowSpan = HtmlToXamlConverter.GetRowSpan((XmlElement) tableRow);
            if (columnSpan > 0)
              element.SetAttribute("ColumnSpan", columnSpan.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            for (int index = num; index < num + columnSpan; ++index)
              activeRowSpans[index] = (object) (rowSpan - 1);
            num += columnSpan;
          }
          HtmlToXamlConverter.AddDataToTableCell(element, tableRow.FirstChild, elementProperties, stylesheet, sourceContext);
          if (element.HasChildNodes)
            xamlTableRowElement.AppendChild((XmlNode) element);
          sourceContext.RemoveAt(sourceContext.Count - 1);
          tableRow = tableRow.NextSibling;
        }
        else
          tableRow = tableRow.NextSibling;
      }
      return tableRow;
    }

    private static void AddDataToTableCell(
      XmlElement xamlTableCellElement,
      XmlNode htmlDataStartNode,
      Hashtable currentProperties,
      CssStylesheet stylesheet,
      List<XmlElement> sourceContext)
    {
      XmlNode htmlNode = htmlDataStartNode;
      while (htmlNode != null)
        htmlNode = HtmlToXamlConverter.AddBlock(xamlTableCellElement, htmlNode, currentProperties, stylesheet, sourceContext)?.NextSibling;
    }

    private static ArrayList AnalyzeTableStructure(
      XmlElement htmlTableElement,
      CssStylesheet stylesheet)
    {
      if (!htmlTableElement.HasChildNodes)
        return (ArrayList) null;
      bool flag = true;
      ArrayList columnStarts = new ArrayList();
      ArrayList activeRowSpans = new ArrayList();
      XmlNode xmlNode = htmlTableElement.FirstChild;
      double tableWidth = 0.0;
      for (; xmlNode != null & flag; xmlNode = xmlNode.NextSibling)
      {
        switch (xmlNode.LocalName.ToLowerInvariant())
        {
          case "tbody":
            double num1 = HtmlToXamlConverter.AnalyzeTbodyStructure((XmlElement) xmlNode, columnStarts, activeRowSpans, tableWidth, stylesheet);
            if (num1 > tableWidth)
            {
              tableWidth = num1;
              break;
            }
            if (num1 == 0.0)
            {
              flag = false;
              break;
            }
            break;
          case "tr":
            double num2 = HtmlToXamlConverter.AnalyzeTRStructure((XmlElement) xmlNode, columnStarts, activeRowSpans, tableWidth, stylesheet);
            if (num2 > tableWidth)
            {
              tableWidth = num2;
              break;
            }
            if (num2 == 0.0)
            {
              flag = false;
              break;
            }
            break;
          case "td":
            flag = false;
            break;
        }
      }
      if (flag)
      {
        columnStarts.Add((object) tableWidth);
        HtmlToXamlConverter.VerifyColumnStartsAscendingOrder(columnStarts);
      }
      else
        columnStarts = (ArrayList) null;
      return columnStarts;
    }

    private static double AnalyzeTbodyStructure(
      XmlElement htmlTbodyElement,
      ArrayList columnStarts,
      ArrayList activeRowSpans,
      double tableWidth,
      CssStylesheet stylesheet)
    {
      double tableWidth1 = 0.0;
      bool flag = true;
      if (!htmlTbodyElement.HasChildNodes)
        return tableWidth1;
      HtmlToXamlConverter.ClearActiveRowSpans(activeRowSpans);
      for (XmlNode htmlTRElement = htmlTbodyElement.FirstChild; htmlTRElement != null & flag; htmlTRElement = htmlTRElement.NextSibling)
      {
        switch (htmlTRElement.LocalName.ToLowerInvariant())
        {
          case "tr":
            double num = HtmlToXamlConverter.AnalyzeTRStructure((XmlElement) htmlTRElement, columnStarts, activeRowSpans, tableWidth1, stylesheet);
            if (num > tableWidth1)
            {
              tableWidth1 = num;
              break;
            }
            break;
          case "td":
            flag = false;
            break;
        }
      }
      HtmlToXamlConverter.ClearActiveRowSpans(activeRowSpans);
      return !flag ? 0.0 : tableWidth1;
    }

    private static double AnalyzeTRStructure(
      XmlElement htmlTRElement,
      ArrayList columnStarts,
      ArrayList activeRowSpans,
      double tableWidth,
      CssStylesheet stylesheet)
    {
      if (!htmlTRElement.HasChildNodes)
        return 0.0;
      bool flag = true;
      double num1 = 0.0;
      XmlNode htmlTDElement = htmlTRElement.FirstChild;
      int num2 = 0;
      if (num2 < activeRowSpans.Count && (double) columnStarts[num2] == num1)
      {
        while (num2 < activeRowSpans.Count && (int) activeRowSpans[num2] > 0)
        {
          activeRowSpans[num2] = (object) ((int) activeRowSpans[num2] - 1);
          ++num2;
          num1 = (double) columnStarts[num2];
        }
      }
      for (; htmlTDElement != null & flag; htmlTDElement = htmlTDElement.NextSibling)
      {
        HtmlToXamlConverter.VerifyColumnStartsAscendingOrder(columnStarts);
        if (htmlTDElement.LocalName.ToLowerInvariant() == "td")
        {
          if (num2 < columnStarts.Count)
          {
            if (num1 < (double) columnStarts[num2])
            {
              columnStarts.Insert(num2, (object) num1);
              activeRowSpans.Insert(num2, (object) 0);
            }
          }
          else
          {
            columnStarts.Add((object) num1);
            activeRowSpans.Add((object) 0);
          }
          double columnWidth = HtmlToXamlConverter.GetColumnWidth((XmlElement) htmlTDElement);
          if (columnWidth != -1.0)
          {
            int rowSpan = HtmlToXamlConverter.GetRowSpan((XmlElement) htmlTDElement);
            int nextColumnIndex = HtmlToXamlConverter.GetNextColumnIndex(num2, columnWidth, columnStarts, activeRowSpans);
            if (nextColumnIndex != -1)
            {
              for (int index = num2; index < nextColumnIndex; ++index)
                activeRowSpans[index] = (object) (rowSpan - 1);
              num2 = nextColumnIndex;
              num1 += columnWidth;
              if (num2 < activeRowSpans.Count && (double) columnStarts[num2] == num1)
              {
                while (num2 < activeRowSpans.Count && (int) activeRowSpans[num2] > 0)
                {
                  activeRowSpans[num2] = (object) ((int) activeRowSpans[num2] - 1);
                  ++num2;
                  if (num2 < columnStarts.Count)
                    num1 = (double) columnStarts[num2];
                }
              }
            }
            else
              flag = false;
          }
          else
            flag = false;
        }
      }
      return !flag ? 0.0 : num1;
    }

    private static int GetRowSpan(XmlElement htmlTDElement)
    {
      string attribute = HtmlToXamlConverter.GetAttribute(htmlTDElement, "rowspan");
      int result;
      if (attribute != null)
      {
        if (!int.TryParse(attribute, out result))
          result = 1;
      }
      else
        result = 1;
      return result;
    }

    private static int GetNextColumnIndex(
      int columnIndex,
      double columnWidth,
      ArrayList columnStarts,
      ArrayList activeRowSpans)
    {
      double columnStart = (double) columnStarts[columnIndex];
      int index = columnIndex + 1;
      while (index < columnStarts.Count && (double) columnStarts[index] < columnStart + columnWidth && index != -1)
      {
        if ((int) activeRowSpans[index] > 0)
          index = -1;
        else
          ++index;
      }
      return index;
    }

    private static void ClearActiveRowSpans(ArrayList activeRowSpans)
    {
      for (int index = 0; index < activeRowSpans.Count; ++index)
        activeRowSpans[index] = (object) 0;
    }

    private static void InitializeActiveRowSpans(ArrayList activeRowSpans, int count)
    {
      for (int index = 0; index < count; ++index)
        activeRowSpans.Add((object) 0);
    }

    private static double GetNextColumnStart(XmlElement htmlTDElement, double columnStart)
    {
      double columnWidth = HtmlToXamlConverter.GetColumnWidth(htmlTDElement);
      return columnWidth != -1.0 ? columnStart + columnWidth : -1.0;
    }

    private static double GetColumnWidth(XmlElement htmlTDElement)
    {
      double length = -1.0;
      string lengthAsString = HtmlToXamlConverter.GetAttribute(htmlTDElement, "width");
      if (string.IsNullOrEmpty(lengthAsString))
        lengthAsString = HtmlToXamlConverter.GetCssAttribute(HtmlToXamlConverter.GetAttribute(htmlTDElement, "style"), "width");
      if (!HtmlToXamlConverter.TryGetLengthValue(lengthAsString, out length) || length == 0.0)
        length = -1.0;
      return length;
    }

    private static int CalculateColumnSpan(
      int columnIndex,
      double columnWidth,
      ArrayList columnStarts)
    {
      int index = columnIndex;
      double num1 = 0.0;
      for (; num1 < columnWidth && index < columnStarts.Count - 1; ++index)
      {
        double num2 = (double) columnStarts[index + 1] - (double) columnStarts[index];
        num1 += num2;
      }
      return index - columnIndex;
    }

    private static void VerifyColumnStartsAscendingOrder(ArrayList columnStarts)
    {
      for (int index = 0; index < columnStarts.Count; ++index)
      {
        double columnStart = (double) columnStarts[index];
      }
    }

    private static void ApplyLocalProperties(
      XmlElement xamlElement,
      Hashtable localProperties,
      bool isBlock)
    {
      bool flag1 = false;
      string top1 = "0";
      string bottom1 = "0";
      string left1 = "0";
      string right1 = "0";
      bool flag2 = false;
      string top2 = "0";
      string bottom2 = "0";
      string left2 = "0";
      string right2 = "0";
      string str1 = (string) null;
      bool flag3 = false;
      string top3 = "0";
      string bottom3 = "0";
      string left3 = "0";
      string right3 = "0";
      IDictionaryEnumerator enumerator = localProperties.GetEnumerator();
      while (enumerator.MoveNext())
      {
        string key = (string) enumerator.Key;
        if (key != null)
        {
          switch (key.Length)
          {
            case 3:
              if (key == "dir")
              {
                xamlElement.SetAttribute("FlowDirection", HtmlToXamlConverter.GetFlowDirection((string) enumerator.Value));
                continue;
              }
              continue;
            case 5:
              switch (key[2])
              {
                case 'd':
                  if (key == "width")
                    continue;
                  continue;
                case 'e':
                  if (key == "clear")
                    break;
                  continue;
                case 'l':
                  if (key == "color")
                  {
                    HtmlToXamlConverter.SetForegroundProperty(xamlElement, (string) enumerator.Value);
                    continue;
                  }
                  continue;
                case 'o':
                  if (key == "float")
                    break;
                  continue;
                default:
                  continue;
              }
              if (!isBlock)
                continue;
              continue;
            case 6:
              if (key == "height")
                continue;
              continue;
            case 7:
              if (key == "display")
                continue;
              continue;
            case 8:
              if (key == "xml:lang")
              {
                xamlElement.SetAttribute("xml:lang", (string) enumerator.Value);
                continue;
              }
              continue;
            case 9:
              if (key == "font-size")
              {
                xamlElement.SetAttribute("FontSize", (string) enumerator.Value);
                continue;
              }
              continue;
            case 10:
              switch (key[0])
              {
                case 'b':
                  if (key == "background")
                    goto label_64;
                  else
                    continue;
                case 'f':
                  if (key == "font-style")
                  {
                    xamlElement.SetAttribute("FontStyle", (string) enumerator.Value);
                    continue;
                  }
                  continue;
                case 'm':
                  if (key == "margin-top")
                  {
                    flag1 = true;
                    top1 = (string) enumerator.Value;
                    continue;
                  }
                  continue;
                case 't':
                  if (key == "text-align" && isBlock)
                  {
                    xamlElement.SetAttribute("TextAlignment", (string) enumerator.Value);
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
            case 11:
              switch (key[6])
              {
                case '-':
                  if (key == "margin-left")
                  {
                    flag1 = true;
                    left1 = (string) enumerator.Value;
                    continue;
                  }
                  continue;
                case 'a':
                  if (key == "font-family")
                    break;
                  continue;
                case 'e':
                  if (key == "font-weight")
                  {
                    xamlElement.SetAttribute("FontWeight", (string) enumerator.Value);
                    continue;
                  }
                  continue;
                case 'g':
                  if (key == "padding-top")
                  {
                    flag2 = true;
                    top2 = (string) enumerator.Value;
                    continue;
                  }
                  continue;
                case 'n':
                  if (key == "text-indent" && isBlock)
                  {
                    xamlElement.SetAttribute("TextIndent", (string) enumerator.Value);
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
              break;
            case 12:
              switch (key[0])
              {
                case 'f':
                  if (key == "font-variant")
                    continue;
                  continue;
                case 'm':
                  if (key == "margin-right")
                  {
                    flag1 = true;
                    right1 = (string) enumerator.Value;
                    continue;
                  }
                  continue;
                case 'p':
                  if (key == "padding-left")
                  {
                    flag2 = true;
                    left2 = (string) enumerator.Value;
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
            case 13:
              switch (key[0])
              {
                case 'm':
                  if (key == "margin-bottom")
                  {
                    flag1 = true;
                    bottom1 = (string) enumerator.Value;
                    continue;
                  }
                  continue;
                case 'p':
                  if (key == "padding-right")
                  {
                    flag2 = true;
                    right2 = (string) enumerator.Value;
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
            case 14:
              switch (key[0])
              {
                case 'p':
                  if (key == "padding-bottom")
                  {
                    flag2 = true;
                    bottom2 = (string) enumerator.Value;
                    continue;
                  }
                  continue;
                case 't':
                  if (key == "text-transform")
                    continue;
                  continue;
                default:
                  continue;
              }
            case 15:
              if (key == "list-style-type" && string.Equals(xamlElement.LocalName, "List", StringComparison.OrdinalIgnoreCase))
              {
                string lowerInvariant = ((string) enumerator.Value).ToLowerInvariant();
                string str2;
                if (lowerInvariant != null)
                {
                  switch (lowerInvariant.Length)
                  {
                    case 3:
                      if (lowerInvariant == "box")
                      {
                        str2 = "Box";
                        goto label_110;
                      }
                      else
                        break;
                    case 4:
                      switch (lowerInvariant[0])
                      {
                        case 'd':
                          if (lowerInvariant == "disc")
                          {
                            str2 = "Disc";
                            goto label_110;
                          }
                          else
                            break;
                        case 'n':
                          if (lowerInvariant == "none")
                          {
                            str2 = "None";
                            goto label_110;
                          }
                          else
                            break;
                      }
                      break;
                    case 6:
                      switch (lowerInvariant[0])
                      {
                        case 'c':
                          if (lowerInvariant == "circle")
                          {
                            str2 = "Circle";
                            goto label_110;
                          }
                          else
                            break;
                        case 's':
                          if (lowerInvariant == "square")
                          {
                            str2 = "Square";
                            goto label_110;
                          }
                          else
                            break;
                      }
                      break;
                    case 7:
                      if (lowerInvariant == "decimal")
                      {
                        str2 = "Decimal";
                        goto label_110;
                      }
                      else
                        break;
                    case 11:
                      switch (lowerInvariant[0])
                      {
                        case 'l':
                          switch (lowerInvariant)
                          {
                            case "lower-latin":
                              str2 = "LowerLatin";
                              goto label_110;
                            case "lower-roman":
                              str2 = "LowerRoman";
                              goto label_110;
                          }
                          break;
                        case 'u':
                          switch (lowerInvariant)
                          {
                            case "upper-latin":
                              str2 = "UpperLatin";
                              goto label_110;
                            case "upper-roman":
                              str2 = "UpperRoman";
                              goto label_110;
                          }
                          break;
                      }
                      break;
                  }
                }
                str2 = "Disc";
label_110:
                xamlElement.SetAttribute("MarkerStyle", str2);
                continue;
              }
              continue;
            case 16:
              switch (key[7])
              {
                case 'c':
                  if (key == "border-color-top")
                  {
                    str1 = (string) enumerator.Value;
                    continue;
                  }
                  continue;
                case 's':
                  if (key == "border-style-top")
                    continue;
                  continue;
                case 'u':
                  if (key == "background-color")
                    goto label_64;
                  else
                    continue;
                case 'w':
                  if (key == "border-width-top")
                  {
                    flag3 = true;
                    top3 = (string) enumerator.Value;
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
            case 17:
              switch (key[7])
              {
                case 'c':
                  if (key == "border-color-left")
                  {
                    str1 = (string) enumerator.Value;
                    continue;
                  }
                  continue;
                case 's':
                  if (key == "border-style-left")
                    continue;
                  continue;
                case 'w':
                  if (key == "border-width-left")
                  {
                    flag3 = true;
                    left3 = (string) enumerator.Value;
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
            case 18:
              switch (key[7])
              {
                case 'c':
                  if (key == "border-color-right")
                  {
                    str1 = (string) enumerator.Value;
                    continue;
                  }
                  continue;
                case 's':
                  if (key == "border-style-right")
                    continue;
                  continue;
                case 'w':
                  if (key == "border-width-right")
                  {
                    flag3 = true;
                    right3 = (string) enumerator.Value;
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
            case 19:
              switch (key[7])
              {
                case 'c':
                  if (key == "border-color-bottom")
                  {
                    str1 = (string) enumerator.Value;
                    continue;
                  }
                  continue;
                case 's':
                  if (key == "border-style-bottom")
                    continue;
                  continue;
                case 'w':
                  if (key == "border-width-bottom")
                  {
                    flag3 = true;
                    bottom3 = (string) enumerator.Value;
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
            case 20:
              if (key == "text-decoration-none")
                goto label_66;
              else
                continue;
            case 21:
              switch (key[0])
              {
                case 'm':
                  if (key == "mso-ascii-font-family")
                    break;
                  continue;
                case 't':
                  if (key == "text-decoration-blink")
                    goto label_66;
                  else
                    continue;
                default:
                  continue;
              }
              break;
            case 24:
              if (key == "text-decoration-overline")
                goto label_66;
              else
                continue;
            case 25:
              if (key == "text-decoration-underline" && string.Equals((string) enumerator.Value, "true", StringComparison.OrdinalIgnoreCase))
              {
                xamlElement.SetAttribute("TextDecorations", "Underline");
                continue;
              }
              continue;
            case 28:
              if (key == "text-decoration-line-through")
                goto label_66;
              else
                continue;
            default:
              continue;
          }
          xamlElement.SetAttribute("FontFamily", (string) enumerator.Value);
          continue;
label_64:
          HtmlToXamlConverter.SetPropertyValue(xamlElement, TextElement.BackgroundProperty, (string) enumerator.Value);
          continue;
label_66:
          if (isBlock)
            ;
        }
      }
      if (!isBlock)
        return;
      if (flag1)
        HtmlToXamlConverter.ComposeThicknessProperty(xamlElement, "Margin", left1, right1, top1, bottom1);
      if (flag2)
        HtmlToXamlConverter.ComposeThicknessProperty(xamlElement, "Padding", left2, right2, top2, bottom2);
      if (str1 != null)
        xamlElement.SetAttribute("BorderBrush", str1);
      if (!flag3)
        return;
      HtmlToXamlConverter.ComposeThicknessProperty(xamlElement, "BorderThickness", left3, right3, top3, bottom3);
    }

    private static void SetForegroundProperty(XmlElement xamlElement, string value)
    {
      if (Color.Equals((Color) ColorConverter.ConvertFromString(value), SystemColors.ControlTextColor))
        return;
      HtmlToXamlConverter.SetPropertyValue(xamlElement, TextElement.ForegroundProperty, value);
    }

    private static string GetFlowDirection(string property)
    {
      if (string.Equals(property, "rtl", StringComparison.OrdinalIgnoreCase))
        return "RightToLeft";
      string.Equals(property, "rtl", StringComparison.OrdinalIgnoreCase);
      return "LeftToRight";
    }

    private static void ComposeThicknessProperty(
      XmlElement xamlElement,
      string propertyName,
      string left,
      string right,
      string top,
      string bottom)
    {
      int num = 0;
      if (left[0] == '0' || left[0] == '-')
      {
        left = "0";
        ++num;
      }
      if (right[0] == '0' || right[0] == '-')
      {
        right = "0";
        ++num;
      }
      if (top[0] == '0' || top[0] == '-')
      {
        top = "0";
        ++num;
      }
      if (bottom[0] == '0' || bottom[0] == '-')
      {
        bottom = "0";
        ++num;
      }
      if (num == 4)
        return;
      string str;
      if (string.Equals(left, right, StringComparison.OrdinalIgnoreCase) && string.Equals(top, bottom, StringComparison.OrdinalIgnoreCase))
        str = !string.Equals(left, top, StringComparison.OrdinalIgnoreCase) ? left + "," + top : left;
      else
        str = left + "," + top + "," + right + "," + bottom;
      xamlElement.SetAttribute(propertyName, str);
    }

    private static void SetPropertyValue(
      XmlElement xamlElement,
      DependencyProperty property,
      string stringValue)
    {
      TypeConverter converter = TypeDescriptor.GetConverter(property.PropertyType);
      try
      {
        if (converter.ConvertFromInvariantString(stringValue) == null)
          return;
        xamlElement.SetAttribute(property.Name, stringValue);
      }
      catch (Exception ex)
      {
      }
    }

    private static Hashtable GetElementProperties(
      XmlElement htmlElement,
      Hashtable inheritedProperties,
      out Hashtable localProperties,
      CssStylesheet stylesheet,
      List<XmlElement> sourceContext)
    {
      Hashtable elementProperties = new Hashtable();
      IDictionaryEnumerator enumerator1 = inheritedProperties.GetEnumerator();
      while (enumerator1.MoveNext())
        elementProperties[enumerator1.Key] = enumerator1.Value;
      string lowerInvariant = htmlElement.LocalName.ToLowerInvariant();
      string namespaceUri = htmlElement.NamespaceURI;
      localProperties = new Hashtable();
      if (lowerInvariant != null)
      {
        switch (lowerInvariant.Length)
        {
          case 1:
            switch (lowerInvariant[0])
            {
              case 'b':
                goto label_35;
              case 'i':
                break;
              case 'u':
                goto label_36;
              default:
                goto label_58;
            }
            break;
          case 2:
            switch (lowerInvariant[1])
            {
              case '1':
                if (lowerInvariant == "h1")
                {
                  localProperties[(object) "font-size"] = (object) "22pt";
                  goto label_58;
                }
                else
                  goto label_58;
              case '2':
                if (lowerInvariant == "h2")
                {
                  localProperties[(object) "font-size"] = (object) "20pt";
                  goto label_58;
                }
                else
                  goto label_58;
              case '3':
                if (lowerInvariant == "h3")
                {
                  localProperties[(object) "font-size"] = (object) "18pt";
                  goto label_58;
                }
                else
                  goto label_58;
              case '4':
                if (lowerInvariant == "h4")
                {
                  localProperties[(object) "font-size"] = (object) "16pt";
                  goto label_58;
                }
                else
                  goto label_58;
              case '5':
                if (lowerInvariant == "h5")
                {
                  localProperties[(object) "font-size"] = (object) "12pt";
                  goto label_58;
                }
                else
                  goto label_58;
              case '6':
                if (lowerInvariant == "h6")
                {
                  localProperties[(object) "font-size"] = (object) "10pt";
                  goto label_58;
                }
                else
                  goto label_58;
              case 'l':
                switch (lowerInvariant)
                {
                  case "ul":
                    localProperties[(object) "list-style-type"] = (object) "disc";
                    goto label_58;
                  case "ol":
                    localProperties[(object) "list-style-type"] = (object) "decimal";
                    goto label_58;
                  default:
                    goto label_58;
                }
              case 'm':
                if (lowerInvariant == "em")
                  break;
                goto label_58;
              default:
                goto label_58;
            }
            break;
          case 3:
            switch (lowerInvariant[2])
            {
              case 'b':
                if (lowerInvariant == "sub")
                  goto label_58;
                else
                  goto label_58;
              case 'e':
                if (lowerInvariant == "pre")
                {
                  localProperties[(object) "font-family"] = (object) "Courier New";
                  localProperties[(object) "font-size"] = (object) "8pt";
                  localProperties[(object) "text-align"] = (object) "Left";
                  goto label_58;
                }
                else
                  goto label_58;
              case 'n':
                if (lowerInvariant == "dfn")
                  goto label_35;
                else
                  goto label_58;
              case 'p':
                if (lowerInvariant == "sup")
                  goto label_58;
                else
                  goto label_58;
              case 'v':
                if (lowerInvariant == "div")
                  goto label_58;
                else
                  goto label_58;
              default:
                goto label_58;
            }
          case 4:
            switch (lowerInvariant[3])
            {
              case 'd':
                if (lowerInvariant == "bold")
                  goto label_35;
                else
                  goto label_58;
              case 'l':
                if (lowerInvariant == "html")
                  goto label_58;
                else
                  goto label_58;
              case 'p':
                if (lowerInvariant == "samp")
                {
                  localProperties[(object) "font-family"] = (object) "Courier New";
                  localProperties[(object) "font-size"] = (object) "8pt";
                  localProperties[(object) "text-align"] = (object) "Left";
                  goto label_58;
                }
                else
                  goto label_58;
              case 't':
                if (lowerInvariant == "font")
                {
                  string attribute1 = HtmlToXamlConverter.GetAttribute(htmlElement, "face");
                  if (attribute1 != null)
                    localProperties[(object) "font-family"] = (object) attribute1;
                  string attribute2 = HtmlToXamlConverter.GetAttribute(htmlElement, "size");
                  if (attribute2 != null)
                  {
                    double num = double.Parse(attribute2, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture) * 4.0;
                    if (num < 1.0)
                      num = 1.0;
                    else if (num > 1000.0)
                      num = 1000.0;
                    localProperties[(object) "font-size"] = (object) num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
                  }
                  string attribute3 = HtmlToXamlConverter.GetAttribute(htmlElement, "color");
                  if (attribute3 != null)
                  {
                    localProperties[(object) "color"] = (object) attribute3;
                    goto label_58;
                  }
                  else
                    goto label_58;
                }
                else
                  goto label_58;
              case 'y':
                if (lowerInvariant == "body")
                  goto label_58;
                else
                  goto label_58;
              default:
                goto label_58;
            }
          case 5:
            if (lowerInvariant == "table")
              goto label_58;
            else
              goto label_58;
          case 6:
            switch (lowerInvariant[0])
            {
              case 'i':
                if (lowerInvariant == "italic")
                  break;
                goto label_58;
              case 's':
                if (lowerInvariant == "strong")
                  goto label_35;
                else
                  goto label_58;
              default:
                goto label_58;
            }
            break;
          case 7:
            if (lowerInvariant == "acronym")
              goto label_58;
            else
              goto label_58;
          case 9:
            if (lowerInvariant == "underline")
              goto label_36;
            else
              goto label_58;
          case 10:
            if (lowerInvariant == "blockquote")
            {
              localProperties[(object) "margin-left"] = (object) "16";
              goto label_58;
            }
            else
              goto label_58;
          default:
            goto label_58;
        }
        localProperties[(object) "font-style"] = (object) "italic";
        goto label_58;
label_35:
        localProperties[(object) "font-weight"] = (object) "bold";
        goto label_58;
label_36:
        localProperties[(object) "text-decoration-underline"] = (object) "true";
      }
label_58:
      string attribute = HtmlToXamlConverter.GetAttribute(htmlElement, "xml:lang");
      if (!string.IsNullOrEmpty(attribute))
        localProperties[(object) "xml:lang"] = (object) attribute;
      HtmlCssParser.GetElementPropertiesFromCssAttributes(htmlElement, lowerInvariant, stylesheet, localProperties, sourceContext);
      IDictionaryEnumerator enumerator2 = localProperties.GetEnumerator();
      while (enumerator2.MoveNext())
        elementProperties[enumerator2.Key] = enumerator2.Value;
      return elementProperties;
    }

    private static string GetCssAttribute(string cssStyle, string attributeName)
    {
      if (cssStyle != null)
      {
        attributeName = attributeName.ToLowerInvariant();
        string str1 = cssStyle;
        char[] chArray1 = new char[1]{ ';' };
        foreach (string str2 in str1.Split(chArray1))
        {
          char[] chArray2 = new char[1]{ ':' };
          string[] strArray = str2.Split(chArray2);
          if (strArray.Length == 2 && string.Equals(strArray[0].Trim(), attributeName, StringComparison.OrdinalIgnoreCase))
            return strArray[1].Trim();
        }
      }
      return (string) null;
    }

    private static bool TryGetLengthValue(string lengthAsString, out double length)
    {
      length = double.NaN;
      if (lengthAsString != null)
      {
        lengthAsString = lengthAsString.Trim().ToLowerInvariant();
        if (lengthAsString.EndsWith("pt"))
        {
          lengthAsString = lengthAsString.Substring(0, lengthAsString.Length - 2);
          length = !double.TryParse(lengthAsString, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out length) ? double.NaN : length * 96.0 / 72.0;
        }
        else if (lengthAsString.EndsWith("px"))
        {
          lengthAsString = lengthAsString.Substring(0, lengthAsString.Length - 2);
          if (!double.TryParse(lengthAsString, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out length))
            length = double.NaN;
        }
        else if (!double.TryParse(lengthAsString, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out length))
          length = double.NaN;
      }
      return !double.IsNaN(length);
    }

    private static string GetColorValue(string colorValue) => colorValue;

    private static void ApplyPropertiesToTableCellElement(
      XmlElement htmlChildNode,
      XmlElement xamlTableCellElement)
    {
      xamlTableCellElement.SetAttribute("BorderThickness", "1,1,1,1");
      xamlTableCellElement.SetAttribute("BorderBrush", "Black");
      string attribute = HtmlToXamlConverter.GetAttribute(htmlChildNode, "rowspan");
      if (attribute == null)
        return;
      xamlTableCellElement.SetAttribute("RowSpan", attribute);
    }
  }
}
