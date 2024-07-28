// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.IKeyVaultClient
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.Rest;
using Microsoft.Rest.Azure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.KeyVault
{
  public interface IKeyVaultClient : IDisposable
  {
    Task<AzureOperationResponse<string>> GetPendingCertificateSigningRequestWithHttpMessagesAsync(
      string vault,
      string certificateName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    JsonSerializerSettings SerializationSettings { get; }

    JsonSerializerSettings DeserializationSettings { get; }

    ServiceClientCredentials Credentials { get; }

    string ApiVersion { get; }

    string AcceptLanguage { get; set; }

    int? LongRunningOperationRetryTimeout { get; set; }

    bool? GenerateClientRequestId { get; set; }

    Task<AzureOperationResponse<KeyBundle>> CreateKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      string kty,
      int? keySize = null,
      IList<string> keyOps = null,
      KeyAttributes keyAttributes = null,
      IDictionary<string, string> tags = null,
      string curve = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<KeyBundle>> ImportKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      JsonWebKey key,
      bool? hsm = null,
      KeyAttributes keyAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeletedKeyBundle>> DeleteKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<KeyBundle>> UpdateKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      IList<string> keyOps = null,
      KeyAttributes keyAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<KeyBundle>> GetKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<KeyItem>>> GetKeyVersionsWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<KeyItem>>> GetKeysWithHttpMessagesAsync(
      string vaultBaseUrl,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<BackupKeyResult>> BackupKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<KeyBundle>> RestoreKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      byte[] keyBundleBackup,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<KeyOperationResult>> EncryptWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      string algorithm,
      byte[] value,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<KeyOperationResult>> DecryptWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      string algorithm,
      byte[] value,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<KeyOperationResult>> SignWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      string algorithm,
      byte[] value,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<KeyVerifyResult>> VerifyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      string algorithm,
      byte[] digest,
      byte[] signature,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<KeyOperationResult>> WrapKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      string algorithm,
      byte[] value,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<KeyOperationResult>> UnwrapKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      string algorithm,
      byte[] value,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeletedKeyItem>>> GetDeletedKeysWithHttpMessagesAsync(
      string vaultBaseUrl,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeletedKeyBundle>> GetDeletedKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> PurgeDeletedKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<KeyBundle>> RecoverDeletedKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<SecretBundle>> SetSecretWithHttpMessagesAsync(
      string vaultBaseUrl,
      string secretName,
      string value,
      IDictionary<string, string> tags = null,
      string contentType = null,
      SecretAttributes secretAttributes = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeletedSecretBundle>> DeleteSecretWithHttpMessagesAsync(
      string vaultBaseUrl,
      string secretName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<SecretBundle>> UpdateSecretWithHttpMessagesAsync(
      string vaultBaseUrl,
      string secretName,
      string secretVersion,
      string contentType = null,
      SecretAttributes secretAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<SecretBundle>> GetSecretWithHttpMessagesAsync(
      string vaultBaseUrl,
      string secretName,
      string secretVersion,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<SecretItem>>> GetSecretsWithHttpMessagesAsync(
      string vaultBaseUrl,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<SecretItem>>> GetSecretVersionsWithHttpMessagesAsync(
      string vaultBaseUrl,
      string secretName,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeletedSecretItem>>> GetDeletedSecretsWithHttpMessagesAsync(
      string vaultBaseUrl,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeletedSecretBundle>> GetDeletedSecretWithHttpMessagesAsync(
      string vaultBaseUrl,
      string secretName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> PurgeDeletedSecretWithHttpMessagesAsync(
      string vaultBaseUrl,
      string secretName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<SecretBundle>> RecoverDeletedSecretWithHttpMessagesAsync(
      string vaultBaseUrl,
      string secretName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<BackupSecretResult>> BackupSecretWithHttpMessagesAsync(
      string vaultBaseUrl,
      string secretName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<SecretBundle>> RestoreSecretWithHttpMessagesAsync(
      string vaultBaseUrl,
      byte[] secretBundleBackup,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<CertificateItem>>> GetCertificatesWithHttpMessagesAsync(
      string vaultBaseUrl,
      int? maxresults = null,
      bool? includePending = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeletedCertificateBundle>> DeleteCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<Contacts>> SetCertificateContactsWithHttpMessagesAsync(
      string vaultBaseUrl,
      Contacts contacts,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<Contacts>> GetCertificateContactsWithHttpMessagesAsync(
      string vaultBaseUrl,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<Contacts>> DeleteCertificateContactsWithHttpMessagesAsync(
      string vaultBaseUrl,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<CertificateIssuerItem>>> GetCertificateIssuersWithHttpMessagesAsync(
      string vaultBaseUrl,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IssuerBundle>> SetCertificateIssuerWithHttpMessagesAsync(
      string vaultBaseUrl,
      string issuerName,
      string provider,
      IssuerCredentials credentials = null,
      OrganizationDetails organizationDetails = null,
      IssuerAttributes attributes = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IssuerBundle>> UpdateCertificateIssuerWithHttpMessagesAsync(
      string vaultBaseUrl,
      string issuerName,
      string provider = null,
      IssuerCredentials credentials = null,
      OrganizationDetails organizationDetails = null,
      IssuerAttributes attributes = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IssuerBundle>> GetCertificateIssuerWithHttpMessagesAsync(
      string vaultBaseUrl,
      string issuerName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IssuerBundle>> DeleteCertificateIssuerWithHttpMessagesAsync(
      string vaultBaseUrl,
      string issuerName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<CertificateOperation>> CreateCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      CertificatePolicy certificatePolicy = null,
      CertificateAttributes certificateAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<CertificateBundle>> ImportCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      string base64EncodedCertificate,
      string password = null,
      CertificatePolicy certificatePolicy = null,
      CertificateAttributes certificateAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<CertificateItem>>> GetCertificateVersionsWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<CertificatePolicy>> GetCertificatePolicyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<CertificatePolicy>> UpdateCertificatePolicyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      CertificatePolicy certificatePolicy,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<CertificateBundle>> UpdateCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      string certificateVersion,
      CertificatePolicy certificatePolicy = null,
      CertificateAttributes certificateAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<CertificateBundle>> GetCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      string certificateVersion,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<CertificateOperation>> UpdateCertificateOperationWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      bool cancellationRequested,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<CertificateOperation>> GetCertificateOperationWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<CertificateOperation>> DeleteCertificateOperationWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<CertificateBundle>> MergeCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      IList<byte[]> x509Certificates,
      CertificateAttributes certificateAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<BackupCertificateResult>> BackupCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<CertificateBundle>> RestoreCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      byte[] certificateBundleBackup,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeletedCertificateItem>>> GetDeletedCertificatesWithHttpMessagesAsync(
      string vaultBaseUrl,
      int? maxresults = null,
      bool? includePending = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeletedCertificateBundle>> GetDeletedCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> PurgeDeletedCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<CertificateBundle>> RecoverDeletedCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<StorageAccountItem>>> GetStorageAccountsWithHttpMessagesAsync(
      string vaultBaseUrl,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeletedStorageAccountItem>>> GetDeletedStorageAccountsWithHttpMessagesAsync(
      string vaultBaseUrl,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeletedStorageBundle>> GetDeletedStorageAccountWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> PurgeDeletedStorageAccountWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<StorageBundle>> RecoverDeletedStorageAccountWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<BackupStorageResult>> BackupStorageAccountWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<StorageBundle>> RestoreStorageAccountWithHttpMessagesAsync(
      string vaultBaseUrl,
      byte[] storageBundleBackup,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeletedStorageBundle>> DeleteStorageAccountWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<StorageBundle>> GetStorageAccountWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<StorageBundle>> SetStorageAccountWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      string resourceId,
      string activeKeyName,
      bool autoRegenerateKey,
      string regenerationPeriod = null,
      StorageAccountAttributes storageAccountAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<StorageBundle>> UpdateStorageAccountWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      string activeKeyName = null,
      bool? autoRegenerateKey = null,
      string regenerationPeriod = null,
      StorageAccountAttributes storageAccountAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<StorageBundle>> RegenerateStorageAccountKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      string keyName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<SasDefinitionItem>>> GetSasDefinitionsWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeletedSasDefinitionItem>>> GetDeletedSasDefinitionsWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeletedSasDefinitionBundle>> GetDeletedSasDefinitionWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      string sasDefinitionName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<SasDefinitionBundle>> RecoverDeletedSasDefinitionWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      string sasDefinitionName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeletedSasDefinitionBundle>> DeleteSasDefinitionWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      string sasDefinitionName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<SasDefinitionBundle>> GetSasDefinitionWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      string sasDefinitionName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<SasDefinitionBundle>> SetSasDefinitionWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      string sasDefinitionName,
      string templateUri,
      string sasType,
      string validityPeriod,
      SasDefinitionAttributes sasDefinitionAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<SasDefinitionBundle>> UpdateSasDefinitionWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      string sasDefinitionName,
      string templateUri = null,
      string sasType = null,
      string validityPeriod = null,
      SasDefinitionAttributes sasDefinitionAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<KeyItem>>> GetKeyVersionsNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<KeyItem>>> GetKeysNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeletedKeyItem>>> GetDeletedKeysNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<SecretItem>>> GetSecretsNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<SecretItem>>> GetSecretVersionsNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeletedSecretItem>>> GetDeletedSecretsNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<CertificateItem>>> GetCertificatesNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<CertificateIssuerItem>>> GetCertificateIssuersNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<CertificateItem>>> GetCertificateVersionsNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeletedCertificateItem>>> GetDeletedCertificatesNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<StorageAccountItem>>> GetStorageAccountsNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeletedStorageAccountItem>>> GetDeletedStorageAccountsNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<SasDefinitionItem>>> GetSasDefinitionsNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeletedSasDefinitionItem>>> GetDeletedSasDefinitionsNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
