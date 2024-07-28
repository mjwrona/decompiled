// Decompiled with JetBrains decompiler
// Type: Nest.LicenseInformation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class LicenseInformation
  {
    [DataMember(Name = "expiry_date")]
    public DateTime ExpiryDate { get; internal set; }

    [DataMember(Name = "expiry_date_in_millis")]
    public long ExpiryDateInMilliseconds { get; internal set; }

    [DataMember(Name = "issue_date")]
    public DateTime IssueDate { get; internal set; }

    [DataMember(Name = "issue_date_in_millis")]
    public long IssueDateInMilliseconds { get; internal set; }

    [DataMember(Name = "issued_to")]
    public string IssuedTo { get; internal set; }

    [DataMember(Name = "issuer")]
    public string Issuer { get; internal set; }

    [DataMember(Name = "max_nodes")]
    public long MaxNodes { get; internal set; }

    [DataMember(Name = "max_resource_units")]
    public int? MaxResourceUnits { get; internal set; }

    [DataMember(Name = "status")]
    public LicenseStatus Status { get; internal set; }

    [DataMember(Name = "type")]
    public LicenseType Type { get; internal set; }

    [DataMember(Name = "uid")]
    public string UID { get; internal set; }
  }
}
