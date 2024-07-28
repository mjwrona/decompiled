// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Migration.MigrationResourceIds
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Migration
{
  [GenerateAllConstants(null)]
  public static class MigrationResourceIds
  {
    public const string AreaName = "Migration";
    public static readonly Guid SourceMigrationsLocationId = new Guid("D0C82C7A-85DC-4B7C-BC68-7D94A8DEA1BF");
    public static readonly Guid LiveHostRollbackBlobLocationId = new Guid("63DADD8E-2ADA-487F-97C2-04DF66DE9AB7");
    public static readonly Guid TargetMigrationsLocationId = new Guid("DACA1573-44A3-4A81-8D43-7ACB4E86F2AD");
    public static readonly Guid FinalizationCheckLocationId = new Guid("78306584-FE7C-4A61-95A5-8413D70320F7");
    public static readonly Guid SasTokenRequestLocationId = new Guid("D6978331-6B7A-4C2A-905F-7797D31A92A0");
    public static readonly Guid DeploymentInformationLocationId = new Guid("DDBECDB7-24DE-49FB-862D-5C6DF20DDF2E");
    public static readonly Guid MigrationJobsLocationId = new Guid("EFF73468-CE4D-4ACA-BB85-7E0137375DD2");
    public static readonly Guid JobMovementLocationId = new Guid("35C5DAF7-3463-40EF-B769-5374D57E06C6");
    public const string SourceHostMigrationResourceName = "SourceHostMigration";
    public const string LiveHostRollbackBlobResourceName = "LiveHostRollbackBlob";
    public const string TargetHostMigrationResourceName = "TargetHostMigration";
    public const string FinalizationCheckResourceName = "FinalizationCheck";
    public const string SasTokenRequestResourceName = "SasTokenRequest";
    public const string DeploymentInformationResourceName = "DeploymentInformation";
    public const string MigrationJobsResourceName = "MigrationJobs";
    public const string JobMovementResourceName = "JobMovement";
  }
}
