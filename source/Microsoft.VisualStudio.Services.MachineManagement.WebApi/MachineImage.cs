// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineImage
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public sealed class MachineImage : ICloneable
  {
    private PropertiesCollection m_properties;

    internal MachineImage()
    {
    }

    private MachineImage(MachineImage cloneFrom)
    {
      this.Description = cloneFrom.Description;
      this.ImageId = cloneFrom.ImageId;
      this.ImageMetadataFileId = cloneFrom.ImageMetadataFileId;
      this.ImageName = cloneFrom.ImageName;
      this.ImageOffer = cloneFrom.ImageOffer;
      this.ImagePublisher = cloneFrom.ImagePublisher;
      this.ImageSku = cloneFrom.ImageSku;
      this.ImageType = cloneFrom.ImageType;
      this.ImageVersion = cloneFrom.ImageVersion;
      this.OperatingSystem = cloneFrom.OperatingSystem;
      if (cloneFrom.m_properties == null)
        return;
      this.m_properties = new PropertiesCollection((IDictionary<string, object>) cloneFrom.m_properties);
    }

    [DataMember(IsRequired = true)]
    internal int ImageId { get; set; }

    [DataMember(IsRequired = true)]
    public string ImageName { get; set; }

    [DataMember(IsRequired = true)]
    public string ImageVersion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ImageType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ImagePublisher { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ImageOffer { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ImageSku { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    internal int? ImageMetadataFileId { get; set; }

    [DataMember(IsRequired = false)]
    public string Description { get; set; }

    [DataMember(IsRequired = true)]
    public string OperatingSystem { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public PropertiesCollection Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = new PropertiesCollection();
        return this.m_properties;
      }
      internal set => this.m_properties = value;
    }

    public MachineImage Clone() => new MachineImage(this);

    object ICloneable.Clone() => (object) this.Clone();

    public bool IsLinuxImage() => !string.IsNullOrEmpty(this.OperatingSystem) && this.OperatingSystem.Equals("Linux", StringComparison.OrdinalIgnoreCase);

    public bool IsMacImage() => !string.IsNullOrEmpty(this.OperatingSystem) && this.OperatingSystem.Equals("MacOS", StringComparison.OrdinalIgnoreCase);

    public bool IsGalleryImage() => !string.IsNullOrEmpty(this.ImageType) && this.ImageType.Equals("Gallery", StringComparison.OrdinalIgnoreCase);
  }
}
