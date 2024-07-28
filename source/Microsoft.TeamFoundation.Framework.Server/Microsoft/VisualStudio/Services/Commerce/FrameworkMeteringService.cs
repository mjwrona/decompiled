// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.FrameworkMeteringService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FrameworkMeteringService : IMeteringService, IVssFrameworkService
  {
    private const string FrameworkMeteringServiceName = "FrameworkMeteringService";
    private const string s_area = "Commerce";
    private const string s_layer = "BusinessLogic";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckHostedDeployment();
      if (requestContext.ServiceHost.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public IEnumerable<ISubscriptionResource> GetResourceStatus(
      IVssRequestContext requestContext,
      bool nextBillingPeriod = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (requestContext.ServiceHost.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.TraceEnter(5103821, "Commerce", "BusinessLogic", nameof (GetResourceStatus));
      try
      {
        requestContext.Trace(5103822, TraceLevel.Info, "Commerce", "BusinessLogic", "Getting all resources status for {0}", (object) requestContext.ServiceHost.InstanceId);
        return CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback<IEnumerable<ISubscriptionResource>>(requestContext, (Func<IEnumerable<ISubscriptionResource>>) (() => this.GetCommerceHttpClient(requestContext).GetResourceStatus(nextBillingPeriod).SyncResult<IEnumerable<ISubscriptionResource>>()), (Func<IEnumerable<ISubscriptionResource>>) (() => this.GetHttpClient(requestContext).GetResourceStatus(nextBillingPeriod).SyncResult<IEnumerable<ISubscriptionResource>>()), "Commerce", "BusinessLogic", 5103822, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5103828, "Commerce", "BusinessLogic", ex);
        switch (ex)
        {
          case VssServiceResponseException _:
          case HttpRequestException _:
          case WebException _:
          case TaskCanceledException _:
            requestContext.Trace(5103829, TraceLevel.Info, "Commerce", "BusinessLogic", "Returning default value because of unhandled exception");
            return (IEnumerable<ISubscriptionResource>) new SubscriptionResource[1]
            {
              new SubscriptionResource() { IsUseable = true }
            };
          default:
            throw;
        }
      }
      finally
      {
        requestContext.TraceLeave(5103830, "Commerce", "BusinessLogic", nameof (GetResourceStatus));
      }
    }

    public ISubscriptionResource GetResourceStatus(
      IVssRequestContext requestContext,
      ResourceName resourceName,
      bool nextBillingPeriod = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (requestContext.ServiceHost.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.TraceEnter(5103801, "Commerce", "BusinessLogic", nameof (GetResourceStatus));
      try
      {
        requestContext.Trace(5103802, TraceLevel.Info, "Commerce", "BusinessLogic", "Getting resource status for {0} and {1}", (object) requestContext.ServiceHost.InstanceId, (object) resourceName, (object) true);
        return CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback<ISubscriptionResource>(requestContext, (Func<ISubscriptionResource>) (() => this.GetCommerceHttpClient(requestContext).GetResourceStatus(new ResourceName?(resourceName), nextBillingPeriod).SyncResult<ISubscriptionResource>()), (Func<ISubscriptionResource>) (() => this.GetHttpClient(requestContext).GetResourceStatus(new ResourceName?(resourceName), nextBillingPeriod).SyncResult<ISubscriptionResource>()), "Commerce", "BusinessLogic", 5103802, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5103808, "Commerce", "BusinessLogic", ex);
        switch (ex)
        {
          case VssServiceResponseException _:
          case HttpRequestException _:
          case WebException _:
          case TaskCanceledException _:
            requestContext.Trace(5103809, TraceLevel.Info, "Commerce", "BusinessLogic", "Returning default value because of unhandled exception");
            return (ISubscriptionResource) new SubscriptionResource()
            {
              IsUseable = true
            };
          default:
            throw;
        }
      }
      finally
      {
        requestContext.TraceLeave(5103810, "Commerce", "BusinessLogic", nameof (GetResourceStatus));
      }
    }

    public void ReportUsage(
      IVssRequestContext requestContext,
      Guid eventUserId,
      ResourceName resourceName,
      int quantity,
      string eventId,
      DateTime billingEventDateTime)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForOutOfRange(quantity, nameof (quantity), 0);
      ArgumentUtility.CheckStringForNullOrEmpty(eventId, nameof (eventId));
      ArgumentUtility.CheckForEmptyGuid(eventUserId, nameof (eventUserId));
      if (requestContext.ServiceHost.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.TraceEnter(5103811, "Commerce", "BusinessLogic", nameof (ReportUsage));
      try
      {
        CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback(requestContext, (Action) (() => this.GetCommerceHttpClient(requestContext).ReportUsage(eventUserId, resourceName, quantity, eventId, billingEventDateTime).Wait()), (Action) (() => this.GetHttpClient(requestContext).ReportUsage(eventUserId, resourceName, quantity, eventId, billingEventDateTime).Wait()), "Commerce", "BusinessLogic", 5103811, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5103819, "Commerce", "BusinessLogic", ex);
        if (ex is AggregateException)
          throw (ex as AggregateException).Flatten().InnerException;
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5103820, "Commerce", "BusinessLogic", nameof (ReportUsage));
      }
    }

    public void TogglePaidBilling(
      IVssRequestContext requestContext,
      ResourceName resourceName,
      bool paidBillingState)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (requestContext.ServiceHost.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.TraceEnter(5103821, "Commerce", "BusinessLogic", nameof (TogglePaidBilling));
      try
      {
        CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback(requestContext, (Action) (() => this.GetCommerceHttpClient(requestContext).TogglePaidBilling(resourceName, paidBillingState).Wait()), (Action) (() => this.GetHttpClient(requestContext).TogglePaidBilling(resourceName, paidBillingState).Wait()), "Commerce", "BusinessLogic", 5103821, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5103829, "Commerce", "BusinessLogic", ex);
        if (ex is AggregateException)
          throw (ex as AggregateException).Flatten().InnerException;
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5103830, "Commerce", "BusinessLogic", nameof (TogglePaidBilling));
      }
    }

    public void SetAccountQuantity(
      IVssRequestContext requestContext,
      ResourceName resourceName,
      int includedQuantity,
      int maximumQuantity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (requestContext.ServiceHost.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.TraceEnter(5103821, "Commerce", "BusinessLogic", nameof (SetAccountQuantity));
      try
      {
        CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback(requestContext, (Action) (() => this.GetCommerceHttpClient(requestContext).SetAccountQuantity(resourceName, includedQuantity, maximumQuantity).Wait()), (Action) (() => this.GetHttpClient(requestContext).SetAccountQuantity(resourceName, includedQuantity, maximumQuantity).Wait()), "Commerce", "BusinessLogic", 5103821, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5103829, "Commerce", "BusinessLogic", ex);
        if (ex is AggregateException)
          throw (ex as AggregateException).Flatten().InnerException;
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5103830, "Commerce", "BusinessLogic", nameof (SetAccountQuantity));
      }
    }

    public IEnumerable<IUsageEventAggregate> GetUsage(
      IVssRequestContext requestContext,
      DateTime startTime,
      DateTime endTime,
      TimeSpan timeSpan,
      ResourceName resource)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (requestContext.ServiceHost.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.TraceEnter(5103831, "Commerce", "BusinessLogic", nameof (GetUsage));
      try
      {
        return CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback<IEnumerable<IUsageEventAggregate>>(requestContext, (Func<IEnumerable<IUsageEventAggregate>>) (() => this.GetCommerceHttpClient(requestContext).GetUsage(startTime, endTime, timeSpan, resource).SyncResult<IEnumerable<IUsageEventAggregate>>()), (Func<IEnumerable<IUsageEventAggregate>>) (() => this.GetHttpClient(requestContext).GetUsage(startTime, endTime, timeSpan, resource).SyncResult<IEnumerable<IUsageEventAggregate>>()), "Commerce", "BusinessLogic", 5103831, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5103839, "Commerce", "BusinessLogic", ex);
        if (ex is AggregateException)
          throw (ex as AggregateException).Flatten().InnerException;
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5103840, "Commerce", "BusinessLogic", nameof (GetUsage));
      }
    }

    internal virtual Microsoft.VisualStudio.Services.Commerce.Client.MeteringHttpClient GetHttpClient(
      IVssRequestContext requestContext)
    {
      return requestContext.GetClient<Microsoft.VisualStudio.Services.Commerce.Client.MeteringHttpClient>();
    }

    internal virtual Microsoft.VisualStudio.Services.Commerce.WebApi.MeteringHttpClient GetCommerceHttpClient(
      IVssRequestContext requestContext)
    {
      return requestContext.GetClient<Microsoft.VisualStudio.Services.Commerce.WebApi.MeteringHttpClient>();
    }
  }
}
