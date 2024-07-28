// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.WebSdkDataProviderBase
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  public abstract class WebSdkDataProviderBase : IExtensionDataProvider
  {
    public virtual string Name => string.Empty;

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      IWebSdkMetadata metadata = this.GetMetadata(requestContext, providerContext, contribution);
      metadata.CheckPermission(requestContext);
      return this.ProcessData((object) metadata);
    }

    protected virtual IWebSdkMetadata GetMetadata(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      return (IWebSdkMetadata) null;
    }

    protected virtual object ProcessData(object data) => data;
  }
}
