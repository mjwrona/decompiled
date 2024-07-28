// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.UsageEventsControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [ControllerApiVersion(1.0)]
  [RequestContentTypeRestriction(AllowXml = true)]
  public abstract class UsageEventsControllerBase : TfsApiController
  {
    private static readonly Dictionary<Type, HttpStatusCode> httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static UsageEventsControllerBase()
    {
      UsageEventsControllerBase.httpExceptions.Add(typeof (CommerceSecurityException), HttpStatusCode.Unauthorized);
      UsageEventsControllerBase.httpExceptions.Add(typeof (AccountQuantityException), HttpStatusCode.RequestEntityTooLarge);
      UsageEventsControllerBase.httpExceptions.Add(typeof (InvalidResourceException), HttpStatusCode.BadRequest);
      UsageEventsControllerBase.httpExceptions.Add(typeof (InvalidOperationException), HttpStatusCode.BadRequest);
    }

    public UsageEventsControllerBase()
    {
    }

    public override string ActivityLogArea => "Commerce";

    internal UsageEventsControllerBase(HttpControllerContext controllerContext) => this.Initialize(controllerContext);

    protected string GetResourceNameFromRouteData()
    {
      string empty = string.Empty;
      if (this.ControllerContext.RouteData.Values.ContainsKey("resourceName"))
        empty = this.ControllerContext.RouteData.Values["resourceName"].ToString();
      ArgumentUtility.CheckStringForNullOrEmpty(empty, "resourceName");
      return empty;
    }

    protected IOfferMeter GetOfferMeter(string resourceName)
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, resourceName) ?? throw new ArgumentNullException(nameof (resourceName));
    }

    protected internal MeteredResource GetResource(string resourceName) => ((OfferMeter) this.GetOfferMeter(resourceName)).ToMeteredResource() ?? throw new ArgumentNullException(nameof (resourceName));

    [HttpPost]
    [TraceFilter(5108591, 5108600)]
    [ClientResponseType(typeof (void), null, null)]
    public virtual HttpResponseMessage ReportUsage(UsageEvent usageEvent)
    {
      ArgumentUtility.CheckForNull<UsageEvent>(usageEvent, nameof (usageEvent));
      string nameFromRouteData = this.GetResourceNameFromRouteData();
      usageEvent.MeterName = this.GetOfferMeter(nameFromRouteData).Name;
      try
      {
        this.TfsRequestContext.Trace(5108592, TraceLevel.Info, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, "Save usage for {0} and {1}", (object) this.TfsRequestContext.ServiceHost.InstanceId, (object) nameFromRouteData);
        CollectionHelper.WithCollectionContext(this.TfsRequestContext, this.TfsRequestContext.ServiceHost.InstanceId, (Action<IVssRequestContext>) (collectionContext => collectionContext.GetService<PlatformOfferSubscriptionService>().ReportUsage(collectionContext, usageEvent.AssociatedUser, usageEvent.MeterName, ResourceRenewalGroup.Monthly, usageEvent.Quantity, usageEvent.EventId, usageEvent.BillableDate, false)), method: nameof (ReportUsage));
        return this.Request.CreateResponse(HttpStatusCode.Accepted);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5108599, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, ex);
        throw;
      }
    }

    [HttpGet]
    [TraceFilter(5108601, 5108610)]
    public virtual IEnumerable<IUsageEventAggregate> GetUsage(
      DateTime startTime,
      DateTime endTime,
      TimeSpan timeSpan)
    {
      try
      {
        string nameFromRouteData = this.GetResourceNameFromRouteData();
        MeteredResource resource = this.GetResource(nameFromRouteData);
        this.TfsRequestContext.Trace(5108602, TraceLevel.Info, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, "Get usage for {0} and {1}", (object) this.TfsRequestContext.ServiceHost.InstanceId, (object) nameFromRouteData);
        return this.TfsRequestContext.GetService<IOfferSubscriptionService>().GetUsage(this.TfsRequestContext, startTime, endTime, timeSpan, resource.ResourceName);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5108609, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, ex);
        throw;
      }
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) UsageEventsControllerBase.httpExceptions;
  }
}
