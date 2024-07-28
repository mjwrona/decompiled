// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ProjectMigrationDetails
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.TestManagement.Client;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class ProjectMigrationDetails
  {
    internal UpgradeMigrationState MigrationState { get; set; }

    internal string MigrationError { get; set; }

    internal int TotalPlansCount { get; set; }

    internal int MigratedPlansCount { get; set; }
  }
}
