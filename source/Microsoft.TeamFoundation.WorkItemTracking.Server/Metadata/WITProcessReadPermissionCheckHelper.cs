// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WITProcessReadPermissionCheckHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class WITProcessReadPermissionCheckHelper : IWITProcessReadPermissionCheckHelper
  {
    private IVssRequestContext m_requestContext;
    private Dictionary<Guid, bool> readProjectProcessPermissionCache;
    private Dictionary<Guid, bool> readProcessPermissionCache;
    private Dictionary<Guid, ProcessReadSecuredObject> projectProcessSecuredObjectCache;
    private Dictionary<Guid, ProcessReadSecuredObject> processSecuredObjectCache;

    public WITProcessReadPermissionCheckHelper(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.readProjectProcessPermissionCache = new Dictionary<Guid, bool>();
      this.readProcessPermissionCache = new Dictionary<Guid, bool>();
      this.projectProcessSecuredObjectCache = new Dictionary<Guid, ProcessReadSecuredObject>();
      this.processSecuredObjectCache = new Dictionary<Guid, ProcessReadSecuredObject>();
    }

    public bool HasProcessReadPermission(
      Guid processId,
      out ProcessReadSecuredObject processReadSecuredObject)
    {
      bool flag1 = false;
      bool flag2;
      if (!this.readProcessPermissionCache.TryGetValue(processId, out flag1))
      {
        flag2 = this.CheckPermissionForProcessDescriptor(this.m_requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.m_requestContext, processId), out processReadSecuredObject);
        this.readProcessPermissionCache[processId] = flag2;
        this.processSecuredObjectCache[processId] = processReadSecuredObject;
      }
      else
      {
        flag2 = this.readProcessPermissionCache[processId];
        processReadSecuredObject = this.processSecuredObjectCache[processId];
      }
      return flag2;
    }

    public bool HasProcessReadPermissionForProject(
      Guid projectId,
      out ProcessReadSecuredObject processReadSecuredObject)
    {
      this.m_requestContext.TraceEnter(902980, "WorkItemTracking", "QueryPermissionChecker", nameof (HasProcessReadPermissionForProject));
      try
      {
        string token1 = (string) null;
        int permission = 0;
        bool flag1 = false;
        bool flag2;
        if (!this.readProjectProcessPermissionCache.TryGetValue(projectId, out flag1))
        {
          this.m_requestContext.GetService<ITeamFoundationProcessService>();
          IWorkItemTrackingProcessService service = this.m_requestContext.GetService<IWorkItemTrackingProcessService>();
          ProcessDescriptor processDescriptor = (ProcessDescriptor) null;
          IVssRequestContext requestContext1 = this.m_requestContext;
          Guid projectId1 = projectId;
          ref ProcessDescriptor local = ref processDescriptor;
          if (service.TryGetLatestProjectProcessDescriptor(requestContext1, projectId1, out local) && this.m_requestContext.ExecutionEnvironment.IsHostedDeployment)
          {
            flag2 = this.CheckPermissionForProcessDescriptor(processDescriptor, out processReadSecuredObject);
          }
          else
          {
            IVssSecurityNamespace securityNamespace = this.m_requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(this.m_requestContext, TeamProjectSecurityConstants.NamespaceId);
            permission = TeamProjectSecurityConstants.GenericRead;
            token1 = TeamProjectSecurityConstants.GetToken(ProjectInfo.GetProjectUri(projectId));
            IVssRequestContext requestContext2 = this.m_requestContext;
            string token2 = token1;
            int requestedPermissions = permission;
            flag2 = securityNamespace.HasPermission(requestContext2, token2, requestedPermissions);
            processReadSecuredObject = flag2 ? new ProcessReadSecuredObject(TeamProjectSecurityConstants.NamespaceId, permission, token1) : (ProcessReadSecuredObject) null;
          }
          this.readProjectProcessPermissionCache[projectId] = flag2;
          this.projectProcessSecuredObjectCache[projectId] = processReadSecuredObject;
          this.m_requestContext.Trace(902981, TraceLevel.Info, "WorkItemTracking", "QueryPermissionChecker", string.Format("Process Read Permission cache was not hit for projectId: {0} which resolved to token: {1} , permission: {2}. ", (object) projectId, (object) token1, (object) permission));
        }
        else
        {
          flag2 = this.readProjectProcessPermissionCache[projectId];
          processReadSecuredObject = this.projectProcessSecuredObjectCache[projectId];
          this.m_requestContext.Trace(902982, TraceLevel.Info, "WorkItemTracking", "QueryPermissionChecker", string.Format("Process Read Permission cache got hit for projectId: {0} which resolved to token: {1} , permission: {2}. ", (object) projectId, (object) token1, (object) permission));
        }
        if (!flag2)
          this.m_requestContext.Trace(902983, TraceLevel.Warning, "WorkItemTracking", "QueryPermissionChecker", string.Format("User {0} does not have permission for projectId: {1} which resolved to token: {2} , permission: {3}. ", (object) this.m_requestContext.GetUserId(), (object) projectId, (object) token1, (object) permission));
        return flag2;
      }
      finally
      {
        this.m_requestContext.TraceLeave(902990, "WorkItemTracking", "QueryPermissionChecker", nameof (HasProcessReadPermissionForProject));
      }
    }

    private bool CheckPermissionForProcessDescriptor(
      ProcessDescriptor processDescriptor,
      out ProcessReadSecuredObject processReadSecuredObject)
    {
      this.m_requestContext.GetService<IWorkItemTrackingProcessService>();
      ITeamFoundationProcessService service = this.m_requestContext.GetService<ITeamFoundationProcessService>();
      bool flag;
      if (this.m_requestContext.ExecutionEnvironment.IsHostedDeployment && service.IsProcessEnabled(this.m_requestContext))
      {
        string securityToken = service.GetSecurityToken(this.m_requestContext, processDescriptor);
        int num = 16;
        flag = service.HasProcessPermission(this.m_requestContext, num, processDescriptor, false);
        processReadSecuredObject = flag ? new ProcessReadSecuredObject(FrameworkSecurity.ProcessNamespaceId, num, securityToken) : (ProcessReadSecuredObject) null;
      }
      else
      {
        flag = true;
        processReadSecuredObject = (ProcessReadSecuredObject) null;
      }
      return flag;
    }
  }
}
