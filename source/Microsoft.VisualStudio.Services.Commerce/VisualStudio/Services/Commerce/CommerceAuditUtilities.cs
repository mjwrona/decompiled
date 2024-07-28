// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceAuditUtilities
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class CommerceAuditUtilities
  {
    private const string Unlimited = "unlimited";
    private const string Area = "Commerce";
    private const string Layer = "CommerceAuditUtilities";

    public static void LogCLTLimitUpdate(
      IVssRequestContext requestContext,
      int previousMaxQuantity,
      int updatedMaxQuantity,
      bool isBillingEnabled,
      int freeQuantity)
    {
      if (previousMaxQuantity == updatedMaxQuantity)
        return;
      if (!requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.EnableLogLimitUpdate"))
        return;
      try
      {
        object previousMaxLimit = CommerceAuditUtilities.GetPreviousMaxLimit(isBillingEnabled, previousMaxQuantity, freeQuantity);
        string str = updatedMaxQuantity > 150000000 ? "unlimited" : updatedMaxQuantity.ToString();
        IVssRequestContext requestContext1 = requestContext;
        string limitUpdate = CommerceAuditConstants.LimitUpdate;
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add(CommerceAuditConstants.MeterName, (object) "Cloud Load Test");
        data.Add(CommerceAuditConstants.PreviousLimitNumber, previousMaxLimit);
        data.Add(CommerceAuditConstants.LimitNumber, (object) str);
        Guid targetHostId = new Guid();
        Guid projectId = new Guid();
        requestContext1.LogAuditEvent(limitUpdate, data, targetHostId, projectId);
        requestContext.TraceAlways(5109355, TraceLevel.Info, "Commerce", nameof (CommerceAuditUtilities), new
        {
          Msg = "LogLimitUpdate information for Cloud Load Test",
          previousMaxQuantity = previousMaxQuantity,
          updatedMaxQuantity = updatedMaxQuantity
        }.Serialize());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5109356, "Commerce", nameof (CommerceAuditUtilities), ex);
      }
    }

    public static void LogSubscriptionLink(IVssRequestContext requestContext, Guid subscriptionGuid)
    {
      if (!requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.EnableLogSubscriptionLink"))
        return;
      try
      {
        IVssRequestContext requestContext1 = requestContext;
        string subscriptionLink = CommerceAuditConstants.SubscriptionLink;
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add(CommerceAuditConstants.NewSubscriptionGuid, (object) subscriptionGuid);
        Guid targetHostId = new Guid();
        Guid projectId = new Guid();
        requestContext1.LogAuditEvent(subscriptionLink, data, targetHostId, projectId);
        requestContext.TraceAlways(5109357, TraceLevel.Info, "Commerce", nameof (CommerceAuditUtilities), new
        {
          Msg = nameof (LogSubscriptionLink),
          subscriptionGuid = subscriptionGuid
        }.Serialize());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5109358, "Commerce", nameof (CommerceAuditUtilities), ex);
      }
    }

    public static void LogSubscriptionUnlink(
      IVssRequestContext requestContext,
      Guid subscriptionGuid)
    {
      if (!requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.EnableLogSubscriptionUnlink"))
        return;
      try
      {
        IVssRequestContext requestContext1 = requestContext;
        string subscriptionUnlink = CommerceAuditConstants.SubscriptionUnlink;
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add(CommerceAuditConstants.PreviousSubscriptionGuid, (object) subscriptionGuid);
        Guid targetHostId = new Guid();
        Guid projectId = new Guid();
        requestContext1.LogAuditEvent(subscriptionUnlink, data, targetHostId, projectId);
        requestContext.TraceAlways(5109359, TraceLevel.Info, "Commerce", nameof (CommerceAuditUtilities), new
        {
          Msg = "LogSubscriptionLink",
          subscriptionGuid = subscriptionGuid
        }.Serialize());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5109360, "Commerce", nameof (CommerceAuditUtilities), ex);
      }
    }

    public static void LogSubscriptionUpdate(
      IVssRequestContext requestContext,
      Guid previousSubscriptionGuid,
      Guid newSubscriptionGuid)
    {
      if (!requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.EnableLogSubscriptionUpdate"))
        return;
      try
      {
        IVssRequestContext requestContext1 = requestContext;
        string subscriptionUpdate = CommerceAuditConstants.SubscriptionUpdate;
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add(CommerceAuditConstants.PreviousSubscriptionGuid, (object) previousSubscriptionGuid);
        data.Add(CommerceAuditConstants.NewSubscriptionGuid, (object) newSubscriptionGuid);
        Guid targetHostId = new Guid();
        Guid projectId = new Guid();
        requestContext1.LogAuditEvent(subscriptionUpdate, data, targetHostId, projectId);
        requestContext.TraceAlways(5109361, TraceLevel.Info, "Commerce", nameof (CommerceAuditUtilities), new
        {
          Msg = nameof (LogSubscriptionUpdate),
          previousSubscriptionGuid = previousSubscriptionGuid,
          newSubscriptionGuid = newSubscriptionGuid
        }.Serialize());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5109362, "Commerce", nameof (CommerceAuditUtilities), ex);
      }
    }

    private static object GetPreviousMaxLimit(
      bool isPaidBillingEnabled,
      int maxQuantity,
      int freeQuantity)
    {
      return isPaidBillingEnabled ? (maxQuantity <= 150000000 ? (object) maxQuantity : (object) "unlimited") : (object) freeQuantity;
    }
  }
}
