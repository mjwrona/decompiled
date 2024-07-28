// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.SubscriptionAdapterFactory
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class SubscriptionAdapterFactory
  {
    internal static ISubscriptionAdapter CreateAdapter(
      IVssRequestContext requestContext,
      string eventType,
      string matcher,
      SubscriptionScope scopeId,
      string artifactType = null,
      bool throwIfNotFound = true,
      bool throwIfEventTypeNull = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(matcher, nameof (matcher));
      if (throwIfEventTypeNull)
        ArgumentUtility.CheckForNull<string>(eventType, nameof (eventType));
      eventType = EventTypeMapper.ToLegacy(requestContext, eventType);
      IReadOnlyList<ISubscriptionAdapter> notificationExtentions = requestContext.GetStaticNotificationExtentions<ISubscriptionAdapter>();
      ISubscriptionAdapter subscriptionAdapter1 = (ISubscriptionAdapter) null;
      ISubscriptionAdapter subscriptionAdapter2 = (ISubscriptionAdapter) null;
      foreach (ISubscriptionAdapter subscriptionAdapter3 in (IEnumerable<ISubscriptionAdapter>) notificationExtentions)
      {
        if (subscriptionAdapter3.GetMatcher(requestContext, eventType).Equals(matcher, StringComparison.InvariantCultureIgnoreCase))
        {
          if (subscriptionAdapter3.SubscriptionTypeName == "*")
            subscriptionAdapter2 = subscriptionAdapter3;
          if (subscriptionAdapter3.SubscriptionTypeName.Equals(eventType, StringComparison.InvariantCultureIgnoreCase))
          {
            subscriptionAdapter1 = subscriptionAdapter3;
            break;
          }
          if (!string.IsNullOrEmpty(artifactType) && subscriptionAdapter3.SubscriptionTypeName.Equals(artifactType, StringComparison.InvariantCultureIgnoreCase))
          {
            subscriptionAdapter1 = subscriptionAdapter3;
            break;
          }
        }
      }
      return SubscriptionAdapterFactory.CreateAndInitializeAdapter(requestContext, subscriptionAdapter1 ?? subscriptionAdapter2, matcher, (string) null, eventType, throwIfNotFound, scopeId);
    }

    internal static ISubscriptionAdapter CreateAdapter(
      IVssRequestContext requestContext,
      ISubscriptionFilter filter,
      SubscriptionScope scopeId,
      bool throwIfNotFound = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ISubscriptionFilter>(filter, nameof (filter));
      string legacy = EventTypeMapper.ToLegacy(requestContext, filter.EventType);
      string str1 = filter is ArtifactFilter artifactFilter ? artifactFilter.ArtifactType : string.Empty;
      bool flag1 = filter.Type.Equals("Expression");
      IReadOnlyList<ISubscriptionAdapter> notificationExtentions = requestContext.GetStaticNotificationExtentions<ISubscriptionAdapter>();
      string str2 = !string.IsNullOrEmpty(legacy) ? NotificationSubscriptionService.GetPathMatcherForEvent(requestContext, legacy) : string.Empty;
      ISubscriptionAdapter subscriptionAdapter1 = (ISubscriptionAdapter) null;
      ISubscriptionAdapter subscriptionAdapter2 = (ISubscriptionAdapter) null;
      foreach (ISubscriptionAdapter subscriptionAdapter3 in (IEnumerable<ISubscriptionAdapter>) notificationExtentions)
      {
        if (subscriptionAdapter3.FilterType.Equals(filter.Type, StringComparison.InvariantCultureIgnoreCase))
        {
          if (subscriptionAdapter3.SubscriptionTypeName == "*")
          {
            bool flag2 = true;
            if (flag1)
            {
              string matcher = subscriptionAdapter3.GetMatcher(requestContext, legacy);
              flag2 = str2.Equals(matcher);
            }
            if (flag2)
              subscriptionAdapter2 = subscriptionAdapter3;
          }
          if (subscriptionAdapter3.SubscriptionTypeName.Equals(legacy, StringComparison.InvariantCultureIgnoreCase))
          {
            subscriptionAdapter1 = subscriptionAdapter3;
            break;
          }
          if (!string.IsNullOrEmpty(str1) && subscriptionAdapter3.SubscriptionTypeName.Equals(str1, StringComparison.InvariantCultureIgnoreCase))
          {
            subscriptionAdapter1 = subscriptionAdapter3;
            break;
          }
        }
      }
      return SubscriptionAdapterFactory.CreateAndInitializeAdapter(requestContext, subscriptionAdapter1 ?? subscriptionAdapter2, (string) null, filter.Type, legacy, throwIfNotFound, scopeId);
    }

    private static ISubscriptionAdapter CreateAndInitializeAdapter(
      IVssRequestContext requestContext,
      ISubscriptionAdapter adapterBestMatch,
      string matcher,
      string filterType,
      string eventType,
      bool throwIfNotFound,
      SubscriptionScope scopeId)
    {
      ISubscriptionAdapter initializeAdapter = (ISubscriptionAdapter) null;
      if (adapterBestMatch != null)
        initializeAdapter = ExtensionsUtil.CreateNewInstance((object) adapterBestMatch) as ISubscriptionAdapter;
      if (initializeAdapter == null)
      {
        if (!throwIfNotFound)
          return (ISubscriptionAdapter) null;
        if (!string.IsNullOrEmpty(filterType))
          throw new NoSubscriptionAdaterFoundException(CoreRes.ErrorLoadSubscriptionAdapterByFilter((object) filterType));
        throw new NoSubscriptionAdaterFoundException(CoreRes.ErrorLoadSubscriptionAdapterByMatcher((object) matcher, (object) eventType));
      }
      initializeAdapter.InitializeAdapter(requestContext, eventType, scopeId);
      return initializeAdapter;
    }
  }
}
