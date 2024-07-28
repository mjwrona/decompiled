// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SchemaDiscoveryPolicy
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.Azure.Documents
{
  internal sealed class SchemaDiscoveryPolicy : JsonSerializable, ICloneable
  {
    public SchemaDiscoveryPolicy() => this.SchemaBuilderMode = SchemaBuilderMode.None;

    [JsonProperty(PropertyName = "mode")]
    [JsonConverter(typeof (StringEnumConverter))]
    public SchemaBuilderMode SchemaBuilderMode
    {
      get
      {
        SchemaBuilderMode schemaBuilderMode = SchemaBuilderMode.Lazy;
        string str = this.GetValue<string>("mode");
        if (!string.IsNullOrEmpty(str))
          schemaBuilderMode = (SchemaBuilderMode) Enum.Parse(typeof (SchemaBuilderMode), str, true);
        return schemaBuilderMode;
      }
      set => this.SetValue("mode", (object) value.ToString());
    }

    public object Clone() => (object) new SchemaDiscoveryPolicy();

    internal override void Validate()
    {
      base.Validate();
      this.GetValue<string>("mode");
    }
  }
}
