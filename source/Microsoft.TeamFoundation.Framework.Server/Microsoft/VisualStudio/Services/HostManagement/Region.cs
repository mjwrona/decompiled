// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostManagement.Region
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.HostManagement
{
  [DataContract]
  public class Region
  {
    [DataMember]
    public string AzureRegion { get; set; }

    [DataMember]
    public string DisplayName { get; set; }

    [DataMember]
    public string CommerceId { get; set; }

    [DataMember]
    public RegionStatus Status { get; set; }

    [DataMember]
    public bool IsDefault { get; set; }

    [DataMember]
    public string GeographyCode { get; set; }

    [DataMember]
    public string GeographyName { get; set; }

    public Region()
    {
    }

    public Region(Region region)
    {
      this.AzureRegion = region.AzureRegion;
      this.Status = region.Status;
      this.IsDefault = region.IsDefault;
      this.CommerceId = region.CommerceId;
      this.DisplayName = region.DisplayName;
      this.GeographyCode = region.GeographyCode;
      this.GeographyName = region.GeographyName;
    }

    public Region Clone() => new Region(this);

    public Microsoft.VisualStudio.Services.Commerce.AzureRegion ToAzureRegion() => new Microsoft.VisualStudio.Services.Commerce.AzureRegion()
    {
      DisplayName = this.DisplayName,
      RegionCode = this.AzureRegion,
      Id = this.CommerceId
    };
  }
}
