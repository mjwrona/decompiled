// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.Common.CodeReviewDispatcher
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F16CDF2D-8103-4EAE-A2A8-4FA5B1C1BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Server.Common.dll

using Microsoft.AspNet.SignalR;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.CodeReview.Server.Common
{
  internal sealed class CodeReviewDispatcher : ICodeReviewDispatcher, IVssFrameworkService
  {
    private const string c_signalrFeatureFlagName = "AzureTfs.SignalR";
    private IHubContext m_hubContext;

    public Task WatchReview(IVssRequestContext requestContext, int reviewId, string clientId) => this.m_hubContext.Groups.Add(clientId, this.GetGroupName(requestContext, reviewId));

    public Task StopWatchingReview(
      IVssRequestContext requestContext,
      int reviewId,
      string clientId)
    {
      return this.m_hubContext.Groups.Remove(clientId, this.GetGroupName(requestContext, reviewId));
    }

    public void SendRealtimeEvent(
      IVssRequestContext requestContext,
      CodeReviewEventNotification notificationEvent)
    {
      if (!requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "AzureTfs.SignalR"))
        return;
      try
      {
        ReviewNotification reviewNotification = notificationEvent as ReviewNotification;
        ReviewersNotification reviewersNotification = notificationEvent as ReviewersNotification;
        IterationNotification iterationNotification = notificationEvent as IterationNotification;
        CommentNotification commentNotification = notificationEvent as CommentNotification;
        AttachmentNotification attachmentNotification = notificationEvent as AttachmentNotification;
        StatusNotification statusNotification = notificationEvent as StatusNotification;
        PropertiesNotification propertiesNotification = notificationEvent as PropertiesNotification;
        object obj = this.m_hubContext.Clients.Group(this.GetGroupName(requestContext, notificationEvent.ReviewId));
        if (reviewersNotification != null)
        {
          // ISSUE: reference to a compiler-generated field
          if (CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, ReviewersNotification>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "reviewerEvents", (IEnumerable<Type>) null, typeof (CodeReviewDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0.Target((CallSite) CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0, obj, requestContext, reviewersNotification);
        }
        else if (iterationNotification != null)
        {
          // ISSUE: reference to a compiler-generated field
          if (CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__1 == null)
          {
            // ISSUE: reference to a compiler-generated field
            CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__1 = CallSite<Action<CallSite, object, IVssRequestContext, IterationNotification>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "iterationEvents", (IEnumerable<Type>) null, typeof (CodeReviewDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__1.Target((CallSite) CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__1, obj, requestContext, iterationNotification);
        }
        else if (commentNotification != null)
        {
          // ISSUE: reference to a compiler-generated field
          if (CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__2 == null)
          {
            // ISSUE: reference to a compiler-generated field
            CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__2 = CallSite<Action<CallSite, object, IVssRequestContext, CommentNotification>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "commentEvents", (IEnumerable<Type>) null, typeof (CodeReviewDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__2.Target((CallSite) CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__2, obj, requestContext, commentNotification);
        }
        else if (attachmentNotification != null)
        {
          // ISSUE: reference to a compiler-generated field
          if (CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__3 == null)
          {
            // ISSUE: reference to a compiler-generated field
            CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__3 = CallSite<Action<CallSite, object, IVssRequestContext, AttachmentNotification>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "attachmentEvents", (IEnumerable<Type>) null, typeof (CodeReviewDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__3.Target((CallSite) CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__3, obj, requestContext, attachmentNotification);
        }
        else if (statusNotification != null)
        {
          // ISSUE: reference to a compiler-generated field
          if (CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__4 == null)
          {
            // ISSUE: reference to a compiler-generated field
            CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__4 = CallSite<Action<CallSite, object, IVssRequestContext, StatusNotification>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "statusEvents", (IEnumerable<Type>) null, typeof (CodeReviewDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__4.Target((CallSite) CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__4, obj, requestContext, statusNotification);
        }
        else if (propertiesNotification != null)
        {
          // ISSUE: reference to a compiler-generated field
          if (CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__5 == null)
          {
            // ISSUE: reference to a compiler-generated field
            CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__5 = CallSite<Action<CallSite, object, IVssRequestContext, PropertiesNotification>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "propertiesEvents", (IEnumerable<Type>) null, typeof (CodeReviewDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__5.Target((CallSite) CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__5, obj, requestContext, propertiesNotification);
        }
        else
        {
          if (reviewNotification == null)
            return;
          // ISSUE: reference to a compiler-generated field
          if (CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__6 == null)
          {
            // ISSUE: reference to a compiler-generated field
            CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__6 = CallSite<Action<CallSite, object, IVssRequestContext, ReviewNotification>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "reviewEvents", (IEnumerable<Type>) null, typeof (CodeReviewDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__6.Target((CallSite) CodeReviewDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__6, obj, requestContext, reviewNotification);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1380902, this.GetType().Namespace, this.GetType().Name, ex);
      }
    }

    private string GetGroupName(IVssRequestContext requestContext, int reviewId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_CodeReview_{1}", (object) requestContext.ServiceHost.CollectionServiceHost.InstanceId, (object) reviewId);

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext) => this.m_hubContext = GlobalHost.ConnectionManager.GetHubContext<CodeReviewDetailHub>();

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => this.m_hubContext = (IHubContext) null;
  }
}
