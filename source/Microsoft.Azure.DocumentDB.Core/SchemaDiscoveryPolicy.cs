// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SchemaDiscoveryPolicy
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
