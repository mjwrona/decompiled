// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RepoMigrationRequest
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class RepoMigrationRequest
  {
    public bool MigrateCode { get; set; }

    public bool MigrateMetadata { get; set; }

    public bool MigrateSecurity { get; set; }

    public string[] BranchesToMigrate { get; set; }

    public string[] ParentRefNames { get; set; }

    public Guid SourceRepoId { get; set; }

    public Guid TargetRepoId { get; set; }

    public bool SkipValidationsForTestingOnly { get; set; }

    public string MigrationBranchPrefix { get; set; }
  }
}
