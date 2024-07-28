// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Telemetry.GalleryTelemetryHelperService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Telemetry
{
  internal class GalleryTelemetryHelperService : IGalleryTelemetryHelperService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void PublishAppInsightsTelemetry(
      IVssRequestContext requestContext,
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
        string str = GalleryTelemetryConstants.GalleryTelemetryRegistryRoot + action;
        if (!service.GetValue<bool>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.CollectTeamFoundationServerSqmData, false))
          return;
        int num = service.GetValue<int>(vssRequestContext, (RegistryQuery) str, 0) + 1;
        registryEntryList.Add(new RegistryEntry(str, num.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        service.WriteEntries(vssRequestContext, (IEnumerable<RegistryEntry>) registryEntryList);
      }
      catch (Exception ex)
      {
      }
    }

    public void PublishAppInsightsPerExtensionTelemetryIncrement(
      IVssRequestContext requestContext,
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
        string str = GalleryTelemetryConstants.GalleryTelemetryRegistryRoot + action;
        if (!service.GetValue<bool>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.CollectTeamFoundationServerSqmData, false))
          return;
        int num = service.GetValue<int>(vssRequestContext, (RegistryQuery) str, 0) + 1;
        registryEntryList.Add(new RegistryEntry(str, num.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        service.WriteEntries(vssRequestContext, (IEnumerable<RegistryEntry>) registryEntryList);
      }
      catch (Exception ex)
      {
      }
    }

    public void PublishAppInsightsPerExtensionTelemetryUpdate(
      IVssRequestContext requestContext,
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
        string registryPath = GalleryTelemetryConstants.GalleryTelemetryRegistryRoot + action;
        if (!service.GetValue<bool>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.CollectTeamFoundationServerSqmData, false))
          return;
        registryEntryList.Add(new RegistryEntry(registryPath, val.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        service.WriteEntries(vssRequestContext, (IEnumerable<RegistryEntry>) registryEntryList);
      }
      catch (Exception ex)
      {
      }
    }

    public void PublishAppInsightsPerExtensionTelemetryHelper(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      string action)
    {
      try
      {
        string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName);
        action = fullyQualifiedName + "/" + action;
        this.PublishAppInsightsPerExtensionTelemetryIncrement(requestContext, action, false);
        action = fullyQualifiedName + "/IsMarketplaceExtension";
        this.PublishAppInsightsPerExtensionTelemetryUpdate(requestContext, publishedExtension.IsMarketExtension(), action, false);
        action = fullyQualifiedName + "/IsPaid";
        this.PublishAppInsightsPerExtensionTelemetryUpdate(requestContext, publishedExtension.IsPaid(), action, false);
      }
      catch (Exception ex)
      {
      }
    }
  }
}
