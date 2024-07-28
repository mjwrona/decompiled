// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.PlatformServices.PlatformWorkItemQueryService
// Assembly: Microsoft.Azure.Devops.Work.PlatformServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7C8E511A-CB9A-4327-9803-A1164853E0F0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Work.PlatformServices.dll

using Microsoft.Azure.Boards.WebApi.Common.Helpers;
using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Devops.Work.PlatformServices
{
  public class PlatformWorkItemQueryService : IWorkItemQueryRemotableService, IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<QueryHierarchyItem> GetQueries(
      IVssRequestContext requestContext,
      Guid projectId,
      QueryExpand expand = QueryExpand.None,
      int depth = 0,
      bool includeDeleted = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return WorkItemQueryControllerHelper.GetQueries(requestContext, projectId, expand, depth, includeDeleted);
    }

    public IEnumerable<QueryHierarchyItem> GetQueries(
      IVssRequestContext requestContext,
      string projectName,
      QueryExpand expand = QueryExpand.None,
      int depth = 0,
      bool includeDeleted = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      IProjectService service = requestContext.GetService<IProjectService>();
      Guid projectId = Guid.Empty;
      if (projectName != null)
        projectId = service.GetProjectId(requestContext, projectName);
      return this.GetQueries(requestContext, projectId, expand, depth, includeDeleted);
    }

    public void ValidateQuery(IVssRequestContext requestContext, Guid projectId, string wiql)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckStringForNullOrEmpty(wiql, nameof (wiql));
      requestContext.GetService<IWorkItemQueryService>().ValidateWiql(requestContext, wiql, projectId);
    }
  }
}
