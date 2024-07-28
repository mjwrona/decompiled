// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.MaterializedViewDefinition
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Documents
{
  internal sealed class MaterializedViewDefinition : JsonSerializable, ICloneable
  {
    [JsonProperty(PropertyName = "sourceCollectionRid")]
    public string SourceCollectionRid
    {
      get => this.GetValue<string>("sourceCollectionRid");
      set => this.SetValue("sourceCollectionRid", (object) value);
    }

    [JsonProperty(PropertyName = "definition")]
    public string Definition
    {
      get => this.GetValue<string>("definition");
      set => this.SetValue("definition", (object) value);
    }

    [JsonProperty(PropertyName = "apiSpecificDefinition", NullValueHandling = NullValueHandling.Ignore)]
    public string ApiSpecificDefinition
    {
      get => this.GetValue<string>("apiSpecificDefinition");
      set => this.SetValue("apiSpecificDefinition", (object) value);
    }

    public object Clone() => (object) new MaterializedViewDefinition()
    {
      SourceCollectionRid = this.SourceCollectionRid,
      Definition = this.Definition,
      ApiSpecificDefinition = this.ApiSpecificDefinition
    };
  }
}
