// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.RawPackageNameRequest`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts
{
  public class RawPackageNameRequest<T> : 
    RawPackageNameRequest,
    IRawPackageNameRequest<T>,
    IRawPackageNameRequest,
    IFeedRequest,
    IProtocolAgnosticFeedRequest,
    IFeedRequest<T>
    where T : class
  {
    public RawPackageNameRequest(IFeedRequest originalRequest, string packageName, T data)
      : base(originalRequest, packageName)
    {
      this.AdditionalData = data ?? throw new ArgumentNullException(nameof (data));
    }

    public RawPackageNameRequest(RawPackageNameRequest originalRequest, T data)
      : this((IFeedRequest) originalRequest, originalRequest.PackageName, data)
    {
    }

    public T AdditionalData { get; }
  }
}
