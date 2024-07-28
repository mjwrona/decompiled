// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DeploymentSqlComponent10
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class DeploymentSqlComponent10 : DeploymentSqlComponent9
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public override IEnumerable<Deployment> ListDeployments(
      Guid projectId,
      int releaseDefinitionId,
      int releaseDefinitionEnvironmentId,
      int releaseId,
      DeploymentStatus deploymentStatus,
      DeploymentOperationStatus operationStatus,
      IList<Guid> createdByIds,
      bool latestAttemptsOnly,
      ReleaseQueryOrder queryOrder,
      int continuationToken,
      bool isDeleted,
      DateTime? minModifiedTime,
      DateTime? maxModifiedTime,
      IList<Guid> createdForIds,
      int maxDeployments,
      string branchName,
      DateTime? minStartedTime,
      DateTime? maxStartedTime,
      string artifactTypeId = "",
      string sourceId = "",
      ReleaseReason releaseReason = ReleaseReason.None)
    {
      this.PrepareStoredProcedure("Release.prc_QueryDeployments", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindInt(nameof (releaseDefinitionEnvironmentId), releaseDefinitionEnvironmentId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindNullableInt(nameof (deploymentStatus), new int?((int) deploymentStatus));
      this.BindNullableInt(nameof (operationStatus), new int?((int) operationStatus));
      this.BindGuidTable("createdByIdsTable", (IEnumerable<Guid>) createdByIds);
      this.BindNullableBoolean(nameof (latestAttemptsOnly), new bool?(latestAttemptsOnly));
      this.BindByte(nameof (queryOrder), (byte) queryOrder);
      this.BindNullableInt("deploymentContinuationToken", new int?(continuationToken));
      this.BindMaxDeployments(maxDeployments);
      this.BindNullableBoolean(nameof (isDeleted), new bool?(isDeleted));
      this.BindModifiedTime(minModifiedTime, maxModifiedTime);
      this.BindCreatedForIds(createdForIds);
      this.BindString(nameof (branchName), branchName, 256, true, SqlDbType.NVarChar);
      return this.GetDeployments();
    }
  }
}
