// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitLfsConstants
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class GitLfsConstants
  {
    public const string LfsMediaType = "application/vnd.git-lfs";
    public const string LfsJsonMediaType = "application/vnd.git-lfs+json";
    public const string OidRouteKeyName = "oid";
    public const string LockIdRouteKeyName = "lockId";
    public const string LockActionRouteKeyName = "lockAction";
    public const string LockVerifyAction = "verify";
    public const string LockUnlockAction = "unlock";
    public const int LockPathLengthLimit = 1024;
    public const string Operation = "operation";
    public const string Batch = "batch";
    public const string LfsOperation = "lfsOperation";
    public const string LfsDownloadOperation = "download";
  }
}
