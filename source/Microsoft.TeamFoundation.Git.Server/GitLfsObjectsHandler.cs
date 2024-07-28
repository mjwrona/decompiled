// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitLfsObjectsHandler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  [GitLfsPublicProjectRequestRestrictions]
  [GitLfsPublicProjectRequestRestrictions(AuthenticationMechanisms.Basic | AuthenticationMechanisms.Windows | AuthenticationMechanisms.Federated | AuthenticationMechanisms.OAuth, UserAgentFilterType.StartsWith, "git/")]
  [GitLfsPublicProjectRequestRestrictions(AuthenticationMechanisms.Basic | AuthenticationMechanisms.Windows | AuthenticationMechanisms.Federated | AuthenticationMechanisms.OAuth, UserAgentFilterType.StartsWith, "git-lfs/")]
  [GitLfsPublicProjectRequestRestrictions(AuthenticationMechanisms.Basic | AuthenticationMechanisms.Windows | AuthenticationMechanisms.Federated | AuthenticationMechanisms.OAuth, UserAgentFilterType.StartsWith, "xcode/")]
  [GitLfsPublicProjectRequestRestrictions(AuthenticationMechanisms.Basic, UserAgentFilterType.StartsWith, "jgit/")]
  public class GitLfsObjectsHandler : GitLfsHandler
  {
    private const string FfDrainConnectionOnUpload = "Git.Lfs.M203.DrainConnectionOnUpload";
    private GitLfsObjectRefBatch m_lfsObjectRefs;
    private GitLfsHandler.LfsHttpResult m_lfsHttpResult;
    private string m_lfsOperation;

    public GitLfsObjectsHandler() => this.Initialize(this.HandlerHttpContext);

    public GitLfsObjectsHandler(HttpContextBase context)
      : base(context)
    {
      this.Initialize(context);
    }

    private void Initialize(HttpContextBase context)
    {
      string httpMethod = context.Request.HttpMethod;
      string str = context.Request.RequestContext.RouteData.Values["oid"].ToString();
      string method = HttpMethod.Post.Method;
      if (!httpMethod.Equals(method) || !str.Equals("batch"))
        return;
      if (!this.AcceptsMediaType("application/vnd.git-lfs+json"))
      {
        this.m_lfsHttpResult = this.ErrorResponse(HttpStatusCode.UnsupportedMediaType, Resources.Format("Only_IsSupported", (object) "application/vnd.git-lfs+json"));
      }
      else
      {
        try
        {
          using (Stream bufferlessInputStream = context.Request.GetBufferlessInputStream())
          {
            if (bufferlessInputStream.Length == 0L)
            {
              this.m_lfsHttpResult = this.ErrorResponse(HttpStatusCode.BadRequest, Resources.Get("ContentsCannotBeEmpty"));
            }
            else
            {
              using (StreamReader streamReader = new StreamReader(bufferlessInputStream))
              {
                string end = streamReader.ReadToEnd();
                this.m_lfsOperation = ((JObject) JsonConvert.DeserializeObject(end))["operation"].Value<string>();
                this.m_lfsObjectRefs = JsonConvert.DeserializeObject<GitLfsObjectRefBatch>(end);
                this.RequestContext.Items.Add("lfsOperation", (object) this.m_lfsOperation);
              }
            }
          }
        }
        catch (JsonException ex)
        {
          this.RequestContext.TraceException(1013857, GitServerUtils.TraceArea, nameof (GitLfsObjectsHandler), (Exception) ex);
          this.m_lfsHttpResult = this.ErrorResponse(HttpStatusCode.BadRequest, Resources.Get("LfsUnableToParseReference"));
        }
      }
    }

    protected override string Layer => nameof (GitLfsObjectsHandler);

    protected override GitLfsHandler.LfsHttpResult GetResult()
    {
      this.HandlerHttpContext.OverrideRequestTimeoutSeconds();
      GitLfsHandler.LfsHttpResult result = this.ErrorResponse(HttpStatusCode.BadRequest);
      string httpMethod = this.HandlerHttpContext.Request.HttpMethod;
      if (httpMethod.Equals(HttpMethod.Post.Method))
      {
        if (this.HandlerHttpContext.Request.RequestContext.RouteData.Values["oid"].ToString() == "batch")
        {
          this.EnterMethod("Batch");
          result = this.GetReferences();
        }
        else
        {
          this.EnterMethod("Verify");
          result = this.InitializeUpload();
        }
      }
      else if (httpMethod.Equals(HttpMethod.Put.Method))
      {
        this.EnterMethod(new MethodInformation("Upload", MethodType.Normal, EstimatedMethodCost.Low, TimeSpan.FromMinutes(60.0)));
        result = this.UploadBlob();
      }
      else if (httpMethod.Equals(HttpMethod.Get.Method))
      {
        this.EnterMethod(new MethodInformation("Download", MethodType.Normal, EstimatedMethodCost.Low, TimeSpan.FromMinutes(60.0)));
        result = this.DownloadBlobOrReference();
      }
      return result;
    }

    private GitLfsHandler.LfsHttpResult InitializeUpload()
    {
      if (!this.AcceptsMediaType("application/vnd.git-lfs+json"))
        return this.ErrorResponse(HttpStatusCode.UnsupportedMediaType, Resources.Format("Only_IsSupported", (object) "application/vnd.git-lfs+json"));
      GitLfsObjectRef gitLfsObjectRef;
      try
      {
        if (this.HandlerHttpContext.Request.ContentLength > 200)
          return this.ErrorResponse(HttpStatusCode.BadRequest, Resources.Get("RequestBodyTooLarge"));
        using (Stream bufferlessInputStream = this.HandlerHttpContext.Request.GetBufferlessInputStream())
        {
          if (bufferlessInputStream.Length > 200L)
            return this.ErrorResponse(HttpStatusCode.BadRequest, Resources.Get("RequestBodyTooLarge"));
          if (bufferlessInputStream.Length == 0L)
            return this.ErrorResponse(HttpStatusCode.BadRequest, Resources.Get("ContentsCannotBeEmpty"));
          using (StreamReader streamReader = new StreamReader(bufferlessInputStream))
            gitLfsObjectRef = JsonConvert.DeserializeObject<GitLfsObjectRef>(streamReader.ReadToEnd());
        }
      }
      catch (JsonException ex)
      {
        this.RequestContext.TraceException(1013858, GitServerUtils.TraceArea, nameof (GitLfsObjectsHandler), (Exception) ex);
        return this.ErrorResponse(HttpStatusCode.BadRequest, Resources.Get("LfsUnableToParseReference"));
      }
      if (!Sha256Id.TryParse(gitLfsObjectRef.OidStr, out Sha256Id _))
        return this.ErrorResponse(HttpStatusCode.BadRequest, Resources.Get("LfsInvalidObjectId"));
      if (gitLfsObjectRef.Size < 0L)
        return this.ErrorResponse(HttpStatusCode.BadRequest, Resources.Get("LfsSizeMustBeGreaterThanMinusOne"));
      using (ITfsGitRepository repository = this.FindRepository())
      {
        bool flag = false;
        GitLfsObjectRef lfsObjectRef = this.RequestContext.GetService<ITeamFoundationGitLfsService>().GetLfsObjectReference(this.RequestContext, repository.Key, gitLfsObjectRef.Oid);
        if (lfsObjectRef != (GitLfsObjectRef) null)
          flag = true;
        else
          lfsObjectRef = new GitLfsObjectRef(gitLfsObjectRef.Oid, gitLfsObjectRef.Size);
        HttpStatusCode statusCode = flag ? HttpStatusCode.OK : HttpStatusCode.Accepted;
        return this.LfsReferenceResult(lfsObjectRef, GitLfsObjectsHandler.ActionType.Upload, statusCode, new Uri(repository.GetRepositoryCloneUri()));
      }
    }

    private GitLfsHandler.LfsHttpResult UploadBlob()
    {
      if (!this.AcceptsMediaType("application/vnd.git-lfs"))
        return this.ErrorResponse(HttpStatusCode.UnsupportedMediaType, Resources.Format("Only_IsSupported", (object) "application/vnd.git-lfs"));
      Sha256Id result;
      if (!Sha256Id.TryParse(this.HandlerHttpContext.Request.RequestContext.RouteData.Values["oid"].ToString(), out result))
        return this.ErrorResponse(HttpStatusCode.BadRequest, Resources.Get("LfsInvalidObjectId"));
      Stream stream = (Stream) null;
      try
      {
        using (ITfsGitRepository repository = this.FindRepository())
        {
          repository.Permissions.CheckWrite(true);
          stream = this.HandlerHttpContext.Request.GetBufferlessInputStream(true);
          switch (this.RequestContext.GetService<ITeamFoundationGitLfsService>().UploadLfsObject(this.RequestContext, repository.Key, result, stream))
          {
            case TfsGitLfsObjectCreateResult.AlreadyExists:
              return new GitLfsHandler.LfsHttpResult(HttpStatusCode.OK);
            case TfsGitLfsObjectCreateResult.CreatedSuccessfully:
              return new GitLfsHandler.LfsHttpResult(HttpStatusCode.Created);
            case TfsGitLfsObjectCreateResult.ContentsDoNotMatchLfsObjectId:
              return this.ErrorResponse(HttpStatusCode.Conflict);
            case TfsGitLfsObjectCreateResult.MissingWritePermission:
              return this.ErrorResponse(HttpStatusCode.Forbidden);
            case TfsGitLfsObjectCreateResult.ContentsCannotBeEmpty:
              return this.ErrorResponse(HttpStatusCode.BadRequest, Resources.Get("ContentsCannotBeEmpty"));
            default:
              return this.ErrorResponse(HttpStatusCode.NotImplemented);
          }
        }
      }
      finally
      {
        if (this.RequestContext.IsFeatureEnabled("Git.Lfs.M203.DrainConnectionOnUpload") && stream != null)
        {
          byte[] buf = Array.Empty<byte>();
          GitStreamUtil.EnsureDrained(stream, buf);
          stream.Dispose();
        }
      }
    }

    private GitLfsHandler.LfsHttpResult DownloadBlobOrReference()
    {
      Sha256Id result;
      if (!Sha256Id.TryParse(this.HandlerHttpContext.Request.RequestContext.RouteData.Values["oid"].ToString(), out result))
        return this.ErrorResponse(HttpStatusCode.BadRequest, Resources.Get("LfsInvalidObjectId"));
      using (ITfsGitRepository repository = this.FindRepository())
      {
        ITeamFoundationGitLfsService service = this.RequestContext.GetService<ITeamFoundationGitLfsService>();
        if (this.AcceptsMediaType("application/vnd.git-lfs+json"))
        {
          GitLfsObjectRef lfsObjectReference = service.GetLfsObjectReference(this.RequestContext, repository.Key, result);
          return lfsObjectReference == (GitLfsObjectRef) null ? this.ErrorResponse(HttpStatusCode.NotFound, Resources.Get("LfsObjectNotFound")) : this.LfsReferenceResult(lfsObjectReference, GitLfsObjectsHandler.ActionType.Download, HttpStatusCode.OK, new Uri(repository.GetRepositoryCloneUri()));
        }
        if (!this.AcceptsMediaType("application/vnd.git-lfs"))
          return this.ErrorResponse(HttpStatusCode.UnsupportedMediaType, Resources.Format("Only_And_AreSupported", (object) "application/vnd.git-lfs", (object) "application/vnd.git-lfs+json"));
        Stream lfsObject = service.GetLfsObject(this.RequestContext, repository.Key, result);
        if (lfsObject == null || lfsObject.Length == 0L)
          return this.ErrorResponse(HttpStatusCode.NotFound, Resources.Get("LfsObjectNotFound"));
        this.RequestContext.UpdateTimeToFirstPage();
        return this.FileResult("git-lfs." + result?.ToString(), lfsObject);
      }
    }

    private GitLfsHandler.LfsHttpResult GetReferences()
    {
      if (this.m_lfsHttpResult != null)
        return this.m_lfsHttpResult;
      using (ITfsGitRepository repository = this.FindRepository())
      {
        ITeamFoundationGitLfsService service = this.RequestContext.GetService<ITeamFoundationGitLfsService>();
        Dictionary<string, GitLfsObjectRef> objectResults = new Dictionary<string, GitLfsObjectRef>();
        foreach (GitLfsObjectRef gitLfsObjectRef in (IEnumerable<GitLfsObjectRef>) this.m_lfsObjectRefs.Objects)
        {
          if (!Sha256Id.TryParse(gitLfsObjectRef.OidStr, out Sha256Id _))
            objectResults.Add(gitLfsObjectRef.OidStr, new GitLfsObjectRef(gitLfsObjectRef.OidStr, gitLfsObjectRef.Size)
            {
              Error = new GitLfsObjectError(422, Resources.Get("LfsInvalidObjectId"))
            });
          else if (gitLfsObjectRef.Size < 0L)
            objectResults.Add(gitLfsObjectRef.OidStr, new GitLfsObjectRef(gitLfsObjectRef.OidStr, gitLfsObjectRef.Size)
            {
              Error = new GitLfsObjectError(422, Resources.Get("LfsSizeMustBeGreaterThanMinusOne"))
            });
        }
        Dictionary<Sha256Id, GitLfsObjectRef> dictionary = new Dictionary<Sha256Id, GitLfsObjectRef>();
        IEnumerable<GitLfsObjectRef> objectReferences = service.GetLfsObjectReferences(this.RequestContext, repository.Key, this.m_lfsObjectRefs.Objects.Where<GitLfsObjectRef>((Func<GitLfsObjectRef, bool>) (o => !objectResults.ContainsKey(o.OidStr))).Select<GitLfsObjectRef, Sha256Id>((Func<GitLfsObjectRef, Sha256Id>) (o => o.Oid)));
        if (objectReferences != null)
          dictionary = objectReferences.ToDictionary<GitLfsObjectRef, Sha256Id, GitLfsObjectRef>((Func<GitLfsObjectRef, Sha256Id>) (i => i.Oid), (Func<GitLfsObjectRef, GitLfsObjectRef>) (i => i));
        Uri repositoryUri = new Uri(repository.GetRepositoryCloneUri());
        foreach (GitLfsObjectRef gitLfsObjectRef1 in (IEnumerable<GitLfsObjectRef>) this.m_lfsObjectRefs.Objects)
        {
          if (!objectResults.ContainsKey(gitLfsObjectRef1.OidStr))
          {
            GitLfsObjectRef gitLfsObjectRef2;
            if (dictionary.ContainsKey(gitLfsObjectRef1.Oid))
            {
              gitLfsObjectRef2 = dictionary[gitLfsObjectRef1.Oid];
              if (string.Equals(this.m_lfsOperation, "Download", StringComparison.OrdinalIgnoreCase))
                gitLfsObjectRef2.Actions = (IDictionary<string, GitLfsObjectAction>) this.ConstructActions(gitLfsObjectRef2.Oid, GitLfsObjectsHandler.ActionType.Download, repositoryUri);
            }
            else
            {
              gitLfsObjectRef2 = new GitLfsObjectRef(gitLfsObjectRef1.Oid, gitLfsObjectRef1.Size);
              if (string.Equals(this.m_lfsOperation, "Upload", StringComparison.OrdinalIgnoreCase))
                gitLfsObjectRef2.Actions = (IDictionary<string, GitLfsObjectAction>) this.ConstructActions(gitLfsObjectRef2.Oid, GitLfsObjectsHandler.ActionType.Upload, repositoryUri);
              else
                gitLfsObjectRef2.Error = new GitLfsObjectError(404, Resources.Get("LfsObjectNotFound"));
            }
            objectResults.Add(gitLfsObjectRef2.OidStr, gitLfsObjectRef2);
          }
        }
        return this.LfsReferenceBatchResult(new GitLfsObjectRefBatch()
        {
          Objects = (ICollection<GitLfsObjectRef>) objectResults.Values
        });
      }
    }

    private GitLfsHandler.LfsHttpResult FileResult(string fileName, Stream stream) => new GitLfsHandler.LfsHttpResult(HttpStatusCode.OK)
    {
      ContentType = new MediaTypeHeaderValue("application/vnd.git-lfs")
      {
        Parameters = {
          new NameValueHeaderValue("header", fileName)
        }
      }.ToString(),
      ProcessBody = (Action) (() =>
      {
        this.HandlerHttpContext.Response.Headers["Content-Disposition"] = "attachment; filename=" + fileName;
        this.HandlerHttpContext.Response.BufferOutput = false;
        using (stream)
          GitStreamUtil.SmartCopyTo(stream, this.HandlerHttpContext.Response.OutputStream, true);
      })
    };

    private GitLfsHandler.LfsHttpResult LfsReferenceResult(
      GitLfsObjectRef lfsObjectRef,
      GitLfsObjectsHandler.ActionType linkType,
      HttpStatusCode statusCode,
      Uri repositoryUri)
    {
      return new GitLfsHandler.LfsHttpResult(statusCode)
      {
        ContentType = "application/vnd.git-lfs+json",
        ProcessBody = (Action) (() =>
        {
          lfsObjectRef.Actions = (IDictionary<string, GitLfsObjectAction>) this.ConstructActions(lfsObjectRef.Oid, linkType, repositoryUri);
          this.HandlerHttpContext.Response.Write(JsonConvert.SerializeObject((object) lfsObjectRef));
        })
      };
    }

    private GitLfsHandler.LfsHttpResult LfsReferenceBatchResult(GitLfsObjectRefBatch batch) => new GitLfsHandler.LfsHttpResult(HttpStatusCode.OK)
    {
      ContentType = "application/vnd.git-lfs+json",
      ProcessBody = (Action) (() => this.HandlerHttpContext.Response.Write(JsonConvert.SerializeObject((object) batch)))
    };

    private Dictionary<string, GitLfsObjectAction> ConstructActions(
      Sha256Id oid,
      GitLfsObjectsHandler.ActionType actionType,
      Uri repositoryUri)
    {
      Dictionary<string, GitLfsObjectAction> dictionary = new Dictionary<string, GitLfsObjectAction>();
      GitLfsObjectAction gitLfsObjectAction = new GitLfsObjectAction(VssHttpUriUtility.ConcatUri(repositoryUri, "/info/lfs/objects/" + oid?.ToString()).AbsoluteUri)
      {
        Header = (IDictionary<string, string>) new Dictionary<string, string>()
        {
          {
            "Accept",
            "application/vnd.git-lfs"
          }
        }
      };
      if (actionType == GitLfsObjectsHandler.ActionType.Upload)
        gitLfsObjectAction.Header.Add("Transfer-Encoding", "chunked");
      dictionary.Add(actionType.ToString().ToLowerInvariant(), gitLfsObjectAction);
      return dictionary;
    }

    private enum ActionType : byte
    {
      Upload,
      Download,
    }
  }
}
