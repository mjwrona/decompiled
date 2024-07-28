// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitLinkedContentUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Git.Server.GitHfs;
using Microsoft.TeamFoundation.Git.Server.Lfs;
using Microsoft.TeamFoundation.Git.Server.LinkedContent;
using Microsoft.TeamFoundation.Git.Server.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class GitLinkedContentUtility
  {
    public static IEnumerable<ITfsLinkedContentResolver> GetTfsLinkedContentResolvers(
      IVssRequestContext requestContext,
      bool resolveLfs)
    {
      List<ITfsLinkedContentResolver> contentResolvers = new List<ITfsLinkedContentResolver>();
      if (resolveLfs)
        contentResolvers.Add((ITfsLinkedContentResolver) new TfsLfsResolver(requestContext));
      if (requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) "/Service/GitRest/Settings/GitHfsFileViewEnabled", false))
        contentResolvers.Add((ITfsLinkedContentResolver) new GitHfsProvider(requestContext));
      return (IEnumerable<ITfsLinkedContentResolver>) contentResolvers;
    }

    public static Stream Resolve(
      ITfsGitRepository repo,
      Stream contentStream,
      IEnumerable<ITfsLinkedContentResolver> resolvers)
    {
      if (resolvers == null || !resolvers.Any<ITfsLinkedContentResolver>())
        return contentStream;
      if (!(contentStream is IRewindableStream rewindableStream1))
        rewindableStream1 = (IRewindableStream) new RewindableStream(contentStream);
      IRewindableStream rewindableStream = rewindableStream1;
      ITfsLinkedContentResolver[] array = resolvers.Where<ITfsLinkedContentResolver>((Func<ITfsLinkedContentResolver, bool>) (x => x != null && x.CanResolve(rewindableStream))).ToArray<ITfsLinkedContentResolver>();
      rewindableStream.Restart();
      return array.Length != 1 ? (Stream) rewindableStream : ((IEnumerable<ITfsLinkedContentResolver>) array).Single<ITfsLinkedContentResolver>().Resolve(repo.Key, (Stream) rewindableStream);
    }
  }
}
