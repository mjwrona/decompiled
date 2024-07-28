// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.KeyVaultClientExtensions
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.Rest.Azure;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.KeyVault
{
  public static class KeyVaultClientExtensions
  {
    public static async Task<KeyOperationResult> EncryptAsync(
      this IKeyVaultClient operations,
      string keyIdentifier,
      string algorithm,
      byte[] plainText,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (string.IsNullOrEmpty(keyIdentifier))
        throw new ArgumentNullException(nameof (keyIdentifier));
      if (string.IsNullOrEmpty(algorithm))
        throw new ArgumentNullException(nameof (algorithm));
      if (plainText == null)
        throw new ArgumentNullException(nameof (plainText));
      KeyIdentifier keyIdentifier1 = new KeyIdentifier(keyIdentifier);
      KeyOperationResult body;
      using (AzureOperationResponse<KeyOperationResult> operationResponse = await operations.EncryptWithHttpMessagesAsync(keyIdentifier1.Vault, keyIdentifier1.Name, keyIdentifier1.Version ?? string.Empty, algorithm, plainText, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyOperationResult> DecryptAsync(
      this IKeyVaultClient operations,
      string keyIdentifier,
      string algorithm,
      byte[] cipherText,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (string.IsNullOrEmpty(keyIdentifier))
        throw new ArgumentNullException(nameof (keyIdentifier));
      if (string.IsNullOrEmpty(algorithm))
        throw new ArgumentNullException(nameof (algorithm));
      if (cipherText == null)
        throw new ArgumentNullException(nameof (cipherText));
      KeyIdentifier keyIdentifier1 = new KeyIdentifier(keyIdentifier);
      KeyOperationResult body;
      using (AzureOperationResponse<KeyOperationResult> operationResponse = await operations.DecryptWithHttpMessagesAsync(keyIdentifier1.Vault, keyIdentifier1.Name, keyIdentifier1.Version ?? string.Empty, algorithm, cipherText, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyOperationResult> SignAsync(
      this IKeyVaultClient operations,
      string keyIdentifier,
      string algorithm,
      byte[] digest,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (string.IsNullOrEmpty(keyIdentifier))
        throw new ArgumentNullException(nameof (keyIdentifier));
      if (string.IsNullOrEmpty(algorithm))
        throw new ArgumentNullException(nameof (algorithm));
      if (digest == null)
        throw new ArgumentNullException(nameof (digest));
      KeyIdentifier keyIdentifier1 = new KeyIdentifier(keyIdentifier);
      KeyOperationResult body;
      using (AzureOperationResponse<KeyOperationResult> operationResponse = await operations.SignWithHttpMessagesAsync(keyIdentifier1.Vault, keyIdentifier1.Name, keyIdentifier1.Version ?? string.Empty, algorithm, digest, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<bool> VerifyAsync(
      this IKeyVaultClient operations,
      string keyIdentifier,
      string algorithm,
      byte[] digest,
      byte[] signature,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (string.IsNullOrEmpty(keyIdentifier))
        throw new ArgumentNullException(nameof (keyIdentifier));
      if (string.IsNullOrEmpty(algorithm))
        throw new ArgumentNullException(nameof (algorithm));
      if (digest == null)
        throw new ArgumentNullException(nameof (digest));
      if (signature == null)
        throw new ArgumentNullException(nameof (signature));
      KeyIdentifier keyIdentifier1 = new KeyIdentifier(keyIdentifier);
      bool flag1;
      using (AzureOperationResponse<KeyVerifyResult> operationResponse = await operations.VerifyWithHttpMessagesAsync(keyIdentifier1.Vault, keyIdentifier1.Name, keyIdentifier1.Version ?? string.Empty, algorithm, digest, signature, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        bool? nullable = operationResponse.Body.Value;
        bool flag2 = true;
        flag1 = nullable.GetValueOrDefault() == flag2 & nullable.HasValue;
      }
      return flag1;
    }

    public static async Task<KeyOperationResult> WrapKeyAsync(
      this IKeyVaultClient operations,
      string keyIdentifier,
      string algorithm,
      byte[] key,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (string.IsNullOrEmpty(keyIdentifier))
        throw new ArgumentNullException(nameof (keyIdentifier));
      if (string.IsNullOrEmpty(algorithm))
        throw new ArgumentNullException(nameof (algorithm));
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      KeyIdentifier keyIdentifier1 = new KeyIdentifier(keyIdentifier);
      KeyOperationResult body;
      using (AzureOperationResponse<KeyOperationResult> operationResponse = await operations.WrapKeyWithHttpMessagesAsync(keyIdentifier1.Vault, keyIdentifier1.Name, keyIdentifier1.Version ?? string.Empty, algorithm, key, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyOperationResult> UnwrapKeyAsync(
      this IKeyVaultClient operations,
      string keyIdentifier,
      string algorithm,
      byte[] wrappedKey,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (string.IsNullOrEmpty(keyIdentifier))
        throw new ArgumentNullException(nameof (keyIdentifier));
      if (string.IsNullOrEmpty(algorithm))
        throw new ArgumentNullException(nameof (algorithm));
      if (wrappedKey == null)
        throw new ArgumentNullException(nameof (wrappedKey));
      KeyIdentifier keyIdentifier1 = new KeyIdentifier(keyIdentifier);
      KeyOperationResult body;
      using (AzureOperationResponse<KeyOperationResult> operationResponse = await operations.UnwrapKeyWithHttpMessagesAsync(keyIdentifier1.Vault, keyIdentifier1.Name, keyIdentifier1.Version ?? string.Empty, algorithm, wrappedKey, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyBundle> GetKeyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (string.IsNullOrEmpty(vaultBaseUrl))
        throw new ArgumentNullException(nameof (vaultBaseUrl));
      if (string.IsNullOrEmpty(keyName))
        throw new ArgumentNullException(nameof (keyName));
      KeyBundle body;
      using (AzureOperationResponse<KeyBundle> operationResponse = await operations.GetKeyWithHttpMessagesAsync(vaultBaseUrl, keyName, string.Empty, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyBundle> GetKeyAsync(
      this IKeyVaultClient operations,
      string keyIdentifier,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyIdentifier keyIdentifier1 = !string.IsNullOrEmpty(keyIdentifier) ? new KeyIdentifier(keyIdentifier) : throw new ArgumentNullException(nameof (keyIdentifier));
      KeyBundle body;
      using (AzureOperationResponse<KeyBundle> operationResponse = await operations.GetKeyWithHttpMessagesAsync(keyIdentifier1.Vault, keyIdentifier1.Name, keyIdentifier1.Version ?? string.Empty, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyBundle> UpdateKeyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      string[] keyOps = null,
      KeyAttributes attributes = null,
      Dictionary<string, string> tags = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (string.IsNullOrEmpty(vaultBaseUrl))
        throw new ArgumentNullException(nameof (vaultBaseUrl));
      if (string.IsNullOrEmpty(keyName))
        throw new ArgumentNullException(nameof (keyName));
      KeyBundle body;
      using (AzureOperationResponse<KeyBundle> operationResponse = await operations.UpdateKeyWithHttpMessagesAsync(vaultBaseUrl, keyName, string.Empty, (IList<string>) keyOps, attributes, (IDictionary<string, string>) tags, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyBundle> UpdateKeyAsync(
      this IKeyVaultClient operations,
      string keyIdentifier,
      string[] keyOps = null,
      KeyAttributes attributes = null,
      Dictionary<string, string> tags = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyIdentifier keyIdentifier1 = !string.IsNullOrEmpty(keyIdentifier) ? new KeyIdentifier(keyIdentifier) : throw new ArgumentNullException(nameof (keyIdentifier));
      KeyBundle body;
      using (AzureOperationResponse<KeyBundle> operationResponse = await operations.UpdateKeyWithHttpMessagesAsync(keyIdentifier1.Vault, keyIdentifier1.Name, keyIdentifier1.Version ?? string.Empty, (IList<string>) keyOps, attributes, (IDictionary<string, string>) tags, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyBundle> CreateKeyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      NewKeyParameters parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (string.IsNullOrEmpty(vaultBaseUrl))
        throw new ArgumentNullException(nameof (vaultBaseUrl));
      if (string.IsNullOrEmpty(keyName))
        throw new ArgumentNullException(nameof (keyName));
      if (parameters == null)
        throw new ArgumentNullException(nameof (parameters));
      KeyBundle body;
      using (AzureOperationResponse<KeyBundle> operationResponse = await operations.CreateKeyWithHttpMessagesAsync(vaultBaseUrl, keyName, parameters.Kty, parameters.KeySize, parameters.KeyOps, parameters.Attributes, parameters.Tags, parameters.CurveName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyBundle> ImportKeyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      KeyBundle keyBundle,
      bool? importToHardware = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (string.IsNullOrEmpty(vaultBaseUrl))
        throw new ArgumentNullException(nameof (vaultBaseUrl));
      if (string.IsNullOrEmpty(keyName))
        throw new ArgumentNullException(nameof (keyName));
      if (keyBundle == null)
        throw new ArgumentNullException(nameof (keyBundle));
      KeyBundle body;
      using (AzureOperationResponse<KeyBundle> operationResponse = await operations.ImportKeyWithHttpMessagesAsync(vaultBaseUrl, keyName, keyBundle.Key, importToHardware, keyBundle.Attributes, keyBundle.Tags, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<SecretBundle> GetSecretAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string secretName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (string.IsNullOrEmpty(vaultBaseUrl))
        throw new ArgumentNullException(nameof (vaultBaseUrl));
      if (string.IsNullOrEmpty(secretName))
        throw new ArgumentNullException(nameof (secretName));
      SecretBundle body;
      using (AzureOperationResponse<SecretBundle> operationResponse = await operations.GetSecretWithHttpMessagesAsync(vaultBaseUrl, secretName, string.Empty, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<SecretBundle> GetSecretAsync(
      this IKeyVaultClient operations,
      string secretIdentifier,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SecretIdentifier secretIdentifier1 = !string.IsNullOrEmpty(secretIdentifier) ? new SecretIdentifier(secretIdentifier) : throw new ArgumentNullException(nameof (secretIdentifier));
      SecretBundle body;
      using (AzureOperationResponse<SecretBundle> operationResponse = await operations.GetSecretWithHttpMessagesAsync(secretIdentifier1.Vault, secretIdentifier1.Name, secretIdentifier1.Version ?? string.Empty, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<SecretBundle> UpdateSecretAsync(
      this IKeyVaultClient operations,
      string secretIdentifier,
      string contentType = null,
      SecretAttributes secretAttributes = null,
      Dictionary<string, string> tags = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SecretIdentifier secretIdentifier1 = !string.IsNullOrEmpty(secretIdentifier) ? new SecretIdentifier(secretIdentifier) : throw new ArgumentNullException(nameof (secretIdentifier));
      SecretBundle body;
      using (AzureOperationResponse<SecretBundle> operationResponse = await operations.UpdateSecretWithHttpMessagesAsync(secretIdentifier1.Vault, secretIdentifier1.Name, secretIdentifier1.Version, contentType, secretAttributes, (IDictionary<string, string>) tags, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<SecretBundle> RecoverDeletedSecretAsync(
      this IKeyVaultClient operations,
      string recoveryId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeletedSecretIdentifier secretIdentifier = !string.IsNullOrEmpty(recoveryId) ? new DeletedSecretIdentifier(recoveryId) : throw new ArgumentNullException(nameof (recoveryId));
      SecretBundle body;
      using (AzureOperationResponse<SecretBundle> operationResponse = await operations.RecoverDeletedSecretWithHttpMessagesAsync(secretIdentifier.Vault, secretIdentifier.Name, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyBundle> RecoverDeletedKeyAsync(
      this IKeyVaultClient operations,
      string recoveryId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeletedKeyIdentifier deletedKeyIdentifier = !string.IsNullOrEmpty(recoveryId) ? new DeletedKeyIdentifier(recoveryId) : throw new ArgumentNullException(nameof (recoveryId));
      KeyBundle body;
      using (AzureOperationResponse<KeyBundle> operationResponse = await operations.RecoverDeletedKeyWithHttpMessagesAsync(deletedKeyIdentifier.Vault, deletedKeyIdentifier.Name, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<CertificateBundle> RecoverDeletedCertificateAsync(
      this IKeyVaultClient operations,
      string recoveryId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeletedCertificateIdentifier certificateIdentifier = !string.IsNullOrEmpty(recoveryId) ? new DeletedCertificateIdentifier(recoveryId) : throw new ArgumentNullException(nameof (recoveryId));
      CertificateBundle body;
      using (AzureOperationResponse<CertificateBundle> operationResponse = await operations.RecoverDeletedCertificateWithHttpMessagesAsync(certificateIdentifier.Vault, certificateIdentifier.Name, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task PurgeDeletedSecretAsync(
      this IKeyVaultClient operations,
      string recoveryId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeletedSecretIdentifier secretIdentifier = !string.IsNullOrEmpty(recoveryId) ? new DeletedSecretIdentifier(recoveryId) : throw new ArgumentNullException(nameof (recoveryId));
      using (await operations.PurgeDeletedSecretWithHttpMessagesAsync(secretIdentifier.Vault, secretIdentifier.Name, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public static async Task PurgeDeletedKeyAsync(
      this IKeyVaultClient operations,
      string recoveryId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeletedKeyIdentifier deletedKeyIdentifier = !string.IsNullOrEmpty(recoveryId) ? new DeletedKeyIdentifier(recoveryId) : throw new ArgumentNullException(nameof (recoveryId));
      using (await operations.PurgeDeletedKeyWithHttpMessagesAsync(deletedKeyIdentifier.Vault, deletedKeyIdentifier.Name, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public static async Task PurgeDeletedCertificateAsync(
      this IKeyVaultClient operations,
      string recoveryId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeletedCertificateIdentifier certificateIdentifier = !string.IsNullOrEmpty(recoveryId) ? new DeletedCertificateIdentifier(recoveryId) : throw new ArgumentNullException(nameof (recoveryId));
      using (await operations.PurgeDeletedCertificateWithHttpMessagesAsync(certificateIdentifier.Vault, certificateIdentifier.Name, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public static async Task<CertificateBundle> GetCertificateAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (string.IsNullOrEmpty(vaultBaseUrl))
        throw new ArgumentNullException(nameof (vaultBaseUrl));
      if (string.IsNullOrEmpty(certificateName))
        throw new ArgumentNullException(nameof (certificateName));
      CertificateBundle body;
      using (AzureOperationResponse<CertificateBundle> operationResponse = await operations.GetCertificateWithHttpMessagesAsync(vaultBaseUrl, certificateName, string.Empty, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<CertificateBundle> GetCertificateAsync(
      this IKeyVaultClient operations,
      string certificateIdentifier,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CertificateIdentifier certificateIdentifier1 = !string.IsNullOrEmpty(certificateIdentifier) ? new CertificateIdentifier(certificateIdentifier) : throw new ArgumentNullException(nameof (certificateIdentifier));
      CertificateBundle body;
      using (AzureOperationResponse<CertificateBundle> operationResponse = await operations.GetCertificateWithHttpMessagesAsync(certificateIdentifier1.Vault, certificateIdentifier1.Name, certificateIdentifier1.Version ?? string.Empty, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<CertificateBundle> UpdateCertificateAsync(
      this IKeyVaultClient operations,
      string certificateIdentifier,
      CertificatePolicy certificatePolicy = null,
      CertificateAttributes certificateAttributes = null,
      IDictionary<string, string> tags = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CertificateIdentifier certificateIdentifier1 = !string.IsNullOrWhiteSpace(certificateIdentifier) ? new CertificateIdentifier(certificateIdentifier) : throw new ArgumentNullException(nameof (certificateIdentifier));
      CertificateBundle body;
      using (AzureOperationResponse<CertificateBundle> operationResponse = await operations.UpdateCertificateWithHttpMessagesAsync(certificateIdentifier1.Vault, certificateIdentifier1.Name, certificateIdentifier1.Version ?? string.Empty, certificatePolicy, certificateAttributes, tags, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<CertificateBundle> ImportCertificateAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      X509Certificate2Collection certificateCollection,
      CertificatePolicy certificatePolicy,
      CertificateAttributes certificateAttributes = null,
      IDictionary<string, string> tags = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (string.IsNullOrWhiteSpace(vaultBaseUrl))
        throw new ArgumentNullException(nameof (vaultBaseUrl));
      if (string.IsNullOrWhiteSpace(certificateName))
        throw new ArgumentNullException(nameof (certificateName));
      if (certificateCollection == null)
        throw new ArgumentNullException(nameof (certificateCollection));
      CertificateBundle body;
      using (AzureOperationResponse<CertificateBundle> operationResponse = await operations.ImportCertificateWithHttpMessagesAsync(vaultBaseUrl, certificateName, Convert.ToBase64String(certificateCollection.Export(X509ContentType.Pfx)), string.Empty, certificatePolicy, certificateAttributes, tags, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<CertificateBundle> MergeCertificateAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      X509Certificate2Collection x509Certificates,
      CertificateAttributes certificateAttributes = null,
      IDictionary<string, string> tags = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (string.IsNullOrWhiteSpace(vaultBaseUrl))
        throw new ArgumentNullException(nameof (vaultBaseUrl));
      if (string.IsNullOrWhiteSpace(certificateName))
        throw new ArgumentNullException(nameof (certificateName));
      if (x509Certificates == null || x509Certificates.Count == 0)
        throw new ArgumentException(nameof (x509Certificates));
      List<byte[]> x509Certificates1 = new List<byte[]>();
      foreach (X509Certificate2 x509Certificate in x509Certificates)
        x509Certificates1.Add(x509Certificate.Export(X509ContentType.Cert));
      CertificateBundle body;
      using (AzureOperationResponse<CertificateBundle> operationResponse = await operations.MergeCertificateWithHttpMessagesAsync(vaultBaseUrl, certificateName, (IList<byte[]>) x509Certificates1, certificateAttributes, tags, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<string> GetPendingCertificateSigningRequestAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (string.IsNullOrWhiteSpace(vaultBaseUrl))
        throw new ArgumentNullException(nameof (vaultBaseUrl));
      if (string.IsNullOrWhiteSpace(certificateName))
        throw new ArgumentNullException(nameof (certificateName));
      string body;
      using (AzureOperationResponse<string> operationResponse = await operations.GetPendingCertificateSigningRequestWithHttpMessagesAsync(vaultBaseUrl, certificateName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyBundle> CreateKeyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      string kty,
      int? keySize = null,
      IList<string> keyOps = null,
      KeyAttributes keyAttributes = null,
      IDictionary<string, string> tags = null,
      string curve = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyBundle body;
      using (AzureOperationResponse<KeyBundle> operationResponse = await operations.CreateKeyWithHttpMessagesAsync(vaultBaseUrl, keyName, kty, keySize, keyOps, keyAttributes, tags, curve, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyBundle> ImportKeyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      JsonWebKey key,
      bool? hsm = null,
      KeyAttributes keyAttributes = null,
      IDictionary<string, string> tags = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyBundle body;
      using (AzureOperationResponse<KeyBundle> operationResponse = await operations.ImportKeyWithHttpMessagesAsync(vaultBaseUrl, keyName, key, hsm, keyAttributes, tags, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<DeletedKeyBundle> DeleteKeyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeletedKeyBundle body;
      using (AzureOperationResponse<DeletedKeyBundle> operationResponse = await operations.DeleteKeyWithHttpMessagesAsync(vaultBaseUrl, keyName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyBundle> UpdateKeyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      IList<string> keyOps = null,
      KeyAttributes keyAttributes = null,
      IDictionary<string, string> tags = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyBundle body;
      using (AzureOperationResponse<KeyBundle> operationResponse = await operations.UpdateKeyWithHttpMessagesAsync(vaultBaseUrl, keyName, keyVersion, keyOps, keyAttributes, tags, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyBundle> GetKeyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyBundle body;
      using (AzureOperationResponse<KeyBundle> operationResponse = await operations.GetKeyWithHttpMessagesAsync(vaultBaseUrl, keyName, keyVersion, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<KeyItem>> GetKeyVersionsAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      int? maxresults = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<KeyItem> body;
      using (AzureOperationResponse<IPage<KeyItem>> operationResponse = await operations.GetKeyVersionsWithHttpMessagesAsync(vaultBaseUrl, keyName, maxresults, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<KeyItem>> GetKeysAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      int? maxresults = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<KeyItem> body;
      using (AzureOperationResponse<IPage<KeyItem>> operationResponse = await operations.GetKeysWithHttpMessagesAsync(vaultBaseUrl, maxresults, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<BackupKeyResult> BackupKeyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BackupKeyResult body;
      using (AzureOperationResponse<BackupKeyResult> operationResponse = await operations.BackupKeyWithHttpMessagesAsync(vaultBaseUrl, keyName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyBundle> RestoreKeyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      byte[] keyBundleBackup,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyBundle body;
      using (AzureOperationResponse<KeyBundle> operationResponse = await operations.RestoreKeyWithHttpMessagesAsync(vaultBaseUrl, keyBundleBackup, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyOperationResult> EncryptAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      string algorithm,
      byte[] value,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyOperationResult body;
      using (AzureOperationResponse<KeyOperationResult> operationResponse = await operations.EncryptWithHttpMessagesAsync(vaultBaseUrl, keyName, keyVersion, algorithm, value, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyOperationResult> DecryptAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      string algorithm,
      byte[] value,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyOperationResult body;
      using (AzureOperationResponse<KeyOperationResult> operationResponse = await operations.DecryptWithHttpMessagesAsync(vaultBaseUrl, keyName, keyVersion, algorithm, value, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyOperationResult> SignAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      string algorithm,
      byte[] value,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyOperationResult body;
      using (AzureOperationResponse<KeyOperationResult> operationResponse = await operations.SignWithHttpMessagesAsync(vaultBaseUrl, keyName, keyVersion, algorithm, value, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyVerifyResult> VerifyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      string algorithm,
      byte[] digest,
      byte[] signature,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVerifyResult body;
      using (AzureOperationResponse<KeyVerifyResult> operationResponse = await operations.VerifyWithHttpMessagesAsync(vaultBaseUrl, keyName, keyVersion, algorithm, digest, signature, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyOperationResult> WrapKeyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      string algorithm,
      byte[] value,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyOperationResult body;
      using (AzureOperationResponse<KeyOperationResult> operationResponse = await operations.WrapKeyWithHttpMessagesAsync(vaultBaseUrl, keyName, keyVersion, algorithm, value, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<KeyOperationResult> UnwrapKeyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      string algorithm,
      byte[] value,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyOperationResult body;
      using (AzureOperationResponse<KeyOperationResult> operationResponse = await operations.UnwrapKeyWithHttpMessagesAsync(vaultBaseUrl, keyName, keyVersion, algorithm, value, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<DeletedKeyItem>> GetDeletedKeysAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      int? maxresults = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeletedKeyItem> body;
      using (AzureOperationResponse<IPage<DeletedKeyItem>> operationResponse = await operations.GetDeletedKeysWithHttpMessagesAsync(vaultBaseUrl, maxresults, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<DeletedKeyBundle> GetDeletedKeyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeletedKeyBundle body;
      using (AzureOperationResponse<DeletedKeyBundle> operationResponse = await operations.GetDeletedKeyWithHttpMessagesAsync(vaultBaseUrl, keyName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task PurgeDeletedKeyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      (await operations.PurgeDeletedKeyWithHttpMessagesAsync(vaultBaseUrl, keyName, cancellationToken: cancellationToken).ConfigureAwait(false)).Dispose();
    }

    public static async Task<KeyBundle> RecoverDeletedKeyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string keyName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyBundle body;
      using (AzureOperationResponse<KeyBundle> operationResponse = await operations.RecoverDeletedKeyWithHttpMessagesAsync(vaultBaseUrl, keyName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<SecretBundle> SetSecretAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string secretName,
      string value,
      IDictionary<string, string> tags = null,
      string contentType = null,
      SecretAttributes secretAttributes = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SecretBundle body;
      using (AzureOperationResponse<SecretBundle> operationResponse = await operations.SetSecretWithHttpMessagesAsync(vaultBaseUrl, secretName, value, tags, contentType, secretAttributes, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<DeletedSecretBundle> DeleteSecretAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string secretName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeletedSecretBundle body;
      using (AzureOperationResponse<DeletedSecretBundle> operationResponse = await operations.DeleteSecretWithHttpMessagesAsync(vaultBaseUrl, secretName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<SecretBundle> UpdateSecretAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string secretName,
      string secretVersion,
      string contentType = null,
      SecretAttributes secretAttributes = null,
      IDictionary<string, string> tags = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SecretBundle body;
      using (AzureOperationResponse<SecretBundle> operationResponse = await operations.UpdateSecretWithHttpMessagesAsync(vaultBaseUrl, secretName, secretVersion, contentType, secretAttributes, tags, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<SecretBundle> GetSecretAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string secretName,
      string secretVersion,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SecretBundle body;
      using (AzureOperationResponse<SecretBundle> operationResponse = await operations.GetSecretWithHttpMessagesAsync(vaultBaseUrl, secretName, secretVersion, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<SecretItem>> GetSecretsAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      int? maxresults = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<SecretItem> body;
      using (AzureOperationResponse<IPage<SecretItem>> operationResponse = await operations.GetSecretsWithHttpMessagesAsync(vaultBaseUrl, maxresults, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<SecretItem>> GetSecretVersionsAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string secretName,
      int? maxresults = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<SecretItem> body;
      using (AzureOperationResponse<IPage<SecretItem>> operationResponse = await operations.GetSecretVersionsWithHttpMessagesAsync(vaultBaseUrl, secretName, maxresults, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<DeletedSecretItem>> GetDeletedSecretsAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      int? maxresults = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeletedSecretItem> body;
      using (AzureOperationResponse<IPage<DeletedSecretItem>> operationResponse = await operations.GetDeletedSecretsWithHttpMessagesAsync(vaultBaseUrl, maxresults, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<DeletedSecretBundle> GetDeletedSecretAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string secretName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeletedSecretBundle body;
      using (AzureOperationResponse<DeletedSecretBundle> operationResponse = await operations.GetDeletedSecretWithHttpMessagesAsync(vaultBaseUrl, secretName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task PurgeDeletedSecretAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string secretName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      (await operations.PurgeDeletedSecretWithHttpMessagesAsync(vaultBaseUrl, secretName, cancellationToken: cancellationToken).ConfigureAwait(false)).Dispose();
    }

    public static async Task<SecretBundle> RecoverDeletedSecretAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string secretName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SecretBundle body;
      using (AzureOperationResponse<SecretBundle> operationResponse = await operations.RecoverDeletedSecretWithHttpMessagesAsync(vaultBaseUrl, secretName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<BackupSecretResult> BackupSecretAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string secretName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BackupSecretResult body;
      using (AzureOperationResponse<BackupSecretResult> operationResponse = await operations.BackupSecretWithHttpMessagesAsync(vaultBaseUrl, secretName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<SecretBundle> RestoreSecretAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      byte[] secretBundleBackup,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SecretBundle body;
      using (AzureOperationResponse<SecretBundle> operationResponse = await operations.RestoreSecretWithHttpMessagesAsync(vaultBaseUrl, secretBundleBackup, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<CertificateItem>> GetCertificatesAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      int? maxresults = null,
      bool? includePending = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<CertificateItem> body;
      using (AzureOperationResponse<IPage<CertificateItem>> operationResponse = await operations.GetCertificatesWithHttpMessagesAsync(vaultBaseUrl, maxresults, includePending, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<DeletedCertificateBundle> DeleteCertificateAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeletedCertificateBundle body;
      using (AzureOperationResponse<DeletedCertificateBundle> operationResponse = await operations.DeleteCertificateWithHttpMessagesAsync(vaultBaseUrl, certificateName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<Contacts> SetCertificateContactsAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      Contacts contacts,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      Contacts body;
      using (AzureOperationResponse<Contacts> operationResponse = await operations.SetCertificateContactsWithHttpMessagesAsync(vaultBaseUrl, contacts, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<Contacts> GetCertificateContactsAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      Contacts body;
      using (AzureOperationResponse<Contacts> operationResponse = await operations.GetCertificateContactsWithHttpMessagesAsync(vaultBaseUrl, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<Contacts> DeleteCertificateContactsAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      Contacts body;
      using (AzureOperationResponse<Contacts> operationResponse = await operations.DeleteCertificateContactsWithHttpMessagesAsync(vaultBaseUrl, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<CertificateIssuerItem>> GetCertificateIssuersAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      int? maxresults = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<CertificateIssuerItem> body;
      using (AzureOperationResponse<IPage<CertificateIssuerItem>> operationResponse = await operations.GetCertificateIssuersWithHttpMessagesAsync(vaultBaseUrl, maxresults, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IssuerBundle> SetCertificateIssuerAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string issuerName,
      string provider,
      IssuerCredentials credentials = null,
      OrganizationDetails organizationDetails = null,
      IssuerAttributes attributes = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IssuerBundle body;
      using (AzureOperationResponse<IssuerBundle> operationResponse = await operations.SetCertificateIssuerWithHttpMessagesAsync(vaultBaseUrl, issuerName, provider, credentials, organizationDetails, attributes, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IssuerBundle> UpdateCertificateIssuerAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string issuerName,
      string provider = null,
      IssuerCredentials credentials = null,
      OrganizationDetails organizationDetails = null,
      IssuerAttributes attributes = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IssuerBundle body;
      using (AzureOperationResponse<IssuerBundle> operationResponse = await operations.UpdateCertificateIssuerWithHttpMessagesAsync(vaultBaseUrl, issuerName, provider, credentials, organizationDetails, attributes, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IssuerBundle> GetCertificateIssuerAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string issuerName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IssuerBundle body;
      using (AzureOperationResponse<IssuerBundle> operationResponse = await operations.GetCertificateIssuerWithHttpMessagesAsync(vaultBaseUrl, issuerName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IssuerBundle> DeleteCertificateIssuerAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string issuerName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IssuerBundle body;
      using (AzureOperationResponse<IssuerBundle> operationResponse = await operations.DeleteCertificateIssuerWithHttpMessagesAsync(vaultBaseUrl, issuerName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<CertificateOperation> CreateCertificateAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      CertificatePolicy certificatePolicy = null,
      CertificateAttributes certificateAttributes = null,
      IDictionary<string, string> tags = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CertificateOperation body;
      using (AzureOperationResponse<CertificateOperation> operationResponse = await operations.CreateCertificateWithHttpMessagesAsync(vaultBaseUrl, certificateName, certificatePolicy, certificateAttributes, tags, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<CertificateBundle> ImportCertificateAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      string base64EncodedCertificate,
      string password = null,
      CertificatePolicy certificatePolicy = null,
      CertificateAttributes certificateAttributes = null,
      IDictionary<string, string> tags = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CertificateBundle body;
      using (AzureOperationResponse<CertificateBundle> operationResponse = await operations.ImportCertificateWithHttpMessagesAsync(vaultBaseUrl, certificateName, base64EncodedCertificate, password, certificatePolicy, certificateAttributes, tags, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<CertificateItem>> GetCertificateVersionsAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      int? maxresults = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<CertificateItem> body;
      using (AzureOperationResponse<IPage<CertificateItem>> operationResponse = await operations.GetCertificateVersionsWithHttpMessagesAsync(vaultBaseUrl, certificateName, maxresults, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<CertificatePolicy> GetCertificatePolicyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CertificatePolicy body;
      using (AzureOperationResponse<CertificatePolicy> operationResponse = await operations.GetCertificatePolicyWithHttpMessagesAsync(vaultBaseUrl, certificateName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<CertificatePolicy> UpdateCertificatePolicyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      CertificatePolicy certificatePolicy,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CertificatePolicy body;
      using (AzureOperationResponse<CertificatePolicy> operationResponse = await operations.UpdateCertificatePolicyWithHttpMessagesAsync(vaultBaseUrl, certificateName, certificatePolicy, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<CertificateBundle> UpdateCertificateAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      string certificateVersion,
      CertificatePolicy certificatePolicy = null,
      CertificateAttributes certificateAttributes = null,
      IDictionary<string, string> tags = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CertificateBundle body;
      using (AzureOperationResponse<CertificateBundle> operationResponse = await operations.UpdateCertificateWithHttpMessagesAsync(vaultBaseUrl, certificateName, certificateVersion, certificatePolicy, certificateAttributes, tags, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<CertificateBundle> GetCertificateAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      string certificateVersion,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CertificateBundle body;
      using (AzureOperationResponse<CertificateBundle> operationResponse = await operations.GetCertificateWithHttpMessagesAsync(vaultBaseUrl, certificateName, certificateVersion, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<CertificateOperation> UpdateCertificateOperationAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      bool cancellationRequested,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CertificateOperation body;
      using (AzureOperationResponse<CertificateOperation> operationResponse = await operations.UpdateCertificateOperationWithHttpMessagesAsync(vaultBaseUrl, certificateName, cancellationRequested, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<CertificateOperation> GetCertificateOperationAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CertificateOperation body;
      using (AzureOperationResponse<CertificateOperation> operationResponse = await operations.GetCertificateOperationWithHttpMessagesAsync(vaultBaseUrl, certificateName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<CertificateOperation> DeleteCertificateOperationAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CertificateOperation body;
      using (AzureOperationResponse<CertificateOperation> operationResponse = await operations.DeleteCertificateOperationWithHttpMessagesAsync(vaultBaseUrl, certificateName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<CertificateBundle> MergeCertificateAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      IList<byte[]> x509Certificates,
      CertificateAttributes certificateAttributes = null,
      IDictionary<string, string> tags = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CertificateBundle body;
      using (AzureOperationResponse<CertificateBundle> operationResponse = await operations.MergeCertificateWithHttpMessagesAsync(vaultBaseUrl, certificateName, x509Certificates, certificateAttributes, tags, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<BackupCertificateResult> BackupCertificateAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BackupCertificateResult body;
      using (AzureOperationResponse<BackupCertificateResult> operationResponse = await operations.BackupCertificateWithHttpMessagesAsync(vaultBaseUrl, certificateName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<CertificateBundle> RestoreCertificateAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      byte[] certificateBundleBackup,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CertificateBundle body;
      using (AzureOperationResponse<CertificateBundle> operationResponse = await operations.RestoreCertificateWithHttpMessagesAsync(vaultBaseUrl, certificateBundleBackup, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<DeletedCertificateItem>> GetDeletedCertificatesAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      int? maxresults = null,
      bool? includePending = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeletedCertificateItem> body;
      using (AzureOperationResponse<IPage<DeletedCertificateItem>> operationResponse = await operations.GetDeletedCertificatesWithHttpMessagesAsync(vaultBaseUrl, maxresults, includePending, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<DeletedCertificateBundle> GetDeletedCertificateAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeletedCertificateBundle body;
      using (AzureOperationResponse<DeletedCertificateBundle> operationResponse = await operations.GetDeletedCertificateWithHttpMessagesAsync(vaultBaseUrl, certificateName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task PurgeDeletedCertificateAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      (await operations.PurgeDeletedCertificateWithHttpMessagesAsync(vaultBaseUrl, certificateName, cancellationToken: cancellationToken).ConfigureAwait(false)).Dispose();
    }

    public static async Task<CertificateBundle> RecoverDeletedCertificateAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string certificateName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CertificateBundle body;
      using (AzureOperationResponse<CertificateBundle> operationResponse = await operations.RecoverDeletedCertificateWithHttpMessagesAsync(vaultBaseUrl, certificateName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<StorageAccountItem>> GetStorageAccountsAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      int? maxresults = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<StorageAccountItem> body;
      using (AzureOperationResponse<IPage<StorageAccountItem>> operationResponse = await operations.GetStorageAccountsWithHttpMessagesAsync(vaultBaseUrl, maxresults, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<DeletedStorageAccountItem>> GetDeletedStorageAccountsAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      int? maxresults = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeletedStorageAccountItem> body;
      using (AzureOperationResponse<IPage<DeletedStorageAccountItem>> operationResponse = await operations.GetDeletedStorageAccountsWithHttpMessagesAsync(vaultBaseUrl, maxresults, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<DeletedStorageBundle> GetDeletedStorageAccountAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string storageAccountName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeletedStorageBundle body;
      using (AzureOperationResponse<DeletedStorageBundle> operationResponse = await operations.GetDeletedStorageAccountWithHttpMessagesAsync(vaultBaseUrl, storageAccountName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task PurgeDeletedStorageAccountAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string storageAccountName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      (await operations.PurgeDeletedStorageAccountWithHttpMessagesAsync(vaultBaseUrl, storageAccountName, cancellationToken: cancellationToken).ConfigureAwait(false)).Dispose();
    }

    public static async Task<StorageBundle> RecoverDeletedStorageAccountAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string storageAccountName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      StorageBundle body;
      using (AzureOperationResponse<StorageBundle> operationResponse = await operations.RecoverDeletedStorageAccountWithHttpMessagesAsync(vaultBaseUrl, storageAccountName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<BackupStorageResult> BackupStorageAccountAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string storageAccountName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BackupStorageResult body;
      using (AzureOperationResponse<BackupStorageResult> operationResponse = await operations.BackupStorageAccountWithHttpMessagesAsync(vaultBaseUrl, storageAccountName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<StorageBundle> RestoreStorageAccountAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      byte[] storageBundleBackup,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      StorageBundle body;
      using (AzureOperationResponse<StorageBundle> operationResponse = await operations.RestoreStorageAccountWithHttpMessagesAsync(vaultBaseUrl, storageBundleBackup, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<DeletedStorageBundle> DeleteStorageAccountAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string storageAccountName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeletedStorageBundle body;
      using (AzureOperationResponse<DeletedStorageBundle> operationResponse = await operations.DeleteStorageAccountWithHttpMessagesAsync(vaultBaseUrl, storageAccountName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<StorageBundle> GetStorageAccountAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string storageAccountName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      StorageBundle body;
      using (AzureOperationResponse<StorageBundle> operationResponse = await operations.GetStorageAccountWithHttpMessagesAsync(vaultBaseUrl, storageAccountName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<StorageBundle> SetStorageAccountAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string storageAccountName,
      string resourceId,
      string activeKeyName,
      bool autoRegenerateKey,
      string regenerationPeriod = null,
      StorageAccountAttributes storageAccountAttributes = null,
      IDictionary<string, string> tags = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      StorageBundle body;
      using (AzureOperationResponse<StorageBundle> operationResponse = await operations.SetStorageAccountWithHttpMessagesAsync(vaultBaseUrl, storageAccountName, resourceId, activeKeyName, autoRegenerateKey, regenerationPeriod, storageAccountAttributes, tags, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<StorageBundle> UpdateStorageAccountAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string storageAccountName,
      string activeKeyName = null,
      bool? autoRegenerateKey = null,
      string regenerationPeriod = null,
      StorageAccountAttributes storageAccountAttributes = null,
      IDictionary<string, string> tags = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      StorageBundle body;
      using (AzureOperationResponse<StorageBundle> operationResponse = await operations.UpdateStorageAccountWithHttpMessagesAsync(vaultBaseUrl, storageAccountName, activeKeyName, autoRegenerateKey, regenerationPeriod, storageAccountAttributes, tags, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<StorageBundle> RegenerateStorageAccountKeyAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string storageAccountName,
      string keyName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      StorageBundle body;
      using (AzureOperationResponse<StorageBundle> operationResponse = await operations.RegenerateStorageAccountKeyWithHttpMessagesAsync(vaultBaseUrl, storageAccountName, keyName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<SasDefinitionItem>> GetSasDefinitionsAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string storageAccountName,
      int? maxresults = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<SasDefinitionItem> body;
      using (AzureOperationResponse<IPage<SasDefinitionItem>> operationResponse = await operations.GetSasDefinitionsWithHttpMessagesAsync(vaultBaseUrl, storageAccountName, maxresults, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<DeletedSasDefinitionItem>> GetDeletedSasDefinitionsAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string storageAccountName,
      int? maxresults = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeletedSasDefinitionItem> body;
      using (AzureOperationResponse<IPage<DeletedSasDefinitionItem>> operationResponse = await operations.GetDeletedSasDefinitionsWithHttpMessagesAsync(vaultBaseUrl, storageAccountName, maxresults, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<DeletedSasDefinitionBundle> GetDeletedSasDefinitionAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string storageAccountName,
      string sasDefinitionName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeletedSasDefinitionBundle body;
      using (AzureOperationResponse<DeletedSasDefinitionBundle> operationResponse = await operations.GetDeletedSasDefinitionWithHttpMessagesAsync(vaultBaseUrl, storageAccountName, sasDefinitionName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<SasDefinitionBundle> RecoverDeletedSasDefinitionAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string storageAccountName,
      string sasDefinitionName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SasDefinitionBundle body;
      using (AzureOperationResponse<SasDefinitionBundle> operationResponse = await operations.RecoverDeletedSasDefinitionWithHttpMessagesAsync(vaultBaseUrl, storageAccountName, sasDefinitionName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<DeletedSasDefinitionBundle> DeleteSasDefinitionAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string storageAccountName,
      string sasDefinitionName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeletedSasDefinitionBundle body;
      using (AzureOperationResponse<DeletedSasDefinitionBundle> operationResponse = await operations.DeleteSasDefinitionWithHttpMessagesAsync(vaultBaseUrl, storageAccountName, sasDefinitionName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<SasDefinitionBundle> GetSasDefinitionAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string storageAccountName,
      string sasDefinitionName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SasDefinitionBundle body;
      using (AzureOperationResponse<SasDefinitionBundle> operationResponse = await operations.GetSasDefinitionWithHttpMessagesAsync(vaultBaseUrl, storageAccountName, sasDefinitionName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<SasDefinitionBundle> SetSasDefinitionAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string storageAccountName,
      string sasDefinitionName,
      string templateUri,
      string sasType,
      string validityPeriod,
      SasDefinitionAttributes sasDefinitionAttributes = null,
      IDictionary<string, string> tags = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SasDefinitionBundle body;
      using (AzureOperationResponse<SasDefinitionBundle> operationResponse = await operations.SetSasDefinitionWithHttpMessagesAsync(vaultBaseUrl, storageAccountName, sasDefinitionName, templateUri, sasType, validityPeriod, sasDefinitionAttributes, tags, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<SasDefinitionBundle> UpdateSasDefinitionAsync(
      this IKeyVaultClient operations,
      string vaultBaseUrl,
      string storageAccountName,
      string sasDefinitionName,
      string templateUri = null,
      string sasType = null,
      string validityPeriod = null,
      SasDefinitionAttributes sasDefinitionAttributes = null,
      IDictionary<string, string> tags = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SasDefinitionBundle body;
      using (AzureOperationResponse<SasDefinitionBundle> operationResponse = await operations.UpdateSasDefinitionWithHttpMessagesAsync(vaultBaseUrl, storageAccountName, sasDefinitionName, templateUri, sasType, validityPeriod, sasDefinitionAttributes, tags, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<KeyItem>> GetKeyVersionsNextAsync(
      this IKeyVaultClient operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<KeyItem> body;
      using (AzureOperationResponse<IPage<KeyItem>> operationResponse = await operations.GetKeyVersionsNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<KeyItem>> GetKeysNextAsync(
      this IKeyVaultClient operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<KeyItem> body;
      using (AzureOperationResponse<IPage<KeyItem>> operationResponse = await operations.GetKeysNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<DeletedKeyItem>> GetDeletedKeysNextAsync(
      this IKeyVaultClient operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeletedKeyItem> body;
      using (AzureOperationResponse<IPage<DeletedKeyItem>> operationResponse = await operations.GetDeletedKeysNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<SecretItem>> GetSecretsNextAsync(
      this IKeyVaultClient operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<SecretItem> body;
      using (AzureOperationResponse<IPage<SecretItem>> operationResponse = await operations.GetSecretsNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<SecretItem>> GetSecretVersionsNextAsync(
      this IKeyVaultClient operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<SecretItem> body;
      using (AzureOperationResponse<IPage<SecretItem>> operationResponse = await operations.GetSecretVersionsNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<DeletedSecretItem>> GetDeletedSecretsNextAsync(
      this IKeyVaultClient operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeletedSecretItem> body;
      using (AzureOperationResponse<IPage<DeletedSecretItem>> operationResponse = await operations.GetDeletedSecretsNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<CertificateItem>> GetCertificatesNextAsync(
      this IKeyVaultClient operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<CertificateItem> body;
      using (AzureOperationResponse<IPage<CertificateItem>> operationResponse = await operations.GetCertificatesNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<CertificateIssuerItem>> GetCertificateIssuersNextAsync(
      this IKeyVaultClient operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<CertificateIssuerItem> body;
      using (AzureOperationResponse<IPage<CertificateIssuerItem>> operationResponse = await operations.GetCertificateIssuersNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<CertificateItem>> GetCertificateVersionsNextAsync(
      this IKeyVaultClient operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<CertificateItem> body;
      using (AzureOperationResponse<IPage<CertificateItem>> operationResponse = await operations.GetCertificateVersionsNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<DeletedCertificateItem>> GetDeletedCertificatesNextAsync(
      this IKeyVaultClient operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeletedCertificateItem> body;
      using (AzureOperationResponse<IPage<DeletedCertificateItem>> operationResponse = await operations.GetDeletedCertificatesNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<StorageAccountItem>> GetStorageAccountsNextAsync(
      this IKeyVaultClient operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<StorageAccountItem> body;
      using (AzureOperationResponse<IPage<StorageAccountItem>> operationResponse = await operations.GetStorageAccountsNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<DeletedStorageAccountItem>> GetDeletedStorageAccountsNextAsync(
      this IKeyVaultClient operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeletedStorageAccountItem> body;
      using (AzureOperationResponse<IPage<DeletedStorageAccountItem>> operationResponse = await operations.GetDeletedStorageAccountsNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<SasDefinitionItem>> GetSasDefinitionsNextAsync(
      this IKeyVaultClient operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<SasDefinitionItem> body;
      using (AzureOperationResponse<IPage<SasDefinitionItem>> operationResponse = await operations.GetSasDefinitionsNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }

    public static async Task<IPage<DeletedSasDefinitionItem>> GetDeletedSasDefinitionsNextAsync(
      this IKeyVaultClient operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeletedSasDefinitionItem> body;
      using (AzureOperationResponse<IPage<DeletedSasDefinitionItem>> operationResponse = await operations.GetDeletedSasDefinitionsNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = operationResponse.Body;
      return body;
    }
  }
}
