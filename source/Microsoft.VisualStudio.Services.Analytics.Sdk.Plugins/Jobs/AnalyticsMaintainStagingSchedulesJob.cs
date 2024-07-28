// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.Jobs.AnalyticsMaintainStagingSchedulesJob
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

using Microsoft.TeamFoundation.Analytics.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Text;

namespace Microsoft.TeamFoundation.Analytics.Plugins.Jobs
{
  public class AnalyticsMaintainStagingSchedulesJob : ITeamFoundationJobExtension
  {
    public static readonly Guid JobId = AnalyticsSdkConstants.AnalyticsMaintainStagingSchedulesJobGuid;

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      if (requestContext.GetService<IAnalyticsFeatureService>().IsAnalyticsEnabled(requestContext, bypassCache: true))
      {
        StringBuilder resultMessageSB = new StringBuilder();
        requestContext.GetService<IAnalyticsStagingJobService>().UpdateFeatureFlaggedStagingJobSchedules(requestContext, (Action<string>) (msg => resultMessageSB.AppendLine(msg)));
        resultMessage = resultMessageSB.ToString();
        return TeamFoundationJobExecutionResult.Succeeded;
      }
      resultMessage = "Analytics disabled -- no action taken.";
      return TeamFoundationJobExecutionResult.Succeeded;
    }
  }
}
