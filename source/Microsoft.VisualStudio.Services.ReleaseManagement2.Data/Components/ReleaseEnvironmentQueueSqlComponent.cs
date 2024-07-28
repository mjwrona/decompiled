// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseEnvironmentQueueSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ReleaseEnvironmentQueueSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[9]
    {
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentQueueSqlComponent>(1),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentQueueSqlComponent2>(2),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentQueueSqlComponent3>(3),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentQueueSqlComponent4>(4),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentQueueSqlComponent5>(5),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentQueueSqlComponent6>(6),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentQueueSqlComponent7>(7),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentQueueSqlComponent8>(8),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentQueueSqlComponent9>(9)
    }, "ReleaseManagementReleaseEnvironmentQueue", "ReleaseManagement");

    public virtual QueuingPolicyResult GetReleaseEnvironmentsTorunAfterEnforcingQueuingPolicy(
      Guid projectId,
      int releaseDefinitionId,
      int definitionEnvironmentId,
      int releaseId,
      int releaseEnvironmentId,
      int maxConcurrent,
      int maxQueueDepth,
      Guid changedBy)
    {
      return new QueuingPolicyResult();
    }

    public virtual void RemoveEnvironmentFromQueue(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId)
    {
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public virtual IEnumerable<ReleaseEnvironmentQueueData> GetUnhealthyReleaseEnvironments(
      Guid projectId,
      int releaseDefinitionId,
      int definitionEnvironmentId,
      int daysToCheck,
      int skipHours = 6)
    {
      return (IEnumerable<ReleaseEnvironmentQueueData>) new List<ReleaseEnvironmentQueueData>();
    }

    public virtual QueuingPolicyResult GetReleaseEnvironmentsTorunAfterCancelingScheduledReleases(
      Guid projectId,
      int releaseDefinitionId,
      int definitionEnvironmentId,
      int releaseId,
      int releaseEnvironmentId,
      int maxConcurrent,
      int maxQueueDepth,
      Guid changedBy)
    {
      return new QueuingPolicyResult();
    }
  }
}
