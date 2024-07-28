// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.DefinitionEnvironmentStepDataExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class DefinitionEnvironmentStepDataExtensions
  {
    public static DefinitionEnvironmentStep ToDefinitionEnvironmentStep(
      this DefinitionEnvironmentStepData stepData)
    {
      if (stepData == null)
        throw new ArgumentNullException(nameof (stepData));
      return new DefinitionEnvironmentStep()
      {
        Rank = stepData.Rank,
        IsAutomated = stepData.IsAutomated,
        IsNotificationOn = stepData.IsNotificationOn,
        ApproverId = stepData.ApproverId,
        StepType = stepData.StepType,
        DefinitionEnvironmentId = stepData.DefinitionEnvironmentId
      };
    }

    public static DefinitionEnvironmentStepData ToDefinitionEnvironmentStepData(
      this DefinitionEnvironmentStep step)
    {
      if (step == null)
        throw new ArgumentNullException(nameof (step));
      return new DefinitionEnvironmentStepData()
      {
        Rank = step.Rank,
        IsAutomated = step.IsAutomated,
        IsNotificationOn = step.IsNotificationOn,
        ApproverId = step.ApproverId,
        StepType = step.StepType,
        DefinitionEnvironmentId = step.DefinitionEnvironmentId
      };
    }

    public static ReleaseDefinitionApprovalStep ToReleaseDefinitionApprovalStep(
      this DefinitionEnvironmentStepData stepData)
    {
      if (stepData == null)
        throw new ArgumentNullException(nameof (stepData));
      return new ReleaseDefinitionApprovalStep()
      {
        Approver = new IdentityRef()
        {
          Id = stepData.ApproverId.ToString()
        },
        IsAutomated = stepData.IsAutomated,
        IsNotificationOn = stepData.IsNotificationOn,
        Rank = stepData.Rank
      };
    }
  }
}
