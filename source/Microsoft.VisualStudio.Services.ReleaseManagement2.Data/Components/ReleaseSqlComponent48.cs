// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent48
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism for AT/DT")]
  public class ReleaseSqlComponent48 : ReleaseSqlComponent47
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public override Release UpdateEnvironmentAndDeploymentStatus(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int attempt,
      Guid changedBy,
      ReleaseEnvironmentStatus environmentStatus,
      DeploymentStatus deploymentStatus,
      ReleaseEnvironmentStatusChangeDetails changeDetails,
      DeploymentOperationStatus operationStatus = DeploymentOperationStatus.Undefined)
    {
      this.PrepareForAuditingAction(ReleaseAuditConstants.DeploymentCompleted, projectId: projectId, excludeSqlParameters: true);
      this.PrepareStoredProcedure("Release.prc_OnDeploymentCompleted", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt(nameof (attempt), attempt);
      this.BindByte(nameof (environmentStatus), (byte) environmentStatus);
      this.BindByte(nameof (deploymentStatus), (byte) deploymentStatus);
      this.BindInt(nameof (operationStatus), (int) operationStatus);
      this.BindGuid(nameof (changedBy), changedBy);
      this.BindChangeDetails(changeDetails);
      return this.GetReleaseObject(projectId);
    }
  }
}
