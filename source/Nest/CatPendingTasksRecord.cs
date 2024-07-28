// Decompiled with JetBrains decompiler
// Type: Nest.CatPendingTasksRecord
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class CatPendingTasksRecord : ICatRecord
  {
    [DataMember(Name = "insertOrder")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    public int? InsertOrder { get; set; }

    [DataMember(Name = "priority")]
    public string Priority { get; set; }

    [DataMember(Name = "source")]
    public string Source { get; set; }

    [DataMember(Name = "timeInQueue")]
    public string TimeInQueue { get; set; }
  }
}
