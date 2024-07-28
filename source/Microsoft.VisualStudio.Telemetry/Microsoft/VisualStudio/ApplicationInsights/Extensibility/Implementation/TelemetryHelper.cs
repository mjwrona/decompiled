// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.TelemetryHelper
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Channel;
using Microsoft.VisualStudio.ApplicationInsights.DataContracts;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation
{
  internal static class TelemetryHelper
  {
    public static void WriteEnvelopeProperties(this ITelemetry telemetry, IJsonWriter json)
    {
      if (telemetry is EventTelemetry telemetry1)
      {
        TelemetryHelper.AddMsInternal(telemetry1, json);
        switch (telemetry1.CommonSchemaVersion)
        {
          case 2:
            json.WriteProperty("time", new DateTimeOffset?(telemetry.Timestamp));
            json.WriteProperty("seq", telemetry.Sequence);
            ((IJsonSerializable) telemetry.Context).Serialize(json);
            break;
          case 4:
            IJsonWriter jsonWriter = json;
            DateTimeOffset dateTimeOffset = telemetry.Timestamp;
            dateTimeOffset = dateTimeOffset.ToUniversalTime();
            string str = dateTimeOffset.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            jsonWriter.WriteProperty("time", str);
            json.WriteProperty("ver", "4.0");
            json.WriteProperty("iKey", string.Format("o:{0}", (object) telemetry.Context.InstrumentationKey.Split('-')[0]));
            break;
          default:
            throw new NotImplementedException();
        }
      }
      else
      {
        json.WriteProperty("time", new DateTimeOffset?(telemetry.Timestamp));
        json.WriteProperty("seq", telemetry.Sequence);
        ((IJsonSerializable) telemetry.Context).Serialize(json);
      }
    }

    private static void AddMsInternal(EventTelemetry telemetry, IJsonWriter json)
    {
      if (!telemetry.IsMicrosoftInternal())
        return;
      json.WritePropertyName("ext");
      json.WriteStartObject();
      json.WritePropertyName("os");
      json.WriteStartObject();
      json.WriteProperty("expId", "msInternal");
      json.WriteEndObject();
      json.WriteEndObject();
    }

    public static void WriteTelemetryName(
      this ITelemetry telemetry,
      IJsonWriter json,
      string telemetryName)
    {
      if (telemetry is EventTelemetry eventTelemetry)
      {
        if ((telemetry as EventTelemetry).CommonSchemaVersion == 4)
          json.WriteProperty("name", eventTelemetry.Data.name.Replace('/', '.'));
        else
          json.WriteProperty("name", eventTelemetry.Data.name);
      }
      else
      {
        bool result = false;
        string str1;
        if (telemetry is ISupportProperties supportProperties && supportProperties.Properties.TryGetValue("DeveloperMode", out str1))
          bool.TryParse(str1, out result);
        string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}", new object[3]
        {
          result ? (object) "Microsoft.ApplicationInsights.Dev." : (object) "Microsoft.ApplicationInsights.",
          (object) TelemetryHelper.NormalizeInstrumentationKey(telemetry.Context.InstrumentationKey),
          (object) telemetryName
        });
        json.WriteProperty("name", str2);
      }
    }

    private static string NormalizeInstrumentationKey(string instrumentationKey) => instrumentationKey.IsNullOrWhiteSpace() ? string.Empty : instrumentationKey.Replace("-", string.Empty).ToLowerInvariant() + ".";
  }
}
