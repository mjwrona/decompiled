// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ReachableObjectResolver.IReachableObjectResolver
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.Filter;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.ReachableObjectResolver
{
  internal interface IReachableObjectResolver
  {
    ISet<Sha1Id> Resolve(
      ITfsGitRepository repository,
      ISet<Sha1Id> haves,
      ISet<Sha1Id> wants,
      ICollection<Sha1Id> shallows,
      GitObjectFilter filter,
      bool wantsPreExpanded,
      out ISet<Sha1Id> foundHaves,
      IObserver<int> statusObserver);
  }
}
