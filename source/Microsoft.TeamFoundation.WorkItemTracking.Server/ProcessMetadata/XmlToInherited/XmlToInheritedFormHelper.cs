// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.XmlToInherited.XmlToInheritedFormHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.XmlToInherited
{
  internal static class XmlToInheritedFormHelper
  {
    private static readonly HashSet<string> CustomizableControlTypes = new HashSet<string>((IEnumerable<string>) new string[4]
    {
      WellKnownControlNames.ClassificationControl,
      WellKnownControlNames.HtmlControl,
      WellKnownControlNames.DateControl,
      WellKnownControlNames.FieldControl
    });

    internal static void MergeForms(
      IVssRequestContext requestContext,
      Guid targetTemplateId,
      string witRefName,
      Layout inheritedLayout,
      Layout xmlLayout)
    {
      inheritedLayout = inheritedLayout.Clone();
      xmlLayout = xmlLayout.Clone();
      IList<Page> children = xmlLayout.Children;
      XmlToInheritedFormHelper.CleanupXmlLayout(xmlLayout);
      foreach (Page child in (IEnumerable<Page>) inheritedLayout.Children)
      {
        Page pageFromInheritedLayout = child;
        Page page = xmlLayout.Children.FirstOrDefault<Page>((Func<Page, bool>) (c => c.Label.Equals(pageFromInheritedLayout.Label, StringComparison.OrdinalIgnoreCase)));
        if (page != null)
        {
          children.Remove(page);
          XmlToInheritedFormHelper.MakeCustomizableControlsOnPageInvisible(requestContext, targetTemplateId, witRefName, pageFromInheritedLayout);
          XmlToInheritedFormHelper.MergeDuplicateSingleHtmlControlsToInherited(requestContext, targetTemplateId, witRefName, pageFromInheritedLayout, page);
          XmlToInheritedFormHelper.MergeDuplicateXmlGroupsAndControlsToInherited(requestContext, targetTemplateId, witRefName, pageFromInheritedLayout, page);
          XmlToInheritedFormHelper.RemoveEmptyCustomizations(page);
          XmlToInheritedFormHelper.MergeControlsUniqueToXml(requestContext, targetTemplateId, witRefName, pageFromInheritedLayout, page);
        }
      }
      XmlToInheritedFormHelper.AddXmlPagesToInherited(requestContext, targetTemplateId, witRefName, children);
    }

    internal static void AddXmlPagesToInherited(
      IVssRequestContext requestContext,
      Guid targetTemplateId,
      string witRefName,
      IList<Page> xmlPages)
    {
      IFormLayoutService service = requestContext.GetService<IFormLayoutService>();
      foreach (Page xmlPage1 in (IEnumerable<Page>) xmlPages)
      {
        Page xmlPage = xmlPage1;
        Page xmlPage2 = xmlPage.Clone();
        foreach (LayoutNodeContainer<Group> child in (IEnumerable<Section>) xmlPage.Children)
          child.Children.Clear();
        try
        {
          Page inheritedPage = service.AddPage(requestContext, targetTemplateId, witRefName, xmlPage, new int?()).Children.First<Page>((Func<Page, bool>) (p => p.Label != null && p.Label.Equals(xmlPage.Label, StringComparison.OrdinalIgnoreCase)));
          XmlToInheritedFormHelper.MergeControlsUniqueToXml(requestContext, targetTemplateId, witRefName, inheritedPage, xmlPage2);
        }
        catch (FormLayoutPageAlreadyExistsException ex)
        {
          requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.WorkItemTracking, nameof (XmlToInheritedFormHelper), CustomerIntelligenceProperty.Failed, "Exception of type " + ex.GetType().Name + " caught while trying to add page, stacktrace '" + ex.StackTrace + "'");
        }
      }
    }

    internal static void CleanupXmlLayout(Layout xmlLayout)
    {
      foreach (Group group in xmlLayout.GetDescendants<Group>().Cast<Group>())
      {
        if (!string.IsNullOrEmpty(group?.Label))
          group.Label = FormValidationUtils.MakeLabelValid(group.Label);
        foreach (Control control in group.Children.ToList<Control>())
        {
          if (!control.IsContribution && !XmlToInheritedFormHelper.CustomizableControlTypes.Contains(control.ControlType))
            group.Children.Remove(control);
          else if (!string.IsNullOrEmpty(control?.Label))
            control.Label = FormValidationUtils.MakeLabelValid(control.Label);
        }
      }
      foreach (Page page in xmlLayout.GetDescendants<Page>().Cast<Page>())
        page.Label = FormValidationUtils.MakeLabelValid(page.Label);
      foreach (Page page in xmlLayout.Children.Where<Page>((Func<Page, bool>) (p => p.Id.EndsWith(WebLayoutXmlHelper.WorkItemFormLinksPageIdSuffix) || p.Id.EndsWith(WebLayoutXmlHelper.WorkItemFormHistoryPageIdSuffix) || p.Id.EndsWith(WebLayoutXmlHelper.WorkItemFormAttachmentsPageIdSuffix))).ToList<Page>())
        xmlLayout.Children.Remove(page);
    }

    internal static void MakeCustomizableControlsOnPageInvisible(
      IVssRequestContext requestContext,
      Guid targetTemplateId,
      string witRefName,
      Page page)
    {
      foreach (Group group in page.GetDescendants<Group>().Cast<Group>())
      {
        foreach (Control child in (IEnumerable<Control>) group.Children)
        {
          if (XmlToInheritedFormHelper.CustomizableControlTypes.Contains(child.ControlType))
          {
            child.Visible = new bool?(false);
            XmlToInheritedFormHelper.SetFieldControlInGroup(requestContext, targetTemplateId, witRefName, group.Id, child, new int?(), true);
          }
        }
      }
    }

    internal static void MergeDuplicateSingleHtmlControlsToInherited(
      IVssRequestContext requestContext,
      Guid targetTemplateId,
      string witRefName,
      Page inheritedPage,
      Page xmlPage)
    {
      foreach (Group group1 in inheritedPage.GetDescendants<Group>().Cast<Group>())
      {
        if (group1.Children.Count == 1 && group1.Children.First<Control>().ControlType == WellKnownControlNames.HtmlControl)
        {
          Control inheritedHtmlControl = group1.Children.First<Control>();
          foreach (Group group2 in xmlPage.GetDescendants<Group>().Cast<Group>().ToList<Group>())
          {
            Control control = group2.Children.Where<Control>((Func<Control, bool>) (c => TFStringComparer.WorkItemTypeReferenceName.Equals(c.Id, inheritedHtmlControl.Id) && c.ControlType == inheritedHtmlControl.ControlType)).FirstOrDefault<Control>();
            if (control != null)
            {
              inheritedHtmlControl.Label = group2.Label;
              inheritedHtmlControl.Visible = new bool?(true);
              XmlToInheritedFormHelper.SetFieldControlInGroup(requestContext, targetTemplateId, witRefName, group1.Id, inheritedHtmlControl, new int?(), true);
              group2.Children.Remove(control);
            }
          }
        }
      }
    }

    internal static void MergeDuplicateXmlGroupsAndControlsToInherited(
      IVssRequestContext requestContext,
      Guid targetTemplateId,
      string witRefName,
      Page inheritedPage,
      Page xmlPage)
    {
      Dictionary<string, Group> dedupedDictionary = inheritedPage.GetDescendants<Group>().Cast<Group>().Where<Group>((Func<Group, bool>) (g => g.Children.Count != 1 || g.Children.Single<Control>().ControlType != WellKnownControlNames.HtmlControl)).Where<Group>((Func<Group, bool>) (g => g.Label != null)).ToDedupedDictionary<Group, string, Group>((Func<Group, string>) (g => g.Label), (Func<Group, Group>) (g => g), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      dedupedDictionary.Values.Where<Group>((Func<Group, bool>) (g => g.Children.Count != 1 || g.Children.Single<Control>().ControlType != WellKnownControlNames.HtmlControl)).SelectMany<Group, Control>((Func<Group, IEnumerable<Control>>) (g => (IEnumerable<Control>) g.Children)).Where<Control>((Func<Control, bool>) (g => g?.Id != null)).ToDedupedDictionary<Control, string, Control>((Func<Control, string>) (g => g.Id), (Func<Control, Control>) (g => g), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (Section section in xmlPage.Children.ToList<Section>())
      {
        foreach (Group group1 in section.Children.ToList<Group>())
        {
          Group group2;
          if (dedupedDictionary.TryGetValue(group1.Label, out group2))
          {
            foreach (Control control1 in group1.Children.ToList<Control>())
            {
              Control xmlControl = control1;
              Control control2 = group2.Children.Where<Control>((Func<Control, bool>) (c => TFStringComparer.WorkItemTypeReferenceName.Equals(c.Id, xmlControl.Id))).FirstOrDefault<Control>();
              if (control2 != null)
              {
                group1.Children.Remove(xmlControl);
                control2.Label = xmlControl.Label;
                control2.Visible = new bool?(true);
                XmlToInheritedFormHelper.SetFieldControlInGroup(requestContext, targetTemplateId, witRefName, group2.Id, control2, new int?(), true);
              }
              else
                XmlToInheritedFormHelper.SetFieldControlInGroup(requestContext, targetTemplateId, witRefName, group2.Id, xmlControl, new int?(), false);
            }
            group1.Children.Clear();
            section.Children.Remove(group1);
          }
        }
      }
    }

    internal static void RemoveEmptyCustomizations(Page page)
    {
      foreach (Section section in page.Children.ToList<Section>())
      {
        foreach (Group group in section.Children.Where<Group>((Func<Group, bool>) (g => !g.Children.Any<Control>())).ToList<Group>())
          section.Children.Remove(group);
      }
    }

    internal static void MergeControlsUniqueToXml(
      IVssRequestContext requestContext,
      Guid targetTemplateId,
      string witRefName,
      Page inheritedPage,
      Page xmlPage)
    {
      IFormLayoutService service = requestContext.GetService<IFormLayoutService>();
      Section child1 = inheritedPage.Children[Math.Min(inheritedPage.Children.Count - 1, 2)];
      for (int index1 = 0; index1 < xmlPage.Children.Count; ++index1)
      {
        int index2 = Math.Min(index1, 2);
        Section child2 = xmlPage.Children[index1];
        Section section = (index2 < inheritedPage.Children.Count ? inheritedPage.Children[index2] : (Section) null) ?? child1;
        foreach (Group child3 in (IEnumerable<Group>) child2.Children)
        {
          if (!child3.IsContribution)
          {
            service.AddGroup(requestContext, targetTemplateId, witRefName, inheritedPage.Id, section.Id, child3, new int?());
            foreach (Control child4 in (IEnumerable<Control>) child3.Children)
              XmlToInheritedFormHelper.SetFieldControlInGroup(requestContext, targetTemplateId, witRefName, child3.Id, child4, new int?(), false);
          }
        }
      }
    }

    private static void SetFieldControlInGroup(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string groupId,
      Control control,
      int? order,
      bool isEdit)
    {
      IFormLayoutService service = requestContext.GetService<IFormLayoutService>();
      try
      {
        service.SetFieldControlInGroup(requestContext, processId, witRefName, groupId, control, order, isEdit);
      }
      catch (Exception ex)
      {
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.WorkItemTracking, nameof (XmlToInheritedFormHelper), CustomerIntelligenceProperty.Failed, "Exception of type " + ex.GetType().Name + " caught while trying to add field id '" + control.Id + "' to group id '" + groupId + "', stacktrace '" + ex.StackTrace + "'");
      }
    }
  }
}
