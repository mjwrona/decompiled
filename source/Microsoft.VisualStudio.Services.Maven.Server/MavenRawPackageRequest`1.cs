// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenRawPackageRequest`1
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenRawPackageRequest<T> : RawPackageRequest<T> where T : class
  {
    public MavenRawPackageRequest(
      IFeedRequest feed,
      string groupId,
      string artifactId,
      string packageVersion,
      T data)
      : this(new MavenRawPackageRequest(feed, groupId, artifactId, packageVersion), data)
    {
    }

    public MavenRawPackageRequest(MavenRawPackageRequest request, T data)
      : base((IRawPackageRequest) request, data)
    {
    }

    public static IConverter<MavenRawPackageRequest, MavenRawPackageRequest<T>> GetConverterFrom(
      T data)
    {
      return (IConverter<MavenRawPackageRequest, MavenRawPackageRequest<T>>) new ByFuncConverter<MavenRawPackageRequest, MavenRawPackageRequest<T>>((Func<MavenRawPackageRequest, MavenRawPackageRequest<T>>) (r => new MavenRawPackageRequest<T>(r, data)));
    }
  }
}
