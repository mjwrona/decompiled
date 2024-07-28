// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.AggregateItem
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.Azure.Documents.Query
{
  internal sealed class AggregateItem
  {
    private static readonly JsonSerializerSettings NoDateParseHandlingJsonSerializerSettings = new JsonSerializerSettings()
    {
      DateParseHandling = DateParseHandling.None
    };
    private readonly Lazy<object> item;

    [JsonProperty("item")]
    private JRaw RawItem { get; set; }

    [JsonProperty("item2")]
    private JRaw RawItem2 { get; set; }

    public AggregateItem(JRaw rawItem, JRaw rawItem2)
    {
      this.RawItem = rawItem;
      this.RawItem2 = rawItem2;
      this.item = new Lazy<object>(new Func<object>(this.InitLazy));
    }

    private object InitLazy()
    {
      object obj = this.RawItem != null ? JsonConvert.DeserializeObject((string) this.RawItem.Value, AggregateItem.NoDateParseHandlingJsonSerializerSettings) : (object) Undefined.Value;
      if (this.RawItem2 != null)
        obj = JsonConvert.DeserializeObject((string) this.RawItem2.Value, AggregateItem.NoDateParseHandlingJsonSerializerSettings);
      return obj;
    }

    public object GetItem() => this.item.Value;
  }
}
