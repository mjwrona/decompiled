// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CVS.ExtensionScanInfo
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server.CVS
{
  public class ExtensionScanInfo
  {
    public string PublisherName { get; set; }

    public string ExtensionId { get; set; }

    public string ExtensionName { get; set; }

    public string Version { get; set; }

    public Guid ScanId { get; set; }

    public long FileCount { get; set; }

    public DateTime StartTime { get; set; }

    public long? PendingRetryCount { get; set; }

    public bool? IsRetry { get; set; }

    public string UserPUID { get; set; }

    public Guid Id { get; set; }

    public DateTime? PublishedTime { get; set; }
  }
}
