// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommercePackageControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [ControllerApiVersion(3.0)]
  [RequestContentTypeRestriction(AllowXml = true)]
  public abstract class CommercePackageControllerBase : TfsApiController
  {
    private const string MissingMeterPrefix = "deleted_";

    public override string TraceArea => "Commerce";

    public string TraceLayer => this.ControllerContext.ControllerDescriptor.ControllerName;

    public CommercePackageControllerBase()
    {
    }

    internal CommercePackageControllerBase(HttpControllerContext controllerContext) => this.Initialize(controllerContext);

    [HttpGet]
    [TraceFilter(5107390, 5107399)]
    public virtual ICommercePackage GetCommercePackage(string version = "")
    {
      try
      {
        CommercePackage commercePackage = (CommercePackage) null;
        CollectionHelper.WithCollectionContext(this.TfsRequestContext, this.TfsRequestContext.ServiceHost.InstanceId, (Action<IVssRequestContext>) (collectionContext =>
        {
          Dictionary<string, string> dictionary = new Dictionary<string, string>()
          {
            {
              CommercePackageSettings.UpdateRegistryAccountId,
              collectionContext.ServiceHost.InstanceId.ToString()
            }
          };
          IOfferSubscriptionService service = this.TfsRequestContext.GetService<IOfferSubscriptionService>();
          commercePackage = new CommercePackage()
          {
            OfferMeters = this.GetVersionedOfferMeters(this.TfsRequestContext, version),
            OfferSubscriptions = service.GetOfferSubscriptions(this.TfsRequestContext).Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (x => x.OfferMeter.Category != 0)).Select<IOfferSubscription, OfferSubscription>((Func<IOfferSubscription, OfferSubscription>) (x => (OfferSubscription) x)),
            Configuration = (IDictionary<string, string>) dictionary
          };
        }), method: nameof (GetCommercePackage));
        return (ICommercePackage) commercePackage;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5107398, this.TraceArea, this.TraceLayer, ex);
        throw;
      }
    }

    private IEnumerable<OfferMeter> GetVersionedOfferMeters(
      IVssRequestContext requestContext,
      string version)
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      List<OfferMeter> list = vssRequestContext.GetService<IOfferMeterService>().GetOfferMeters(vssRequestContext).Select<IOfferMeter, OfferMeter>((Func<IOfferMeter, OfferMeter>) (x => (x as OfferMeter).Clone())).ToList<OfferMeter>();
      if (string.IsNullOrEmpty(version))
        this.ModifyOfferMetersForTfs2017Rtm((IList<OfferMeter>) list);
      this.RemoveMeterIdGaps(requestContext, (IList<OfferMeter>) list);
      return (IEnumerable<OfferMeter>) list;
    }

    private void ModifyOfferMetersForTfs2017Rtm(IList<OfferMeter> offerMeters)
    {
      DateTime dateTime = new DateTime(2100, 1, 1);
      OfferMeter offerMeter1 = offerMeters.SingleOrDefault<OfferMeter>((Func<OfferMeter, bool>) (m => m.GalleryId == "ms.vss-code-search"));
      if (offerMeter1 != (OfferMeter) null)
        offerMeter1.BillingStartDate = new DateTime?(dateTime);
      OfferMeter offerMeter2 = offerMeters.SingleOrDefault<OfferMeter>((Func<OfferMeter, bool>) (m => m.GalleryId == "ms.vss-exploratorytesting-web"));
      if (!(offerMeter2 != (OfferMeter) null))
        return;
      offerMeter2.BillingStartDate = new DateTime?(dateTime);
    }

    private void RemoveMeterIdGaps(IVssRequestContext requestContext, IList<OfferMeter> offerMeters)
    {
      if (offerMeters.IsNullOrEmpty<OfferMeter>())
        return;
      IList<int> list = (IList<int>) offerMeters.Select<OfferMeter, int>((Func<OfferMeter, int>) (m => m.MeterId)).OrderBy<int, int>((Func<int, int>) (m => m)).ToList<int>();
      foreach (int num in (IEnumerable<int>) Enumerable.Range(list.First<int>(), list.Last<int>() - list.First<int>() + 1).Except<int>((IEnumerable<int>) list).ToList<int>())
      {
        requestContext.Trace(5107391, TraceLevel.Warning, this.TraceArea, this.TraceLayer, string.Format("MeterId gap found at {0}", (object) num));
        offerMeters.Add(new OfferMeter()
        {
          MeterId = num,
          Category = MeterCategory.Extension,
          GalleryId = string.Format("{0}{1}", (object) "deleted_", (object) num),
          Status = MeterState.Deleted,
          BillingEntity = BillingProvider.SelfManaged,
          MinimumRequiredAccessLevel = MinimumRequiredServiceLevel.Express,
          AssignmentModel = OfferMeterAssignmentModel.Explicit,
          Unit = "Seat",
          BillingState = MeterBillingState.Free,
          OfferScope = OfferScope.User,
          Name = string.Format("{0}{1}", (object) "deleted_", (object) num)
        });
      }
    }
  }
}
