// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.ReleaseEnvironmentStepConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class ReleaseEnvironmentStepConverter
  {
    private static readonly IDictionary<EnvironmentStepType, ApprovalType> StepToApprovalTypeMap = (IDictionary<EnvironmentStepType, ApprovalType>) new Dictionary<EnvironmentStepType, ApprovalType>()
    {
      {
        EnvironmentStepType.PreDeploy,
        ApprovalType.PreDeploy
      },
      {
        EnvironmentStepType.PostDeploy,
        ApprovalType.PostDeploy
      },
      {
        EnvironmentStepType.All,
        ApprovalType.Undefined
      }
    };
    private static readonly IDictionary<ApprovalType, EnvironmentStepType> ApprovalTypeToStepTypeMap = (IDictionary<ApprovalType, EnvironmentStepType>) new Dictionary<ApprovalType, EnvironmentStepType>()
    {
      {
        ApprovalType.PreDeploy,
        EnvironmentStepType.PreDeploy
      },
      {
        ApprovalType.PostDeploy,
        EnvironmentStepType.PostDeploy
      },
      {
        ApprovalType.Undefined,
        EnvironmentStepType.All
      },
      {
        ApprovalType.All,
        EnvironmentStepType.All
      }
    };

    public static EnvironmentStepType ToEnvironmentStepType(this ApprovalType approvalType) => ReleaseEnvironmentStepConverter.ApprovalTypeToStepTypeMap[approvalType];

    public static ApprovalType ToWebApiStepType(this EnvironmentStepType stepType) => ReleaseEnvironmentStepConverter.StepToApprovalTypeMap[stepType];

    public static ReleaseApproval ToApproval(this ReleaseEnvironmentStep step) => step.ToApproval((ReleaseDefinitionEnvironmentsSnapshot) null);

    public static ReleaseApproval ToApproval(
      this ReleaseEnvironmentStep step,
      ReleaseDefinitionEnvironmentsSnapshot definitionSnapshot)
    {
      if (step == null)
        throw new ArgumentNullException(nameof (step));
      if (!ReleaseEnvironmentStepConverter.StepToApprovalTypeMap.ContainsKey(step.StepType))
        throw new ReleaseManagementObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ApprovalNotFoundException, (object) step.Id));
      bool flag = !step.IsAutomated;
      if (definitionSnapshot != null)
      {
        DefinitionEnvironmentData definitionEnvironmentData = definitionSnapshot.Environments.FirstOrDefault<DefinitionEnvironmentData>((Func<DefinitionEnvironmentData, bool>) (env => env.Id == step.DefinitionEnvironmentId));
        if (definitionEnvironmentData != null)
        {
          DefinitionEnvironmentStepData environmentStepData = definitionEnvironmentData.Steps.FirstOrDefault<DefinitionEnvironmentStepData>((Func<DefinitionEnvironmentStepData, bool>) (s => s.DefinitionEnvironmentId == step.DefinitionEnvironmentId && s.Rank == step.Rank && s.StepType == step.StepType));
          if (environmentStepData != null)
            flag = environmentStepData.IsNotificationOn;
        }
      }
      return new ReleaseApproval()
      {
        Id = step.Id,
        Comments = step.ApproverComment,
        CreatedOn = step.CreatedOn,
        ModifiedOn = step.ModifiedOn,
        IsAutomated = step.IsAutomated,
        IsNotificationOn = flag,
        TrialNumber = step.TrialNumber,
        Attempt = step.TrialNumber,
        Approver = new IdentityRef()
        {
          Id = step.ApproverId.ToString()
        },
        ApprovedBy = new IdentityRef()
        {
          Id = step.ActualApproverId.ToString()
        },
        Rank = step.Rank,
        Revision = 1,
        ReleaseReference = new ReleaseShallowReference()
        {
          Id = step.ReleaseId,
          Name = step.ReleaseName
        },
        ReleaseDefinitionReference = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference()
        {
          Id = step.ReleaseDefinitionId,
          Name = step.ReleaseDefinitionName,
          Path = step.ReleaseDefinitionPath
        },
        ReleaseEnvironmentReference = new ReleaseEnvironmentShallowReference()
        {
          Id = step.ReleaseEnvironmentId,
          Name = step.ReleaseEnvironmentName
        },
        Status = step.Status.ToWebApi(),
        ApprovalType = ReleaseEnvironmentStepConverter.StepToApprovalTypeMap[step.StepType]
      };
    }

    public static ReleaseApproval ToApproval(
      this ReleaseEnvironmentStep step,
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      if (step == null)
        throw new ArgumentNullException(nameof (step));
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      ReleaseApproval approval = step.ToApproval(release.DefinitionSnapshot);
      if (requestContext != null)
      {
        approval.Approver.DisplayName = IdentityExtensions.GetIdentityDisplayName(requestContext, approval.Approver.Id);
        approval.ApprovedBy.DisplayName = IdentityExtensions.GetIdentityDisplayName(requestContext, approval.ApprovedBy.Id);
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment = release.GetEnvironment(step.ReleaseEnvironmentId);
        approval.ReleaseReference.Url = RestUrlHelper.GetRestUrlForRelease(requestContext, projectId, release.Id);
        approval.ReleaseDefinitionReference.Id = release.ReleaseDefinitionId;
        approval.ReleaseDefinitionReference.Url = RestUrlHelper.GetRestUrlForReleaseDefinition(requestContext, projectId, release.ReleaseDefinitionId);
        approval.ReleaseEnvironmentReference.Id = environment.DefinitionEnvironmentId;
        approval.ReleaseEnvironmentReference.Name = environment.Name;
      }
      return approval;
    }

    public static ReleaseApproval ToWebApiApprovalWithHistory(
      this ReleaseEnvironmentStep step,
      IEnumerable<ReleaseEnvironmentStep> approvalHistorySteps)
    {
      return step.ToWebApiApprovalWithHistory(approvalHistorySteps, (ReleaseDefinitionEnvironmentsSnapshot) null);
    }

    public static ReleaseApproval ToWebApiApprovalWithHistory(
      this ReleaseEnvironmentStep step,
      IEnumerable<ReleaseEnvironmentStep> approvalHistorySteps,
      ReleaseDefinitionEnvironmentsSnapshot definitionSnapshot)
    {
      ReleaseApproval approvalWithHistory = step != null ? step.ToApproval(definitionSnapshot) : throw new ArgumentNullException(nameof (step));
      if (approvalHistorySteps == null || !approvalHistorySteps.Any<ReleaseEnvironmentStep>())
        return approvalWithHistory;
      approvalWithHistory.History = new List<ReleaseApprovalHistory>();
      int num = 1;
      foreach (ReleaseEnvironmentStep releaseEnvironmentStep in (IEnumerable<ReleaseEnvironmentStep>) approvalHistorySteps.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s.GroupStepId == step.GroupStepId && s.Id != step.Id)).OrderBy<ReleaseEnvironmentStep, int>((Func<ReleaseEnvironmentStep, int>) (s => s.Id)))
      {
        ReleaseApprovalHistory releaseApprovalHistory = new ReleaseApprovalHistory()
        {
          Revision = num,
          Approver = new IdentityRef()
          {
            Id = releaseEnvironmentStep.ApproverId.ToString()
          },
          ChangedBy = new IdentityRef()
          {
            Id = releaseEnvironmentStep.ActualApproverId.ToString()
          },
          Comments = releaseEnvironmentStep.ApproverComment,
          CreatedOn = releaseEnvironmentStep.CreatedOn,
          ModifiedOn = releaseEnvironmentStep.ModifiedOn
        };
        ++num;
        approvalWithHistory.History.Add(releaseApprovalHistory);
      }
      approvalWithHistory.Revision = num;
      return approvalWithHistory;
    }
  }
}
