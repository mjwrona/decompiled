// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.DevFabricRelay.DevFabricRelayService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud.DevFabricRelay
{
  internal class DevFabricRelayService : IDevFabricRelayService, IVssFrameworkService
  {
    private DevFabricRelaySettings m_settings;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      if (!systemRequestContext.ExecutionEnvironment.IsDevFabricDeployment)
      {
        this.m_settings = new DevFabricRelaySettings(new bool?(false));
      }
      else
      {
        systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), in DevFabricRelaySettings.RegistryQuery);
        Interlocked.CompareExchange<DevFabricRelaySettings>(ref this.m_settings, DevFabricRelaySettings.FromRegistry(systemRequestContext), (DevFabricRelaySettings) null);
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));

    private void OnRegistrySettingsChanged(
      IVssRequestContext systemRequestContext,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write<DevFabricRelaySettings>(ref this.m_settings, DevFabricRelaySettings.FromRegistry(systemRequestContext));
    }

    public string GetBlobConnectionString(IVssRequestContext requestContext) => this.GetStrongboxSecret(requestContext, "DevFabricRelayBlobConnectionString");

    public string GetConnectionString(IVssRequestContext requestContext) => this.GetStrongboxSecret(requestContext, "DevFabricRelayConnectionString");

    public string GetNoAuthnHybridConnectionString(IVssRequestContext requestContext) => this.GetStrongboxSecret(requestContext, "NoAuthnHybridConnectionStringStrongBoxKey");

    private string GetStrongboxSecret(IVssRequestContext requestContext, string lookupKey)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(requestContext, "AzureRelaySecrets", true);
      return service.GetString(requestContext, drawerId, lookupKey);
    }

    public DevFabricRelayInstallData GetWindowsInstallData(IVssRequestContext requestContext)
    {
      string str = "powershell.exe -ExecutionPolicy Unrestricted -c \"" + ("./" + this.Settings.WindowsInstallScriptName + " -DropPath " + this.Settings.WindowsPackageName + " -ConnectionString '" + EscapeForSingle(this.GetConnectionString(requestContext)) + "'") + "\"";
      return new DevFabricRelayInstallData()
      {
        InstallCommand = str,
        InstallScriptUri = this.GetSasUri(requestContext, this.Settings.WindowsInstallScriptName),
        PackageUri = this.GetSasUri(requestContext, this.Settings.WindowsPackageName)
      };

      static string EscapeForSingle(string value) => value.Replace("'", "''");
    }

    public DevFabricRelayInstallData GetLinuxInstallData(IVssRequestContext requestContext)
    {
      string str = EscapeForSingle(this.GetConnectionString(requestContext));
      return new DevFabricRelayInstallData()
      {
        InstallCommand = "./" + this.Settings.LinuxInstallScriptName + " ./" + this.Settings.LinuxPackageName + " '" + str + "'",
        InstallScriptUri = this.GetSasUri(requestContext, this.Settings.LinuxInstallScriptName),
        PackageUri = this.GetSasUri(requestContext, this.Settings.LinuxPackageName)
      };

      static string EscapeForSingle(string value) => value.Replace("'", "'\"'\"'");
    }

    internal virtual string GetSasUri(IVssRequestContext requestContext, string fileName)
    {
      CloudBlockBlob blockBlobReference = CloudStorageAccount.Parse(this.GetBlobConnectionString(requestContext)).CreateCloudBlobClient().GetContainerReference("devfabricrelay").GetBlockBlobReference(fileName);
      string sharedAccessSignature = blockBlobReference.GetSharedAccessSignature(new SharedAccessBlobPolicy()
      {
        SharedAccessExpiryTime = new DateTimeOffset?((DateTimeOffset) DateTime.UtcNow.Add(TimeSpan.FromHours(1.0))),
        Permissions = SharedAccessBlobPermissions.Read
      });
      return blockBlobReference.Uri?.ToString() + sharedAccessSignature;
    }

    public DevFabricRelaySettings Settings => this.m_settings;
  }
}
