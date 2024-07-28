// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.KeyVaultProvider2
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.Rest;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  internal class KeyVaultProvider2
  {
    private const string c_layer = "JiraConnectAppKeyVaultProvider2";
    private readonly IVssRequestContext requestContext;
    private readonly string vaultBaseUrl;
    private readonly ISecretProvider secretProvider;
    private readonly CommandSetter commandSetter;
    private bool m_isDevFabric;
    private bool m_useManagedIdentity;
    private static readonly string DefaultKeyVaultDnsSuffix = "vault.azure.net";
    private static readonly TimeSpan GetKeyVaultSecretValueCommandDefaultTimeout = TimeSpan.FromMinutes(1.0);
    private static readonly int CircuitBreakerRequestVolumeThreshold = 10;

    internal KeyVaultProvider2(IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
      string keyVaultName = KeyVaultProvider2.GetKeyVaultName(requestContext);
      this.vaultBaseUrl = "vault://" + keyVaultName;
      this.GetFeatureFlagValues(requestContext);
      this.secretProvider = this.CreateSecretProvider();
      string str = keyVaultName + "." + KeyVaultProvider2.DefaultKeyVaultDnsSuffix;
      this.commandSetter = CommandSetter.WithGroupKey((CommandGroupKey) "Pipelines.").AndCommandKey((CommandKey) str).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(KeyVaultProvider2.GetKeyVaultSecretValueCommandDefaultTimeout).WithCircuitBreakerRequestVolumeThreshold(KeyVaultProvider2.CircuitBreakerRequestVolumeThreshold));
    }

    internal SecretChangedResult DeleteSecret(string key) => new CommandService<SecretChangedResult>(this.requestContext, this.commandSetter, (Func<SecretChangedResult>) (() =>
    {
      try
      {
        return ((IEnumerable<SecretChangedResult>) ((IWritableSecretProvider) this.secretProvider).DeleteSecrets(this.vaultBaseUrl + "/" + key, false)).First<SecretChangedResult>();
      }
      catch (SecretProviderException ex)
      {
        ((Exception) ex).Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
        throw;
      }
      catch (Exception ex)
      {
        this.requestContext.TraceError(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, "JiraConnectAppKeyVaultProvider2", string.Format("{0}- Delete key vault request throwed exception '{1}'", (object) nameof (DeleteSecret), (object) ex));
        return (SecretChangedResult) null;
      }
    })).Execute();

    internal bool PurgeSecret(string key) => new CommandService<bool>(this.requestContext, this.commandSetter, (Func<bool>) (() =>
    {
      try
      {
        ((IWritableSecretProvider) this.secretProvider).PurgeDeletedSecrets(this.vaultBaseUrl + "/" + key, false);
        return true;
      }
      catch (SecretProviderException ex)
      {
        ((Exception) ex).Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
        throw;
      }
      catch (Exception ex)
      {
        this.requestContext.TraceError(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, "JiraConnectAppKeyVaultProvider2", string.Format("{0}- Purge key vault request throwed exception '{1}'", (object) nameof (PurgeSecret), (object) ex));
        return false;
      }
    })).Execute();

    internal string GetSecret(string key) => new CommandService<string>(this.requestContext, this.commandSetter, (Func<string>) (() =>
    {
      try
      {
        return ((IReadOnlySecretProvider) this.secretProvider).GetSecret(this.vaultBaseUrl + "/" + key, "") is SecretString secret2 ? secret2.Value : (string) null;
      }
      catch (SecretProviderException ex)
      {
        ((Exception) ex).Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
        throw;
      }
      catch (Exception ex)
      {
        this.requestContext.TraceError(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, "JiraConnectAppKeyVaultProvider2", string.Format("{0}- Get key vault request throwed exception '{1}'", (object) nameof (GetSecret), (object) ex));
        return (string) null;
      }
    })).Execute();

    internal SecretString SetSecret(string key, string value) => new CommandService<SecretString>(this.requestContext, this.commandSetter, (Func<SecretString>) (() =>
    {
      try
      {
        string str = this.vaultBaseUrl + "/" + key;
        SecretString secretString1 = new SecretString();
        ((SecretObject) secretString1).SecretReference = str;
        secretString1.Value = value;
        SecretString secretString2 = secretString1;
        ((IWritableSecretProvider) this.secretProvider).SetSecret((SecretObject) secretString2, false);
        return secretString2;
      }
      catch (SecretProviderException ex)
      {
        ((Exception) ex).Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
        throw;
      }
      catch (Exception ex)
      {
        this.requestContext.TraceError(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, "JiraConnectAppKeyVaultProvider2", string.Format("{0}- Set key vault request throwed exception '{1}'", (object) nameof (SetSecret), (object) ex));
        throw;
      }
    })).Execute();

    private static string GetKeyVaultName(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string keyVaultName = vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) "/Configuration/ConnectedService/PipelinesJiraConnectApp/KeyVault", (string) null);
      if (!string.IsNullOrWhiteSpace(keyVaultName))
        return keyVaultName;
      requestContext.TraceError(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, "JiraConnectAppKeyVaultProvider2", "GetKeyVaultName- key vault name not set in registry");
      throw new ArgumentException("vaultName");
    }

    private void GetFeatureFlagValues(IVssRequestContext requestContext)
    {
      this.m_isDevFabric = requestContext.ExecutionEnvironment.IsDevFabricDeployment;
      this.m_useManagedIdentity = requestContext.IsFeatureEnabled("AzureDevOps.Services.ManagedIdentity.UseForKeyVaultAccess");
    }

    private ISecretProvider CreateSecretProvider() => (ISecretProvider) new SecretProvider(SecretProviderFactory.CreateStorageProvider(this.m_isDevFabric, new KeyVaultSecretStorageProvider(KeyVaultClientAdapterFactory.GetKeyVaultClientAdapter((ServiceClientCredentials) new DefaultKeyVaultCredentials(this.m_useManagedIdentity), true, (ITFLogger) null), (ITFLogger) null), (ITFLogger) null), (ITFLogger) null, false);
  }
}
