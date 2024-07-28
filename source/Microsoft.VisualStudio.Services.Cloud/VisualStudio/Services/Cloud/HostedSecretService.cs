// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostedSecretService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Rest;
using Microsoft.ServiceBus.Messaging;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Microsoft.VisualStudio.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class HostedSecretService : IVssSecretService, IVssFrameworkService
  {
    private Func<ISecretProvider> m_secretProviderFactory;
    private Lazy<ISecretProvider> m_secretProvider;
    private ITFLogger m_logger;
    private bool m_isDevFabric;
    private bool m_useManagedIdentity;
    private static readonly string s_runtimeRealmVaultSetting = "RuntimeRealmVault";
    private static readonly TimeSpan s_staleSecretAlertThreshold = TimeSpan.FromMinutes(10.0);
    private static readonly string s_secretRotationSharedAccessKeyToken = "SecretRotationSharedAccessKeyValue";
    private static readonly string s_area = "Secrets";
    private static readonly string s_layer = "Service";

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckSystemRequestContext();
      systemRequestContext.CheckDeploymentRequestContext();
      this.m_isDevFabric = systemRequestContext.ExecutionEnvironment.IsDevFabricDeployment;
      this.m_logger = (ITFLogger) new TraceLogger((ITraceRequest) new RawTraceRequest(systemRequestContext.E2EId), HostedSecretService.s_area, HostedSecretService.s_layer);
      if (AzureRoleUtil.IsAvailable)
        HostedSecretService.CheckRealmVaultName();
      this.InitializeSecretProvider(systemRequestContext);
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnFeatureFlagChanged), string.Format(FeatureAvailbilityRegistryConstants.FeatureStateRegistryFormat, (object) "AzureDevOps.Services.ManagedIdentity.UseForKeyVaultAccess"));
    }

    private void OnFeatureFlagChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceAlways(15117093, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, "Recreating secret provider since the identity to use has changed.");
      this.InitializeSecretProvider(requestContext);
    }

    private void InitializeSecretProvider(IVssRequestContext requestContext)
    {
      this.m_useManagedIdentity = requestContext.IsFeatureEnabled("AzureDevOps.Services.ManagedIdentity.UseForKeyVaultAccess");
      this.m_secretProvider = new Lazy<ISecretProvider>(this.m_secretProviderFactory);
    }

    private static void CheckRealmVaultName()
    {
      if (string.IsNullOrEmpty(AzureRoleUtil.GetOverridableConfigurationSetting(HostedSecretService.s_runtimeRealmVaultSetting)))
        throw new Exception(HostedSecretService.s_runtimeRealmVaultSetting + " is not defined, no runtime key vaults available");
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnFeatureFlagChanged));

    public HostedSecretService()
    {
      this.m_secretProviderFactory = new Func<ISecretProvider>(this.CreateSecretProvider);
      this.m_logger = (ITFLogger) new NullLogger();
    }

    internal HostedSecretService(ISecretProvider testSecretProvider)
    {
      this.m_secretProviderFactory = (Func<ISecretProvider>) (() => testSecretProvider);
      this.m_secretProvider = new Lazy<ISecretProvider>(this.m_secretProviderFactory);
      this.m_logger = (ITFLogger) new NullLogger();
    }

    public bool UpdateSecret(
      IVssRequestContext requestContext,
      Uri keyVaultSecretIdentifier,
      bool verboseTrace)
    {
      try
      {
        requestContext.TraceEnter(15117010, HostedSecretService.s_area, HostedSecretService.s_layer, nameof (UpdateSecret));
        requestContext.CheckDeploymentRequestContext();
        requestContext.CheckSystemRequestContext();
        ITeamFoundationStrongBoxService service1 = requestContext.GetService<ITeamFoundationStrongBoxService>();
        IVssRegistryService service2 = requestContext.GetService<IVssRegistryService>();
        string suffixString;
        string mappingRegistryPath = HostedSecretService.GetMappingRegistryPath(keyVaultSecretIdentifier, out suffixString);
        IVssRequestContext requestContext1 = requestContext;
        // ISSUE: explicit reference operation
        ref RegistryQuery local = @(RegistryQuery) mappingRegistryPath;
        string serializedObject = service2.GetValue(requestContext1, in local, (string) null);
        if (string.IsNullOrEmpty(serializedObject))
        {
          requestContext.Trace(15117011, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, "Key Vault SecretIdentifier {0} doesn't have a StrongBox mapping registered. Skipping ConfigurationSecrets drawer update.", (object) keyVaultSecretIdentifier);
          return false;
        }
        StrongBoxMapping strongBoxMapping = TeamFoundationSerializationUtility.Deserialize<StrongBoxMapping>(serializedObject);
        Guid drawer = service1.UnlockOrCreateDrawer(requestContext, "ConfigurationSecrets");
        SecretObject secret = this.GetSecretProvider(requestContext).GetSecret(keyVaultSecretIdentifier);
        string lookupKey = strongBoxMapping.LookupKey + suffixString;
        this.UpdateStrongBox(requestContext, secret, drawer, lookupKey, false, verboseTrace);
        if (secret != null)
          requestContext.TraceAlways(15117104, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, "Secret at URI: " + keyVaultSecretIdentifier.ToString() + " was updated");
        else
          requestContext.TraceAlways(15117105, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, "Secret at URI: " + keyVaultSecretIdentifier.ToString() + " was removed");
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15117018, HostedSecretService.s_area, HostedSecretService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(15117019, HostedSecretService.s_area, HostedSecretService.s_layer, nameof (UpdateSecret));
      }
    }

    public int UpdateRegisteredSecrets(
      IVssRequestContext requestContext,
      bool alertOnStaleValues,
      bool verboseTrace)
    {
      return this.UpdateRegisteredSecrets(requestContext, alertOnStaleValues, new HashSet<string>(), verboseTrace);
    }

    public void RegisterSecrets(
      IVssRequestContext requestContext,
      List<ConfigurationSecretData> secrets,
      bool deleteMissing,
      ITFLogger logger = null)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.CheckSystemRequestContext();
      if (logger == null)
        logger = (ITFLogger) new NullLogger();
      logger.Info("Executing RegisterSecrets. Number of secrets: {0}, deleteMissing: {1}", (object) secrets.Count, (object) deleteMissing);
      ITeamFoundationStrongBoxService service1 = requestContext.GetService<ITeamFoundationStrongBoxService>();
      IVssRegistryService service2 = requestContext.GetService<IVssRegistryService>();
      logger.Info("Unlocking StrongBox drawer: ConfigurationSecrets");
      Guid drawer = service1.UnlockOrCreateDrawer(requestContext, "ConfigurationSecrets");
      logger.Info("Drawer id: {0}", (object) drawer);
      HashSet<string> placedLookupKeysSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, List<KeyVaultConfigurationSecretData>> dictionary = new Dictionary<string, List<KeyVaultConfigurationSecretData>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      logger.Info("Getting registered secret entries");
      RegistryEntryCollection registeredSecretEntries1 = this.GetRegisteredSecretEntries(requestContext);
      logger.Info("Found {0} entries.", (object) registeredSecretEntries1.Count);
      List<Uri> secretsToUpdate = new List<Uri>();
      ISecretProvider secretProvider = this.GetSecretProvider(requestContext);
      foreach (ConfigurationSecretData secret in secrets)
      {
        logger.Info("Processing {0}", (object) secret.SecretName);
        KeyVaultConfigurationSecretData configurationSecretData1 = secret as KeyVaultConfigurationSecretData;
        HardcodedConfigurationSecretData configurationSecretData2 = secret as HardcodedConfigurationSecretData;
        if (configurationSecretData1 != null)
        {
          secretsToUpdate.Add(configurationSecretData1.KeyVaultSecretIdentifier);
          StrongBoxMapping mapping = new StrongBoxMapping()
          {
            LookupKey = secret.SecretName,
            IsArray = secret.IsArray
          };
          if (secret.IsArray)
            secretProvider.GetSecrets(configurationSecretData1.KeyVaultSecretIdentifier, secret.IsArray);
          else
            secretProvider.GetSecret(configurationSecretData1.KeyVaultSecretIdentifier);
          string registryPath = HostedSecretService.GetMappingRegistryPath(configurationSecretData1.KeyVaultSecretIdentifier);
          foreach (string registryPathPattern in registeredSecretEntries1.Where<RegistryEntry>((Func<RegistryEntry, bool>) (entry => string.Equals(this.GetMapping(entry).LookupKey, mapping.LookupKey, StringComparison.OrdinalIgnoreCase) && !string.Equals(entry.Path, registryPath, StringComparison.Ordinal))).Select<RegistryEntry, string>((Func<RegistryEntry, string>) (entryToRemove => entryToRemove.Path)))
          {
            logger.Warning("Removing registry entry " + registryPathPattern + " because it is a conflicting KeyVault-StrongBox mapping");
            service2.DeleteEntries(requestContext, registryPathPattern);
          }
          string str = TeamFoundationSerializationUtility.SerializeToString<StrongBoxMapping>(mapping);
          service2.SetValue<string>(requestContext, registryPath, str);
          List<KeyVaultConfigurationSecretData> configurationSecretDataList;
          if (!dictionary.TryGetValue(registryPath, out configurationSecretDataList))
          {
            configurationSecretDataList = new List<KeyVaultConfigurationSecretData>();
            dictionary.Add(registryPath, configurationSecretDataList);
          }
          configurationSecretDataList.Add(configurationSecretData1);
        }
        else
        {
          if (configurationSecretData2 == null)
            throw new NotSupportedException(secret.GetType().FullName + " is not supported.");
          for (int index = 0; index < configurationSecretData2.Values.Length; ++index)
          {
            string secretName = secret.SecretName;
            if (secret.IsArray)
              secretName += index.ToString();
            StrongBoxItemInfo info = new StrongBoxItemInfo()
            {
              DrawerId = drawer,
              LookupKey = secretName,
              CredentialName = configurationSecretData2.CredentialName
            };
            object obj = configurationSecretData2.Values[index];
            X509Certificate2 cert = obj as X509Certificate2;
            string connectionString = obj as string;
            if (obj == null)
            {
              logger.Warning(secretName + " is null. Skipping addition to strongBox.");
            }
            else
            {
              if (cert != null)
                service1.AddCertificate(requestContext, info, cert);
              else if (connectionString != null)
              {
                CloudStorageAccount account;
                if (CloudStorageAccount.TryParse(connectionString, out account))
                  info.CredentialName = account.Credentials.AccountName;
                service1.AddString(requestContext, info, connectionString);
              }
              else
                throw new NotSupportedException("Error processing secret '" + secret.SecretName + "': Secret type " + obj.GetType().FullName + " is not supported.");
              placedLookupKeysSet.Add(secretName);
            }
          }
        }
      }
      bool flag = false;
      foreach (List<KeyVaultConfigurationSecretData> source in dictionary.Values)
      {
        if (source.Count > 1)
        {
          string str = string.Join(", ", source.Select<KeyVaultConfigurationSecretData, string>((Func<KeyVaultConfigurationSecretData, string>) (s => s.SecretName)));
          logger.Error("A runtime Key Vault secret location can only be referred to by only one LightRail setting. The following LightRail settings refer to the same key vault location: " + str);
          flag = true;
        }
      }
      if (flag)
        return;
      if (deleteMissing)
      {
        logger.Info("Deleting any Key Vault mappings that were not set.");
        RegistryEntryCollection registeredSecretEntries2 = this.GetRegisteredSecretEntries(requestContext);
        List<string> registryPathPatterns = new List<string>();
        foreach (RegistryEntry registryEntry in registeredSecretEntries2)
        {
          if (!dictionary.ContainsKey(registryEntry.Path))
          {
            logger.Info("Deleting " + registryEntry.Path + " from the registry.");
            registryPathPatterns.Add(registryEntry.Path);
          }
        }
        if (registryPathPatterns.Count > 0)
          service2.DeleteEntries(requestContext, (IEnumerable<string>) registryPathPatterns);
      }
      this.UpdateRegisteredSecrets(requestContext, false, placedLookupKeysSet, false, secretsToUpdate);
      if (!deleteMissing)
        return;
      logger.Info("Deleting any strongbox items that were not set.");
      foreach (StrongBoxItemInfo drawerContent in service1.GetDrawerContents(requestContext, drawer))
      {
        if (!placedLookupKeysSet.Contains(drawerContent.LookupKey))
        {
          if (drawerContent.LookupKey.Equals("CouponCodeDecryptionCertificateThumbprint-previous", StringComparison.OrdinalIgnoreCase))
          {
            logger.Info("Skipping deleting '" + drawerContent.LookupKey + " from ConfigurationSecrets drawer");
          }
          else
          {
            logger.Info("Deleting unplaced lookupKey '" + drawerContent.LookupKey + "' from ConfigurationSecrets drawer.");
            service1.DeleteItem(requestContext, drawer, drawerContent.LookupKey);
          }
        }
      }
    }

    public void UpdateBootstrapSettings(IVssRequestContext requestContext, bool verboseTrace)
    {
      if (!this.m_useManagedIdentity)
      {
        AzureServicePrincipalProvider principalProvider = new AzureServicePrincipalProvider();
        CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add("RuntimeServicePrincipalCertThumbprint", principalProvider.RuntimeServicePrincipalCertificate.Thumbprint);
        intelligenceData.Add("RuntimeServicePrincipalCertStartDate", (object) principalProvider.RuntimeServicePrincipalCertificate.NotBefore);
        intelligenceData.Add("RuntimeServicePrincipalCertExpirationDate", (object) principalProvider.RuntimeServicePrincipalCertificate.NotAfter);
        IVssRequestContext requestContext1 = requestContext;
        string secret = CustomerIntelligenceArea.Secret;
        string hostedSecretService = CustomerIntelligenceFeature.HostedSecretService;
        CustomerIntelligenceData properties = intelligenceData;
        service.Publish(requestContext1, secret, hostedSecretService, properties);
      }
      Uri uriForLookupKey = this.FindUriForLookupKey(requestContext, HostedSecretService.s_secretRotationSharedAccessKeyToken);
      if (!this.UpdateSecret(requestContext, uriForLookupKey, verboseTrace))
        throw new ApplicationException(string.Format("StrongBox mapping for bootstrap Key Vault SecretIdentifier {0} not found. Bootstrap secrets can't be read from Key Vault and updated in StrongBox.", (object) uriForLookupKey));
    }

    public RegistryEntryCollection GetRegisteredSecretEntries(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      RegistryQuery registryQuery = new RegistryQuery(FrameworkServerConstants.StrongBoxSecretMappingRegistryRootPath + "/**");
      IVssRequestContext requestContext1 = requestContext;
      RegistryQuery query = registryQuery;
      return service.ReadEntries(requestContext1, query);
    }

    public bool TryGetRegisteredSecret(
      IVssRequestContext requestContext,
      string secretPath,
      out string strongBoxKeyName)
    {
      requestContext.CheckSystemRequestContext();
      requestContext.CheckDeploymentRequestContext();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      strongBoxKeyName = service.GetValue(requestContext, (RegistryQuery) secretPath, (string) null);
      return !string.IsNullOrEmpty(strongBoxKeyName);
    }

    public void UpdateServiceBusSubscriptionFilter(
      IVssRequestContext requestContext,
      ITFLogger logger)
    {
      requestContext.CheckDeploymentRequestContext();
      List<string> list = this.GetRegisteredSecretMappings(requestContext).Select<KeyValuePair<Uri, StrongBoxMapping>, string>((Func<KeyValuePair<Uri, StrongBoxMapping>, string>) (x => x.Key.Host.ToLower())).Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToList<string>();
      this.UpdateServiceBusSubscriptionFilter(requestContext, list, logger);
    }

    public void UpdateServiceBusSubscription(
      IVssRequestContext requestContext,
      MessageBusSubscriberSettings subscriberSettings,
      ITFLogger logger)
    {
      if (!this.IsSecretNamespaceRegistered(requestContext))
      {
        logger.Info("Secret rotation namespace has not been registered yet, doing nothing");
      }
      else
      {
        IMessageBusManagementService service = requestContext.GetService<IMessageBusManagementService>();
        string nameForScaleUnit = service.GetSubscriberNameForScaleUnit(requestContext, SecretRotationConstants.SecretRotationTopic);
        logger.Info("Updating settings for SB subscription " + nameForScaleUnit + ".");
        service.UpdateSubscribers(requestContext, subscriberSettings, SecretRotationConstants.SecretRotationTopic, nameForScaleUnit);
        logger.Info("Settings have been updated for Subscription " + nameForScaleUnit + ".");
      }
    }

    public static string GetMappingRegistryPath(Uri keyVaultSecretIdentifier)
    {
      string suffixString;
      string mappingRegistryPath = HostedSecretService.GetMappingRegistryPath(keyVaultSecretIdentifier, out suffixString);
      if (string.IsNullOrEmpty(suffixString))
        return mappingRegistryPath;
      throw new ArgumentException("secretIdentifier has an array index suffix. This is only supported by the longer overload of GetMappingRegistryPath.", nameof (keyVaultSecretIdentifier));
    }

    public static string GetMappingRegistryPath(
      Uri keyVaultSecretIdentifier,
      out string suffixString)
    {
      string str;
      SuffixedString.Parse(keyVaultSecretIdentifier.GetComponents(UriComponents.Host | UriComponents.Path, UriFormat.SafeUnescaped), ref str, ref suffixString);
      return FrameworkServerConstants.StrongBoxSecretMappingRegistryRootPath + "/" + str;
    }

    private bool UpdateStrongBox(
      IVssRequestContext requestContext,
      SecretObject obj,
      Guid drawerId,
      string lookupKey,
      bool alertOnStaleValues,
      bool verboseTrace)
    {
      HostedSecretService.TraceHandler traceHandler = !verboseTrace ? new HostedSecretService.TraceHandler(((VssRequestContextExtensions) requestContext).Trace) : new HostedSecretService.TraceHandler(((VssRequestContextExtensions) requestContext).TraceAlways);
      traceHandler(15117020, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: Checking if StrongBox is up-to-date with Key Vault.", new object[1]
      {
        (object) lookupKey
      });
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo info = new StrongBoxItemInfo()
      {
        DrawerId = drawerId,
        LookupKey = lookupKey
      };
      StrongBoxItemInfo previousSlotInfo = (StrongBoxItemInfo) null;
      StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, drawerId, lookupKey, false);
      X509Certificate2 existingCert = (X509Certificate2) null;
      string str = (string) null;
      if (itemInfo != null)
      {
        previousSlotInfo = new StrongBoxItemInfo()
        {
          DrawerId = drawerId,
          LookupKey = lookupKey + "-previous",
          CredentialName = itemInfo.CredentialName,
          ExpirationDate = itemInfo.ExpirationDate
        };
        if (itemInfo.ItemKind == StrongBoxItemKind.File)
        {
          traceHandler(15117030, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: Existing file entry found.", new object[1]
          {
            (object) lookupKey
          });
          existingCert = service.RetrieveFileAsCertificate(requestContext, drawerId, lookupKey, true);
        }
        else
        {
          traceHandler(15117031, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: Existing string entry found.", new object[1]
          {
            (object) lookupKey
          });
          str = service.GetString(requestContext, drawerId, lookupKey);
        }
      }
      else
        traceHandler(15117032, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: No current entry found in StrongBox.", new object[1]
        {
          (object) lookupKey
        });
      SecretCertificate secretCertificate = obj as SecretCertificate;
      SecretStorageAccountInfo storageAccountInfo = obj as SecretStorageAccountInfo;
      SecretCredential secretCredential = obj as SecretCredential;
      if (obj == null)
      {
        if (itemInfo == null)
        {
          traceHandler(15117040, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: Secret not present in Key Vault, but no value is present in StrongBox. Skipping update.", new object[1]
          {
            (object) lookupKey
          });
          return false;
        }
        this.BackupExistingItem(requestContext, service, previousSlotInfo, itemInfo, existingCert, str, lookupKey);
        requestContext.TraceAlways(15117041, TraceLevel.Warning, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: Secret deleted from Key Vault. Deleting current StrongBox value.", (object) lookupKey);
        service.DeleteItem(requestContext, drawerId, lookupKey);
        requestContext.TraceAlways(15117042, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: StrongBox updated. (DeleteItem)", (object) lookupKey);
      }
      else if (SecretCertificate.op_Inequality(secretCertificate, (SecretCertificate) null))
      {
        X509Certificate2 certificate = secretCertificate.ToCertificate();
        if (existingCert != null && string.Equals(certificate.Thumbprint, existingCert.Thumbprint, StringComparison.OrdinalIgnoreCase) && certificate.NotAfter == existingCert.NotAfter)
        {
          traceHandler(15117050, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: Certificate in StrongBox already matches the Key Vault certificate. Skipping update. Thumbprint: {1}", new object[2]
          {
            (object) lookupKey,
            (object) certificate.Thumbprint
          });
          return false;
        }
        if (itemInfo != null)
          this.BackupExistingItem(requestContext, service, previousSlotInfo, itemInfo, existingCert, str, lookupKey);
        info.ExpirationDate = new DateTime?(certificate.NotAfter);
        requestContext.TraceAlways(15117051, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: Updating certificate to match Key Vault. Thumbprint: {1}", (object) lookupKey, (object) certificate.Thumbprint);
        service.AddCertificate(requestContext, info, certificate);
        requestContext.TraceAlways(15117052, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: StrongBox updated. (AddCertificate)", (object) lookupKey);
      }
      else if (SecretStorageAccountInfo.op_Inequality(storageAccountInfo, (SecretStorageAccountInfo) null))
      {
        string a = storageAccountInfo.ToString();
        if (string.Equals(a, str, StringComparison.Ordinal) && string.Equals(storageAccountInfo.StorageAccountName, itemInfo.CredentialName, StringComparison.Ordinal))
        {
          traceHandler(15117060, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: Storage connection string and credential name in StrongBox already matches the Key Vault connection string and storage account name, respectively. Skipping update. Account name: {1}. UseSecondaryKey: {2}", new object[3]
          {
            (object) lookupKey,
            (object) storageAccountInfo.StorageAccountName,
            (object) storageAccountInfo.UseSecondaryKey
          });
          return false;
        }
        info.CredentialName = storageAccountInfo.StorageAccountName;
        if (itemInfo != null)
          this.BackupExistingItem(requestContext, service, previousSlotInfo, itemInfo, existingCert, str, lookupKey);
        requestContext.TraceAlways(15117061, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: Updating storage connection string to match Key Vault. Account name: {1}. UseSecondaryKey: {2}", (object) lookupKey, (object) storageAccountInfo.StorageAccountName, (object) storageAccountInfo.UseSecondaryKey);
        service.AddString(requestContext, info, a);
        requestContext.TraceAlways(15117062, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: StrongBox updated. (AddString)", (object) lookupKey);
      }
      else if (SecretCredential.op_Inequality(secretCredential, (SecretCredential) null))
      {
        if (string.Equals(secretCredential.Password, str, StringComparison.Ordinal) && string.Equals(secretCredential.Username, itemInfo.CredentialName, StringComparison.Ordinal))
        {
          traceHandler(15117070, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: Credential in StrongBox already matches the Key Vault credential. Skipping update. Username: {1}", new object[2]
          {
            (object) lookupKey,
            (object) secretCredential.Username
          });
          return false;
        }
        if (itemInfo != null)
          this.BackupExistingItem(requestContext, service, previousSlotInfo, itemInfo, existingCert, str, lookupKey);
        info.CredentialName = secretCredential.Username;
        requestContext.TraceAlways(15117071, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: Updating credential to match Key Vault. Username: {1}", (object) lookupKey, (object) secretCredential.Username);
        service.AddString(requestContext, info, secretCredential.Password);
        requestContext.TraceAlways(15117072, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: StrongBox updated. (AddString)", (object) lookupKey);
      }
      else
      {
        requestContext.Trace(15117080, TraceLevel.Error, HostedSecretService.s_area, HostedSecretService.s_layer, "Type " + obj.GetType().FullName + " is not a supported secret type.");
        throw new InvalidOperationException("Type " + obj.GetType().FullName + " is not a supported secret type.");
      }
      if (alertOnStaleValues)
      {
        if (obj != null && obj.LastUpdateTime.HasValue)
        {
          TimeSpan timeSpan = DateTime.UtcNow - obj.LastUpdateTime.Value;
          if (timeSpan > HostedSecretService.s_staleSecretAlertThreshold)
          {
            string message = string.Format("Secret {0} was out of date in StrongBox for {1} minutes, prior to update. The secret was last updated in Key Vault at {2}.", (object) lookupKey, (object) (int) timeSpan.TotalMinutes, (object) obj.LastUpdateTime.Value);
            requestContext.Trace(15117090, TraceLevel.Error, HostedSecretService.s_area, HostedSecretService.s_layer, message);
            TeamFoundationEventLog.Default.Log(requestContext, message, TeamFoundationEventId.StrongBoxStaleConfigurationSecretAlert, EventLogEntryType.Error);
          }
          else
            requestContext.TraceAlways(15117091, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: StrongBox value was out of date for {1} minutes prior to update, less than the threshold of {2} minutes. Skipping alert.", (object) lookupKey, (object) timeSpan.TotalMinutes, (object) HostedSecretService.s_staleSecretAlertThreshold.TotalMinutes);
        }
        else
          requestContext.TraceAlways(15117092, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: Age of Key Vault secret value unknown. Skipping alert.", (object) lookupKey);
      }
      return true;
    }

    private void BackupExistingItem(
      IVssRequestContext requestContext,
      ITeamFoundationStrongBoxService strongBox,
      StrongBoxItemInfo previousSlotInfo,
      StrongBoxItemInfo existingItemInfo,
      X509Certificate2 existingCert,
      string existingStringValue,
      string lookupKey)
    {
      if (existingItemInfo.ItemKind == StrongBoxItemKind.File)
      {
        requestContext.TraceAlways(15117100, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: Backing up existing certificate. Thumbprint: {1}", (object) lookupKey, (object) existingCert.Thumbprint);
        strongBox.AddCertificate(requestContext, previousSlotInfo, existingCert);
        requestContext.TraceAlways(15117101, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: StrongBox previous slot updated. (AddCertificate)", (object) lookupKey);
      }
      else
      {
        requestContext.TraceAlways(15117102, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: Backing up existing string.", (object) lookupKey);
        strongBox.AddString(requestContext, previousSlotInfo, existingStringValue);
        requestContext.TraceAlways(15117103, TraceLevel.Info, HostedSecretService.s_area, HostedSecretService.s_layer, (string[]) null, "{0}: StrongBox previous slot updated. (AddString)", (object) lookupKey);
      }
    }

    internal int UpdateRegisteredSecrets(
      IVssRequestContext requestContext,
      bool alertOnStaleValues,
      HashSet<string> placedLookupKeysSet,
      bool verboseTrace,
      List<Uri> secretsToUpdate = null)
    {
      Dictionary<Uri, StrongBoxMapping> registeredSecrets = this.GetRegisteredSecretMappings(requestContext);
      if (secretsToUpdate != null)
      {
        registeredSecrets = registeredSecrets.Where<KeyValuePair<Uri, StrongBoxMapping>>((Func<KeyValuePair<Uri, StrongBoxMapping>, bool>) (x => secretsToUpdate.Contains(x.Key))).ToDictionary<KeyValuePair<Uri, StrongBoxMapping>, Uri, StrongBoxMapping>((Func<KeyValuePair<Uri, StrongBoxMapping>, Uri>) (x => x.Key), (Func<KeyValuePair<Uri, StrongBoxMapping>, StrongBoxMapping>) (x => x.Value));
        List<string> list = secretsToUpdate.Where<Uri>((Func<Uri, bool>) (x => !registeredSecrets.Keys.Contains<Uri>(x))).Select<Uri, string>((Func<Uri, string>) (x => x.AbsoluteUri)).ToList<string>();
        if (list.Count != 0)
          throw new SecretProviderException("Attempted to update non-registered secrets: " + string.Join(",", list.ToArray()));
      }
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(requestContext, "ConfigurationSecrets", true);
      int num = 0;
      foreach (KeyValuePair<Uri, StrongBoxMapping> keyValuePair in registeredSecrets)
      {
        if (!requestContext.IsCanceled())
        {
          Uri key = keyValuePair.Key;
          StrongBoxMapping strongBoxMapping = keyValuePair.Value;
          if (strongBoxMapping.IsArray)
          {
            List<SecretObject> secrets = this.GetSecretProvider(requestContext).GetSecrets(key, strongBoxMapping.IsArray);
            for (int index = 0; index < secrets.Count; ++index)
            {
              SecretObject secretObject = secrets[index];
              string lookupKey = strongBoxMapping.LookupKey + index.ToString();
              placedLookupKeysSet.Add(lookupKey);
              if (this.UpdateStrongBox(requestContext, secretObject, drawerId, lookupKey, alertOnStaleValues, verboseTrace))
                ++num;
            }
            foreach (StrongBoxItemInfo drawerContent in service.GetDrawerContents(requestContext, drawerId))
            {
              int result = 0;
              if (drawerContent.LookupKey.StartsWith(strongBoxMapping.LookupKey, StringComparison.OrdinalIgnoreCase) && int.TryParse(drawerContent.LookupKey.Substring(strongBoxMapping.LookupKey.Length), out result) && result >= secrets.Count && this.UpdateStrongBox(requestContext, (SecretObject) null, drawerId, drawerContent.LookupKey, alertOnStaleValues, verboseTrace))
                ++num;
            }
          }
          else
          {
            SecretObject secret = this.GetSecretProvider(requestContext).GetSecret(key);
            placedLookupKeysSet.Add(strongBoxMapping.LookupKey);
            if (this.UpdateStrongBox(requestContext, secret, drawerId, strongBoxMapping.LookupKey, alertOnStaleValues, verboseTrace))
              ++num;
          }
        }
        else
          break;
      }
      return num;
    }

    internal void UpdateServiceBusSubscriptionFilter(
      IVssRequestContext requestContext,
      List<string> uniqueVaults,
      ITFLogger logger)
    {
      string subscriptionFilter = this.GetServiceBusSubscriptionFilter(uniqueVaults);
      string path = "/Service/MessageBus/ServiceBus/Management/Topics/" + SecretRotationConstants.SecretRotationTopic + "/SubscriptionFilter";
      requestContext.GetService<IVssRegistryService>().SetValue<string>(requestContext, path, subscriptionFilter);
      if (requestContext.ExecutionEnvironment.IsDevFabricDeployment)
        return;
      if (!this.IsSecretNamespaceRegistered(requestContext))
        logger.Info("Secret rotation namespace has not been registered yet, doing nothing");
      else if (!this.DoesSubscriberFilterNeedToBeUpdated(requestContext, subscriptionFilter))
      {
        logger.Info("Vault filter already matches, doing nothing");
      }
      else
      {
        IMessageBusManagementService service = requestContext.GetService<IMessageBusManagementService>();
        string nameForScaleUnit = service.GetSubscriberNameForScaleUnit(requestContext, SecretRotationConstants.SecretRotationTopic);
        logger.Info("Updating SB subscription " + nameForScaleUnit + " to " + subscriptionFilter);
        service.UpdateSubscriptionFilter(requestContext, SecretRotationConstants.SecretRotationTopic, nameForScaleUnit, subscriptionFilter, logger: logger);
        logger.Info("Filter is updated for Subscription " + nameForScaleUnit);
      }
    }

    internal string GetServiceBusSubscriptionFilter(List<string> allRegisteredVaults)
    {
      StringBuilder stringBuilder = new StringBuilder("NOT EXISTS(KeyVaultName)");
      foreach (string allRegisteredVault in allRegisteredVaults)
        stringBuilder.Append(" OR KeyVaultName = '" + allRegisteredVault.ToLower() + "'");
      return stringBuilder.ToString();
    }

    private bool DoesSubscriberFilterNeedToBeUpdated(
      IVssRequestContext requestContext,
      string desiredFilter)
    {
      IServiceBusManager service = requestContext.GetService<IServiceBusManager>();
      string nameForScaleUnit = requestContext.GetService<IMessageBusManagementService>().GetSubscriberNameForScaleUnit(requestContext, SecretRotationConstants.SecretRotationTopic);
      IVssRequestContext requestContext1 = requestContext;
      string secretRotationTopic = SecretRotationConstants.SecretRotationTopic;
      string subscriptionName = nameForScaleUnit;
      IEnumerable<RuleDescription> subscriptionRules = service.GetSubscriptionRules(requestContext1, secretRotationTopic, subscriptionName);
      return subscriptionRules == null || !subscriptionRules.Any<RuleDescription>((Func<RuleDescription, bool>) (r => r.Filter is SqlFilter filter && string.Equals(filter.SqlExpression, desiredFilter, StringComparison.Ordinal)));
    }

    private bool IsSecretNamespaceRegistered(IVssRequestContext requestContext) => !string.IsNullOrWhiteSpace(requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/Management/Topics/Microsoft.VisualStudio.Services.Secrets/Namespace", string.Empty));

    internal Dictionary<Uri, StrongBoxMapping> GetRegisteredSecretMappings(
      IVssRequestContext requestContext)
    {
      return this.GetRegisteredSecretEntries(requestContext).ToDictionary<RegistryEntry, Uri, StrongBoxMapping>((Func<RegistryEntry, Uri>) (e => this.GetKeyVaultSecretIdentifier(e)), (Func<RegistryEntry, StrongBoxMapping>) (e => this.GetMapping(e)));
    }

    private Uri GetKeyVaultSecretIdentifier(RegistryEntry entry)
    {
      string str = entry.Path.Substring(FrameworkServerConstants.StrongBoxSecretMappingRegistryRootPath.Length + 1);
      return new Uri(Uri.UriSchemeHttps + "://" + str);
    }

    private StrongBoxMapping GetMapping(RegistryEntry entry) => TeamFoundationSerializationUtility.Deserialize<StrongBoxMapping>(entry.Value);

    protected virtual ISecretProvider CreateSecretProvider() => (ISecretProvider) new SecretProvider(SecretProviderFactory.CreateStorageProvider(this.m_isDevFabric, new KeyVaultSecretStorageProvider(KeyVaultClientAdapterFactory.GetKeyVaultClientAdapter((ServiceClientCredentials) new DefaultKeyVaultCredentials(this.m_useManagedIdentity, this.m_logger), true, this.m_logger), this.m_logger), this.m_logger), this.m_logger, true);

    private ISecretProvider GetSecretProvider(IVssRequestContext requestContext) => this.m_secretProvider.Value;

    private Uri FindUriForLookupKey(IVssRequestContext requestContext, string lookupKey)
    {
      List<KeyValuePair<Uri, StrongBoxMapping>> list = this.GetRegisteredSecretMappings(requestContext).Where<KeyValuePair<Uri, StrongBoxMapping>>((Func<KeyValuePair<Uri, StrongBoxMapping>, bool>) (x => string.Equals(x.Value.LookupKey, lookupKey, StringComparison.OrdinalIgnoreCase))).ToList<KeyValuePair<Uri, StrongBoxMapping>>();
      if (list.Count > 1)
        throw new SecretProviderException("Secret " + lookupKey + " is registered multiple times");
      return list.Count != 0 ? list.Single<KeyValuePair<Uri, StrongBoxMapping>>().Key : throw new SecretProviderException("Secret " + lookupKey + " is not registered");
    }

    private delegate void TraceHandler(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string format,
      params object[] args);
  }
}
