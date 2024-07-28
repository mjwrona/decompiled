// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DocumentBuilder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Web;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DocumentBuilder
  {
    private List<string> m_fields;
    private List<string> m_values;
    private IVssRequestContext m_requestContext;

    internal DocumentBuilder(IVssRequestContext requestContext)
    {
      this.m_fields = new List<string>();
      this.m_values = new List<string>();
      this.m_requestContext = requestContext;
    }

    internal XmlDocument RenderWorkItemXml(Payload workItemPayload, string userLocale)
    {
      this.m_requestContext.TraceEnter(900158, "DataAccessLayer", nameof (DocumentBuilder), nameof (RenderWorkItemXml));
      XmlDocument xmlDocument = new XmlDocument();
      XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", "yes");
      xmlDocument.AppendChild((XmlNode) xmlDeclaration);
      XmlElement element1 = xmlDocument.CreateElement("WorkItem");
      xmlDocument.AppendChild((XmlNode) element1);
      XmlElement element2 = xmlDocument.CreateElement("Classification");
      element1.AppendChild((XmlNode) element2);
      XmlElement element3 = xmlDocument.CreateElement("RevisionFields");
      element1.AppendChild((XmlNode) element3);
      XmlElement element4 = xmlDocument.CreateElement("Description");
      element1.AppendChild((XmlNode) element4);
      XmlElement element5 = xmlDocument.CreateElement("Discussion");
      element1.AppendChild((XmlNode) element5);
      XmlElement element6 = xmlDocument.CreateElement("References");
      element1.AppendChild((XmlNode) element6);
      XmlElement element7 = xmlDocument.CreateElement("RelatedLinks");
      element6.AppendChild((XmlNode) element7);
      XmlElement element8 = xmlDocument.CreateElement("HyperLinks");
      element6.AppendChild((XmlNode) element8);
      XmlElement element9 = xmlDocument.CreateElement("ExternalLinks");
      element6.AppendChild((XmlNode) element9);
      XmlElement element10 = xmlDocument.CreateElement("Attachments");
      element6.AppendChild((XmlNode) element10);
      try
      {
        CultureInfo cultureInfo = CultureInfo.InvariantCulture;
        try
        {
          string validSetLocale = DocumentBuilder.AcceptLanguageToValidSetLocale(userLocale);
          if (!string.IsNullOrEmpty(validSetLocale))
            cultureInfo = CultureInfo.CreateSpecificCulture(validSetLocale);
        }
        catch (Exception ex)
        {
          cultureInfo = CultureInfo.InvariantCulture;
        }
        Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
        Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
        PayloadTable table1 = workItemPayload.Tables["Fields"];
        this.m_requestContext.Trace(900168, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Table: {0}", (object) table1.TableName);
        foreach (PayloadTable.PayloadRow row in workItemPayload.Tables[table1.TableName].Rows)
        {
          this.m_requestContext.Trace(900169, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Field: {0}", (object) row["ReferenceName"].ToString());
          dictionary1.Add(row["ReferenceName"].ToString(), row["Name"].ToString());
          dictionary2.Add(row["ReferenceName"].ToString(), DocumentBuilder.GetFieldTypeById(Convert.ToInt32(row["Type"], (IFormatProvider) CultureInfo.InvariantCulture)));
        }
        string localTimeZoneOffset = TFCommonUtil.GetLocalTimeZoneOffset();
        string empty1 = string.Empty;
        string str1 = !TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now) ? TimeZone.CurrentTimeZone.StandardName : TimeZone.CurrentTimeZone.DaylightName;
        element1.SetAttribute("TimeZone", str1);
        element1.SetAttribute("Offset", localTimeZoneOffset);
        PayloadTable table2 = workItemPayload.Tables["WorkItemInfo"];
        int count1 = table2.Columns.Count;
        List<string> stringList = new List<string>();
        stringList.Add("System.PersonId");
        stringList.Add("System.ExternalLinkCount");
        stringList.Add("System.HyperLinkCount");
        stringList.Add("System.AttachedFileCount");
        stringList.Add("System.ChangedOrder");
        stringList.Add("System.RevisedDate");
        stringList.Add("System.AreaId");
        stringList.Add("System.RelatedLinkCount");
        stringList.Add("System.Parent");
        stringList.Add("System.NodeName");
        stringList.Add("System.NodeType");
        stringList.Add("System.IterationId");
        stringList.Add("System.History");
        this.m_requestContext.Trace(900170, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Table: {0}", (object) table2.TableName);
        if (workItemPayload.Tables[table2.TableName].Rows.Count == 0)
          throw new LegacyDeniedOrNotExist(DalResourceStrings.Get("AccessDeniedOrDoesNotExist"));
        foreach (PayloadTable.PayloadRow row in workItemPayload.Tables[table2.TableName].Rows)
        {
          this.m_requestContext.Trace(900159, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Row");
          this.m_requestContext.Trace(900160, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Add core fields");
          this.AddField("System.AssignedTo", row, cultureInfo);
          this.AddField("System.State", row, cultureInfo);
          this.AddField("System.Reason", row, cultureInfo);
          this.AddField("System.ChangedBy", row, cultureInfo);
          this.AddField("System.ChangedDate", row, cultureInfo);
          this.AddField("System.CreatedBy", row, cultureInfo);
          this.AddField("System.CreatedDate", row, cultureInfo);
          this.m_requestContext.Trace(900161, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Get rest of fields");
          for (int index = 0; index < count1; ++index)
          {
            this.m_requestContext.Trace(900171, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Column {0}", (object) index);
            string name = table2.Columns[index].Name;
            string str2 = string.Empty;
            object obj = row[index];
            if (obj != null)
              str2 = !(obj is DateTime) ? row[index].ToString() : DocumentBuilder.GetDateFieldString(this.m_requestContext, Convert.ToDateTime(row[index], (IFormatProvider) CultureInfo.InvariantCulture));
            if (TFStringComparer.WorkItemFieldReferenceName.Equals(name, "System.AreaPath") || TFStringComparer.WorkItemFieldReferenceName.Equals(name, "System.IterationPath"))
              str2 = DataAccessLayerImpl.TranslatePath(str2);
            this.m_requestContext.Trace(900172, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "{0}:{1}", (object) name, (object) str2);
            if (TFStringComparer.WorkItemFieldReferenceName.Equals(name, "System.Id") || TFStringComparer.WorkItemFieldReferenceName.Equals(name, "System.AreaPath") || TFStringComparer.WorkItemFieldReferenceName.Equals(name, "System.IterationPath") || TFStringComparer.WorkItemFieldReferenceName.Equals(name, "System.Title") || TFStringComparer.WorkItemFieldReferenceName.Equals(name, "System.WorkItemType"))
            {
              XmlElement element11 = xmlDocument.CreateElement(name);
              XmlText textNode = xmlDocument.CreateTextNode(str2);
              element11.AppendChild((XmlNode) textNode);
              element2.AppendChild((XmlNode) element11);
              this.m_requestContext.Trace(900173, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Value: {0}", (object) str2);
            }
            else if (!stringList.Contains(name) && !this.m_fields.Contains(name) && str2.Length > 0)
            {
              this.m_fields.Add(name);
              this.m_values.Add(str2);
            }
          }
          int index1 = 0;
          foreach (string field in this.m_fields)
          {
            XmlElement element12 = xmlDocument.CreateElement("Field");
            element12.SetAttribute("ReferenceName", field);
            this.m_requestContext.Trace(900174, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Field: {0}", (object) field);
            element12.SetAttribute("Name", dictionary1[field]);
            this.m_requestContext.Trace(900175, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Field Name: {0}", (object) dictionary1[field]);
            element12.SetAttribute("Type", dictionary2[field]);
            this.m_requestContext.Trace(900176, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Field Type: {0}", (object) dictionary2[field]);
            XmlElement element13 = xmlDocument.CreateElement("Value");
            XmlText textNode = xmlDocument.CreateTextNode(this.m_values[index1]);
            element13.AppendChild((XmlNode) textNode);
            this.m_requestContext.Trace(900177, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Value: {0}", (object) this.m_values[index1]);
            element12.AppendChild((XmlNode) element13);
            element3.AppendChild((XmlNode) element12);
            ++index1;
          }
        }
        PayloadTable table3 = workItemPayload.Tables["Texts"];
        Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
        this.m_requestContext.Trace(900178, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Table: {0}", (object) table3.TableName);
        foreach (PayloadTable.PayloadRow row in workItemPayload.Tables[table3.TableName].Rows)
        {
          if (!(row["ReferenceName"].ToString() == "System.History"))
          {
            string key = row["ReferenceName"].ToString();
            string str3 = row["Words"].ToString();
            if (!dictionary3.ContainsKey(key))
              dictionary3.Add(key, str3);
            else
              dictionary3[key] = str3;
          }
        }
        foreach (KeyValuePair<string, string> keyValuePair in dictionary3)
        {
          this.m_requestContext.Trace(900162, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Row");
          string str4 = keyValuePair.Value;
          string text = string.Compare(dictionary2[keyValuePair.Key], "HTML", StringComparison.OrdinalIgnoreCase) != 0 ? DocumentBuilder.CleanLongText(HttpUtility.HtmlEncode(str4)) : DocumentBuilder.CleanHTML(str4);
          if (TFStringComparer.WorkItemFieldReferenceName.Equals(keyValuePair.Key, "System.Description"))
          {
            if (text.Length > 0)
            {
              XmlElement element14 = xmlDocument.CreateElement("Text");
              XmlText textNode = xmlDocument.CreateTextNode(text);
              element14.AppendChild((XmlNode) textNode);
              element4.AppendChild((XmlNode) element14);
              this.m_requestContext.Trace(900179, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Description = {0}", (object) text);
            }
          }
          else
          {
            XmlElement element15 = xmlDocument.CreateElement("FieldX");
            element15.SetAttribute("ReferenceName", keyValuePair.Key);
            this.m_requestContext.Trace(900180, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Field: {0}", (object) keyValuePair.Key);
            element15.SetAttribute("Name", dictionary1[keyValuePair.Key]);
            this.m_requestContext.Trace(900181, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Field Name: {0}", (object) dictionary1[keyValuePair.Key]);
            element15.SetAttribute("Type", dictionary2[keyValuePair.Key]);
            this.m_requestContext.Trace(900182, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Field Type: {0}", (object) dictionary2[keyValuePair.Key]);
            XmlElement element16 = xmlDocument.CreateElement("Value");
            XmlText textNode = xmlDocument.CreateTextNode(text);
            element16.AppendChild((XmlNode) textNode);
            this.m_requestContext.Trace(900183, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Value: {0}", (object) text);
            element15.AppendChild((XmlNode) element16);
            element3.AppendChild((XmlNode) element15);
          }
        }
        PayloadTable table4 = workItemPayload.Tables["History"];
        this.m_requestContext.Trace(900184, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Table: {0}", (object) table4.TableName);
        int count2 = table4.Columns.Count;
        foreach (PayloadTable.PayloadRow row in workItemPayload.Tables[table4.TableName].Rows)
        {
          this.m_requestContext.Trace(900163, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Row");
          XmlElement element17 = xmlDocument.CreateElement("Entry");
          element5.AppendChild((XmlNode) element17);
          for (int index = 0; index < count2; ++index)
          {
            this.m_requestContext.Trace(900185, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Field: {0}", (object) table4.Columns[index].Name);
            this.m_requestContext.Trace(900186, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Value: {0}", (object) row[index].ToString());
            if (VssStringComparer.FieldName.Equals(table4.Columns[index].Name, "ChangedBy"))
            {
              XmlElement element18 = xmlDocument.CreateElement("ChangedBy");
              XmlText textNode = xmlDocument.CreateTextNode(row[index].ToString());
              element18.AppendChild((XmlNode) textNode);
              element17.AppendChild((XmlNode) element18);
            }
            else if (VssStringComparer.FieldName.Equals(table4.Columns[index].Name, "AddedDate"))
            {
              XmlElement element19 = xmlDocument.CreateElement("AddedDate");
              string text = Convert.ToDateTime(row[index], (IFormatProvider) CultureInfo.InvariantCulture).ToLocalTime().ToString((IFormatProvider) cultureInfo);
              XmlText textNode = xmlDocument.CreateTextNode(text);
              element19.AppendChild((XmlNode) textNode);
              element17.AppendChild((XmlNode) element19);
            }
            else if (VssStringComparer.FieldName.Equals(table4.Columns[index].Name, "Words"))
            {
              XmlElement element20 = xmlDocument.CreateElement("Text");
              string text = DocumentBuilder.CleanHTML(row[index].ToString());
              XmlText textNode = xmlDocument.CreateTextNode(text);
              element20.AppendChild((XmlNode) textNode);
              element17.AppendChild((XmlNode) element20);
            }
          }
        }
        PayloadTable table5 = workItemPayload.Tables["RelatedLinks"];
        this.m_requestContext.Trace(900187, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Table: {0}", (object) table5.TableName);
        int count3 = table5.Columns.Count;
        foreach (PayloadTable.PayloadRow row in workItemPayload.Tables[table5.TableName].Rows)
        {
          this.m_requestContext.Trace(900164, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Row");
          XmlElement element21 = xmlDocument.CreateElement("Related");
          element7.AppendChild((XmlNode) element21);
          for (int index = 0; index < count3; ++index)
          {
            string empty2 = string.Empty;
            if (row[index] != null)
              empty2 = row[index].ToString();
            this.m_requestContext.Trace(900188, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Field: {0}", (object) table5.Columns[index].Name);
            this.m_requestContext.Trace(900189, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Value: {0}", (object) empty2);
            switch (table5.Columns[index].Name)
            {
              case "ID":
                XmlElement element22 = xmlDocument.CreateElement("ID");
                XmlText textNode1 = xmlDocument.CreateTextNode(empty2);
                element22.AppendChild((XmlNode) textNode1);
                element21.AppendChild((XmlNode) element22);
                break;
              case "RelatedID":
                XmlElement element23 = xmlDocument.CreateElement("RelatedID");
                XmlText textNode2 = xmlDocument.CreateTextNode(empty2);
                element23.AppendChild((XmlNode) textNode2);
                element21.AppendChild((XmlNode) element23);
                break;
              case "Comment":
                XmlElement element24 = xmlDocument.CreateElement("Comment");
                XmlText textNode3 = xmlDocument.CreateTextNode(empty2);
                element24.AppendChild((XmlNode) textNode3);
                element21.AppendChild((XmlNode) element24);
                break;
              case "LinkType":
                XmlElement element25 = xmlDocument.CreateElement("LinkType");
                XmlText textNode4 = xmlDocument.CreateTextNode(empty2);
                element25.AppendChild((XmlNode) textNode4);
                element21.AppendChild((XmlNode) element25);
                break;
            }
          }
        }
        PayloadTable table6 = workItemPayload.Tables["Hyperlinks"];
        this.m_requestContext.Trace(900190, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Table: {0}", (object) table6.TableName);
        int count4 = table6.Columns.Count;
        foreach (PayloadTable.PayloadRow row in workItemPayload.Tables[table6.TableName].Rows)
        {
          this.m_requestContext.Trace(900165, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Row");
          XmlElement element26 = xmlDocument.CreateElement("Hyperlink");
          element8.AppendChild((XmlNode) element26);
          for (int index = 0; index < count4; ++index)
          {
            this.m_requestContext.Trace(900191, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Field: {0}", (object) table6.Columns[index].Name);
            this.m_requestContext.Trace(900192, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Value: {0}", (object) row[index].ToString());
            if (VssStringComparer.FieldName.Equals(table6.Columns[index].Name, "Url"))
            {
              XmlElement element27 = xmlDocument.CreateElement("Url");
              XmlText textNode = xmlDocument.CreateTextNode(row[index].ToString());
              element27.AppendChild((XmlNode) textNode);
              element26.AppendChild((XmlNode) element27);
            }
            else if (VssStringComparer.FieldName.Equals(table6.Columns[index].Name, "Comment"))
            {
              XmlElement element28 = xmlDocument.CreateElement("Comment");
              XmlText textNode = xmlDocument.CreateTextNode(row[index].ToString());
              element28.AppendChild((XmlNode) textNode);
              element26.AppendChild((XmlNode) element28);
            }
          }
        }
        TeamFoundationLinkingService service = this.m_requestContext.GetService<TeamFoundationLinkingService>();
        PayloadTable table7 = workItemPayload.Tables["ExternalLinks"];
        this.m_requestContext.Trace(900193, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Table: {0}", (object) table7.TableName);
        int count5 = table7.Columns.Count;
        foreach (PayloadTable.PayloadRow row in workItemPayload.Tables[table7.TableName].Rows)
        {
          this.m_requestContext.Trace(900166, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Row");
          string x = row["ArtifactName"].ToString();
          bool flag = false;
          string empty3 = string.Empty;
          string empty4 = string.Empty;
          if (VssStringComparer.ArtifactTool.Equals(x, "Fixed in Changeset"))
          {
            flag = true;
            empty3 = DalResourceStrings.Get("LinkTypeChangesetArtifactName");
            empty4 = DalResourceStrings.Get("LinkTypeChangeset");
          }
          if (VssStringComparer.ArtifactTool.Equals(x, "Source Code File"))
          {
            flag = true;
            empty3 = DalResourceStrings.Get("LinkTypeSourceCodeArtifactName");
            empty4 = DalResourceStrings.Get("LinkTypeSourceCode");
          }
          if (VssStringComparer.ArtifactTool.Equals(x, "Test Result"))
          {
            flag = true;
            empty3 = DalResourceStrings.Get("LinkTypeTestResultArtifactName");
            empty4 = DalResourceStrings.Get("LinkTypeTestResult");
          }
          if (flag)
          {
            string text1 = row["Comment"].ToString();
            string str5 = row["Uri"].ToString();
            XmlElement element29 = xmlDocument.CreateElement("ExternalLink");
            element9.AppendChild((XmlNode) element29);
            XmlElement element30 = xmlDocument.CreateElement("Uri");
            XmlText textNode5 = xmlDocument.CreateTextNode(str5);
            element30.AppendChild((XmlNode) textNode5);
            element29.AppendChild((XmlNode) element30);
            XmlElement element31 = xmlDocument.CreateElement("Url");
            string text2;
            try
            {
              text2 = service.GetArtifactUrlExternal(this.m_requestContext, str5);
            }
            catch (ArgumentException ex)
            {
              text2 = str5;
            }
            XmlText textNode6 = xmlDocument.CreateTextNode(text2);
            element31.AppendChild((XmlNode) textNode6);
            element29.AppendChild((XmlNode) element31);
            string text3;
            try
            {
              ArtifactId artifactId = LinkingUtilities.DecodeUri(str5);
              text3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, empty3, (object) artifactId.ToolSpecificId);
            }
            catch (ArgumentException ex)
            {
              text3 = str5;
            }
            XmlElement element32 = xmlDocument.CreateElement("Description");
            XmlText textNode7 = xmlDocument.CreateTextNode(text3);
            element32.AppendChild((XmlNode) textNode7);
            element29.AppendChild((XmlNode) element32);
            XmlElement element33 = xmlDocument.CreateElement("Artifact");
            XmlText textNode8 = xmlDocument.CreateTextNode(empty4);
            element33.AppendChild((XmlNode) textNode8);
            element29.AppendChild((XmlNode) element33);
            XmlElement element34 = xmlDocument.CreateElement("Comment");
            XmlText textNode9 = xmlDocument.CreateTextNode(text1);
            element34.AppendChild((XmlNode) textNode9);
            element29.AppendChild((XmlNode) element34);
          }
        }
        PayloadTable table8 = workItemPayload.Tables["Files"];
        this.m_requestContext.Trace(900194, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Table: {0}", (object) table8.TableName);
        int count6 = table8.Columns.Count;
        foreach (PayloadTable.PayloadRow row in workItemPayload.Tables[table8.TableName].Rows)
        {
          this.m_requestContext.Trace(900167, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Row");
          XmlElement element35 = xmlDocument.CreateElement("Attachment");
          element10.AppendChild((XmlNode) element35);
          for (int index = 0; index < count6; ++index)
          {
            this.m_requestContext.Trace(900195, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Field: {0}", (object) table8.Columns[index].Name);
            this.m_requestContext.Trace(900196, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "Value: {0}", (object) row[index].ToString());
            if (VssStringComparer.FieldName.Equals(table8.Columns[index].Name, "ID"))
            {
              XmlElement element36 = xmlDocument.CreateElement("ID");
              XmlText textNode = xmlDocument.CreateTextNode(row[index].ToString());
              element36.AppendChild((XmlNode) textNode);
              element35.AppendChild((XmlNode) element36);
            }
            else if (VssStringComparer.FieldName.Equals(table8.Columns[index].Name, "FileName"))
            {
              XmlElement element37 = xmlDocument.CreateElement("FileName");
              XmlText textNode = xmlDocument.CreateTextNode(row[index].ToString());
              element37.AppendChild((XmlNode) textNode);
              element35.AppendChild((XmlNode) element37);
            }
            else if (VssStringComparer.FieldName.Equals(table8.Columns[index].Name, "Length"))
            {
              XmlElement element38 = xmlDocument.CreateElement("Size");
              int num = 0;
              try
              {
                num = Convert.ToInt32(row[index], (IFormatProvider) CultureInfo.InvariantCulture) / 1000;
              }
              catch
              {
              }
              XmlText textNode = xmlDocument.CreateTextNode(num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
              element38.AppendChild((XmlNode) textNode);
              element35.AppendChild((XmlNode) element38);
            }
            else if (VssStringComparer.FieldName.Equals(table8.Columns[index].Name, "Comment"))
            {
              XmlElement element39 = xmlDocument.CreateElement("Comment");
              XmlText textNode = xmlDocument.CreateTextNode(row[index].ToString());
              element39.AppendChild((XmlNode) textNode);
              element35.AppendChild((XmlNode) element39);
            }
          }
        }
      }
      catch (LegacyServerException ex)
      {
        if (xmlDocument.HasChildNodes)
          xmlDocument.RemoveAll();
        XmlElement element40 = xmlDocument.CreateElement("Error");
        XmlElement element41 = xmlDocument.CreateElement("Message");
        XmlText textNode = xmlDocument.CreateTextNode(ex.Message);
        element41.AppendChild((XmlNode) textNode);
        element40.AppendChild((XmlNode) element41);
        xmlDocument.AppendChild((XmlNode) element40);
      }
      this.m_requestContext.Trace(900502, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), xmlDocument.OuterXml);
      this.m_requestContext.TraceLeave(900503, "DataAccessLayer", nameof (DocumentBuilder), nameof (RenderWorkItemXml));
      return xmlDocument;
    }

    private static string CleanHTML(string html) => html.Replace(Environment.NewLine, string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\\", "&#92;").Replace("'", "\\'");

    private static string CleanLongText(string text)
    {
      string str = text;
      string newValue1 = "<br/>";
      string newLine = Environment.NewLine;
      string newValue2 = newValue1;
      return str.Replace(newLine, newValue2).Replace("\n", newValue1).Replace("\r", newValue1).Replace("\\", "&#92;").Replace("'", "\\'");
    }

    private void AddField(string fieldName, PayloadTable.PayloadRow row, CultureInfo ci)
    {
      string str = string.Empty;
      object obj = row[fieldName];
      if (obj != null)
        str = !(obj is DateTime) ? row[fieldName].ToString() : Convert.ToDateTime(row[fieldName], (IFormatProvider) CultureInfo.InvariantCulture).ToLocalTime().ToString((IFormatProvider) ci);
      this.m_fields.Add(fieldName);
      this.m_values.Add(str);
      this.m_requestContext.Trace(900197, TraceLevel.Verbose, "DataAccessLayer", nameof (DocumentBuilder), "{0}:{1}", (object) fieldName, (object) str);
    }

    private static string GetFieldTypeById(int dataType)
    {
      dataType &= 65532;
      string fieldTypeById;
      switch (dataType)
      {
        case 16:
          fieldTypeById = "String";
          break;
        case 32:
          fieldTypeById = "Integer";
          break;
        case 48:
          fieldTypeById = "DateTime";
          break;
        case 64:
          fieldTypeById = "PlainText";
          break;
        case 160:
          fieldTypeById = "TreeNode";
          break;
        case 240:
          fieldTypeById = "Double";
          break;
        case 576:
          fieldTypeById = "HTML";
          break;
        default:
          fieldTypeById = "";
          break;
      }
      return fieldTypeById;
    }

    internal static string GetDateFieldString(IVssRequestContext requestContext, DateTime date)
    {
      Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
      date = date.ToLocalTime();
      if (date > calendar.MaxSupportedDateTime)
        date = calendar.MaxSupportedDateTime;
      else if (date < calendar.MinSupportedDateTime)
        date = calendar.MinSupportedDateTime;
      return date.ToString((IFormatProvider) requestContext.ServiceHost.GetCulture(requestContext));
    }

    private static string AcceptLanguageToValidSetLocale(string userLocale)
    {
      string validSetLocale = string.Empty;
      if (!string.IsNullOrEmpty(userLocale))
      {
        int length = userLocale.IndexOf(",", 1, StringComparison.OrdinalIgnoreCase);
        validSetLocale = length <= 0 ? userLocale : userLocale.Substring(0, length);
      }
      return validSetLocale;
    }
  }
}
