// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.DataImportPackageConstants
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public static class DataImportPackageConstants
  {
    public const string SourceLocation = "Location";
    [Obsolete("No longer used in Data Import pipeline")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string SourceIdentityMapping = "IdentityMapping";
    public const string SourceDacpac = "Dacpac";
    public const string SourceConnectionString = "ConnectionString";
    public const string AccountOwner = "Owner";
    public const string AccountRegion = "Region";
    public const string AccountScaleUnit = "ScaleUnit";
    public const string SkipValidation = "SkipValidation";
    public const string SkipWITImport = "SkipWITImport";
    public const string SkipFileImport = "SkipFileImport";
    public const string SourceDacpacLocation = "SourceDacpacLocation";
    public const string HostToMovePostImport = "HostToMovePostImport";
    public const string NeighborHostName = "NeighborHostName";
    public const string TargetDatabaseDowngradeSize = "TargetDatabaseDowngradeSize";
    public const string TryMapADGroupsToAAD = "TryMapADGroupsToAAD";
    public const string ImportType = "ImportType";
    public const string TfsMigratorImportVersion = "TfsMigratorImportVersion";
    public const string ImportPackage = "ImportPackage";
    public const string DatabaseTotalSize = "DatabaseTotalSize";
    public const string DatabaseBlobSize = "DatabaseBlobSize";
    public const string DatabaseTableSize = "DatabaseTableSize";
    public const string ActiveUserCount = "ActiveUserCount";
    public const string TfsVersion = "TfsVersion";
    public const string TfsMigratorVersion = "TfsMigratorVersion";
    public const string Collation = "Collation";
    public const string CommandExecutionTime = "CommandExecutionTime";
    public const string CommandExecutionCount = "CommandExecutionCount";
    public const string Force = "Force";
    public const string ServicesToInclude = "ServicesToInclude";
    public const string ServicesToExclude = "ServicesToExclude";
    public const string ValidationChecksum = "ValidationChecksum";
    public const string ValidationChecksumVersion = "ValidationChecksumVersion";
    [Obsolete("Import Code are no longer supported")]
    public const string ImportCode = "ImportCode";
  }
}
