// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Protocol.PackAndRefIngester
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Streams;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.Protocol
{
  public class PackAndRefIngester
  {
    private readonly ReceivePackParser m_parser;

    internal PackAndRefIngester(
      IVssRequestContext rc,
      ITfsGitRepository repo,
      Stream packStream,
      ClientTraceData ctData)
    {
      this.m_parser = new ReceivePackParser(rc, repo, packStream, (Stream) null, ctData, DefaultGitDependencyRoot.Instance, true);
    }

    internal static PackAndRefIngester CreateWithWriteableStream(
      IVssRequestContext rc,
      ITfsGitRepository repo,
      out Stream tempPackStream)
    {
      tempPackStream = (Stream) new ReceivePackStream(GitServerUtils.GetCacheDirectory(rc, repo.Key.RepoId));
      return new PackAndRefIngester(rc, repo, tempPackStream, (ClientTraceData) null);
    }

    public void AddRefUpdateRequest(string refName, Sha1Id oldObjectId, Sha1Id newObjectId) => this.m_parser.AddRefUpdateRequest(refName, oldObjectId, newObjectId);

    public TfsGitRefUpdateResultSet Ingest(
      IProgress<ReceivePackStep> stepObserver = null,
      bool applyContentPolicies = true)
    {
      return this.m_parser.ProcessPackAndRefUpdates(true, stepObserver, applyContentPolicies);
    }
  }
}
