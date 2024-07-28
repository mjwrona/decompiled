// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BlobCopyRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Cloud.BlobWrappers;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class BlobCopyRequest
  {
    public BlobCopyRequest(
      ICloudBlobContainerWrapper sourceContainer,
      ICloudBlobWrapper sourceBlob,
      ICloudBlobWrapper targetBlob)
    {
      this.SourceContainer = sourceContainer;
      this.SourceBlob = sourceBlob;
      this.TargetBlob = targetBlob;
    }

    public ICloudBlobContainerWrapper SourceContainer { get; private set; }

    public ICloudBlobWrapper SourceBlob { get; private set; }

    public ICloudBlobWrapper TargetBlob { get; private set; }

    public Uri SourceUri => this.SourceContainer.GetUriWithCredentials(this.SourceBlob);
  }
}
