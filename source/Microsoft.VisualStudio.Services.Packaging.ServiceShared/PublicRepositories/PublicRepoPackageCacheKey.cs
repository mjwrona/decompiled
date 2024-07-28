// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories.PublicRepoPackageCacheKey
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories
{
  public record PublicRepoPackageCacheKey(
    string Universe,
    string SourceTag,
    string NormalizedPackageName)
  {
    public static PublicRepoPackageCacheKey Create(
      string universe,
      WellKnownUpstreamSource source,
      IPackageName packageName)
    {
      return new PublicRepoPackageCacheKey(universe, source.TagName, packageName.NormalizedName);
    }
  }
}
