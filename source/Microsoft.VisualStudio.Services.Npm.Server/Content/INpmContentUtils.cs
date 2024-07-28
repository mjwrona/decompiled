// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Content.INpmContentUtils
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Content
{
  public interface INpmContentUtils
  {
    Task<Stream> GetLocalPackageVersionContentAsync(
      FeedCore feed,
      NpmPackageName packageName,
      SemanticVersion packageVersion,
      VersionSearchFilter searchFilter);

    Task<Stream> GetUpstreamPackageVersionContentAsync(
      FeedCore feed,
      NpmPackageName packageName,
      SemanticVersion packageVersion,
      VersionSearchFilter searchFilter,
      bool blockUnsafeIngestion);
  }
}
