// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Native.AsyncOperationParameters
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using LibGit2Sharp;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Native
{
  internal class AsyncOperationParameters
  {
    public AsyncOperationParameters(
      IVssRequestContext requestContext,
      Sha1Id ontoRefHead,
      Signature committer,
      IReadOnlyList<Sha1Id> commitIds)
    {
      this.RequestContext = requestContext;
      this.OntoRefHead = ontoRefHead;
      this.Committer = committer;
      this.CommitIds = commitIds;
    }

    public IVssRequestContext RequestContext { get; }

    public IReadOnlyList<Sha1Id> CommitIds { get; }

    public Signature Committer { get; }

    public Sha1Id OntoRefHead { get; }
  }
}
