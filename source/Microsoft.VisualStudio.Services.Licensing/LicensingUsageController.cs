// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingUsageController
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [ControllerApiVersion(1.0)]
  public class LicensingUsageController : LicensingApiController
  {
    private readonly ServiceFactory<ILicensingEntitlementService> m_accountEntitlementServiceFactory;
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (InvalidRightNameException),
        HttpStatusCode.BadRequest
      }
    };
    private const string s_layer = "LicensingUsageController";

    public LicensingUsageController()
      : this((ServiceFactory<ILicensingEntitlementService>) (x => x.GetService<ILicensingEntitlementService>()))
    {
    }

    internal LicensingUsageController(
      ServiceFactory<ILicensingEntitlementService> accountEntitlementServiceFactory)
    {
      this.m_accountEntitlementServiceFactory = accountEntitlementServiceFactory;
    }

    [HttpGet]
    [TraceFilter(1039000, 1039009)]
    [ClientResponseType(typeof (IEnumerable<AccountLicenseUsage>), null, null)]
    public HttpResponseMessage GetAccountLicensesUsage()
    {
      try
      {
        if (!this.TfsRequestContext.ServiceHost.IsOnly(TeamFoundationHostType.ProjectCollection))
          throw new UnexpectedHostTypeException(this.TfsRequestContext.ServiceHost.HostType);
        return this.Request.CreateResponse<IEnumerable<AccountLicenseUsage>>(HttpStatusCode.OK, (IEnumerable<AccountLicenseUsage>) this.m_accountEntitlementServiceFactory(this.TfsRequestContext).GetLicensesUsage(this.TfsRequestContext, true));
      }
      catch (Exception ex)
      {
        this.HandleException(ex, 1039008);
        throw;
      }
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) LicensingUsageController.s_httpExceptions;

    private void HandleException(Exception ex, int tracepoint)
    {
      if (LicensingUsageController.s_httpExceptions.ContainsKey(ex.GetType()))
        return;
      this.TfsRequestContext.TraceException(tracepoint, "VisualStudio.Services.LicensingService", nameof (LicensingUsageController), ex);
      TeamFoundationEventLog.Default.LogException(ex.Message, ex);
    }
  }
}
