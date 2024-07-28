// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.HttpHandlers.MavenPublishHttpHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Constants;
using Microsoft.VisualStudio.Services.Maven.Server.Exceptions;
using Microsoft.VisualStudio.Services.Maven.Server.Exceptions.Helpers;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Maven.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.Maven.Server.HttpHandlers
{
  public class MavenPublishHttpHandler : PackageIngestionHttpHandler
  {
    internal const string PathRouteKey = "path";
    private readonly IFeedCacheService feedCacheService;
    private readonly IAsyncHandler<PackageIngestionRequest<MavenPackageIdentity, MavenPackageFileInfo>> packageIngester;
    private static readonly IReadOnlyList<HttpMethod> StaticAllowedHttpMethods = (IReadOnlyList<HttpMethod>) ImmutableList.Create<HttpMethod>(HttpMethod.Put, HttpMethod.Post);

    protected override string FeedIdRouteKey => "feed";

    public MavenPublishHttpHandler(
      IFeedCacheService feedCacheService,
      IAsyncHandler<PackageIngestionRequest<MavenPackageIdentity, MavenPackageFileInfo>> packageIngester)
    {
      ArgumentUtility.CheckForNull<IFeedCacheService>(feedCacheService, nameof (feedCacheService));
      this.AddExceptionMappings(MavenExceptionMappings.HttpExceptionMapping);
      this.feedCacheService = feedCacheService;
      this.packageIngester = packageIngester;
    }

    public override string UserRequestTimeoutSecondsPath => "/Configuration/" + this.Service + "/Publish/UserRequestTimeoutSeconds";

    protected override string Service => "maven";

    protected override string Method => this.Service + ".Publish";

    public override void ValidateRequestParameters(HttpRequestMessage request, RouteData routeData)
    {
      if (routeData.Values["path"] == null)
        throw new MavenInvalidFilePathException(Microsoft.VisualStudio.Services.Maven.Server.Resources.Error_MavenInvalidFilePath((object) ""));
    }

    protected override IReadOnlyList<HttpMethod> AllowedHttpMethods => MavenPublishHttpHandler.StaticAllowedHttpMethods;

    public async Task PublishRequestAsync(
      IVssRequestContext requestContext,
      HttpRequestMessage httpRequest,
      IFeedRequest feedRequest,
      RouteData routeData)
    {
      MavenPublishHttpHandler publishHttpHandler = this;
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<HttpRequestMessage>(httpRequest, nameof (httpRequest));
      ArgumentUtility.CheckForNull<IFeedRequest>(feedRequest, nameof (feedRequest));
      ArgumentUtility.CheckForNull<RouteData>(routeData, nameof (routeData));
      string tempFilePath = (string) null;
      try
      {
        MavenSecurityHelper.CheckForReadAndAddPackagePermissions(requestContext, feedRequest.Feed);
        IMavenFilePath filePath = MavenFilePath.Parse(routeData.Values["path"].ToString(), requestContext.IsFeatureEnabledWithLogging("Packaging.Maven.AllowSnapshotLiteralInGroupIdAndArtifactId"));
        tempFilePath = publishHttpHandler.CreateTemporaryFileForIngestion().FullName;
        using (Stream stream = await publishHttpHandler.ReadStreamToFile(requestContext, httpRequest, tempFilePath))
        {
          switch (filePath)
          {
            case IMavenMetadataFilePath metadataFilePath3:
              switch (metadataFilePath3)
              {
                case IMavenVersionLevelMetadataFilePath metadataFilePath1:
                  requestContext.Items["Packaging.PackageName"] = (object) metadataFilePath1.PackageName;
                  break;
                case IMavenArtifactIdLevelMetadataFilePath metadataFilePath2:
                  requestContext.Items["Packaging.PackageName"] = (object) metadataFilePath2.PackageName;
                  break;
              }
              break;
            case IMavenArtifactFilePath filePath1:
              requestContext.Items["Packaging.PackageIdentity"] = (object) new MavenPackageIdentity(filePath1.PackageName, filePath1.PackageVersion);
              if (!MavenFileNameUtility.IsChecksumFile(filePath1.FileName))
              {
                stream.Position = 0L;
                NullResult nullResult = await publishHttpHandler.packageIngester.Handle(new PackageIngestionRequest<MavenPackageIdentity, MavenPackageFileInfo>(feedRequest, new MavenPackageFileInfo(stream, filePath1), Protocol.Maven.V1));
                break;
              }
              break;
          }
        }
        filePath = (IMavenFilePath) null;
      }
      catch (TaskCanceledException ex)
      {
        if (requestContext.IsCanceled)
          publishHttpHandler.HandleClientCancelledException((Exception) ex);
        throw;
      }
      finally
      {
        publishHttpHandler.DeleteTempFile(requestContext, tempFilePath);
      }
      tempFilePath = (string) null;
    }

    private void DeleteTempFile(IVssRequestContext requestContext, string fileName)
    {
      try
      {
        File.Delete(fileName);
      }
      catch (Exception ex)
      {
        requestContext.Trace(12090630, TraceLevel.Warning, MavenTracerPoints.Ingestion.IngestionTraceData.Area, MavenTracerPoints.Ingestion.IngestionTraceData.Layer, string.Format("Unable to delete the temporary file {0}. Exception: {1}. Message: {2}", (object) fileName, (object) ex.GetType(), (object) ex.Message));
      }
    }

    [ControllerMethodTraceFilter(12090000)]
    protected override async Task AddPackageFromRequestAsync(
      IVssRequestContext vssRequestContext,
      HttpRequestMessage httpRequest,
      IFeedRequest feedRequest,
      RouteData routeData)
    {
      await this.PublishRequestAsync(vssRequestContext, httpRequest, feedRequest, routeData);
    }

    protected override void ValidatePackageSize(IVssRequestContext requestContext, long packageSize) => PackageIngestionUtils.ValidateMaxPackageSize(requestContext, "maven", packageSize);

    protected override void HandleClientCancelledException(Exception e) => throw new MavenServiceCanceledDueToClientException(e);

    protected override IProtocol GetProtocol() => (IProtocol) Protocol.Maven;

    protected override string GetClientSessionIdFrom(HttpRequestMessage requestMessage) => (string) null;

    private async Task<Stream> ReadStreamToFile(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      string filePath)
    {
      Stream file;
      using (requestContext.CreateTimeToFirstPageExclusionBlock())
      {
        FileStream result = (FileStream) null;
        if (request.Content.IsMimeMultipartContent())
        {
          MultipartMemoryStreamProvider memoryStreamProvider = await request.Content.ReadAsMultipartAsync();
          if (memoryStreamProvider.Contents.Count > 1)
            throw new InvalidUserRequestException(Microsoft.VisualStudio.Services.Maven.Server.Resources.Error_RequestHasTooManyFiles());
          using (HttpContent firstContent = memoryStreamProvider.Contents.FirstOrDefault<HttpContent>())
          {
            if (firstContent != null)
            {
              result = new FileStream(filePath, FileMode.Create);
              await firstContent.CopyToAsync((Stream) result);
            }
          }
        }
        else
        {
          using (Stream stream = await request.Content.ReadAsStreamAsync())
          {
            if (stream != null)
            {
              result = new FileStream(filePath, FileMode.Create);
              stream.CopyTo((Stream) result);
            }
          }
        }
        if (result != null && result.Length > 0L)
        {
          file = (Stream) result;
          goto label_25;
        }
        else
          result = (FileStream) null;
      }
      throw new MavenFileNotFoundException(Microsoft.VisualStudio.Services.Maven.Server.Resources.Error_PackageAddOrUpdateMustHaveBody());
label_25:
      return file;
    }
  }
}
