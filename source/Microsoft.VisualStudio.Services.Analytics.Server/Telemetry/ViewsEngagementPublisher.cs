// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Telemetry.ViewsEngagementPublisher
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System;
using System.Collections.Specialized;

namespace Microsoft.VisualStudio.Services.Analytics.Telemetry
{
  public class ViewsEngagementPublisher
  {
    private const string Feature = "Views";

    public static void PublishGetViewsEvent(IVssRequestContext requestContext)
    {
      CustomerIntelligenceData baseProperties = EngagementTelemetryPublisherBase.GetBaseProperties(requestContext);
      EngagementTelemetryPublisherBase.Publish(requestContext, "Views", "GetViews", baseProperties);
    }

    public static void PublishGetViewEvent(
      IVssRequestContext requestContext,
      Guid viewId,
      AnalyticsViewExpandFlags expand)
    {
      CustomerIntelligenceData baseProperties = EngagementTelemetryPublisherBase.GetBaseProperties(requestContext);
      baseProperties.Add("ViewId", viewId.ToString());
      baseProperties.Add("Expand", Enum.GetName(expand.GetType(), (object) expand));
      EngagementTelemetryPublisherBase.Publish(requestContext, "Views", "GetView", baseProperties);
    }

    public static void PublishCreateViewEvent(IVssRequestContext requestContext, bool preview)
    {
      CustomerIntelligenceData baseProperties = EngagementTelemetryPublisherBase.GetBaseProperties(requestContext);
      baseProperties.Add("Preview", preview);
      EngagementTelemetryPublisherBase.Publish(requestContext, "Views", "CreateView", baseProperties);
    }

    public static void PublishReplaceViewEvent(IVssRequestContext requestContext, Guid viewId)
    {
      CustomerIntelligenceData baseProperties = EngagementTelemetryPublisherBase.GetBaseProperties(requestContext);
      baseProperties.Add("ViewId", (object) viewId);
      EngagementTelemetryPublisherBase.Publish(requestContext, "Views", "ReplaceView", baseProperties);
    }

    public static void PublishDeleteViewEvent(IVssRequestContext requestContext, Guid viewId)
    {
      CustomerIntelligenceData baseProperties = EngagementTelemetryPublisherBase.GetBaseProperties(requestContext);
      baseProperties.Add("ViewId", (object) viewId);
      EngagementTelemetryPublisherBase.Publish(requestContext, "Views", "DeleteView", baseProperties);
    }

    public static void PublishLoadViewEvent(
      IVssRequestContext requestContext,
      NameValueCollection parameters)
    {
      CustomerIntelligenceData baseProperties = EngagementTelemetryPublisherBase.GetBaseProperties(requestContext);
      bool result1 = false;
      bool.TryParse(parameters["IsVerification"], out result1);
      bool result2 = false;
      bool.TryParse(parameters["IsDefaultView"], out result2);
      Guid result3 = Guid.Empty;
      Guid.TryParse(parameters["ViewId"], out result3);
      baseProperties.Add("ViewId", (object) result3);
      baseProperties.Add("IsVerification", result1);
      baseProperties.Add("IsDefaultView", result2);
      EngagementTelemetryPublisherBase.Publish(requestContext, "Views", "LoadView", baseProperties);
    }
  }
}
