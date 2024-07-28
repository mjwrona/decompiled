// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.WebLayoutParser
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormExtensions;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.FormInput;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  internal class WebLayoutParser
  {
    private ITraceRequest m_tracer;
    private Dictionary<string, Contribution> m_pageContributionsLookup = new Dictionary<string, Contribution>();
    private Dictionary<string, Contribution> m_groupContributionsLookup = new Dictionary<string, Contribution>();
    private Dictionary<string, Contribution> m_controlContributionsLookup = new Dictionary<string, Contribution>();
    private IDictionary<string, int> controlContributionIdMap = (IDictionary<string, int>) new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Lazy<IEnumerable<Contribution>> m_formContributions;
    private LayoutHelper m_layoutHelper;

    internal WebLayoutParser(
      ITraceRequest tracer,
      Lazy<IEnumerable<Contribution>> formContributions,
      Layout oobLayout = null)
    {
      this.m_tracer = tracer;
      this.m_formContributions = formContributions;
      this.m_layoutHelper = new LayoutHelper(oobLayout);
    }

    internal Layout Parse(
      string formXml,
      string processName,
      string workItemTypeName,
      bool injectSystemPages)
    {
      ArgumentUtility.CheckForNull<string>(formXml, nameof (formXml));
      if (this.m_tracer != null)
        this.m_tracer.TraceEnter(909809, "FormLayout", "FormTransformsLayer", nameof (Parse));
      try
      {
        Layout formLayout = this.ParseFormLayout(this.LoadXmlDocument(formXml), processName, workItemTypeName, injectSystemPages);
        if (formLayout.Children.Any<Page>())
          this.m_layoutHelper.FillRanksInLayout(formLayout);
        return formLayout;
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
          this.m_tracer.TraceLeave(909810, "FormLayout", "FormTransformsLayer", nameof (Parse));
      }
    }

    private Layout ParseFormLayout(
      XmlDocument layoutDocument,
      string processName,
      string workItemTypeName,
      bool injectSystemPages)
    {
      Layout layout = new Layout();
      if (!(layoutDocument.SelectSingleNode(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) ProvisionTags.Form, (object) "WebLayout")) is XmlElement webLayoutElement))
      {
        if (this.m_tracer != null)
          this.m_tracer.Trace(909808, TraceLevel.Info, "FormLayout", "FormTransformsLayer", (string[]) null, "Could not find a WebLayout element in the form (would happen if OnPrem or Phase0/1 customer enable new form but not specify that in template)");
        return layout;
      }
      string attribute = webLayoutElement.GetAttribute(ProvisionAttributes.LayoutShowEmptyReadOnlyFields);
      if (!string.IsNullOrEmpty(attribute))
        layout.ShowEmptyReadOnlyFields = new bool?(XmlConvert.ToBoolean(attribute));
      WebLayoutXmlHelper.GeneratePageAndGroupIds(processName, workItemTypeName, this.m_layoutHelper.GetOobFirstPageLabel(), webLayoutElement, (Action<string>) null, (Action<string>) null, (Action<string>) null, (Action<string, string>) null);
      XmlNode xmlNode = webLayoutElement.SelectSingleNode(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.SystemControls));
      if (xmlNode != null)
      {
        foreach (XmlElement controlElement1 in xmlNode.ChildNodes.OfType<XmlElement>())
        {
          Control controlElement2 = this.ParseControlElement(controlElement1, true);
          layout.SystemControls.Add(controlElement2);
        }
      }
      XmlNodeList xmlNodeList1 = webLayoutElement.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.Extension));
      if (xmlNodeList1 != null && xmlNodeList1.Count > 0)
      {
        if (this.m_formContributions.Value != null && this.m_formContributions.Value.Any<Contribution>())
        {
          this.m_pageContributionsLookup = FormExtensionsUtility.GetFilteredContributionsByType(this.m_formContributions.Value, "ms.vss-work-web.work-item-form-page").ToDictionary<Contribution, string>((Func<Contribution, string>) (c => c.Id), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          this.m_groupContributionsLookup = FormExtensionsUtility.GetFilteredContributionsByType(this.m_formContributions.Value, "ms.vss-work-web.work-item-form-group").ToDictionary<Contribution, string>((Func<Contribution, string>) (c => c.Id), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          this.m_controlContributionsLookup = FormExtensionsUtility.GetFilteredContributionsByType(this.m_formContributions.Value, "ms.vss-work-web.work-item-form-control").ToDictionary<Contribution, string>((Func<Contribution, string>) (c => c.Id), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        }
        foreach (XmlElement xmlElement in xmlNodeList1)
        {
          string extensionId = xmlElement.GetAttribute(ProvisionAttributes.ExtensionId);
          if (!layout.Extensions.Any<Extension>((Func<Extension, bool>) (ex => ex.Id.Equals(extensionId, StringComparison.OrdinalIgnoreCase))))
            layout.Extensions.Add(new Extension()
            {
              Id = extensionId
            });
        }
      }
      XmlNodeList xmlNodeList2 = webLayoutElement.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "./{0} | ./{1}", (object) ProvisionTags.Page, (object) ProvisionTags.PageContribution));
      if (xmlNodeList2 != null && xmlNodeList2.Count > 0)
      {
        foreach (XmlElement xmlElement in xmlNodeList2)
        {
          if (xmlElement.LocalName == ProvisionTags.PageContribution)
            this.ParsePageContribution(layout, xmlElement, processName, workItemTypeName);
          else
            this.ParsePage(layout, xmlElement);
        }
      }
      if (injectSystemPages)
        this.m_layoutHelper.InjectSystemPages(layout, processName, workItemTypeName);
      return layout;
    }

    private void ParsePage(Layout layout, XmlElement pageElement)
    {
      string attribute1 = pageElement.GetAttribute(ProvisionAttributes.Label);
      string attribute2 = pageElement.GetAttribute(ProvisionAttributes.WebLayoutElementId);
      string attribute3 = pageElement.GetAttribute(ProvisionAttributes.PageLayoutMode);
      string label = attribute1;
      Page page = LayoutHelper.CreatePage(attribute2, label, false, PageType.Custom);
      PageLayoutMode result;
      if (Enum.TryParse<PageLayoutMode>(attribute3, out result))
        page.LayoutMode = result;
      XmlNodeList xmlNodeList = pageElement.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "./{0}", (object) ProvisionTags.Section));
      if (xmlNodeList != null && xmlNodeList.Count > 0 && xmlNodeList.Count <= WebLayoutXmlHelper.MaxNumberOfSections)
      {
        for (int index = 0; index < xmlNodeList.Count; ++index)
          this.ParseSection(layout, page, xmlNodeList[index] as XmlElement, index);
      }
      layout.Children.Add(page);
    }

    private void ParsePageContribution(
      Layout layout,
      XmlElement pageContributionElement,
      string processName,
      string workItemTypeName)
    {
      string attribute1 = pageContributionElement.GetAttribute(ProvisionAttributes.Label);
      string attribute2 = pageContributionElement.GetAttribute(ProvisionAttributes.ContributionId);
      string attribute3 = pageContributionElement.GetAttribute(ProvisionAttributes.Visible);
      Page page = LayoutHelper.CreatePage(processName + "." + workItemTypeName + "." + attribute2, attribute1, false, PageType.Custom);
      page.Visible = new bool?(string.IsNullOrEmpty(attribute3) || XmlConvert.ToBoolean(attribute3));
      page.Contribution = new WitContribution(attribute2);
      Contribution contribution;
      if (this.m_pageContributionsLookup.TryGetValue(attribute2 ?? string.Empty, out contribution))
        page.Contribution.ShowOnDeletedWorkItem = FormExtensionsUtility.GetShowOnDeletedWorkItem(contribution);
      layout.Children.Add(page);
    }

    private void ParseSection(
      Layout layout,
      Page page,
      XmlElement sectionElement,
      int sectionIndex)
    {
      Section child = page.Children[sectionIndex];
      XmlNodeList xmlNodeList = sectionElement.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "./{0} | ./{1}", (object) ProvisionTags.Group, (object) ProvisionTags.GroupContribution));
      if (xmlNodeList == null || xmlNodeList.Count <= 0)
        return;
      foreach (XmlElement xmlElement in xmlNodeList)
      {
        if (xmlElement.LocalName == ProvisionTags.GroupContribution)
          this.ParseGroupContribution(layout, page, child, xmlElement);
        else
          this.ParseGroup(layout, child, xmlElement);
      }
    }

    private void ParseGroup(Layout layout, Section section, XmlElement groupElement)
    {
      string attribute = groupElement.GetAttribute(ProvisionAttributes.Label);
      Group group = LayoutHelper.CreateGroup(groupElement.GetAttribute(ProvisionAttributes.WebLayoutElementId), attribute);
      XmlNodeList xmlNodeList = groupElement.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "./{0} | ./{1}", (object) ProvisionTags.Control, (object) ProvisionTags.ControlContribution));
      if (xmlNodeList != null && xmlNodeList.Count > 0)
      {
        foreach (XmlElement xmlElement in xmlNodeList)
        {
          if (xmlElement.LocalName == ProvisionTags.ControlContribution)
            this.ParseControlContribution(layout, group, xmlElement);
          else
            this.ParseControl(layout, group, xmlElement);
        }
      }
      section.Children.Add(group);
    }

    private void ParseGroupContribution(
      Layout layout,
      Page page,
      Section section,
      XmlElement groupContributionElement)
    {
      string attribute1 = groupContributionElement.GetAttribute(ProvisionAttributes.Label);
      string attribute2 = groupContributionElement.GetAttribute(ProvisionAttributes.ContributionId);
      string attribute3 = groupContributionElement.GetAttribute(ProvisionAttributes.Visible);
      string attribute4 = groupContributionElement.GetAttribute(ProvisionAttributes.Height);
      Group group = LayoutHelper.CreateGroup(page.Id + "." + attribute2, attribute1);
      group.Visible = new bool?(string.IsNullOrEmpty(attribute3) || XmlConvert.ToBoolean(attribute3));
      group.Contribution = new WitContribution(attribute2);
      Contribution contribution;
      if (this.m_groupContributionsLookup.TryGetValue(attribute2 ?? string.Empty, out contribution))
      {
        group.Contribution.ShowOnDeletedWorkItem = FormExtensionsUtility.GetShowOnDeletedWorkItem(contribution);
        int result;
        if (!string.IsNullOrEmpty(attribute4) && int.TryParse(attribute4, out result) && result > 0)
          group.Height = new int?(result);
        else
          group.Contribution.Height = FormExtensionsUtility.GetContributionHeight(contribution, 150);
      }
      section.Children.Add(group);
    }

    private void ParseControl(Layout layout, Group group, XmlElement controlElement)
    {
      Control controlElement1 = this.ParseControlElement(controlElement);
      controlElement1.Metadata = controlElement.InnerXml;
      group.Children.Add(controlElement1);
    }

    private Control ParseControlElement(XmlElement controlElement, bool allowVisibleAttribute = false)
    {
      Control control = LayoutHelper.CreateControl();
      control.ControlType = controlElement.GetAttribute(ProvisionAttributes.ControlType);
      control.Name = controlElement.GetAttribute(ProvisionAttributes.ControlName);
      control.ReplacesFieldReferenceName = controlElement.GetAttribute(ProvisionAttributes.ControlReplacesFieldReferenceName);
      if (allowVisibleAttribute)
      {
        string attribute = controlElement.GetAttribute(ProvisionAttributes.Visible);
        control.Visible = new bool?(string.IsNullOrEmpty(attribute) || XmlConvert.ToBoolean(attribute));
      }
      string attribute1 = controlElement.GetAttribute(ProvisionAttributes.ControlHeight);
      int result1;
      if (!string.IsNullOrEmpty(attribute1) && int.TryParse(attribute1, out result1) && result1 > 0)
        control.Height = new int?(result1);
      string attribute2 = controlElement.GetAttribute(ProvisionAttributes.ControlFieldName);
      if (!string.IsNullOrEmpty(control.ControlType) && (string.Equals(control.ControlType, WellKnownControlNames.AssociatedAutomationControl, StringComparison.OrdinalIgnoreCase) || string.Equals(control.ControlType, WellKnownControlNames.LinksControl, StringComparison.OrdinalIgnoreCase) || string.Equals(control.ControlType, WellKnownControlNames.AttachmentsControl, StringComparison.OrdinalIgnoreCase) || string.Equals(control.ControlType, WellKnownControlNames.DeploymentsControl, StringComparison.OrdinalIgnoreCase)))
        control.Id = control.Name;
      else
        control.Id = attribute2;
      control.Label = controlElement.GetAttribute(ProvisionAttributes.Label);
      control.Watermark = controlElement.GetAttribute(ProvisionAttributes.ControlEmptyText);
      bool result2 = false;
      bool.TryParse(controlElement.GetAttribute(ProvisionAttributes.ControlReadOnly), out result2);
      control.ReadOnly = new bool?(result2);
      return control;
    }

    private void ParseControlContribution(
      Layout layout,
      Group group,
      XmlElement controlContributionElement)
    {
      string attribute1 = controlContributionElement.GetAttribute("Label");
      string attribute2 = controlContributionElement.GetAttribute(ProvisionAttributes.ContributionId);
      string attribute3 = controlContributionElement.GetAttribute(ProvisionAttributes.Visible);
      string attribute4 = controlContributionElement.GetAttribute(ProvisionAttributes.Height);
      Control control = LayoutHelper.CreateControl();
      int num1;
      if (this.controlContributionIdMap.TryGetValue(attribute2, out num1))
      {
        int num2;
        this.controlContributionIdMap[attribute2] = num2 = num1 + 1;
        control.Id = string.Format("{0}.{1}_{2}", (object) group.Id, (object) attribute2, (object) num2);
      }
      else
      {
        control.Id = string.Format("{0}.{1}", (object) group.Id, (object) attribute2);
        this.controlContributionIdMap[attribute2] = 1;
      }
      if (!string.IsNullOrEmpty(attribute1))
        control.Label = attribute1;
      control.Visible = new bool?(string.IsNullOrEmpty(attribute3) || XmlConvert.ToBoolean(attribute3));
      Contribution contribution;
      if (this.m_controlContributionsLookup.TryGetValue(attribute2 ?? string.Empty, out contribution))
      {
        JToken property = contribution.Properties["inputs"];
        Dictionary<string, string> contributionInputsDataTypeMap = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        if (property != null)
        {
          foreach (JToken jtoken1 in (IEnumerable<JToken>) property)
          {
            string key = jtoken1.Value<string>((object) "id");
            JToken jtoken2 = jtoken1.SelectToken("validation");
            if (!string.IsNullOrWhiteSpace(key) && !contributionInputsDataTypeMap.ContainsKey(key))
            {
              string str = "String";
              if (jtoken2 != null)
                str = jtoken2.SelectToken("dataType") != null ? jtoken2.Value<string>((object) "dataType") : str;
              contributionInputsDataTypeMap[key] = str;
            }
          }
        }
        control.Contribution = new WitContribution(attribute2, inputs: this.GetContributionInputs(controlContributionElement, contributionInputsDataTypeMap));
        control.Metadata = contribution.Description;
        control.Contribution.ShowOnDeletedWorkItem = FormExtensionsUtility.GetShowOnDeletedWorkItem(contribution);
        int result;
        if (!string.IsNullOrEmpty(attribute4) && int.TryParse(attribute4, out result) && result > 0)
          control.Height = new int?(result);
        else
          control.Contribution.Height = FormExtensionsUtility.GetContributionHeight(contribution, 75);
      }
      else
        control.Contribution = new WitContribution(attribute2);
      group.Children.Add(control);
    }

    private XmlDocument LoadXmlDocument(string xml)
    {
      XmlDocument xmlDocument = new XmlDocument();
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null,
        IgnoreComments = true
      };
      using (StringReader input = new StringReader(xml))
      {
        using (XmlReader reader = XmlReader.Create((TextReader) input, settings))
          xmlDocument.Load(reader);
      }
      return xmlDocument;
    }

    private IDictionary<string, object> GetContributionInputs(
      XmlElement controlContributionElement,
      Dictionary<string, string> contributionInputsDataTypeMap)
    {
      IDictionary<string, object> contributionInputs = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (XmlElement selectNode in controlContributionElement.SelectNodes(".//Inputs/Input"))
      {
        string attribute1 = selectNode.GetAttribute("Id");
        string attribute2 = selectNode.GetAttribute("Value");
        if (!string.IsNullOrWhiteSpace(attribute1))
        {
          string str;
          InputDataType result1;
          if (contributionInputsDataTypeMap.TryGetValue(attribute1, out str) && Enum.TryParse<InputDataType>(str, out result1))
          {
            switch (result1)
            {
              case InputDataType.Number:
                double result2;
                if (double.TryParse(attribute2, NumberStyles.Number, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result2))
                {
                  contributionInputs[attribute1] = (object) result2;
                  break;
                }
                break;
              case InputDataType.Boolean:
                bool result3;
                if (bool.TryParse(attribute2, out result3))
                {
                  contributionInputs[attribute1] = (object) result3;
                  break;
                }
                break;
              default:
                contributionInputs[attribute1] = (object) attribute2;
                break;
            }
          }
          if (!contributionInputs.TryGetValue(attribute1, out object _))
            contributionInputs[attribute1] = (object) attribute2;
        }
      }
      return contributionInputs;
    }
  }
}
