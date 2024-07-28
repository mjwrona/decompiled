// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.PopulateShardDetailsJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class PopulateShardDetailsJob : ITeamFoundationJobExtension
  {
    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      try
      {
        requestContext.GetService<ITeamFoundationJobService>().DeleteJobDefinitions(requestContext, (IEnumerable<Guid>) new List<Guid>()
        {
          jobDefinition.JobId
        });
        resultMessage = "No-op for this job. The population of shard details is done whenever we are unable to fetch shard information.The run just deletes the job definition.";
        return TeamFoundationJobExecutionResult.Succeeded;
      }
      catch (Exception ex)
      {
        resultMessage = "Deletion of one time job definition for shard deletion failed. " + ex?.ToString();
        return TeamFoundationJobExecutionResult.Failed;
      }
    }
  }
}
