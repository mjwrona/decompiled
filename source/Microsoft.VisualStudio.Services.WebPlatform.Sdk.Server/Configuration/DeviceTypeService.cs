// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Configuration.DeviceTypeService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Configuration
{
  public class DeviceTypeService : IDeviceTypeService, IVssFrameworkService
  {
    private DeviceTypeService.ServiceSettings m_settings;
    private static readonly RegistryQuery s_registrySettingsQuery = (RegistryQuery) "/Service/DeviceType/*";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), in DeviceTypeService.s_registrySettingsQuery);
      Interlocked.CompareExchange<DeviceTypeService.ServiceSettings>(ref this.m_settings, new DeviceTypeService.ServiceSettings(systemRequestContext), (DeviceTypeService.ServiceSettings) null);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));

    public bool IsMobileDevice(IVssRequestContext requestContext, bool ignoreBypassCookie = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return (ignoreBypassCookie || !DeviceTypeService.HasBypassMobileCookie(requestContext)) && !string.IsNullOrEmpty(requestContext.UserAgent) && requestContext.UserAgent.IndexOf(this.m_settings.MobileToken, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public bool IsTabletDevice(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (string.IsNullOrEmpty(requestContext.UserAgent))
        return false;
      try
      {
        return this.m_settings.TabletRegEx.IsMatch(requestContext.UserAgent);
      }
      catch (RegexMatchTimeoutException ex)
      {
        return false;
      }
    }

    private static bool HasBypassMobileCookie(IVssRequestContext requestContext)
    {
      HttpRequestBase request = requestContext.WebRequestContextInternal(false)?.HttpContext?.Request;
      return request != null && request.Cookies["VstsBypassMobile"] != null;
    }

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write<DeviceTypeService.ServiceSettings>(ref this.m_settings, new DeviceTypeService.ServiceSettings(requestContext));
    }

    private class ServiceSettings
    {
      private const string c_DefaultTabletRegex = "Tablet|iPad|PlayBook|BB10|Z30|Nexus 10|Nexus 7|GT-P|SCH-I800|Xoom|Kindle|Silk|KFAPWI";
      private const string c_DefaultMobileToken = "mobi";
      public readonly Regex TabletRegEx;
      public readonly string MobileToken;

      public ServiceSettings(IVssRequestContext requestContext)
      {
        RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, DeviceTypeService.s_registrySettingsQuery);
        this.TabletRegEx = new Regex(registryEntryCollection.GetValueFromPath<string>(nameof (TabletRegEx), "Tablet|iPad|PlayBook|BB10|Z30|Nexus 10|Nexus 7|GT-P|SCH-I800|Xoom|Kindle|Silk|KFAPWI"), RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(10.0));
        this.MobileToken = registryEntryCollection.GetValueFromPath<string>(nameof (MobileToken), "mobi");
      }
    }
  }
}
