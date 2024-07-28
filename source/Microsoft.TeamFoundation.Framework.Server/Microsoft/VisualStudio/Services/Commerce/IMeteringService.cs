// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.IMeteringService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DefaultServiceImplementation(typeof (FrameworkMeteringService))]
  public interface IMeteringService : IVssFrameworkService
  {
    IEnumerable<ISubscriptionResource> GetResourceStatus(
      IVssRequestContext requestContext,
      bool nextBillingPeriod = false);

    ISubscriptionResource GetResourceStatus(
      IVssRequestContext requestContext,
      ResourceName resourceName,
      bool nextBillingPeriod = false);

    void ReportUsage(
      IVssRequestContext requestContext,
      Guid eventUserId,
      ResourceName resourceName,
      int quantity,
      string eventId,
      DateTime billingEventDateTime);

    void TogglePaidBilling(
      IVssRequestContext requestContext,
      ResourceName resourceName,
      bool paidBillingState);

    void SetAccountQuantity(
      IVssRequestContext requestContext,
      ResourceName resourceName,
      int includedQuantity,
      int maximumQuantity);

    IEnumerable<IUsageEventAggregate> GetUsage(
      IVssRequestContext requestContext,
      DateTime startTime,
      DateTime endTime,
      TimeSpan timeSpan,
      ResourceName resource);
  }
}
