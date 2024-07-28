// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.PackageCountController
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [ClientIgnore]
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "packageResultsCount")]
  public class PackageCountController : SearchApiController
  {
    protected ICountHandlerFactory countHandlerFactory { get; set; }

    public PackageCountController() => this.countHandlerFactory = (ICountHandlerFactory) new CountHandlerFactory();

    protected override void InitializeInternal(HttpControllerContext controllerContext) => base.InitializeInternal(controllerContext);

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.IFormatProvider,System.String,System.Object)", Justification = "CurrentUICulture is to be used by design.")]
    [HttpPost]
    [ClientIgnore]
    [ClientLocationId("178F2962-6A7E-448D-917B-E5E70922382E")]
    public CountResponse FetchPackageResultsCount([FromBody] CountRequest query, string entityType = "package")
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1083079, "REST-API", "REST-API", nameof (FetchPackageResultsCount));
      if (query == null)
        throw new InvalidQueryException(SearchWebApiResources.NullQueryMessage);
      try
      {
        IEntityType entityType1 = (IEntityType) null;
        bool flag = false;
        try
        {
          entityType1 = this.GetEntityObjectFromFactory(this.TfsRequestContext, entityType);
        }
        finally
        {
          if (entityType1 != null)
            flag = true;
        }
        ICountHandler countRequestHandler1;
        bool countRequestHandler2 = this.countHandlerFactory.TryGetCountRequestHandler(entityType1, out countRequestHandler1);
        if (flag & countRequestHandler2)
          return countRequestHandler1.HandleCountRequest(this.TfsRequestContext, query, this.ProjectInfo);
        throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.UnsupportedEntityMesssage, (object) entityType));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1083080, "REST-API", "REST-API", nameof (FetchPackageResultsCount));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    private IEntityType GetEntityObjectFromFactory(
      IVssRequestContext requestContext,
      string entityType)
    {
      return EntityPluginsFactory.GetEntityType(requestContext, entityType);
    }
  }
}
