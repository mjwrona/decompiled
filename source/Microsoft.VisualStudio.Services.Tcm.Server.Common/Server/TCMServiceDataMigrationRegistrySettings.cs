// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TCMServiceDataMigrationRegistrySettings
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public static class TCMServiceDataMigrationRegistrySettings
  {
    public const string TCMServiceDataMigrationWaterMark = "/Service/TestManagement/TCMServiceDataMigration/WaterMark";
    public const string TCMServiceDataMigrationBatchSize = "/Service/TestManagement/TCMServiceDataMigration/BatchSize";
    public const string TCMServiceDataMigrationStepBatchSize = "/Service/TestManagement/TCMServiceDataMigration/StepBatchSize/{0}";
    public const string TCMServiceDataMigrationStatus = "/Service/TestManagement/TCMServiceDataMigration/MigrationStatus";
    public const string TCMServiceDataMigrationAllowNonS2SCreds = "/Service/TestManagement/TCMServiceDataMigration/AllowNonS2SCreds";
    public const string TCMServiceDataMigrationStopAfterCurrentStep = "/Service/TestManagement/TCMServiceDataMigration/StopAfterCurrentStep";
    public const string TCMServiceDataMigrationPointResultsWaterMark = "/Service/TestManagement/TCMServiceDataMigration/PointResultsWaterMark";
    public const string TCMServiceDataMigrationPointResultsBatchSize = "/Service/TestManagement/TCMServiceDataMigration/PointResultsBatchSize";
    public const string TCMServiceDataMigrationPointResultsMigrationStatus = "/Service/TestManagement/TCMServiceDataMigration/PointResultsMigrationStatus";
    public const string TCMServiceDataMigrationPointOutcomeWaterMark = "/Service/TestManagement/TCMServiceDataMigration/PointOutcomeWaterMark";
    public const string TCMServiceDataMigrationPointOutcomeBatchSize = "/Service/TestManagement/TCMServiceDataMigration/PointOutcomeBatchSize";
    public const string TCMServiceDataMigrationPointOutcomeMigrationStatus = "/Service/TestManagement/TCMServiceDataMigration/PointOutcomeMigrationStatus";
    public const string TCMServiceDataMigrationBackfillOutcomeForPointHistoryStatus = "/Service/TestManagement/TCMServiceDataMigration/BackfillOutcomeForPointHistoryStatus";
    public const string TCMServiceDataMigrationBackfillOutcomeForPointHistoryWatermark = "/Service/TestManagement/TCMServiceDataMigration/BackfillOutcomeForPointHistoryWatermark";
    public const string TCMServiceDataMigrationBackfillOutcomeForPointHistoryBatchSize = "/Service/TestManagement/TCMServiceDataMigration/BackfillOutcomeForPointHistoryBatchSize";
  }
}
