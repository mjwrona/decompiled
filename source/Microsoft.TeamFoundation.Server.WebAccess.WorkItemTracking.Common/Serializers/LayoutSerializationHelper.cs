// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Serializers.LayoutSerializationHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Models;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Serializers
{
  public static class LayoutSerializationHelper
  {
    public static void InjectLinkTypes(
      IVssRequestContext requestContext,
      Layout layout,
      ProcessDescriptor descriptor)
    {
      LayoutSerializationHelper.InjectGitHubLinkType(requestContext, layout, descriptor);
      LayoutSerializationHelper.InjectGitHubIssueLinkType(requestContext, layout, descriptor);
      LayoutSerializationHelper.InjectWorkItemRemoteLinkTypes(requestContext, layout);
    }

    private static void InjectGitHubLinkType(
      IVssRequestContext requestContext,
      Layout layout,
      ProcessDescriptor descriptor)
    {
      if ((descriptor != null ? (descriptor.IsCustom ? 1 : 0) : 1) == 0)
        return;
      LayoutSerializationHelper.InjectLinkType(requestContext, layout, LinkFilterType.GitHub);
    }

    private static void InjectWorkItemRemoteLinkTypes(
      IVssRequestContext requestContext,
      Layout layout)
    {
      if (!CommonWITUtils.IsRemoteLinkingEnabled(requestContext))
        return;
      LayoutSerializationHelper.InjectLinkType(requestContext, layout, LinkFilterType.RemoteLinks);
    }

    private static void InjectGitHubIssueLinkType(
      IVssRequestContext requestContext,
      Layout layout,
      ProcessDescriptor descriptor)
    {
      if ((descriptor != null ? (descriptor.IsCustom ? 1 : 0) : 1) == 0)
        return;
      LayoutSerializationHelper.InjectLinkType(requestContext, layout, LinkFilterType.GitHubIssue);
    }

    private static void InjectLinkType(
      IVssRequestContext requestContext,
      Layout layout,
      LinkFilterType linkFilterType)
    {
      LinksControlTransformer controlTransformer = new LinksControlTransformer();
      try
      {
        controlTransformer.InjectLinkTypeFilter(layout, linkFilterType);
      }
      catch (Exception ex)
      {
        string str = layout?.Id ?? "A layout without id";
        requestContext.Trace(909814, TraceLevel.Error, "FormLayout", "FormTransformsLayer", str + " is meeting exception this with message: " + ex.Message);
      }
    }
  }
}
