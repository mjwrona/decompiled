// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Presentation.HtmlFormatter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Presentation
{
  internal class HtmlFormatter
  {
    private const string c_htmlDocumentLabel = "html";
    private const string c_htmlHeaderLabel = "head";
    private const string c_htmlBodyLabel = "body";
    private const string c_htmlLineBreakLabel = "br";
    private const string c_htmlTableLabel = "table";
    private const string c_htmlBorderLabel = "border";
    private const string c_htmlBorderColorLabel = "bordercolor";
    private const string c_htmlColumnWidthLabel = "width";
    private const string c_htmlColumnGroupLabel = "colgroup";
    private const string c_htmlColumnLabel = "col";
    private const string c_htmlColumnSpanLabel = "span";
    private const string c_htmlCellPaddingLabel = "cellpadding";
    private const string c_htmlCellSpacingLabel = "cellspacing";
    private const string c_htmlTableHeaderLabel = "th";
    private const string c_htmlTableRowLabel = "tr";
    private const string c_htmlTableCellLabel = "td";
    private const string c_htmlAnchorLabel = "a";
    private const string c_htmlHrefLabel = "href";
    private const string c_htmlBoldLabel = "b";
    private const string c_htmlTextcolorLabel = "text";
    private const string c_htmlSpanLabel = "span";
    private const string c_htmlStyleLabel = "style";
    private const string c_htmlParagraphTextLabel = "p";
    private const string c_htmlTargetLabel = "target";
    private const string c_htmlTabIndex = "tabindex";
    private const string c_htmlDefaultBodyStyle = "font-family:Calibri";
    private const int c_htmlBorderValue = 0;
    private const int c_htmlColumnWidthValue = 200;
    private const int c_htmlCellPaddingValue = 0;
    private const int c_htmlCellSpacingValue = 0;
    private const string c_generalLongTimeFormat = "G";
    private const string c_htmlTargetBlankValue = "_blank";
    private const string c_headerStyle = "-size:11pt;font-family:\"Calibri\",\"sans-serif\";border:none;margin:0px;";
    private const string c_tableStyle = "-size:11pt;font-family:\"Calibri\",\"sans-serif\";border-collapse:collapse";
    private const string c_cellStyle = "border:none;border-right:solid white 1.0pt;padding:1.45pt .05in 1.45pt .05in;font-family:Segoe UI, Helvetica Neue, Helvetica, Arial, Verdana;font-size:12px";
    private const string c_headerColorCode = "#106EBE";
    private const string c_rowBgColorCode = "#FFFFFF";
    private const string c_rowBgColorCodeAlternate = "#F8F8F8";
    private const string c_defaultIdColumnName = "ID";
    private const string c_defaultTitleColumnName = "Title";
    private const int c_defaultColumnCount = 1;
    private const string c_themeRowBgColorCode = "";
    private const string c_themeRowBgColorCodeAlternate = "rgba(124, 124, 124, 0.5)";
    private Dictionary<string, string> m_columnNameDictionary = new Dictionary<string, string>(1);

    public void SetDefaultColumnDisplayName(string corefieldReferenceName, string fieldDisplayName) => this.m_columnNameDictionary[corefieldReferenceName] = fieldDisplayName;

    public string FormatInputData(
      FormattingWorkItemCollection workItemList,
      IEnumerable<FormattingFieldDefinition> displayFieldList,
      IWorkItemUrlBuilder builder,
      bool useThemeSpecificBgColor = false)
    {
      return new XElement((XName) "html", new object[2]
      {
        (object) new XElement((XName) "head"),
        (object) new XElement((XName) "body", new object[6]
        {
          (object) new XAttribute((XName) "style", (object) "font-family:Calibri"),
          (object) new XElement((XName) "br"),
          (object) HtmlFormatter.CreateHeader(workItemList, builder),
          (object) new XElement((XName) "br"),
          (object) this.CreateWorkItemTable(workItemList, displayFieldList, builder, useThemeSpecificBgColor),
          (object) new XElement((XName) "br")
        })
      }).ToString();
    }

    private static XElement CreateHeader(
      FormattingWorkItemCollection workItemCollection,
      IWorkItemUrlBuilder builder)
    {
      XElement header = new XElement((XName) "p", (object) new XAttribute((XName) "style", (object) "-size:11pt;font-family:\"Calibri\",\"sans-serif\";border:none;margin:0px;"));
      if (workItemCollection.QueryId.HasValue && workItemCollection.ProjectUri != (Uri) null)
      {
        string resultsHyperlink = builder.GetQueryResultsHyperlink(workItemCollection.ProjectUri, workItemCollection.QueryId.Value);
        if (!string.IsNullOrEmpty(resultsHyperlink))
          header.Add((object) new XElement((XName) "b", (object) new XElement((XName) "span", new object[2]
          {
            (object) new XAttribute((XName) "style", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "padding:0in .05in 0in 0in;margin-bottom:4.0pt;color:{0}", (object) "#106EBE")),
            (object) InternalsResourceStrings.Get("FormatterQuery")
          })), (object) new XElement((XName) "span", new object[2]
          {
            (object) new XAttribute((XName) "style", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "margin-bottom:4.0pt;color:{0}", (object) "#106EBE")),
            (object) HtmlFormatter.GetAnchorNode(resultsHyperlink, (object) workItemCollection.QueryName)
          }));
      }
      return header;
    }

    private XElement CreateWorkItemTable(
      FormattingWorkItemCollection workItemCollection,
      IEnumerable<FormattingFieldDefinition> displayFieldList,
      IWorkItemUrlBuilder builder,
      bool useThemeSpecificBgColor = false)
    {
      XElement workItemTable = new XElement((XName) "table", new object[4]
      {
        (object) new XAttribute((XName) "border", (object) 0),
        (object) new XAttribute((XName) "cellpadding", (object) 0),
        (object) new XAttribute((XName) "cellspacing", (object) 0),
        (object) new XAttribute((XName) "style", (object) "-size:11pt;font-family:\"Calibri\",\"sans-serif\";border-collapse:collapse")
      });
      workItemTable.Add((object) this.CreateWorkItemHeader(displayFieldList));
      bool flag = true;
      foreach (FormattingWorkItem workItem in (IEnumerable<FormattingWorkItem>) workItemCollection.WorkItems)
      {
        try
        {
          string currentRowBgColor = !useThemeSpecificBgColor ? (flag ? "#FFFFFF" : "#F8F8F8") : (flag ? "" : "rgba(124, 124, 124, 0.5)");
          XElement workItemRow = HtmlFormatter.CreateWorkItemRow(workItem, currentRowBgColor, displayFieldList, builder, workItemCollection.ProjectUri);
          workItemTable.Add((object) workItemRow);
          flag = !flag;
        }
        catch (Exception ex)
        {
        }
      }
      return workItemTable;
    }

    private XElement CreateWorkItemHeader(
      IEnumerable<FormattingFieldDefinition> displayFieldList)
    {
      XElement workItemHeader = new XElement((XName) "tr", new object[2]
      {
        (object) new XAttribute((XName) "valign", (object) "top"),
        (object) new XAttribute((XName) "style", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "background:{0};color:white", (object) "#106EBE"))
      });
      workItemHeader.Add((object) new XElement((XName) "th", new object[2]
      {
        (object) new XAttribute((XName) "style", (object) "border:none;border-right:solid white 1.0pt;padding:1.45pt .05in 1.45pt .05in;font-family:Segoe UI, Helvetica Neue, Helvetica, Arial, Verdana;font-size:12px"),
        !this.m_columnNameDictionary.ContainsKey("System.Id") || this.m_columnNameDictionary["System.Id"] == null ? (object) "ID" : (object) this.m_columnNameDictionary["System.Id"]
      }));
      workItemHeader.Add((object) new XElement((XName) "th", new object[2]
      {
        (object) new XAttribute((XName) "style", (object) "border:none;border-right:solid white 1.0pt;padding:1.45pt .05in 1.45pt .05in;font-family:Segoe UI, Helvetica Neue, Helvetica, Arial, Verdana;font-size:12px"),
        !this.m_columnNameDictionary.ContainsKey("System.Title") || this.m_columnNameDictionary["System.Title"] == null ? (object) "Title" : (object) this.m_columnNameDictionary["System.Title"]
      }));
      List<XElement> xelementList = new List<XElement>();
      foreach (FormattingFieldDefinition displayField in displayFieldList)
      {
        if (!this.m_columnNameDictionary.ContainsKey(displayField.ReferenceName) && !displayField.ReferenceName.Equals("System.Links.LinkType") && !displayField.ReferenceName.Equals("System.Title"))
          xelementList.Add(new XElement((XName) "th", new object[2]
          {
            (object) new XAttribute((XName) "style", (object) "border:none;border-right:solid white 1.0pt;padding:1.45pt .05in 1.45pt .05in;font-family:Segoe UI, Helvetica Neue, Helvetica, Arial, Verdana;font-size:12px"),
            (object) displayField.Name
          }));
      }
      workItemHeader.Add((object[]) xelementList.ToArray());
      return workItemHeader;
    }

    private static XElement CreateWorkItemRow(
      FormattingWorkItem wi,
      string currentRowBgColor,
      IEnumerable<FormattingFieldDefinition> displayFieldList,
      IWorkItemUrlBuilder builder,
      Uri projectUri)
    {
      List<object> objectList = new List<object>()
      {
        (object) new XAttribute((XName) "valign", (object) "top")
      };
      if (!string.IsNullOrEmpty(currentRowBgColor))
        objectList.Add((object) new XAttribute((XName) "style", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "background:{0}", (object) currentRowBgColor)));
      XElement workItemRow = new XElement((XName) "tr", objectList.ToArray());
      workItemRow.Add((object) new XElement((XName) "td", new object[2]
      {
        (object) new XAttribute((XName) "style", (object) "border:none;border-right:solid white 1.0pt;padding:1.45pt .05in 1.45pt .05in;font-family:Segoe UI, Helvetica Neue, Helvetica, Arial, Verdana;font-size:12px"),
        (object) HtmlFormatter.GetAnchorNode(builder.GetWorkItemHyperlink(projectUri, wi.Id), (object) wi.Id)
      }));
      string str = "";
      if (wi.FieldValues.ContainsKey("System.Title"))
        str = wi.FieldValues["System.Title"].ToString();
      workItemRow.Add((object) new XElement((XName) "td", new object[2]
      {
        (object) new XAttribute((XName) "style", (object) "border:none;border-right:solid white 1.0pt;padding:1.45pt .05in 1.45pt .05in;font-family:Segoe UI, Helvetica Neue, Helvetica, Arial, Verdana;font-size:12px"),
        (object) str
      }));
      List<XElement> xelementList = new List<XElement>();
      foreach (FormattingFieldDefinition displayField in displayFieldList)
      {
        if (!displayField.ReferenceName.Equals("System.Id") && !displayField.ReferenceName.Equals("System.Title") && !displayField.ReferenceName.Equals("System.Links.LinkType"))
        {
          if (!wi.FieldValues.ContainsKey(displayField.ReferenceName) || wi.FieldValues[displayField.ReferenceName] == null)
          {
            xelementList.Add(new XElement((XName) "td", new object[2]
            {
              (object) new XAttribute((XName) "style", (object) "border:none;border-right:solid white 1.0pt;padding:1.45pt .05in 1.45pt .05in;font-family:Segoe UI, Helvetica Neue, Helvetica, Arial, Verdana;font-size:12px"),
              (object) string.Empty
            }));
          }
          else
          {
            DateTime result;
            if (displayField.FieldType == InternalFieldType.DateTime && wi.FieldValues[displayField.ReferenceName] != null && DateTime.TryParse(wi.FieldValues[displayField.ReferenceName].ToString(), (IFormatProvider) CultureInfo.CurrentCulture, DateTimeStyles.None, out result))
            {
              xelementList.Add(new XElement((XName) "td", (object) result.ToString("G", (IFormatProvider) CultureInfo.CurrentCulture)));
            }
            else
            {
              string plainText = wi.FieldValues[displayField.ReferenceName].ToString();
              if (displayField.FieldType == InternalFieldType.Html)
                plainText = HtmlConverter.ConvertToPlainText(plainText);
              xelementList.Add(new XElement((XName) "td", new object[2]
              {
                (object) new XAttribute((XName) "style", (object) "border:none;border-right:solid white 1.0pt;padding:1.45pt .05in 1.45pt .05in;font-family:Segoe UI, Helvetica Neue, Helvetica, Arial, Verdana;font-size:12px"),
                (object) plainText
              }));
            }
          }
        }
      }
      workItemRow.Add((object[]) xelementList.ToArray());
      return workItemRow;
    }

    private static XElement GetAnchorNode(string url, object value) => new XElement((XName) "a", new object[4]
    {
      (object) new XAttribute((XName) "href", (object) url),
      (object) new XAttribute((XName) "target", (object) "_blank"),
      (object) new XAttribute((XName) "tabindex", (object) "0"),
      value
    });
  }
}
