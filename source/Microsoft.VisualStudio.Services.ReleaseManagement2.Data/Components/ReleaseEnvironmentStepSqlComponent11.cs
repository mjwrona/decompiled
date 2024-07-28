// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseEnvironmentStepSqlComponent11
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Necessary to handle AT/DT mismatch")]
  public class ReleaseEnvironmentStepSqlComponent11 : ReleaseEnvironmentStepSqlComponent10
  {
    public override IEnumerable<ReleaseEnvironmentStep> AddReleaseEnvironmentSteps(
      Guid projectId,
      IEnumerable<ReleaseEnvironmentStep> environmentStepsToAdd,
      bool handleParallelApprovers,
      Guid changedBy)
    {
      if (environmentStepsToAdd == null)
        throw new ArgumentNullException(nameof (environmentStepsToAdd));
      if (!environmentStepsToAdd.Any<ReleaseEnvironmentStep>())
        return (IEnumerable<ReleaseEnvironmentStep>) new List<ReleaseEnvironmentStep>();
      this.PrepareStoredProcedure("Release.prc_AddReleaseEnvironmentStep", projectId);
      this.BindInt("releaseId", environmentStepsToAdd.First<ReleaseEnvironmentStep>().ReleaseId);
      this.BindToReleaseEnvironmentStepsTable("releaseEnvironmentStep", environmentStepsToAdd);
      this.BindBoolean(nameof (handleParallelApprovers), handleParallelApprovers);
      this.BindGuid(nameof (changedBy), changedBy);
      return this.GetReleaseEnvironmentStepsObject();
    }
  }
}
