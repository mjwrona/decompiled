// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Telemetry.IVssRequestContextTelemetryExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Telemetry
{
  public static class IVssRequestContextTelemetryExtensions
  {
    public static void AddSqlTimings(
      this IVssRequestContext requestContext,
      string storedProcedureName,
      IEnumerable<KeyValuePair<string, int>> timings)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(storedProcedureName, nameof (storedProcedureName));
      if (timings == null)
        return;
      if (!timings.Any<KeyValuePair<string, int>>())
        return;
      try
      {
        IDictionary<string, IEnumerable<KeyValuePair<string, int>>> dictionary;
        if (!requestContext.TryGetItem<IDictionary<string, IEnumerable<KeyValuePair<string, int>>>>("WorkItemTracking.SqlTimings", out dictionary))
        {
          dictionary = (IDictionary<string, IEnumerable<KeyValuePair<string, int>>>) new Dictionary<string, IEnumerable<KeyValuePair<string, int>>>();
          requestContext.Items["WorkItemTracking.SqlTimings"] = (object) dictionary;
        }
        dictionary.TryAdd<string, IEnumerable<KeyValuePair<string, int>>>(storedProcedureName, timings);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(908701, "WorkItemTelemetry", "WorkItemTrackingRequestContext", ex);
      }
    }
  }
}
