// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseEnvironmentStepSqlComponent6
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "This is our versioning mechanism")]
  public class ReleaseEnvironmentStepSqlComponent6 : ReleaseEnvironmentStepSqlComponent5
  {
    public override IEnumerable<ReleaseEnvironmentStep> GetReleaseEnvironmentApprovalSteps(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int rank,
      int trialNumber)
    {
      List<int> source = new List<int>() { releaseId };
      this.PrepareStoredProcedure("Release.prc_GetApprovals", projectId);
      this.BindTable("releaseIds", "dbo.typ_Int32Table", source.Select<int, SqlDataRecord>(new Func<int, SqlDataRecord>(((ReleaseEnvironmentStepSqlComponent4) this).ConvertToSqlDataRecord)));
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      return this.GetReleaseEnvironmentStepsObject().Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s.Rank == rank && s.TrialNumber == trialNumber));
    }

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
      return this.GetReleaseEnvironmentStepsObject();
    }

    public override IEnumerable<ReleaseEnvironmentStep> GetApprovalHistory(
      Guid projectId,
      IEnumerable<int> approvalStepId)
    {
      this.PrepareStoredProcedure("Release.prc_GetApprovalHistory", projectId);
      this.BindTable("approvalStepIds", "dbo.typ_Int32Table", approvalStepId.Select<int, SqlDataRecord>(new Func<int, SqlDataRecord>(((ReleaseEnvironmentStepSqlComponent4) this).ConvertToSqlDataRecord)));
      return this.GetReleaseEnvironmentStepsObject();
    }

    public override IEnumerable<ReleaseEnvironmentStep> UpdatePendingReleaseEnvironmentStep(
      Guid projectId,
      ReleaseEnvironmentStep environmentStep)
    {
      if (environmentStep == null)
        throw new ArgumentNullException(nameof (environmentStep));
      this.PrepareStoredProcedure("Release.prc_UpdateReleaseEnvironmentStep", projectId);
      this.BindInt("releaseId", environmentStep.ReleaseId);
      this.BindToReleaseEnvironmentStepTable("releaseEnvironmentStep", environmentStep);
      return this.GetReleaseEnvironmentStepsObject();
    }

    public override void BindToReleaseEnvironmentStepsTable(
      string parameterName,
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps)
    {
      this.BindReleaseEnvironmentStepTable5(parameterName, releaseEnvironmentSteps);
    }

    public override void BindToReleaseEnvironmentStepTable(
      string parameterName,
      ReleaseEnvironmentStep releaseEnvironmentStep)
    {
      this.BindReleaseEnvironmentStepTable5(parameterName, (IEnumerable<ReleaseEnvironmentStep>) new List<ReleaseEnvironmentStep>()
      {
        releaseEnvironmentStep
      });
    }
  }
}
