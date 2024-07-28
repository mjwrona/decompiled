// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.NpmPackageIngestionServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.PackageIngestion;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly
{
  public class NpmPackageIngestionServiceFacade : INpmIngestionService
  {
    private readonly IVssRequestContext requestContext;

    public NpmPackageIngestionServiceFacade(IVssRequestContext requestContext) => this.requestContext = requestContext ?? throw new ArgumentNullException(nameof (requestContext));

    public Task AddPackageAsync(FeedCore feed, PackageMetadata newPackageMetadata) => this.requestContext.GetService<INpmPackageIngestionService>().AddPackageAsync(this.requestContext, feed, newPackageMetadata);

    public Task SaveStreamToFeedAsync(
      FeedCore feed,
      UpstreamPackageContent upstreamPackageContent,
      NpmPackageIdentity expectedIdentity,
      string? deprecateMessage)
    {
      return this.requestContext.GetService<INpmPackageIngestionService>().SaveStreamToFeedAsync(this.requestContext, feed, upstreamPackageContent, expectedIdentity, deprecateMessage);
    }
  }
}
