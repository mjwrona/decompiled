// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.BillingEventsManipulationBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal abstract class BillingEventsManipulationBase
  {
    public abstract bool EnableSkipBilling(IVssRequestContext requestContext);

    public abstract bool EnableAdditionalMetersSkipBilling(IVssRequestContext requestContext);

    public abstract void DenyEventsForBilling(
      BillableEvent billableEvent,
      bool additionalMetersToSkip = false);

    public void UpdateEvents(IVssRequestContext requestContext, BillableEvent billableEvent)
    {
      if (!this.EnableSkipBilling(requestContext))
        return;
      requestContext.TraceAlways(5109288, TraceLevel.Info, "Commerce", nameof (BillingEventsManipulationBase), string.Format("Updating billing events to skip billing for denied meters. Event for the host {0} with PlatformMeter Guid {1}.", (object) requestContext.ServiceHost.InstanceId, (object) billableEvent?.MeterPlatformGuid));
      this.DenyEventsForBilling(billableEvent, this.EnableAdditionalMetersSkipBilling(requestContext));
      requestContext.TraceAlways(5109286, TraceLevel.Info, "Commerce", nameof (BillingEventsManipulationBase), string.Format("Manipulated event for the host {0} with PlatformMeter Guid {1}.", (object) requestContext.ServiceHost.InstanceId, (object) billableEvent?.MeterPlatformGuid));
    }
  }
}
