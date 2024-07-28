// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GvfsServerConstants
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class GvfsServerConstants
  {
    public static readonly string PackResponseContentType = "application/x-git-packfile";
    public static readonly string LooseObjectResponseContentType = "application/x-git-loose-object";
    public static readonly string GroupedLooseObjectResponseContentType = "application/x-gvfs-loose-objects";
    public static readonly string PrefetchPackFilesAndIndexesContentType = "application/x-gvfs-timestamped-packfiles-indexes";
  }
}
