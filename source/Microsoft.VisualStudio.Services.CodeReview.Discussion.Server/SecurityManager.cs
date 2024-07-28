// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.SecurityManager
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  internal static class SecurityManager
  {
    public static Dictionary<int, bool> CheckPermission(
      IVssRequestContext requestContext,
      IEnumerable<DiscussionThread> discussions,
      DiscussionPermissions toCheck,
      bool throwOnAccessDenied)
    {
      requestContext.TraceEnter(600256, TraceArea.Discussion, TraceLayer.Security, nameof (CheckPermission));
      Dictionary<int, bool> discussionsChecked = new Dictionary<int, bool>(discussions.Count<DiscussionThread>());
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DiscussionSecurity.NamespaceId);
      string token = SecurityManager.GetToken(requestContext);
      bool flag1;
      if (throwOnAccessDenied)
      {
        securityNamespace.CheckPermission(requestContext, token, (int) toCheck, false);
        flag1 = true;
      }
      else
        flag1 = securityNamespace.HasPermission(requestContext, token, (int) toCheck, false);
      if (flag1)
      {
        Dictionary<string, List<IGrouping<string, DiscussionThread>>> dictionary = new Dictionary<string, List<IGrouping<string, DiscussionThread>>>();
        List<IGrouping<string, DiscussionThread>> groupingList = new List<IGrouping<string, DiscussionThread>>();
        IEnumerable<IGrouping<string, DiscussionThread>> source = discussions.GroupBy<DiscussionThread, string>((Func<DiscussionThread, string>) (x => x.ArtifactUri), (IEqualityComparer<string>) TFStringComparer.ArtiFactUrl);
        if (source.Any<IGrouping<string, DiscussionThread>>())
        {
          ITeamFoundationDiscussionService service = requestContext.GetService<ITeamFoundationDiscussionService>();
          foreach (IGrouping<string, DiscussionThread> grouping in source)
          {
            bool flag2 = false;
            ArtifactId artifact = LinkingUtilities.DecodeUri(grouping.Key);
            foreach (IDiscussionArtifactPlugin artifactPlugin in service.ArtifactPlugins)
            {
              if (artifactPlugin.CanResolveArtifactId(artifact))
              {
                string name = artifactPlugin.GetType().Name;
                if (!dictionary.ContainsKey(name))
                  dictionary.Add(name, new List<IGrouping<string, DiscussionThread>>());
                dictionary[name].Add(grouping);
                flag2 = true;
                break;
              }
            }
            if (!flag2)
              groupingList.Add(grouping);
          }
          foreach (KeyValuePair<string, List<IGrouping<string, DiscussionThread>>> keyValuePair in dictionary)
          {
            foreach (IDiscussionArtifactPlugin artifactPlugin in service.ArtifactPlugins)
            {
              if (artifactPlugin.GetType().Name.Equals(keyValuePair.Key))
              {
                artifactPlugin.CheckPermission(requestContext, toCheck, keyValuePair.Value, discussionsChecked, throwOnAccessDenied);
                break;
              }
            }
          }
          foreach (IEnumerable<DiscussionThread> discussionThreads in groupingList)
          {
            foreach (DiscussionThread discussionThread in discussionThreads)
            {
              if (!discussionsChecked.ContainsKey(discussionThread.DiscussionId))
                discussionsChecked.Add(discussionThread.DiscussionId, true);
            }
          }
        }
      }
      requestContext.TraceLeave(600257, TraceArea.Discussion, TraceLayer.Security, nameof (CheckPermission));
      return discussionsChecked;
    }

    private static string GetToken(IVssRequestContext requestContext)
    {
      if (!(requestContext.DataspaceIdentifier != Guid.Empty))
        return DiscussionSecurity.NamespaceRootToken;
      return FormattableString.Invariant(FormattableStringFactory.Create("{0}{1}/", (object) DiscussionSecurity.NamespaceRootToken, (object) requestContext.DataspaceIdentifier));
    }
  }
}
