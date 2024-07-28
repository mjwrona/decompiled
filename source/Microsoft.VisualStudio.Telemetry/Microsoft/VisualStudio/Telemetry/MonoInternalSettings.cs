// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.MonoInternalSettings
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing;
using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class MonoInternalSettings : InternalSettingsBase
  {
    internal const string TelemetryUserDirKeyPath = "VSTelemetry";
    private const int ChannelExplicitlyEnabled = 1;
    private const int ChannelExplicitlyDisabled = 0;
    private JObject channelSettingsJson;

    public MonoInternalSettings(
      IDiagnosticTelemetry diagnosticTelemetry,
      IRegistryTools registryTools)
      : base(diagnosticTelemetry, registryTools)
    {
      this.LoadChannelSettings();
    }

    public override ChannelInternalSetting GetChannelSettings(string channelId)
    {
      channelId.RequiresArgumentNotNullAndNotEmpty(nameof (channelId));
      int num = -1;
      try
      {
        if (this.channelSettingsJson != null)
        {
          JToken jtoken;
          if (this.channelSettingsJson.TryGetValue(channelId, StringComparison.OrdinalIgnoreCase, out jtoken))
          {
            string str = jtoken.ToString();
            if (str == "enabled")
              num = 1;
            if (str == "disabled")
              num = 0;
          }
        }
      }
      catch
      {
        num = -1;
      }
      if (num == 1)
        return ChannelInternalSetting.ExplicitlyEnabled;
      return num == 0 ? ChannelInternalSetting.ExplicitlyDisabled : base.GetChannelSettings(channelId);
    }

    private void LoadChannelSettings()
    {
      try
      {
        string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "VSTelemetry", "channels.json");
        if (!File.Exists(path))
          return;
        this.channelSettingsJson = JsonConvert.DeserializeObject(File.ReadAllText(path)) as JObject;
      }
      catch
      {
        CoreEventSource.Log.LogError("Could not deserialize channel settings json");
        this.channelSettingsJson = (JObject) null;
      }
    }
  }
}
