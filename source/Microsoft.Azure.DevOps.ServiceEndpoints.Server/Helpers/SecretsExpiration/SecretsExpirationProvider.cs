// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.Helpers.SecretsExpiration.SecretsExpirationProvider
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.Helpers.SecretsExpiration
{
  internal sealed class SecretsExpirationProvider : ISecretsExpirationProvider, IVssFrameworkService
  {
    internal const string RegistryKey = "/Service/tfs/ServiceEndpoints/ARM/SecretExpirationInDays";
    internal const int DefaultExpirationInDays = 90;
    private readonly RegistryQuery _registryPath = new RegistryQuery("/Service/tfs/ServiceEndpoints/ARM/SecretExpirationInDays");
    private readonly IVssRegistryService _registryService;
    private int _expirationInDays;

    public SecretsExpirationProvider(IVssRegistryService registryService) => this._registryService = registryService;

    public DateTime GetSecretEndDate() => DateTime.UtcNow.AddDays((double) this._expirationInDays);

    public void ServiceStart(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      this._registryService.RegisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), in this._registryPath);
      Interlocked.CompareExchange(ref this._expirationInDays, this.ReadExpirationInDaysFromRegistry(vssRequestContext), 0);
    }

    public void ServiceEnd(IVssRequestContext requestContext) => this._registryService.UnregisterNotification(requestContext.To(TeamFoundationHostType.Deployment), new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write(ref this._expirationInDays, this.ReadExpirationInDaysFromRegistry(requestContext.To(TeamFoundationHostType.Deployment)));
    }

    private int ReadExpirationInDaysFromRegistry(IVssRequestContext deploymentContext) => this._registryService.GetValue<int>(deploymentContext, in this._registryPath, 90);
  }
}
