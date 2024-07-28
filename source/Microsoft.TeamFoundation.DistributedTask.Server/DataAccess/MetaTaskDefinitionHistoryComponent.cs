// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.MetaTaskDefinitionHistoryComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class MetaTaskDefinitionHistoryComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<MetaTaskDefinitionHistoryComponent>(1, true),
      (IComponentCreator) new ComponentCreator<MetaTaskDefinitionHistoryComponent2>(2),
      (IComponentCreator) new ComponentCreator<MetaTaskDefinitionHistoryComponent3>(3),
      (IComponentCreator) new ComponentCreator<MetaTaskDefinitionHistoryComponent4>(4)
    }, "DistributedTaskMetaTaskDefinitionHistory", "DistributedTask");

    public MetaTaskDefinitionHistoryComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual void AddTaskGroupRevision(
      Guid projectId,
      Guid definitionId,
      Guid changedById,
      int revision,
      int majorVersion,
      int fileId,
      string comment,
      AuditAction changeType)
    {
    }

    public virtual void DeleteTaskGroupHistory(Guid projectId, Guid taskGroupId)
    {
    }

    public virtual List<MetaTaskDefinitionRevisionData> GetTaskGroupHistory(
      Guid projectId,
      Guid definitionId)
    {
      return new List<MetaTaskDefinitionRevisionData>();
    }

    public virtual MetaTaskDefinitionRevisionData GetTaskGroupRevision(
      Guid projectId,
      Guid definitionId,
      int definitionRevision)
    {
      return (MetaTaskDefinitionRevisionData) null;
    }
  }
}
