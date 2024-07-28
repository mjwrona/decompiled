// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.Processor.Builders.ReleaseEnvironmentStepBuilder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.Processor.Builders
{
  public static class ReleaseEnvironmentStepBuilder
  {
    public static ReleaseEnvironmentStep Build(
      Release release,
      DefinitionEnvironmentStep definitionEnvironmentStep,
      int definitionEnvironmentRank,
      int trialNumber)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (definitionEnvironmentStep == null)
        throw new ArgumentNullException(nameof (definitionEnvironmentStep));
      return new ReleaseEnvironmentStep()
      {
        IsAutomated = definitionEnvironmentStep.IsAutomated,
        TrialNumber = trialNumber,
        ApproverId = definitionEnvironmentStep.ApproverId,
        Status = ReleaseEnvironmentStepStatus.Pending,
        StepType = definitionEnvironmentStep.StepType,
        Rank = definitionEnvironmentStep.Rank,
        DefinitionEnvironmentId = definitionEnvironmentStep.DefinitionEnvironmentId,
        DefinitionEnvironmentRank = definitionEnvironmentRank,
        ReleaseEnvironmentId = release.Environments.Single<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (environment => environment.DefinitionEnvironmentId == definitionEnvironmentStep.DefinitionEnvironmentId)).Id,
        ReleaseId = release.Id
      };
    }
  }
}
