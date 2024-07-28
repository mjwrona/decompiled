// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Telemetry.IGalleryTelemetryHelperService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Telemetry
{
  [DefaultServiceImplementation(typeof (GalleryTelemetryHelperService))]
  internal interface IGalleryTelemetryHelperService : IVssFrameworkService
  {
    void PublishAppInsightsTelemetry(IVssRequestContext requestContext, string action, bool isPii = true);

    void PublishAppInsightsPerExtensionTelemetryIncrement(
      IVssRequestContext requestContext,
      string action,
      bool isPii = true);

    void PublishAppInsightsPerExtensionTelemetryUpdate(
      IVssRequestContext requestContext,
      bool val,
      string action,
      bool isPii = true);

    void PublishAppInsightsPerExtensionTelemetryHelper(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      string action);
  }
}
