// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.Common.ICodeReviewDispatcher
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F16CDF2D-8103-4EAE-A2A8-4FA5B1C1BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi.Events;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.CodeReview.Server.Common
{
  [DefaultServiceImplementation(typeof (CodeReviewDispatcher))]
  public interface ICodeReviewDispatcher : IVssFrameworkService
  {
    void SendRealtimeEvent(
      IVssRequestContext requestContext,
      CodeReviewEventNotification notificationEvent);

    Task WatchReview(IVssRequestContext requestContext, int reviewId, string clientId);

    Task StopWatchingReview(IVssRequestContext requestContext, int reviewId, string clientId);
  }
}
