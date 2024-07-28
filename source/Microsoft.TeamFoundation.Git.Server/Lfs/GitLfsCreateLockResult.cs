// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Lfs.GitLfsCreateLockResult
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server.Lfs
{
  public struct GitLfsCreateLockResult
  {
    public readonly LfsLock Lock;
    public readonly GitLfsCreateLockResultStatus Status;

    public GitLfsCreateLockResult(GitLfsCreateLockResultStatus status)
      : this((LfsLock) null, status)
    {
    }

    public GitLfsCreateLockResult(LfsLock lfsLock, GitLfsCreateLockResultStatus status)
    {
      this.Lock = lfsLock;
      this.Status = status;
    }
  }
}
