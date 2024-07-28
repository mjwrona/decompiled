// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferMeterFrameworkCacheChangeInvalidationHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [Export(typeof (ICommerceFrameworkCacheInvalidationHandler))]
  public class OfferMeterFrameworkCacheChangeInvalidationHandler : 
    ICommerceFrameworkCacheInvalidationHandler
  {
    public const string OfferMeterChangeTopic = "Microsoft.VisualStudio.Services.Commerce.OfferMeterChange";
    private static readonly string s_Area = "Commerce";
    private static readonly string s_Layer = nameof (OfferMeterFrameworkCacheChangeInvalidationHandler);

    public TeamFoundationHostType AcceptedHostTypes => TeamFoundationHostType.Deployment;

    public string SupportedTopic => "Microsoft.VisualStudio.Services.Commerce.OfferMeterChange";

    public void Invalidate(IVssRequestContext requestContext, object message)
    {
      try
      {
        requestContext.TraceEnter(5104261, OfferMeterFrameworkCacheChangeInvalidationHandler.s_Area, OfferMeterFrameworkCacheChangeInvalidationHandler.s_Layer, nameof (Invalidate));
        if (!(message is OfferMeterCacheChangeMessage objectToSerialize))
          requestContext.Trace(5104268, TraceLevel.Error, OfferMeterFrameworkCacheChangeInvalidationHandler.s_Area, OfferMeterFrameworkCacheChangeInvalidationHandler.s_Layer, "Message received is not compatible with type {0}, ensure multiple types are not registered on topic {1}", (object) "OfferMeterCacheChangeMessage", (object) this.SupportedTopic);
        else
          requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, SqlNotificationEventClasses.OfferMeterChanged, TeamFoundationSerializationUtility.SerializeToString<OfferMeterCacheChangeMessage>(objectToSerialize));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104269, OfferMeterFrameworkCacheChangeInvalidationHandler.s_Area, OfferMeterFrameworkCacheChangeInvalidationHandler.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceEnter(5104270, OfferMeterFrameworkCacheChangeInvalidationHandler.s_Area, OfferMeterFrameworkCacheChangeInvalidationHandler.s_Layer, nameof (Invalidate));
      }
    }
  }
}
