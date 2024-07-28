// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.MaterializedViews
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Documents
{
  internal sealed class MaterializedViews : JsonSerializable, ICloneable
  {
    [JsonProperty(PropertyName = "id")]
    public string Id
    {
      get => this.GetValue<string>("id");
      set => this.SetValue("id", (object) value);
    }

    [JsonProperty(PropertyName = "_rid")]
    public string Rid
    {
      get => this.GetValue<string>("_rid");
      set => this.SetValue("_rid", (object) value);
    }

    public object Clone() => (object) new MaterializedViews()
    {
      Id = this.Id,
      Rid = this.Rid
    };
  }
}
