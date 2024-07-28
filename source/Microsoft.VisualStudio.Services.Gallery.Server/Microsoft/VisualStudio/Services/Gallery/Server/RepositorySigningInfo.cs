// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.RepositorySigningInfo
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class RepositorySigningInfo
  {
    public string PublisherName { get; set; }

    public string ExtensionName { get; set; }

    public string Version { get; set; }

    public string TargetPlatform { get; set; }

    public Guid ValidationId { get; set; }

    public Guid ValidationStepId { get; set; }

    public RepositorySigningStepStatus Status { get; set; }

    public string SignatureManifestFilePath { get; set; }

    public string SignatureManifestFileUrl { get; set; }

    public string SignatureFilePath { get; set; }

    public string SignatureFileUrl { get; set; }

    public string PackageFilePath { get; set; }

    public string SignatureArchiveFilePath { get; set; }

    public EsrpOperationStatus OperationStatus { get; set; }

    public string OperationId { get; set; }

    public long FileSize { get; set; }

    public int RetryCount { get; set; }

    public bool IsRetry { get; set; }

    public bool IsFromExistingExtensionSigningJob { get; set; }
  }
}
