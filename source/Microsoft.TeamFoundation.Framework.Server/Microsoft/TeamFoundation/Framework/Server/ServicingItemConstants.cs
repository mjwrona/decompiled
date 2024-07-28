// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingItemConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ServicingItemConstants
  {
    public static readonly string InitialCollectionProperties = nameof (InitialCollectionProperties);
    public static readonly string CollectionProperties = nameof (CollectionProperties);
    public static readonly string HostProperties = nameof (HostProperties);
    public static readonly string ConnectionInfo = nameof (ConnectionInfo);
    public static readonly string DboConnectionInfo = nameof (DboConnectionInfo);
    public static readonly string SourceConnectionInfo = nameof (SourceConnectionInfo);
    public static readonly string ConnectionString = nameof (ConnectionString);
    public static readonly string RedisConnectionStringZr = nameof (RedisConnectionStringZr);
    public static readonly string CollectionDatabaseId = nameof (CollectionDatabaseId);
    public static readonly string RequestingIdentity = nameof (RequestingIdentity);
    public static readonly string RequestingIdentityObject = nameof (RequestingIdentityObject);
    public static readonly string RequestingUserContext = nameof (RequestingUserContext);
    public static readonly string OriginalCollectionId = nameof (OriginalCollectionId);
    public static readonly string ProvisionedDataTiers = nameof (ProvisionedDataTiers);
    public static readonly string SourceInstanceId = nameof (SourceInstanceId);
    public static readonly string TargetInstanceId = nameof (TargetInstanceId);
    public static readonly string CollectionDatabaseReachable = nameof (CollectionDatabaseReachable);
    public static readonly string DeploymentHost = nameof (DeploymentHost);
    public static readonly string IdentityServicingHelper = nameof (IdentityServicingHelper);
    public static readonly string ExportIdentities = nameof (ExportIdentities);
    public static readonly string SecurityMigrationHandler = nameof (SecurityMigrationHandler);
    public static readonly string DatabaseCredential = nameof (DatabaseCredential);
    public static readonly string MigrationBlobContainers = nameof (MigrationBlobContainers);
    public static readonly string MigrationAdditionalBlobStorageInfo = nameof (MigrationAdditionalBlobStorageInfo);
    public static readonly string MigrationContainerLists = nameof (MigrationContainerLists);
    public static readonly string GitBlobContainerIds = nameof (GitBlobContainerIds);
    public static readonly string MigrationDatabaseLock = "MigrationLocks";
    public static readonly string AccountId = nameof (AccountId);
    public static readonly string AccountHostId = nameof (AccountHostId);
    public static readonly string AccountName = nameof (AccountName);
    public static readonly string ServicingLogLevel = nameof (ServicingLogLevel);
    public static readonly string AccountPreferences = nameof (AccountPreferences);
    public static readonly string PreCreateAccount = nameof (PreCreateAccount);
    public static readonly string PreCreatePoolId = nameof (PreCreatePoolId);
    public static readonly string CreateAccountHost = nameof (CreateAccountHost);
    public static readonly string Collation = nameof (Collation);
    public static readonly string CollectionCreationContext = nameof (CollectionCreationContext);
    public static readonly string CollectionName = nameof (CollectionName);
    public static readonly string SkipCollectionCreation = nameof (SkipCollectionCreation);
    public static readonly string GetResults = nameof (GetResults);
    public static readonly string ValidationErrors = nameof (ValidationErrors);
    public static readonly string ConsoleProgressWriter = nameof (ConsoleProgressWriter);
    public static readonly string SumBlobBytes = nameof (SumBlobBytes);
    public static readonly string HostedDeployment = nameof (HostedDeployment);
    public static readonly string VerifyFromDateTime = nameof (VerifyFromDateTime);
    public static readonly string VerifyToDateTime = nameof (VerifyToDateTime);
    public static readonly string DeploymentHealth = nameof (DeploymentHealth);
    public static readonly string GetStorageConnectionStringsFunc = nameof (GetStorageConnectionStringsFunc);
    public const string AcquireLockTimeoutInSeconds = "Servicing.AcquireLockTimeoutInSeconds";
    public const string AcquireLockMaxAttempts = "Servicing.AcquireLockMaxAttempts";
    public const string LongRunningThreshold = "Servicing.LongRunningThreshold";
    public static readonly string ResourceValidationContext = nameof (ResourceValidationContext);
    public static readonly string ConfigurationSecretsForStrongBox = nameof (ConfigurationSecretsForStrongBox);
    public const string HostInstanceMappings = "HostInstanceMappings";
    public static readonly string DatabaseReplicationConfiguration = nameof (DatabaseReplicationConfiguration);
    public static readonly string VmssHelper = nameof (VmssHelper);
    public static readonly string SourceVmssHelper = nameof (SourceVmssHelper);
    public static readonly string VmssCertificateReferences = nameof (VmssCertificateReferences);
    public static readonly string VmssConfigZip = nameof (VmssConfigZip);
  }
}
