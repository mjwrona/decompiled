// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.TelemetryUtil
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public static class TelemetryUtil
  {
    public static void TraceErrorAlways(
      this IVssRequestContext requestContext,
      string message,
      string ciKey,
      TimedCiEvent ciData,
      int tracePoint = 15250700,
      string layer = "Jobs")
    {
      ciData.Properties.AddOrIncrement(ciKey, 1L);
      requestContext.TraceAlways(tracePoint, TraceLevel.Error, "Wiki", layer, message);
    }

    public static void TraceException(
      this IVssRequestContext requestContext,
      Exception ex,
      int tracePoint = 15250700,
      string traceLayer = "Jobs")
    {
      requestContext.TraceException(tracePoint, "Wiki", traceLayer, ex);
    }

    public static void AddOrIncrement(
      this IDictionary<string, object> dictionary,
      string key,
      long value)
    {
      object obj;
      if (dictionary.TryGetValue(key, out obj))
        value += (long) obj;
      dictionary[key] = (object) value;
    }

    public static void EmitHasPageIdCiDataIfNeeded(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      GitVersionDescriptor wikiVersion,
      WikiPage wikiPage,
      string callerName,
      string area,
      string feature)
    {
      if (wikiPage.GitItemPath == null || !wikiPage.GitItemPath.IsMdFile())
        return;
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("HasPageId", wikiPage.Id.HasValue);
      properties.Add("CallerName", callerName);
      properties.Add("WikiId", (object) wiki.Id);
      properties.Add("Version", wikiVersion.Version);
      properties.Add("WikiType", (object) wiki.Type);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, area, feature, properties);
    }
  }
}
