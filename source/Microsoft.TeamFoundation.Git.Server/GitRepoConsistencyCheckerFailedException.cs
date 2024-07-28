// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRepoConsistencyCheckerFailedException
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitRepoConsistencyCheckerFailedException : TeamFoundationServiceException
  {
    public GitRepoConsistencyCheckerFailedException(RepoKey repoKey, string reasonPhrase)
      : base(FormattableString.Invariant(FormattableStringFactory.Create("The repository {0} is in an inconsistent state: {1}", (object) repoKey, (object) reasonPhrase)))
    {
    }

    public GitRepoConsistencyCheckerFailedException(RepoKey repoKey, Exception inner)
      : base(FormattableString.Invariant(FormattableStringFactory.Create("The repository {0} is in an inconsistent state.", (object) repoKey)), inner)
    {
    }
  }
}
