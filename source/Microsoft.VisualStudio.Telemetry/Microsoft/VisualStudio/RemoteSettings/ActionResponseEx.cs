// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.ActionResponseEx
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal static class ActionResponseEx
  {
    private static readonly Lazy<JsonConverter[]> matchConvertersLazy = new Lazy<JsonConverter[]>((Func<JsonConverter[]>) (() => new JsonConverter[2]
    {
      (JsonConverter) new JsonTelemetryEventMatchConverter(),
      (JsonConverter) new JsonTelemetryManifestMatchValueConverter()
    }));

    public static ActionWrapper<T> AsTypedAction<T>(this ActionResponse actionResponse)
    {
      try
      {
        T obj = JsonConvert.DeserializeObject<T>(actionResponse.ActionJson, ActionResponseEx.matchConvertersLazy.Value);
        return new ActionWrapper<T>()
        {
          RuleId = actionResponse.RuleId,
          FlightName = actionResponse.FlightName,
          ActionPath = actionResponse.ActionPath,
          Precedence = actionResponse.Precedence,
          Action = obj
        };
      }
      catch (Exception ex)
      {
        throw new TargetedNotificationsException(ex.Message, ex);
      }
    }

    public static Dictionary<string, ITelemetryEventMatch> GetTriggers(
      this ActionResponse actionResponse)
    {
      try
      {
        return string.IsNullOrWhiteSpace(actionResponse.TriggerJson) ? (Dictionary<string, ITelemetryEventMatch>) null : JsonConvert.DeserializeObject<Dictionary<string, ITelemetryEventMatch>>(actionResponse.TriggerJson, ActionResponseEx.matchConvertersLazy.Value);
      }
      catch (Exception ex)
      {
        throw new TargetedNotificationsException(ex.Message, ex);
      }
    }

    public static Dictionary<string, ActionTriggerOptions> GetTriggerOptions(
      this ActionResponse actionResponse)
    {
      try
      {
        return string.IsNullOrWhiteSpace(actionResponse.TriggerJson) ? (Dictionary<string, ActionTriggerOptions>) null : JsonConvert.DeserializeObject<Dictionary<string, ActionTriggerOptions>>(actionResponse.TriggerJson);
      }
      catch (Exception ex)
      {
        throw new TargetedNotificationsException(ex.Message, ex);
      }
    }
  }
}
