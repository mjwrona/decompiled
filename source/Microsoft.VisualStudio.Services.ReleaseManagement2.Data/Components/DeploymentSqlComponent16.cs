// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DeploymentSqlComponent16
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism for AT/DT")]
  public class DeploymentSqlComponent16 : DeploymentSqlComponent15
  {
    public override DeploymentGate UpdateGreenlightingSucceedingSince(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int stepId,
      Guid runPlanId,
      DateTime? succeedingSince)
    {
      this.PrepareStoredProcedure("Release.prc_UpdateGreenlightingSucceedingSince", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt(nameof (stepId), stepId);
      this.BindGuid(nameof (runPlanId), runPlanId);
      this.BindNullableDateTime(nameof (succeedingSince), succeedingSince);
      return this.GetDeploymentGateObject();
    }

    public override DeploymentGate HandleGreenlightingStabilizationCompletion(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int stepId,
      Guid runPlanId)
    {
      this.PrepareStoredProcedure("Release.prc_HandleGreenlightingStabilizationCompletion", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt(nameof (stepId), stepId);
      this.BindGuid(nameof (runPlanId), runPlanId);
      return this.GetDeploymentGateObject();
    }
  }
}
