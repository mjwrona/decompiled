// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.CustomerIntelligenceEventsController
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  public class CustomerIntelligenceEventsController : TfsApiController
  {
    private const string s_area = "Telemetry";
    private const string s_layer = "CustomerIntelligenceEventsController";

    public override string ActivityLogArea => "CustomerIntelligence";

    [HttpPost]
    [PublicAllRequestRestrictions(false, true, null)]
    public void PublishEvents([FromBody] CustomerIntelligenceEvent[] events)
    {
      this.TfsRequestContext.TraceEnter(15095000, "Telemetry", nameof (CustomerIntelligenceEventsController), nameof (PublishEvents));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) events, nameof (events));
      foreach (CustomerIntelligenceEvent intelligenceEvent in events)
      {
        ArgumentUtility.CheckForNull<CustomerIntelligenceEvent>(intelligenceEvent, "event");
        try
        {
          CiEventParamUtility.CheckParameterForValidCharacters(intelligenceEvent.Area, "event.area");
          CiEventParamUtility.CheckParameterForValidCharacters(intelligenceEvent.Feature, "event.feature");
        }
        catch (ArgumentException ex)
        {
          this.TfsRequestContext.Trace(15095001, TraceLevel.Error, "Telemetry", nameof (CustomerIntelligenceEventsController), "Argument Exception publishing CI event. Area: " + intelligenceEvent.Area + " Feature: " + intelligenceEvent.Feature + ".");
          throw;
        }
        if (this.TfsRequestContext.UserContext == (IdentityDescriptor) null || this.TfsRequestContext.IsAnonymousPrincipal())
          this.AddProperty(intelligenceEvent, "Anonymous", (object) true);
        CustomerIntelligenceData properties = new CustomerIntelligenceData((IDictionary<string, object>) intelligenceEvent.Properties);
        this.TfsRequestContext.GetService<CustomerIntelligenceService>().Publish(this.TfsRequestContext, intelligenceEvent.Area, intelligenceEvent.Feature, properties);
      }
      this.TfsRequestContext.TraceLeave(15095020, "Telemetry", nameof (CustomerIntelligenceEventsController), nameof (PublishEvents));
    }

    private void AddProperty(
      CustomerIntelligenceEvent ciEvent,
      string propertyName,
      object propertyValue,
      bool replaceExisting = false)
    {
      if (ciEvent.Properties == null)
      {
        ciEvent.Properties = new Dictionary<string, object>();
        ciEvent.Properties.Add(propertyName, propertyValue);
      }
      else
      {
        KeyValuePair<string, object> keyValuePair = ciEvent.Properties.FirstOrDefault<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (property => string.Equals(property.Key, propertyName, StringComparison.OrdinalIgnoreCase)));
        if (keyValuePair.Equals((object) new KeyValuePair<string, object>()))
        {
          ciEvent.Properties.Add(propertyName, propertyValue);
        }
        else
        {
          if (!replaceExisting)
            return;
          ciEvent.Properties[keyValuePair.Key] = propertyValue;
        }
      }
    }
  }
}
