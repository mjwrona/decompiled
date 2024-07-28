// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.FormTransformer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormExtensions;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  public class FormTransformer
  {
    private ITraceRequest m_tracer;

    public FormTransformer(ITraceRequest tracer = null) => this.m_tracer = tracer;

    public Layout ConvertToNewFormLayout(
      string formXml,
      string processName,
      string workItemTypeName,
      Lazy<IEnumerable<Contribution>> formContributions,
      Layout oobLayout,
      ControlLabelResolver labelResolver = null,
      bool injectSystemPages = true)
    {
      return this.ConvertFromWebLayoutXml(formXml, processName, workItemTypeName, formContributions, oobLayout, labelResolver, injectSystemPages);
    }

    public void TransformForExport(
      XmlElement formElement,
      Lazy<IEnumerable<Contribution>> formContributions,
      Layout oobLayout,
      ControlLabelResolver labelResolver = null,
      bool injectSystemPages = true)
    {
      string outerXml = formElement.OuterXml;
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      Lazy<IEnumerable<Contribution>> formContributions1 = formContributions;
      ControlLabelResolver controlLabelResolver = labelResolver;
      bool flag = injectSystemPages;
      Layout oobLayout1 = oobLayout;
      ControlLabelResolver labelResolver1 = controlLabelResolver;
      int num = flag ? 1 : 0;
      Layout o = this.ConvertFromWebLayoutXml(outerXml, empty1, empty2, formContributions1, oobLayout1, labelResolver1, num != 0);
      if (formElement.SelectSingleNode("WebLayout") is XmlElement oldChild)
        formElement.RemoveChild((XmlNode) oldChild);
      using (StringWriter output = new StringWriter())
      {
        using (XmlWriter xmlWriter = XmlWriter.Create((TextWriter) output))
        {
          new XmlSerializer(typeof (Layout)).Serialize(xmlWriter, (object) o);
          formElement.CreateNavigator().AppendChild(output.ToString());
        }
      }
      XmlElement webLayoutElement = formElement.SelectSingleNode("WebLayout") as XmlElement;
      WebLayoutXmlHelper.RemovePageAndGroupIds(webLayoutElement);
      string extensionInjection = FormExtensionXmlCommentBuilder.GetCommentBlobForExtensionInjection(formContributions.Value);
      if (string.IsNullOrEmpty(extensionInjection))
        return;
      XmlComment comment = webLayoutElement.OwnerDocument.CreateComment(extensionInjection);
      webLayoutElement.PrependChild((XmlNode) comment);
    }

    public string ConvertToNewFormXml(Layout layout) => new NewFormXmlSerializer(this.m_tracer).Serialize(layout);

    private Layout ConvertFromLegacyLayout(
      string formXml,
      string processName,
      string workItemTypeName,
      ControlLabelResolver labelResolver,
      bool injectSystemPages,
      Layout oobLayout)
    {
      return new LegacyFormDeserializer(this.m_tracer, labelResolver, oobLayout).Deserialize(formXml, workItemTypeName, processName, injectSystemPages);
    }

    private Layout ConvertFromWebLayoutXml(
      string formXml,
      string processName,
      string workItemTypeName,
      Lazy<IEnumerable<Contribution>> formContributions,
      Layout oobLayout,
      ControlLabelResolver labelResolver = null,
      bool injectSystemPages = true)
    {
      if (this.m_tracer != null)
        this.m_tracer.TraceEnter(909806, "FormLayout", "FormTransformsLayer", nameof (ConvertFromWebLayoutXml));
      try
      {
        Layout layout = new WebLayoutParser(this.m_tracer, formContributions, oobLayout).Parse(formXml, processName, workItemTypeName, injectSystemPages);
        this.AutoInjectExtensionContributions(layout, formContributions, processName, workItemTypeName);
        if (!layout.Children.Any<Page>())
          layout = this.ConvertFromLegacyLayout(formXml, processName, workItemTypeName, labelResolver, injectSystemPages, oobLayout);
        if (oobLayout != null)
          this.MergeSystemControls(layout, oobLayout.SystemControls.ToList<Control>());
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
          this.m_tracer.TraceLeave(909807, "FormLayout", "FormTransformsLayer", nameof (ConvertFromWebLayoutXml));
      }
    }

    private void MergeSystemControls(Layout layout, List<Control> systemControlsFromOobLayout)
    {
      if (layout == null || systemControlsFromOobLayout == null)
        return;
      bool flag = layout.SystemControls.Any<Control>((Func<Control, bool>) (c => !string.IsNullOrEmpty(c.ReplacesFieldReferenceName)));
      Dictionary<string, Control> dictionary = layout.SystemControls.ToDictionary<Control, string, Control>((Func<Control, string>) (c => c.Id), (Func<Control, Control>) (c => c), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, Control> dedupedDictionary = layout.SystemControls.ToDedupedDictionary<Control, string, Control>((Func<Control, string>) (c => c.ControlType), (Func<Control, Control>) (c => c), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (Control control in systemControlsFromOobLayout)
      {
        if (WebLayoutXmlHelper.HeaderControls.Contains(control.ControlType))
        {
          if (!dedupedDictionary.ContainsKey(control.ControlType))
            layout.SystemControls.Add(control);
        }
        else if (!dictionary.ContainsKey(control.Id) && (!flag || !WebLayoutXmlHelper.ReplaceableOrHidableSystemControlFields.Contains(control.Id)))
          layout.SystemControls.Add(control);
      }
    }

    private void AutoInjectExtensionContributions(
      Layout layout,
      Lazy<IEnumerable<Contribution>> formContributions,
      string processName,
      string workItemTypeName)
    {
      if (!layout.Children.Any<Page>() || !layout.Extensions.Any<Extension>())
        return;
      IEnumerable<Contribution> contributions = Enumerable.Empty<Contribution>();
      IEnumerable<string> strings = layout.Extensions.Select<Extension, string>((Func<Extension, string>) (ext => ext.Id));
      if (strings.Any<string>())
        contributions = formContributions.Value;
      Page page1 = layout.Children.FirstOrDefault<Page>((Func<Page, bool>) (page => page.PageType == PageType.Custom && !page.IsContribution));
      if (page1 != null)
      {
        IEnumerable<Contribution> extensionsAndType = FormExtensionsUtility.GetFilteredContributionsByExtensionsAndType(contributions, strings, "ms.vss-work-web.work-item-form-group");
        HashSet<string> existingGroupContributionIds = new HashSet<string>(layout.GetDescendants<Group>().Cast<Group>().Where<Group>((Func<Group, bool>) (g => g.IsContribution)).Select<Group, string>((Func<Group, string>) (g => g.Contribution.ContributionId)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        Func<Contribution, bool> predicate = (Func<Contribution, bool>) (c => !existingGroupContributionIds.Contains(c.Id));
        foreach (Contribution contribution in extensionsAndType.Where<Contribution>(predicate))
        {
          string str = page1.Id + "." + contribution.Id;
          Group group1 = new Group();
          group1.Id = str;
          group1.Label = FormExtensionsUtility.GetContributionName(contribution);
          group1.Visible = new bool?(true);
          group1.FromInheritedLayout = false;
          Group group2 = group1;
          group2.Contribution = new WitContribution(contribution.Id, FormExtensionsUtility.GetShowOnDeletedWorkItem(contribution), height: FormExtensionsUtility.GetContributionHeight(contribution, 150));
          page1.Children[1].Children.Add(group2);
        }
      }
      IEnumerable<Contribution> extensionsAndType1 = FormExtensionsUtility.GetFilteredContributionsByExtensionsAndType(contributions, strings, "ms.vss-work-web.work-item-form-page");
      HashSet<string> existingPageContributionIds = new HashSet<string>(layout.GetDescendants<Page>().Cast<Page>().Where<Page>((Func<Page, bool>) (p => p.IsContribution)).Select<Page, string>((Func<Page, string>) (p => p.Contribution.ContributionId)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Func<Contribution, bool> predicate1 = (Func<Contribution, bool>) (c => !existingPageContributionIds.Contains(c.Id));
      foreach (Contribution contribution in extensionsAndType1.Where<Contribution>(predicate1))
      {
        string str = processName + "." + workItemTypeName + "." + contribution.Id;
        Page page2 = new Page(false);
        page2.Id = str;
        page2.Label = FormExtensionsUtility.GetContributionName(contribution);
        page2.Visible = new bool?(true);
        page2.FromInheritedLayout = false;
        page2.Locked = new bool?(false);
        page2.PageType = PageType.Custom;
        Page page3 = page2;
        page3.Contribution = new WitContribution(contribution.Id, FormExtensionsUtility.GetShowOnDeletedWorkItem(contribution));
        Page page4 = layout.Children.FirstOrDefault<Page>((Func<Page, bool>) (page => page.PageType != PageType.Custom));
        if (page4 == null)
        {
          layout.Children.Add(page3);
        }
        else
        {
          int index = layout.Children.IndexOf(page4);
          layout.Children.Insert(index, page3);
        }
      }
    }
  }
}
