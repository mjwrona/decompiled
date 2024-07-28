// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ImsSearchCacheWarmer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity.Events;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class ImsSearchCacheWarmer : ISubscriber
  {
    private static readonly Guid s_searchWarmerTaskId = new Guid("034438C2-D470-4148-996B-5654617F2BC8");
    private static readonly string s_name = nameof (ImsSearchCacheWarmer);
    private static readonly SubscriberPriority s_priority = SubscriberPriority.AboveNormal;
    private static readonly Type[] s_subscribedTypes = new Type[1]
    {
      typeof (ImsSearchCacheExpiryEvent)
    };
    private const string s_area = "Microsoft.VisualStudio.Services.Identity";
    private const string s_layer = "ImsSearchCacheWarmer";

    public string Name => ImsSearchCacheWarmer.s_name;

    public SubscriberPriority Priority => ImsSearchCacheWarmer.s_priority;

    public Type[] SubscribedTypes() => ImsSearchCacheWarmer.s_subscribedTypes;

    public EventNotificationStatus ProcessEvent(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      object notificationEventArgs,
      out int statusCode,
      out string statusMessage,
      out ExceptionPropertyCollection properties)
    {
      statusCode = 0;
      statusMessage = (string) null;
      properties = (ExceptionPropertyCollection) null;
      if (!(notificationEventArgs is ImsSearchCacheExpiryEvent taskArgs) || taskArgs.HostId == Guid.Empty || taskArgs.ScopeId == Guid.Empty)
        return EventNotificationStatus.ActionPermitted;
      if (taskArgs.HostId != requestContext.ServiceHost.InstanceId)
      {
        statusMessage = string.Format("Expected a request context on host '{0}' but received one on host '{1}'.", (object) taskArgs.HostId, (object) requestContext.ServiceHost.InstanceId);
        throw new InvalidRequestContextHostException(statusMessage);
      }
      try
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        requestContext.GetService<LongRunningTaskService>().ScheduleLongRunningTask(requestContext, taskArgs.ScopeId, ImsSearchCacheWarmer.\u003C\u003EO.\u003C0\u003E__WarmUpSearchCache ?? (ImsSearchCacheWarmer.\u003C\u003EO.\u003C0\u003E__WarmUpSearchCache = new TeamFoundationTaskCallback(ImsSearchCacheWarmer.WarmUpSearchCache)), (object) taskArgs);
      }
      catch (TaskScheduleExistsException ex)
      {
        requestContext.TraceException(3241642, TraceLevel.Info, "Microsoft.VisualStudio.Services.Identity", nameof (ImsSearchCacheWarmer), (Exception) ex);
      }
      return EventNotificationStatus.ActionPermitted;
    }

    private static void WarmUpSearchCache(IVssRequestContext requestContext, object taskArgs)
    {
      ImsSearchCacheExpiryEvent var = taskArgs as ImsSearchCacheExpiryEvent;
      ArgumentUtility.CheckForNull<ImsSearchCacheExpiryEvent>(var, "imsSearchCacheExpiryEvent");
      ArgumentUtility.CheckForEmptyGuid(var.HostId, "imsSearchCacheExpiryEvent.HostId");
      ArgumentUtility.CheckForEmptyGuid(var.ScopeId, "imsSearchCacheExpiryEvent.ScopeId");
      requestContext.GetService<IdentityService>().IdentityServiceInternal().RefreshSearchIdentitiesCache(requestContext, var.ScopeId);
    }
  }
}
