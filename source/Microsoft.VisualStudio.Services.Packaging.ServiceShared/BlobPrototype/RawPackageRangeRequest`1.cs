// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.RawPackageRangeRequest`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class RawPackageRangeRequest<T> : RawPackageRangeRequest where T : class
  {
    public T AdditionalData { get; }

    public RawPackageRangeRequest(
      IFeedRequest feedRequest,
      string packageName,
      string packageVersionLower,
      string packageVersionUpper,
      HttpRequestMessage httpRequest,
      T data)
      : base(feedRequest, packageName, packageVersionLower, packageVersionUpper, httpRequest)
    {
      this.AdditionalData = data ?? throw new ArgumentNullException(nameof (data));
    }

    public RawPackageRangeRequest(RawPackageRangeRequest originalRequest, T data)
      : this((IFeedRequest) originalRequest, originalRequest.PackageName, originalRequest.PackageVersionLower, originalRequest.PackageVersionUpper, originalRequest.HttpRequest, data)
    {
    }
  }
}
