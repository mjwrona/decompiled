// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.RefreshPackageFailure
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  [DataContract]
  public class RefreshPackageFailure
  {
    public RefreshPackageFailure(
      string packageName,
      UpstreamStatusCategory category,
      Exception exception)
    {
      this.PackageName = packageName;
      this.Category = category;
      this.Exception = exception;
    }

    [DataMember]
    public string PackageName { get; }

    [DataMember]
    public UpstreamStatusCategory Category { get; }

    [IgnoreDataMember]
    public Exception Exception { get; }

    [DataMember]
    public string ExceptionType => this.Exception?.GetType().Name;

    [IgnoreDataMember]
    public string ExceptionMessage => this.Exception == null ? (string) null : StackTraceCompressor.CompressStackTrace(this.Exception.ToString());
  }
}
