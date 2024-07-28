// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseEnvironmentStepSqlComponent2
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ReleaseEnvironmentStepSqlComponent2 : ReleaseEnvironmentStepSqlComponent
  {
    public override IEnumerable<ReleaseEnvironmentStep> GetApprovalHistory(
      Guid projectId,
      IEnumerable<int> approvalStepId)
    {
      if (approvalStepId == null)
        throw new ArgumentNullException(nameof (approvalStepId));
      List<ReleaseEnvironmentStep> source1 = new List<ReleaseEnvironmentStep>();
      foreach (int releaseEnvironmentStepId in approvalStepId)
        source1.Add(this.GetReleaseEnvironmentStep(projectId, releaseEnvironmentStepId, false).SingleOrDefault<ReleaseEnvironmentStep>());
      IEnumerable<int> releaseIds = source1.Select<ReleaseEnvironmentStep, int>((Func<ReleaseEnvironmentStep, int>) (s => s.ReleaseId)).Distinct<int>();
      IEnumerable<int> ints = source1.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s.ReleaseEnvironmentId != 0)).Select<ReleaseEnvironmentStep, int>((Func<ReleaseEnvironmentStep, int>) (s => s.GroupStepId)).Distinct<int>();
      IEnumerable<ReleaseEnvironmentStep> source2 = this.ListReleaseApprovalSteps(projectId, releaseIds, new ReleaseEnvironmentStepStatus?(), new Guid?());
      List<ReleaseEnvironmentStep> approvalHistory = new List<ReleaseEnvironmentStep>();
      foreach (int num in ints)
      {
        int reassignStepId = num;
        approvalHistory.AddRange(source2.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s.GroupStepId == reassignStepId && s.Status == ReleaseEnvironmentStepStatus.Reassigned)));
      }
      return (IEnumerable<ReleaseEnvironmentStep>) approvalHistory;
    }
  }
}
