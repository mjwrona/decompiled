// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitLfsLocksHandler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Lfs;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitLfsLocksHandler : GitLfsHandler
  {
    private const int c_defaultLimit = 100;

    public GitLfsLocksHandler()
    {
    }

    public GitLfsLocksHandler(HttpContextBase context)
      : base(context)
    {
    }

    protected override string Layer => nameof (GitLfsLocksHandler);

    protected override GitLfsHandler.LfsHttpResult GetResult()
    {
      GitLfsHandler.LfsHttpResult result1 = this.ErrorResponse(HttpStatusCode.BadRequest);
      string httpMethod = this.HandlerHttpContext.Request.HttpMethod;
      if (!this.AcceptsMediaType("application/vnd.git-lfs+json") || httpMethod.Equals(HttpMethod.Post.Method, StringComparison.OrdinalIgnoreCase) && !this.ContentTypeMatches("application/vnd.git-lfs+json"))
        result1 = this.ErrorResponse(HttpStatusCode.UnsupportedMediaType, Resources.Format("UnsupportedMediaType", (object) "application/vnd.git-lfs+json"));
      else if (this.RepoInfo.IsValid)
      {
        if (httpMethod.Equals(HttpMethod.Post.Method, StringComparison.OrdinalIgnoreCase))
        {
          object obj1;
          string s;
          if (this.RouteValues.TryGetValue("lockId", out obj1) && (s = obj1.ToString()).Length > 0)
          {
            if (s.Equals("verify"))
            {
              this.EnterMethod("VerifyLocks");
              result1 = this.VerifyLocks();
            }
            else
            {
              int result2;
              object obj2;
              if (int.TryParse(s, out result2) && this.RouteValues.TryGetValue("lockAction", out obj2) && obj2.ToString().Equals("unlock"))
              {
                this.EnterMethod("Unlock");
                result1 = this.Unlock(result2);
              }
            }
          }
          else
          {
            this.EnterMethod("CreateLock");
            result1 = this.CreateLock();
          }
        }
        else if (httpMethod.Equals(HttpMethod.Get.Method, StringComparison.OrdinalIgnoreCase))
        {
          object obj;
          string s;
          if (this.RouteValues.TryGetValue("lockId", out obj) && (s = obj.ToString()).Length > 0)
          {
            int result3;
            if (int.TryParse(s, out result3))
            {
              this.EnterMethod("GetLock");
              result1 = this.GetLock(result3);
            }
          }
          else
          {
            this.EnterMethod("ListLocks");
            result1 = this.ListLocks();
          }
        }
      }
      return result1;
    }

    protected override Action<string> GetReportAction() => (Action<string>) (errorMessage => this.WriteHttpResult(this.ErrorResponse(HttpStatusCode.BadRequest, errorMessage)));

    protected override Exception HandleException(
      Exception exception,
      string exceptionHeader,
      int statusCode,
      bool closeResponse)
    {
      this.HandlerHttpContext.Response.StatusCode = statusCode;
      return exception;
    }

    private RouteValueDictionary RouteValues => this.HandlerHttpContext.Request.RequestContext.RouteData.Values;

    private NameValueCollection Params => this.HandlerHttpContext.Request.Params;

    private GitLfsHandler.LfsHttpResult ListLocks()
    {
      int result1 = 0;
      int result2 = 100;
      int result3 = -1;
      string path = (string) null;
      if (this.Params != null)
      {
        foreach (string key in this.Params.Keys)
        {
          string s = this.Params[key];
          switch (key)
          {
            case "id":
              if (!int.TryParse(s, out result3))
                return this.ErrorResponse(HttpStatusCode.BadRequest);
              continue;
            case "path":
              path = s;
              continue;
            case "cursor":
              if (!int.TryParse(s, out result1))
                return this.ErrorResponse(HttpStatusCode.BadRequest);
              continue;
            case "limit":
              if (!int.TryParse(s, out result2))
                return this.ErrorResponse(HttpStatusCode.BadRequest);
              continue;
            default:
              continue;
          }
        }
      }
      GitLfsLockCollection resultObj = new GitLfsLockCollection();
      using (ITfsGitRepository repository = this.FindRepository())
      {
        if (result3 >= 0)
        {
          LfsLock lfsLock = this.RequestContext.GetService<IGitLfsLockService>().GetLock(this.RequestContext, repository, result3);
          if (lfsLock != null)
            resultObj.Locks = (IEnumerable<GitLfsLock>) new GitLfsLock[1]
            {
              this.TranslateInternalLock(lfsLock)
            };
          else
            resultObj.Locks = (IEnumerable<GitLfsLock>) Array.Empty<GitLfsLock>();
          return this.GetLfsHttpResult<GitLfsLockCollection>(HttpStatusCode.OK, resultObj);
        }
        GitLfsGetLocksResult locks = this.RequestContext.GetService<IGitLfsLockService>().GetLocks(this.RequestContext, repository, result1, result2, path);
        List<GitLfsLock> gitLfsLockList = new List<GitLfsLock>();
        for (int index = 0; index < locks.Locks.Count; ++index)
          gitLfsLockList.Add(this.TranslateInternalLock(locks.Locks[index]));
        resultObj.Locks = (IEnumerable<GitLfsLock>) gitLfsLockList;
        GitLfsLockCollection lfsLockCollection = resultObj;
        ref readonly int? local = ref locks.NextCursor;
        string str = local.HasValue ? local.GetValueOrDefault().ToString() : (string) null;
        lfsLockCollection.NextCursor = str;
      }
      return this.GetLfsHttpResult<GitLfsLockCollection>(HttpStatusCode.OK, resultObj);
    }

    private GitLfsHandler.LfsHttpResult VerifyLocks()
    {
      GitLfsVerifyLockRequest verifyLockRequest;
      using (Stream bufferlessInputStream = this.HandlerHttpContext.Request.GetBufferlessInputStream())
      {
        using (StreamReader streamReader = new StreamReader(bufferlessInputStream))
        {
          if (bufferlessInputStream.Length == 0L)
          {
            verifyLockRequest = new GitLfsVerifyLockRequest()
            {
              Cursor = 0,
              Limit = 100
            };
          }
          else
          {
            GitLfsHandler.LfsHttpResult lfsHttpResult = this.TryDeserializeJson<GitLfsVerifyLockRequest>(streamReader.ReadToEnd(), out verifyLockRequest);
            if (lfsHttpResult != null)
              return lfsHttpResult;
          }
        }
      }
      if (verifyLockRequest.Limit <= 0)
        verifyLockRequest.Limit = 100;
      GitLfsVerifyLocksResult verifyLocksResult;
      using (ITfsGitRepository repository = this.FindRepository())
        verifyLocksResult = this.RequestContext.GetService<IGitLfsLockService>().VerifyLocks(this.RequestContext, repository, verifyLockRequest.Cursor, verifyLockRequest.Limit);
      List<GitLfsLock> gitLfsLockList1 = new List<GitLfsLock>();
      for (int index = 0; index < verifyLocksResult.Ours.Count; ++index)
        gitLfsLockList1.Add(this.TranslateInternalLock(verifyLocksResult.Ours[index]));
      List<GitLfsLock> gitLfsLockList2 = new List<GitLfsLock>();
      int index1 = 0;
      while (true)
      {
        int num = index1;
        int? count = verifyLocksResult.Theirs?.Count;
        int valueOrDefault = count.GetValueOrDefault();
        if (num < valueOrDefault & count.HasValue)
        {
          gitLfsLockList2.Add(this.TranslateInternalLock(verifyLocksResult.Theirs[index1]));
          ++index1;
        }
        else
          break;
      }
      GitLfsLockCollection resultObj = new GitLfsLockCollection();
      resultObj.Ours = (IEnumerable<GitLfsLock>) gitLfsLockList1;
      resultObj.Theirs = (IEnumerable<GitLfsLock>) gitLfsLockList2;
      ref readonly int? local = ref verifyLocksResult.NextCursor;
      resultObj.NextCursor = local.HasValue ? local.GetValueOrDefault().ToString() : (string) null;
      return this.GetLfsHttpResult<GitLfsLockCollection>(HttpStatusCode.OK, resultObj);
    }

    private GitLfsHandler.LfsHttpResult GetLock(int lockId)
    {
      LfsLock lfsLock = (LfsLock) null;
      using (ITfsGitRepository repository = this.FindRepository())
        lfsLock = this.RequestContext.GetService<IGitLfsLockService>().GetLock(this.RequestContext, repository, lockId);
      if (lfsLock == null)
        return this.ErrorResponse(HttpStatusCode.NotFound, Resources.Format("GitLfsLockDoesNotExist", (object) lockId));
      return this.GetLfsHttpResult<GitLfsLockResponse>(HttpStatusCode.OK, new GitLfsLockResponse()
      {
        Lock = this.TranslateInternalLock(lfsLock)
      });
    }

    private GitLfsHandler.LfsHttpResult CreateLock()
    {
      GitLfsLock gitLfsLock;
      using (StreamReader streamReader = new StreamReader(this.HandlerHttpContext.Request.GetBufferlessInputStream()))
      {
        GitLfsHandler.LfsHttpResult lfsHttpResult = this.TryDeserializeJson<GitLfsLock>(streamReader.ReadToEnd(), out gitLfsLock);
        if (lfsHttpResult != null)
          return lfsHttpResult;
      }
      if (gitLfsLock == null)
        return this.ErrorResponse(HttpStatusCode.BadRequest, Resources.Get("ExpectedNonemptyPostBody"));
      if (string.IsNullOrEmpty(gitLfsLock.Path))
        return this.ErrorResponse(HttpStatusCode.BadRequest, Resources.Get("LfsLocksMustSpecifyPath"));
      GitLfsCreateLockResult createLockResult;
      using (ITfsGitRepository repository = this.FindRepository())
        createLockResult = this.RequestContext.GetService<IGitLfsLockService>().TryCreateLock(this.RequestContext, repository, gitLfsLock.Path);
      switch (createLockResult.Status)
      {
        case GitLfsCreateLockResultStatus.CreateFailed:
          this.RequestContext.Status = (Exception) new InvalidOperationException(Resources.Get("LfsLockFailedToCreate"));
          return this.ErrorResponse(HttpStatusCode.InternalServerError, Resources.Get("LfsLockFailedToCreate"));
        case GitLfsCreateLockResultStatus.NotAllowedToCreate:
          return this.ErrorResponse(HttpStatusCode.Forbidden, Resources.Get("LfsLockRequireContributeToLock"));
        case GitLfsCreateLockResultStatus.PathTooLong:
          return this.ErrorResponse(HttpStatusCode.BadRequest, Resources.Format("LfsLockPathTooLong", (object) 1024));
        default:
          GitLfsLockResponse resultObj = new GitLfsLockResponse()
          {
            Lock = this.TranslateInternalLock(createLockResult.Lock)
          };
          HttpStatusCode statusCode = HttpStatusCode.Created;
          if (createLockResult.Status == GitLfsCreateLockResultStatus.Conflict)
          {
            statusCode = HttpStatusCode.Conflict;
            resultObj.Message = Resources.Get("LfsLockConflict");
          }
          return this.GetLfsHttpResult<GitLfsLockResponse>(statusCode, resultObj);
      }
    }

    private GitLfsHandler.LfsHttpResult Unlock(int lockId)
    {
      GitLfsDeleteLockRequest deleteLockRequest;
      using (Stream bufferlessInputStream = this.HandlerHttpContext.Request.GetBufferlessInputStream())
      {
        using (StreamReader streamReader = new StreamReader(bufferlessInputStream))
        {
          if (bufferlessInputStream.Length == 0L)
          {
            deleteLockRequest = new GitLfsDeleteLockRequest()
            {
              Force = false
            };
          }
          else
          {
            GitLfsHandler.LfsHttpResult lfsHttpResult = this.TryDeserializeJson<GitLfsDeleteLockRequest>(streamReader.ReadToEnd(), out deleteLockRequest);
            if (lfsHttpResult != null)
              return lfsHttpResult;
          }
        }
      }
      GitLfsDeleteLockResult deleteLockResult;
      using (ITfsGitRepository repository = this.FindRepository())
        deleteLockResult = this.RequestContext.GetService<IGitLfsLockService>().DeleteLock(this.RequestContext, repository, lockId, deleteLockRequest.Force);
      if (deleteLockResult.Status == GitLfsDeleteLockResultStatus.Deleted)
        return this.GetLfsHttpResult<GitLfsLockResponse>(HttpStatusCode.OK, new GitLfsLockResponse()
        {
          Lock = this.TranslateInternalLock(deleteLockResult.Lock)
        });
      string message = (string) null;
      HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
      switch (deleteLockResult.Status)
      {
        case GitLfsDeleteLockResultStatus.MustForceDelete:
          message = Resources.Get("LfsLockMustForceDelete");
          statusCode = HttpStatusCode.Forbidden;
          break;
        case GitLfsDeleteLockResultStatus.LockDoesNotExist:
          message = Resources.Format("GitLfsLockDoesNotExist", (object) lockId);
          statusCode = HttpStatusCode.NotFound;
          break;
        case GitLfsDeleteLockResultStatus.NotAllowedToDelete:
          message = Resources.Get("LfsLockNotAllowedToDelete");
          statusCode = HttpStatusCode.Forbidden;
          break;
        case GitLfsDeleteLockResultStatus.NotAllowedToForceDelete:
          message = Resources.Get("LfsLockNotAllowedToForceDelete");
          statusCode = HttpStatusCode.Forbidden;
          break;
      }
      return this.ErrorResponse(statusCode, message);
    }

    private GitLfsHandler.LfsHttpResult TryDeserializeJson<T>(string json, out T value) where T : class
    {
      try
      {
        value = JsonConvert.DeserializeObject<T>(json);
        return (GitLfsHandler.LfsHttpResult) null;
      }
      catch (JsonException ex)
      {
        value = default (T);
        return this.ErrorResponse(HttpStatusCode.BadRequest, "Invalid JSON: " + ex.Message);
      }
    }

    private bool ContentTypeMatches(string typeToMatch)
    {
      string str = this.HandlerHttpContext.Request.ContentType;
      int length;
      if ((length = str.IndexOf(';')) > 0)
        str = str.Substring(0, length);
      return str.Equals(typeToMatch, StringComparison.OrdinalIgnoreCase);
    }

    private GitLfsLock TranslateInternalLock(LfsLock lfsLock)
    {
      if (lfsLock == null)
        return (GitLfsLock) null;
      return new GitLfsLock()
      {
        Id = lfsLock.Id.ToString(),
        Path = lfsLock.Path,
        Owner = new GitLfsLockOwner()
        {
          Name = lfsLock.OwnerName
        },
        LockedAt = (DateTimeOffset) lfsLock.LockedAt
      };
    }
  }
}
