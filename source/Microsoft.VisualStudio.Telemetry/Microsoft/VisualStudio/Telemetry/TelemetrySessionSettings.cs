// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetrySessionSettings
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.WindowsErrorReporting;
using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Microsoft.VisualStudio.Telemetry
{
  [DataContract]
  internal sealed class TelemetrySessionSettings
  {
    private const string DefaultHostName = "Default";
    private const uint DefaultAppId = 1000;

    [DataMember]
    public bool IsOptedIn { get; set; }

    [DataMember(IsRequired = false)]
    public bool IsInitialSession { get; [param: AllowNull] set; }

    [DataMember]
    public string HostName { get; set; }

    [DataMember]
    public string AppInsightsInstrumentationKey { get; set; }

    [DataMember]
    public string AsimovInstrumentationKey { get; set; }

    [DataMember]
    public string CollectorApiKey { get; set; }

    [DataMember]
    public uint AppId { get; set; }

    [DataMember]
    public Guid? UserId { get; set; }

    [DataMember(IsRequired = true)]
    public string Id { get; set; }

    [DataMember(IsRequired = true)]
    public long ProcessStartTime { get; set; }

    [DataMember]
    public string VSExeVersion { get; set; }

    [DataMember]
    public string SkuName { get; set; }

    [DataMember]
    public List<BucketFilter> BucketFiltersToEnableWatsonForFaults { get; set; } = new List<BucketFilter>();

    [DataMember]
    public List<BucketFilter> BucketFiltersToAddDumpsToFaults { get; set; } = new List<BucketFilter>();

    [IgnoreDataMember]
    public bool CanOverrideHostName { get; private set; }

    [IgnoreDataMember]
    public bool CanOverrideAppId { get; private set; }

    public TelemetrySessionSettings(
      string id,
      IInternalSettings internalSettings,
      string appInsightsIKey,
      string asimovIKey,
      string collectorApiKey,
      IProcessCreationTime processCreation)
    {
      internalSettings.RequiresArgumentNotNull<IInternalSettings>(nameof (internalSettings));
      this.Id = id;
      this.AppInsightsInstrumentationKey = appInsightsIKey;
      this.AsimovInstrumentationKey = asimovIKey;
      this.CollectorApiKey = collectorApiKey;
      this.ProcessStartTime = processCreation.GetProcessCreationTime();
      string testHostName;
      if (internalSettings.TryGetTestHostName(out testHostName))
      {
        this.HostName = testHostName;
      }
      else
      {
        this.HostName = "Default";
        this.CanOverrideHostName = true;
      }
      uint testAppId;
      if (internalSettings.TryGetTestAppId(out testAppId))
      {
        this.AppId = testAppId;
      }
      else
      {
        this.AppId = 1000U;
        this.CanOverrideAppId = true;
      }
    }

    internal static bool IsSessionIdValid(string sessionID) => !sessionID.IsNullOrWhiteSpace();

    internal static TelemetrySessionSettings Parse(string serializedSession)
    {
      if (!TelemetrySessionSettings.IsSerializedSessionValid(serializedSession))
        throw new ArgumentException("Serialized session is not valid", nameof (serializedSession));
      TelemetrySessionSettings telemetrySessionSettings;
      try
      {
        telemetrySessionSettings = TelemetrySessionSettings.Deserialize(serializedSession);
      }
      catch (Exception ex)
      {
        throw new ArgumentException("Could not deserialize to TelemetrySession object.", nameof (serializedSession), ex);
      }
      return TelemetrySessionSettings.IsSessionIdValid(telemetrySessionSettings.Id) ? telemetrySessionSettings : throw new ArgumentException("Session ID in sessionSettings is not valid", "sessionSettings");
    }

    public override bool Equals(object other)
    {
      if (!(other is TelemetrySessionSettings telemetrySessionSettings) || (int) this.AppId != (int) telemetrySessionSettings.AppId || !(this.AppInsightsInstrumentationKey == telemetrySessionSettings.AppInsightsInstrumentationKey) || !(this.AsimovInstrumentationKey == telemetrySessionSettings.AsimovInstrumentationKey) || !(this.CollectorApiKey == telemetrySessionSettings.CollectorApiKey) || !(this.HostName == telemetrySessionSettings.HostName) || !(this.Id == telemetrySessionSettings.Id) || this.IsOptedIn != telemetrySessionSettings.IsOptedIn || this.ProcessStartTime != telemetrySessionSettings.ProcessStartTime)
        return false;
      Guid? userId1 = this.UserId;
      Guid? userId2 = telemetrySessionSettings.UserId;
      return (userId1.HasValue == userId2.HasValue ? (userId1.HasValue ? (userId1.GetValueOrDefault() == userId2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && string.Equals(this.VSExeVersion, telemetrySessionSettings.VSExeVersion, StringComparison.Ordinal) && string.Equals(this.SkuName, telemetrySessionSettings.SkuName, StringComparison.OrdinalIgnoreCase) && TelemetrySessionSettings.GetFaultEventBucketFilterJson(this.BucketFiltersToEnableWatsonForFaults) == TelemetrySessionSettings.GetFaultEventBucketFilterJson(telemetrySessionSettings.BucketFiltersToEnableWatsonForFaults) && TelemetrySessionSettings.GetFaultEventBucketFilterJson(this.BucketFiltersToAddDumpsToFaults) == TelemetrySessionSettings.GetFaultEventBucketFilterJson(telemetrySessionSettings.BucketFiltersToAddDumpsToFaults);
    }

    public override string ToString()
    {
      string bucketFilterJson1 = TelemetrySessionSettings.GetFaultEventBucketFilterJson(this.BucketFiltersToEnableWatsonForFaults);
      string bucketFilterJson2 = TelemetrySessionSettings.GetFaultEventBucketFilterJson(this.BucketFiltersToAddDumpsToFaults);
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, string> keyValuePair in new Dictionary<string, string>()
      {
        {
          "IsOptedIn",
          this.IsOptedIn.ToString().ToLowerInvariant()
        },
        {
          "HostName",
          TelemetrySessionSettings.StringToJsonValue(this.HostName)
        },
        {
          "AppInsightsInstrumentationKey",
          TelemetrySessionSettings.StringToJsonValue(this.AppInsightsInstrumentationKey)
        },
        {
          "AsimovInstrumentationKey",
          TelemetrySessionSettings.StringToJsonValue(this.AsimovInstrumentationKey)
        },
        {
          "CollectorApiKey",
          TelemetrySessionSettings.StringToJsonValue(this.CollectorApiKey)
        },
        {
          "AppId",
          this.AppId.ToString()
        },
        {
          "UserId",
          TelemetrySessionSettings.StringToJsonValue(this.UserId.ToString())
        },
        {
          "Id",
          TelemetrySessionSettings.StringToJsonValue(this.Id)
        },
        {
          "ProcessStartTime",
          this.ProcessStartTime.ToString()
        },
        {
          "SkuName",
          TelemetrySessionSettings.StringToJsonValue(this.SkuName)
        },
        {
          "VSExeVersion",
          TelemetrySessionSettings.StringToJsonValue(this.VSExeVersion)
        }
      })
        stringBuilder.AppendFormat("\"{0}\":{1},", (object) keyValuePair.Key, (object) keyValuePair.Value);
      string str1 = stringBuilder.ToString().TrimEnd(',');
      string str2 = str1 + ",\"BucketFiltersToEnableWatsonForFaults\":" + bucketFilterJson1 + ",\"BucketFiltersToAddDumpsToFaults\":" + bucketFilterJson2;
      return str2.Length > 28672 ? "{" + str1 + "}" : "{" + str2 + "}";
    }

    internal static string GetFaultEventBucketFilterJson(List<BucketFilter> bucketFilters)
    {
      if (bucketFilters == null)
        return "[]";
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('[');
      for (int index1 = 0; index1 < bucketFilters.Count; ++index1)
      {
        BucketFilter bucketFilter = bucketFilters[index1];
        stringBuilder.Append('{');
        stringBuilder.Append("\"AdditionalProperties\":[");
        int count = bucketFilter.AdditionalProperties.Count;
        int num = 0;
        foreach (KeyValuePair<string, string> additionalProperty in bucketFilter.AdditionalProperties)
        {
          stringBuilder.Append("{\"Key\":\"" + additionalProperty.Key + "\",\"Value\":\"" + additionalProperty.Value + "\"}");
          if (num++ < count - 1)
            stringBuilder.Append(',');
        }
        stringBuilder.Append("],");
        stringBuilder.Append(string.Format("\"Id\":\"{0}\",", (object) bucketFilter.Id));
        stringBuilder.Append("\"WatsonEventType\":\"" + bucketFilter.WatsonEventType + "\",");
        stringBuilder.Append("\"BucketParameterFilters\":[");
        for (int index2 = 0; index2 < bucketFilter.BucketParameterFilters.Length; ++index2)
        {
          string bucketParameterFilter = bucketFilter.BucketParameterFilters[index2];
          if (bucketParameterFilter == null)
            stringBuilder.Append("null");
          else
            stringBuilder.Append(JsonConvert.ToString(bucketParameterFilter) ?? "");
          if (index2 < bucketFilter.BucketParameterFilters.Length - 1)
            stringBuilder.Append(',');
        }
        stringBuilder.Append(']');
        stringBuilder.Append('}');
        if (index1 < bucketFilters.Count - 1)
          stringBuilder.Append(',');
      }
      stringBuilder.Append(']');
      return stringBuilder.ToString();
    }

    public override int GetHashCode() => ((((((((((((-1285232646 * -1521134295 + this.IsOptedIn.GetHashCode()) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.HostName)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.AppInsightsInstrumentationKey)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.AsimovInstrumentationKey)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.CollectorApiKey)) * -1521134295 + this.AppId.GetHashCode()) * -1521134295 + EqualityComparer<Guid?>.Default.GetHashCode(this.UserId)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Id)) * -1521134295 + this.ProcessStartTime.GetHashCode()) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.SkuName)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.VSExeVersion)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TelemetrySessionSettings.GetFaultEventBucketFilterJson(this.BucketFiltersToEnableWatsonForFaults))) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TelemetrySessionSettings.GetFaultEventBucketFilterJson(this.BucketFiltersToAddDumpsToFaults));

    private static string StringToJsonValue(string value) => value == null ? "null" : "\"" + value.Replace(' ', '_') + "\"";

    private static TelemetrySessionSettings Deserialize(string settings)
    {
      using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(settings)))
        return (TelemetrySessionSettings) new DataContractJsonSerializer(typeof (TelemetrySessionSettings)).ReadObject((Stream) memoryStream);
    }

    private static bool IsSerializedSessionValid(string serializedSession) => !serializedSession.IsNullOrWhiteSpace();
  }
}
