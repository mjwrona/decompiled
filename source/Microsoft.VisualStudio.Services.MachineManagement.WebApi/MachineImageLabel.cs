// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineImageLabel
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public sealed class MachineImageLabel : ICloneable
  {
    internal MachineImageLabel()
    {
    }

    public MachineImageLabel(
      string imageLabel,
      string imageName,
      string currentImageVersion = null,
      string previousImageVersion = null,
      int? imageLabelMetadataFileId = null,
      bool? isVisible = null)
    {
      this.ImageLabel = imageLabel;
      this.ImageName = imageName;
      this.CurrentImageVersion = currentImageVersion;
      this.PreviousImageVersion = previousImageVersion;
      this.ImageLabelMetadataFileId = imageLabelMetadataFileId;
      this.IsVisible = isVisible.GetValueOrDefault();
    }

    private MachineImageLabel(MachineImageLabel cloneFrom)
    {
      this.ImageLabelId = cloneFrom.ImageLabelId;
      this.ImageLabel = cloneFrom.ImageLabel;
      this.ImageName = cloneFrom.ImageName;
      this.CurrentImageVersion = cloneFrom.CurrentImageVersion;
      this.PreviousImageVersion = cloneFrom.PreviousImageVersion;
      this.ImageLabelMetadataFileId = cloneFrom.ImageLabelMetadataFileId;
      this.IsVisible = cloneFrom.IsVisible;
    }

    [DataMember(IsRequired = true)]
    internal int ImageLabelId { get; set; }

    [DataMember(IsRequired = true)]
    public string ImageLabel { get; set; }

    [DataMember(IsRequired = true)]
    public string ImageName { get; set; }

    [DataMember(IsRequired = false)]
    public string CurrentImageVersion { get; set; }

    [DataMember(IsRequired = false)]
    public string PreviousImageVersion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    internal int? ImageLabelMetadataFileId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    internal bool IsVisible { get; set; }

    public MachineImageLabel Clone() => new MachineImageLabel(this);

    object ICloneable.Clone() => (object) this.Clone();
  }
}
