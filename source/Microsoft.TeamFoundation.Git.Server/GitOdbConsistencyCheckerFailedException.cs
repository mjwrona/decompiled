// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitOdbConsistencyCheckerFailedException
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitOdbConsistencyCheckerFailedException : TeamFoundationServiceException
  {
    public GitOdbConsistencyCheckerFailedException(OdbId odbId, string reasonPhrase)
      : base(FormattableString.Invariant(FormattableStringFactory.Create("The odb {0} is in an inconsistent state: {1}", (object) odbId, (object) reasonPhrase)))
    {
    }

    public GitOdbConsistencyCheckerFailedException(OdbId odbId, Exception inner)
      : base(FormattableString.Invariant(FormattableStringFactory.Create("The odb {0} is in an inconsistent state.", (object) odbId)), inner)
    {
    }
  }
}
