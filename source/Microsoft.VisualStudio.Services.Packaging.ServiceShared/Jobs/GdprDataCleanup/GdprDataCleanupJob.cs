// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.GdprDataCleanup.GdprDataCleanupJob
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.GdprDataCleanup
{
  public class GdprDataCleanupJob : ITeamFoundationJobExtension
  {
    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      requestContext.CheckServiceHostType(TeamFoundationHostType.Deployment);
      VssJobResult vssJobResult = new GdprDataCleanupJobHandlerBootstrapper(requestContext, jobDefinition.JobId).Bootstrap().Handle(jobDefinition).ToVssJobResult();
      resultMessage = vssJobResult.Message;
      return vssJobResult.Result;
    }
  }
}
