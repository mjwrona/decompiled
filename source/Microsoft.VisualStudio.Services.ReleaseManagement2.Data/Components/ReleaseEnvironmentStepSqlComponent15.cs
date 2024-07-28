// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseEnvironmentStepSqlComponent15
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Necessary to handle AT/DT mismatch")]
  public class ReleaseEnvironmentStepSqlComponent15 : ReleaseEnvironmentStepSqlComponent14
  {
    public override IEnumerable<ReleaseEnvironmentStep> UpdateReleaseEnvironmentSteps(
      Guid projectId,
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps)
    {
      ReleaseEnvironmentStep releaseEnvironmentStep = releaseEnvironmentSteps != null ? releaseEnvironmentSteps.FirstOrDefault<ReleaseEnvironmentStep>() : throw new ArgumentNullException(nameof (releaseEnvironmentSteps));
      if (releaseEnvironmentSteps.Count<ReleaseEnvironmentStep>() > 1)
      {
        string approvalsCompleted = ReleaseAuditConstants.ApprovalsCompleted;
        Guid projectId1 = projectId;
        Dictionary<string, Dictionary<string, string>> commonAuditCollectionItemsData = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        int num = releaseEnvironmentStep.ReleaseId;
        dictionary.Add("ReleaseId", num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        dictionary.Add("ActualApproverId", releaseEnvironmentStep.ActualApproverId.ToString());
        dictionary.Add("ApproverId", releaseEnvironmentStep.ApproverId.ToString());
        dictionary.Add("ApproverComment", releaseEnvironmentStep.ApproverComment);
        dictionary.Add("IsAutomated", releaseEnvironmentStep.IsAutomated.ToString());
        num = releaseEnvironmentStep.Rank;
        dictionary.Add("Rank", num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        dictionary.Add("Status", ((byte) releaseEnvironmentStep.Status).ToString((IFormatProvider) CultureInfo.InvariantCulture));
        num = (int) releaseEnvironmentStep.StepType;
        dictionary.Add("StepType", num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        num = releaseEnvironmentStep.TrialNumber;
        dictionary.Add("TrialNumber", num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        dictionary.Add("Logs", releaseEnvironmentStep.Logs);
        Guid? runPlanId = releaseEnvironmentStep.RunPlanId;
        ref Guid? local = ref runPlanId;
        dictionary.Add("RunPlanId", local.HasValue ? local.GetValueOrDefault().ToString() : (string) null);
        num = releaseEnvironmentStep.GroupStepId;
        dictionary.Add("ReassignedFromStepId", num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        dictionary.Add("BuildId", "0");
        commonAuditCollectionItemsData.Add(nameof (releaseEnvironmentSteps), dictionary);
        this.PrepareForAuditingAction(approvalsCompleted, projectId: projectId1, commonAuditCollectionItemsData: commonAuditCollectionItemsData);
      }
      else if (releaseEnvironmentSteps.Count<ReleaseEnvironmentStep>() == 1 && !releaseEnvironmentStep.IsAutomated)
        this.PrepareForAuditingAction(ReleaseAuditConstants.ApprovalCompleted, projectId: projectId);
      this.PrepareStoredProcedure("Release.prc_UpdateReleaseEnvironmentStep", projectId);
      this.BindToReleaseEnvironmentStepsTable(nameof (releaseEnvironmentSteps), releaseEnvironmentSteps);
      return this.GetReleaseEnvironmentStepsObject();
    }

    public override IEnumerable<int> GetAllPendingApprovalIds(Guid projectId, int days)
    {
      this.PrepareStoredProcedure("Release.prc_GetAllPendingApprovalIds", projectId);
      this.BindInt(nameof (days), days);
      List<int> pendingApprovalIds = new List<int>();
      using (SqlDataReader sqlDataReader = this.ExecuteReader())
      {
        while (sqlDataReader.Read())
          pendingApprovalIds.Add(sqlDataReader.GetInt32(0));
        return (IEnumerable<int>) pendingApprovalIds;
      }
    }

    public override void BindToReleaseEnvironmentStepTable(
      string parameterName,
      ReleaseEnvironmentStep releaseEnvironmentStep)
    {
      this.BindReleaseEnvironmentStepTable7(parameterName, (IEnumerable<ReleaseEnvironmentStep>) new List<ReleaseEnvironmentStep>()
      {
        releaseEnvironmentStep
      });
    }

    public override void BindToReleaseEnvironmentStepsTable(
      string parameterName,
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps)
    {
      this.BindReleaseEnvironmentStepTable7(parameterName, releaseEnvironmentSteps);
    }
  }
}
