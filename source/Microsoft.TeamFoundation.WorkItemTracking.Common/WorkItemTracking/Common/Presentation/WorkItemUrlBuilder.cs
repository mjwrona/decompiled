// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Presentation.WorkItemUrlBuilder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Security;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Presentation
{
  internal class WorkItemUrlBuilder : IWorkItemUrlBuilder
  {
    private TswaClientHyperlinkService m_tswaLinkingService;
    private ILinking m_linkingService;
    private readonly bool m_useProjectContextForUrl;

    public WorkItemUrlBuilder(
      TfsTeamProjectCollection teamProjectCollection,
      bool useProjectContextForUrl = false)
    {
      this.m_tswaLinkingService = teamProjectCollection.GetService<TswaClientHyperlinkService>();
      this.m_linkingService = teamProjectCollection.GetService<ILinking>();
      this.m_useProjectContextForUrl = useProjectContextForUrl;
    }

    public string GetWorkItemHyperlink(int workItemId)
    {
      string workItemHyperlink = this.GetWorkItemTswaHyperlink(workItemId);
      if (string.IsNullOrEmpty(workItemHyperlink))
        workItemHyperlink = this.GetWorkItemPageHyperlink(workItemId);
      return workItemHyperlink;
    }

    public string GetWorkItemHyperlink(Uri projectUri, int workItemId)
    {
      string workItemHyperlink = this.m_useProjectContextForUrl ? this.GetWorkItemTswaHyperlink(projectUri, workItemId) : this.GetWorkItemHyperlink(workItemId);
      if (string.IsNullOrEmpty(workItemHyperlink))
        workItemHyperlink = this.GetWorkItemPageHyperlink(workItemId);
      return workItemHyperlink;
    }

    public string GetWorkItemTswaHyperlink(int workItemId)
    {
      try
      {
        if (this.m_tswaLinkingService != null)
          return this.m_tswaLinkingService.GetWorkItemEditorUrl(workItemId).AbsoluteUri;
      }
      catch (InvalidOperationException ex)
      {
        this.m_tswaLinkingService = (TswaClientHyperlinkService) null;
      }
      catch (NotSupportedException ex)
      {
        this.m_tswaLinkingService = (TswaClientHyperlinkService) null;
      }
      return string.Empty;
    }

    public string GetNoWebAccessHyperlink(string projectUri)
    {
      string empty = string.Empty;
      if (this.m_tswaLinkingService == null)
        return NonConfiguredSiteHelper.GenerateNotFoundUri(NonConfiguredSiteHelper.SiteType.WebAccess);
      try
      {
        return this.m_tswaLinkingService.GetHomeUrl(new Uri(projectUri)).AbsoluteUri;
      }
      catch (InvalidOperationException ex)
      {
        return NonConfiguredSiteHelper.GenerateNotFoundUri(NonConfiguredSiteHelper.SiteType.WebAccess);
      }
      catch (NotSupportedException ex)
      {
        return NonConfiguredSiteHelper.GenerateNotFoundUri(NonConfiguredSiteHelper.SiteType.WebAccess);
      }
    }

    public string GetQueryResultsHyperlink(Uri projectUri, Guid queryId)
    {
      try
      {
        if (this.m_tswaLinkingService != null)
          return this.m_tswaLinkingService.GetWorkItemQueryResultsUrl(projectUri, queryId).AbsoluteUri;
      }
      catch (InvalidOperationException ex)
      {
      }
      catch (NotSupportedException ex)
      {
      }
      return string.Empty;
    }

    public string GetWorkItemPageHyperlink(int workItemId)
    {
      try
      {
        return this.m_linkingService.GetArtifactUrlExternal(LinkingUtilities.EncodeUri(new ArtifactId()
        {
          ArtifactType = "WorkItem",
          Tool = "WorkItemTracking",
          ToolSpecificId = workItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        }));
      }
      catch (SecurityException ex)
      {
      }
      return string.Empty;
    }

    private string GetWorkItemTswaHyperlink(Uri projectUri, int workItemId)
    {
      try
      {
        if (this.m_tswaLinkingService != null)
          return this.m_tswaLinkingService.GetWorkItemEditorUrl(projectUri, workItemId).AbsoluteUri;
      }
      catch (InvalidOperationException ex)
      {
        this.m_tswaLinkingService = (TswaClientHyperlinkService) null;
      }
      catch (NotSupportedException ex)
      {
        this.m_tswaLinkingService = (TswaClientHyperlinkService) null;
      }
      return string.Empty;
    }
  }
}
