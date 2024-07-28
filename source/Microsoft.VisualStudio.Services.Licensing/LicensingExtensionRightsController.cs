// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingExtensionRightsController
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [ControllerApiVersion(1.0)]
  public class LicensingExtensionRightsController : LicensingApiController
  {
    private readonly ServiceFactory<PlatformLicensingService> m_licensingServiceFactory;
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (InvalidExtensionIdException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidOperationException),
        HttpStatusCode.BadRequest
      }
    };
    private const string s_layer = "LicensingExtensionRightsController";

    public LicensingExtensionRightsController()
      : this((ServiceFactory<PlatformLicensingService>) (x => x.GetService<PlatformLicensingService>()))
    {
    }

    internal LicensingExtensionRightsController(
      ServiceFactory<PlatformLicensingService> licensingServiceFactory)
    {
      this.m_licensingServiceFactory = licensingServiceFactory;
    }

    [HttpPost]
    [TraceFilter(1037000, 1037009)]
    public IDictionary<string, bool> ComputeExtensionRights([FromBody] string[] ids)
    {
      try
      {
        ArgumentUtility.CheckForNull<string[]>(ids, nameof (ids));
        if (!this.TfsRequestContext.ServiceHost.IsOnly(TeamFoundationHostType.ProjectCollection))
          throw new InvalidOperationException("Invalid request context type.");
        this.TfsRequestContext.Trace(1037001, TraceLevel.Info, "VisualStudio.Services.LicensingService", nameof (LicensingExtensionRightsController), "ComputeExtensionRights called with ids: " + string.Join(",", ids));
        return this.m_licensingServiceFactory(this.TfsRequestContext).GetExtensionRights(this.TfsRequestContext, (IEnumerable<string>) ids);
      }
      catch (Exception ex)
      {
        this.HandleException(ex, 1037008);
        throw;
      }
    }

    [HttpGet]
    [TraceFilter(1037010, 1037019)]
    public ExtensionRightsResult GetExtensionRights()
    {
      try
      {
        if (!this.TfsRequestContext.ServiceHost.IsOnly(TeamFoundationHostType.ProjectCollection))
          throw new InvalidOperationException("Invalid request context type.");
        this.TfsRequestContext.Trace(1037012, TraceLevel.Info, "VisualStudio.Services.LicensingService", nameof (LicensingExtensionRightsController), "GetExtensionRights called");
        return this.m_licensingServiceFactory(this.TfsRequestContext).GetExtensionRights(this.TfsRequestContext);
      }
      catch (Exception ex)
      {
        this.HandleException(ex, 1037018);
        throw;
      }
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) LicensingExtensionRightsController.s_httpExceptions;

    private void HandleException(Exception ex, int tracepoint)
    {
      if (LicensingExtensionRightsController.s_httpExceptions.ContainsKey(ex.GetType()))
        return;
      this.TfsRequestContext.TraceException(tracepoint, "VisualStudio.Services.LicensingService", nameof (LicensingExtensionRightsController), ex);
      TeamFoundationEventLog.Default.LogException(ex.Message, ex);
    }
  }
}
