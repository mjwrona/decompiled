// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.Client.Region
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Organization.Client
{
  [DataContract]
  public sealed class Region
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string DisplayName { get; set; }

    [DataMember]
    public string NameInAzure { get; set; }

    [DataMember]
    public bool IsDefault { get; set; }

    [DataMember]
    public Guid ServiceInstanceId { get; set; }

    [DataMember]
    public int AvailableHostsCount { get; set; }

    [DataMember]
    public RegionStatus RegionStatus { get; set; }

    [DataMember]
    public string GeographyCode { get; set; }

    [DataMember]
    public string GeographyName { get; set; }

    public Region()
    {
    }

    public Region(Region region)
    {
      this.Name = region.Name;
      this.DisplayName = region.DisplayName;
      this.IsDefault = region.IsDefault;
      this.ServiceInstanceId = region.ServiceInstanceId;
      this.AvailableHostsCount = region.AvailableHostsCount;
      this.RegionStatus = region.RegionStatus;
      this.GeographyCode = region.GeographyCode;
      this.GeographyName = region.GeographyName;
    }
  }
}
