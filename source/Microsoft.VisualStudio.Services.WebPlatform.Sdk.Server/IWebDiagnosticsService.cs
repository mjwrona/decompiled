// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.IWebDiagnosticsService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  [DefaultServiceImplementation(typeof (WebDiagnosticsService))]
  public interface IWebDiagnosticsService : IVssFrameworkService
  {
    bool IsBundlingEnabled(IVssRequestContext requestContext);

    bool IsDebugContentEnabled(IVssRequestContext requestContext);

    bool IsCdnAvailable(IVssRequestContext requestContext);

    bool IsCdnEnabled(IVssRequestContext requestContext);

    bool IsTracePointCollectionEnabled(IVssRequestContext requestContext);

    bool IsRequestDiagnosticsEnabled(IVssRequestContext requestContext);

    bool SupportsESNextScripts(IVssRequestContext requestContext);
  }
}
