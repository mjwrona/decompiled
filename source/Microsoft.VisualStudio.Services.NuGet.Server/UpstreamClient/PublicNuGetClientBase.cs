// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.UpstreamClient.PublicNuGetClientBase
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.UpstreamClient
{
  public abstract class PublicNuGetClientBase
  {
    protected Uri PackageSourceUri { get; }

    protected IHttpClient HttpClient { get; }

    public PublicNuGetClientBase(Uri packageSourceUri, IHttpClient httpClient)
    {
      this.PackageSourceUri = packageSourceUri;
      this.HttpClient = httpClient;
    }

    internal static Uri ConstructEscapedRelativeUri(
      Uri absoluteUri,
      string relativePathFormat,
      params string[] args)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      args = ((IEnumerable<string>) args).Select<string, string>(PublicNuGetClientBase.\u003C\u003EO.\u003C0\u003E__EscapeDataString ?? (PublicNuGetClientBase.\u003C\u003EO.\u003C0\u003E__EscapeDataString = new Func<string, string>(Uri.EscapeDataString))).ToArray<string>();
      Uri uri = new Uri(absoluteUri, string.Format(relativePathFormat, (object[]) args));
      return absoluteUri.IsBaseOf(uri) ? uri : throw new ArgumentFormatException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_InvalidUpstreamUrl());
    }

    public abstract Task<NuGetPackageRegistrationState> GetRegistrationState(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageName packageName,
      IEnumerable<VssNuGetPackageVersion> versions);

    public async Task<NuGetUpstreamMetadata> GetUpstreamMetadata(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageIdentity packageIdentity)
    {
      NuGetRegistrationState registrationState;
      (await this.GetRegistrationState(downstreamFeedRequest, packageIdentity.Name, (IEnumerable<VssNuGetPackageVersion>) new VssNuGetPackageVersion[1]
      {
        packageIdentity.Version
      })).Versions.TryGetValue(packageIdentity.Version, out registrationState);
      return new NuGetUpstreamMetadata()
      {
        SourceChain = (IReadOnlyCollection<UpstreamSourceInfo>) Array.Empty<UpstreamSourceInfo>(),
        StorageId = (IStorageId) null,
        Listed = (object) registrationState == null || registrationState.Listed
      };
    }

    internal class DummyVersionCountsImplementationMetrics : IVersionCountsImplementationMetrics
    {
      public int PackagesUnpacked => 0;

      public int PackagesPacked => 0;

      public int NumPackagesNeedingUnpack => 0;

      public int NumPackagesNeedingRepack => 0;

      public int NumPackagesNeedingSave => 0;

      public int NumPackages { get; }

      public int NumTotalVersions { get; }

      public DummyVersionCountsImplementationMetrics(int numPackages, int numTotalVersions)
      {
        this.NumPackages = numPackages;
        this.NumTotalVersions = numTotalVersions;
      }
    }
  }
}
