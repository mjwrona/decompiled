// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.LayoutHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  internal class LayoutHelper
  {
    private readonly IEnumerable<string> m_defaultLinkColumns = (IEnumerable<string>) new string[2]
    {
      "System.State",
      "System.ChangedDate"
    };
    private Layout m_oobLayout;

    public LayoutHelper(Layout oobLayout) => this.m_oobLayout = oobLayout;

    public Layout CreateDefaultSystemWorkItemTypeLayout(
      string processName,
      string workItemTypeName,
      CultureInfo serverCulture)
    {
      Layout layout = new Layout();
      this.InjectDefaultCustomWorkItemTypeLayout(layout, processName, workItemTypeName, serverCulture);
      this.InjectSystemControls(layout);
      this.InjectSystemPages(layout, processName, workItemTypeName);
      return layout;
    }

    private void InjectSystemControls(Layout layout)
    {
      if (this.m_oobLayout == null)
        return;
      layout.SystemControls.UnionWith((IEnumerable<Control>) this.m_oobLayout.SystemControls);
    }

    private void InjectDefaultCustomWorkItemTypeLayout(
      Layout layout,
      string processName,
      string workItemTypeName,
      CultureInfo serverCulture)
    {
      Section sectionFromLayout = LayoutHelper.GetFirstPageThirdSectionFromLayout(this.m_oobLayout);
      Page page = new Page(true);
      page.Label = InternalsResourceStrings.Get("NewFormFirstPageTitle", serverCulture);
      page.Id = LayoutHelper.NormalizeLayoutElementId(processName + "." + workItemTypeName + "." + page.Label);
      page.Visible = new bool?(true);
      Section child1 = page.Children[0];
      Group group = LayoutHelper.CreateGroup(page.Id, ServerResources.Description(serverCulture), true);
      Control control = LayoutHelper.CreateControl();
      control.Id = "System.Description";
      control.ControlType = WellKnownControlNames.HtmlControl;
      group.Children.Add(control);
      child1.Children.Add(group);
      if (sectionFromLayout != null)
      {
        Section child2 = page.Children[2];
        Group linksControlGroup = WellKnownProcessLayout.GetDeploymentLinksControlGroup(sectionFromLayout);
        if (linksControlGroup != null)
          child2.Children.Add(linksControlGroup);
        child2.Children.Add(WellKnownProcessLayout.GetDevelopmentLinksControlGroup(sectionFromLayout));
        child2.Children.Add(WellKnownProcessLayout.GetRelatedWorkItemLinksControlGroup(sectionFromLayout));
        layout.Children.Insert(0, page);
      }
      this.FillRanksInLayout(layout);
    }

    public void AutoInjectSystemWorkItemControls(Layout layout)
    {
      Page page = layout != null ? layout.Children.Where<Page>((Func<Page, bool>) (p => p.PageType == PageType.Custom && !p.IsContribution)).FirstOrDefault<Page>() : (Page) null;
      Section sectionFromLayout = LayoutHelper.GetFirstPageThirdSectionFromLayout(this.m_oobLayout);
      if (page == null || sectionFromLayout == null)
        return;
      IList<Section> children = page.Children;
      if ((children != null ? (children.Count > 2 ? 1 : 0) : 0) == 0)
        return;
      Section child = page.Children[2];
      int index = 0;
      Group linksControlGroup1 = WellKnownProcessLayout.GetDeploymentLinksControlGroup(sectionFromLayout);
      if (linksControlGroup1 != null)
      {
        child.Children.Insert(index, linksControlGroup1);
        ++index;
      }
      Group linksControlGroup2 = WellKnownProcessLayout.GetDevelopmentLinksControlGroup(sectionFromLayout);
      if (linksControlGroup2 != null)
      {
        child.Children.Insert(index, linksControlGroup2);
        ++index;
      }
      Group linksControlGroup3 = WellKnownProcessLayout.GetRelatedWorkItemLinksControlGroup(sectionFromLayout);
      if (linksControlGroup3 == null)
        return;
      child.Children.Insert(index, linksControlGroup3);
    }

    public void InjectSystemPages(Layout layout, string processName, string workItemTypeName)
    {
      this.InjectSystemPage(layout, processName, workItemTypeName, PageType.History, WebLayoutXmlHelper.WorkItemFormHistoryPageIdSuffix, WellKnownControlNames.WorkItemLogControl, (Func<Group, Control>) (group => LayoutHelper.CreateControlWithId("System.History")), WebLayoutXmlHelper.WorkItemFormHistoryLabel);
      this.InjectSystemPage(layout, processName, workItemTypeName, PageType.Links, WebLayoutXmlHelper.WorkItemFormLinksPageIdSuffix, WellKnownControlNames.LinksControl, (Func<Group, Control>) (group => LayoutHelper.CreateControlWithMetadata(this.BuildLinkOptions())), WebLayoutXmlHelper.WorkItemFormLinksLabel);
      this.InjectSystemPage(layout, processName, workItemTypeName, PageType.Attachments, WebLayoutXmlHelper.WorkItemFormAttachmentsPageIdSuffix, WellKnownControlNames.AttachmentsControl, (Func<Group, Control>) (group => LayoutHelper.CreateControlWithLabel(group.Label)), WebLayoutXmlHelper.WorkItemFormAttachmentsLabel);
    }

    private void InjectSystemPage(
      Layout layout,
      string processName,
      string workItemTypeName,
      PageType pageType,
      string pageIdSuffix,
      string controlType,
      Func<Group, Control> getControl,
      string labelInUICulture)
    {
      string labelFromLayouts = this.GetSystemPageLabelFromLayouts(layout, controlType, labelInUICulture);
      Page page = LayoutHelper.CreatePage(string.Format("{0}.{1}.{2}", (object) processName, (object) workItemTypeName, (object) pageIdSuffix), labelFromLayouts, true, pageType);
      page.Rank = new int?(int.MaxValue);
      Group group = LayoutHelper.CreateGroup(page.Id, page.Label, false);
      Control control = getControl(group);
      if (control != null)
      {
        control.ControlType = controlType;
        group.Children.Add(control);
      }
      page.Children[0].Children.Add(group);
      layout.Children.Add(page);
    }

    public string GetOobFirstPageLabel()
    {
      Layout oobLayout = this.m_oobLayout;
      if (oobLayout == null)
        return (string) null;
      return oobLayout.Children.Where<Page>((Func<Page, bool>) (p => p.PageType == PageType.Custom && !p.IsContribution)).FirstOrDefault<Page>()?.Label;
    }

    public string GetSystemPageLabelFromLayouts(
      Layout layout,
      string controlType,
      string labelInUICulture)
    {
      string str = layout.SystemControls.Where<Control>((Func<Control, bool>) (c => c.ControlType.Equals(controlType, StringComparison.OrdinalIgnoreCase))).Select<Control, string>((Func<Control, string>) (l => l.Label)).FirstOrDefault<string>();
      string labelFromLayouts = (string) null;
      if (!string.IsNullOrEmpty(str))
        labelFromLayouts = str;
      else if (this.m_oobLayout != null)
        labelFromLayouts = this.m_oobLayout.SystemControls.Where<Control>((Func<Control, bool>) (c => c.ControlType.Equals(controlType, StringComparison.OrdinalIgnoreCase))).Select<Control, string>((Func<Control, string>) (l => l.Label)).FirstOrDefault<string>();
      if (string.IsNullOrEmpty(labelFromLayouts))
        labelFromLayouts = labelInUICulture;
      return labelFromLayouts;
    }

    private static Section GetFirstPageThirdSectionFromLayout(Layout layout) => (layout != null ? layout.Children.Where<Page>((Func<Page, bool>) (p => p.PageType == PageType.Custom && !p.IsContribution)).FirstOrDefault<Page>() : (Page) null)?.FindDescendant<Section>(string.Format("Section{0}", (object) 3));

    public void FillRanksInLayout(Layout layout)
    {
      int num1 = 0;
      foreach (Page child1 in (IEnumerable<Page>) layout.Children)
      {
        child1.Rank = new int?(num1++);
        int num2 = 0;
        foreach (Section child2 in (IEnumerable<Section>) child1.Children)
        {
          child2.Rank = new int?(num2++);
          int num3 = 0;
          foreach (Group child3 in (IEnumerable<Group>) child2.Children)
          {
            child3.Rank = new int?(num3++);
            int num4 = 0;
            foreach (LayoutNode child4 in (IEnumerable<Control>) child3.Children)
              child4.Rank = new int?(num4++);
          }
        }
      }
    }

    public static Control CreateControl()
    {
      Control control = new Control();
      control.Visible = new bool?(true);
      control.FromInheritedLayout = false;
      return control;
    }

    private static Control CreateControlWithId(string id)
    {
      Control control = LayoutHelper.CreateControl();
      control.Id = id;
      return control;
    }

    private static Control CreateControlWithMetadata(string metadata)
    {
      Control control = LayoutHelper.CreateControl();
      control.Metadata = metadata;
      return control;
    }

    private static Control CreateControlWithLabel(string label)
    {
      Control control = LayoutHelper.CreateControl();
      control.Label = label;
      return control;
    }

    public static Group CreateGroup(string pageId, string label, bool isSealedGroup)
    {
      string id = pageId + "." + label;
      if (isSealedGroup)
        id = id + "." + WebLayoutXmlHelper.SealedGroupSuffix;
      return LayoutHelper.CreateGroup(id, label);
    }

    public static Group CreateGroup(string id, string label)
    {
      Group group = new Group();
      group.Id = id;
      group.Label = label;
      group.Visible = new bool?(true);
      group.FromInheritedLayout = false;
      return group;
    }

    public static Page CreatePage(string id, string label, bool locked, PageType type)
    {
      Page page = new Page(true);
      page.Id = LayoutHelper.NormalizeLayoutElementId(id);
      page.Label = label;
      page.Visible = new bool?(true);
      page.FromInheritedLayout = false;
      page.Locked = new bool?(locked);
      page.PageType = type;
      return page;
    }

    private string BuildLinkOptions()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string defaultLinkColumn in this.m_defaultLinkColumns)
        stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<{0} {1}=\"{2}\" />", (object) ProvisionTags.WebLayoutLinkColumn, (object) ProvisionAttributes.WebLayoutColumnName, (object) defaultLinkColumn));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<{0} {1}=\"{2}\" />", (object) ProvisionTags.WebLayoutLinkColumn, (object) ProvisionAttributes.WebLayoutColumnName, (object) ProvisionValues.ColumnLinkComment));
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<{0} ZeroDataExperience=\"CallToAction\"><{1}>{2}</{1}></{0}>", (object) ProvisionTags.LinksControlOptions, (object) ProvisionTags.WebLayoutLinkColumns, (object) stringBuilder);
    }

    public static string NormalizeLayoutElementId(string id) => id.Replace("/", " ");

    public static Layout RemoveDeploymentsControls(Layout layout)
    {
      Layout layout1 = layout.Clone();
      foreach (LayoutNodeContainer<Section> child1 in (IEnumerable<Page>) layout1.Children)
      {
        foreach (LayoutNodeContainer<Group> child2 in (IEnumerable<Section>) child1.Children)
        {
          foreach (Group child3 in (IEnumerable<Group>) child2.Children)
          {
            Control control = child3.Children.FirstOrDefault<Control>((Func<Control, bool>) (g => g.ControlType == WellKnownControlNames.DeploymentsControl));
            if (control != null)
              child3.Children.Remove(control);
          }
        }
      }
      return layout1;
    }
  }
}
