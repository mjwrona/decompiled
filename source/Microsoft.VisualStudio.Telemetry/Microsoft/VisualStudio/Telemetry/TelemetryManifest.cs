// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifest
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifest
  {
    public const uint FormatVersion = 2;
    [JsonIgnore]
    private readonly HashSet<string> invalidRules = new HashSet<string>();

    [JsonProperty(PropertyName = "version", Required = Required.Always)]
    public string Version { get; set; }

    [DefaultValue(false)]
    [JsonProperty(PropertyName = "useCollector", Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool UseCollector { get; set; }

    [JsonProperty(PropertyName = "etag")]
    public string Etag { get; set; }

    [JsonProperty(PropertyName = "throttlingThresholdPerWindow", Required = Required.Default)]
    public long ThrottlingThreshold { get; set; }

    [JsonProperty(PropertyName = "throttlingTimerResetInSeconds", Required = Required.Default)]
    public double ThrottlingTimerReset { get; set; }

    [DefaultValue(true)]
    [JsonProperty(PropertyName = "shouldSendAliasForAllInternalUsers", Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool ShouldSendAliasForAllInternalUsers { get; set; }

    [JsonIgnore]
    public IEnumerable<string> InvalidRules => (IEnumerable<string>) this.invalidRules;

    [JsonIgnore]
    public int InvalidActionCount { get; private set; }

    [JsonProperty(PropertyName = "rules")]
    public IEnumerable<TelemetryManifestRule> Rules { get; set; }

    [JsonProperty(PropertyName = "machineIdentificationConfig")]
    public TelemetryManifestMachineIdentityConfig MachineIdentityConfig { get; set; }

    public static TelemetryManifest BuildDefaultManifest() => new TelemetryManifest()
    {
      Version = "default",
      UseCollector = false,
      ShouldSendAliasForAllInternalUsers = true
    };

    public IEnumerable<ITelemetryManifestAction> GetActionsForEvent(TelemetryEvent telemetryEvent)
    {
      telemetryEvent.RequiresArgumentNotNull<TelemetryEvent>(nameof (telemetryEvent));
      List<ITelemetryManifestAction> actionsForEvent = new List<ITelemetryManifestAction>();
      foreach (TelemetryManifestRule telemetryManifestRule in this.Rules.EmptyIfNull<TelemetryManifestRule>())
      {
        if (telemetryManifestRule.When.IsEventMatch(telemetryEvent))
          actionsForEvent.AddRange(telemetryManifestRule.Actions);
      }
      return (IEnumerable<ITelemetryManifestAction>) actionsForEvent;
    }

    public void Validate()
    {
      this.invalidRules.Clear();
      this.InvalidActionCount = 0;
      if (this.Version.IsNullOrWhiteSpace())
        throw new TelemetryManifestValidationException("'version' must be valid non-empty string, represented version");
      this.Rules = (IEnumerable<TelemetryManifestRule>) this.Rules.EmptyIfNull<TelemetryManifestRule>().Where<TelemetryManifestRule>((Func<TelemetryManifestRule, bool>) (rule =>
      {
        try
        {
          rule.Validate();
          return true;
        }
        catch (TelemetryManifestValidationException ex)
        {
          if (!string.IsNullOrEmpty(rule.Name))
            this.invalidRules.Add(rule.Name);
        }
        finally
        {
          this.InvalidActionCount += rule.InvalidActionCount;
        }
        return false;
      })).ToList<TelemetryManifestRule>();
    }

    internal string CalculateAllSamplings(TelemetrySession session)
    {
      StringBuilder stringBuilder = new StringBuilder("[");
      foreach (TelemetryManifestRule telemetryManifestRule in this.Rules.EmptyIfNull<TelemetryManifestRule>())
        stringBuilder.AppendFormat("{0}, ", (object) telemetryManifestRule.CalculateAllSamplings(session));
      stringBuilder.Append("]");
      return stringBuilder.ToString();
    }

    internal IEnumerable<TelemetryManifestMatchSampling.Path> GetAllSamplings() => this.Rules.EmptyIfNull<TelemetryManifestRule>().SelectMany<TelemetryManifestRule, TelemetryManifestMatchSampling.Path>((Func<TelemetryManifestRule, IEnumerable<TelemetryManifestMatchSampling.Path>>) (rule => rule.GetAllSamplings().Select<TelemetryManifestMatchSampling, TelemetryManifestMatchSampling.Path>((Func<TelemetryManifestMatchSampling, TelemetryManifestMatchSampling.Path>) (sample => new TelemetryManifestMatchSampling.Path(rule, sample)))));

    [OnDeserialized]
    internal void ValidateAfterDeserialization(StreamingContext context) => this.Validate();
  }
}
