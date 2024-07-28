// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Mru.IdentityMruSettings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Mru
{
  internal class IdentityMruSettings : IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Configuration/Identity/IdentityMru/MruService/...");
      this.ReloadSettings(requestContext);
    }

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    private void OnRegistryChanged(
      IVssRequestContext context,
      RegistryEntryCollection changedEntries)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<RegistryEntryCollection>(changedEntries, nameof (changedEntries));
      if (!changedEntries.Any<RegistryEntry>())
        return;
      this.ReloadSettings(context);
    }

    private void ReloadSettings(IVssRequestContext context)
    {
      IVssRegistryService service = context.GetService<IVssRegistryService>();
      this.Settings = new IdentityMruSettings.IdentityMruServiceSettings()
      {
        MaxMruSize = service.GetValue<int>(context, (RegistryQuery) "/Configuration/Identity/IdentityMru/MruService/MruMaxSizeKey", 50)
      };
    }

    internal IdentityMruSettings.IdentityMruServiceSettings Settings { get; private set; }

    internal class IdentityMruServiceSettings
    {
      internal int MaxMruSize { get; set; }
    }
  }
}
