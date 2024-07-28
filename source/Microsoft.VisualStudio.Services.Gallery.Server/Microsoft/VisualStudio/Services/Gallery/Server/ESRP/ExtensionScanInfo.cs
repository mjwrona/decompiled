// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ESRP.ExtensionScanInfo
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using MS.Ess.EsrpClient.Contracts.Common;
using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server.ESRP
{
  public class ExtensionScanInfo
  {
    public string PublisherName { get; set; }

    public string ExtensionName { get; set; }

    public string Version { get; set; }

    public string BlobFilePath { get; set; }

    public Guid OperationId { get; set; }

    public long FileSize { get; set; }

    public FileInfo FileInfo { get; set; }

    public long? RetryCount { get; set; }

    public bool? IsRetry { get; set; }
  }
}
