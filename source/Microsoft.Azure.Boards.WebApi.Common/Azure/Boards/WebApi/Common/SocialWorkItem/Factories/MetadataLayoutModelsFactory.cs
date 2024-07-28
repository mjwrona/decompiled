// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.SocialWorkItem.Factories.MetadataLayoutModelsFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SocialWorkItem.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common.SocialWorkItem.Factories
{
  internal static class MetadataLayoutModelsFactory
  {
    internal static IEnumerable<string> LayoutCoreFieldRefNames = (IEnumerable<string>) new string[5]
    {
      CoreFieldReferenceNames.AssignedTo,
      CoreFieldReferenceNames.State,
      CoreFieldReferenceNames.AreaPath,
      CoreFieldReferenceNames.IterationPath,
      CoreFieldReferenceNames.Tags
    };
    internal static IEnumerable<string> LayoutHeaderFieldRefNames = (IEnumerable<string>) new string[1]
    {
      CoreFieldReferenceNames.Title
    };

    internal static WorkItemLayout GetLayout(
      ISecuredObject securedObject,
      Layout webLayout,
      IDictionary<string, string> fieldNameByRefName)
    {
      string str = (string) null;
      HashSet<string> first = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      HashSet<string> fieldRefNames = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      foreach (Control visibleControl in MetadataLayoutModelsFactory.GetVisibleControls(webLayout))
      {
        if (!string.IsNullOrEmpty(visibleControl.Id) && !(visibleControl.Id == CoreFieldReferenceNames.History) && !(visibleControl.ControlType == "LinksControl"))
        {
          if (visibleControl.ControlType == "HtmlFieldControl")
          {
            if (str == null)
              str = visibleControl.Id;
            fieldRefNames.Add(visibleControl.Id);
          }
          else
            first.Add(visibleControl.Id);
        }
      }
      WorkItemLayout layout = new WorkItemLayout(securedObject);
      WorkItemLayout workItemLayout = layout;
      WorkItemLayoutPage workItemLayoutPage1 = new WorkItemLayoutPage(securedObject);
      workItemLayoutPage1.FieldRefNames = MetadataLayoutModelsFactory.LayoutCoreFieldRefNames;
      WorkItemLayoutPage workItemLayoutPage2 = workItemLayoutPage1;
      IEnumerable<string> strings;
      if (!string.IsNullOrEmpty(str))
        strings = (IEnumerable<string>) new string[1]{ str };
      else
        strings = Enumerable.Empty<string>();
      workItemLayoutPage2.RichTextFieldRefNames = strings;
      WorkItemLayoutPage workItemLayoutPage3 = workItemLayoutPage1;
      workItemLayout.Overview = workItemLayoutPage3;
      layout.AllFields = new WorkItemLayoutPage(securedObject)
      {
        FieldRefNames = MetadataLayoutModelsFactory.SortLayoutFields(fieldNameByRefName, first.Concat<string>(MetadataLayoutModelsFactory.LayoutCoreFieldRefNames)),
        RichTextFieldRefNames = MetadataLayoutModelsFactory.SortLayoutFields(fieldNameByRefName, (IEnumerable<string>) fieldRefNames)
      };
      return layout;
    }

    internal static IEnumerable<string> SortLayoutFields(
      IDictionary<string, string> fieldsByRefName,
      IEnumerable<string> fieldRefNames)
    {
      string str;
      return (IEnumerable<string>) fieldRefNames.Distinct<string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName).OrderBy<string, string>((Func<string, string>) (refName => fieldsByRefName.TryGetValue(refName, out str) ? str : string.Empty));
    }

    internal static IEnumerable<Control> GetVisibleControls(Layout webLayout)
    {
      foreach (LayoutNodeContainer<Section> layoutNodeContainer1 in webLayout.Children.Where<Page>((Func<Page, bool>) (p => (!p.Visible.HasValue || p.Visible.Value) && !p.IsContribution)))
      {
        foreach (LayoutNodeContainer<Group> layoutNodeContainer2 in layoutNodeContainer1.Children.Where<Section>((Func<Section, bool>) (s => (!s.Visible.HasValue || s.Visible.Value) && !s.IsContribution)))
        {
          foreach (LayoutNodeContainer<Control> layoutNodeContainer3 in layoutNodeContainer2.Children.Where<Group>((Func<Group, bool>) (g => (!g.Visible.HasValue || g.Visible.Value) && !g.IsContribution)))
          {
            foreach (Control visibleControl in layoutNodeContainer3.Children.Where<Control>((Func<Control, bool>) (c => c.Id != null && (!c.Visible.HasValue || c.Visible.Value) && !c.IsContribution)))
              yield return visibleControl;
          }
        }
      }
    }
  }
}
