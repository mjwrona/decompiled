// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.Server.SecuritySettingsService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Security.Server
{
  internal class SecuritySettingsService : IVssFrameworkService
  {
    private SecuritySettingsService.SecurityServiceSettings m_settings;
    private static readonly RegistryQuery s_query = (RegistryQuery) "/Service/Security/Settings/...";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), in SecuritySettingsService.s_query);
      Interlocked.CompareExchange<SecuritySettingsService.SecurityServiceSettings>(ref this.m_settings, new SecuritySettingsService.SecurityServiceSettings(requestContext), (SecuritySettingsService.SecurityServiceSettings) null);
    }

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write<SecuritySettingsService.SecurityServiceSettings>(ref this.m_settings, new SecuritySettingsService.SecurityServiceSettings(requestContext));
    }

    public SecuritySettingsService.SecurityServiceSettings Settings => this.m_settings;

    public class SecurityServiceSettings
    {
      public readonly int TokenDeltaPurgePeriod;
      public readonly int CacheLifetimeInMilliseconds;
      public readonly bool AllowDescriptorResponsesFromSecurityBackingStore;
      public readonly bool AllowDescriptorRequestsFromSecurityBackingStore;
      public readonly int MessageBusTaskDelayInSeconds;
      public readonly bool AllowPollForRequestLocalInvalidation;
      public readonly int InitialLoadGateSize;
      public readonly bool BackingStoreRespectsThrowOnInvalidIdentity;
      public readonly bool BypassIdentityServiceCache;
      public readonly int PermissionTracingSamplingDivisor;
      public readonly int NamespaceMetadataCacheLifetimeInMilliseconds;
      public readonly int LoadGateWaiterLimit;

      public SecurityServiceSettings(IVssRequestContext requestContext)
      {
        RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, SecuritySettingsService.s_query);
        this.TokenDeltaPurgePeriod = Math.Max(0, registryEntryCollection.GetValueFromPath<int>(nameof (TokenDeltaPurgePeriod), 7));
        this.CacheLifetimeInMilliseconds = Math.Max(0, registryEntryCollection.GetValueFromPath<int>(nameof (CacheLifetimeInMilliseconds), 1200000));
        this.AllowDescriptorResponsesFromSecurityBackingStore = registryEntryCollection.GetValueFromPath<bool>(nameof (AllowDescriptorResponsesFromSecurityBackingStore), true);
        this.AllowDescriptorRequestsFromSecurityBackingStore = registryEntryCollection.GetValueFromPath<bool>(nameof (AllowDescriptorRequestsFromSecurityBackingStore), true);
        this.MessageBusTaskDelayInSeconds = Math.Max(0, registryEntryCollection.GetValueFromPath<int>(nameof (MessageBusTaskDelayInSeconds), 5));
        this.AllowPollForRequestLocalInvalidation = registryEntryCollection.GetValueFromPath<bool>(nameof (AllowPollForRequestLocalInvalidation), true);
        this.InitialLoadGateSize = Math.Max(1, registryEntryCollection.GetValueFromPath<int>(nameof (InitialLoadGateSize), 3));
        this.BackingStoreRespectsThrowOnInvalidIdentity = registryEntryCollection.GetValueFromPath<bool>(nameof (BackingStoreRespectsThrowOnInvalidIdentity), false);
        this.BypassIdentityServiceCache = registryEntryCollection.GetValueFromPath<bool>(nameof (BypassIdentityServiceCache), false);
        this.PermissionTracingSamplingDivisor = Math.Max(0, registryEntryCollection.GetValueFromPath<int>(nameof (PermissionTracingSamplingDivisor), 8));
        this.NamespaceMetadataCacheLifetimeInMilliseconds = Math.Max(0, registryEntryCollection.GetValueFromPath<int>(nameof (NamespaceMetadataCacheLifetimeInMilliseconds), 7200000));
        this.LoadGateWaiterLimit = Math.Min(int.MaxValue, registryEntryCollection.GetValueFromPath<int>(nameof (LoadGateWaiterLimit), 500));
      }
    }
  }
}
