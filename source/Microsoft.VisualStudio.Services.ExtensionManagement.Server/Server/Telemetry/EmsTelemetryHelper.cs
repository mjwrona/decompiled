// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.Telemetry.EmsTelemetryHelper
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server.Telemetry
{
  internal static class EmsTelemetryHelper
  {
    public static void PublishAppInsightsTelemetry(
      this IVssRequestContext requestContext,
      string action,
      bool isPii = true)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment | isPii)
        return;
      if (requestContext.IsServicingContext)
        return;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
        List<RegistryEntry> registryEntryList = new List<RegistryEntry>();
        action = "ExtensionsPerDay/" + action;
        string str = EmsTelemetryConstants.EmsTelemetryRegistryRoot + action;
        if (!service.GetValue<bool>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.CollectTeamFoundationServerSqmData, false))
          return;
        int num = service.GetValue<int>(vssRequestContext, (RegistryQuery) str, 0) + 1;
        registryEntryList.Add(new RegistryEntry(str, num.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        using (requestContext.AcquireWriterLock(requestContext.ServiceHost.CreateLockName(str)))
          service.WriteEntries(vssRequestContext, (IEnumerable<RegistryEntry>) registryEntryList);
      }
      catch (Exception ex)
      {
      }
    }

    public static void PublishAppInsightsPerExtensionTelemetryIncrement(
      this IVssRequestContext requestContext,
      string action,
      bool isPii = true)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment | isPii)
        return;
      if (requestContext.IsServicingContext)
        return;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
        List<RegistryEntry> registryEntryList = new List<RegistryEntry>();
        string str = EmsTelemetryConstants.EmsTelemetryRegistryRoot + action;
        if (!service.GetValue<bool>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.CollectTeamFoundationServerSqmData, false))
          return;
        int num = service.GetValue<int>(vssRequestContext, (RegistryQuery) str, 0) + 1;
        registryEntryList.Add(new RegistryEntry(str, num.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        using (requestContext.AcquireWriterLock(requestContext.ServiceHost.CreateLockName(str)))
          service.WriteEntries(vssRequestContext, (IEnumerable<RegistryEntry>) registryEntryList);
      }
      catch (Exception ex)
      {
      }
    }

    public static void PublishAppInsightsPerExtensionTelemetryUpdate(
      this IVssRequestContext requestContext,
      bool val,
      string action,
      bool isPii = true)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment | isPii)
        return;
      if (requestContext.IsServicingContext)
        return;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
        List<RegistryEntry> registryEntryList = new List<RegistryEntry>();
        string str = EmsTelemetryConstants.EmsTelemetryRegistryRoot + action;
        if (!service.GetValue<bool>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.CollectTeamFoundationServerSqmData, false))
          return;
        registryEntryList.Add(new RegistryEntry(str, val.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        using (requestContext.AcquireWriterLock(requestContext.ServiceHost.CreateLockName(str)))
          service.WriteEntries(vssRequestContext, (IEnumerable<RegistryEntry>) registryEntryList);
      }
      catch (Exception ex)
      {
      }
    }

    public static void PublishAppInsightsPerExtensionTelemetryHelper(
      this IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      string action)
    {
      try
      {
        string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName);
        action = fullyQualifiedName + "/" + action;
        requestContext.PublishAppInsightsPerExtensionTelemetryIncrement(action, false);
        action = fullyQualifiedName + "/" + CustomerIntelligenceActions.ExtensionProperties.IsMarketplaceExtension;
        requestContext.PublishAppInsightsPerExtensionTelemetryUpdate(publishedExtension.IsMarketExtension(), action, false);
        action = fullyQualifiedName + "/" + CustomerIntelligenceActions.ExtensionProperties.IsPaidExtension;
        requestContext.PublishAppInsightsPerExtensionTelemetryUpdate(publishedExtension.IsPaid(), action, false);
      }
      catch (Exception ex)
      {
      }
    }
  }
}
