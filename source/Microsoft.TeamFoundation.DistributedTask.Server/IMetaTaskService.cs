// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IMetaTaskService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DefaultServiceImplementation(typeof (MetaTaskService))]
  internal interface IMetaTaskService : IVssFrameworkService
  {
    TaskGroup AddTaskGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskGroupCreateParameter taskGroupCreateParameter);

    bool SoftDeleteTaskGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid taskGroupId,
      string comment);

    bool UndeleteTaskGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid taskGroupId,
      string comment);

    List<TaskGroup> GetTaskGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? taskGroupId = null,
      bool? expanded = false,
      Guid? taskIdFilter = null,
      bool? deleted = false,
      DateTime? continuationToken = null,
      int top = 0,
      TaskGroupQueryOrder queryOrder = TaskGroupQueryOrder.CreatedOnAscending);

    TaskGroup GetTaskGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid taskGroupId,
      string versionSpec,
      bool? expanded = false);

    TaskGroup UpdateTaskGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskGroupUpdateParameter taskGroupUpdateParameter);

    IEnumerable<TaskGroup> PublishTaskGroup(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      Guid taskGroupId,
      PublishTaskGroupMetadata taskGroupMetadata);

    IEnumerable<TaskGroup> PublishPreviewTaskGroup(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      Guid taskGroupId,
      TaskGroupPublishPreviewParameter patchTaskGroup);

    bool DeleteRevisionHistory(IVssRequestContext requestContext, Guid projectId, Guid taskGroupId);

    Stream GetRevision(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid metaTaskDefinitionId,
      int revision);

    List<TaskGroupRevision> GetRevisionHistory(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid taskGroupId);

    int SaveRevision(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskGroup taskGroup,
      string comment,
      AuditAction changeType);
  }
}
