// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Statistic.VSCodeWebExtensionService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Statistic
{
  internal class VSCodeWebExtensionService : IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void UpdateVSCodeWebExtensionStatistics(
      IVssRequestContext requestContext,
      string itemName,
      string version,
      VSCodeWebExtensionStatisicsType statType)
    {
      IPublishedExtensionService service1 = requestContext.GetService<IPublishedExtensionService>();
      IExtensionStatisticService service2 = requestContext.GetService<IExtensionStatisticService>();
      string[] strArray = itemName.Split(new char[1]{ '.' }, 2);
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      string publisherName = strArray.Length == 2 ? strArray[0] : throw new ArgumentException(GalleryResources.InvalidItemName((object) itemName));
      string extensionName = strArray[1];
      ExtensionQueryFlags flags = ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeLatestVersionOnly;
      PublishedExtension publishedExtension = service1.QueryExtension(requestContext, publisherName, extensionName, version, flags, (string) null, true);
      if (!publishedExtension.IsVsCodeExtension())
      {
        this.LogVSCodeWebExtensionTelemetry(requestContext, itemName, version, statType, "failure", "request for non-vscode extension");
        throw new NotSupportedException(GalleryResources.NotSupportedForNonVSCodeExtensions());
      }
      bool isBatchingEnabled = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.Publisher360.EnableStatBatchLogging");
      switch (statType)
      {
        case VSCodeWebExtensionStatisicsType.Install:
          if (service2.ShouldUpdateStatCount(requestContext, publishedExtension.ExtensionId, version, "install"))
          {
            service2.IncrementStatCount(requestContext, publishedExtension, "install");
            DailyStatsHelper.IncrementInstallCount(requestContext, publishedExtension, isBatchingEnabled);
            break;
          }
          break;
        case VSCodeWebExtensionStatisicsType.Update:
          if (service2.ShouldUpdateStatCount(requestContext, publishedExtension.ExtensionId, version, "update"))
          {
            service2.IncrementStatCount(requestContext, publishedExtension, "updateCount");
            break;
          }
          break;
        case VSCodeWebExtensionStatisicsType.Uninstall:
          if (service2.ShouldUpdateStatCount(requestContext, publishedExtension.ExtensionId, version, "uninstall"))
          {
            DailyStatsHelper.IncrementUnInstallCount(requestContext, publishedExtension, isBatchingEnabled);
            break;
          }
          break;
        default:
          throw new NotSupportedException(GalleryResources.InvalidVSCodeStatType());
      }
      this.LogVSCodeWebExtensionTelemetry(requestContext, itemName, version, statType, "success", "");
    }

    public void LogVSCodeWebExtensionTelemetry(
      IVssRequestContext requestContext,
      string itemName,
      string version,
      VSCodeWebExtensionStatisicsType statType,
      string status,
      string errorReason)
    {
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add(nameof (itemName), itemName);
      intelligenceData.Add(nameof (version), version);
      intelligenceData.Add(nameof (statType), (object) statType);
      intelligenceData.Add(nameof (status), status);
      intelligenceData.Add(nameof (errorReason), errorReason);
      IVssRequestContext requestContext1 = requestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext1, "Microsoft.VisualStudio.Services.Gallery", "vscodeweb", properties);
    }
  }
}
