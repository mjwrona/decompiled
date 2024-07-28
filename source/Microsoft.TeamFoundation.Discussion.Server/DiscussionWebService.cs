// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Discussion.Server.DiscussionWebService
// Assembly: Microsoft.TeamFoundation.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4DCA91C2-88ED-4792-BE4A-3104961AE8D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Discussion.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Core.WebServices;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Discussion.Server
{
  [WebService(Name = "DiscussionWebService", Namespace = "http://schemas.microsoft.com/TeamFoundation/2012/Discussion")]
  [ClientService(ComponentName = "Discussion", RegistrationName = "Discussion", ServiceName = "DiscussionWebService", CollectionServiceIdentifier = "88829EB6-D205-4B0E-B66E-2B178748C6B8")]
  public sealed class DiscussionWebService : TeamFoundationWebService
  {
    public DiscussionWebService()
    {
      this.RequestContext.ServiceName = "Discussion";
      this.DiscussionService = this.RequestContext.GetService<TeamFoundationDiscussionService>();
    }

    private TeamFoundationDiscussionService DiscussionService { get; set; }

    [WebMethod]
    [ClientServiceMethod(AsyncPattern = true, SyncPattern = false)]
    public List<int> PublishDiscussions(
      [ClientType(typeof (IEnumerable<LegacyDiscussionThread>)), XmlArray("discussions")] LegacyDiscussionThread[] legacyDiscussions,
      [ClientType(typeof (IEnumerable<LegacyComment>)), XmlArray("comments")] LegacyComment[] legacyComments,
      [ClientType(typeof (IEnumerable<CommentId>))] CommentId[] deletedComments,
      [XmlArray, XmlArrayItem(typeof (short))] out List<short> commentIds,
      out DateTime lastUpdatedDate)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (PublishDiscussions), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<LegacyDiscussionThread>("discussions", (IList<LegacyDiscussionThread>) legacyDiscussions);
        methodInformation.AddArrayParameter<LegacyComment>("comments", (IList<LegacyComment>) legacyComments);
        methodInformation.AddArrayParameter<CommentId>(nameof (deletedComments), (IList<CommentId>) deletedComments);
        this.EnterMethod(methodInformation);
        DiscussionThread[] discussions = (DiscussionThread[]) null;
        DiscussionComment[] comments = (DiscussionComment[]) null;
        if (legacyDiscussions != null)
          discussions = ((IEnumerable<LegacyDiscussionThread>) legacyDiscussions).Select<LegacyDiscussionThread, DiscussionThread>((Func<LegacyDiscussionThread, DiscussionThread>) (x => x.ToDiscussionThread())).ToArray<DiscussionThread>();
        if (legacyComments != null)
          comments = ((IEnumerable<LegacyComment>) legacyComments).Select<LegacyComment, DiscussionComment>((Func<LegacyComment, DiscussionComment>) (x => x.ToDiscussionComment())).ToArray<DiscussionComment>();
        return this.DiscussionService.PublishDiscussions(this.RequestContext, discussions, comments, deletedComments, out commentIds, out lastUpdatedDate);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(690000, TraceArea.Discussion, TraceLayer.Service, ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    private static List<LegacyComment> ConvertToLegacyComments(
      List<DiscussionComment> discussionComments)
    {
      Dictionary<int, short> rootCommentIds = new Dictionary<int, short>();
      List<LegacyComment> legacyComments = new List<LegacyComment>(discussionComments.Count);
      discussionComments.ForEach((Action<DiscussionComment>) (comment =>
      {
        LegacyComment legacyComment = comment.ToLegacyComment();
        if (legacyComment.ParentCommentId == (short) 0)
        {
          short num;
          if (rootCommentIds.TryGetValue(legacyComment.DiscussionId, out num))
            legacyComment.ParentCommentId = num;
          else
            rootCommentIds.Add(legacyComment.DiscussionId, legacyComment.CommentId);
        }
        legacyComments.Add(legacyComment);
      }));
      return legacyComments;
    }

    [WebMethod]
    [ClientServiceMethod(AsyncPattern = true, SyncPattern = false)]
    public List<LegacyDiscussionThread> QueryDiscussionsByVersion(
      string versionUri,
      [XmlArray("comments"), XmlArrayItem(typeof (LegacyComment))] out List<LegacyComment> legacyComments,
      [XmlArray, XmlArrayItem(typeof (TeamFoundationIdentity))] out TeamFoundationIdentity[] authors)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryDiscussionsByVersion), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (versionUri), (object) versionUri);
        this.EnterMethod(methodInformation);
        List<DiscussionComment> comments;
        IdentityRef[] authors1;
        List<LegacyDiscussionThread> list = this.DiscussionService.QueryDiscussionsByArtifactUri(this.RequestContext, versionUri, out comments, out authors1).Select<DiscussionThread, LegacyDiscussionThread>((Func<DiscussionThread, LegacyDiscussionThread>) (x => x.ToLegacyDiscussionThread())).ToList<LegacyDiscussionThread>();
        authors = this.ToTeamFoundationIdentities(authors1);
        legacyComments = DiscussionWebService.ConvertToLegacyComments(comments);
        return list;
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(690001, TraceArea.Discussion, TraceLayer.Service, ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [ClientServiceMethod(AsyncPattern = true, SyncPattern = false)]
    public List<LegacyDiscussionThread> QueryDiscussionsByCodeReviewRequest(
      int workItemId,
      [XmlArray("comments"), XmlArrayItem(typeof (LegacyComment))] out List<LegacyComment> legacyComments,
      [XmlArray, XmlArrayItem(typeof (TeamFoundationIdentity))] out TeamFoundationIdentity[] authors)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryDiscussionsByCodeReviewRequest), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (workItemId), (object) workItemId);
        this.EnterMethod(methodInformation);
        List<DiscussionComment> comments;
        IdentityRef[] authors1;
        List<DiscussionThread> source = this.DiscussionService.QueryDiscussionsByCodeReviewRequest(this.RequestContext, workItemId, out comments, out authors1);
        authors = this.ToTeamFoundationIdentities(authors1);
        legacyComments = DiscussionWebService.ConvertToLegacyComments(comments);
        ILookup<int, LegacyComment> commentMap = legacyComments.ToLookup<LegacyComment, int>((Func<LegacyComment, int>) (x => x.DiscussionId));
        Func<DiscussionThread, bool> predicate = (Func<DiscussionThread, bool>) (x => commentMap.Contains(x.DiscussionId));
        return source.Where<DiscussionThread>(predicate).Select<DiscussionThread, LegacyDiscussionThread>((Func<DiscussionThread, LegacyDiscussionThread>) (x => x.ToLegacyDiscussionThread())).ToList<LegacyDiscussionThread>();
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(690002, TraceArea.Discussion, TraceLayer.Service, ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    private TeamFoundationIdentity[] ToTeamFoundationIdentities(IdentityRef[] identityRefs) => this.RequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.RequestContext, ((IEnumerable<IdentityRef>) identityRefs).Select<IdentityRef, Guid>((Func<IdentityRef, Guid>) (identity => new Guid(identity.Id))).ToArray<Guid>());
  }
}
