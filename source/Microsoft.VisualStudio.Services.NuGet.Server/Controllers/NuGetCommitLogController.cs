// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.NuGetCommitLogController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog;
using Microsoft.VisualStudio.Services.NuGet.Server.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers
{
  [ClientIgnore]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "nuget", ResourceName = "commitLog")]
  [FeatureEnabled("NuGet.CommitLog.Controller")]
  public class NuGetCommitLogController : NuGetApiController
  {
    [HttpGet]
    [ControllerMethodTraceFilter(5721810)]
    public async Task<IHttpActionResult> GetEntryAsync(string feedId, string commitId)
    {
      NuGetCommitLogController commitLogController = this;
      FeedCore feed = commitLogController.GetFeedRequest(feedId).Feed;
      PackagingCommitId commitId1 = PackagingCommitId.Parse(commitId);
      if (commitId1 == PackagingCommitId.Empty)
        return (IHttpActionResult) commitLogController.NotFound();
      CommitLogEntry entryAsync = await commitLogController.TfsRequestContext.GetService<INuGetCommitLogService>().GetEntryAsync(commitLogController.TfsRequestContext, feed, commitId1);
      return !(entryAsync.CommitOperationData is NuGetAddOperationData commitOperationData1) ? (!(entryAsync.CommitOperationData is IDeleteOperationData commitOperationData2) ? commitLogController.GetResultJson(feed, entryAsync) : commitLogController.GetResultJson(feed, entryAsync, commitOperationData2)) : commitLogController.GetResultJson(feed, entryAsync, commitOperationData1);
    }

    [HttpGet]
    [ControllerMethodTraceFilter(5721800)]
    public async Task<IHttpActionResult> GetIndexAsync(string feedId)
    {
      NuGetCommitLogController commitLogController1 = this;
      FeedCore feed = commitLogController1.GetFeedRequest(feedId).Feed;
      INuGetCommitLogService commitLogService = commitLogController1.TfsRequestContext.GetService<INuGetCommitLogService>();
      CommitLogBookmark head = await commitLogService.GetNewestCommitBookmarkAsync(commitLogController1.TfsRequestContext, feed);
      CommitLogBookmark commitBookmarkAsync = await commitLogService.GetOldestCommitBookmarkAsync(commitLogController1.TfsRequestContext, feed);
      if (head == CommitLogBookmark.Empty)
        return (IHttpActionResult) commitLogController1.StatusCode(HttpStatusCode.NoContent);
      NuGetCommitLogController commitLogController2 = commitLogController1;
      CommitLogBookmark commitLogBookmark1 = head;
      CommitLogBookmark commitLogBookmark2 = commitBookmarkAsync;
      IVssRequestContext tfsRequestContext1 = commitLogController1.TfsRequestContext;
      Guid id = feed.Id;
      string feedId1 = id.ToString();
      PackagingCommitId commitId1 = head.CommitId;
      var data1 = new
      {
        href = NuGetUriBuilder.GetCommitLogEntryUri(tfsRequestContext1, feedId1, commitId1).AbsoluteUri
      };
      IVssRequestContext tfsRequestContext2 = commitLogController1.TfsRequestContext;
      id = feed.Id;
      string feedId2 = id.ToString();
      PackagingCommitId commitId2 = commitBookmarkAsync.CommitId;
      var data2 = new
      {
        href = NuGetUriBuilder.GetCommitLogEntryUri(tfsRequestContext2, feedId2, commitId2).AbsoluteUri
      };
      var data3 = new
      {
        headCommit = data1,
        tailCommit = data2
      };
      var content = new
      {
        headCommitId = commitLogBookmark1,
        tailCommitId = commitLogBookmark2,
        _links = data3
      };
      return (IHttpActionResult) commitLogController2.Json(content);
    }

    private IHttpActionResult GetResultJson(FeedCore feed, CommitLogEntry entry) => (IHttpActionResult) this.Json(new
    {
      commitId = entry.CommitId,
      previousCommitId = entry.PreviousCommitId,
      nextCommitId = entry.NextCommitId,
      createdDate = entry.CreatedDate,
      modifiedDate = entry.ModifiedDate,
      _links = new
      {
        previousCommit = new
        {
          href = NuGetUriBuilder.GetCommitLogEntryUri(this.TfsRequestContext, feed.Id.ToString(), entry.PreviousCommitId).AbsoluteUri
        },
        nextCommit = new
        {
          href = NuGetUriBuilder.GetCommitLogEntryUri(this.TfsRequestContext, feed.Id.ToString(), entry.NextCommitId).AbsoluteUri
        }
      }
    });

    private IHttpActionResult GetResultJson(
      FeedCore feed,
      CommitLogEntry entry,
      NuGetAddOperationData addOperationData)
    {
      return (IHttpActionResult) this.Json(new
      {
        commitId = entry.CommitId,
        previousCommitId = entry.PreviousCommitId,
        nextCommitId = entry.NextCommitId,
        createdDate = entry.CreatedDate,
        modifiedDate = entry.ModifiedDate,
        nuspec = Encoding.UTF8.GetString(addOperationData.NuspecBytes),
        _links = new
        {
          previousCommit = new
          {
            href = NuGetUriBuilder.GetCommitLogEntryUri(this.TfsRequestContext, feed.Id.ToString(), entry.PreviousCommitId).AbsoluteUri
          },
          nextCommit = new
          {
            href = NuGetUriBuilder.GetCommitLogEntryUri(this.TfsRequestContext, feed.Id.ToString(), entry.NextCommitId).AbsoluteUri
          },
          registration = new
          {
            href = NuGetUriBuilder.GetPackageVersionNewRegistrationUri(this.TfsRequestContext, feed.Id.ToString(), addOperationData.Metadata.Identity).AbsoluteUri
          }
        }
      });
    }

    private IHttpActionResult GetResultJson(
      FeedCore feed,
      CommitLogEntry entry,
      IDeleteOperationData deleteOperationData)
    {
      return (IHttpActionResult) this.Json(new
      {
        commitId = entry.CommitId,
        previousCommitId = entry.PreviousCommitId,
        nextCommitId = entry.NextCommitId,
        createdDate = entry.CreatedDate,
        modifiedDate = entry.ModifiedDate,
        deletedDate = deleteOperationData.DeletedDate,
        _links = new
        {
          previousCommit = new
          {
            href = NuGetUriBuilder.GetCommitLogEntryUri(this.TfsRequestContext, feed.Id.ToString(), entry.PreviousCommitId).AbsoluteUri
          },
          nextCommit = new
          {
            href = NuGetUriBuilder.GetCommitLogEntryUri(this.TfsRequestContext, feed.Id.ToString(), entry.NextCommitId).AbsoluteUri
          }
        }
      });
    }
  }
}
