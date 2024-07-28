// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RuntimeKeyVaultProviderExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.CloudConfiguration;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class RuntimeKeyVaultProviderExtensions
  {
    public static SecretObject GetSecret(
      this ISecretProvider secretProvider,
      Uri keyVaultSecretIdentifier)
    {
      SecretReference secretReference = ((IReadOnlySecretProvider) secretProvider).SecretProviderUriToSecretReference(keyVaultSecretIdentifier, false);
      return ((IReadOnlySecretProvider) secretProvider).GetSecret(secretReference.ToString(), "");
    }

    public static List<SecretObject> GetSecrets(
      this ISecretProvider secretProvider,
      Uri keyVaultSecretIdentifier,
      bool isArray)
    {
      SecretReference secretReference = ((IReadOnlySecretProvider) secretProvider).SecretProviderUriToSecretReference(keyVaultSecretIdentifier, isArray);
      return ((IReadOnlySecretProvider) secretProvider).GetSecrets(secretReference.ToString());
    }
  }
}
