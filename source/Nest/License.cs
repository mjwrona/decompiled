// Decompiled with JetBrains decompiler
// Type: Nest.License
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.IO;
using System.Runtime.Serialization;

namespace Nest
{
  public class License
  {
    [DataMember(Name = "expiry_date_in_millis")]
    public long ExpiryDateInMilliseconds { get; set; }

    [DataMember(Name = "issue_date_in_millis")]
    public long IssueDateInMilliseconds { get; set; }

    [DataMember(Name = "issued_to")]
    public string IssuedTo { get; set; }

    [DataMember(Name = "issuer")]
    public string Issuer { get; set; }

    [DataMember(Name = "max_nodes")]
    public long MaxNodes { get; set; }

    [DataMember(Name = "signature")]
    public string Signature { get; set; }

    [DataMember(Name = "type")]
    public LicenseType Type { get; set; }

    [DataMember(Name = "uid")]
    public string UID { get; set; }

    public static License LoadFromDisk(string path) => JsonSerializer.Deserialize<License.Wrapped>(File.ReadAllText(path))?.License;

    private class Wrapped
    {
      [DataMember(Name = "license")]
      public License License { get; set; }
    }
  }
}
