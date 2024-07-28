// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingMigrationTracePoints
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class LicensingMigrationTracePoints
  {
    public const int TraceMigrationException = 4100000;
    public const int ForwardRequestFailed = 4100001;
    public const int InvalidLicesinsingSnapshotCounts = 4100002;
    public const int InvalidLicesinsingSnapshotData = 4100003;
    public const int ReadDiffDetected = 4100004;
    public const int ForwardReadRequestFailed = 4100005;
    public const int ForwardWriteRequestFailed = 4100006;
    public const int ForwardReadWriteRequestFailed = 4100007;
    public const int TraceDeletionException = 4100009;
    public const int DeleteLicensingData = 4100010;
    public const int ForwardAttempted = 4100011;
    public const int EnterMigrateLicensingData = 4100020;
    public const int SetLicensingWriteDisabledFeatureFlagOn = 4100021;
    public const int MigrateLicensingData = 4100022;
    public const int SetAccountMigratedFeatureFlagOn = 4100023;
    public const int SetLicensingWriteDisabledFeatureFlagUndefined = 4100024;
    public const int TraceTotalMigrationTime = 4100025;
    public const int TraceMigrateLicensingDataException = 4100026;
    public const int RollbackLicensingWriteDisabledFeatureFlag = 4100027;
    public const int RollbackAccountMigratedFeatureFlag = 4100028;
    public const int RollbackComplete = 4100029;
    public const int RollbackFailed = 4100030;
    public const int LeaveMigrateLicensingData = 4100040;
  }
}
