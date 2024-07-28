// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionOption.AcquisitionOptions
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionOption
{
  public class AcquisitionOptions
  {
    public string ItemId { get; set; }

    public string Target { get; set; }

    public List<AcquisitionOperation> Operations { get; set; }

    public AcquisitionOperation DefaultOperation { get; set; }
  }
}
