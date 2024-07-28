// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.IBlobCopyContext
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.BlobWrappers;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public interface IBlobCopyContext
  {
    int Compare(
      IVssRequestContext requestContext,
      ICloudBlobReadOnlyInfo sourceBlob,
      ICloudBlobReadOnlyInfo targetBlob);

    void OnSourceAndTarget(
      IVssRequestContext requestContext,
      ICloudBlobWrapper sourceBlob,
      ICloudBlobWrapper targetBlob);

    void OnSourceOnly(IVssRequestContext requestContext, ICloudBlobWrapper sourceBlob);

    void OnTargetOnly(IVssRequestContext requestContext, ICloudBlobWrapper targetBlob);

    void StartIteration(
      IVssRequestContext requestContext,
      ICloudBlobContainerWrapper sourceContainer,
      ICloudBlobContainerWrapper targetContainer,
      ICloudBlobWrapper sourceBlob,
      ICloudBlobWrapper targetBlob);
  }
}
