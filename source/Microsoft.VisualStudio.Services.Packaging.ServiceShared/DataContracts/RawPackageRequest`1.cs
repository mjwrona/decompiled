// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.RawPackageRequest`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts
{
  public class RawPackageRequest<T> : 
    RawPackageRequest,
    IRawPackageRequest<T>,
    IRawPackageRequest,
    IRawPackageNameRequest,
    IFeedRequest,
    IProtocolAgnosticFeedRequest,
    IRawPackageNameRequest<T>,
    IFeedRequest<T>
    where T : class
  {
    public RawPackageRequest(IRawPackageRequest originalRequest, T data)
      : base((IFeedRequest) originalRequest, originalRequest.PackageName, originalRequest.PackageVersion)
    {
      this.AdditionalData = data ?? throw new ArgumentNullException(nameof (data));
    }

    public RawPackageRequest(
      IFeedRequest feedRequest,
      string packageName,
      string packageVersion,
      T data)
      : base(feedRequest, packageName, packageVersion)
    {
      data.ThrowIfNull<T>((Func<Exception>) (() =>
      {
        throw new InvalidUserRequestException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_ArgumentRequired((object) nameof (data)));
      }));
      this.AdditionalData = data;
    }

    public T AdditionalData { get; }
  }
}
