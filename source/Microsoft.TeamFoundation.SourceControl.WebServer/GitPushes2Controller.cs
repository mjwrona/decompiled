// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPushes2Controller
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Git.Server.Utils;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "pushes", ResourceVersion = 2)]
  public class GitPushes2Controller : GitPushesController
  {
    private static readonly RegistryQuery s_maxPostRequestSizeQuery = new RegistryQuery("/Service/GitRest/Settings/PushesControllerMaxPostBodyBytes", false);
    private static readonly RegistryQuery s_extendedMaxPostRequestSizeQuery = new RegistryQuery("/Service/GitRest/Settings/PushesControllerExtendedMaxPostBodyBytes", false);
    private static readonly RegistryQuery s_extendedMaxPostRequestSizeAppIdsQuery = new RegistryQuery("/Service/GitRest/Settings/PushesControllerExtendedMaxPostBodyBytesAppIds", false);
    private const int c_codePageUtf8 = 65001;

    [HttpPost]
    [ClientExample("POST__git_repositories__tempRepoId__pushes.json", "Initial commit (Create a new branch)", null, null)]
    [ClientExample("POST__git_repositories__tempRepoId__pushes2.json", "Add a text file", null, null)]
    [ClientExample("POST__git_repositories__tempRepoId__pushes3.json", "Add a binary file", null, null)]
    [ClientExample("POST__git_repositories__tempRepoId__pushes4.json", "Update a file", null, null)]
    [ClientExample("POST__git_repositories__tempRepoId__pushes5.json", "Delete a file", null, null)]
    [ClientExample("POST__git_repositories__tempRepoId__pushes6.json", "Rename a file", null, null)]
    [ClientExample("POST__git_repositories__tempRepoId__pushes7.json", "Move a file", null, null)]
    [ClientExample("POST__git_repositories__tempRepoId__pushes8.json", "Update a file in a new branch", null, null)]
    [ClientExample("POST__git_repositories__tempRepoId__pushes9.json", "Multiple changes", null, null)]
    [ClientResponseType(typeof (GitPush), null, null)]
    [ClientLocationId("EA98D07B-3C87-4971-8EDE-A613694FFB55")]
    [ClientRequestBodyType(typeof (GitPush), "push")]
    public virtual HttpResponseMessage CreatePush([ClientParameterType(typeof (Guid), true)] string repositoryId)
    {
      GitPush pushInternal = this.CreatePushInternal(repositoryId);
      GitStatusStateMapper.MapGitEntity<GitPush>(pushInternal, this.TfsRequestContext);
      return this.Request.CreateResponse<GitPush>(HttpStatusCode.Created, pushInternal);
    }

    protected GitPush CreatePushInternal(string repositoryId)
    {
      if (this.Request?.Content == null)
        this.TfsRequestContext.TraceAlways(1013797, TraceLevel.Error, this.TraceArea, "GitPushesController", "Invalid request: Request present? {0}. Content present? {1}", (object) (this.Request != null), (object) (this.Request?.Content != null));
      Sha1Id newObjectId;
      int pushId;
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId))
      {
        if (tfsGitRepository.IsInMaintenance)
          throw new GitRepoInMaintenanceException(MaintenanceMessageUtils.GetSanitizedMaintenanceMessage(this.TfsRequestContext));
        int maxPostBodySize = this.GetMaxPostBodySize();
        string end;
        GitPush gitPush;
        using (RestrictedStream restrictedStream = new RestrictedStream(this.Request.Content.ReadAsStreamAsync().Result, 0L, (long) (maxPostBodySize + 1), true))
        {
          try
          {
            end = new StreamReader((Stream) restrictedStream).ReadToEnd();
            gitPush = JsonConvert.DeserializeObject<GitPush>(end);
          }
          catch (JsonException ex)
          {
            this.TfsRequestContext.TraceCatch(1013250, TraceLevel.Warning, this.TraceArea, "GitPushesController", (Exception) ex);
            if (restrictedStream.Position >= (long) maxPostBodySize)
              throw new InvalidArgumentValueException("contentStream", Resources.Format("RequestMaxSizeExceeded", (object) maxPostBodySize));
            throw new InvalidArgumentValueException("contentStream", Resources.Get("InvalidJsonBody"));
          }
        }
        if (gitPush == null || gitPush.Date != new DateTime() || gitPush.PushId != 0 || gitPush.PushedBy != null || gitPush.Repository != null && gitPush.Repository.Id != tfsGitRepository.Key.RepoId)
          throw new InvalidArgumentValueException("newPush", Resources.Get("InvalidParameters"));
        if (gitPush.Commits == null || gitPush.Commits.Count<GitCommitRef>() != 1 || gitPush.Commits.First<GitCommitRef>() == null || gitPush.RefUpdates == null || gitPush.RefUpdates.Count<GitRefUpdate>() != 1 || gitPush.RefUpdates.First<GitRefUpdate>() == null)
          throw new InvalidArgumentValueException("newPush", Resources.Get("InvalidPushCommitRefCount"));
        if (this.TfsRequestContext.IsFeatureEnabled("SourceControl.EnableReturningPartiallySucceededGitStatusState") && gitPush.Commits != null && gitPush.Commits.Any<GitCommitRef>((Func<GitCommitRef, bool>) (c => c?.Statuses != null && c.Statuses.Any<GitStatus>((Func<GitStatus, bool>) (s => s != null && s.State == GitStatusState.PartiallySucceeded)))))
          throw new InvalidArgumentValueException(Resources.Get("InvalidGitStatusStateValue"), "State");
        GitRefUpdate gitRefUpdate = gitPush.RefUpdates.First<GitRefUpdate>();
        if (string.IsNullOrEmpty(gitRefUpdate.Name) || !string.IsNullOrEmpty(gitRefUpdate.NewObjectId))
          throw new InvalidArgumentValueException("refUpdate", Resources.Get("InvalidParameters"));
        GitCommitRef gitCommitRef = gitPush.Commits.First<GitCommitRef>();
        if (!string.IsNullOrEmpty(gitCommitRef.CommitId) || string.IsNullOrEmpty(gitCommitRef.Comment) || gitCommitRef.Changes == null || !gitCommitRef.Changes.Any<GitChange>())
          throw new InvalidArgumentValueException("newCommit", Resources.Get("InvalidParameters"));
        if (gitCommitRef.Changes.Any<GitChange>((Func<GitChange, bool>) (x => x == null)))
          throw new InvalidArgumentValueException("Changes", Resources.Get("InvalidParameters"));
        if (gitCommitRef.Parents != null && gitCommitRef.Parents.Count<string>() > 1)
          throw new InvalidArgumentValueException("newCommit", Resources.Get("InvalidParameters"));
        Sha1Id baseCommitId = Sha1Id.Empty;
        bool flag = false;
        if (!string.IsNullOrEmpty(gitRefUpdate.OldObjectId))
        {
          if (gitRefUpdate.OldObjectId.Length != 40)
            throw new InvalidArgumentValueException("OldObjectId", Resources.Get("InvalidParameters"));
          baseCommitId = GitCommitUtility.ParseSha1Id(gitRefUpdate.OldObjectId);
          flag = true;
        }
        if (gitCommitRef.Parents != null && gitCommitRef.Parents.Any<string>())
        {
          string sha1Id1 = gitCommitRef.Parents.FirstOrDefault<string>();
          if (!string.IsNullOrEmpty(sha1Id1))
          {
            Sha1Id sha1Id2 = sha1Id1.Length == 40 ? GitCommitUtility.ParseSha1Id(sha1Id1) : throw new InvalidArgumentValueException("parentCommitId", Resources.Get("InvalidParameters"));
            if (baseCommitId != Sha1Id.Empty && baseCommitId != sha1Id2)
              throw new InvalidArgumentValueException("baseCommitId", Resources.Get("InvalidParameters"));
            baseCommitId = sha1Id2;
            flag = true;
          }
        }
        if (!flag)
          throw new InvalidArgumentValueException("baseCommitId", Resources.Get("InvalidParameters"));
        foreach (GitChange change in gitCommitRef.Changes)
        {
          if (change?.NewContentTemplate != null && change?.NewContent != null)
            throw new InvalidArgumentValueException(Resources.Get("InvalidGitChangeContentType"));
          if (change?.NewContentTemplate != null)
          {
            if (change.ChangeType != VersionControlChangeType.Add)
              throw new InvalidArgumentValueException("ChangeType", Resources.Get("UnsupportedTemplateChangeType"));
            ArgumentUtility.CheckStringForNullOrEmpty(change.NewContentTemplate.Name, "commits.changes.newContentTemplate.name");
            ArgumentUtility.CheckStringForNullOrEmpty(change.NewContentTemplate.Type, "commits.changes.newContentTemplate.type");
            this.CreateChangeContentFromTemplate(change);
          }
        }
        TimeSpan? offset1 = new TimeSpan?();
        TimeSpan? offset2 = new TimeSpan?();
        if (gitCommitRef.Author != null && gitCommitRef.Author.Date != new DateTime())
        {
          string dateInput = this.ReadStringDateFromJson(end, "commits", "author", "date");
          gitCommitRef.Author.Date = this.ConvertUserDateToUTC(dateInput, gitCommitRef.Author.Date, out offset1);
        }
        if (gitCommitRef.Committer != null && gitCommitRef.Committer.Date != new DateTime())
        {
          string dateInput = this.ReadStringDateFromJson(end, "commits", "committer", "date");
          gitCommitRef.Committer.Date = this.ConvertUserDateToUTC(dateInput, gitCommitRef.Committer.Date, out offset2);
        }
        TfsGitRefUpdateResultSet refUpdateResultSet;
        try
        {
          refUpdateResultSet = tfsGitRepository.ModifyPaths(gitRefUpdate.Name, baseCommitId, gitCommitRef.Comment, gitCommitRef.Changes, gitCommitRef.Author, gitCommitRef.Committer, offset1, offset2);
        }
        catch (Exception ex) when (
        {
          // ISSUE: unable to correctly present filter
          int num;
          switch (ex)
          {
            case ArgumentException _:
            case GitPackDeserializerException _:
            case TreeObjectFailedToParseException _:
              num = 1;
              break;
            default:
              num = ex is FormatException ? 1 : 0;
              break;
          }
          if ((uint) num > 0U)
          {
            SuccessfulFiltering;
          }
          else
            throw;
        }
        )
        {
          throw new InvalidArgumentValueException(ex.Message, "newPush", ex);
        }
        TfsGitRefUpdateResult refUpdate = refUpdateResultSet.Results.Single<TfsGitRefUpdateResult>();
        GitPushUtil.CheckRefUpdateSuccess(this.TfsRequestContext, refUpdate);
        newObjectId = refUpdate.NewObjectId;
        pushId = refUpdateResultSet.PushId.Value;
      }
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId))
      {
        GitPush push = GitPushUtility.GetPush(this.TfsRequestContext, tfsGitRepository, this.Url, pushId, 0, true);
        TfsGitCommit tfsGitCommit = tfsGitRepository.LookupObject<TfsGitCommit>(newObjectId);
        push.Commits = (IEnumerable<GitCommitRef>) new GitCommit[1]
        {
          new GitCommitTranslator(this.TfsRequestContext, tfsGitRepository.Key, this.Url).ToGitCommit(tfsGitCommit)
        };
        return push;
      }
    }

    protected int GetMaxPostBodySize()
    {
      IVssRegistryService service = this.TfsRequestContext.GetService<IVssRegistryService>();
      int num1 = service.GetValue<int>(this.TfsRequestContext, in GitPushes2Controller.s_maxPostRequestSizeQuery, true, 26214400);
      int num2 = service.GetValue<int>(this.TfsRequestContext, in GitPushes2Controller.s_extendedMaxPostRequestSizeQuery, true, 134217728);
      return !this.ShouldApplyExtendedLimit(service) ? num1 : num2;
    }

    private bool ShouldApplyExtendedLimit(IVssRegistryService registryService) => ((IEnumerable<Guid>) ((IEnumerable<string>) registryService.GetValue(this.TfsRequestContext, in GitPushes2Controller.s_extendedMaxPostRequestSizeAppIdsQuery, true, "00000009-0000-0000-c000-000000000000").Split(new char[1]
    {
      ','
    }, StringSplitOptions.RemoveEmptyEntries)).Select<string, Guid>((Func<string, Guid>) (x => Guid.Parse(x))).ToArray<Guid>()).Contains<Guid>(this.TfsRequestContext.GetOAuthAppId());

    protected string ReadStringDateFromJson(string originalPushJson, params string[] properties)
    {
      using (StringReader reader = new StringReader(originalPushJson))
      {
        using (JsonTextReader jsonTextReader = new JsonTextReader((TextReader) reader))
        {
          int index = 0;
          while (jsonTextReader.Read())
          {
            if (jsonTextReader.Value as string == properties[index])
              ++index;
            if (index == properties.Length)
              return jsonTextReader.ReadAsString();
          }
        }
      }
      return (string) null;
    }

    protected DateTime ConvertUserDateToUTC(string dateInput, DateTime date, out TimeSpan? offset)
    {
      offset = new TimeSpan?();
      DateTime date1;
      TimeSpan utcOffset;
      if (this.TfsRequestContext.GetTimeZone().TryParseDate(dateInput, out date1, out utcOffset))
      {
        offset = new TimeSpan?(utcOffset);
        date = date1;
      }
      return date;
    }

    protected void CreateChangeContentFromTemplate(GitChange templatedChange)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(GitTemplatesProvider.GetTemplateContent(templatedChange.NewContentTemplate.Name, templatedChange.NewContentTemplate.Type));
      templatedChange.NewContent = new ItemContent()
      {
        Content = Convert.ToBase64String(bytes),
        ContentType = ItemContentType.Base64Encoded
      };
      templatedChange.NewContentTemplate = (GitTemplate) null;
    }
  }
}
