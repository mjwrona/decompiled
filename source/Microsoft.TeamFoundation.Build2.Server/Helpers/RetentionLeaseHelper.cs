// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Helpers.RetentionLeaseHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build2.Server.Helpers
{
  internal static class RetentionLeaseHelper
  {
    public const string BranchOwnerIdPrefix = "Branch";
    public const string BuildOwnerIdPrefix = "Build";
    public const string UserOwnerIdPrefix = "User";
    public const string RMOwnerIdPrefix = "RM";
    public const string PipelineOwnerIdPrefix = "Pipeline";
    public const string LegacyRetentionModelUser = "Legacy Retention Model";

    public static string GetOwnerIdForUser(Guid userId) => string.Format("{0}:{1}", (object) "User", (object) userId);

    public static string GetOwnerIdForBranch(MinimalBuildRepository repository, string branch) => "Branch:" + repository.Id + ":" + branch;

    public static string GetOwnerIdForBuild(BuildData build) => string.Format("{0}:{1}:{2}", (object) "Build", (object) build.Id, (object) build.ProjectId);

    public static string GetOwnerIdForPipeline(MinimalBuildDefinition definition) => string.Format("{0}:{1}", (object) "Pipeline", (object) definition.Id);

    public static RetentionLeaseHelper.OwnerType GetOwnerType(string ownerId)
    {
      if (ownerId.StartsWith("Branch:", StringComparison.OrdinalIgnoreCase))
        return RetentionLeaseHelper.OwnerType.Branch;
      if (ownerId.StartsWith("User:", StringComparison.OrdinalIgnoreCase))
        return RetentionLeaseHelper.OwnerType.User;
      if (ownerId.Equals("RM", StringComparison.OrdinalIgnoreCase) || ownerId.StartsWith("RM:", StringComparison.OrdinalIgnoreCase))
        return RetentionLeaseHelper.OwnerType.RM;
      if (ownerId.StartsWith("Pipeline:", StringComparison.OrdinalIgnoreCase))
        return RetentionLeaseHelper.OwnerType.Pipeline;
      return ownerId.StartsWith("Build:", StringComparison.OrdinalIgnoreCase) ? RetentionLeaseHelper.OwnerType.Build : RetentionLeaseHelper.OwnerType.Unknown;
    }

    public enum OwnerType
    {
      Branch,
      Build,
      User,
      Pipeline,
      RM,
      Unknown,
    }

    public enum RetentionLeaseChange
    {
      LeaseAdded,
      LeaseRemoved,
    }
  }
}
