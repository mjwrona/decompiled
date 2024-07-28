// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Upload.CargoPublishRouteHandler
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using System.Web;
using System.Web.Routing;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Upload
{
  public class CargoPublishRouteHandler : IRouteHandler
  {
    public IHttpHandler GetHttpHandler(RequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = ((VisualStudioServicesApplication) requestContext.HttpContext.ApplicationInstance).VssRequestContext;
      return (IHttpHandler) new CargoPublishHttpHandler(new CargoPackageIngesterBootstrapper(vssRequestContext).Bootstrap(), PackageSizeValidatingHandler.MaxSizeRegistrySettingDefinition.Bootstrap(vssRequestContext), CargoSettings.MaxIngestionManifestLengthSetting.Bootstrap(vssRequestContext));
    }
  }
}
