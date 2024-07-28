// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.LayoutOperations
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  public class LayoutOperations : ILayoutOperations
  {
    private static readonly HashSet<string> s_Details = new HashSet<string>()
    {
      "详细信息",
      "詳細資料",
      "Detaily",
      "Details",
      "Detalles",
      "Détails",
      "Dettagli",
      "詳細",
      "정보",
      "Szczegóły",
      "Detalhes",
      "Подробно",
      "Ayrıntılar"
    };

    private bool IsDetails(string maybeDetails) => LayoutOperations.s_Details.Contains(maybeDetails);

    public Layout BuildCombinedLayout(
      IVssRequestContext requestContext,
      Layout baseLayout,
      Layout deltaLayout)
    {
      ArgumentUtility.CheckForNull<Layout>(baseLayout, nameof (baseLayout));
      ArgumentUtility.CheckForNull<Layout>(deltaLayout, nameof (deltaLayout));
      Layout mergedLayout = baseLayout.Clone();
      Layout deltaLayout1 = deltaLayout.Clone();
      mergedLayout.GetDescendants<LayoutNode>().ToList<LayoutNode>().ForEach((Action<LayoutNode>) (node => node.FromInheritedLayout = true));
      List<Page> list = mergedLayout.GetDescendants<Page>().Cast<Page>().ToList<Page>();
      Dictionary<string, Page> dictionary = list.ToDictionary<Page, string, Page>((Func<Page, string>) (x => x.Id), (Func<Page, Page>) (x => x), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (Page page1 in deltaLayout.GetDescendants<Page>().Cast<Page>().ToList<Page>())
      {
        Page combinedPage;
        if (dictionary.TryGetValue(page1.Id, out combinedPage))
          this.Merge(combinedPage, page1);
        else if (page1.Id.Equals("$inherited", StringComparison.OrdinalIgnoreCase))
        {
          foreach (Group deltaGroup in page1.GetDescendants<Group>().Cast<Group>())
          {
            Group combinedGroup = mergedLayout.FindDescendant<Group>(deltaGroup.Id);
            if (combinedGroup == null && WorkItemTrackingFeatureFlags.IsDetailsLocalizationFixEnabled(requestContext))
            {
              string[] strArray = deltaGroup.Id.Split('.');
              if (strArray.Length == 4)
              {
                string maybeDetails = strArray[2];
                string str = strArray[3];
                if (str != null && maybeDetails != null && this.IsDetails(maybeDetails))
                {
                  Page page2 = list.FirstOrDefault<Page>();
                  string groupId = page2?.Id + "." + str;
                  Group group;
                  if (page2 == null)
                  {
                    group = (Group) null;
                  }
                  else
                  {
                    // ISSUE: explicit non-virtual call
                    IEnumerable<Group> descendants = __nonvirtual (page2.GetDescendants<Group>());
                    if (descendants == null)
                    {
                      group = (Group) null;
                    }
                    else
                    {
                      IEnumerable<Group> source = descendants.Cast<Group>();
                      group = source != null ? source.FirstOrDefault<Group>((Func<Group, bool>) (g => string.Equals(g.Id, groupId, StringComparison.OrdinalIgnoreCase))) : (Group) null;
                    }
                  }
                  combinedGroup = group;
                }
              }
            }
            if (combinedGroup != null)
              this.Merge(combinedGroup, deltaGroup);
            else
              mergedLayout.OrphanedNodes.Add(deltaGroup);
          }
        }
        else if (this.IsDetailsPage(requestContext, page1.Id, list.FirstOrDefault<Page>()?.Id))
          this.Merge(list.First<Page>(), page1);
        else
          mergedLayout.Children.InsertRankedItem<Page>(page1, IfThereAreDuplicateRanks.InsertAfterItemsWithSameRank);
      }
      this.MergeSystemControls(mergedLayout, deltaLayout1);
      return mergedLayout;
    }

    private bool IsDetailsPage(
      IVssRequestContext requestContext,
      string deltaPageId,
      string basePageId)
    {
      if (!WorkItemTrackingFeatureFlags.IsDetailsLocalizationFixEnabled(requestContext) || basePageId == null)
        return false;
      string[] strArray1 = deltaPageId.Split('.');
      string[] strArray2 = basePageId.Split('.');
      if (strArray1.Length == 3 && this.IsDetails(strArray1[2]) && strArray2.Length == 3)
      {
        if (string.Equals(string.Join(".", new string[3]
        {
          strArray1[0],
          strArray1[1],
          strArray2[2]
        }), basePageId, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      if (strArray1.Length == 4 && this.IsDetails(strArray1[3]) && strArray2.Length == 4)
      {
        if (string.Equals(string.Join(".", new string[4]
        {
          strArray1[0],
          strArray1[1],
          strArray1[2],
          strArray2[3]
        }), basePageId, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    private void Merge(Page combinedPage, Page deltaPage)
    {
      Dictionary<string, Section> dictionary = combinedPage.Children.Where<Section>((Func<Section, bool>) (x => x.Id != null)).ToDictionary<Section, string, Section>((Func<Section, string>) (x => x.Id), (Func<Section, Section>) (x => x), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (!string.IsNullOrEmpty(deltaPage.Label))
      {
        combinedPage.Label = deltaPage.Label;
        combinedPage.Overridden = new bool?(true);
      }
      foreach (Section section in deltaPage.Children.Where<Section>((Func<Section, bool>) (x => x.Id != null)))
      {
        Section combinedSection;
        if (!dictionary.TryGetValue(section.Id, out combinedSection))
          combinedPage.Children.InsertRankedItem<Section>(section, IfThereAreDuplicateRanks.InsertAfterItemsWithSameRank);
        else
          this.Merge(combinedSection, section);
      }
    }

    private void Merge(Section combinedSection, Section deltaSection)
    {
      Dictionary<string, Group> dictionary = combinedSection.Children.Where<Group>((Func<Group, bool>) (x => x.Id != null)).ToDictionary<Group, string, Group>((Func<Group, string>) (x => x.Id), (Func<Group, Group>) (x => x), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (Group group in deltaSection.Children.Where<Group>((Func<Group, bool>) (x => x.Id != null)))
      {
        Group combinedGroup;
        if (!dictionary.TryGetValue(group.Id, out combinedGroup))
          combinedSection.Children.InsertRankedItem<Group>(group, IfThereAreDuplicateRanks.InsertAfterItemsWithSameRank);
        else
          this.Merge(combinedGroup, group);
      }
    }

    private void Merge(Group combinedGroup, Group deltaGroup)
    {
      Dictionary<string, Control> dictionary = combinedGroup.Children.Where<Control>((Func<Control, bool>) (x => x.Id != null)).ToDictionary<Control, string, Control>((Func<Control, string>) (x => x.Id), (Func<Control, Control>) (x => x), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (!string.IsNullOrEmpty(deltaGroup.Label))
      {
        combinedGroup.Label = deltaGroup.Label;
        combinedGroup.Overridden = new bool?(true);
      }
      foreach (Control control in deltaGroup.Children.Where<Control>((Func<Control, bool>) (x => x.Id != null)))
      {
        Control mergedControl;
        if (!dictionary.TryGetValue(control.Id, out mergedControl))
          combinedGroup.Children.InsertRankedItem<Control>(control, IfThereAreDuplicateRanks.InsertAfterItemsWithSameRank);
        else
          this.Merge(mergedControl, control);
      }
    }

    private void Merge(Control mergedControl, Control deltaControl)
    {
      if (!string.IsNullOrEmpty(deltaControl.Label))
      {
        mergedControl.Label = deltaControl.Label;
        mergedControl.Overridden = new bool?(true);
      }
      if (!deltaControl.Visible.HasValue)
        return;
      mergedControl.Visible = deltaControl.Visible;
      mergedControl.Overridden = new bool?(true);
    }

    public Section EnsureInheritSection(Layout layout)
    {
      Page page = layout.Children.FirstOrDefault<Page>((Func<Page, bool>) (x => x.IsInheritPlaceholderNode()));
      if (page == null)
      {
        page = Page.CreateInheritPlaceholderNode();
        layout.Children.Insert(0, page);
      }
      return page.Children.First<Section>();
    }

    public Layout PutControlInExistingGroup(
      string witRefName,
      Layout combinedLayout,
      Layout deltaLayout,
      string groupId,
      Control control,
      int? order)
    {
      deltaLayout = deltaLayout.Clone();
      Group group1 = combinedLayout.GetDescendants<Group>().FirstOrDefault<Group>((Func<Group, bool>) (x => x.Id.Equals(groupId, StringComparison.OrdinalIgnoreCase)));
      Group group2 = deltaLayout.GetDescendants<Group>().FirstOrDefault<Group>((Func<Group, bool>) (x => x.Id.Equals(groupId, StringComparison.OrdinalIgnoreCase)));
      if (group1 == null)
        throw new FormLayoutGroupDoesNotExistException(witRefName, groupId);
      combinedLayout.GetDescendants<Page>().Cast<Page>().Where<Page>((Func<Page, bool>) (p => p.GetDescendants<Group>().Any<Group>((Func<Group, bool>) (g => g.Id.Equals(groupId, StringComparison.OrdinalIgnoreCase))))).FirstOrDefault<Page>();
      if (group2 == null)
      {
        Section section = this.EnsureInheritSection(deltaLayout);
        Group group3 = new Group();
        group3.Id = groupId;
        group2 = group3;
        section.Children.Add(group2);
      }
      group2.Children.InsertRankedItem<Control>(control, group1.Children, order);
      return deltaLayout;
    }

    public Layout RemoveControlFromGroup(
      string witRefName,
      Layout composedLayout,
      Layout deltaLayout,
      string groupId,
      string fieldRefName)
    {
      deltaLayout = deltaLayout.Clone();
      Group descendant = deltaLayout.FindDescendant<Group>(groupId);
      Control control = descendant != null ? descendant.FindDescendant<Control>(fieldRefName) : throw new FormLayoutGroupDoesNotExistException(witRefName, groupId);
      if (control != null)
      {
        Section ancestorOf1 = composedLayout.FindAncestorOf<Group, Section>(groupId);
        Page ancestorOf2 = composedLayout.FindAncestorOf<Group, Page>(groupId);
        if (descendant.Children.Any<Control>((Func<Control, bool>) (c => LayoutConstants.WideControls.Contains(c.ControlType))))
          deltaLayout = this.RemoveGroup(witRefName, composedLayout, deltaLayout, ancestorOf2.Id, ancestorOf1.Id, groupId, true);
        else
          descendant.Children.Remove(control);
      }
      return deltaLayout;
    }

    public Layout RemoveControlFromLayout(
      string witRefName,
      Layout composedLayout,
      Layout deltaLayout,
      string fieldRefName)
    {
      deltaLayout = deltaLayout.Clone();
      foreach (Group group in deltaLayout.Children.SelectMany<Page, Group>((Func<Page, IEnumerable<Group>>) (page => page.Children.SelectMany<Section, Group>((Func<Section, IEnumerable<Group>>) (section => (IEnumerable<Group>) section.Children)))))
      {
        Control descendant = group.FindDescendant<Control>(fieldRefName);
        if (descendant != null)
        {
          Section ancestorOf1 = composedLayout.FindAncestorOf<Group, Section>(group.Id);
          Page ancestorOf2 = composedLayout.FindAncestorOf<Group, Page>(group.Id);
          if (group.Children.Any<Control>((Func<Control, bool>) (c => LayoutConstants.WideControls.Contains(c.ControlType))))
            deltaLayout = this.RemoveGroup(witRefName, composedLayout, deltaLayout, ancestorOf2.Id, ancestorOf1.Id, group.Id, true);
          else
            group.Children.Remove(descendant);
        }
      }
      return deltaLayout;
    }

    public Layout AddGroup(
      string witRefName,
      Layout combinedLayout,
      Layout baseLayout,
      Layout deltaLayout,
      string pageId,
      string sectionId,
      Group group,
      int? order)
    {
      return this.AddOrEditGroup(witRefName, combinedLayout, baseLayout, deltaLayout, pageId, sectionId, group, order);
    }

    public Layout EditGroup(
      string witRefName,
      Layout composedLayout,
      Layout baseLayout,
      Layout deltaLayout,
      string pageId,
      string sectionId,
      Group group,
      int? order)
    {
      deltaLayout = deltaLayout.Clone();
      if (composedLayout.FindDescendant<Group>(group.Id) == null)
        throw new FormLayoutGroupDoesNotExistException(witRefName, group.Id);
      Group descendant = deltaLayout.FindDescendant<Group>(group.Id);
      if (descendant == null)
      {
        deltaLayout = this.AddOrEditGroup(witRefName, composedLayout, baseLayout, deltaLayout, pageId, sectionId, group, order);
      }
      else
      {
        descendant.Label = group.Label;
        bool? visible = group.Visible;
        if (visible.HasValue)
        {
          Group group1 = descendant;
          visible = group.Visible;
          bool? nullable = new bool?(visible.Value);
          group1.Visible = nullable;
        }
        if (order.HasValue)
        {
          Section ancestorOf1 = deltaLayout.FindAncestorOf<Group, Section>(descendant.Id);
          if (!ancestorOf1.IsInheritPlaceholderNode())
          {
            Section ancestorOf2 = composedLayout.FindAncestorOf<Group, Section>(group.Id);
            ancestorOf1.Children.InsertRankedItem<Group>(descendant, ancestorOf2.Children, order);
          }
        }
      }
      return deltaLayout;
    }

    public Layout RemoveGroup(
      string witRefName,
      Layout composedLayout,
      Layout deltaLayout,
      string pageId,
      string sectionId,
      string groupId,
      bool ignoreChildren = false)
    {
      deltaLayout = deltaLayout.Clone();
      Page child1 = (Page) null;
      if (!composedLayout.TryGetChild(pageId, out child1))
        throw new FormLayoutPageDoesNotExistException(witRefName, pageId);
      Section child2 = (Section) null;
      if (!child1.TryGetChild(sectionId, out child2))
        throw new FormLayoutSectionDoesNotExistException(witRefName, sectionId);
      Group descendant = child2.FindDescendant<Group>(groupId);
      if (descendant == null)
        throw new FormLayoutGroupDoesNotExistException(witRefName, groupId);
      if (descendant.Children.Any<Control>() && !ignoreChildren)
        throw new FormLayoutGroupHasChildrenException(witRefName, descendant.Label);
      deltaLayout.FindAncestorOf<Group, Section>(groupId).Children.Remove(deltaLayout.FindDescendant<Group>(groupId) ?? throw new FormLayoutGroupDoesNotExistException(witRefName, groupId));
      return deltaLayout;
    }

    private Layout AddOrEditGroup(
      string witRefName,
      Layout combinedLayout,
      Layout baseLayout,
      Layout deltaLayout,
      string pageId,
      string sectionId,
      Group group,
      int? order)
    {
      deltaLayout = deltaLayout.Clone();
      Page child1 = (Page) null;
      if (!combinedLayout.TryGetChild(pageId, out child1))
        throw new FormLayoutPageDoesNotExistException(witRefName, pageId);
      Section child2 = (Section) null;
      if (!child1.TryGetChild(sectionId, out child2))
        throw new FormLayoutSectionDoesNotExistException(witRefName, sectionId);
      if (string.IsNullOrEmpty(group.Id))
        group.Id = Guid.NewGuid().ToString();
      if (baseLayout.FindDescendant<Group>(group.Id) != null)
        this.EnsureInheritSection(deltaLayout).Children.Add(group);
      else
        this.AddGroupToPage(deltaLayout, child1.Id, child2, group, order);
      return deltaLayout;
    }

    private Layout AddGroupToPage(
      Layout deltaLayout,
      string pageId,
      Section composedLayoutSection,
      Group groupToAdd,
      int? order)
    {
      Page child;
      if (!deltaLayout.TryGetChild(pageId, out child))
      {
        Page page = new Page(true);
        page.Id = pageId;
        child = page;
        deltaLayout.Children.Add(child);
      }
      child.FindDescendant<Section>(composedLayoutSection.Id).Children.InsertRankedItem<Group>(groupToAdd, composedLayoutSection.Children, order);
      return deltaLayout;
    }

    private void MergeSystemControls(Layout mergedLayout, Layout deltaLayout)
    {
      Dictionary<string, Control> dictionary = mergedLayout.SystemControls.ToDictionary<Control, string, Control>((Func<Control, string>) (c => c.Id), (Func<Control, Control>) (c => c));
      foreach (Control systemControl in deltaLayout.SystemControls)
      {
        Control control;
        if (dictionary.TryGetValue(systemControl.Id, out control))
        {
          control.Visible = systemControl.Visible;
          control.Label = systemControl.Label;
        }
        else
        {
          string fieldReferenceName = systemControl.ReplacesFieldReferenceName;
          if (!string.IsNullOrEmpty(fieldReferenceName) && dictionary.ContainsKey(fieldReferenceName))
          {
            mergedLayout.SystemControls.Add(systemControl.Clone());
            mergedLayout.SystemControls.Remove(dictionary[fieldReferenceName]);
          }
        }
      }
    }

    public Layout AddGroupToPage(
      Layout deltaLayout,
      string pageId,
      string sectionId,
      Group groupToAdd)
    {
      Page child;
      if (!deltaLayout.TryGetChild(pageId, out child))
      {
        Page page = new Page(true);
        page.Id = pageId;
        child = page;
        deltaLayout.Children.Add(child);
      }
      child.FindDescendant<Section>(sectionId).Children.Add(groupToAdd);
      return deltaLayout;
    }

    public Layout AddPage(
      string witRefName,
      Layout composedLayout,
      Layout deltaLayout,
      Page page,
      int? order)
    {
      return this.AddOrEditPage(witRefName, composedLayout, deltaLayout, page, order);
    }

    private Layout AddOrEditPage(
      string witRefName,
      Layout combinedLayout,
      Layout deltaLayout,
      Page page,
      int? order)
    {
      deltaLayout = deltaLayout.Clone();
      if (!order.HasValue)
        order = new int?(combinedLayout.Children.Count<Page>((Func<Page, bool>) (p => p.PageType == PageType.Custom)));
      if (string.IsNullOrEmpty(page.Id))
        page.Id = Guid.NewGuid().ToString();
      deltaLayout.Children.InsertRankedItem<Page>(page, combinedLayout.Children, order);
      return deltaLayout;
    }

    public Layout EditPage(
      string witRefName,
      Layout composedLayout,
      Layout baseLayout,
      Layout deltaLayout,
      Page page,
      int? order)
    {
      deltaLayout = deltaLayout.Clone();
      if (composedLayout.FindDescendant<Page>(page.Id) == null)
        throw new FormLayoutPageDoesNotExistException(witRefName, page.Label);
      Page descendant = deltaLayout.FindDescendant<Page>(page.Id);
      if (descendant == null)
      {
        deltaLayout = this.AddOrEditPage(witRefName, composedLayout, deltaLayout, page, order);
      }
      else
      {
        descendant.Label = page.Label;
        bool? visible = page.Visible;
        if (visible.HasValue)
        {
          Page page1 = descendant;
          visible = page.Visible;
          bool? nullable = new bool?(visible.Value);
          page1.Visible = nullable;
        }
        if (order.HasValue && baseLayout.FindDescendant<Page>(descendant.Id) == null)
          deltaLayout.Children.InsertRankedItem<Page>(descendant, composedLayout.Children, order);
      }
      return deltaLayout;
    }

    public Layout RemovePage(string witRefName, Layout deltaLayout, string pageId)
    {
      deltaLayout = deltaLayout.Clone();
      Page child = (Page) null;
      if (!deltaLayout.TryGetChild(pageId, out child))
        throw new FormLayoutPageDoesNotExistException(witRefName, pageId);
      deltaLayout.Children.Remove(child);
      return deltaLayout;
    }

    public Layout AddOrEditSystemControls(Layout deltaLayout, ISet<Control> updatedSystemControls)
    {
      deltaLayout = deltaLayout.Clone();
      foreach (Control updatedSystemControl in (IEnumerable<Control>) updatedSystemControls)
      {
        deltaLayout.SystemControls.Remove(updatedSystemControl);
        deltaLayout.SystemControls.Add(updatedSystemControl);
      }
      return deltaLayout;
    }

    public Layout RemoveSystemControls(Layout deltaLayout, ISet<string> systemControlIds)
    {
      deltaLayout = deltaLayout.Clone();
      Control control = new Control();
      foreach (string systemControlId in (IEnumerable<string>) systemControlIds)
      {
        control.Id = systemControlId;
        if (!deltaLayout.SystemControls.Remove(control))
          throw new FormLayoutSystemControlDoesNotExistException(systemControlId);
      }
      return deltaLayout;
    }
  }
}
