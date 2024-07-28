// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseEnvironmentStepSqlComponent5
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "This is our versioning mechanism")]
  public class ReleaseEnvironmentStepSqlComponent5 : ReleaseEnvironmentStepSqlComponent4
  {
    public override ReleaseEnvironmentStep UpdateReleaseEnvironmentStep(
      Guid projectId,
      ReleaseEnvironmentStep environmentStep)
    {
      if (environmentStep == null)
        throw new ArgumentNullException(nameof (environmentStep));
      this.PrepareStoredProcedure("Release.prc_ReleaseEnvironmentStep_Update", projectId);
      this.BindInt("releaseId", environmentStep.ReleaseId);
      this.BindToReleaseEnvironmentStepTable("releaseEnvironmentStep", environmentStep);
      return this.GetReleaseEnvironmentStepsObject().FirstOrDefault<ReleaseEnvironmentStep>();
    }

    public override IEnumerable<ReleaseEnvironmentStep> UpdatePendingReleaseEnvironmentStep(
      Guid projectId,
      ReleaseEnvironmentStep environmentStep)
    {
      if (environmentStep == null)
        throw new ArgumentNullException(nameof (environmentStep));
      ReleaseEnvironmentStep releaseEnvironmentStep = new ReleaseEnvironmentStep();
      int groupStepId = environmentStep.GroupStepId;
      int newParentStepId = groupStepId == 0 ? environmentStep.Id : environmentStep.GroupStepId;
      if (environmentStep.Status == ReleaseEnvironmentStepStatus.Reassigned)
      {
        releaseEnvironmentStep = this.GetReleaseEnvironmentStep(projectId, environmentStep.Id, false).SingleOrDefault<ReleaseEnvironmentStep>();
        releaseEnvironmentStep.ApproverId = environmentStep.ApproverId;
        releaseEnvironmentStep.GroupStepId = newParentStepId;
        environmentStep.GroupStepId = newParentStepId;
      }
      this.PrepareStoredProcedure("Release.prc_ReleaseEnvironmentStep_UpdatePending", projectId);
      this.BindInt("releaseId", environmentStep.ReleaseId);
      this.BindToReleaseEnvironmentStepTable("releaseEnvironmentStep", environmentStep);
      IEnumerable<ReleaseEnvironmentStep> environmentStepsObject = this.GetReleaseEnvironmentStepsObject();
      if (groupStepId == 0 && environmentStep.Status != ReleaseEnvironmentStepStatus.Reassigned)
        return environmentStepsObject;
      if (environmentStep.Status == ReleaseEnvironmentStepStatus.Reassigned)
      {
        Guid projectId1 = projectId;
        List<ReleaseEnvironmentStep> environmentStepsToAdd = new List<ReleaseEnvironmentStep>();
        environmentStepsToAdd.Add(releaseEnvironmentStep);
        Guid empty = Guid.Empty;
        this.AddReleaseEnvironmentSteps(projectId1, (IEnumerable<ReleaseEnvironmentStep>) environmentStepsToAdd, false, empty);
      }
      Guid projectId2 = projectId;
      List<int> releaseIds = new List<int>();
      releaseIds.Add(environmentStep.ReleaseId);
      ReleaseEnvironmentStepStatus? status = new ReleaseEnvironmentStepStatus?();
      Guid? approverId = new Guid?();
      return this.ListReleaseApprovalSteps(projectId2, (IEnumerable<int>) releaseIds, status, approverId).Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.GroupStepId == newParentStepId));
    }
  }
}
