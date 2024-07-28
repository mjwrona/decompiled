// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.QueryItem
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.Documents.Query
{
  internal sealed class QueryItem
  {
    private static readonly JsonSerializerSettings NoDateParseHandlingJsonSerializerSettings = new JsonSerializerSettings()
    {
      DateParseHandling = DateParseHandling.None
    };
    private bool isItemDeserialized;
    private object item;

    [JsonProperty("item")]
    private JRaw RawItem { get; set; }

    public object GetItem()
    {
      if (!this.isItemDeserialized)
      {
        this.item = this.RawItem != null ? JsonConvert.DeserializeObject((string) this.RawItem.Value, QueryItem.NoDateParseHandlingJsonSerializerSettings) : (object) Undefined.Value;
        this.isItemDeserialized = true;
      }
      return this.item;
    }
  }
}
