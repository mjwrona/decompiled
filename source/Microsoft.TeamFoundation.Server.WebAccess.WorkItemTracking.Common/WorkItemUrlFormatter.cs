// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemUrlFormatter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Presentation;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class WorkItemUrlFormatter : IWorkItemUrlBuilder
  {
    private readonly TswaServerHyperlinkService m_serverLinkService;
    private readonly bool m_useProjectContextForUrl;
    private readonly IVssRequestContext m_requestContext;

    public WorkItemUrlFormatter(
      IVssRequestContext requestContext,
      TswaServerHyperlinkService serverLinkService,
      bool useProjectContextForUrl = false)
    {
      this.m_serverLinkService = serverLinkService;
      this.m_useProjectContextForUrl = useProjectContextForUrl;
      this.m_requestContext = requestContext;
    }

    public string GetWorkItemHyperlink(int workItemId) => this.m_serverLinkService.GetWorkItemEditorUrl(this.m_requestContext, workItemId).ToString();

    public string GetWorkItemHyperlink(Uri projectUri, int workItemId) => !this.m_useProjectContextForUrl ? this.m_serverLinkService.GetWorkItemEditorUrl(this.m_requestContext, workItemId).ToString() : this.m_serverLinkService.GetWorkItemEditorUrl(this.m_requestContext, projectUri, workItemId).ToString();

    public string GetQueryResultsHyperlink(Uri projectUri, Guid queryId) => this.m_serverLinkService.GetWorkItemQueryResultsUrl(this.m_requestContext, projectUri, queryId).ToString();
  }
}
