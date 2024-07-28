// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ClientTraceEventsController
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "ClientTrace", ResourceName = "Events")]
  public class ClientTraceEventsController : TfsApiController
  {
    private const string s_area = "Telemetry";
    private const string s_layer = "ClientTraceEventsController";

    public override string ActivityLogArea => "ClientTrace";

    [HttpPost]
    [PublicAllRequestRestrictions(false, true, null)]
    public void PublishEvents([FromBody] ClientTraceEvent[] events)
    {
      Action run = (Action) (() =>
      {
        if (events == null)
          return;
        foreach (ClientTraceEvent clientTraceEvent in events)
        {
          ArgumentUtility.CheckForNull<ClientTraceEvent>(clientTraceEvent, "event");
          try
          {
            CiEventParamUtility.CheckParameterForValidCharacters(clientTraceEvent.Area, "event.area");
            CiEventParamUtility.CheckParameterForValidCharacters(clientTraceEvent.Feature, "event.feature");
          }
          catch (ArgumentException ex)
          {
            this.TfsRequestContext.Trace(15095001, TraceLevel.Error, "Telemetry", nameof (ClientTraceEventsController), "Argument Exception publishing CI event. Area: " + clientTraceEvent.Area + " Feature: " + clientTraceEvent.Feature + ".");
            throw;
          }
          if (this.TfsRequestContext.IsAnonymous())
            this.AddProperty(clientTraceEvent, "Anonymous", (object) true);
          ClientTraceData properties = new ClientTraceData((IDictionary<string, object>) clientTraceEvent.Properties);
          this.TfsRequestContext.GetService<ClientTraceService>().Publish(this.TfsRequestContext, clientTraceEvent.Area, clientTraceEvent.Feature, properties, clientTraceEvent.Level, clientTraceEvent.Method, clientTraceEvent.Component, clientTraceEvent.Message, clientTraceEvent.ExceptionType);
        }
      });
      new CommandService(this.TfsRequestContext, CommandSetter.WithGroupKey((CommandGroupKey) "ServiceInsights.").AndCommandKey((CommandKey) "TeamFoundationTracingService.ClientTrace").AndCommandPropertiesDefaults(ClientTraceEventsController.ClientTraceCircuitBreakerSettings), run).Execute();
    }

    private void AddProperty(
      ClientTraceEvent ctEvent,
      string propertyName,
      object propertyValue,
      bool replaceExisting = false)
    {
      if (ctEvent.Properties == null)
      {
        ctEvent.Properties = new Dictionary<string, object>();
        ctEvent.Properties.Add(propertyName, propertyValue);
      }
      else
      {
        KeyValuePair<string, object> keyValuePair = ctEvent.Properties.FirstOrDefault<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (property => string.Equals(property.Key, propertyName, StringComparison.OrdinalIgnoreCase)));
        if (keyValuePair.Equals((object) new KeyValuePair<string, object>()))
        {
          ctEvent.Properties.Add(propertyName, propertyValue);
        }
        else
        {
          if (!replaceExisting)
            return;
          ctEvent.Properties[keyValuePair.Key] = propertyValue;
        }
      }
    }

    private static CommandPropertiesSetter ClientTraceCircuitBreakerSettings => new CommandPropertiesSetter().WithExecutionMaxRequests(4000);
  }
}
