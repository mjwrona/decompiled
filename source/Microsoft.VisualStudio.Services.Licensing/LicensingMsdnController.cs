// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingMsdnController
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Licensing.Http;
using Microsoft.VisualStudio.Services.UserLicensing.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [ControllerApiVersion(1.0)]
  public class LicensingMsdnController : LicensingApiController
  {
    public static readonly string MsdnEntitlementsFeatureName = "VisualStudio.LicensingService.GetMsdnEntitlements";
    public static readonly string FallbackToLocalEntitlementsOnDiff = "VisualStudio.LicensingService.FallbackToLocalEntitlements.OnDiff";
    private readonly ServiceFactory<PlatformLicensingService> m_licensingServiceFactory;
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (MsdnServiceUnavailableException),
        HttpStatusCode.ServiceUnavailable
      }
    };
    private const string s_layer = "LicensingMsdnController";

    public LicensingMsdnController()
      : this((ServiceFactory<PlatformLicensingService>) (x => x.GetService<PlatformLicensingService>()))
    {
    }

    internal LicensingMsdnController(
      ServiceFactory<PlatformLicensingService> licensingServiceFactory)
    {
      this.m_licensingServiceFactory = licensingServiceFactory;
    }

    [HttpGet]
    [ActionName("Presence")]
    [TraceFilter(1039800, 1039809)]
    [ClientLocationId("69522C3F-EECC-48D0-B333-F69FFB8FA6CC")]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage GetMsdnPresence()
    {
      try
      {
        License msdnLicense = this.m_licensingServiceFactory(this.TfsRequestContext).GetMsdnLicense(this.TfsRequestContext);
        return new HttpResponseMessage(!(msdnLicense != (License) null) || !(msdnLicense != License.None) || !(msdnLicense is MsdnLicense) ? HttpStatusCode.NoContent : HttpStatusCode.OK);
      }
      catch (Exception ex)
      {
        this.HandleException(ex, 1039808);
        throw;
      }
    }

    [HttpGet]
    [ActionName("Entitlements")]
    [TraceFilterWithException(1039810, 1039819, 1039818)]
    [ClientLocationId("1cc6137e-12d5-4d44-a4f2-765006c9e85d")]
    [ClientResponseType(typeof (IList<MsdnEntitlement>), null, null)]
    public HttpResponseMessage GetEntitlements()
    {
      if (!this.TfsRequestContext.IsFeatureEnabled(LicensingMsdnController.MsdnEntitlementsFeatureName))
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      IList<MsdnEntitlement> msdnEntitlements = this.m_licensingServiceFactory(this.TfsRequestContext).GetMsdnEntitlements(this.TfsRequestContext);
      try
      {
        List<MsdnEntitlement> target = this.TfsRequestContext.GetClient<UserLicensingHttpClient>(FrameworkServerConstants.UserExtensionPrincipal).GetMsdnEntitlementsAsync((string) LicensingApiController.GetUserSubjectDescriptor(this.TfsRequestContext), cancellationToken: this.TfsRequestContext.CancellationToken).SyncResult<List<MsdnEntitlement>>();
        MsdnEntitlementsDiffLogger entitlementsDiffLogger = new MsdnEntitlementsDiffLogger(msdnEntitlements, (IList<MsdnEntitlement>) target);
        entitlementsDiffLogger.LogDiff(this.TfsRequestContext);
        return entitlementsDiffLogger.HasDiff() && this.TfsRequestContext.IsFeatureEnabled(LicensingMsdnController.FallbackToLocalEntitlementsOnDiff) ? this.Request.CreateResponse<IList<MsdnEntitlement>>(HttpStatusCode.OK, msdnEntitlements) : this.Request.CreateResponse<List<MsdnEntitlement>>(HttpStatusCode.OK, target);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(141531, "VisualStudio.Services.LicensingService", nameof (LicensingMsdnController), ex);
      }
      return this.Request.CreateResponse<IList<MsdnEntitlement>>(HttpStatusCode.OK, msdnEntitlements);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) LicensingMsdnController.s_httpExceptions;

    private void HandleException(Exception ex, int tracepoint)
    {
      if (LicensingMsdnController.s_httpExceptions.ContainsKey(ex.GetType()))
        return;
      this.TfsRequestContext.TraceException(tracepoint, "VisualStudio.Services.LicensingService", nameof (LicensingMsdnController), ex);
      TeamFoundationEventLog.Default.LogException(ex.Message, ex);
    }
  }
}
