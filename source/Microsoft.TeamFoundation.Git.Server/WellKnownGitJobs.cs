// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.WellKnownGitJobs
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class WellKnownGitJobs
  {
    private const string c_pluginsNamespace = "Microsoft.TeamFoundation.Git.Server.Plugins.";
    private const string c_pluginsJobsNamespace = "Microsoft.TeamFoundation.Git.Server.Plugins.Jobs.";

    public static class GitAdvSecBillableCommittersBackfillJob
    {
      public const string Name = "GitAdvSecBillableCommittersBackfillJob";
      public const string ExtensionName = "Microsoft.TeamFoundation.Git.Server.Plugins.Jobs.GitAdvSecBillableCommittersBackfillJob";
      public const string Id = "d93e72b8-37b6-4b95-bc1d-c7619933fd52";
    }

    public static class GitAdvSecCommitScanningJob
    {
      public const string Name = "GitAdvSecCommitScanningJob";
      public const string ExtensionName = "Microsoft.TeamFoundation.Git.Server.Plugins.GitAdvSecCommitScanningJob";
    }

    public static class GitAdvSecInitializePermissionsJob
    {
      public const string Name = "GitAdvSecInitializePermissionsJob";
      public const string ExtensionName = "Microsoft.TeamFoundation.Git.Server.Plugins.Jobs.GitAdvSecInitializePermissionsJob";
    }

    public static class GitAdvSecInvalidatePermissionCacheJob
    {
      public const string Name = "GitAdvSecInvalidatePermissionCacheJob";
      public const string ExtensionName = "Microsoft.TeamFoundation.Git.Server.Plugins.Jobs.GitAdvSecInvalidatePermissionCacheJob";
    }

    public static class GitBitmapComputationJob
    {
      public const string Name = "GitBitmapComputationJob";
      public const string ExtensionName = "Microsoft.TeamFoundation.Git.Server.Plugins.Jobs.GitBitmapComputationJob";
    }

    public static class GitCommitMetadataCatchupJob
    {
      public const string Name = "GitCommitMetadataCatchupJob";
      public const string ExtensionName = "Microsoft.TeamFoundation.Git.Server.Plugins.GitCommitMetadataCatchupJob";
    }

    public static class GitGraphSerializationJob
    {
      public const string Name = "GitGraphSerializationJob";
      public const string ExtensionName = "Microsoft.TeamFoundation.Git.Server.Plugins.GitGraphSerializationJob";
    }

    public static class GitRepackerJob
    {
      public const string Name = "GitRepackerJob";
      public const string ExtensionName = "Microsoft.TeamFoundation.Git.Server.Plugins.GitRepackerJob";
    }
  }
}
