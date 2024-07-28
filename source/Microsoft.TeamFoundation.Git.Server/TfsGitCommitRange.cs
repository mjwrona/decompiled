// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitCommitRange
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TfsGitCommitRange
  {
    public TfsGitCommitRange(
      IEnumerable<Sha1Id> reachableFromSet,
      IEnumerable<Sha1Id> notReachableFromSet)
    {
      this.ReachableFromSet = reachableFromSet;
      this.NotReachableFromSet = notReachableFromSet;
    }

    public IEnumerable<Sha1Id> ReachableFromSet { get; }

    public IEnumerable<Sha1Id> NotReachableFromSet { get; }
  }
}
