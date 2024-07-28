// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.XslNotificationTemplateTypeCache
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class XslNotificationTemplateTypeCache : 
    VssMemoryCacheService<string, bool>,
    INotificationTemplateTypeCache,
    IVssFrameworkService
  {
    private static readonly TimeSpan s_maxCacheLife = TimeSpan.FromHours(6.0);
    private static readonly TimeSpan s_maxCacheInactivityAge = TimeSpan.FromMinutes(15.0);
    private static readonly TimeSpan s_cacheCleanupInterval = TimeSpan.FromMinutes(5.0);

    public XslNotificationTemplateTypeCache()
      : base(XslNotificationTemplateTypeCache.s_cacheCleanupInterval)
    {
      this.InactivityInterval.Value = XslNotificationTemplateTypeCache.s_maxCacheInactivityAge;
      this.ExpiryInterval.Value = XslNotificationTemplateTypeCache.s_maxCacheLife;
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      base.ServiceStart(requestContext);
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext) => base.ServiceEnd(requestContext);

    public bool HasXslTemplate(
      IVssRequestContext requestContext,
      string searchPath,
      string xslFileName)
    {
      bool flag = false;
      return this.TryGetValue(requestContext, xslFileName, out flag) ? flag : this.SetXslTemplate(requestContext, searchPath, xslFileName);
    }

    private bool SetXslTemplate(
      IVssRequestContext requestContext,
      string searchPath,
      string xslFileName)
    {
      bool flag = false;
      string path2 = requestContext.ServiceHost.GetCulture(requestContext).LCID.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      if (File.Exists(Path.Combine(searchPath, path2, xslFileName)))
        flag = true;
      if (File.Exists(Path.Combine(searchPath, xslFileName)))
        flag = true;
      this.Set(requestContext, xslFileName, flag);
      return flag;
    }
  }
}
