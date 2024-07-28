// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Telemetry.ODataEngagementPublisher
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;

namespace Microsoft.VisualStudio.Services.Analytics.Telemetry
{
  internal class ODataEngagementPublisher
  {
    private const string Feature = "OData";

    public static void PublishInProductExperienceQueryEvent(
      IVssRequestContext requestContext,
      string entityTypeName)
    {
      CustomerIntelligenceData baseProperties = EngagementTelemetryPublisherBase.GetBaseProperties(requestContext);
      baseProperties.Add("EntityType", entityTypeName);
      EngagementTelemetryPublisherBase.Publish(requestContext, "OData", "InProductOData", baseProperties);
    }

    public static void PublishMashupQueryEvent(
      IVssRequestContext requestContext,
      string entityTypeName,
      MashupFlavor flavor)
    {
      CustomerIntelligenceData baseProperties = EngagementTelemetryPublisherBase.GetBaseProperties(requestContext);
      baseProperties.Add("EntityType", entityTypeName);
      baseProperties.Add("MashupFlavor", (object) flavor);
      EngagementTelemetryPublisherBase.Publish(requestContext, "OData", "PowerBIQuery", baseProperties);
    }

    public static void PublishODataQueryEvent(
      IVssRequestContext requestContext,
      string entityTypeName)
    {
      CustomerIntelligenceData baseProperties = EngagementTelemetryPublisherBase.GetBaseProperties(requestContext);
      baseProperties.Add("EntityType", entityTypeName);
      EngagementTelemetryPublisherBase.Publish(requestContext, "OData", "ODataQuery", baseProperties);
    }
  }
}
