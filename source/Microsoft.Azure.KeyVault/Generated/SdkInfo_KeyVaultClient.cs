// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.SdkInfo
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.KeyVault
{
  internal static class SdkInfo
  {
    public static IEnumerable<Tuple<string, string, string>> ApiInfo_KeyVaultClient => ((IEnumerable<Tuple<string, string, string>>) new Tuple<string, string, string>[78]
    {
      new Tuple<string, string, string>("KeyVaultClient", "BackupCertificate", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "BackupKey", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "BackupSecret", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "BackupStorageAccount", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "CreateCertificate", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "CreateKey", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "DeleteCertificate", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "DeleteCertificateContacts", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "DeleteCertificateIssuer", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "DeleteCertificateOperation", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "DeleteKey", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "DeleteSasDefinition", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "DeleteSecret", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "DeleteStorageAccount", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetCertificate", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetCertificateContacts", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetCertificateIssuer", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetCertificateIssuers", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetCertificateOperation", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetCertificatePolicy", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetCertificateVersions", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetCertificates", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetDeletedCertificate", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetDeletedCertificates", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetDeletedKey", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetDeletedKeys", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetDeletedSasDefinition", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetDeletedSasDefinitions", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetDeletedSecret", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetDeletedSecrets", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetDeletedStorageAccount", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetDeletedStorageAccounts", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetKey", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetKeyVersions", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetKeys", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetSasDefinition", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetSasDefinitions", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetSecret", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetSecretVersions", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetSecrets", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetStorageAccount", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "GetStorageAccounts", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "ImportCertificate", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "ImportKey", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "MergeCertificate", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "PurgeDeletedCertificate", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "PurgeDeletedKey", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "PurgeDeletedSecret", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "PurgeDeletedStorageAccount", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "RecoverDeletedCertificate", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "RecoverDeletedKey", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "RecoverDeletedSasDefinition", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "RecoverDeletedSecret", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "RecoverDeletedStorageAccount", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "RegenerateStorageAccountKey", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "RestoreCertificate", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "RestoreKey", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "RestoreSecret", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "RestoreStorageAccount", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "SetCertificateContacts", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "SetCertificateIssuer", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "SetSasDefinition", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "SetSecret", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "SetStorageAccount", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "UpdateCertificate", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "UpdateCertificateIssuer", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "UpdateCertificateOperation", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "UpdateCertificatePolicy", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "UpdateKey", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "UpdateSasDefinition", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "UpdateSecret", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "UpdateStorageAccount", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "decrypt", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "encrypt", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "sign", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "unwrapKey", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "verify", "7.0"),
      new Tuple<string, string, string>("KeyVaultClient", "wrapKey", "7.0")
    }).AsEnumerable<Tuple<string, string, string>>();
  }
}
