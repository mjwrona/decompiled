// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.IVariableGroupService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [DefaultServiceImplementation(typeof (FrameworkVariableGroupService))]
  public interface IVariableGroupService : IVssFrameworkService
  {
    VariableGroup AddVariableGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      VariableGroupParameters variableGroupParameters);

    VariableGroup UpdateVariableGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      int groupId,
      VariableGroupParameters variableGroupParameters);

    VariableGroup GetVariableGroup(IVssRequestContext requestContext, Guid projectId, int groupId);

    IList<VariableGroup> GetVariableGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> groupIds,
      VariableGroupActionFilter actionFilter = VariableGroupActionFilter.None);

    IList<VariableGroup> GetVariableGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      string groupName,
      VariableGroupActionFilter actionFilter = VariableGroupActionFilter.None,
      int? continuationToken = null,
      int top = 0,
      VariableGroupQueryOrder queryOrder = VariableGroupQueryOrder.IdDescending);

    void DeleteVariableGroup(IVssRequestContext requestContext, Guid projectId, int groupId);

    VariableGroup AddVariableGroupForCollection(
      IVssRequestContext requestContext,
      VariableGroupParameters addVariableGroupParameters);

    VariableGroup UpdateVariableGroupForCollection(
      IVssRequestContext requestContext,
      int groupId,
      VariableGroupParameters updateVariableGroupParameters);

    void DeleteVariableGroupForCollection(
      IVssRequestContext requestContext,
      int groupId,
      IList<Guid> projectIds);

    void ShareVariableGroupForCollection(
      IVssRequestContext requestContext,
      int groupId,
      IList<VariableGroupProjectReference> variableGroupProjectReferences);
  }
}
