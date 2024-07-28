// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.RemoteSettingsJsonLogFile
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class RemoteSettingsJsonLogFile : 
    BaseJsonLogFile<RemoteSettingsLogger.RemoteSettingsLogMessage>
  {
    public RemoteSettingsJsonLogFile(ITelemetryWriter writer = null)
      : base(writer)
    {
    }

    protected override string ConvertEventToString(
      RemoteSettingsLogger.RemoteSettingsLogMessage eventData)
    {
      try
      {
        return JsonConvert.SerializeObject((object) eventData, new JsonSerializerSettings()
        {
          NullValueHandling = NullValueHandling.Ignore
        });
      }
      catch (JsonSerializationException ex)
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\"Name\":\"{0}\",\"Error\":\"Cannot serialize log message. Error: {1}\"}}", new object[2]
        {
          (object) eventData.Message,
          (object) ex.Message
        });
      }
    }
  }
}
