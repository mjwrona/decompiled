// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.CleanupAgileArtifactsJobExtension
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.JobAgentPlugins;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.JobAgentPlugins.Logic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  public class CleanupAgileArtifactsJobExtension : ITeamFoundationJobExtension
  {
    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      List<JobStepResult> results = new List<JobStepResult>()
      {
        new DeleteBoardsAndSettingsForDeletedTeamsJobStep().Execute(requestContext),
        new DeleteBoardsForDeletedBacklogLevelsJobStep().Execute(requestContext),
        new CleanupDeletedTeamConfigurationJobStep().Execute(requestContext)
      };
      resultMessage = JsonConvert.SerializeObject((object) results, Formatting.Indented);
      return this.ComputeJobExecutionResult(results);
    }

    internal TeamFoundationJobExecutionResult ComputeJobExecutionResult(List<JobStepResult> results) => !results.Where<JobStepResult>((Func<JobStepResult, bool>) (res => res.ExecutionResult != 0)).Any<JobStepResult>() ? TeamFoundationJobExecutionResult.Succeeded : TeamFoundationJobExecutionResult.Failed;
  }
}
