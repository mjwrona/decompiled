// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories.PublicRepositoryPollingJobBase
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories
{
  public abstract class PublicRepositoryPollingJobBase : VssAsyncJobExtension
  {
    public override sealed async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      IUpstreamFollower upstreamFollower = this.BootstrapFollower(requestContext);
      UpstreamFollowerTelemetryInfo telemetryInfo = new UpstreamFollowerTelemetryInfo();
      try
      {
        await upstreamFollower.ProcessRecentUpstreamChangesAsync(telemetryInfo);
        return new VssJobResult(TranslateExitReasonToResult(telemetryInfo.ExitReason), new
        {
          results = telemetryInfo
        }.Serialize(true));
      }
      catch (Exception ex)
      {
        return new VssJobResult(TeamFoundationJobExecutionResult.Failed, new
        {
          results = telemetryInfo,
          exceptionType = ex.GetType().Name,
          exceptionMessage = StackTraceCompressor.CompressStackTrace(ex.ToString())
        }.Serialize(true));
      }

      static TeamFoundationJobExecutionResult TranslateExitReasonToResult(
        UpstreamFollowerExitReason exitReason)
      {
        switch (exitReason)
        {
          case UpstreamFollowerExitReason.ReachedCurrent:
            return TeamFoundationJobExecutionResult.Succeeded;
          case UpstreamFollowerExitReason.ReachedLimit:
            return TeamFoundationJobExecutionResult.PartiallySucceeded;
          case UpstreamFollowerExitReason.UpstreamNotUpToDate:
            return TeamFoundationJobExecutionResult.PartiallySucceeded;
          case UpstreamFollowerExitReason.NoInterest:
            return TeamFoundationJobExecutionResult.Succeeded;
          case UpstreamFollowerExitReason.Exception:
            return TeamFoundationJobExecutionResult.Failed;
          case UpstreamFollowerExitReason.NoChanges:
            return TeamFoundationJobExecutionResult.Succeeded;
          default:
            throw new InvalidOperationException(string.Format("Unknown exit reason {0}", (object) exitReason));
        }
      }
    }

    protected abstract IUpstreamFollower BootstrapFollower(IVssRequestContext requestContext);
  }
}
