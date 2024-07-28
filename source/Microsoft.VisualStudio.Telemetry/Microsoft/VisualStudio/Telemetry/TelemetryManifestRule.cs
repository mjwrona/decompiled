// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestRule
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestRule
  {
    [JsonProperty(PropertyName = "name", Required = Required.Always)]
    public string Name { get; set; }

    [JsonIgnore]
    public int InvalidActionCount { get; private set; }

    [JsonProperty(PropertyName = "when", Required = Required.Always)]
    public ITelemetryManifestMatch When { get; set; }

    [JsonProperty(PropertyName = "do", Required = Required.Always)]
    public IEnumerable<ITelemetryManifestAction> Actions { get; set; }

    public IEnumerable<TelemetryManifestMatchSampling> GetAllSamplings() => this.When.GetDescendantsAndItself().OfType<TelemetryManifestMatchSampling>();

    public string CalculateAllSamplings(TelemetrySession session)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("{0}{{", (object) this.Name);
      foreach (TelemetryManifestMatchSampling allSampling in this.GetAllSamplings())
      {
        allSampling.CalculateIsSampleActive(this, session);
        stringBuilder.AppendFormat("{0}, ", (object) allSampling.GetFullName(this));
      }
      stringBuilder.Append("}");
      return stringBuilder.ToString();
    }

    public virtual void Validate()
    {
      this.InvalidActionCount = 0;
      if (string.IsNullOrEmpty(this.Name))
        throw new TelemetryManifestValidationException("'name' must be non-null valid string");
      if (this.When == null)
        throw new TelemetryManifestValidationException("'when' must be non-null valid matching rule");
      if (this.Actions == null)
        throw new TelemetryManifestValidationException("'do' must be non-null valid array of the rules");
      this.When.Validate();
      this.Actions = (IEnumerable<ITelemetryManifestAction>) this.Actions.Where<ITelemetryManifestAction>((Func<ITelemetryManifestAction, bool>) (action =>
      {
        try
        {
          action.Validate();
          return true;
        }
        catch (TelemetryManifestValidationException ex)
        {
          ++this.InvalidActionCount;
        }
        return false;
      })).ToList<ITelemetryManifestAction>();
      if (!this.Actions.Any<ITelemetryManifestAction>())
        throw new TelemetryManifestValidationException("'do' must have at least 1 action");
    }
  }
}
