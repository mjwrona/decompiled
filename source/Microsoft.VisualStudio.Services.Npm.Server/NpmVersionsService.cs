// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NpmVersionsService
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.UpdatePackageVersions;
using Microsoft.VisualStudio.Services.Npm.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.Npm;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server
{
  public class NpmVersionsService : INpmVersionsService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public async Task UpdatePackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      NpmPackagesBatchRequest batchRequest)
    {
      FeedRequest feed1 = new FeedRequest(feed, (IProtocol) Protocol.npm);
      FeedValidator.GetFeedIsNotReadOnlyValidator().Validate(feed1.Feed);
      NullResult nullResult = await new NpmPackagesBatchBootstrapper(requestContext).Bootstrap().Handle(new BatchRawRequest((IFeedRequest) feed1, (IPackagesBatchRequest) batchRequest));
    }

    public async Task UpdateRecycleBinPackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      NpmPackagesBatchRequest batchRequest)
    {
      FeedValidator.GetFeedIsNotReadOnlyValidator().Validate(feed);
      FeedRequest feed1 = new FeedRequest(feed, (IProtocol) Protocol.npm);
      HttpResponseMessage httpResponseMessage = await new NpmRecycleBinPackagesBatchBootstrapper(requestContext).Bootstrap().Handle(new BatchRawRequest((IFeedRequest) feed1, (IPackagesBatchRequest) batchRequest));
    }
  }
}
