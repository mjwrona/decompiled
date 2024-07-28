// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Telemetry.EngagementTelemetryPublisherBase
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Analytics.Telemetry
{
  internal class EngagementTelemetryPublisherBase
  {
    private static void PublishCustomerIntelligence(
      IVssRequestContext requestContext,
      CustomerIntelligenceData properties)
    {
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Reporting", "Engagement", properties);
    }

    public static CustomerIntelligenceData GetBaseProperties(IVssRequestContext requestContext) => new CustomerIntelligenceData();

    public static void Publish(
      IVssRequestContext requestContext,
      string feature,
      string scenario,
      CustomerIntelligenceData properties)
    {
      properties.Add("Feature", feature);
      properties.Add("Scenario", scenario);
      EngagementTelemetryPublisherBase.PublishCustomerIntelligence(requestContext, properties);
    }
  }
}
