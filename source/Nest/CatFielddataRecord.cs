// Decompiled with JetBrains decompiler
// Type: Nest.CatFielddataRecord
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  [JsonFormatter(typeof (CatFielddataRecordFormatter))]
  public class CatFielddataRecord : ICatRecord
  {
    public string Field { get; set; }

    public string Host { get; set; }

    public string Id { get; set; }

    public string Ip { get; set; }

    public string Node { get; set; }

    public string Size { get; set; }
  }
}
