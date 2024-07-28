// Decompiled with JetBrains decompiler
// Type: Nest.MinimalLicenseInformation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class MinimalLicenseInformation
  {
    [DataMember(Name = "expiry_date_in_millis")]
    public long ExpiryDateInMilliseconds { get; set; }

    [DataMember(Name = "mode")]
    public LicenseType Mode { get; internal set; }

    [DataMember(Name = "status")]
    public LicenseStatus Status { get; internal set; }

    [DataMember(Name = "type")]
    public LicenseType Type { get; internal set; }

    [DataMember(Name = "uid")]
    public string UID { get; internal set; }
  }
}
