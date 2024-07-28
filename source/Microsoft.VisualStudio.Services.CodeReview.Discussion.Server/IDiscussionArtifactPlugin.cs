// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.IDiscussionArtifactPlugin
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  [InheritedExport]
  public interface IDiscussionArtifactPlugin
  {
    bool CanResolveArtifactId(ArtifactId artifact);

    void ResolveArtifactId(ArtifactId artifact);

    object[] DecodeArtifactId(ArtifactId artifact);

    bool CanResolveVersionId(string versionId);

    string CreateUri(params object[] uriParameters);

    void CheckPermission(
      IVssRequestContext requestContext,
      DiscussionPermissions toCheck,
      List<IGrouping<string, DiscussionThread>> discussionsByArtifactId,
      Dictionary<int, bool> discussionsChecked,
      bool throwOnAccessDenied);

    string TranslateIncomingPath(IVssRequestContext requestContext, string projectNamePath);

    string TranslateOutgoingPath(IVssRequestContext requestContext, string dataspacePath);
  }
}
