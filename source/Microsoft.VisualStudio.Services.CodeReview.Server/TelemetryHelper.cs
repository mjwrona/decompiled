// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.TelemetryHelper
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal static class TelemetryHelper
  {
    internal static void Publish(
      IVssRequestContext requestContext,
      string feature,
      List<Tuple<string, string>> ctProperies)
    {
      ClientTraceData properties = new ClientTraceData();
      foreach (Tuple<string, string> ctPropery in ctProperies)
        properties.Add(ctPropery.Item1, (object) ctPropery.Item2);
      requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.CodeReview.Server", feature, properties);
    }

    internal static void ExecuteAndMeasure(ClientTraceData ctData, string key, Action action)
    {
      if (ctData == null || string.IsNullOrEmpty(key) || action == null)
        return;
      Stopwatch stopwatch = Stopwatch.StartNew();
      action();
      ctData.Add(key, (object) stopwatch.ElapsedMilliseconds);
    }
  }
}
