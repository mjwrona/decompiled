// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.CountController
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
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
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "resultsCount")]
  public class CountController : SearchApiController
  {
    protected ICountHandlerFactory countHandlerFactory { get; set; }

    public CountController() => this.countHandlerFactory = (ICountHandlerFactory) new CountHandlerFactory();

    protected override void InitializeInternal(HttpControllerContext controllerContext) => base.InitializeInternal(controllerContext);

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.IFormatProvider,System.String,System.Object)", Justification = "CurrentUICulture is to be used by design.")]
    [HttpPost]
    [ClientLocationId("D26EF242-F129-40ED-B9DB-B31A792065E0")]
    [PublicProjectRequestRestrictions]
    public CountResponse ResultsCount([FromBody] CountRequest query, [FromUri] string entityType)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1083009, "REST-API", "REST-API", nameof (ResultsCount));
      if (query == null)
        throw new InvalidQueryException(SearchWebApiResources.NullQueryMessage);
      if (string.IsNullOrWhiteSpace(entityType))
        throw new InvalidQueryException(SearchSharedWebApiResources.NullOrEmptyEntityTypeMessage);
      try
      {
        IEntityType entityType1 = (IEntityType) null;
        bool flag = false;
        try
        {
          entityType1 = this.GetEntityObjectFromFactory(this.TfsRequestContext, entityType);
        }
        catch (Exception ex)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1083008, "REST-API", "REST-API", ex);
          throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.UnsupportedEntityMesssage, (object) entityType));
        }
        finally
        {
          if (entityType1 != null)
            flag = true;
        }
        ICountHandler countRequestHandler1;
        bool countRequestHandler2 = this.countHandlerFactory.TryGetCountRequestHandler(entityType1, out countRequestHandler1);
        if (!(flag & countRequestHandler2))
          throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.UnsupportedEntityMesssage, (object) entityType));
        CountResponse response = countRequestHandler1.HandleCountRequest(this.TfsRequestContext, query, this.ProjectInfo);
        this.PopulateSearchSecuredObjectInResponse(this.TfsRequestContext, response);
        return response;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1083010, "REST-API", "REST-API", nameof (ResultsCount));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    public virtual IEntityType GetEntityObjectFromFactory(
      IVssRequestContext requestContext,
      string entityType)
    {
      return EntityPluginsFactory.GetEntityType(this.TfsRequestContext, entityType);
    }

    protected void PopulateSearchSecuredObjectInResponse(
      IVssRequestContext requestContext,
      CountResponse response)
    {
      Guid namespaceId;
      if (!requestContext.TryGetItem<Guid>("searchServiceSecurityNamespaceGuidKey", out namespaceId))
        throw new InvalidOperationException("SecurityNamespaceGuid not found.");
      int requiredPermissions;
      if (!requestContext.TryGetItem<int>("searchServiceSecurityPermissionKey", out requiredPermissions))
        throw new InvalidOperationException("RequiredPermissions not found");
      string token;
      if (!requestContext.TryGetItem<string>("searchServiceSecurityTokenKey", out token) && string.IsNullOrEmpty(token))
        throw new InvalidOperationException("Token not found");
      response.SetSecuredObject(namespaceId, requiredPermissions, token);
    }
  }
}
