// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseEnvironmentStepSqlComponent14
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Necessary to handle AT/DT mismatch")]
  public class ReleaseEnvironmentStepSqlComponent14 : ReleaseEnvironmentStepSqlComponent13
  {
    public override ReleaseEnvironmentStep HandleEnvironmentDeployJobStarted(
      Guid projectId,
      int releaseId,
      int releaseStepId,
      int releaseDeployPhaseId)
    {
      this.PrepareStoredProcedure("Release.prc_EnvironmentDeployJobStarted", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseStepId), releaseStepId);
      this.BindInt(nameof (releaseDeployPhaseId), releaseDeployPhaseId);
      return this.GetReleaseEnvironmentStepsObject().FirstOrDefault<ReleaseEnvironmentStep>();
    }
  }
}
