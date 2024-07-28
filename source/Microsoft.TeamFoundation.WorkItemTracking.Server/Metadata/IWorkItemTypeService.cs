// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.IWorkItemTypeService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  [DefaultServiceImplementation(typeof (WorkItemTypeService))]
  public interface IWorkItemTypeService : IVssFrameworkService
  {
    void RemoveProjectWorkitemTypesFromCache(
      IVssRequestContext requestContext,
      IEnumerable<ProjectInfo> projectInfos);

    IReadOnlyCollection<WorkItemType> GetWorkItemTypes(
      IVssRequestContext requestContext,
      Guid projectId);

    IReadOnlyCollection<WorkItemType> GetWorkItemTypes(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectId);

    WorkItemType GetWorkItemTypeByReferenceName(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemTypeName);

    bool TryGetWorkItemTypeByReferenceName(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemTypeName,
      out WorkItemType workItemType);

    bool TryGetWorkItemTypeByName(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemTypeName,
      out WorkItemType workItemType);

    WorkItemType GetWorkItemTypeById(IVssRequestContext requestContext, Guid projectId, int typeId);

    bool TryGetWorkItemTypeById(
      IVssRequestContext requestContext,
      Guid projectId,
      int typeId,
      out WorkItemType workItemType);

    WorkItemType RenameWorkItemType(
      IVssRequestContext requestContext,
      Guid projectId,
      string oldWorkItemTypeName,
      string newWorkItemTypeName);

    IReadOnlyCollection<ProcessChangedRecord> GetProcessesForChangedWorkItemTypes(
      IVssRequestContext requestContext,
      DateTime sinceWatermark);

    IReadOnlyCollection<ProjectGuidChangedRecord> GetProjectsForChangedWorkItemTypes(
      IVssRequestContext requestContext,
      long sinceWatermark);

    bool HasAnyWorkItemsOfTypeForProcess(
      IVssRequestContext requestContext,
      Guid processType,
      string workItemTypeName);

    bool HasAnyWorkItemsOfTypeForProject(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemTypeName);

    IReadOnlyCollection<ProcessChangedRecord> GetProcessesForChangedWorkItemTypeBehaviors(
      IVssRequestContext requestContext,
      DateTime dateTime);
  }
}
