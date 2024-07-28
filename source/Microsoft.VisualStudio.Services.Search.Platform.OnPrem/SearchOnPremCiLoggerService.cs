// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Search.Platform.OnPrem.SearchOnPremCiLoggerService
// Assembly: Microsoft.VisualStudio.Services.Search.Platform.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 34F866B2-C282-4F28-9C5F-A4E5E97C2DB9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platform.OnPrem.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Search.Platform.Common;
using Microsoft.TeamFoundation.Server.Core.Telemetry;
using Microsoft.VisualStudio.Telemetry;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.Search.Platform.OnPrem
{
  [Export(typeof (ISearchCiLoggerService))]
  public class SearchOnPremCiLoggerService : ISearchCiLoggerService, IVssFrameworkService
  {
    public void PublishOnPremisesEvent(
      IVssRequestContext requestContext,
      string eventName,
      Dictionary<string, string> eventProperties)
    {
      try
      {
        TeamFoundationTelemetryService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTelemetryService>();
        TelemetryEvent telemetryEvent = new TelemetryEvent(eventName);
        if (eventProperties == null || eventProperties.Count == 0)
          return;
        foreach (KeyValuePair<string, string> eventProperty in eventProperties)
          telemetryEvent.Properties[eventProperty.Key] = (object) eventProperty.Value;
        service.PostEvent(telemetryEvent);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1082104, "Diagnostics", "Diagnostics", ex);
      }
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      CustomerIntelligenceData properties)
    {
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      string property,
      bool value)
    {
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      string property,
      double value)
    {
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      string property,
      string value)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
