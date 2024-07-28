// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestsBaseController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Directories.DirectoryService;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GitPullRequestsBaseController : GitApiController
  {
    private static readonly IReadOnlyDictionary<string, object> CommonEntityDescriptorProperties = (IReadOnlyDictionary<string, object>) new Dictionary<string, object>()
    {
      {
        "InvitationMethod",
        (object) "ProcessReviewersList"
      },
      {
        "RootWithActiveMembership",
        (object) true
      }
    };

    protected List<TfsGitPullRequest.ReviewerBase> ProcessReviewersList(
      IdentityRefWithVote[] reviewers,
      ITfsGitRepository repo)
    {
      if (reviewers == null || reviewers.Length == 0)
        return new List<TfsGitPullRequest.ReviewerBase>();
      List<IdentityRefWithVote> source1 = (List<IdentityRefWithVote>) null;
      List<TfsGitPullRequest.ReviewerBase> source2 = new List<TfsGitPullRequest.ReviewerBase>(reviewers.Length);
      foreach (IdentityRefWithVote reviewer1 in reviewers)
      {
        if (reviewer1 != null)
        {
          if (reviewer1 != null && reviewer1.Vote != (short) 0)
            throw new InvalidArgumentValueException(Resources.Get("CannotCreateWithVotes"));
          if (reviewer1.IsAadIdentity)
          {
            if (source1 == null)
              source1 = new List<IdentityRefWithVote>();
            source1.Add(reviewer1);
          }
          else
          {
            Guid reviewer2 = GitPullRequestsBaseController.ThrowIfIdentityInvalid(reviewer1.Id);
            source2.Add(new TfsGitPullRequest.ReviewerBase(reviewer2, reviewer1.IsRequired));
          }
        }
      }
      IVssSecurityNamespace securityNamespace;
      string securityToken = GitPermissionsUtility.CreateSecurityToken(this.TfsRequestContext, repo, out securityNamespace);
      ITeamFoundationIdentityService service1 = this.TfsRequestContext.GetService<ITeamFoundationIdentityService>();
      if (source2.Any<TfsGitPullRequest.ReviewerBase>())
        GitPermissionsUtility.VerifyPermissionsForIdentities(this.TfsRequestContext, (IEnumerable<TeamFoundationIdentity>) service1.ReadIdentities(this.TfsRequestContext, source2.Select<TfsGitPullRequest.ReviewerBase, Guid>((Func<TfsGitPullRequest.ReviewerBase, Guid>) (reviewer => reviewer.Reviewer)).ToArray<Guid>()), securityNamespace, securityToken, false, false);
      if (source1 != null)
      {
        foreach (IGrouping<bool, IdentityRefWithVote> source3 in source1.GroupBy<IdentityRefWithVote, bool>((Func<IdentityRefWithVote, bool>) (id => id.IsContainer)))
        {
          if (source3.Key)
          {
            GraphHttpClient client = this.TfsRequestContext.GetClient<GraphHttpClient>();
            foreach (IdentityRefWithVote identityRefWithVote in (IEnumerable<IdentityRefWithVote>) source3)
            {
              try
              {
                IdentityDescriptor descriptorFromSid = Microsoft.TeamFoundation.Framework.Server.IdentityHelper.CreateDescriptorFromSid(SidIdentityHelper.ConstructAadGroupSid(new Guid(identityRefWithVote.Id)));
                TeamFoundationIdentity foundationIdentity = service1.ReadIdentity(this.TfsRequestContext, descriptorFromSid, MembershipQuery.None, ReadIdentityOptions.None);
                if (foundationIdentity != null && foundationIdentity.GetProperty<bool>("IsGroupDeleted", false))
                  foundationIdentity = (TeamFoundationIdentity) null;
                if (foundationIdentity == null)
                {
                  try
                  {
                    GraphGroupOriginIdCreationContext creationContext = new GraphGroupOriginIdCreationContext()
                    {
                      OriginId = identityRefWithVote.Id
                    };
                    client.CreateGroupAsync((GraphGroupCreationContext) creationContext).SyncResult<GraphGroup>();
                    foundationIdentity = service1.ReadIdentity(this.TfsRequestContext, descriptorFromSid, MembershipQuery.None, ReadIdentityOptions.None);
                  }
                  catch (Exception ex)
                  {
                    this.TfsRequestContext.TraceException(700344, Microsoft.TeamFoundation.VersionControl.Server.TraceArea.General, TraceLayer.Command, ex);
                    throw;
                  }
                }
                source2.Add(new TfsGitPullRequest.ReviewerBase(foundationIdentity.TeamFoundationId, identityRefWithVote.IsRequired));
              }
              catch (Exception ex)
              {
                throw new GitPullRequestInvalidReviewer(identityRefWithVote.DisplayName);
              }
            }
          }
          else
          {
            IDirectoryService service2 = this.TfsRequestContext.GetService<IDirectoryService>();
            IVssRequestContext requestContext = this.TfsRequestContext.To(TeamFoundationHostType.ProjectCollection).Elevate();
            IReadOnlyList<DirectoryEntityDescriptor> members = (IReadOnlyList<DirectoryEntityDescriptor>) source3.Select<IdentityRefWithVote, DirectoryEntityDescriptor>((Func<IdentityRefWithVote, DirectoryEntityDescriptor>) (id => new DirectoryEntityDescriptor("User", "aad", id.Id, displayName: id.DisplayName, properties: GitPullRequestsBaseController.CommonEntityDescriptorProperties))).ToList<DirectoryEntityDescriptor>().AsReadOnly();
            IVssRequestContext tfsRequestContext = this.TfsRequestContext;
            IReadOnlyList<IdentityDirectoryEntityResult<TeamFoundationIdentity>> directoryEntityResultList = service2.IncludeTeamFoundationIdentities(tfsRequestContext).AddMembers(requestContext, (IReadOnlyList<IDirectoryEntityDescriptor>) members, license: "Optimal");
            for (int index = 0; index < directoryEntityResultList.Count; ++index)
            {
              if (directoryEntityResultList[index].Status != "Success" || !directoryEntityResultList[index].Identity.IsActive)
                throw new GitPullRequestInvalidReviewer(source1[index].DisplayName);
              GitPermissionsUtility.VerifyPermissionsForIdentities(this.TfsRequestContext, (IEnumerable<TeamFoundationIdentity>) new TeamFoundationIdentity[1]
              {
                directoryEntityResultList[index].Identity
              }, securityNamespace, securityToken, false, false);
              source2.Add(new TfsGitPullRequest.ReviewerBase(directoryEntityResultList[index].Identity.TeamFoundationId, source1[index].IsRequired));
            }
          }
        }
      }
      return source2;
    }

    protected static Guid ThrowIfIdentityInvalid(string reviewerId)
    {
      Guid result;
      if (string.IsNullOrEmpty(reviewerId) || !Guid.TryParse(reviewerId, out result))
        throw new InvalidArgumentValueException(Resources.Format("InvalidIdentity", (object) (reviewerId == null ? Resources.Get("NoGuidSupplied") : reviewerId)));
      return result;
    }
  }
}
