// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.TelemetryHelper
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Licensing.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class TelemetryHelper
  {
    internal static void StoreClientRightsTelemetry(
      IVssRequestContext requestContext,
      ClientRightsTelemetryContext telemetryContext,
      IRightsQueryContext queryContext,
      string serializedClientRights,
      int creationDurationMilliseconds,
      string area,
      string layer)
    {
      try
      {
        CustomerIntelligenceData eventData = new CustomerIntelligenceData();
        if (telemetryContext != null && telemetryContext.Attributes != null && telemetryContext.Attributes.Count > 0)
        {
          foreach (KeyValuePair<string, string> attribute in (IEnumerable<KeyValuePair<string, string>>) telemetryContext.Attributes)
          {
            if (!string.IsNullOrEmpty(attribute.Key))
              eventData.Add(attribute.Key, attribute.Value ?? string.Empty);
          }
        }
        eventData.Add(CustomerIntelligenceProperty.MachineId, queryContext.MachineId ?? string.Empty);
        if (queryContext.ProductVersion != (Version) null)
          eventData.Add(CustomerIntelligenceProperty.ProductVersion, queryContext.ProductVersion.ToString());
        eventData.Add(CustomerIntelligenceProperty.ProductVersionBuildLab, queryContext.ProductVersionBuildLab ?? string.Empty);
        eventData.Add(CustomerIntelligenceProperty.ReleaseType, queryContext.ReleaseType.ToString());
        eventData.Add(CustomerIntelligenceProperty.RequestType, queryContext.RequestType.ToString());
        eventData.Add(CustomerIntelligenceProperty.SingleRightName, queryContext.SingleRightName ?? string.Empty);
        eventData.Add(CustomerIntelligenceProperty.VisualStudioFamily, queryContext.VisualStudioFamily.ToString());
        eventData.Add(CustomerIntelligenceProperty.VisualStudioEdition, queryContext.VisualStudioEdition.ToString());
        eventData.Add(CustomerIntelligenceProperty.Duration, (double) creationDurationMilliseconds);
        eventData.Add(CustomerIntelligenceProperty.ClientRights, serializedClientRights ?? string.Empty);
        TelemetryHelper.PublishCustomerIntelligenceEvent(requestContext, "LicenseTelemetry", eventData);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1038601, area, layer, ex);
      }
    }

    internal static void AddMsdnEntitlementServiceFailure(
      IVssRequestContext requestContext,
      string serviceUri,
      string area,
      string layer)
    {
      try
      {
        CustomerIntelligenceData eventData = new CustomerIntelligenceData();
        eventData.Add(CustomerIntelligenceProperty.ActivityId, (object) requestContext.ActivityId);
        TelemetryHelper.PublishCustomerIntelligenceEvent(requestContext, "MsdnEnitlementServiceFailed", eventData);
        TeamFoundationEventLog.Default.Log(requestContext, FrameworkResources.MsdnEntitlementSeriveFailed((object) requestContext.ActivityId, (object) serviceUri), TeamFoundationEventId.LicensingGetMsdnEntitlementsFailed, EventLogEntryType.Error);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1038602, area, layer, ex);
      }
    }

    internal static void RecordCIEventForAddGuestUserToAadResult(
      IVssRequestContext requestContext,
      string invitedAadGuestUserId,
      string area,
      string layer)
    {
      try
      {
        CustomerIntelligenceData eventData = new CustomerIntelligenceData();
        eventData.Add("invitedUserId", invitedAadGuestUserId);
        TelemetryHelper.PublishCustomerIntelligenceEvent(requestContext, "GetRedeemUrlForAadGuestUser", eventData);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1038603, area, layer, ex);
      }
    }

    internal static void PublishCustomerIntelligenceEvent(
      IVssRequestContext requestContext,
      string feature,
      CustomerIntelligenceData eventData)
    {
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.Licensing, feature, eventData);
    }
  }
}
