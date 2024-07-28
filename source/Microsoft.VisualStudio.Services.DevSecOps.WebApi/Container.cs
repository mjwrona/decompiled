// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.WebApi.Container
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78BC9F0A-6262-4C96-81AF-14E523464B20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.WebApi.dll

using System;
using System.Collections.Generic;
using System.IO.Compression;

namespace Microsoft.VisualStudio.Services.DevSecOps.WebApi
{
  public class Container
  {
    public Container(ContainerMetadata metadata, Guid? correlationId)
    {
      this.Metadata = metadata;
      this.CorrelationId = correlationId;
      this.ScanTargets = (IList<ScanTarget>) new List<ScanTarget>();
    }

    public Container(ContainerMetadata metadata, Guid? correlationId, ZipArchive zipArchive)
    {
      this.Metadata = metadata;
      this.ZipArchive = zipArchive;
      this.CorrelationId = correlationId;
    }

    public ContainerMetadata Metadata { get; }

    public Guid? CorrelationId { get; }

    public IList<ScanTarget> ScanTargets { get; }

    public ZipArchive ZipArchive { get; internal set; }
  }
}
