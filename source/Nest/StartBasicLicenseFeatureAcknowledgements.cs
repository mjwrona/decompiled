// Decompiled with JetBrains decompiler
// Type: Nest.StartBasicLicenseFeatureAcknowledgements
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Nest
{
  [JsonFormatter(typeof (StartBasicLicenseFeatureAcknowledgementsFormatter))]
  public class StartBasicLicenseFeatureAcknowledgements : ReadOnlyDictionary<string, string[]>
  {
    internal StartBasicLicenseFeatureAcknowledgements(IDictionary<string, string[]> dictionary)
      : base(dictionary)
    {
    }

    [DataMember(Name = "message")]
    public string Message { get; internal set; }
  }
}
