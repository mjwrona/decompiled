// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Lfs.TfsLfsResolver
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.LinkedContent;
using Microsoft.TeamFoundation.Git.Server.Streams;
using Microsoft.VisualStudio.Services.Common;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.Lfs
{
  public class TfsLfsResolver : TfsLinkedContentResolverBase
  {
    private readonly ITeamFoundationGitLfsService m_lfsService;

    public TfsLfsResolver(
      IVssRequestContext requestContext,
      ITeamFoundationGitLfsService lfsService = null)
      : base(requestContext, LfsPointerFile.VersionMarker)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.m_lfsService = lfsService ?? this.RequestContext.GetService<ITeamFoundationGitLfsService>();
    }

    public override Stream Resolve(RepoKey repoKey, Stream contentStream)
    {
      ArgumentUtility.CheckForNull<RepoKey>(repoKey, nameof (repoKey));
      ArgumentUtility.CheckForNull<Stream>(contentStream, nameof (contentStream));
      if (!(contentStream is IRewindableStream rewindableStream1))
        rewindableStream1 = (IRewindableStream) new RewindableStream(contentStream);
      IRewindableStream rewindableStream2 = rewindableStream1;
      LfsPointerFile lfsPointerFile = LfsPointerFile.TryParse((Stream) rewindableStream2);
      if (lfsPointerFile != null && lfsPointerFile.ObjectId != (Sha256Id) null)
      {
        Stream lfsObject = this.m_lfsService.GetLfsObject(this.RequestContext, repoKey, lfsPointerFile.ObjectId);
        if (lfsObject != null)
        {
          ((Stream) rewindableStream2).Close();
          return lfsObject;
        }
      }
      rewindableStream2.Restart();
      return (Stream) rewindableStream2;
    }
  }
}
