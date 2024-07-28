// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.TelemetryHelper
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  public static class TelemetryHelper
  {
    public static void AddToResults(this ClientTraceData ctData, string key, string value)
    {
      IDictionary<string, object> data = ctData.GetData();
      object obj;
      if (!data.TryGetValue(key, out obj))
        data.Add(key, (object) new List<string>() { value });
      else
        ((List<string>) obj).Add(value);
    }

    public static void AddToResults(this ClientTraceData ctData, string key, object value)
    {
      IDictionary<string, object> data = ctData.GetData();
      object obj;
      if (!data.TryGetValue(key, out obj))
        data.Add(key, (object) new List<object>() { value });
      else
        ((List<object>) obj).Add(value);
    }

    public static void AddRangeToResults(
      this ClientTraceData ctData,
      string key,
      IEnumerable<string> value)
    {
      object obj;
      if (!ctData.GetData().TryGetValue(key, out obj))
        ctData.Add(key, (object) new List<string>(value));
      else
        ((List<string>) obj).AddRange(value);
    }

    public static void PublishClientTrace(
      this ClientTraceData ctData,
      IVssRequestContext context,
      string area,
      string feature)
    {
      context.GetService<ClientTraceService>().Publish(context, area, feature, ctData);
    }
  }
}
