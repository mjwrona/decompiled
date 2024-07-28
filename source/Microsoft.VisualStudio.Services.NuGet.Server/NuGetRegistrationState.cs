// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.NuGetRegistrationState
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories;
using System;
using System.Collections.Immutable;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server
{
  public record NuGetRegistrationState(
    VssNuGetPackageIdentity CanonicalIdentity,
    NuGetDeprecation? Deprecation,
    bool Listed,
    DateTime? PublishDate,
    IImmutableList<NuGetVulnerability> Vulnerabilities,
    NuGetCatalogCursor? CatalogCursorPosition)
  ;
}
