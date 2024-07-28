// Decompiled with JetBrains decompiler
// Type: Nest.ISqlRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public interface ISqlRequest
  {
    [DataMember(Name = "fetch_size")]
    int? FetchSize { get; set; }

    [DataMember(Name = "filter")]
    QueryContainer Filter { get; set; }

    [DataMember(Name = "params")]
    IList<object> Params { get; set; }

    [DataMember(Name = "query")]
    string Query { get; set; }

    [DataMember(Name = "time_zone")]
    string TimeZone { get; set; }

    [DataMember(Name = "runtime_mappings")]
    IRuntimeFields RuntimeFields { get; set; }
  }
}
