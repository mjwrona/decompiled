// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.HttpHandlers.NuGetV2PushHttpHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2;
using Microsoft.VisualStudio.Services.NuGet.Server.Exceptions;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.HttpHandlers
{
  public class NuGetV2PushHttpHandler : PackageIngestionHttpHandler
  {
    internal const 
    #nullable disable
    string NuGetApiKeyHeader = "X-NUGET-APIKEY";
    private readonly IFileStreamFactory fileStreamFactory;
    private readonly IPackageIngestionService packageIngestionService;
    private static readonly IReadOnlyList<HttpMethod> StaticAllowedHttpMethods = (IReadOnlyList<HttpMethod>) ImmutableList.Create<HttpMethod>(HttpMethod.Put, HttpMethod.Post);

    protected override string FeedIdRouteKey => "feedId";

    public NuGetV2PushHttpHandler(
      IFileStreamFactory fileStreamFactory,
      IPackageIngestionService packageIngestionService)
    {
      this.AddExceptionMappings(NuGetExceptionMappings.Mappings);
      this.fileStreamFactory = fileStreamFactory;
      this.packageIngestionService = packageIngestionService;
    }

    public override string UserRequestTimeoutSecondsPath => "/Configuration/NuGet/Push/UserRequestTimeoutSeconds";

    protected override string Service => "NuGet";

    protected override string Method => "NuGetV2Push.PushPackageV2Async";

    public async Task AddPackageAsync(
      IVssRequestContext requestContext,
      IFeedRequest feedRequest,
      ICollection<MultipartFileData> parts)
    {
      ArgumentUtility.CheckForNull<ICollection<MultipartFileData>>(parts, nameof (parts));
      MultipartFileData multipartFileData = parts.Count == 1 ? parts.First<MultipartFileData>() : throw new InvalidPushRequestException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_V2PushRequiresPackagePart());
      if (multipartFileData.Headers == null || multipartFileData.Headers.ContentDisposition == null || multipartFileData.Headers.ContentType == null || !VssStringComparer.ContentType.Equals(multipartFileData.Headers.ContentType.MediaType, "application/octet-stream"))
        throw new InvalidPushRequestException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_V2PushRequiresPackagePart());
      using (Stream packageStream = this.fileStreamFactory.CreateFileStream(multipartFileData.LocalFileName))
        await this.packageIngestionService.AddPackageFromStreamAsync(requestContext, feedRequest, packageStream, "v2", (IEnumerable<UpstreamSourceInfo>) new List<UpstreamSourceInfo>(), false);
    }

    public override void ValidateRequestParameters(HttpRequestMessage request, RouteData routeData)
    {
      if (!request.Content.IsMimeMultipartContent())
        throw new Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions.UnsupportedMediaTypeException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_UnexpectedMediaType((object) "multipart/form-data"));
      if (!request.Headers.Contains("X-NUGET-APIKEY"))
        throw new NuGetApiKeyRequiredException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_NuGetApiKeyRequired());
    }

    protected override IReadOnlyList<HttpMethod> AllowedHttpMethods => NuGetV2PushHttpHandler.StaticAllowedHttpMethods;

    protected override async Task AddPackageFromRequestAsync(
      IVssRequestContext vssRequestContext,
      HttpRequestMessage request,
      IFeedRequest feedRequest,
      RouteData routeData)
    {
      NuGetV2PushHttpHandler v2PushHttpHandler = this;
      Collection<MultipartFileData> fileData = (Collection<MultipartFileData>) null;
      DirectoryInfo workingDirectory = (DirectoryInfo) null;
      try
      {
        workingDirectory = v2PushHttpHandler.CreateTemporaryDirectoryForIngestion();
        vssRequestContext.Trace(5720901, TraceLevel.Info, NuGetTracePoints.V2PushController.TraceData.Area, NuGetTracePoints.V2PushController.TraceData.Layer, "Receiving package files into temporary directory " + workingDirectory.FullName);
        using (vssRequestContext.CreateTimeToFirstPageExclusionBlock())
          fileData = await HttpHandlerHelper.ReadMultipartFileDataFromRequest(vssRequestContext, request, workingDirectory.FullName);
        await v2PushHttpHandler.AddPackageAsync(vssRequestContext, feedRequest, (ICollection<MultipartFileData>) fileData);
      }
      catch (IOException ex) when (ex.InnerException is HttpException)
      {
        if (ex.InnerException.Message.Contains("The client disconnected"))
          v2PushHttpHandler.HandleClientCancelledException(ex.InnerException);
        throw;
      }
      catch (IOException ex) when (ex.InnerException is TaskCanceledException)
      {
        if (vssRequestContext.IsCanceled)
          v2PushHttpHandler.HandleClientCancelledException(ex.InnerException);
        throw;
      }
      catch (TaskCanceledException ex)
      {
        if (vssRequestContext.IsCanceled)
          v2PushHttpHandler.HandleClientCancelledException((Exception) ex);
        throw;
      }
      finally
      {
        await v2PushHttpHandler.DeleteWorkingDirectoryAsync(vssRequestContext, workingDirectory, fileData);
      }
      fileData = (Collection<MultipartFileData>) null;
      workingDirectory = (DirectoryInfo) null;
    }

    protected override void ApplyWarningMessageToResponse(
      HttpContextBase httpContext,
      PackagingWarningMessage message)
    {
      base.ApplyWarningMessageToResponse(httpContext, message);
      if (!message.StatusDescriptionMessageTruncated)
        return;
      httpContext.Response.Headers["X-NuGet-Warning"] = message.CustomWarningMessage;
    }

    protected override void ValidatePackageSize(IVssRequestContext requestContext, long packageSize)
    {
      long maxPackageSize = IngestionUtils.GetMaxPackageSize(requestContext);
      if (packageSize > maxPackageSize)
        throw new TooLargePackagePushException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_PackageTooLarge((object) maxPackageSize));
    }

    protected override void HandleClientCancelledException(Exception e) => throw new ServiceCanceledDueToClientException(e);

    protected override IProtocol GetProtocol() => (IProtocol) Protocol.NuGet;

    protected override string GetClientSessionIdFrom(HttpRequestMessage requestMessage)
    {
      IEnumerable<string> values;
      return !requestMessage.Headers.TryGetValues("X-NuGet-Session-Id", out values) ? (string) null : values.FirstOrDefault<string>();
    }

    private async Task DeleteWorkingDirectoryAsync(
      IVssRequestContext requestContext,
      DirectoryInfo directory,
      Collection<MultipartFileData> fileData)
    {
      if (fileData != null)
      {
        foreach (MultipartFileData multipartFileData in fileData)
          NuGetV2PushHttpHandler.TryDeleteFile(requestContext, multipartFileData.LocalFileName);
      }
      if (directory.EnumerateFileSystemInfos("*").Any<FileSystemInfo>())
        await Task.Delay(3000);
      this.TryDeleteDirectory(requestContext, directory);
    }

    private static void TryDeleteFile(IVssRequestContext requestContext, string fileName)
    {
      try
      {
        File.Delete(fileName);
      }
      catch (Exception ex)
      {
        requestContext.Trace(5720900, TraceLevel.Warning, NuGetTracePoints.V2PushController.TraceData.Area, NuGetTracePoints.V2PushController.TraceData.Layer, string.Format("Unable to delete the temporary file {0}. Exception: {1}. Message: {2}", (object) fileName, (object) ex.GetType(), (object) ex.Message));
      }
    }

    private void TryDeleteDirectory(IVssRequestContext requestContext, DirectoryInfo directory)
    {
      try
      {
        if (directory == null)
          return;
        directory.Refresh();
        if (!directory.Exists)
          return;
        directory.Delete(true);
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(5720940, TraceLevel.Error, NuGetTracePoints.V2PushController.TraceData.Area, NuGetTracePoints.V2PushController.TraceData.Layer, string.Format("Unable to delete the directory {0}. Exception: {1}. Message: {2}", (object) directory, (object) ex.GetType(), (object) ex.Message));
      }
    }
  }
}
