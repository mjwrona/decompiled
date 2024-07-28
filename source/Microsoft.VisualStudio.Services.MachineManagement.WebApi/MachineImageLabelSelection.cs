// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineImageLabelSelection
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public class MachineImageLabelSelection
  {
    internal MachineImageLabelSelection()
    {
    }

    public MachineImageLabelSelection(string vmImage) => this.VMImage = vmImage;

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string VMImage { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ImageLabel { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<string> RequestTags { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string PerformanceMetrics { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ActionsBillingAndAbuseInformation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Region { get; set; }
  }
}
