// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestMatchSampling
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestMatchSampling : ITelemetryManifestMatch, ITelemetryEventMatch
  {
    [JsonProperty(PropertyName = "flightName")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "samplingRate", Required = Required.Always)]
    public double Rate { get; set; }

    [JsonProperty(PropertyName = "samplingInputs", Required = Required.Always, ItemConverterType = typeof (StringEnumConverter))]
    public IEnumerable<HashInput> Inputs { get; set; }

    [JsonIgnore]
    public bool IsSampleActive { get; set; }

    [JsonIgnore]
    public bool IsCalculateCalled { get; private set; }

    [JsonIgnore]
    public bool IsRateTooLow { get; private set; }

    [JsonIgnore]
    public bool IsRateTooHigh { get; private set; }

    [JsonIgnore]
    public bool IsInputStringEmpty { get; private set; }

    [JsonIgnore]
    public string InputString { get; private set; }

    [JsonIgnore]
    public ulong Hash { get; private set; }

    public bool IsEventMatch(TelemetryEvent telemetryEvent) => this.IsSampleActive;

    public IEnumerable<ITelemetryManifestMatch> GetChildren() => Enumerable.Empty<ITelemetryManifestMatch>();

    public string GetFullName(TelemetryManifestRule rule) => rule.Name + (string.IsNullOrEmpty(this.Name) ? string.Empty : "." + this.Name);

    public void CalculateIsSampleActive(TelemetryManifestRule rule, TelemetrySession session)
    {
      this.IsCalculateCalled = true;
      if (this.Rate <= 0.0)
      {
        this.IsSampleActive = false;
        this.IsRateTooLow = true;
      }
      else if (this.Rate >= 1.0)
      {
        this.IsSampleActive = true;
        this.IsRateTooHigh = true;
      }
      else
      {
        string s = ((IEnumerable<Tuple<HashInput, string>>) new Tuple<HashInput, string>[5]
        {
          Tuple.Create<HashInput, string>(HashInput.MachineId, session.MachineId.ToString()),
          Tuple.Create<HashInput, string>(HashInput.UserId, session.UserId.ToString()),
          Tuple.Create<HashInput, string>(HashInput.RuleId, rule.Name),
          Tuple.Create<HashInput, string>(HashInput.SamplingId, this.GetFullName(rule)),
          Tuple.Create<HashInput, string>(HashInput.SessionId, session.SessionId)
        }).Where<Tuple<HashInput, string>>((Func<Tuple<HashInput, string>, bool>) (t => this.Inputs.Contains<HashInput>(t.Item1))).Select<Tuple<HashInput, string>, string>((Func<Tuple<HashInput, string>, string>) (t => t.Item2)).Join(".");
        if (string.IsNullOrEmpty(s))
        {
          this.IsSampleActive = false;
          this.IsInputStringEmpty = true;
        }
        else
        {
          this.InputString = s;
          byte[] bytes = Encoding.UTF8.GetBytes(s);
          ulong uint64 = BitConverter.ToUInt64(FipsCompliantSha.Sha256.Value.ComputeHash(bytes), 0);
          this.Hash = uint64;
          this.IsSampleActive = (double) uint64 <= this.Rate * 1.8446744073709552E+19;
        }
      }
    }

    void ITelemetryManifestMatch.ValidateItself()
    {
      if (!this.Inputs.Any<HashInput>())
        throw new TelemetryManifestValidationException("'inputs' must have at least one input");
      if (this.Rate < 0.0 || this.Rate > 1.0)
        throw new TelemetryManifestValidationException("'rate' must be a number between 0 and 1");
    }

    public sealed class Path
    {
      public readonly TelemetryManifestRule Rule;
      public readonly TelemetryManifestMatchSampling Sampling;

      public string FullName => this.Sampling.GetFullName(this.Rule);

      public Path(TelemetryManifestRule rule, TelemetryManifestMatchSampling sampling)
      {
        this.Rule = rule;
        this.Sampling = sampling;
      }
    }
  }
}
