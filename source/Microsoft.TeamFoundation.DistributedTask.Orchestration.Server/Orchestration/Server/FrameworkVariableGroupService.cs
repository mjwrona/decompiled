// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.FrameworkVariableGroupService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class FrameworkVariableGroupService : IVariableGroupService, IVssFrameworkService
  {
    private const string c_layer = "FrameworkVariableGroupService";

    public VariableGroup AddVariableGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      VariableGroupParameters variableGroupParameters)
    {
      using (new MethodScope(requestContext, nameof (FrameworkVariableGroupService), nameof (AddVariableGroup)))
      {
        variableGroupParameters.VariableGroupProjectReferences = (IList<VariableGroupProjectReference>) new List<VariableGroupProjectReference>()
        {
          new VariableGroupProjectReference()
          {
            Name = variableGroupParameters.Name,
            Description = variableGroupParameters.Description,
            ProjectReference = new ProjectReference()
            {
              Id = projectId,
              Name = string.Empty
            }
          }
        };
        return requestContext.GetClient<TaskAgentHttpClient>().AddVariableGroupAsync(variableGroupParameters).SyncResult<VariableGroup>();
      }
    }

    public VariableGroup AddVariableGroupForCollection(
      IVssRequestContext requestContext,
      VariableGroupParameters variableGroupParameters)
    {
      using (new MethodScope(requestContext, nameof (FrameworkVariableGroupService), nameof (AddVariableGroupForCollection)))
        return requestContext.GetClient<TaskAgentHttpClient>().AddVariableGroupAsync(variableGroupParameters).SyncResult<VariableGroup>();
    }

    public void DeleteVariableGroup(IVssRequestContext requestContext, Guid projectId, int groupId)
    {
      using (new MethodScope(requestContext, nameof (FrameworkVariableGroupService), nameof (DeleteVariableGroup)))
      {
        TaskAgentHttpClient client = requestContext.GetClient<TaskAgentHttpClient>();
        int groupId1 = groupId;
        List<string> projectIds = new List<string>();
        projectIds.Add(projectId.ToString());
        CancellationToken cancellationToken = new CancellationToken();
        client.DeleteVariableGroupAsync(groupId1, (IEnumerable<string>) projectIds, cancellationToken: cancellationToken).SyncResult();
      }
    }

    public VariableGroup GetVariableGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      int groupId)
    {
      using (new MethodScope(requestContext, nameof (FrameworkVariableGroupService), nameof (GetVariableGroup)))
        return requestContext.GetClient<TaskAgentHttpClient>().GetVariableGroupAsync(projectId, groupId).SyncResult<VariableGroup>();
    }

    public IList<VariableGroup> GetVariableGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> groupIds,
      VariableGroupActionFilter actionFilter = VariableGroupActionFilter.None)
    {
      using (new MethodScope(requestContext, nameof (FrameworkVariableGroupService), nameof (GetVariableGroups)))
        return (IList<VariableGroup>) requestContext.GetClient<TaskAgentHttpClient>().GetVariableGroupsByIdAsync(projectId, (IEnumerable<int>) groupIds).SyncResult<List<VariableGroup>>();
    }

    public IList<VariableGroup> GetVariableGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      string groupName,
      VariableGroupActionFilter actionFilter = VariableGroupActionFilter.None,
      int? continuationToken = null,
      int top = -1,
      VariableGroupQueryOrder queryOrder = VariableGroupQueryOrder.IdDescending)
    {
      using (new MethodScope(requestContext, nameof (FrameworkVariableGroupService), nameof (GetVariableGroups)))
        return (IList<VariableGroup>) requestContext.GetClient<TaskAgentHttpClient>().GetVariableGroupsAsync(projectId, groupName, new VariableGroupActionFilter?(actionFilter), new int?(top), continuationToken, new VariableGroupQueryOrder?(queryOrder)).SyncResult<List<VariableGroup>>();
    }

    public VariableGroup UpdateVariableGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      int groupId,
      VariableGroupParameters variableGroupParameters)
    {
      using (new MethodScope(requestContext, nameof (FrameworkVariableGroupService), nameof (UpdateVariableGroup)))
      {
        variableGroupParameters.VariableGroupProjectReferences = (IList<VariableGroupProjectReference>) new List<VariableGroupProjectReference>()
        {
          new VariableGroupProjectReference()
          {
            Name = variableGroupParameters.Name,
            Description = variableGroupParameters.Description,
            ProjectReference = new ProjectReference()
            {
              Id = projectId,
              Name = string.Empty
            }
          }
        };
        return requestContext.GetClient<TaskAgentHttpClient>().UpdateVariableGroupAsync(groupId, variableGroupParameters).SyncResult<VariableGroup>();
      }
    }

    public VariableGroup UpdateVariableGroupForCollection(
      IVssRequestContext requestContext,
      int groupId,
      VariableGroupParameters updateVariableGroupParameters)
    {
      return requestContext.GetClient<TaskAgentHttpClient>().UpdateVariableGroupAsync(groupId, updateVariableGroupParameters).SyncResult<VariableGroup>();
    }

    public void DeleteVariableGroupForCollection(
      IVssRequestContext requestContext,
      int groupId,
      IList<Guid> projectIds)
    {
      IList<string> projectIds1 = (IList<string>) new List<string>();
      if (projectIds != null)
      {
        foreach (Guid projectId in (IEnumerable<Guid>) projectIds)
          projectIds1.Add(projectId.ToString());
      }
      requestContext.GetClient<TaskAgentHttpClient>().DeleteVariableGroupAsync(groupId, (IEnumerable<string>) projectIds1).SyncResult();
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public void ShareVariableGroupForCollection(
      IVssRequestContext requestContext,
      int groupId,
      IList<VariableGroupProjectReference> variableGroupProjectReferences)
    {
      requestContext.GetClient<TaskAgentHttpClient>().ShareVariableGroupAsync(groupId, (IEnumerable<VariableGroupProjectReference>) variableGroupProjectReferences).SyncResult();
    }
  }
}
