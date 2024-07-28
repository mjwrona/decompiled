// Decompiled with JetBrains decompiler
// Type: Nest.IDateIndexNameProcessor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface IDateIndexNameProcessor : IProcessor
  {
    [DataMember(Name = "date_formats")]
    IEnumerable<string> DateFormats { get; set; }

    [DataMember(Name = "date_rounding")]
    Nest.DateRounding? DateRounding { get; set; }

    [DataMember(Name = "field")]
    Field Field { get; set; }

    [DataMember(Name = "index_name_format")]
    string IndexNameFormat { get; set; }

    [DataMember(Name = "index_name_prefix")]
    string IndexNamePrefix { get; set; }

    [DataMember(Name = "locale")]
    string Locale { get; set; }

    [DataMember(Name = "timezone")]
    string TimeZone { get; set; }
  }
}
