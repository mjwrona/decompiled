// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.FollowUtils
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public static class FollowUtils
  {
    private const int WorkItemTrackingStart = 900000;
    private const int NotificationTracePointBase = 901100;

    public static bool IsFollowEvent(
      IVssRequestContext requestContext,
      string subscriptionDescription,
      ChangedFieldsType changedFields)
    {
      if (!requestContext.IsFeatureEnabled("WorkItemTracking.Server.FollowsFilter"))
        return true;
      FollowsFilter followsFilter = FollowUtils.Deserialize<FollowsFilter>(requestContext, subscriptionDescription);
      if (followsFilter == null || followsFilter.FieldFilters == null || !followsFilter.FieldFilters.Any<string>())
        return true;
      if (changedFields == null)
        return false;
      bool flag = false;
      foreach (string fieldFilter1 in followsFilter.FieldFilters)
      {
        string fieldFilter = fieldFilter1;
        if (changedFields.IntegerFields != null && ((IEnumerable<IntegerField>) changedFields.IntegerFields).Any<IntegerField>((Func<IntegerField, bool>) (x => TFStringComparer.WorkItemFieldReferenceName.Equals(fieldFilter, x.ReferenceName))))
        {
          flag = true;
          break;
        }
        if (changedFields.StringFields != null && ((IEnumerable<StringField>) changedFields.StringFields).Any<StringField>((Func<StringField, bool>) (x => TFStringComparer.WorkItemFieldReferenceName.Equals(fieldFilter, x.ReferenceName))))
        {
          flag = true;
          break;
        }
        if (changedFields.BooleanFields != null && ((IEnumerable<BooleanField>) changedFields.BooleanFields).Any<BooleanField>((Func<BooleanField, bool>) (x => TFStringComparer.WorkItemFieldReferenceName.Equals(fieldFilter, x.ReferenceName))))
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    public static T Deserialize<T>(IVssRequestContext requestContext, string data)
    {
      if (string.IsNullOrEmpty(data))
        return default (T);
      try
      {
        requestContext.TraceEnter(901118, "Plugins", "NotificationFilter", nameof (Deserialize));
        return JsonConvert.DeserializeObject<T>(data);
      }
      catch
      {
      }
      finally
      {
        requestContext.TraceLeave(901120, "Plugins", "NotificationFilter", nameof (Deserialize));
      }
      return default (T);
    }

    public static bool IsFollowSubscription(string matcher) => !string.IsNullOrEmpty(matcher) && matcher.Equals("FollowsMatcher", StringComparison.OrdinalIgnoreCase);
  }
}
