// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ConflictResolutionPolicy
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.Azure.Documents
{
  internal sealed class ConflictResolutionPolicy : JsonSerializable
  {
    public ConflictResolutionPolicy() => this.Mode = ConflictResolutionMode.LastWriterWins;

    [JsonProperty(PropertyName = "mode")]
    [JsonConverter(typeof (StringEnumConverter))]
    public ConflictResolutionMode Mode
    {
      get
      {
        ConflictResolutionMode mode = ConflictResolutionMode.LastWriterWins;
        string str = this.GetValue<string>("mode");
        if (!string.IsNullOrEmpty(str))
          mode = (ConflictResolutionMode) Enum.Parse(typeof (ConflictResolutionMode), str, true);
        return mode;
      }
      set => this.SetValue("mode", (object) value.ToString());
    }

    [JsonProperty(PropertyName = "conflictResolutionPath")]
    public string ConflictResolutionPath
    {
      get => this.GetValue<string>("conflictResolutionPath");
      set => this.SetValue("conflictResolutionPath", (object) value);
    }

    [JsonProperty(PropertyName = "conflictResolutionProcedure")]
    public string ConflictResolutionProcedure
    {
      get => this.GetValue<string>("conflictResolutionProcedure");
      set => this.SetValue("conflictResolutionProcedure", (object) value);
    }

    internal override void Validate()
    {
      base.Validate();
      Helpers.ValidateEnumProperties<ConflictResolutionMode>(this.Mode);
      this.GetValue<string>("conflictResolutionPath");
      this.GetValue<string>("conflictResolutionProcedure");
    }
  }
}
