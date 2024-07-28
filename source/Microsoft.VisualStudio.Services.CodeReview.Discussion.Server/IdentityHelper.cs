// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.IdentityHelper
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  public static class IdentityHelper
  {
    internal static Dictionary<string, Guid> NamesToIdentities(
      IVssRequestContext requestContext,
      IEnumerable<string> names)
    {
      requestContext.TraceEnter(600200, TraceArea.Discussion, TraceLayer.Service, nameof (NamesToIdentities));
      Dictionary<string, Guid> identities = new Dictionary<string, Guid>();
      foreach (string name in names)
      {
        if (!string.IsNullOrWhiteSpace(name) && !identities.ContainsKey(name))
        {
          Guid identity = IdentityHelper.NameToIdentity(requestContext, name);
          identities.Add(name, identity);
          requestContext.Trace(600201, TraceLevel.Info, TraceArea.Discussion, TraceLayer.Service, "Found identity for name '{0}'", (object) name);
        }
      }
      requestContext.TraceLeave(600209, TraceArea.Discussion, TraceLayer.Service, nameof (NamesToIdentities));
      return identities;
    }

    internal static string IdentityToUniqueName(IVssRequestContext requestContext, Guid id)
    {
      requestContext.TraceEnter(600210, TraceArea.Discussion, TraceLayer.Service, nameof (IdentityToUniqueName));
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        id
      }, QueryMembership.None, (IEnumerable<string>) null);
      if (identityList.Count != 1 || identityList[0] == null)
      {
        requestContext.Trace(600211, TraceLevel.Error, TraceArea.Discussion, TraceLayer.Service, "Identity cannot be found for id '{0}'", (object) id);
        throw new IdentityNotFoundException(id);
      }
      string uniqueName = Microsoft.TeamFoundation.Framework.Server.IdentityHelper.GetUniqueName(identityList[0]);
      requestContext.TraceLeave(600219, TraceArea.Discussion, TraceLayer.Service, nameof (IdentityToUniqueName));
      return uniqueName;
    }

    internal static Guid NameToIdentity(IVssRequestContext requestContext, string name)
    {
      requestContext.TraceEnter(600220, TraceArea.Discussion, TraceLayer.Service, nameof (NameToIdentity));
      Microsoft.VisualStudio.Services.Identity.Identity identity = Microsoft.TeamFoundation.Framework.Server.IdentityHelper.FindIdentity(requestContext, name, true, true);
      requestContext.TraceLeave(600229, TraceArea.Discussion, TraceLayer.Service, nameof (NameToIdentity));
      return identity.Id;
    }

    internal static void PopulateAuthorDisplayNames(
      IVssRequestContext requestContext,
      IEnumerable<DiscussionComment> comments)
    {
      HashSet<Guid> source = new HashSet<Guid>();
      foreach (DiscussionComment comment in comments)
      {
        source.Add(comment.GetAuthorId());
        if (comment.UsersLiked != null)
        {
          foreach (IdentityRef identityRef in comment.UsersLiked)
            source.Add(new Guid(identityRef.Id));
        }
      }
      Dictionary<Guid, IdentityRef> dictionary = IdentityHelper.PopulateDisplayNames(requestContext, (IList<Guid>) source.ToList<Guid>());
      foreach (DiscussionComment comment in comments)
      {
        Guid authorId = comment.GetAuthorId();
        if (dictionary[authorId] != null)
        {
          comment.Author = dictionary[authorId];
        }
        else
        {
          comment.Author = new IdentityRef()
          {
            DisplayName = "<" + Resources.CommentAuthorNotFound + ">",
            Id = authorId.ToString()
          };
          requestContext.Trace(600321, TraceLevel.Error, TraceArea.Discussion, TraceLayer.Service, new StringBuilder().AppendLine(string.Format("Unable to resolve authorId '{0}'", (object) authorId)).AppendLine(string.Format("CommentId: {0}", (object) comment.CommentId)).AppendLine(string.Format("DiscussionId: {0}", (object) comment.DiscussionId)).AppendLine(string.Format("ParentCommentId: {0}", (object) comment.ParentCommentId)).AppendLine(string.Format("ResolvedIdentitiesCount: {0}", (object) dictionary.Count)).ToString());
        }
        if (comment.UsersLiked != null)
        {
          List<IdentityRef> identityRefList = new List<IdentityRef>();
          foreach (IdentityRef identityRef in comment.UsersLiked)
            identityRefList.Add(dictionary[new Guid(identityRef.Id)] ?? identityRef);
          comment.UsersLiked = identityRefList;
        }
      }
    }

    internal static Dictionary<Guid, IdentityRef> PopulateDisplayNames(
      IVssRequestContext requestContext,
      IList<Guid> users)
    {
      Dictionary<Guid, IdentityRef> dictionary = new Dictionary<Guid, IdentityRef>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, users, QueryMembership.None, (IEnumerable<string>) null);
      for (int index = 0; index < identityList.Count; ++index)
      {
        if (identityList[index] == null)
          requestContext.Trace(600320, TraceLevel.Error, TraceArea.Discussion, TraceLayer.Service, "ReadIdentities returned null for '{0}'", (object) users[index]);
        dictionary.Add(users[index], identityList[index].ToCRIdentityRef(requestContext));
      }
      return dictionary;
    }

    public static IdentityRef ToCRIdentityRef(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext requestContext)
    {
      if (identity == null)
        return (IdentityRef) null;
      IdentityRef identityRef = identity.ToIdentityRef(requestContext);
      identityRef.UniqueName = identity.GetProperty<string>("Mail", (string) null);
      return identityRef;
    }

    public static IList<Guid> GetInvalidIdentities(
      IVssRequestContext requestContext,
      IList<Guid> userIdentities)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> mappedIdentities = IdentityHelper.GetMappedIdentities(requestContext, userIdentities);
      IList<Guid> invalidIdentities = (IList<Guid>) new List<Guid>();
      for (int index = 0; index < mappedIdentities.Count; ++index)
      {
        if (mappedIdentities[index] == null)
          invalidIdentities.Add(userIdentities[index]);
      }
      return invalidIdentities;
    }

    public static IList<Microsoft.VisualStudio.Services.Identity.Identity> GetMappedIdentities(
      IVssRequestContext requestContext,
      IList<Guid> uniqueUserIdentities)
    {
      IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identitiesLookup = IdentityHelper.GenerateIdentitiesLookup(requestContext, uniqueUserIdentities);
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) uniqueUserIdentities.Select<Guid, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) (id => identitiesLookup[id])).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    private static IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> GenerateIdentitiesLookup(
      IVssRequestContext requestContext,
      IList<Guid> uniqueUserIdentities)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, uniqueUserIdentities, QueryMembership.None, (IEnumerable<string>) null);
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identitiesLookup = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList)
      {
        if (identity != null)
          identitiesLookup[identity.Id] = identity;
      }
      if (uniqueUserIdentities.Count == identityList.Count)
      {
        for (int index = 0; index < uniqueUserIdentities.Count; ++index)
        {
          Guid uniqueUserIdentity = uniqueUserIdentities[index];
          if (!identitiesLookup.ContainsKey(uniqueUserIdentity))
            identitiesLookup[uniqueUserIdentity] = identityList[index];
        }
      }
      return (IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) identitiesLookup;
    }
  }
}
