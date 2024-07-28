// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TeamFoundationGitCommitStatusService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class TeamFoundationGitCommitStatusService : 
    ITeamFoundationGitCommitStatusService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public GitStatus AddStatus(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id commitId,
      GitStatus gitCommitStatusToCreate)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext), "git");
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repository, nameof (repository), "git");
      TeamFoundationGitCommitStatusService.ValidateCommitStatus(requestContext, gitCommitStatusToCreate);
      repository.Permissions.CheckWrite();
      IdentityRef identityRef = requestContext.GetService<ITeamFoundationIdentityService>().ReadRequestIdentity(requestContext).ToIdentityRef(requestContext);
      gitCommitStatusToCreate.CreatedBy = identityRef;
      repository.LookupObject<TfsGitCommit>(commitId);
      ClientTraceData properties = new ClientTraceData();
      properties.Add("LightWeightStatusCommitId", (object) commitId);
      properties.Add("LightWeightStatusGenre", (object) gitCommitStatusToCreate.Context.Genre);
      properties.Add("LightWeightStatusName", (object) gitCommitStatusToCreate.Context.Name);
      properties.Add("LightWeightStatusState", (object) gitCommitStatusToCreate.State);
      properties.Add("LightWeightStatusCreatedBy", (object) gitCommitStatusToCreate.CreatedBy?.Id);
      requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "LightWeightCommitStatus", properties);
      using (GitCommitStatusComponent component = requestContext.CreateComponent<GitCommitStatusComponent>())
      {
        GitStatus gitStatus = component.AddStatus(repository.Key, commitId, gitCommitStatusToCreate);
        gitStatus.CreatedBy = identityRef;
        return gitStatus;
      }
    }

    public ILookup<Sha1Id, GitStatus> GetStatuses(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      IEnumerable<Sha1Id> commits,
      int top,
      int skip,
      bool latestOnly = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repository, nameof (repository));
      ArgumentUtility.CheckForNull<IEnumerable<Sha1Id>>(commits, nameof (commits), "git");
      ArgumentUtility.CheckBoundsInclusive(top, 1, 1000, nameof (top), "git");
      if (!commits.Any<Sha1Id>())
        return Enumerable.Empty<GitStatus>().ToLookup<GitStatus, Sha1Id>((Func<GitStatus, Sha1Id>) (x => new Sha1Id()));
      skip = Math.Max(skip, 0);
      ILookup<Sha1Id, GitStatus> toReturn;
      using (GitCommitStatusComponent component = requestContext.CreateComponent<GitCommitStatusComponent>())
        toReturn = component.QueryStatuses(repository.Key, commits, top, skip);
      if (((toReturn == null ? 0 : (toReturn.Count > 0 ? 1 : 0)) & (latestOnly ? 1 : 0)) != 0)
      {
        List<KeyValuePair<Sha1Id, GitStatus>> source1 = new List<KeyValuePair<Sha1Id, GitStatus>>();
        foreach (IGrouping<Sha1Id, GitStatus> grouping1 in (IEnumerable<IGrouping<Sha1Id, GitStatus>>) toReturn)
        {
          IGrouping<Sha1Id, GitStatus> grouping = grouping1;
          IEnumerable<KeyValuePair<Sha1Id, GitStatus>> source2 = grouping.GroupBy<GitStatus, string>((Func<GitStatus, string>) (x => x.Context.Genre + x.Context.Name)).Select<IGrouping<string, GitStatus>, GitStatus>((Func<IGrouping<string, GitStatus>, GitStatus>) (x => x.OrderByDescending<GitStatus, int>((Func<GitStatus, int>) (y => y.Id)).First<GitStatus>())).Select<GitStatus, KeyValuePair<Sha1Id, GitStatus>>((Func<GitStatus, KeyValuePair<Sha1Id, GitStatus>>) (x => new KeyValuePair<Sha1Id, GitStatus>(grouping.Key, x)));
          Dictionary<string, GitStatus> latestStatusByUrl = source2.Where<KeyValuePair<Sha1Id, GitStatus>>((Func<KeyValuePair<Sha1Id, GitStatus>, bool>) (pair => !string.IsNullOrWhiteSpace(pair.Value.TargetUrl))).GroupBy<KeyValuePair<Sha1Id, GitStatus>, string>((Func<KeyValuePair<Sha1Id, GitStatus>, string>) (pair => pair.Value.TargetUrl)).ToDictionary<IGrouping<string, KeyValuePair<Sha1Id, GitStatus>>, string, GitStatus>((Func<IGrouping<string, KeyValuePair<Sha1Id, GitStatus>>, string>) (urlPair => urlPair.Key), (Func<IGrouping<string, KeyValuePair<Sha1Id, GitStatus>>, GitStatus>) (urlPair => urlPair.OrderByDescending<KeyValuePair<Sha1Id, GitStatus>, int>((Func<KeyValuePair<Sha1Id, GitStatus>, int>) (pair => pair.Value.Id)).First<KeyValuePair<Sha1Id, GitStatus>>().Value));
          source1.AddRange(source2.Where<KeyValuePair<Sha1Id, GitStatus>>((Func<KeyValuePair<Sha1Id, GitStatus>, bool>) (pair => !latestStatusByUrl.ContainsKey(pair.Value.TargetUrl ?? "") || latestStatusByUrl[pair.Value.TargetUrl].Id == pair.Value.Id)));
        }
        toReturn = source1.ToLookup<KeyValuePair<Sha1Id, GitStatus>, Sha1Id, GitStatus>((Func<KeyValuePair<Sha1Id, GitStatus>, Sha1Id>) (kvp => kvp.Key), (Func<KeyValuePair<Sha1Id, GitStatus>, GitStatus>) (kvp => kvp.Value));
      }
      Guid[] array = toReturn.SelectMany<IGrouping<Sha1Id, GitStatus>, GitStatus>((Func<IGrouping<Sha1Id, GitStatus>, IEnumerable<GitStatus>>) (x => toReturn[x.Key])).Select<GitStatus, Guid>((Func<GitStatus, Guid>) (status => new Guid(status.CreatedBy.Id))).Distinct<Guid>().ToArray<Guid>();
      Dictionary<string, IdentityRef> dictionary = ((IEnumerable<TeamFoundationIdentity>) requestContext.GetService<ITeamFoundationIdentityService>().ReadIdentities(requestContext, array)).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (x => x != null)).Select<TeamFoundationIdentity, IdentityRef>((Func<TeamFoundationIdentity, IdentityRef>) (x => x.ToIdentityRef(requestContext))).GroupBy<IdentityRef, string>((Func<IdentityRef, string>) (x => x.Id)).Select<IGrouping<string, IdentityRef>, IdentityRef>((Func<IGrouping<string, IdentityRef>, IdentityRef>) (x => x.First<IdentityRef>())).ToDictionary<IdentityRef, string>((Func<IdentityRef, string>) (y => y.Id), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (GitStatus gitStatus in toReturn.SelectMany<IGrouping<Sha1Id, GitStatus>, GitStatus>((Func<IGrouping<Sha1Id, GitStatus>, IEnumerable<GitStatus>>) (x => toReturn[x.Key])))
      {
        IdentityRef identityRef;
        if (dictionary.TryGetValue(gitStatus.CreatedBy.Id, out identityRef))
          gitStatus.CreatedBy = identityRef;
      }
      return toReturn;
    }

    private static void ValidateCommitStatus(
      IVssRequestContext requestContext,
      GitStatus gitCommitStatusToCreate)
    {
      ArgumentUtility.CheckForNull<GitStatus>(gitCommitStatusToCreate, nameof (gitCommitStatusToCreate));
      ArgumentUtility.CheckForNull<GitStatusContext>(gitCommitStatusToCreate.Context, "gitCommitStatusToCreate.Context");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(gitCommitStatusToCreate.Context.Name, "gitCommitStatusToCreate.Context.Name");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(gitCommitStatusToCreate.Context.Genre, "gitCommitStatusToCreate.Context.Genre");
      if (gitCommitStatusToCreate.Context.Name.Length > 128)
        throw new ArgumentException(Resources.Format("GitCommitStatusArgLengthException", (object) "gitCommitStatusToCreate.Context.Name", (object) 128), "gitCommitStatusToCreate.Context.Name");
      if (gitCommitStatusToCreate.Context.Genre.Length > 128)
        throw new ArgumentException(Resources.Format("GitCommitStatusArgLengthException", (object) "gitCommitStatusToCreate.Context.Genre", (object) 128), "gitCommitStatusToCreate.Context.Genre");
      string description = gitCommitStatusToCreate.Description;
      if ((description != null ? (description.Length > 2048 ? 1 : 0) : 0) != 0)
        throw new ArgumentException(Resources.Format("GitCommitStatusArgLengthException", (object) "gitCommitStatusToCreate.Description", (object) 2048), "gitCommitStatusToCreate.Description");
      if (gitCommitStatusToCreate.TargetUrl != null)
      {
        if (gitCommitStatusToCreate.TargetUrl.Length > 2048)
          throw new ArgumentException(Resources.Format("GitCommitStatusArgLengthException", (object) "gitCommitStatusToCreate.TargetUrl", (object) 2048), "gitCommitStatusToCreate.TargetUrl");
        if (!Uri.TryCreate(gitCommitStatusToCreate.TargetUrl, UriKind.Absolute, out Uri _))
          throw new ArgumentException(Resources.Format("GitCommitStatusArgTargetUrlInvalidException", (object) "gitCommitStatusToCreate.TargetUrl"), "gitCommitStatusToCreate.TargetUrl");
      }
      if (gitCommitStatusToCreate.State == GitStatusState.NotSet)
        throw new ArgumentException(Resources.Format("GitCommitStatusInvalidStateException", (object) "gitCommitStatusToCreate.State"));
    }
  }
}
