// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.TeamFoundationRequestContextExtension
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class TeamFoundationRequestContextExtension
  {
    public const string WitContextKey = "WorkItemTrackingRequestContext";

    public static WorkItemTrackingRequestContext WitContext(this IVssRequestContext context)
    {
      WorkItemTrackingRequestContext trackingRequestContext;
      if (!context.Items.TryGetValue<WorkItemTrackingRequestContext>("WorkItemTrackingRequestContext", out trackingRequestContext))
      {
        trackingRequestContext = new WorkItemTrackingRequestContext(context);
        context.Items["WorkItemTrackingRequestContext"] = (object) trackingRequestContext;
      }
      return trackingRequestContext;
    }

    public static T CreateReadReplicaAwareComponent<T>(
      this IVssRequestContext requestContext,
      WitReadReplicaContext? readReplicaContext)
      where T : WorkItemTrackingResourceComponent, new()
    {
      DatabaseConnectionType? connectionType = requestContext.IsReadReplicaEnabled(readReplicaContext) ? new DatabaseConnectionType?(DatabaseConnectionType.IntentReadOnly) : new DatabaseConnectionType?();
      return requestContext.CreateComponent<T>(connectionType: connectionType);
    }

    internal static bool IsReadReplicaEnabled(
      this IVssRequestContext requestContext,
      WitReadReplicaContext? readReplicaContext)
    {
      if (!readReplicaContext.HasValue || readReplicaContext.Value.Feature == null || !requestContext.IsFeatureEnabled(readReplicaContext.Value.Feature) || requestContext.Method == null || string.IsNullOrEmpty(requestContext.Method.Name))
        return false;
      WorkItemTrackingReadReplicaConfiguration readReplicaSettings = requestContext.WitContext().ServerSettings.ReadReplicaSettings;
      if (requestContext.IsFeatureEnabled("WorkItemTracking.Server.Commands.ForcedReadFromReadReplica") && readReplicaSettings.ForcedReadReplicaCommands.ContainsKey(requestContext.Method.Name))
        return true;
      return readReplicaSettings.ReadReplicaEnabledCommands.ContainsKey(requestContext.Method.Name) && requestContext.IsReadReplicaUserOrUserAgent();
    }

    internal static bool IsReadReplicaUserOrUserAgent(this IVssRequestContext requestContext)
    {
      WorkItemTrackingReadReplicaConfiguration readReplicaSettings = requestContext.WitContext().ServerSettings.ReadReplicaSettings;
      return readReplicaSettings.ReadReplicaUsers.ContainsKey(requestContext.GetUserId().ToString()) || !string.IsNullOrEmpty(requestContext.UserAgent) && (readReplicaSettings.ReadReplicaUserAgents.ContainsKey(requestContext.UserAgent) || readReplicaSettings.ReadReplicaUserAgents.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (ua => requestContext.UserAgent.IndexOf(ua.Key, StringComparison.OrdinalIgnoreCase) != -1)));
    }
  }
}
