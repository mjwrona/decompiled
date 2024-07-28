// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.PlugIns.CommentSocialEngagementProvider
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.TeamFoundation.Comments.Server.Exceptions;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Social.WebApi;
using Microsoft.VisualStudio.Services.SocialEngagement.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Comments.Server.PlugIns
{
  public class CommentSocialEngagementProvider : ISocialSdkSocialEngagementProvider
  {
    public IDictionary<SocialEngagementType, string> GetSupportedSocialEngagement(
      IVssRequestContext requestContext)
    {
      return (IDictionary<SocialEngagementType, string>) new Dictionary<SocialEngagementType, string>()
      {
        {
          SocialEngagementType.Likes,
          "PlatformComment"
        },
        {
          SocialEngagementType.Dislikes,
          "PlatformComment"
        },
        {
          SocialEngagementType.Hearts,
          "PlatformComment"
        },
        {
          SocialEngagementType.Hooray,
          "PlatformComment"
        },
        {
          SocialEngagementType.Smile,
          "PlatformComment"
        },
        {
          SocialEngagementType.Confused,
          "PlatformComment"
        }
      };
    }

    public AggregationType EnableMetricsAggregation() => AggregationType.Hourly;

    public int HoursToRetainAggregation() => 720;

    public void ValidateArtifactId(
      IVssRequestContext requestContext,
      string artifactId,
      ArtifactScope artifactScope,
      out ISecuredObject securedObject)
    {
      securedObject = (ISecuredObject) null;
      if (!artifactScope.Type.Equals("Project", StringComparison.InvariantCultureIgnoreCase))
        throw new InvalidArtifactScopeException();
      string[] source = artifactId.Split(':');
      if (source == null || ((IEnumerable<string>) source).Count<string>() != 3)
        throw new InvalidArtifactIdException(artifactId);
      Guid result1;
      if (!Guid.TryParse(artifactScope.Id, out result1))
        throw new InvalidArtifactScopeException(Resources.InvalidProjectId);
      string artifactId1 = source[1];
      int result2;
      Guid artifactKind;
      if (!Guid.TryParse(source[0], out artifactKind) || !int.TryParse(source[2], out result2))
        throw new InvalidArtifactIdException(artifactId1);
      IDisposableReadOnlyList<ICommentProvider> extensions = requestContext.GetExtensions<ICommentProvider>(ExtensionLifetime.Service);
      ICommentProvider commentProvider = extensions != null ? extensions.FirstOrDefault<ICommentProvider>((Func<ICommentProvider, bool>) (p => p.ArtifactKind == artifactKind)) : (ICommentProvider) null;
      if (commentProvider == null)
        throw new CommentProviderNotRegisteredException(artifactKind);
      Comment comment = requestContext.GetService<ICommentService>().GetComment(requestContext, result1, artifactKind, artifactId1, result2);
      commentProvider.CheckReactPermission(requestContext, result1, comment, out securedObject);
    }
  }
}
