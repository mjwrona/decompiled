// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SasTokenRequestService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Azure.Storage;
using Azure.Storage.Sas;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Security;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class SasTokenRequestService : ISasTokenRequestService, IVssFrameworkService
  {
    private const string c_blob = "blob";
    private const string c_table = "table";
    internal const string c_area = "SasTokenRequest";
    internal const string c_layer = "Service";

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public string GetSasToken(
      IVssRequestContext requestContext,
      string resourceUri,
      SasRequestPermissions permissions,
      TimeSpan expiration)
    {
      return this.GetSasToken(requestContext, resourceUri, permissions, expiration, SasTokenVersion.V2019_07_07);
    }

    public string GetSasToken(
      IVssRequestContext requestContext,
      Uri resourceUri,
      SasRequestPermissions permissions,
      TimeSpan expiration)
    {
      return this.GetSasToken(requestContext, resourceUri, permissions, expiration, SasTokenVersion.V2019_07_07);
    }

    public string GetSasToken(
      IVssRequestContext requestContext,
      string resourceUri,
      SasRequestPermissions permissions,
      TimeSpan expiration,
      SasTokenVersion version)
    {
      return this.GetSasToken(requestContext, new Uri(resourceUri), permissions, expiration, version);
    }

    public string GetSasToken(
      IVssRequestContext requestContext,
      Uri resourceUri,
      SasRequestPermissions permissions,
      TimeSpan expiration,
      SasTokenVersion version)
    {
      ArgumentUtility.CheckForNull<Uri>(resourceUri, nameof (resourceUri));
      requestContext.TraceAlways(555555, TraceLevel.Info, "SasTokenRequest", "Service", "Sas token request for resource: {0}. Permissions: {1}, TimeSpan: {2}, from user: {3}", (object) resourceUri, (object) permissions.ToString(), (object) expiration, (object) requestContext.GetAuthenticatedDescriptor());
      SasTokenRequestUtils.CheckRequestPermission(requestContext, permissions);
      IVssRequestContext requestContext1 = requestContext.Elevate();
      string accountName = SasTokenRequestService.GetAccountName(resourceUri, out string _);
      string connectionString = this.GetConnectionString(requestContext1, accountName);
      if (connectionString == null)
      {
        requestContext.Trace(555557, TraceLevel.Error, "SasTokenRequest", "Service", "Storage account " + accountName + " not found in strongbox");
        throw new InvalidOperationException("Storage account " + accountName + " not found in strongBox.");
      }
      return this.GetEncryptedSasToken(requestContext1, connectionString, permissions, expiration, resourceUri, version: version);
    }

    internal string GetSasToken(
      IVssRequestContext requestContext,
      string connectionString,
      Guid containerId,
      SasRequestPermissions permissions,
      TimeSpan expiration)
    {
      return this.GetSasToken(requestContext, connectionString, containerId.ToString("n"), permissions, expiration);
    }

    internal string GetSasToken(
      IVssRequestContext requestContext,
      string connectionString,
      string containerName,
      SasRequestPermissions permissions,
      TimeSpan expiration,
      SasTokenVersion version = SasTokenVersion.V2019_07_07)
    {
      ArgumentUtility.CheckForNull<string>(connectionString, nameof (connectionString));
      requestContext.TraceAlways(555555, TraceLevel.Info, "SasTokenRequest", "Service", "Sas token request for connectionString: {0} and containerName: {1}. Permissions: {2}, TimeSpan: {3}, from user: {4}", (object) SecretUtility.ScrubSecrets(connectionString, false), (object) containerName, (object) permissions.ToString(), (object) expiration, (object) requestContext.GetAuthenticatedDescriptor());
      SasTokenRequestUtils.CheckRequestPermission(requestContext, permissions);
      return this.GetEncryptedSasToken(requestContext.Elevate(), connectionString, permissions, expiration, containerName: containerName, version: version);
    }

    internal string GetAccountSasToken(
      IVssRequestContext requestContext,
      string connectionString,
      SasRequestPermissions permissions,
      TimeSpan expiration,
      SasRequestServices requestedAccountServices)
    {
      return this.GetAccountSasToken(requestContext, connectionString, permissions, expiration, requestedAccountServices, SasTokenVersion.V2019_07_07);
    }

    internal string GetAccountSasToken(
      IVssRequestContext requestContext,
      string connectionString,
      SasRequestPermissions permissions,
      TimeSpan expiration,
      SasRequestServices requestedAccountServices,
      SasTokenVersion version)
    {
      ArgumentUtility.CheckForNull<string>(connectionString, nameof (connectionString));
      requestContext.TraceAlways(555558, TraceLevel.Info, "SasTokenRequest", "Service", "Sas token request for connectionString: {0}. Permissions: {1}, RequestedServices: {2}, TimeSpan: {3}, from user: {4}", (object) SecretUtility.ScrubSecrets(connectionString, false), (object) permissions.ToString(), (object) requestedAccountServices, (object) expiration, (object) requestContext.GetAuthenticatedDescriptor());
      SasTokenRequestUtils.CheckRequestPermission(requestContext, permissions);
      return this.GetEncryptedAccountSasToken(requestContext.Elevate(), connectionString, permissions, expiration, requestedAccountServices, version);
    }

    public string EncryptToken(IVssRequestContext requestContext, string sasToken)
    {
      using (SecureString secureString = StringUtil.CreateSecureString(sasToken))
        return requestContext.GetService<IS2SEncryptionService>().Encrypt(requestContext, secureString);
    }

    public string DecryptToken(IVssRequestContext requestContext, string encryptedSasToken)
    {
      using (SecureString sec = requestContext.GetService<IS2SEncryptionService>().Decrypt(requestContext, encryptedSasToken))
        return StringUtil.GetSecureStringContent(sec);
    }

    private static string GetAccountName(Uri resourceUri, out string storageType)
    {
      string[] strArray = resourceUri.Host.Split(new char[1]
      {
        '.'
      }, StringSplitOptions.None);
      string var = strArray[0];
      storageType = strArray[1];
      ArgumentUtility.CheckForNull<string>(var, "Storage Account");
      if (!storageType.Equals("blob", StringComparison.OrdinalIgnoreCase) && !storageType.Equals("table", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(string.Format("Resource Uri {0} is not an expected format.  Expected either a blob or table storage uri.", (object) resourceUri));
      return strArray[0];
    }

    internal string GetConnectionString(IVssRequestContext requestContext, string accountName)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId1 = service.UnlockDrawer(requestContext, FrameworkServerConstants.ConfigurationSecretsDrawerName, true);
      List<StrongBoxItemInfo> drawerContents = service.GetDrawerContents(requestContext, drawerId1);
      string connectionString1 = (string) null;
      foreach (StrongBoxItemInfo strongBoxItemInfo in drawerContents)
      {
        if (string.Equals(strongBoxItemInfo.CredentialName, accountName, StringComparison.OrdinalIgnoreCase))
        {
          connectionString1 = service.GetString(requestContext, strongBoxItemInfo);
          break;
        }
      }
      if (connectionString1 == null)
      {
        Guid drawerId2 = service.UnlockDrawer(requestContext, FrameworkServerConstants.StorageConnectionsDrawerName, false);
        if (drawerId2 != Guid.Empty)
        {
          foreach (StrongBoxItemInfo drawerContent in service.GetDrawerContents(requestContext, drawerId2))
          {
            string connectionString2 = service.GetString(requestContext, drawerContent);
            string storageAccountName = this.ParseStorageAccountName(requestContext, connectionString2);
            if (storageAccountName != null && string.Equals(storageAccountName, accountName, StringComparison.OrdinalIgnoreCase))
            {
              connectionString1 = connectionString2;
              break;
            }
          }
        }
      }
      return connectionString1;
    }

    private string ParseStorageAccountName(
      IVssRequestContext requestContext,
      string connectionString)
    {
      CloudStorageAccount account;
      if (!CloudStorageAccount.TryParse(connectionString, out account))
        return (string) null;
      if (account.Credentials.IsAnonymous)
      {
        requestContext.Trace(789456, TraceLevel.Info, "SasTokenRequest", "Service", "Anonymous storage connection strings not supported for generating SAS tokens. Skipping the provided connection string for account: {0}", (object) account.Credentials.AccountName);
        return (string) null;
      }
      if (!account.Credentials.IsSAS)
        return account.Credentials.AccountName;
      requestContext.Trace(789457, TraceLevel.Info, "SasTokenRequest", "Service", "SAS storage connection strings not supported for generating SAS tokens. Skipping the provided connection string for account: {0}", (object) account.Credentials.AccountName);
      return (string) null;
    }

    private string GetEncryptedSasToken(
      IVssRequestContext requestContext,
      string connectionString,
      SasRequestPermissions permissions,
      TimeSpan expiration,
      Uri resourceUri = null,
      string containerName = null,
      SasTokenVersion version = SasTokenVersion.V2019_07_07)
    {
      CloudStorageAccount account;
      if (!CloudStorageAccount.TryParse(connectionString, out account))
        throw new InvalidOperationException("Error creating a CloudStorageAccount with the provided connection string.");
      if (resourceUri == (Uri) null)
      {
        resourceUri = account.BlobEndpoint;
        if (!string.IsNullOrEmpty(containerName))
          resourceUri = new UriBuilder(resourceUri)
          {
            Path = containerName
          }.Uri;
      }
      string storageType;
      SasTokenRequestService.GetAccountName(resourceUri, out storageType);
      string sasToken;
      switch (storageType.ToLowerInvariant())
      {
        case "blob":
          sasToken = this.GetBlobSasToken(requestContext, account, resourceUri, permissions, expiration, version);
          break;
        case "table":
          sasToken = this.GetAccountSasToken(requestContext, account, permissions, expiration, SasRequestServices.Table, version);
          break;
        default:
          throw new InvalidOperationException("Unexpected storage type for SAS token request: " + storageType);
      }
      return this.EncryptToken(requestContext, sasToken);
    }

    private string GetEncryptedAccountSasToken(
      IVssRequestContext requestContext,
      string connectionString,
      SasRequestPermissions permissions,
      TimeSpan expiration,
      SasRequestServices requestedAccountServices,
      SasTokenVersion version)
    {
      CloudStorageAccount account;
      if (!CloudStorageAccount.TryParse(connectionString, out account))
        throw new InvalidOperationException("Error creating a CloudStorageAccount with the provided connection string.");
      string accountSasToken = this.GetAccountSasToken(requestContext, account, permissions, expiration, requestedAccountServices, version);
      return this.EncryptToken(requestContext, accountSasToken);
    }

    private string GetAccountSasToken(
      IVssRequestContext elevatedContext,
      CloudStorageAccount account,
      SasRequestPermissions permissions,
      TimeSpan expiration,
      SasRequestServices requestedAccountServices,
      SasTokenVersion version)
    {
      if (version != SasTokenVersion.V2019_07_07)
      {
        if (version != SasTokenVersion.V2020_08_04)
          throw new InvalidEnumArgumentException(nameof (version), (int) version, typeof (SasTokenVersion));
        string accountName = account.Credentials.AccountName;
        string base64String = Convert.ToBase64String(account.Credentials.ExportKey());
        return "?" + new AccountSasBuilder(SasTokenRequestUtils.ToAccountSasPermissions(permissions), DateTimeOffset.UtcNow.Add(expiration), SasTokenRequestUtils.ToAccountSasServices(requestedAccountServices), (AccountSasResourceTypes) -1)
        {
          Protocol = (this.IsDevfabConnectionString(account.ToString()) ? (SasProtocol) 1 : (SasProtocol) 2),
          StartsOn = ((DateTimeOffset) (DateTime.UtcNow - TimeSpan.FromMinutes(60.0)))
        }.ToSasQueryParameters(new StorageSharedKeyCredential(accountName, base64String)).ToString();
      }
      return account.GetSharedAccessSignature(new SharedAccessAccountPolicy()
      {
        Permissions = SasTokenRequestUtils.ToSharedAccessAccountPermissions(permissions),
        ResourceTypes = SharedAccessAccountResourceTypes.Service | SharedAccessAccountResourceTypes.Container | SharedAccessAccountResourceTypes.Object,
        Protocols = new SharedAccessProtocol?(this.IsDevfabConnectionString(account.ToString()) ? SharedAccessProtocol.HttpsOrHttp : SharedAccessProtocol.HttpsOnly),
        Services = SasTokenRequestUtils.ToSharedAccessAccountServices(requestedAccountServices),
        SharedAccessStartTime = new DateTimeOffset?((DateTimeOffset) (DateTime.UtcNow - TimeSpan.FromMinutes(60.0))),
        SharedAccessExpiryTime = new DateTimeOffset?((DateTimeOffset) (DateTime.UtcNow + expiration))
      });
    }

    private string GetBlobSasToken(
      IVssRequestContext requestContext,
      CloudStorageAccount account,
      Uri resourceUri,
      SasRequestPermissions permissions,
      TimeSpan expiration,
      SasTokenVersion version)
    {
      string accountName = account.Credentials.AccountName;
      string base64String = Convert.ToBase64String(account.Credentials.ExportKey());
      if (Uri.Compare(account.BlobStorageUri.PrimaryUri, resourceUri, UriComponents.AbsoluteUri, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0)
        return this.GetAccountSasToken(requestContext, account, permissions, expiration, SasRequestServices.Blob, version);
      CloudBlobContainer cloudBlobContainer = new CloudBlobContainer(new StorageUri(resourceUri), account.Credentials);
      if (version != SasTokenVersion.V2019_07_07)
      {
        if (version != SasTokenVersion.V2020_08_04)
          throw new InvalidEnumArgumentException(nameof (version), (int) version, typeof (SasTokenVersion));
        return "?" + new BlobSasBuilder(SasTokenRequestUtils.ToBlobContainerSasPermissions(permissions), DateTimeOffset.UtcNow.Add(expiration))
        {
          BlobContainerName = cloudBlobContainer.Name,
          Protocol = (this.IsDevfabConnectionString(account.ToString()) ? (SasProtocol) 1 : (SasProtocol) 2),
          StartsOn = ((DateTimeOffset) (DateTime.UtcNow - TimeSpan.FromMinutes(60.0)))
        }.ToSasQueryParameters(new StorageSharedKeyCredential(accountName, base64String)).ToString();
      }
      return cloudBlobContainer.GetSharedAccessSignature(new SharedAccessBlobPolicy()
      {
        Permissions = SasTokenRequestUtils.ToSharedAccessBlobPermissions(permissions),
        SharedAccessStartTime = new DateTimeOffset?((DateTimeOffset) (DateTime.UtcNow - TimeSpan.FromMinutes(60.0))),
        SharedAccessExpiryTime = new DateTimeOffset?((DateTimeOffset) (DateTime.UtcNow + expiration))
      });
    }

    private bool IsDevfabConnectionString(string connectionString) => string.Equals(connectionString, FrameworkServerConstants.DevStorageConnectionString, StringComparison.OrdinalIgnoreCase);
  }
}
