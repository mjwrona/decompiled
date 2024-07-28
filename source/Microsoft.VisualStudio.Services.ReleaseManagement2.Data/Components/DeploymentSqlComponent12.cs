// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DeploymentSqlComponent12
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class DeploymentSqlComponent12 : DeploymentSqlComponent11
  {
    public override IEnumerable<Deployment> GetLatestDeploymentsByReleaseDefinitions(
      Guid projectId,
      int releaseDefinitionsCount,
      IEnumerable<int> releaseDefinitionIdsToExclude)
    {
      this.PrepareStoredProcedure("Release.prc_QueryLatestDeploymentsGroupedByDefinitions", projectId);
      this.BindInt(nameof (releaseDefinitionsCount), releaseDefinitionsCount);
      this.BindInt32Table("rdIdsToExclude", releaseDefinitionIdsToExclude);
      return this.GetDeployments();
    }

    public override IEnumerable<Deployment> GetDeploymentsCreatedByAnIdentity(
      Guid projectId,
      Guid createdById,
      DeploymentStatus deploymentStatus,
      DeploymentOperationStatus operationStatus,
      int maxDeployments,
      DateTime minQueuedTime,
      DateTime maxQueuedTime)
    {
      Guid projectId1 = projectId;
      int num = (int) deploymentStatus;
      int operationStatus1 = (int) operationStatus;
      List<Guid> createdByIds = new List<Guid>();
      createdByIds.Add(createdById);
      DateTime? minModifiedTime = new DateTime?(minQueuedTime);
      DateTime? maxModifiedTime = new DateTime?(maxQueuedTime);
      int maxDeployments1 = maxDeployments;
      DateTime? minStartedTime = new DateTime?();
      DateTime? maxStartedTime = new DateTime?();
      return this.ListDeployments(projectId1, 0, 0, 0, (DeploymentStatus) num, (DeploymentOperationStatus) operationStatus1, (IList<Guid>) createdByIds, false, ReleaseQueryOrder.IdDescending, 0, false, minModifiedTime, maxModifiedTime, (IList<Guid>) null, maxDeployments1, (string) null, minStartedTime, maxStartedTime, "", "", ReleaseReason.None);
    }
  }
}
