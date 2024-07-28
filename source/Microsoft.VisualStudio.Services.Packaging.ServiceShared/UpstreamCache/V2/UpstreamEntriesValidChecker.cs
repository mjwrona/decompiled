// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2.UpstreamEntriesValidChecker
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2
{
  public class UpstreamEntriesValidChecker : IUpstreamEntriesValidChecker
  {
    public UpstreamEntriesValidChecker(
      IUpstreamsConfigurationHasher upstreamConfigurationHasher,
      IUpstreamMetadataValidityPeriodProvider validityPeriodProvider,
      ITimeProvider timeProvider)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CupstreamConfigurationHasher\u003EP = upstreamConfigurationHasher;
      // ISSUE: reference to a compiler-generated field
      this.\u003CvalidityPeriodProvider\u003EP = validityPeriodProvider;
      // ISSUE: reference to a compiler-generated field
      this.\u003CtimeProvider\u003EP = timeProvider;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public bool IsUpstreamInfoValid(IFeedRequest request, IMetadataDocument metadataDocument) => this.CheckCore(request, metadataDocument, out string _);

    public string GetErrorMessage(IFeedRequest request, IMetadataDocument metadataDocument)
    {
      string message;
      this.CheckCore(request, metadataDocument, out message);
      return message;
    }

    private bool CheckCore(
      IFeedRequest feedRequest,
      IMetadataDocument metadataDocument,
      out string message)
    {
      IMetadataDocumentProperties properties = metadataDocument.Properties;
      if (!feedRequest.Feed.UpstreamEnabled)
      {
        message = "Upstream entries are invalid if upstreams are turned off on the feed, regardless of the sources";
        return false;
      }
      // ISSUE: reference to a compiler-generated field
      TimeSpan validityPeriod = this.\u003CvalidityPeriodProvider\u003EP.GetValidityPeriod(feedRequest, metadataDocument);
      // ISSUE: reference to a compiler-generated field
      DateTime dateTime = this.\u003CtimeProvider\u003EP.Now.ToUniversalTime() - validityPeriod;
      DateTime? lastRefreshedUtc = properties.UpstreamsLastRefreshedUtc;
      if (lastRefreshedUtc.HasValue)
      {
        lastRefreshedUtc = properties.UpstreamsLastRefreshedUtc;
        if (!(lastRefreshedUtc.Value < dateTime))
        {
          IPackageNameRequest packageNameRequest = (IPackageNameRequest) feedRequest.WithPackageName<IPackageName>(properties.PackageName);
          // ISSUE: reference to a compiler-generated field
          string hash = this.\u003CupstreamConfigurationHasher\u003EP.GetHash(new UpstreamsConfiguration(feedRequest), packageNameRequest);
          if (properties.UpstreamsConfigurationHash != hash)
          {
            message = "Invalid if configuration hash doesn't match, expected " + hash + ", got " + properties.UpstreamsConfigurationHash;
            return false;
          }
          message = "No Error Detected";
          return true;
        }
      }
      message = "Invalid if out of date";
      return false;
    }

    [ExcludeFromCodeCoverage]
    public static IUpstreamEntriesValidChecker Bootstrap(IVssRequestContext requestContext) => (IUpstreamEntriesValidChecker) new UpstreamEntriesValidChecker(UpstreamsConfigurationSha256Hasher.Bootstrap(requestContext), (IUpstreamMetadataValidityPeriodProvider) new UpstreamMetadataValidityPeriodProvider(UpstreamMetadata.MetadataValidityPeriodWithEntries.Bootstrap(requestContext), UpstreamMetadata.MetadataValidityPeriodWithoutEntries.Bootstrap(requestContext), UpstreamMetadata.AddToRefreshListOnlyIfAnyVersionsPresent.Bootstrap(requestContext)), (ITimeProvider) new DefaultTimeProvider());
  }
}
