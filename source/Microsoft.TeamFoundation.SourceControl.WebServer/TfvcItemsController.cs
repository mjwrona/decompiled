// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcItemsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class TfvcItemsController : TfvcApiController
  {
    private const long c_maxItemContentBytes = 5242880;
    private const int c_maxChangesetRange = 100;

    [ClientExample("GET__tfvc_items_path.json", "GET item metadata and/or content for a single item.", null, null)]
    [HttpGet]
    [ClientResponseType(typeof (TfvcItem), null, null)]
    [ClientResponseType(typeof (Stream), "GetItemText", "text/plain")]
    [ClientResponseType(typeof (Stream), "GetItemContent", "application/octet-stream")]
    [ClientResponseType(typeof (Stream), "GetItemZip", "application/zip")]
    public HttpResponseMessage GetItem(
      [ClientQueryParameter] string path,
      string fileName = null,
      bool download = false,
      string scopePath = null,
      [ClientParameterType(typeof (VersionControlRecursionType), false)] string recursionLevel = "None",
      [FromUri(Name = "$format"), ClientIgnore] string format = null,
      [ModelBinder(typeof (TfvcVersionDescriptorModelBinder))] TfvcVersionDescriptor versionDescriptor = null,
      bool includeContent = false)
    {
      this.TfsRequestContext.RequestTimeout = TimeSpan.FromMinutes(60.0);
      if (string.IsNullOrWhiteSpace(path))
        return this.GetItems(scopePath, recursionLevel, versionDescriptor: versionDescriptor);
      if (!string.IsNullOrWhiteSpace(scopePath))
        throw new ArgumentException(Resources.Get("ErrorItemAndScopePaths")).Expected(this.TfsRequestContext.ServiceName);
      if (this.ProjectId != Guid.Empty)
        path = this.ProjectScopedPath(path);
      if (this.CheckRecursionLevel(recursionLevel) != VersionControlRecursionType.None)
        throw new ArgumentException(Resources.Get("ErrorItemPathWithRecursion")).Expected(this.TfsRequestContext.ServiceName);
      path = VersionControlPath.GetFullPath(path);
      RequestMediaType requestMediaType = MediaTypeFormatUtility.GetPrioritizedAcceptHeaders(this.Request, new List<RequestMediaType>()
      {
        RequestMediaType.Json,
        RequestMediaType.Zip,
        RequestMediaType.Text,
        RequestMediaType.OctetStream,
        RequestMediaType.None
      }).First<RequestMediaType>();
      long scanBytesForEncoding = 0;
      if (requestMediaType == RequestMediaType.Text || requestMediaType == RequestMediaType.Json & includeContent)
      {
        long to;
        this.GetRequestedByteRange(out long _, out to);
        scanBytesForEncoding = to;
      }
      TfvcItem items = TfvcItemUtility.GetItemsCollection(this.TfsRequestContext, this.Url, path, versionDescriptor, VersionControlRecursionType.None, true, true, scanBytesForEncoding)[0];
      if (requestMediaType != RequestMediaType.Zip && (items.IsFolder || items.IsBranch))
        requestMediaType = RequestMediaType.Json;
      this.TfsRequestContext.UpdateTimeToFirstPage();
      switch (requestMediaType)
      {
        case RequestMediaType.None:
        case RequestMediaType.OctetStream:
          if (items.IsFolder || items.IsBranch)
            return this.Request.CreateResponse<TfvcItem>(HttpStatusCode.OK, items);
          if (string.IsNullOrWhiteSpace(fileName))
            fileName = VersionControlPath.GetFileName(items.Path);
          return this.CreateFileResponse(items, fileName, download, false);
        case RequestMediaType.Zip:
          return this.CreateZipDownloadResponse(path, versionDescriptor, fileName, items);
        case RequestMediaType.Text:
          if (string.IsNullOrWhiteSpace(fileName))
            fileName = VersionControlPath.GetFileName(items.Path);
          return download ? this.CreateFileResponse(items, fileName, true, false) : this.CreateFileResponse(items, fileName, false, true);
        default:
          ContentRangeHeaderValue contentRange = (ContentRangeHeaderValue) null;
          if (includeContent)
            items.Content = this.GetItemContent(items, out contentRange);
          items.Url = this.Url.RestLink(this.TfsRequestContext, TfvcConstants.TfvcItemsLocationId, (object) new
          {
            path = path,
            versionType = versionDescriptor.VersionType,
            version = versionDescriptor.Version,
            versionOptions = versionDescriptor.VersionOption
          });
          HttpResponseMessage response = this.Request.CreateResponse<TfvcItem>(HttpStatusCode.OK, items);
          if (contentRange != null)
          {
            response.Content.Headers.ContentRange = contentRange;
            response.StatusCode = HttpStatusCode.PartialContent;
          }
          return response;
      }
    }

    [HttpGet]
    [ClientResponseType(typeof (List<TfvcItem>), null, null)]
    [ClientExample("GET__tfvc_items_scopePath-_folder_.json", "A folder", null, null)]
    [ClientExample("GET__tfvc_items_scopePath-_folder__recursionLevel-_recursionLevel_.json", "A folder and its children", null, null)]
    [ClientExample("GET__tfvc_items__itempath__versionType-Changeset_version-_version_.json", "Changeset version", null, null)]
    public HttpResponseMessage GetItems(
      string scopePath = null,
      [ClientParameterType(typeof (VersionControlRecursionType), false)] string recursionLevel = "None",
      bool includeLinks = false,
      [ModelBinder(typeof (TfvcVersionDescriptorModelBinder))] TfvcVersionDescriptor versionDescriptor = null)
    {
      if (this.ProjectId != Guid.Empty)
        scopePath = this.ProjectScopedPath(scopePath);
      if (string.IsNullOrWhiteSpace(scopePath))
        scopePath = "$/";
      VersionControlPath.ValidatePath(scopePath);
      VersionControlRecursionType controlRecursionType = this.CheckRecursionLevel(recursionLevel);
      VersionControlRecursionType vcRecursionType = controlRecursionType == VersionControlRecursionType.None ? VersionControlRecursionType.OneLevel : controlRecursionType;
      VersionSpec versionSpec = TfvcVersionSpecUtility.GetVersionSpec(this.TfsRequestContext, versionDescriptor);
      return this.GenerateResponse<TfvcItem>(TfvcItemUtility.GetItems(this.TfsRequestContext, this.Url, scopePath, versionSpec, TfvcCommonUtility.ConvertVersionControlRecursionType(vcRecursionType), DeletedState.NonDeleted, includeLinks));
    }

    [HttpGet]
    [ClientIgnore]
    [ClientResponseType(typeof (IPagedList<TfvcItem>), null, null)]
    public HttpResponseMessage GetItemsPaged(
      [FromUri(Name = "$top")] int top,
      string scopePath = null,
      int? changeset = null,
      string continuationToken = null)
    {
      if (string.IsNullOrWhiteSpace(scopePath))
        throw new InvalidArgumentValueException(Resources.Format("ScopePathRequiredForPagingItems", (object) nameof (scopePath)));
      TfvcItemsContinuationToken token = (TfvcItemsContinuationToken) null;
      if (continuationToken != null && !TfvcItemsContinuationToken.TryParseContinuationToken(scopePath, continuationToken, out token))
        return this.Request.CreateErrorResponse((Exception) new InvalidArgumentValueException(Resources.Format("InvalidContinuationToken", (object) continuationToken)), (IHttpController) this);
      int changesetId;
      if (token != null)
      {
        if (changeset.HasValue && changeset.Value != token.ChangesetId)
          return this.Request.CreateErrorResponse((Exception) new InvalidArgumentValueException(Resources.Format("InvalidContinuationToken", (object) continuationToken)), (IHttpController) this);
        changesetId = token.ChangesetId;
      }
      else
        changesetId = !changeset.HasValue ? TfvcChangesetUtility.GetLatestChangesetId(this.TfsRequestContext) : changeset.Value;
      if (this.ProjectId != Guid.Empty)
        scopePath = this.ProjectScopedPath(scopePath);
      string lastItemPaged;
      List<TfvcItem> list;
      try
      {
        list = TfvcItemUtility.GetItemsPaged(this.TfsRequestContext, this.Url, scopePath, changesetId, top, token == null ? (string) null : token.ContinuationPath, out lastItemPaged).ToList<TfvcItem>();
        if (token == null)
        {
          if (list != null)
          {
            if (list.Any<TfvcItem>())
              goto label_17;
          }
          return this.Request.CreateErrorResponse((Exception) new ItemNotFoundException(scopePath), (IHttpController) this);
        }
      }
      catch (InvalidLastServerItemForPaging ex)
      {
        return this.Request.CreateErrorResponse((Exception) new InvalidArgumentValueException(Resources.Format("InvalidContinuationToken", (object) continuationToken)), (IHttpController) this);
      }
label_17:
      HttpResponseMessage response = this.GenerateResponse<TfvcItem>((IEnumerable<TfvcItem>) list);
      if (!string.IsNullOrWhiteSpace(lastItemPaged))
      {
        TfvcItemsContinuationToken continuationToken1 = new TfvcItemsContinuationToken(scopePath, lastItemPaged, changesetId);
        response.Headers.Add("x-ms-continuationtoken", continuationToken1.ToString());
      }
      return response;
    }

    [HttpGet]
    [ClientIgnore]
    [ClientResponseType(typeof (IPagedList<TfvcItemPreviousHash>), null, null)]
    public HttpResponseMessage GetItemsByChangesetPaged(
      [FromUri(Name = "$top")] int top,
      [FromUri(Name = "baseChangeset")] int baseChangeset,
      int? targetChangeset = null,
      string scopePath = null,
      string continuationToken = null)
    {
      if (string.IsNullOrWhiteSpace(scopePath))
        throw new InvalidArgumentValueException(Resources.Format("ScopePathRequiredForPagingItems", (object) nameof (scopePath)));
      if (top < 1 || top > 10000)
        throw new InvalidArgumentValueException(Resources.Format("TopOutOfRangeException"));
      TfvcItemsByChangesetContinuationToken token = (TfvcItemsByChangesetContinuationToken) null;
      if (continuationToken != null && !TfvcItemsByChangesetContinuationToken.TryParseContinuationToken(scopePath, continuationToken, out token))
        return this.Request.CreateErrorResponse((Exception) new InvalidArgumentValueException(Resources.Format("InvalidContinuationToken", (object) continuationToken)), (IHttpController) this);
      int baseChangesetId;
      int targetChangesetId;
      if (token != null)
      {
        if (baseChangeset != token.BaseChangesetId)
          return this.Request.CreateErrorResponse((Exception) new InvalidArgumentValueException(Resources.Format("InvalidContinuationToken", (object) continuationToken)), (IHttpController) this);
        baseChangesetId = token.BaseChangesetId;
        targetChangesetId = token.TargetChangesetId;
      }
      else
      {
        baseChangesetId = baseChangeset;
        targetChangesetId = !targetChangeset.HasValue ? TfvcChangesetUtility.GetLatestChangesetId(this.TfsRequestContext) : targetChangeset.Value;
      }
      if (baseChangesetId > targetChangesetId)
        throw new InvalidArgumentValueException(Resources.Format("BaseChangesetHigherThanTargetChangesetException"));
      if (targetChangesetId - baseChangesetId > 100)
        throw new InvalidArgumentValueException(Resources.Format("ChangesetRangeTooLarge", (object) baseChangesetId, (object) targetChangesetId, (object) 100));
      if (this.ProjectId != Guid.Empty)
        scopePath = this.ProjectScopedPath(scopePath);
      string lastItemPaged;
      List<TfvcItemPreviousHash> list;
      try
      {
        list = TfvcItemUtility.GetItemsByChangesetPaged(this.TfsRequestContext, this.Url, scopePath, baseChangesetId, targetChangesetId, top, token == null ? (string) null : token.ContinuationPath, out lastItemPaged).ToList<TfvcItemPreviousHash>();
      }
      catch (InvalidLastServerItemForPaging ex)
      {
        return this.Request.CreateErrorResponse((Exception) new InvalidArgumentValueException(Resources.Format("InvalidContinuationToken", (object) continuationToken)), (IHttpController) this);
      }
      HttpResponseMessage response = this.GenerateResponse<TfvcItemPreviousHash>((IEnumerable<TfvcItemPreviousHash>) list);
      if (!string.IsNullOrWhiteSpace(lastItemPaged))
      {
        TfvcItemsByChangesetContinuationToken continuationToken1 = new TfvcItemsByChangesetContinuationToken(scopePath, lastItemPaged, baseChangesetId, targetChangesetId);
        response.Headers.Add("x-ms-continuationtoken", continuationToken1.ToString());
      }
      return response;
    }

    private VersionControlRecursionType CheckRecursionLevel(string recursionLevel)
    {
      VersionControlRecursionType result;
      if (!System.Enum.TryParse<VersionControlRecursionType>(recursionLevel, true, out result))
        throw new ArgumentException(Resources.Format("InvalidQueryParam", (object) recursionLevel, (object) nameof (recursionLevel), (object) string.Join(",", System.Enum.GetNames(typeof (VersionControlRecursionType))))).Expected(this.TfsRequestContext.ServiceName);
      return result;
    }

    private HttpResponseMessage CreateZipDownloadResponse(
      string path,
      TfvcVersionDescriptor versionDescriptor,
      string zipFileName,
      TfvcItem tfvcItem)
    {
      VersionControlPath.ValidatePath(path);
      VersionSpec versionSpec = TfvcVersionSpecUtility.GetVersionSpec(this.TfsRequestContext, versionDescriptor);
      IEnumerable<TfvcItem> items = TfvcItemUtility.GetItems(this.TfsRequestContext, this.Url, path, versionSpec, RecursionType.Full, DeletedState.NonDeleted, false);
      if (string.IsNullOrWhiteSpace(zipFileName))
      {
        zipFileName = VersionControlPath.GetFileName(tfvcItem.Path);
        if (string.IsNullOrWhiteSpace(zipFileName))
          zipFileName = this.TfsRequestContext.ServiceHost.Name;
      }
      zipFileName += Resources.Get("ZipFileExtension");
      return TfvcFileUtility.CreateZipDownloadResponse(this.Request, this.TfsRequestContext, items, zipFileName, path);
    }

    private HttpResponseMessage CreateFileResponse(
      TfvcItem tfvcItem,
      string fileName,
      bool toDownload,
      bool isText)
    {
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      string contentType = MimeMapper.GetContentType(tfvcItem.ContentMetadata.Extension);
      string responseContentType = MediaTypeFormatUtility.GetSafeResponseContentType(string.IsNullOrEmpty(contentType) ? MediaTypeFormatUtility.AcceptHeaderToString(RequestMediaType.OctetStream) : contentType);
      toDownload = toDownload || responseContentType == "application/octet-stream";
      if (toDownload || !isText)
      {
        response.Content = (HttpContent) new VssServerStreamContent(TfvcFileUtility.GetFileContentStream(this.TfsRequestContext, (ItemModel) tfvcItem), (object) tfvcItem);
        if (toDownload)
          response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(fileName);
      }
      else
      {
        ContentRangeHeaderValue contentRange;
        string itemContent = this.GetItemContent(tfvcItem, out contentRange);
        response.Content = (HttpContent) new VssServerStringContent(itemContent, (object) tfvcItem);
        if (contentRange != null)
        {
          response.Content.Headers.ContentRange = contentRange;
          response.StatusCode = HttpStatusCode.PartialContent;
        }
      }
      response.Content.Headers.ContentType = new MediaTypeHeaderValue(responseContentType);
      return response;
    }

    private string GetItemContent(TfvcItem tfvcItem, out ContentRangeHeaderValue contentRange)
    {
      long from;
      long to;
      this.GetRequestedByteRange(out from, out to);
      string itemContent;
      using (RangeStream contentStream = new RangeStream(new Lazy<Stream>((Func<Stream>) (() => TfvcFileUtility.GetFileContentStream(this.TfsRequestContext, (ItemModel) tfvcItem))), from, (int) (to - from)))
      {
        itemContent = VersionControlFileReader.ReadFileContent((Stream) contentStream, tfvcItem.ContentMetadata.Encoding);
        contentRange = contentStream.Truncated ? new ContentRangeHeaderValue(from, from + (long) contentStream.BytesRead) : (ContentRangeHeaderValue) null;
      }
      return itemContent;
    }

    private void GetRequestedByteRange(out long from, out long to)
    {
      from = 0L;
      to = 5242880L;
      RangeHeaderValue range = this.Request.Headers.Range;
      if (range != null)
      {
        RangeItemHeaderValue rangeItemHeaderValue = range.Ranges.FirstOrDefault<RangeItemHeaderValue>();
        if (rangeItemHeaderValue != null)
        {
          ref long local1 = ref from;
          long? nullable = rangeItemHeaderValue.From;
          long num1 = nullable ?? from;
          local1 = num1;
          ref long local2 = ref to;
          nullable = rangeItemHeaderValue.To;
          long num2 = nullable ?? to;
          local2 = num2;
        }
      }
      if (to - from <= 5242880L)
        return;
      to = from + 5242880L;
    }
  }
}
