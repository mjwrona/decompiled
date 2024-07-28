// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.IBlobContinuationTokenStorageService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.BlobWrappers;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DefaultServiceImplementation(typeof (BlobContinuationTokenStorageService))]
  public interface IBlobContinuationTokenStorageService : IVssFrameworkService
  {
    long GetStorageFrequency(IVssRequestContext requestContext);

    void ClearContinuationToken(
      IVssRequestContext requestContext,
      string operationName,
      ICloudBlobContainerWrapper containerWrapper,
      Action<string> log = null);

    void SetContinuationToken(
      IVssRequestContext requestContext,
      string operationName,
      ICloudBlobContainerWrapper containerWrapper,
      BlobContinuationToken continuationToken,
      Action<string> log = null);

    BlobContinuationToken GetContinuationToken(
      IVssRequestContext requestContext,
      string operationName,
      ICloudBlobContainerWrapper containerWrapper,
      Action<string> log = null);

    int ClearAllContinuationTokens(
      IVssRequestContext requestContext,
      string operationName,
      Action<string> log = null);
  }
}
