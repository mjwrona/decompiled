// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.SocialEngagementAggregatesCleanupJob
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.SocialServer.Server
{
  public class SocialEngagementAggregatesCleanupJob : ITeamFoundationJobExtension
  {
    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      int num = requestContext.GetService<ISocialEngService>().DeleteOldAggregatedSocialEngagementMetrics(requestContext);
      resultMessage = string.Format("deleted {0} old records", (object) num);
      return TeamFoundationJobExecutionResult.Succeeded;
    }
  }
}
