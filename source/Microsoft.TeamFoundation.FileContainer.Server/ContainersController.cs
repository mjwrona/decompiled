// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ContainersController
// Assembly: Microsoft.TeamFoundation.FileContainer.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D69E508F-D51F-4EF2-B979-7E96E61DA19E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.FileContainer.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FileContainer;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Container", ResourceName = "Containers", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true, AllowEmptyOrPlainTextContentTypeForCompatServiceCalls = true)]
  public class ContainersController : TfsApiController
  {
    private static readonly char[] s_invalidFileChars = Path.GetInvalidFileNameChars();
    private static readonly List<RequestMediaType> s_supportedTypes = new List<RequestMediaType>(4)
    {
      RequestMediaType.Json,
      RequestMediaType.Zip,
      RequestMediaType.OctetStream,
      RequestMediaType.None
    };

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<UriFormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ContainerWriteAccessDeniedException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ContainerNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ContainerItemNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ArgumentOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ContainerItemExistsException>(HttpStatusCode.Conflict);
    }

    public override string TraceArea => "FileContainer";

    public override string ActivityLogArea => "Framework";

    [HttpGet]
    [TraceFilter(1008000, 1008010)]
    [ClientLocationId("E4F5C81E-E250-447B-9FEF-BD48471BEA5E")]
    public IQueryable<Microsoft.VisualStudio.Services.FileContainer.FileContainer> GetContainers(
      string artifactUris = null)
    {
      return this.GetContainers(new Guid?(), artifactUris);
    }

    protected IQueryable<Microsoft.VisualStudio.Services.FileContainer.FileContainer> GetContainers(
      Guid? scopeIdentifier,
      string artifactUris = null)
    {
      List<Uri> artifactUris1 = (List<Uri>) null;
      if (!string.IsNullOrEmpty(artifactUris))
      {
        artifactUris1 = new List<Uri>();
        IEnumerable<string> source = ((IEnumerable<string>) artifactUris.Split(',')).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x)));
        artifactUris1.AddRange(source.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Select<string, Uri>((Func<string, Uri>) (x => new Uri(x))));
      }
      TeamFoundationFileContainerService service = this.TfsRequestContext.GetService<TeamFoundationFileContainerService>();
      List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> source1 = !scopeIdentifier.HasValue ? service.QueryContainers(this.TfsRequestContext, (IList<Uri>) artifactUris1) : service.QueryContainers(this.TfsRequestContext, (IList<Uri>) artifactUris1, scopeIdentifier.Value);
      foreach (Microsoft.VisualStudio.Services.FileContainer.FileContainer fileContainer in source1)
      {
        fileContainer.ContentLocation = this.Url.RestLink(this.TfsRequestContext, FileContainerResourceIds.FileContainer, (object) new
        {
          containerId = fileContainer.Id
        });
        fileContainer.ItemLocation = fileContainer.ContentLocation.TrimEnd('/') + "?metadata=True";
        fileContainer.SecurityToken = string.Empty;
        fileContainer.SigningKeyId = Guid.Empty;
      }
      return source1.AsQueryable<Microsoft.VisualStudio.Services.FileContainer.FileContainer>();
    }

    [HttpGet]
    [TraceFilter(1008021, 1008030)]
    [ClientResponseType(typeof (List<FileContainerItem>), null, null)]
    [ClientLocationId("E4F5C81E-E250-447B-9FEF-BD48471BEA5E")]
    public HttpResponseMessage GetItems(
      long containerId,
      [ClientQueryParameter] string itemPath = null,
      bool metadata = false,
      [FromUri(Name = "$format")] string format = null,
      string downloadFileName = null,
      bool includeDownloadTickets = false,
      bool isShallow = false,
      bool ignoreRequestedMediaType = false,
      bool includeBlobMetadata = false,
      bool saveAbsolutePath = true,
      bool preferRedirect = false)
    {
      return this.GetItems(containerId, new Guid?(), itemPath, metadata, downloadFileName, includeDownloadTickets, includeBlobMetadata, isShallow, ignoreRequestedMediaType: ignoreRequestedMediaType, saveAbsolutePath: saveAbsolutePath, preferRedirect: preferRedirect);
    }

    protected HttpResponseMessage GetItems(
      long containerId,
      Guid? scopeIdentifier,
      string itemPath = null,
      bool metadata = false,
      string downloadFileName = null,
      bool includeDownloadTickets = false,
      bool includeBlobMetadata = false,
      bool isShallow = false,
      bool allowCompression = true,
      RequestMediaType formatRequested = RequestMediaType.None,
      bool ignoreRequestedMediaType = false,
      bool saveAbsolutePath = true,
      bool preferRedirect = false)
    {
      if (ignoreRequestedMediaType)
        formatRequested = RequestMediaType.None;
      else if (formatRequested == RequestMediaType.None)
        formatRequested = MediaTypeFormatUtility.GetPrioritizedAcceptHeaders(this.Request, ContainersController.s_supportedTypes).FirstOrDefault<RequestMediaType>();
      TeamFoundationFileContainerService service = this.TfsRequestContext.GetService<TeamFoundationFileContainerService>();
      if (string.IsNullOrEmpty(itemPath) & metadata)
      {
        Microsoft.VisualStudio.Services.FileContainer.FileContainer fileContainer = !scopeIdentifier.HasValue ? service.GetContainer(this.TfsRequestContext, containerId) : service.GetContainer(this.TfsRequestContext, containerId, scopeIdentifier.Value, true);
        if (fileContainer == null)
          throw new ContainerNotFoundException(containerId);
        fileContainer.ContentLocation = this.Url.RestLink(this.TfsRequestContext, FileContainerResourceIds.FileContainer, (object) new
        {
          containerId = fileContainer.Id
        });
        fileContainer.ItemLocation = fileContainer.ContentLocation.TrimEnd('/') + "?metadata=True";
        fileContainer.SecurityToken = string.Empty;
        fileContainer.SigningKeyId = Guid.Empty;
        return this.Request.CreateResponse<Microsoft.VisualStudio.Services.FileContainer.FileContainer>(HttpStatusCode.OK, fileContainer);
      }
      if ((!scopeIdentifier.HasValue ? service.GetContainer(this.TfsRequestContext, containerId) : service.GetContainer(this.TfsRequestContext, containerId, scopeIdentifier.Value, true)) == null)
        throw new ContainerNotFoundException(containerId);
      List<FileContainerItem> items1 = !scopeIdentifier.HasValue ? service.QueryItems(this.TfsRequestContext, containerId, itemPath, isShallow, includeBlobMetadata) : service.QueryItems(this.TfsRequestContext, containerId, itemPath, scopeIdentifier.Value, isShallow, includeBlobMetadata);
      if (items1.Count == 0)
      {
        if (!string.IsNullOrEmpty(itemPath))
          throw new ContainerItemNotFoundException(containerId, itemPath);
        return formatRequested == RequestMediaType.Zip ? this.Request.CreateResponse(HttpStatusCode.NoContent) : this.Request.CreateResponse<FileContainerItem[]>(HttpStatusCode.OK, Array.Empty<FileContainerItem>());
      }
      FileContainerItem fileContainerItem1 = items1[0];
      bool flag1 = false;
      if (string.IsNullOrEmpty(itemPath) || fileContainerItem1.ItemType == ContainerItemType.Folder)
        flag1 = true;
      if (metadata || formatRequested == RequestMediaType.Json && !flag1)
      {
        string baseUrl = this.Url.RestLink(this.TfsRequestContext, FileContainerResourceIds.FileContainer, (object) new
        {
          containerId = containerId
        }).TrimEnd('/');
        fileContainerItem1.ContentLocation = ContainersController.AddQueryString(baseUrl, itemPath);
        fileContainerItem1.ItemLocation = ContainersController.AddQueryString(baseUrl, itemPath, true);
        List<FileContainerItem> items2 = new List<FileContainerItem>()
        {
          fileContainerItem1
        };
        if (includeDownloadTickets)
          items2.GenerateContainerItemTickets(this.TfsRequestContext);
        return this.Request.CreateResponse<List<FileContainerItem>>(HttpStatusCode.OK, items2);
      }
      if (formatRequested == RequestMediaType.Zip)
        return this.CreateZipDownloadResponse(containerId, itemPath, downloadFileName, scopeIdentifier, saveAbsolutePath);
      if (flag1)
      {
        string baseUrl = this.Url.RestLink(this.TfsRequestContext, FileContainerResourceIds.FileContainer, (object) new
        {
          containerId = containerId
        }).TrimEnd('/');
        bool flag2 = this.TfsRequestContext.IsFeatureEnabled("FileContainer.DontSendFileIds");
        bool flag3 = flag2 || this.TfsRequestContext.IsFeatureEnabled("FileContainer.DontSendLongFileIds");
        foreach (FileContainerItem fileContainerItem2 in items1)
        {
          fileContainerItem2.ContentLocation = ContainersController.AddQueryString(baseUrl, fileContainerItem2.Path);
          fileContainerItem2.ItemLocation = ContainersController.AddQueryString(baseUrl, fileContainerItem2.Path, true);
          if (fileContainerItem2.FileId != 0)
          {
            if (flag2)
              fileContainerItem2.FileId = 0;
            else if (flag3 && fileContainerItem2.FileId > int.MaxValue)
              fileContainerItem2.FileId = 0;
          }
        }
        if (includeDownloadTickets)
          items1.GenerateContainerItemTickets(this.TfsRequestContext);
        return this.Request.CreateResponse<List<FileContainerItem>>(HttpStatusCode.OK, items1);
      }
      if (fileContainerItem1.Status == ContainerItemStatus.PendingUpload)
        throw new ContainerItemNotFoundException(FrameworkResources.FileContainerCannotDownloadPartialFile());
      if (fileContainerItem1.FileLength > 0L)
        return this.CreateDownloadResponse(fileContainerItem1, downloadFileName, allowCompression, preferRedirect);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new ByteArrayContent(Array.Empty<byte>());
      response.Content.Headers.ContentLength = new long?(0L);
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(ContainersController.GetDownloadFileName(fileContainerItem1.ContainerId, fileContainerItem1.Path, downloadFileName, false));
      return response;
    }

    private static string AddQueryString(string baseUrl, string itemPath, bool metadata = false) => metadata ? baseUrl + "?itemPath=" + Uri.EscapeDataString(itemPath) + "&metadata=True" : baseUrl + "?itemPath=" + Uri.EscapeDataString(itemPath);

    [HttpPut]
    [TraceFilter(1008011, 1008020)]
    [ClientRequestBodyIsStream]
    [ClientResponseType(typeof (FileContainerItem), null, null)]
    [ClientLocationId("E4F5C81E-E250-447B-9FEF-BD48471BEA5E")]
    [ClientInclude(RestClientLanguages.CSharp | RestClientLanguages.Java | RestClientLanguages.TypeScript)]
    public HttpResponseMessage CreateItem(int containerId, [ClientQueryParameter] string itemPath) => this.CreateItem(containerId, itemPath, new Guid?());

    protected HttpResponseMessage CreateItemFromArtifactUpload(
      int containerId,
      string itemPath,
      Guid? scopeIdentifier,
      string artifactHash,
      long? fileLength)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(itemPath, nameof (itemPath));
      ArgumentUtility.CheckStringForNullOrEmpty(artifactHash, nameof (artifactHash));
      ArgumentUtility.CheckForNull<long>(fileLength, nameof (fileLength));
      TeamFoundationFileContainerService service = this.TfsRequestContext.GetService<TeamFoundationFileContainerService>();
      if ((!scopeIdentifier.HasValue ? service.GetContainer(this.TfsRequestContext, (long) containerId, false) : service.GetContainer(this.TfsRequestContext, (long) containerId, scopeIdentifier.Value, false)) == null)
        throw new ContainerNotFoundException((long) containerId);
      FileContainerItem fileContainerItem1;
      if (scopeIdentifier.HasValue)
      {
        TeamFoundationFileContainerService containerService = service;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        long containerId1 = (long) containerId;
        FileContainerItem fileContainerItem2 = new FileContainerItem();
        fileContainerItem2.Path = itemPath;
        fileContainerItem2.ItemType = ContainerItemType.File;
        fileContainerItem2.FileLength = fileLength.Value;
        fileContainerItem2.FileHash = (byte[]) null;
        fileContainerItem2.FileEncoding = 1;
        fileContainerItem2.FileType = 1;
        fileContainerItem2.Status = ContainerItemStatus.Created;
        string artifactHash1 = artifactHash;
        Guid? scopeIdentifier1 = new Guid?(scopeIdentifier.Value);
        fileContainerItem1 = containerService.CreateItemFromArtifact(tfsRequestContext, containerId1, fileContainerItem2, artifactHash1, scopeIdentifier1).FirstOrDefault<FileContainerItem>();
      }
      else
      {
        TeamFoundationFileContainerService containerService = service;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        long containerId2 = (long) containerId;
        FileContainerItem fileContainerItem3 = new FileContainerItem();
        fileContainerItem3.Path = itemPath;
        fileContainerItem3.ItemType = ContainerItemType.File;
        fileContainerItem3.FileLength = fileLength.Value;
        fileContainerItem3.FileHash = (byte[]) null;
        fileContainerItem3.FileEncoding = 1;
        fileContainerItem3.FileType = 1;
        fileContainerItem3.Status = ContainerItemStatus.Created;
        string artifactHash2 = artifactHash;
        Guid? scopeIdentifier2 = new Guid?();
        fileContainerItem1 = containerService.CreateItemFromArtifact(tfsRequestContext, containerId2, fileContainerItem3, artifactHash2, scopeIdentifier2).FirstOrDefault<FileContainerItem>();
      }
      fileContainerItem1.ContentLocation = this.Url.RestLink(this.TfsRequestContext, FileContainerResourceIds.FileContainer, (object) new
      {
        containerId = containerId,
        itemPath = itemPath
      });
      fileContainerItem1.ItemLocation = this.Url.RestLink(this.TfsRequestContext, FileContainerResourceIds.FileContainer, (object) new
      {
        controller = "containers",
        itemPath = fileContainerItem1.Path,
        metadata = true
      });
      fileContainerItem1.ContentId = (byte[]) null;
      if (fileContainerItem1.FileId != 0)
      {
        if (this.TfsRequestContext.IsFeatureEnabled("FileContainer.DontSendFileIds"))
          fileContainerItem1.FileId = 0;
        else if (fileContainerItem1.FileId > int.MaxValue && this.TfsRequestContext.IsFeatureEnabled("FileContainer.DontSendLongFileIds"))
          fileContainerItem1.FileId = 0;
      }
      HttpResponseMessage response = this.Request.CreateResponse<FileContainerItem>(HttpStatusCode.Created, fileContainerItem1);
      response.Headers.Location = this.Request.RequestUri;
      return response;
    }

    protected HttpResponseMessage CreateItem(
      int containerId,
      string itemPath,
      Guid? scopeIdentifier)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(itemPath, nameof (itemPath));
      TeamFoundationFileContainerService service = this.TfsRequestContext.GetService<TeamFoundationFileContainerService>();
      if ((!scopeIdentifier.HasValue ? service.GetContainer(this.TfsRequestContext, (long) containerId, false) : service.GetContainer(this.TfsRequestContext, (long) containerId, scopeIdentifier.Value, false)) == null)
        throw new ContainerNotFoundException((long) containerId);
      List<FileContainerItem> fileContainerItemList = !scopeIdentifier.HasValue ? service.QueryItems(this.TfsRequestContext, (long) containerId, itemPath) : service.QueryItems(this.TfsRequestContext, (long) containerId, itemPath, scopeIdentifier.Value, false, false);
      if (fileContainerItemList.Count > 1)
        return this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, FrameworkResources.FileContainerContentForFolderError((object) itemPath));
      if (fileContainerItemList.Count == 1 && fileContainerItemList[0].ItemType == ContainerItemType.Folder)
        return this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, FrameworkResources.FileContainerContentForFolderError((object) itemPath));
      if (fileContainerItemList.Count == 1 && fileContainerItemList[0].FileId != 0)
      {
        if (this.TfsRequestContext.IsFeatureEnabled("FileContainer.DontSendFileIds"))
          fileContainerItemList[0].FileId = 0;
        else if (fileContainerItemList[0].FileId > int.MaxValue && this.TfsRequestContext.IsFeatureEnabled("FileContainer.DontSendLongFileIds"))
          fileContainerItemList[0].FileId = 0;
      }
      HttpContent content = this.Request.Content;
      if (content == null)
        return this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, FrameworkResources.FileContainerUploadContentError());
      long offsetFrom = 0;
      long compressedLength = 0;
      if (content.Headers.ContentRange != null && content.Headers.ContentRange.From.HasValue && content.Headers.ContentRange.Length.HasValue)
      {
        offsetFrom = content.Headers.ContentRange.From.Value;
        compressedLength = content.Headers.ContentRange.Length.Value;
      }
      else
      {
        long? contentLength = content.Headers.ContentLength;
        long num = 0;
        if (contentLength.GetValueOrDefault() > num & contentLength.HasValue)
          return this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, FrameworkResources.FileContainerUploadControllerRangeHeaderError());
      }
      long result;
      CompressionType compressionType;
      if (content.Headers.ContentEncoding.Contains("gzip"))
      {
        IEnumerable<string> values;
        if (!this.Request.Headers.TryGetValues("x-tfs-filelength", out values) || !long.TryParse(values.FirstOrDefault<string>(), out result) || result <= 0L)
          return this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, FrameworkResources.FileContainerGZipNotSupported());
        compressionType = CompressionType.GZip;
      }
      else
      {
        compressionType = CompressionType.None;
        result = compressedLength;
      }
      byte[] clientContentId = (byte[]) null;
      FileContainerItem fileContainerItem;
      if (offsetFrom == 0L || fileContainerItemList.Count == 0)
      {
        if (scopeIdentifier.HasValue)
        {
          TeamFoundationFileContainerService containerService = service;
          IVssRequestContext tfsRequestContext = this.TfsRequestContext;
          long containerId1 = (long) containerId;
          List<FileContainerItem> items = new List<FileContainerItem>();
          items.Add(new FileContainerItem()
          {
            Path = itemPath,
            ItemType = ContainerItemType.File,
            FileLength = result,
            FileHash = (byte[]) null,
            FileEncoding = 1,
            FileType = 1,
            ContentId = clientContentId
          });
          Guid scopeIdentifier1 = scopeIdentifier.Value;
          fileContainerItem = containerService.CreateItems(tfsRequestContext, containerId1, (IList<FileContainerItem>) items, scopeIdentifier1).FirstOrDefault<FileContainerItem>();
        }
        else
          fileContainerItem = service.CreateItems(this.TfsRequestContext, (long) containerId, (IList<FileContainerItem>) new List<FileContainerItem>()
          {
            new FileContainerItem()
            {
              Path = itemPath,
              ItemType = ContainerItemType.File,
              FileLength = result,
              FileHash = (byte[]) null,
              FileEncoding = 1,
              FileType = 1,
              ContentId = clientContentId
            }
          }).FirstOrDefault<FileContainerItem>();
      }
      else
      {
        if (fileContainerItemList[0].Status != ContainerItemStatus.PendingUpload)
          return this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, FrameworkResources.FileContainerContentAlreadyComplete());
        fileContainerItem = fileContainerItemList[0];
      }
      fileContainerItem.ContentLocation = this.Url.RestLink(this.TfsRequestContext, FileContainerResourceIds.FileContainer, (object) new
      {
        containerId = containerId,
        itemPath = itemPath
      });
      fileContainerItem.ItemLocation = this.Url.RestLink(this.TfsRequestContext, FileContainerResourceIds.FileContainer, (object) new
      {
        controller = "containers",
        itemPath = fileContainerItem.Path,
        metadata = true
      });
      if (compressedLength != 0L && fileContainerItem.Status == ContainerItemStatus.PendingUpload)
        fileContainerItem = !scopeIdentifier.HasValue ? service.UploadFile(this.TfsRequestContext, (long) containerId, fileContainerItem, content.ReadAsStreamAsync().Result, offsetFrom, compressedLength, compressionType, clientContentId) : service.UploadFile(this.TfsRequestContext, (long) containerId, fileContainerItem, content.ReadAsStreamAsync().Result, offsetFrom, compressedLength, compressionType, scopeIdentifier.Value, clientContentId);
      fileContainerItem.ContentId = (byte[]) null;
      HttpResponseMessage httpResponseMessage = fileContainerItem.Status != ContainerItemStatus.Created ? this.Request.CreateResponse<FileContainerItem>(HttpStatusCode.Accepted, fileContainerItem) : this.Request.CreateResponse<FileContainerItem>(HttpStatusCode.Created, fileContainerItem);
      httpResponseMessage.Headers.Location = this.Request.RequestUri;
      return httpResponseMessage;
    }

    [HttpDelete]
    [TraceFilter(1008131, 1008140)]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("E4F5C81E-E250-447B-9FEF-BD48471BEA5E")]
    public HttpResponseMessage DeleteItem(long containerId, [ClientQueryParameter] string itemPath) => this.DeleteItem(containerId, itemPath, new Guid?());

    protected HttpResponseMessage DeleteItem(
      long containerId,
      string itemPath,
      Guid? scopeIdentifier)
    {
      if (string.IsNullOrEmpty(itemPath))
        return this.Request.CreateResponse(HttpStatusCode.BadRequest);
      string[] paths = new string[1]{ itemPath };
      TeamFoundationFileContainerService service = this.TfsRequestContext.GetService<TeamFoundationFileContainerService>();
      if (scopeIdentifier.HasValue)
        service.DeleteItems(this.TfsRequestContext, containerId, (IList<string>) paths, scopeIdentifier.Value);
      else
        service.DeleteItems(this.TfsRequestContext, containerId, (IList<string>) paths);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    [HttpPost]
    [TraceFilter(1008141, 1008150)]
    [ClientResponseType(typeof (List<FileContainerItem>), null, null)]
    [ClientLocationId("E4F5C81E-E250-447B-9FEF-BD48471BEA5E")]
    public HttpResponseMessage CreateItems(
      int containerId,
      VssJsonCollectionWrapper<List<FileContainerItem>> items)
    {
      return this.CreateItems(containerId, items, new Guid?());
    }

    protected HttpResponseMessage CreateItems(
      int containerId,
      VssJsonCollectionWrapper<List<FileContainerItem>> items,
      Guid? scopeIdentifier)
    {
      return this.Request.CreateResponse(HttpStatusCode.NotFound);
    }

    private HttpResponseMessage CreateDownloadResponse(
      FileContainerItem item,
      string downloadFileName,
      bool allowCompression,
      bool preferRedirect = false)
    {
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      this.TfsRequestContext.UpdateTimeToFirstPage();
      CompressionType compressionType;
      if (item.ArtifactId.HasValue)
      {
        TeamFoundationFileContainerService containerService = this.TfsRequestContext.GetService<TeamFoundationFileContainerService>();
        ContainerItemBlobReference blobReference = containerService.GetBlobReference(this.TfsRequestContext, item.ArtifactId.Value, new Guid?(item.ScopeIdentifier));
        (IDomainId domainId, DedupIdentifier dedupId) = TeamFoundationFileContainerService.GetDomainIdAndDedupIdFromArtifactHash(blobReference.ArtifactHash);
        bool requestCompression = blobReference.CompressionType == BlobCompressionType.GZip & allowCompression && this.TfsRequestContext.IsFeatureEnabled("VisualStudio.FileContainerService.GZipBlobStreamsForDownload");
        if (preferRedirect && this.TfsRequestContext.IsFeatureEnabled("FileContainer.RedirectToContentStitcher"))
        {
          HttpResponseMessage downloadResponse = this.TfsRequestContext.RunSynchronously<HttpResponseMessage>((Func<Task<HttpResponseMessage>>) (async () =>
          {
            string downloadFileName1 = ContainersController.GetDownloadFileName(item.ContainerId, item.Path, downloadFileName, false);
            Uri stitcherRedirectAsync = await TeamFoundationFileContainerService.GetContentStitcherRedirectAsync(this.TfsRequestContext, domainId, dedupId, downloadFileName1);
            return (object) stitcherRedirectAsync != null ? await this.Redirect(stitcherRedirectAsync).ExecuteAsync(new CancellationToken()) : (HttpResponseMessage) null;
          }));
          if (downloadResponse != null)
          {
            ContainersController.IncrementFileDownloadCounters(item);
            return downloadResponse;
          }
        }
        Stream blobStream = this.TfsRequestContext.RunSynchronously<Stream>((Func<Task<Stream>>) (() => containerService.GetStreamFromBlobStoreAsync(this.TfsRequestContext, requestCompression, domainId, dedupId)));
        if (blobReference.CompressionType == BlobCompressionType.None & allowCompression && this.TfsRequestContext.IsFeatureEnabled("VisualStudio.FileContainerService.GZipBlobStreamsForDownload"))
        {
          response.Content = (HttpContent) new PushStreamContent((Func<Stream, HttpContent, TransportContext, Task>) (async (stream, httpContent, transportContext) =>
          {
            using (stream)
            {
              using (GZipStream gzipStream = new GZipStream(stream, CompressionLevel.Fastest))
                await blobStream.CopyToAsync((Stream) gzipStream);
            }
          }));
          compressionType = CompressionType.GZip;
        }
        else
        {
          response.Content = (HttpContent) new StreamContent(blobStream);
          compressionType = blobReference.CompressionType == BlobCompressionType.GZip ? CompressionType.GZip : CompressionType.None;
        }
      }
      else
      {
        TeamFoundationFileService service = this.TfsRequestContext.GetService<TeamFoundationFileService>();
        response.Content = (HttpContent) new StreamContent(service.RetrieveFile(this.TfsRequestContext, (long) item.FileId, allowCompression, out byte[] _, out long _, out compressionType));
      }
      if (compressionType == CompressionType.GZip)
        response.Content.Headers.ContentEncoding.Add("gzip");
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      string downloadFileName2 = ContainersController.GetDownloadFileName(item.ContainerId, item.Path, downloadFileName, false);
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(downloadFileName2);
      ContainersController.IncrementFileDownloadCounters(item);
      return response;
    }

    private static void IncrementFileDownloadCounters(FileContainerItem item)
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_TotalDownloads").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_DownloadSec").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_TotalBytesDownloaded").IncrementBy(item.FileLength);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_BytesDownloadedSec").IncrementBy(item.FileLength);
    }

    private HttpResponseMessage CreateZipDownloadResponse(
      long containerId,
      string path,
      string downloadFileName,
      Guid? scopeIdentifier,
      bool saveAbsolutePath = true)
    {
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      TeamFoundationFileContainerService containerService = this.TfsRequestContext.GetService<TeamFoundationFileContainerService>();
      response.Content = (HttpContent) new PushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        try
        {
          this.TfsRequestContext.UpdateTimeToFirstPage();
          if (scopeIdentifier.HasValue)
            containerService.WriteContents(this.TfsRequestContext, containerId, path, stream, scopeIdentifier.Value, saveAbsolutePath, true);
          else
            containerService.WriteContents(this.TfsRequestContext, containerId, path, stream, saveAbsolutePath: saveAbsolutePath);
        }
        finally
        {
          stream?.Dispose();
        }
      }), new MediaTypeHeaderValue("application/zip"));
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(ContainersController.GetDownloadFileName(containerId, path, downloadFileName, true));
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_TotalDownloads").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_DownloadSec").Increment();
      return response;
    }

    private static string GetDownloadFileName(
      long containerId,
      string path,
      string downloadFileName,
      bool isZip)
    {
      string downloadFileName1;
      if (!string.IsNullOrEmpty(downloadFileName))
        downloadFileName1 = ContainersController.IsValidFileName(downloadFileName) ? downloadFileName : throw new ArgumentException(FrameworkResources.InvalidQueryParamFileName((object) downloadFileName, (object) nameof (downloadFileName)));
      else
        downloadFileName1 = !string.IsNullOrEmpty(path) ? ContainersController.ExtractFilename(path) : containerId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      if (isZip && !downloadFileName1.EndsWith(".zip", true, CultureInfo.InvariantCulture))
        downloadFileName1 += ".zip";
      return downloadFileName1;
    }

    private static bool IsValidFileName(string fileName)
    {
      if (string.IsNullOrEmpty(fileName))
        return false;
      foreach (char ch in fileName)
      {
        if (Array.IndexOf<char>(ContainersController.s_invalidFileChars, ch) > -1)
          return false;
      }
      return true;
    }

    private static string ExtractFilename(string path)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      string str = path.TrimEnd('/');
      int num = str.Length != 0 ? str.LastIndexOf('/') : throw new ArgumentException(FrameworkResources.CannotExtractFilenameFromFileContainerPath((object) path));
      return num >= 0 ? str.Substring(num + 1) : str;
    }
  }
}
