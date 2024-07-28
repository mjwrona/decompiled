// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseEnvironmentStepSqlComponent13
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Necessary to handle AT/DT mismatch")]
  public class ReleaseEnvironmentStepSqlComponent13 : ReleaseEnvironmentStepSqlComponent12
  {
    public override void HandleEnvironmentPipelineStatusUpdate(
      Guid projectId,
      int releaseId,
      int releaseStepId,
      DeploymentOperationStatus operationStatus)
    {
      if (operationStatus != DeploymentOperationStatus.QueuedForAgent)
        return;
      this.PrepareStoredProcedure("Release.prc_EnvironmentPipelineAssigned", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseStepId), releaseStepId);
      this.ExecuteScalar();
    }
  }
}
