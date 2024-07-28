// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataImport.DataImportValidationConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.DataImport
{
  public class DataImportValidationConstants
  {
    public const string Region = "Region";
    public const string IdentityMapLog = "IdentityMapLog";
    public const string InvokedFromCommand = "InvokedFromCommand";
    public const string ProcessMap = "ProcessMap";
    public const string Output = "Output";
    public const string Preview = "Preview";
    public const string Collection = "Collection";
    public const string ConnectionString = "ConnectionString";
    public const string Skip = "Skip";
    public const string TenantDomainName = "TenantDomainName";
    public const string IncludeInactiveUsers = "IncludeInactiveUsers";
    public const string ImportFile = "ImportFile";
    public const string ValidateOnly = "ValidateOnly";
    public const string ServiceUrl = "ServiceUrl";
    public const string NoPrompt = "NoPrompt";
    public const string SaveProcesses = "SaveProcesses";
    public const string IgnoreProjectLimit = "IgnoreProjectLimit";
    public const string ProcessMode = "ProcessMode";
    public const string IgnoreErrors = "IgnoreErrors";
    public const string KeepMeSignedIn = "KeepMeSignedIn";
    public const string SendTimeout = "SendTimeout";
    public const string Token = "Token";
    public const string ValidationDataPrefix = "ValidationData.";
    public const string SourceHostId = "ValidationData.SourceHostId";
    public const string UserLicenses = "ValidationData.UserLicenses";
    public const string LicenseTypes = "ValidationData.LicenseTypes";
    public const string ServiceEndpointScopeIds = "ValidationData.ServiceEndpointScopeIds";
    public const string PackageValidationDataPrefix = "ImportValidationData.";
    public const string DatabaseBlobSize = "ImportValidationData.DatabaseBlobSize";
    public const string DatabaseTotalSize = "ImportValidationData.DatabaseTotalSize";
    public const string DatabaseTableSize = "ImportValidationData.DatabaseTableSize";
    public const string DatabaseLargestTableSize = "ImportValidationData.DatabaseLargestTableSize";
    public const string DispositionTooLarge = "ImportValidationData.DispositionTooLarge";
    public const string DatabaseCollation = "ImportValidationData.DatabaseCollation";
    public const string ActiveUserCount = "ImportValidationData.ActiveUserCount";
    public const string TfsVersion = "ImportValidationData.TfsVersion";
    public const string TfsMigratorVersion = "ImportValidationData.TfsMigratorVersion";
    public const string CommandExecutionCount = "ImportValidationData.CommandExecutionCount";
    public const string CommandExecutionTime = "ImportValidationData.CommandExecutionTime";
    public const string TenantId = "ImportValidationData.TenantId";
    public const string ImportRunType = "ImportValidationData.ImportRunType";
    public const string SourceCollectionId = "ImportValidationData.SourceCollectionId";
    public const string DataImportCollectionId = "ImportValidationData.DataImportCollectionId";
    public const string ServicesToInclude = "ImportValidationData.ServicesToInclude";
    public const string ServicesToExclude = "ImportValidationData.ServicesToExclude";
    public const string ImportPackagePath = "ImportPackagePath";
    public const string ProcessMapPath = "ProcessMapPath";
    public const string ImportPackage = "ImportPackage";
    public const string DataImportValidationSettingsRoot = "/Configuration/DataImport/Validation/Settings/";
    public const string DataImportPrefix = "DataImport.";
    public const string DacPacSizeThresholdInGB = "DataImport.DacPacSizeThresholdInGB";
    public const string LargestTableSizeThresholdInGB = "DataImport.LargestTableSizeThresholdInGB";
    public const string DatabaseMetadataSizeWarningThresholdInGB = "DataImport.DatabaseMetadataSizeWarningThresholdInGB";
    public const string DatabaseMetadataSizeErrorThresholdInGB = "DataImport.DatabaseMetadataSizeErrorThresholdInGB";
    public const string IdentitiesToImport = "DataImport.IdentitiesToImport";
    public const string SuccessfullyLoadedProperties = "DataImport.SuccessfullyLoadedProperties";
    public const string DefaultMinSqlServerVersion = "13.0.0.0";
    public const int DacPacSizeThresholdInGBDefault = 150;

    public class Commands
    {
      public const string Prepare = "prepare";
      public const string PrepareAlias = "p";
      public const string Validate = "validate";
      public const string ValidateAlias = "v";
      public const string Import = "import";
      public const string ImportAlias = "i";
      public const string IpList = "iplist";
      public const string IpListAlias = "l";
    }
  }
}
