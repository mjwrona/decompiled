// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.TeamFoundationQueryItemPermissionService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems
{
  public class TeamFoundationQueryItemPermissionService : 
    ITeamFoundationQueryItemPermissionService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool HasQueryPermission(
      IVssRequestContext requestContext,
      QueryItem query,
      int requestedPermission,
      bool? alwaysAllowAdmins = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<QueryItem>(query, nameof (query));
      return requestContext.TraceBlock<bool>(902810, 902819, 902818, "Services", "QueryPermissionService", nameof (HasQueryPermission), (Func<bool>) (() =>
      {
        bool flag;
        if (query.IsPublic)
        {
          bool alwaysAllowAdministrators = alwaysAllowAdmins.HasValue && alwaysAllowAdmins.Value || TeamFoundationQueryItemPermissionService.IsIrrevocableAdminPermission(requestedPermission);
          IVssSecurityNamespace securityNamespace = TeamFoundationQueryItemPermissionService.GetSecurityNamespace(requestContext);
          flag = securityNamespace.HasPermission(requestContext, query.SecurityToken, requestedPermission, alwaysAllowAdministrators);
          if (requestedPermission == 1)
            flag = flag || securityNamespace.HasPermissionForAnyChildren(requestContext, query.SecurityToken, requestedPermission, alwaysAllowAdministrators: alwaysAllowAdministrators);
        }
        else
          flag = query.CreatedById == requestContext.WitContext().RequestIdentity.Id;
        return flag;
      }));
    }

    public void CheckQueryPermission(
      IVssRequestContext requestContext,
      QueryItem query,
      int requestedPermission,
      bool? alwaysAllowAdmins = null)
    {
      if (!this.HasQueryPermission(requestContext, query, requestedPermission, alwaysAllowAdmins))
        throw new WorkItemTrackingQueryUnauthorizedAccessException(query.Id, TeamFoundationQueryItemPermissionService.GetAssociatedAccessType(requestedPermission));
    }

    public QueryItem FilterByReadPermission(IVssRequestContext requestContext, QueryItem query)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<QueryItem>(query, nameof (query));
      return this.FilterByReadPermission(requestContext, (IEnumerable<QueryItem>) new QueryItem[1]
      {
        query
      }).FirstOrDefault<QueryItem>();
    }

    public IEnumerable<QueryItem> FilterByReadPermission(
      IVssRequestContext requestContext,
      IEnumerable<QueryItem> queries)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<QueryItem>>(queries, nameof (queries));
      return requestContext.TraceBlock<IEnumerable<QueryItem>>(902830, 902839, 902838, "Services", "QueryPermissionService", nameof (FilterByReadPermission), (Func<IEnumerable<QueryItem>>) (() => this.FilterByReadPermissionInternal(requestContext.WitContext(), queries, TeamFoundationQueryItemPermissionService.GetSecurityNamespace(requestContext))));
    }

    private IEnumerable<QueryItem> FilterByReadPermissionInternal(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<QueryItem> queries,
      IVssSecurityNamespace securityNamespace)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      foreach (QueryItem query in queries)
      {
        if (query.IsPublic)
        {
          bool flag = securityNamespace.HasPermission(requestContext, query.SecurityToken, 1);
          if (!flag && query.ParentId == Guid.Empty && query.IsPublic && TeamFoundationQueryItemPermissionService.IsCreatingProject(requestContext, query.ProjectId))
            flag = true;
          if (flag || securityNamespace.HasPermissionForAnyChildren(requestContext, query.SecurityToken, 1))
          {
            if (query is QueryFolder queryFolder && (!securityNamespace.HasPermissionForAllChildren(requestContext, query.SecurityToken, 1) || !flag))
              queryFolder.Children = (IList<QueryItem>) this.FilterByReadPermissionInternal(witRequestContext, (IEnumerable<QueryItem>) queryFolder.Children, securityNamespace).ToList<QueryItem>();
            yield return query;
          }
        }
        else if (query.CreatedById.Equals(witRequestContext.RequestIdentity.Id))
          yield return query;
      }
    }

    private static bool IsIrrevocableAdminPermission(int permission) => permission == 1 || permission == 8;

    private static IVssSecurityNamespace GetSecurityNamespace(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, QueryItemSecurityConstants.NamespaceGuid);

    private static AccessType GetAssociatedAccessType(int requestedPermission) => requestedPermission == 4 ? AccessType.Delete : AccessType.Read;

    private static bool IsCreatingProject(IVssRequestContext tfRequestContext, Guid projectId)
    {
      IProjectService service1 = tfRequestContext.GetService<IProjectService>();
      try
      {
        ProjectInfo project = service1.GetProject(tfRequestContext.Elevate(), projectId);
        if (project != null)
        {
          if (project.State == ProjectState.New)
          {
            IdentityService service2 = tfRequestContext.GetService<IdentityService>();
            Microsoft.VisualStudio.Services.Identity.Identity identity = service2.ReadIdentities(tfRequestContext.Elevate(), IdentitySearchFilter.AdministratorsGroup, project.Uri, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
            return identity != null && service2.IsMemberOrSame(tfRequestContext, identity.Descriptor);
          }
        }
      }
      catch (TeamFoundationServiceException ex)
      {
      }
      catch (ProjectException ex)
      {
      }
      return false;
    }
  }
}
