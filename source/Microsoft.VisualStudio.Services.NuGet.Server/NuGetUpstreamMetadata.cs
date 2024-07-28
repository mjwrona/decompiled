// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.NuGetUpstreamMetadata
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server
{
  [RequiredMember]
  public class NuGetUpstreamMetadata
  {
    public IStorageId? StorageId { get; init; }

    [RequiredMember]
    public IReadOnlyCollection<UpstreamSourceInfo> SourceChain { get; init; }

    [RequiredMember]
    public bool Listed { get; init; }

    [Obsolete("Constructors of types with required members are not supported in this version of your compiler.", true)]
    [CompilerFeatureRequired("RequiredMembers")]
    public NuGetUpstreamMetadata()
    {
    }
  }
}
