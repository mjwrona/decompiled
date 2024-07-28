// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.LegacyFormDeserializer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormExtensions;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  internal class LegacyFormDeserializer
  {
    private static readonly string[] DEFAULT_LINK_COLUMNS = new string[1]
    {
      "System.Links.Description"
    };
    private const int c_defaultNumberOfSections = 3;
    private const string c_NumberOfSectionsAttribute = "NumberOfSections";
    private List<Group> m_attachmentGroups = new List<Group>();
    private Dictionary<string, List<Group>> m_linksGroups = new Dictionary<string, List<Group>>();
    private LinksControlTransformer m_linksControlTransformer = new LinksControlTransformer();
    private List<Group> m_logGroups = new List<Group>();
    private ControlLabelResolver m_labelResolver;
    private ITraceRequest m_tracer;
    private HashSet<string> m_pageIdSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private LayoutHelper m_layoutHelper;

    internal LegacyFormDeserializer(
      ITraceRequest tracer = null,
      ControlLabelResolver labelResolver = null,
      Layout oobLayout = null)
    {
      this.m_tracer = tracer;
      this.m_labelResolver = labelResolver;
      this.m_layoutHelper = new LayoutHelper(oobLayout);
    }

    internal Layout Deserialize(
      string formXml,
      string workItemTypeName,
      string processName,
      bool injectSystemPages)
    {
      ArgumentUtility.CheckForNull<string>(formXml, nameof (formXml));
      if (this.m_tracer != null)
        this.m_tracer.TraceEnter(909804, "FormLayout", "FormTransformsLayer", nameof (Deserialize));
      try
      {
        XmlDocument document = new XmlDocument();
        XmlReaderSettings settings = new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        };
        using (StringReader input = new StringReader(formXml))
        {
          using (XmlReader reader = XmlReader.Create((TextReader) input, settings))
            document.Load(reader);
        }
        Layout layout = this.TransformLayout(document, workItemTypeName, processName, injectSystemPages);
        this.m_layoutHelper.AutoInjectSystemWorkItemControls(layout);
        this.m_layoutHelper.FillRanksInLayout(layout);
        return layout;
      }
      catch (XmlException ex)
      {
        if (this.m_tracer != null)
          this.m_tracer.TraceException(909802, TraceLevel.Error, "FormLayout", "FormTransformsLayer", (Exception) ex);
        throw;
      }
      finally
      {
        if (this.m_tracer != null)
          this.m_tracer.TraceLeave(909805, "FormLayout", "FormTransformsLayer", nameof (Deserialize));
      }
    }

    private Layout TransformLayout(
      XmlDocument document,
      string workItemTypeName,
      string processName,
      bool injectSystemPages)
    {
      Layout layout = new Layout();
      XmlElement layoutElement = this.GetLayoutElement(document);
      if (layoutElement == null)
      {
        if (this.m_tracer != null)
          this.m_tracer.Trace(909801, TraceLevel.Warning, "FormLayout", "FormTransformsLayer", (string[]) null, "Could not find a layout elements in the form");
        return layout;
      }
      int result1 = 3;
      XmlAttribute attribute1 = layoutElement.Attributes["NumberOfSections"];
      if (attribute1 != null)
        int.TryParse(attribute1.Value, out result1);
      XmlAttribute attribute2 = layoutElement.Attributes[ProvisionAttributes.LayoutHideReadOnlyEmptyFields];
      bool result2;
      if (attribute2 != null && bool.TryParse(attribute2.Value, out result2))
        layout.ShowEmptyReadOnlyFields = new bool?(!result2);
      XmlNodeList xmlNodeList = layoutElement.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "./{0}/{1}", (object) ProvisionTags.TabGroup, (object) ProvisionTags.Tab));
      if (xmlNodeList == null || xmlNodeList.Count == 0)
      {
        Page page = this.CreatePage(string.Format("{0}.{1}.{2}", (object) processName, (object) workItemTypeName, (object) workItemTypeName), InternalsResourceStrings.Get("NewFormFirstPageTitle"), false, PageType.Custom);
        layout.Children.Add(page);
        this.FillSections(this.SelectGroups(layoutElement, page, layout), page, result1);
      }
      else
      {
        int num = 0;
        foreach (XmlElement pageElement in xmlNodeList)
        {
          string attribute3 = pageElement.GetAttribute(ProvisionAttributes.Label);
          Page page = this.CreatePage(string.Format("{0}.{1}.{2}", (object) processName, (object) workItemTypeName, (object) attribute3), attribute3, false, PageType.Custom);
          string attribute4 = pageElement.GetAttribute(ProvisionAttributes.ContributionId);
          if (!string.IsNullOrEmpty(attribute4))
            page.Contribution = new WitContribution(attribute4);
          layout.Children.Add(page);
          List<Group> groups = this.SelectGroups(pageElement, page, layout);
          if (num == 0)
          {
            List<Group> collection = this.SelectGroups(this.GetTopLevelNode(layoutElement, document), page, layout);
            groups.InsertRange(0, (IEnumerable<Group>) collection);
          }
          this.FillSections(groups, page, result1);
          ++num;
        }
      }
      if (injectSystemPages)
      {
        this.InjectHistoryPage(layout, workItemTypeName, processName);
        this.InjectLinksPage(layout, workItemTypeName, processName);
        this.InjectAttachmentsPage(layout, workItemTypeName, processName);
      }
      this.RemoveEmptyPages(layout);
      return layout;
    }

    private void RemoveEmptyPages(Layout layout)
    {
      List<Page> pageList = new List<Page>();
      foreach (Page child in (IEnumerable<Page>) layout.Children)
      {
        if (child.Children.Sum<Section>((Func<Section, int>) (s => s.Children.Count)) == 0 && !child.IsContribution)
          pageList.Add(child);
      }
      foreach (Page page in pageList)
        layout.Children.Remove(page);
    }

    private XmlElement GetLayoutElement(XmlDocument document)
    {
      XmlNodeList source = document.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) ProvisionTags.Form, (object) ProvisionTags.Layout));
      if (source == null || source.Count == 0)
        return (XmlElement) null;
      XmlElement layoutElement = source[0] as XmlElement;
      if (source.Count > 1)
      {
        XmlElement xmlElement1 = source.Cast<XmlElement>().FirstOrDefault<XmlElement>((Func<XmlElement, bool>) (x => ProvisionValues.LayoutTargetNewForm.Equals(x.GetAttribute(ProvisionAttributes.LayoutTarget), StringComparison.OrdinalIgnoreCase)));
        XmlElement xmlElement2 = source.Cast<XmlElement>().FirstOrDefault<XmlElement>((Func<XmlElement, bool>) (x => ProvisionValues.LayoutTargetWeb.Equals(x.GetAttribute(ProvisionAttributes.LayoutTarget), StringComparison.OrdinalIgnoreCase)));
        if (xmlElement1 != null)
          layoutElement = xmlElement1;
        else if (xmlElement2 != null)
          layoutElement = xmlElement2;
      }
      return layoutElement;
    }

    private XmlElement GetTopLevelNode(XmlElement layout, XmlDocument document)
    {
      XmlElement element = document.CreateElement("root");
      foreach (XmlElement selectNode in layout.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "./{0}|./{1}", (object) ProvisionTags.Group, (object) ProvisionTags.Control)))
        element.AppendChild((XmlNode) selectNode);
      return element;
    }

    private List<Group> SelectGroups(XmlElement pageElement, Page page, Layout layout)
    {
      List<Group> source = new List<Group>();
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "./{0}", (object) ProvisionTags.Control);
      XmlNodeList controlElements = pageElement.SelectNodes(xpath);
      if (controlElements != null && controlElements.Count > 0)
      {
        Group group = this.CreateGroup(page.Label);
        source.Add(group);
        this.AddControls(controlElements, group, page, layout);
      }
      foreach (XmlElement xmlElement in pageElement.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}/{1}[./{2}][not(ancestor::{3})]", (object) ProvisionTags.Group, (object) ProvisionTags.Column, (object) ProvisionTags.Control, (object) ProvisionTags.Tab)).Cast<XmlNode>().Concat<XmlNode>(pageElement.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}/{1}[./{2}][ancestor::{3}]|.//{3}[./{2}]", (object) ProvisionTags.Group, (object) ProvisionTags.Column, (object) ProvisionTags.Control, (object) ProvisionTags.Tab)).Cast<XmlNode>()))
      {
        string label = xmlElement.GetAttribute(ProvisionAttributes.Label);
        for (XmlElement parentNode = xmlElement.ParentNode as XmlElement; string.IsNullOrEmpty(label) && parentNode != null; parentNode = parentNode.ParentNode as XmlElement)
          label = parentNode.GetAttribute(ProvisionAttributes.Label);
        if (string.IsNullOrWhiteSpace(label))
          label = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemFormOtherGroups();
        Group group1 = source.FirstOrDefault<Group>((Func<Group, bool>) (g => string.Equals(label, g.Label, StringComparison.OrdinalIgnoreCase)));
        Group group2;
        if (group1 != null)
        {
          group2 = group1;
        }
        else
        {
          string.Format("{0}.{1}", (object) page.Id, (object) label);
          group2 = this.CreateGroup(label);
        }
        this.AddControls(xmlElement.SelectNodes(xpath), group2, page, layout);
        if (group1 == null)
          source.Add(group2);
      }
      return source;
    }

    private void AddControls(XmlNodeList controlElements, Group group, Page page, Layout layout)
    {
      foreach (XmlElement controlElement in controlElements)
      {
        Control control = this.CreateControl();
        control.ControlType = controlElement.GetAttribute(ProvisionAttributes.ControlType);
        control.Name = controlElement.GetAttribute(ProvisionAttributes.ControlName);
        string attribute1 = controlElement.GetAttribute(ProvisionAttributes.ControlFieldName);
        control.Id = attribute1 ?? string.Empty;
        control.Label = controlElement.GetAttribute(ProvisionAttributes.Label);
        control.Watermark = controlElement.GetAttribute(ProvisionAttributes.ControlEmptyText);
        string attribute2 = controlElement.GetAttribute(ProvisionAttributes.ControlReadOnly);
        bool flag = false;
        ref bool local = ref flag;
        if (!bool.TryParse(attribute2, out local))
          flag = false;
        control.ReadOnly = new bool?(flag);
        control.Metadata = controlElement.InnerXml;
        if (this.m_labelResolver != null && string.IsNullOrEmpty(control.Label) && !string.IsNullOrWhiteSpace(control.Id) && !control.Id.Equals("System.Title") && !LayoutConstants.WideControls.Contains(control.ControlType))
          control.Label = this.m_labelResolver(control.Id) ?? control.Label;
        if (!LayoutConstants.FieldsNotInjectedDuringLegacyFormDeserialization.Contains(control.Id) && !LayoutConstants.SystemControls.Contains(control.ControlType))
          group.Children.Add(control);
        else if (WebLayoutXmlHelper.HeaderFields.Contains(control.Id))
        {
          control.Metadata = (string) null;
          layout.SystemControls.Add(control);
        }
        else if (WebLayoutXmlHelper.HeaderControls.Contains(control.ControlType) && !layout.SystemControls.Any<Control>((Func<Control, bool>) (c => c.ControlType.Equals(control.ControlType))))
        {
          if (string.Equals(control.ControlType, WellKnownControlNames.AttachmentsControl, StringComparison.OrdinalIgnoreCase))
          {
            control.Name = "Attachments";
            control.Id = control.Name;
            control.Label = this.m_layoutHelper.GetSystemPageLabelFromLayouts(layout, WellKnownControlNames.AttachmentsControl, WebLayoutXmlHelper.WorkItemFormAttachmentsLabel);
            layout.SystemControls.Add(control);
          }
          if (string.Equals(control.ControlType, WellKnownControlNames.LinksControl, StringComparison.OrdinalIgnoreCase))
          {
            Control control1 = this.CreateControl();
            control1.ControlType = WellKnownControlNames.LinksControl;
            control1.Label = this.m_layoutHelper.GetSystemPageLabelFromLayouts(layout, WellKnownControlNames.LinksControl, WebLayoutXmlHelper.WorkItemFormLinksLabel);
            control1.Name = "Links";
            control1.Id = control1.Name;
            layout.SystemControls.Add(control1);
          }
        }
        if (LayoutConstants.HeaderFieldsExistInBody.Contains(control.Id))
          group.Children.Add(control);
        if (LayoutConstants.SystemControls.Contains(control.ControlType))
        {
          Group group1 = new Group();
          group1.Label = group.Label;
          group1.Children.Add(control);
          if (control.ControlType == WellKnownControlNames.AttachmentsControl)
            this.m_attachmentGroups.Add(group1);
          else if (control.ControlType == WellKnownControlNames.LinksControl)
          {
            if (!this.m_linksGroups.ContainsKey(page.Id))
              this.m_linksGroups[page.Id] = new List<Group>();
            this.m_linksGroups[page.Id].Add(group1);
          }
          else if (control.ControlType == WellKnownControlNames.WorkItemLogControl)
            this.m_logGroups.Add(group1);
        }
      }
    }

    private void FillSections(List<Group> groups, Page page, int numSections)
    {
      Section[] sectionArray = new Section[numSections];
      for (int index = 0; index < numSections; ++index)
        sectionArray[index] = page.SectionFromIndex(index);
      int num1 = 0;
      List<Group> groupList = new List<Group>();
      HashSet<string> existingIds = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (Group group1 in groups)
      {
        List<Control> list1 = group1.Children.Where<Control>((Func<Control, bool>) (x => LayoutConstants.WideControls.Contains(x.ControlType))).ToList<Control>();
        HashSet<string> section1ControlTypes = new HashSet<string>(list1.Select<Control, string>((Func<Control, string>) (field => field.ControlType)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        List<Control> list2 = group1.Children.Where<Control>((Func<Control, bool>) (x => !section1ControlTypes.Contains(x.ControlType))).ToList<Control>();
        if (list1.Any<Control>())
        {
          foreach (Control control in list1)
          {
            string label = control.Label;
            if (string.IsNullOrWhiteSpace(label))
              label = group1.Label;
            Group group2 = this.CreateGroup(string.Format("{0}.{1}", (object) WebLayoutXmlHelper.GenerateUniqueId(string.Format("{0}.{1}", (object) page.Id, (object) label), existingIds), (object) WebLayoutXmlHelper.SealedGroupSuffix), label);
            group2.Children.Add(control);
            sectionArray[0].Children.Add(group2);
            groupList.Add(group2);
          }
        }
        if (list2.Any<Control>())
        {
          Group group3 = this.CreateGroup(string.Format("{0}.{1}", (object) page.Id, (object) group1.Label), group1.Label);
          HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          foreach (Control control in list2)
          {
            if (string.IsNullOrWhiteSpace(control.Id) || !stringSet.Contains(control.Id))
            {
              group3.Children.Add(control);
              stringSet.Add(control.Id);
            }
          }
          int index = numSections > 1 ? num1 % (numSections - 1) + 1 : 0;
          page.SectionFromIndex(index).Children.Add(group3);
          groupList.Add(group3);
          ++num1;
        }
      }
      if (sectionArray[0].Children.Count != 0)
        return;
      foreach (LayoutNodeContainer<Group> layoutNodeContainer in sectionArray)
        layoutNodeContainer.Children.Clear();
      int num2 = 0;
      foreach (Group group in groupList)
      {
        sectionArray[num2 % numSections].Children.Add(group);
        ++num2;
      }
    }

    private void InjectHistoryPage(Layout layout, string workItemTypeName, string processName)
    {
      string label = LegacyFormDeserializer.GetInjectedGroupLabel((IList<Group>) this.m_logGroups);
      if (layout.Children.Any<Page>((Func<Page, bool>) (page => string.Equals(page.Label, label, StringComparison.OrdinalIgnoreCase))) || string.IsNullOrEmpty(label))
        label = this.m_layoutHelper.GetSystemPageLabelFromLayouts(layout, WellKnownControlNames.WorkItemLogControl, WebLayoutXmlHelper.WorkItemFormHistoryLabel);
      Page page1 = this.CreatePage(string.Format("{0}.{1}.{2}", (object) processName, (object) workItemTypeName, (object) WebLayoutXmlHelper.WorkItemFormHistoryPageIdSuffix), label, true, PageType.History);
      layout.Children.Add(page1);
      Group group = this.CreateGroup(string.Format("{0}.{1}", (object) page1.Id, (object) page1.Label), page1.Label);
      Control control = this.CreateControl();
      control.Id = "System.History";
      control.ControlType = WellKnownControlNames.WorkItemLogControl;
      group.Children.Add(control);
      page1.Children[0].Children.Add(group);
    }

    private void InjectLinksPage(Layout layout, string workItemTypeName, string processName)
    {
      string labelFromLayouts = this.m_layoutHelper.GetSystemPageLabelFromLayouts(layout, WellKnownControlNames.LinksControl, WebLayoutXmlHelper.WorkItemFormLinksLabel);
      Page page = this.CreatePage(string.Format("{0}.{1}.{2}", (object) processName, (object) workItemTypeName, (object) WebLayoutXmlHelper.WorkItemFormLinksPageIdSuffix), labelFromLayouts, true, PageType.Links);
      layout.Children.Add(page);
      this.InjectLinksControl(page, this.m_linksGroups.SelectMany<KeyValuePair<string, List<Group>>, Group>((Func<KeyValuePair<string, List<Group>>, IEnumerable<Group>>) (kvp => kvp.Value.Select<Group, Group>((Func<Group, Group>) (group => this.TransformLinksGroup(group))))));
    }

    private void InjectAttachmentsPage(Layout layout, string workItemTypeName, string processName)
    {
      string labelFromLayouts = this.m_layoutHelper.GetSystemPageLabelFromLayouts(layout, WellKnownControlNames.AttachmentsControl, WebLayoutXmlHelper.WorkItemFormAttachmentsLabel);
      Page page = this.CreatePage(string.Format("{0}.{1}.{2}", (object) processName, (object) workItemTypeName, (object) WebLayoutXmlHelper.WorkItemFormAttachmentsPageIdSuffix), labelFromLayouts, true, PageType.Attachments);
      layout.Children.Add(page);
      this.InjectAttachmentsControl(page, (IList<Group>) this.m_attachmentGroups);
    }

    private void InjectAttachmentsControl(Page attachmentsPage, IList<Group> attachmentGroups)
    {
      Section section = attachmentsPage.SectionFromIndex(0);
      string label = attachmentsPage.Label ?? LegacyFormDeserializer.GetInjectedGroupLabel(attachmentGroups);
      Group group = this.CreateGroup(string.Format("{0}.{1}", (object) attachmentsPage.Id, (object) label), label);
      Control control = this.CreateControl();
      control.ControlType = WellKnownControlNames.AttachmentsControl;
      control.Label = group.Label;
      group.Children.Add(control);
      section.Children.Add(group);
    }

    private static string GetInjectedGroupLabel(IList<Group> groups)
    {
      string a = (string) null;
      if (groups.Count == 1 && groups[0].Children.Count == 1)
      {
        a = groups[0].Label;
        if (string.IsNullOrWhiteSpace(a) || string.Equals(a, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemFormOtherGroups()))
          a = groups[0].Children[0].Label;
      }
      return a;
    }

    private Group TransformLinksGroup(Group group)
    {
      Group group1 = group.Clone();
      Control control = group1.Children.First<Control>();
      control.Metadata = this.m_linksControlTransformer.ConvertLinksMetadata(control.Metadata);
      return group1;
    }

    private void InjectLinksControl(Page linksPage, IEnumerable<Group> linksGroups)
    {
      Section child1 = linksPage.Children[0];
      Group group = this.CreateGroup(string.Format("{0}.{1}", (object) linksPage.Id, (object) linksPage.Label), linksPage.Label);
      List<string> stringList = new List<string>();
      foreach (LayoutNodeContainer<Control> linksGroup in linksGroups)
      {
        Control child2 = linksGroup.Children[0];
        XmlDocument xmlDocument = new XmlDocument();
        try
        {
          XmlReaderSettings settings = new XmlReaderSettings()
          {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = (XmlResolver) null
          };
          using (StringReader input = new StringReader(child2.Metadata))
          {
            using (XmlReader reader = XmlReader.Create((TextReader) input, settings))
              xmlDocument.Load(reader);
          }
        }
        catch (XmlException ex)
        {
          if (this.m_tracer != null)
          {
            this.m_tracer.TraceException(909803, TraceLevel.Warning, "FormLayout", "FormTransformsLayer", (Exception) ex);
            continue;
          }
          continue;
        }
        foreach (XmlElement selectNode in xmlDocument.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.WebLayoutLinkColumn)))
        {
          string attribute = selectNode.GetAttribute(ProvisionAttributes.WebLayoutColumnName);
          if (!string.IsNullOrWhiteSpace(attribute) && !stringList.Contains<string>(attribute, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) && !string.Equals(attribute, ProvisionValues.ColumnLinkComment, StringComparison.OrdinalIgnoreCase))
            stringList.Add(attribute);
        }
      }
      Control control = this.CreateControl();
      control.ControlType = WellKnownControlNames.LinksControl;
      control.Metadata = LegacyFormDeserializer.BuildLinkOptions(stringList);
      group.Children.Add(control);
      child1.Children.Add(group);
    }

    private Group CreateGroup(string id, string label)
    {
      Group group = this.CreateGroup(label);
      group.Id = LayoutHelper.NormalizeLayoutElementId(id);
      return group;
    }

    private Group CreateGroup(string label)
    {
      Group group = new Group();
      group.Label = label;
      group.Visible = new bool?(true);
      group.FromInheritedLayout = false;
      return group;
    }

    private Page CreatePage(string id, string label, bool locked, PageType type)
    {
      Page page = new Page(true);
      page.Id = this.GetUniquePageId(id);
      page.Label = label;
      page.Visible = new bool?(true);
      page.FromInheritedLayout = false;
      page.Locked = new bool?(locked);
      page.PageType = type;
      return page;
    }

    private Control CreateControl()
    {
      Control control = new Control();
      control.Visible = new bool?(true);
      control.FromInheritedLayout = false;
      return control;
    }

    private static string BuildLinkOptions(List<string> linkColumns)
    {
      if (linkColumns.Count == 0)
        linkColumns = ((IEnumerable<string>) LegacyFormDeserializer.DEFAULT_LINK_COLUMNS).ToList<string>();
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<{0}><{1}>{2}</{1}></{0}>", (object) ProvisionTags.LinksControlOptions, (object) ProvisionTags.WebLayoutLinkColumns, (object) LegacyFormDeserializer.BuildLinkColumns(linkColumns));
    }

    private static string BuildLinkColumns(List<string> linkColumns)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string linkColumn in linkColumns)
        stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<{0} {1}=\"{2}\" />", (object) ProvisionTags.WebLayoutLinkColumn, (object) ProvisionAttributes.WebLayoutColumnName, (object) linkColumn));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<{0} {1}=\"{2}\" />", (object) ProvisionTags.WebLayoutLinkColumn, (object) ProvisionAttributes.WebLayoutColumnName, (object) ProvisionValues.ColumnLinkComment));
      return stringBuilder.ToString();
    }

    private string GetUniquePageId(string id)
    {
      id = LayoutHelper.NormalizeLayoutElementId(id);
      return WebLayoutXmlHelper.GenerateUniqueId(id, this.m_pageIdSet);
    }
  }
}
