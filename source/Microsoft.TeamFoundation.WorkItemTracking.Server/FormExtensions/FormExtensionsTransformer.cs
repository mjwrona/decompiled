// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormExtensions.FormExtensionsTransformer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormExtensions
{
  public static class FormExtensionsTransformer
  {
    private const string m_groupcontributionsectionid = "Section2";

    public static string AddContributionPages(
      IVssRequestContext requestContext,
      string formXml,
      string workItemType)
    {
      IEnumerable<Contribution> filteredContributions = FormExtensionsUtility.GetFilteredContributions(requestContext, "ms.vss-work-web.work-item-form-page");
      if (!filteredContributions.Any<Contribution>())
        return formXml;
      XDocument xdocument = XDocument.Parse(formXml);
      if (xdocument == null)
        return formXml;
      XElement xelement1 = xdocument.Root.Element((XName) "Layout");
      if (xelement1 == null)
        return formXml;
      XElement xelement2 = xelement1.Elements().FirstOrDefault<XElement>();
      if (xelement2 == null || !VssStringComparer.XmlElement.Equals(xelement2.Name.LocalName, "TabGroup"))
      {
        XElement content = new XElement((XName) "TabGroup", (object) new XElement((XName) "Tab", new object[2]
        {
          (object) new XAttribute((XName) "Label", (object) workItemType),
          (object) xelement1.Elements()
        }));
        xelement1.ReplaceNodes((object) content);
      }
      foreach (Contribution contribution in filteredContributions)
      {
        XElement content = new XElement((XName) "Tab", new object[4]
        {
          (object) new XAttribute((XName) "Label", (object) FormExtensionsUtility.GetContributionName(contribution)),
          (object) new XAttribute((XName) "contributionid", (object) contribution.Id),
          (object) new XAttribute((XName) "showOnDeletedWorkItem", (object) FormExtensionsUtility.GetShowOnDeletedWorkItem(contribution)),
          (object) new XAttribute((XName) "instanceid", (object) (contribution.Properties["registeredObjectId"] ?? (JToken) new ContributionIdentifier(contribution.Id).RelativeId))
        });
        xelement1.Element((XName) "TabGroup").Add((object) content);
      }
      return xdocument.ToString();
    }

    public static Layout AddContributions(
      IVssRequestContext requestContext,
      Layout baseLayout,
      Layout deltaLayout)
    {
      Layout deltaLayout1 = deltaLayout.Clone();
      FormExtensionsTransformer.AddContributionGroups(requestContext, baseLayout, deltaLayout1);
      FormExtensionsTransformer.AddContributionPages(requestContext, baseLayout, deltaLayout1);
      IEnumerable<string> validContributionIds = FormExtensionsUtility.GetFilteredContributions(requestContext).Select<Contribution, string>((Func<Contribution, string>) (c => c.Id));
      FormExtensionsTransformer.FilterUnInstalledOrDisabledExtensions(deltaLayout1, validContributionIds);
      return deltaLayout1;
    }

    private static void AddContributionPages(
      IVssRequestContext requestContext,
      Layout baseLayout,
      Layout deltaLayout)
    {
      IEnumerable<Contribution> filteredContributions = FormExtensionsUtility.GetFilteredContributions(requestContext, "ms.vss-work-web.work-item-form-page");
      if (!filteredContributions.Any<Contribution>())
        return;
      int num = baseLayout.Children.Where<Page>((Func<Page, bool>) (page => page.Locked.HasValue && page.Locked.Value)).Select<Page, int>((Func<Page, int>) (page => page.Rank.Value - 1)).First<int>();
      IEnumerable<Page> source = deltaLayout.GetDescendants<Page>().Where<Page>((Func<Page, bool>) (g => g.IsContribution));
      foreach (Contribution contribution1 in filteredContributions)
      {
        Contribution contribution = contribution1;
        if (!source.Any<Page>((Func<Page, bool>) (p => p.Contribution.ContributionId.Equals(contribution.Id, StringComparison.OrdinalIgnoreCase))))
        {
          Page page = new Page(true);
          page.Label = FormExtensionsUtility.GetContributionName(contribution);
          page.Contribution = new WitContribution(contribution.Id, FormExtensionsUtility.GetShowOnDeletedWorkItem(contribution));
          page.Visible = new bool?(true);
          page.Id = contribution.Id;
          page.Rank = new int?(num);
          Page insertedObject = page;
          deltaLayout.Children.InsertRankedItem<Page>(insertedObject, IfThereAreDuplicateRanks.InsertAfterItemsWithSameRank);
        }
      }
    }

    private static void AddContributionGroups(
      IVssRequestContext requestContext,
      Layout baseLayout,
      Layout deltaLayout)
    {
      IEnumerable<Contribution> filteredContributions = FormExtensionsUtility.GetFilteredContributions(requestContext, "ms.vss-work-web.work-item-form-group");
      if (!filteredContributions.Any<Contribution>())
        return;
      Page page = deltaLayout.Children.FirstOrDefault<Page>((Func<Page, bool>) (p => !p.IsContribution && p.Id.Equals(baseLayout.Children.First<Page>().Id, StringComparison.OrdinalIgnoreCase)));
      if (page == null)
      {
        page = LayoutHelper.CreatePage(baseLayout.Children.First<Page>().Id, baseLayout.Children.First<Page>().Label, false, PageType.Custom);
        page.Rank = new int?(0);
        deltaLayout.Children.Add(page);
      }
      Section section1 = page.Children.Where<Section>((Func<Section, bool>) (sec => sec.Id.Equals("Section2", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<Section>();
      if (section1 == null)
      {
        Section section2 = new Section();
        section2.Id = "Section2";
        section1 = section2;
        page.Children.Add(section1);
      }
      IEnumerable<Group> source = deltaLayout.GetDescendants<Group>().Where<Group>((Func<Group, bool>) (g => g.IsContribution));
      foreach (Contribution contribution1 in filteredContributions)
      {
        Contribution contribution = contribution1;
        if (!source.Any<Group>((Func<Group, bool>) (g => g.Contribution.ContributionId.Equals(contribution.Id, StringComparison.OrdinalIgnoreCase))))
        {
          Group group1 = new Group();
          group1.Label = FormExtensionsUtility.GetContributionName(contribution);
          group1.Contribution = new WitContribution(contribution.Id, FormExtensionsUtility.GetShowOnDeletedWorkItem(contribution));
          group1.Visible = new bool?(true);
          group1.Id = contribution.Id;
          Group group2 = group1;
          section1.Children.Add(group2);
        }
      }
    }

    private static void FilterUnInstalledOrDisabledExtensions(
      Layout deltaLayout,
      IEnumerable<string> validContributionIds)
    {
      foreach (Page page in deltaLayout.Children.Where<Page>((Func<Page, bool>) (p => p.IsContribution && !validContributionIds.Contains<string>(p.Id, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))).ToList<Page>())
        deltaLayout.Children.Remove(page);
      foreach (Page child1 in (IEnumerable<Page>) deltaLayout.Children)
      {
        if (!child1.IsContribution)
        {
          foreach (Section child2 in (IEnumerable<Section>) child1.Children)
          {
            foreach (Group group in child2.Children.Where<Group>((Func<Group, bool>) (g => g.IsContribution && !validContributionIds.Contains<string>(g.Id, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))).ToList<Group>())
              child2.Children.Remove(group);
            foreach (Group child3 in (IEnumerable<Group>) child2.Children)
            {
              foreach (Control control in child3.Children.Where<Control>((Func<Control, bool>) (c => c.IsContribution && !validContributionIds.Contains<string>(c.Contribution.ContributionId, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))).ToList<Control>())
                child3.Children.Remove(control);
            }
          }
        }
      }
    }

    public static void ResolveFormContributions(Layout layout, IVssRequestContext requestContext)
    {
      IEnumerable<LayoutNode> source1 = layout.GetDescendants<LayoutNode>().Where<LayoutNode>((Func<LayoutNode, bool>) (l => l.IsContribution));
      if (source1 == null || !source1.Any<LayoutNode>())
        return;
      List<string> list = source1.Select<LayoutNode, string>((Func<LayoutNode, string>) (n => n.Contribution.ContributionId)).ToList<string>();
      if (!list.Any<string>())
        return;
      IEnumerable<Contribution> source2 = requestContext.GetService<IContributionService>().QueryContributions(requestContext, (IEnumerable<string>) list);
      foreach (LayoutNode layoutNode1 in source1)
      {
        LayoutNode layoutNode = layoutNode1;
        Contribution contribution = source2.FirstOrDefault<Contribution>((Func<Contribution, bool>) (c => c.Id.Equals(layoutNode.Contribution.ContributionId, StringComparison.OrdinalIgnoreCase)));
        if (contribution == null)
        {
          layoutNode.Visible = new bool?(false);
        }
        else
        {
          layoutNode.Contribution.LayoutInstanceId = Guid.NewGuid().ToString();
          layoutNode.Contribution.ShowOnDeletedWorkItem = FormExtensionsUtility.GetShowOnDeletedWorkItem(contribution);
          int? height;
          if (layoutNode is Group)
          {
            height = ((Group) layoutNode).Height;
            if (!height.HasValue)
              layoutNode.Contribution.Height = FormExtensionsUtility.GetContributionHeight(contribution, 150);
          }
          else if (layoutNode is Control)
          {
            height = ((Control) layoutNode).Height;
            if (!height.HasValue)
              layoutNode.Contribution.Height = FormExtensionsUtility.GetContributionHeight(contribution, 75);
          }
        }
      }
    }
  }
}
