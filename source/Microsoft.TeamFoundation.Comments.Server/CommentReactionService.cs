// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.CommentReactionService
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.TeamFoundation.Comments.Server.Events;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.Social.WebApi;
using Microsoft.VisualStudio.Services.SocialServer.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Comments.Server
{
  public class CommentReactionService : ICommentReactionService, IVssFrameworkService
  {
    private static Dictionary<SocialEngagementType, string> ReactionTypeToIconMap = new Dictionary<SocialEngagementType, string>()
    {
      {
        SocialEngagementType.Likes,
        "&#x1F44D;"
      },
      {
        SocialEngagementType.Dislikes,
        "&#x1F44E;"
      },
      {
        SocialEngagementType.Hooray,
        "&#x1F389;"
      },
      {
        SocialEngagementType.Hearts,
        "&#x2764;"
      },
      {
        SocialEngagementType.Smile,
        "&#x1F603;"
      },
      {
        SocialEngagementType.Confused,
        "&#x1F616;"
      }
    };

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public CommentReaction CreateCommentReaction(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      int commentId,
      SocialEngagementType reactionType)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckStringForNullOrEmpty(artifactId, nameof (artifactId));
      requestContext.TraceEnter(140301, "CommentService", "Service", nameof (CreateCommentReaction));
      try
      {
        ISocialEngService service = requestContext.GetService<ISocialEngService>();
        SocialEngagementRecord engagementRecord = service.CreateSocialEngagementRecord(requestContext, new SocialEngagementCreateParameter()
        {
          ArtifactId = this.GetArtifactId(artifactKind, artifactId, commentId),
          ArtifactType = "PlatformComment",
          EngagementType = reactionType,
          ArtifactScope = new ArtifactScope("Project", projectId.ToString())
        });
        requestContext.Trace(140302, TraceLevel.Info, "CommentService", "Service", string.Format("Reaction added info - {0}: {1}, {2}: {3}, {4}: {5},", (object) nameof (projectId), (object) projectId, (object) nameof (artifactKind), (object) artifactKind, (object) nameof (artifactId), (object) artifactId) + string.Format(" {0}: {1}, {2}: {3}", (object) nameof (commentId), (object) commentId, (object) nameof (reactionType), (object) reactionType));
        this.FireCommentReactionChangedEvent(requestContext, service, artifactKind, projectId, artifactId, commentId, reactionType);
        return new CommentReaction(commentId, engagementRecord);
      }
      finally
      {
        requestContext.TraceLeave(140309, "CommentService", "Service", nameof (CreateCommentReaction));
      }
    }

    public CommentReaction DeleteCommentReaction(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      int commentId,
      SocialEngagementType reactionType)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckStringForNullOrEmpty(artifactId, nameof (artifactId));
      requestContext.TraceEnter(140311, "CommentService", "Service", nameof (DeleteCommentReaction));
      try
      {
        ISocialEngService service = requestContext.GetService<ISocialEngService>();
        SocialEngagementRecord record = service.DeleteSocialEngagementRecord(requestContext, new SocialEngagementCreateParameter()
        {
          ArtifactId = this.GetArtifactId(artifactKind, artifactId, commentId),
          ArtifactType = "PlatformComment",
          EngagementType = reactionType,
          ArtifactScope = new ArtifactScope("Project", projectId.ToString())
        });
        this.FireCommentReactionChangedEvent(requestContext, service, artifactKind, projectId, artifactId, commentId, reactionType, false);
        return new CommentReaction(commentId, record);
      }
      finally
      {
        requestContext.TraceLeave(140319, "CommentService", "Service", nameof (DeleteCommentReaction));
      }
    }

    public IList<CommentReaction> GetCommentReactions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      ISet<int> commentIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckForNull<ISet<int>>(commentIds, nameof (commentIds));
      requestContext.TraceEnter(140331, "CommentService", "Service", nameof (GetCommentReactions));
      try
      {
        ISocialEngService service = requestContext.GetService<ISocialEngService>();
        IEnumerable<SocialEngagementType> socialEngagementTypes1 = Enum.GetValues(typeof (SocialEngagementType)).Cast<SocialEngagementType>();
        ISet<string> hashSet = (ISet<string>) commentIds.Select<int, string>((Func<int, string>) (c => this.GetArtifactId(artifactKind, artifactId, c))).ToHashSet();
        IVssRequestContext requestContext1 = requestContext;
        ArtifactScope artifactScope = new ArtifactScope("Project", projectId.ToString());
        Guid userId = requestContext.GetUserId();
        ISet<string> artifactIds = hashSet;
        IEnumerable<SocialEngagementType> socialEngagementTypes2 = socialEngagementTypes1;
        return (IList<CommentReaction>) CommentReactionService.GetReactionResultsFromSocialEngagementRecords(service.GetSocialEngagementRecords(requestContext1, artifactScope, userId, "PlatformComment", artifactIds, socialEngagementTypes2) ?? (IEnumerable<SocialEngagementRecord>) new List<SocialEngagementRecord>());
      }
      finally
      {
        requestContext.TraceLeave(140339, "CommentService", "Service", nameof (GetCommentReactions));
      }
    }

    public IEnumerable<IdentityRef> GetEngagedUsers(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      int commentId,
      SocialEngagementType reactionType,
      int top = 20,
      int skip = 0)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckStringForNullOrEmpty(artifactId, nameof (artifactId));
      requestContext.TraceEnter(140321, "CommentService", "Service", nameof (GetEngagedUsers));
      try
      {
        SocialEngagementCreateParameter socialEngagementCreateParameter = new SocialEngagementCreateParameter()
        {
          ArtifactId = this.GetArtifactId(artifactKind, artifactId, commentId),
          ArtifactType = "PlatformComment",
          EngagementType = reactionType,
          ArtifactScope = new ArtifactScope("Project", projectId.ToString())
        };
        return requestContext.GetService<ISocialEngService>().GetEngagedUsers(requestContext, socialEngagementCreateParameter, top, skip);
      }
      finally
      {
        requestContext.TraceLeave(140329, "CommentService", "Service", nameof (GetEngagedUsers));
      }
    }

    private static List<CommentReaction> GetReactionResultsFromSocialEngagementRecords(
      IEnumerable<SocialEngagementRecord> socialEngagementRecords)
    {
      List<CommentReaction> engagementRecords = new List<CommentReaction>();
      foreach (SocialEngagementRecord engagementRecord in socialEngagementRecords)
      {
        if (!string.IsNullOrEmpty(engagementRecord.ArtifactId))
        {
          string[] strArray = engagementRecord.ArtifactId.Split(':');
          int result;
          if (strArray.Length == 3 && int.TryParse(strArray[2], out result))
            engagementRecords.Add(new CommentReaction(result, engagementRecord));
        }
      }
      return engagementRecords;
    }

    private string GetArtifactId(Guid artifactKind, string artifactId, int commentId) => string.Format("{0}:{1}:{2}", (object) artifactKind, (object) artifactId, (object) commentId);

    private void FireCommentReactionChangedEvent(
      IVssRequestContext requestContext,
      ISocialEngService socialEngagementService,
      Guid artifactKind,
      Guid projectId,
      string artifactId,
      int commentId,
      SocialEngagementType reactionType,
      bool isReactionAdded = true)
    {
      using (requestContext.TraceBlock(140343, 140344, "CommentService", "Service", nameof (FireCommentReactionChangedEvent)))
      {
        if (!this.GetCommentProvider(requestContext, artifactKind).ShouldFireReactionChangedEvent)
          return;
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        int num = 0;
        if (isReactionAdded)
        {
          ISocialEngService socialEngService = socialEngagementService;
          IVssRequestContext requestContext1 = requestContext;
          ArtifactScope artifactScope = new ArtifactScope("Project", projectId.ToString());
          Guid id = userIdentity.Id;
          HashSet<string> artifactIds = new HashSet<string>();
          artifactIds.Add(this.GetArtifactId(artifactKind, artifactId, commentId));
          IEnumerable<SocialEngagementType> socialEngagementTypes = Enum.GetValues(typeof (SocialEngagementType)).Cast<SocialEngagementType>();
          num = socialEngService.GetSocialEngagementRecords(requestContext1, artifactScope, id, "PlatformComment", (ISet<string>) artifactIds, socialEngagementTypes).Sum<SocialEngagementRecord>((Func<SocialEngagementRecord, int>) (s => s.SocialEngagementStatistics.UserCount));
        }
        IdentityRef identityRef = new IdentityRef()
        {
          Id = userIdentity.Id.ToString(),
          DisplayName = userIdentity.DisplayName,
          Inactive = !userIdentity.IsActive,
          IsContainer = userIdentity.IsContainer
        };
        CommentReactionChangedEvent notificationEvent = new CommentReactionChangedEvent()
        {
          IsReactionAdded = isReactionAdded,
          ReactionType = reactionType.ToString(),
          ArtifactKind = artifactKind,
          ArtifactId = artifactId,
          CommentId = commentId,
          ProjectId = projectId,
          ReactionTypeIcon = CommentReactionService.ReactionTypeToIconMap[reactionType],
          TotalReactionsCount = num,
          ReactionModifiedBy = identityRef
        };
        requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) notificationEvent);
      }
    }

    private ICommentProvider GetCommentProvider(
      IVssRequestContext requestContext,
      Guid artifactKind)
    {
      IDisposableReadOnlyList<ICommentProvider> extensions = requestContext.GetExtensions<ICommentProvider>(ExtensionLifetime.Service);
      return (extensions != null ? extensions.FirstOrDefault<ICommentProvider>((Func<ICommentProvider, bool>) (p => p.ArtifactKind == artifactKind)) : (ICommentProvider) null) ?? throw new CommentProviderNotRegisteredException(artifactKind);
    }
  }
}
