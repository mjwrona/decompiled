// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.OAuth2SettingsService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Authentication.UserAuthentication;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  internal class OAuth2SettingsService : IOAuth2SettingsService, IVssFrameworkService
  {
    private long m_version;
    private IOAuth2CommonSettings m_commonSettings;
    private IDelegatedAuthSettings m_delegatedAuthSettings;
    private IUserAuthSettings m_userAuthSettings;
    private OAuth2SettingsService.SigningKeys m_signingKeys;
    private IS2SAuthSettings m_s2sSettings;
    private IAADAuthSettings m_AADSettings;
    private IJWTSigningSettings m_JWTSigningSettings;
    private bool m_serviceStarted;
    private SemaphoreSlim m_signingKeySemaphore = new SemaphoreSlim(10);
    private const string s_Area = "Authentication";
    private const string s_Layer = "OAuth2SettingsService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      this.m_serviceStarted = true;
      IVssRegistryService service1 = systemRequestContext.GetService<IVssRegistryService>();
      ITeamFoundationStrongBoxService service2 = systemRequestContext.GetService<ITeamFoundationStrongBoxService>();
      service1.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnCommonSettingsChanged), false, (IEnumerable<RegistryQuery>) OAuth2SettingsService.CommonSettings.CommonSettingsRegistryPaths);
      service1.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnDelegatedAuthSettingsChanged), false, (IEnumerable<RegistryQuery>) OAuth2SettingsService.DelegatedAuthSettings.DelegatedAuthRegistryPaths);
      service1.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnUserAuthSettingsChanged), false, (IEnumerable<RegistryQuery>) OAuth2SettingsService.UserAuthSettings.UserAuthRegistryPaths);
      service1.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSigningKeysChanged), in OAuth2SettingsService.SigningKeys.SigningKeysRegistryPath);
      service1.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnS2SSettingsChanged), in OAuth2SettingsService.S2SSettings.S2SSettingsRegistryPath);
      service1.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnAADSettingsChanged), false, (IEnumerable<RegistryQuery>) OAuth2SettingsService.AADSettings.AADSettingsRegistryPaths);
      service2.RegisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnUserAuthCertificatesChanged), "ConfigurationSecrets", (IEnumerable<string>) new string[2]
      {
        ServicingTokenConstants.UserAuthCookieSigningCertificateThumbprint,
        ServicingTokenConstants.SecondaryUserAuthCookieSigningCertificateThumbprint
      });
      service2.RegisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnS2SStrongBoxDrawerChanged), "ConfigurationSecrets", (IEnumerable<string>) new string[2]
      {
        ServicingTokenConstants.OAuthS2SSigningCertThumbprint,
        ServicingTokenConstants.SecondaryOAuthS2SSigningCertThumbprint
      });
      service2.RegisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnJWTSigningStrongBoxDrawerChanged), "ConfigurationSecrets", (IEnumerable<string>) new string[2]
      {
        "JWTSigningCertificateThumbprint",
        "SecondaryJWTSigningCertificateThumbprint"
      });
      Interlocked.CompareExchange<IOAuth2CommonSettings>(ref this.m_commonSettings, (IOAuth2CommonSettings) new OAuth2SettingsService.CommonSettings(systemRequestContext), (IOAuth2CommonSettings) null);
      Interlocked.CompareExchange<IDelegatedAuthSettings>(ref this.m_delegatedAuthSettings, (IDelegatedAuthSettings) new OAuth2SettingsService.DelegatedAuthSettings(systemRequestContext), (IDelegatedAuthSettings) null);
      Interlocked.CompareExchange<IUserAuthSettings>(ref this.m_userAuthSettings, (IUserAuthSettings) new OAuth2SettingsService.UserAuthSettings(systemRequestContext), (IUserAuthSettings) null);
      OAuth2SettingsService.SigningKeys signingKeys1 = new OAuth2SettingsService.SigningKeys(systemRequestContext);
      OAuth2SettingsService.SigningKeys signingKeys2 = Interlocked.CompareExchange<OAuth2SettingsService.SigningKeys>(ref this.m_signingKeys, signingKeys1, (OAuth2SettingsService.SigningKeys) null);
      Interlocked.CompareExchange<IS2SAuthSettings>(ref this.m_s2sSettings, (IS2SAuthSettings) new OAuth2SettingsService.S2SSettings(systemRequestContext), (IS2SAuthSettings) null);
      Interlocked.CompareExchange<IAADAuthSettings>(ref this.m_AADSettings, (IAADAuthSettings) new OAuth2SettingsService.AADSettings(systemRequestContext), (IAADAuthSettings) null);
      Interlocked.CompareExchange<IJWTSigningSettings>(ref this.m_JWTSigningSettings, (IJWTSigningSettings) new OAuth2SettingsService.JWTSigningSettings(systemRequestContext, (IJWTSigningSettings) null), (IJWTSigningSettings) null);
      Interlocked.Increment(ref this.m_version);
      if (signingKeys2 == null)
        return;
      signingKeys1.Dispose();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_serviceStarted = false;
      IVssRegistryService service1 = systemRequestContext.GetService<IVssRegistryService>();
      ITeamFoundationStrongBoxService service2 = systemRequestContext.GetService<ITeamFoundationStrongBoxService>();
      service1.UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnCommonSettingsChanged));
      service1.UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnDelegatedAuthSettingsChanged));
      service1.UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnUserAuthSettingsChanged));
      service1.UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSigningKeysChanged));
      service1.UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnS2SSettingsChanged));
      service1.UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnAADSettingsChanged));
      service2.UnregisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnUserAuthCertificatesChanged));
      service2.UnregisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnS2SStrongBoxDrawerChanged));
      service2.UnregisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnJWTSigningStrongBoxDrawerChanged));
      this.m_signingKeys.Dispose();
      this.m_signingKeys = (OAuth2SettingsService.SigningKeys) null;
    }

    public long GetVersion(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(5555502, "Authentication", nameof (OAuth2SettingsService), nameof (GetVersion));
      try
      {
        requestContext.Trace(555505, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), string.Format("Returning OAuthSettings version: {0}", (object) this.m_version));
        return this.m_version;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5555503, "Authentication", nameof (OAuth2SettingsService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5555504, "Authentication", nameof (OAuth2SettingsService), nameof (GetVersion));
      }
    }

    public bool AreSigningKeysExpired(IVssRequestContext requestContext)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Authentication.EnableExpirationLogic"))
        return DateTime.UtcNow.CompareTo(this.m_signingKeys.ExpirationTime) >= 0;
      requestContext.Trace(951564, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), "Ignoring expiry logic as FF is off");
      return false;
    }

    public IOAuth2CommonSettings GetCommonSettings(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(5555502, "Authentication", nameof (OAuth2SettingsService), nameof (GetCommonSettings));
      try
      {
        requestContext.Trace(5555506, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), "Returning OAuth Common Settings: " + this.m_commonSettings.ToString());
        return this.m_commonSettings;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5555503, "Authentication", nameof (OAuth2SettingsService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5555504, "Authentication", nameof (OAuth2SettingsService), nameof (GetCommonSettings));
      }
    }

    public IDelegatedAuthSettings GetDelegatedAuthSettings(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(5555502, "Authentication", nameof (OAuth2SettingsService), nameof (GetDelegatedAuthSettings));
      try
      {
        return this.m_delegatedAuthSettings;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5555503, "Authentication", nameof (OAuth2SettingsService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5555504, "Authentication", nameof (OAuth2SettingsService), nameof (GetDelegatedAuthSettings));
      }
    }

    public IUserAuthSettings GetUserAuthSettings(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(5555502, "Authentication", nameof (OAuth2SettingsService), nameof (GetUserAuthSettings));
      try
      {
        requestContext.Trace(5555508, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), "Returning OAuth UserAuth Settings: " + this.m_userAuthSettings.ToString());
        return this.m_userAuthSettings;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5555503, "Authentication", nameof (OAuth2SettingsService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5555504, "Authentication", nameof (OAuth2SettingsService), nameof (GetUserAuthSettings));
      }
    }

    public IAADAuthSettings GetAADAuthSettings(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(5555502, "Authentication", nameof (OAuth2SettingsService), nameof (GetAADAuthSettings));
      try
      {
        requestContext.Trace(5555508, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), "Returning OAuth AAD Settings: " + this.m_AADSettings.ToString());
        return this.m_AADSettings;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5555503, "Authentication", nameof (OAuth2SettingsService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5555504, "Authentication", nameof (OAuth2SettingsService), nameof (GetAADAuthSettings));
      }
    }

    public IS2SAuthSettings GetS2SAuthSettings(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(5555502, "Authentication", nameof (OAuth2SettingsService), nameof (GetS2SAuthSettings));
      try
      {
        requestContext.Trace(5555509, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), "Returning OAuth S2S Settings: " + this.m_s2sSettings.ToString());
        return this.m_s2sSettings;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5555503, "Authentication", nameof (OAuth2SettingsService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5555504, "Authentication", nameof (OAuth2SettingsService), nameof (GetS2SAuthSettings));
      }
    }

    public IJWTSigningSettings GetJWTSigningSettings(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(5555502, "Authentication", nameof (OAuth2SettingsService), nameof (GetJWTSigningSettings));
      try
      {
        requestContext.Trace(5555509, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), "Returning JWT Signing Settings: " + this.m_JWTSigningSettings.ToString());
        return this.m_JWTSigningSettings;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5555503, "Authentication", nameof (OAuth2SettingsService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5555504, "Authentication", nameof (OAuth2SettingsService), nameof (GetJWTSigningSettings));
      }
    }

    public IDictionary<string, X509Certificate2> GetRegistryBasedValidationCertificates(
      IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(5555502, "Authentication", nameof (OAuth2SettingsService), nameof (GetRegistryBasedValidationCertificates));
      try
      {
        requestContext.Trace(555511, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), "Returning OAuth validation certificates: " + this.m_signingKeys.ToString());
        return this.m_signingKeys.ValidationCertificatesFromRegistry;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5555503, "Authentication", nameof (OAuth2SettingsService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5555504, "Authentication", nameof (OAuth2SettingsService), nameof (GetRegistryBasedValidationCertificates));
      }
    }

    public bool UpdateUserAuthCertificatesIfExpired(IVssRequestContext requestContext)
    {
      if (!this.AreSigningKeysExpired(requestContext))
        return false;
      requestContext.Trace(911619, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), "User auth signing key expired. Try to update it");
      bool flag = false;
      try
      {
        flag = this.m_signingKeySemaphore.Wait(1);
        if (flag)
        {
          requestContext.Trace(961629, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), "Updating user auth signing key after expiration");
          this.m_signingKeys.UpdateUserAuthCertificates(requestContext);
        }
        else
          requestContext.Trace(961630, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), "Ignoring update since we failed to acquire lock");
      }
      finally
      {
        if (flag)
          this.m_signingKeySemaphore.Release();
      }
      return flag;
    }

    public IDictionary<string, X509Certificate2> GetValidationCertificates(
      IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(114282, "Authentication", nameof (OAuth2SettingsService), nameof (GetValidationCertificates));
      try
      {
        requestContext.Trace(961619, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), "Returning OAuth validation certificates: " + this.m_signingKeys.ToString());
        return this.m_signingKeys.GetValidationCertificates();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(201037, "Authentication", nameof (OAuth2SettingsService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(724704, "Authentication", nameof (OAuth2SettingsService), nameof (GetValidationCertificates));
      }
    }

    private void OnCommonSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "CommonSettings changing. Current Settings Version: " + this.m_version.ToString());
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "Settings Before:" + (this.m_commonSettings?.ToString() ?? "CommonSettings: Empty"));
      Volatile.Write<IOAuth2CommonSettings>(ref this.m_commonSettings, (IOAuth2CommonSettings) new OAuth2SettingsService.CommonSettings(requestContext));
      Interlocked.Increment(ref this.m_version);
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "Settings After:" + this.m_commonSettings.ToString());
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "CommonSettings changed. Current Settings Version: " + this.m_version.ToString());
    }

    private void OnDelegatedAuthSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "DelegatedAuthSettings changing. Current Settings Version: " + this.m_version.ToString());
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "Settings Before:" + (this.m_delegatedAuthSettings?.ToString() ?? "DelegatedAuthSettings: Empty"));
      Volatile.Write<IDelegatedAuthSettings>(ref this.m_delegatedAuthSettings, (IDelegatedAuthSettings) new OAuth2SettingsService.DelegatedAuthSettings(requestContext));
      Interlocked.Increment(ref this.m_version);
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "Settings After:" + this.m_delegatedAuthSettings.ToString());
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "DelegatedAuthSettings changed. Current Settings Version: " + this.m_version.ToString());
    }

    private void OnUserAuthSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "UserAuthSettings changing. Current Settings Version: " + this.m_version.ToString());
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "Settings Before:" + (this.m_userAuthSettings?.ToString() ?? "UserAuthSettings: Empty"));
      Volatile.Write<IUserAuthSettings>(ref this.m_userAuthSettings, (IUserAuthSettings) new OAuth2SettingsService.UserAuthSettings(requestContext));
      Interlocked.Increment(ref this.m_version);
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "Settings After:" + this.m_userAuthSettings.ToString());
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "UserAuthSettings changed. Current Settings Version: " + this.m_version.ToString());
    }

    private void OnUserAuthCertificatesChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      if (!this.m_serviceStarted)
        return;
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "SigningKeys changing. Current Settings Version: " + this.m_version.ToString());
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "Settings Before:" + (this.m_signingKeys?.ToString() ?? "SigningKeys: Empty"));
      OAuth2SettingsService.SigningKeys signingKeys = Interlocked.Exchange<OAuth2SettingsService.SigningKeys>(ref this.m_signingKeys, new OAuth2SettingsService.SigningKeys(requestContext));
      Interlocked.Increment(ref this.m_version);
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "Settings After:" + this.m_signingKeys.ToString());
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "SigningKeys changed. Current Settings Version: " + this.m_version.ToString());
      signingKeys?.Dispose();
    }

    private void OnSigningKeysChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      if (!this.m_serviceStarted)
        return;
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "SigningKeys changing. Current Settings Version: " + this.m_version.ToString());
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "Settings Before:" + (this.m_signingKeys?.ToString() ?? "SigningKeys: Empty"));
      OAuth2SettingsService.SigningKeys signingKeys = Interlocked.Exchange<OAuth2SettingsService.SigningKeys>(ref this.m_signingKeys, new OAuth2SettingsService.SigningKeys(requestContext));
      Interlocked.Increment(ref this.m_version);
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "Settings After:" + this.m_signingKeys.ToString());
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "SigningKeys changed. Current Settings Version: " + this.m_version.ToString());
      signingKeys?.Dispose();
    }

    private void OnS2SSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "S2SSettings changing. Current Settings Version: " + this.m_version.ToString());
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "Settings Before:" + (this.m_s2sSettings?.ToString() ?? "S2SSettings: Empty"));
      Volatile.Write<IS2SAuthSettings>(ref this.m_s2sSettings, (IS2SAuthSettings) new OAuth2SettingsService.S2SSettings(requestContext));
      Interlocked.Increment(ref this.m_version);
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "Settings After:" + this.m_s2sSettings.ToString());
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "S2SSettings changed. Current Settings Version: " + this.m_version.ToString());
    }

    private void OnAADSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "AADSettings changing. Current Settings Version: " + this.m_version.ToString());
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "Settings Before:" + (this.m_AADSettings?.ToString() ?? "AADSettings: Empty"));
      Volatile.Write<IAADAuthSettings>(ref this.m_AADSettings, (IAADAuthSettings) new OAuth2SettingsService.AADSettings(requestContext));
      Interlocked.Increment(ref this.m_version);
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "Settings After:" + this.m_AADSettings.ToString());
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "AADSettings changed. Current Settings Version: " + this.m_version.ToString());
    }

    private void OnS2SStrongBoxDrawerChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "S2SSettings changing. Current Settings Version: " + this.m_version.ToString());
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "Settings Before:" + (this.m_s2sSettings?.ToString() ?? "S2SSettings: Empty"));
      Volatile.Write<IS2SAuthSettings>(ref this.m_s2sSettings, (IS2SAuthSettings) new OAuth2SettingsService.S2SSettings(requestContext));
      Interlocked.Increment(ref this.m_version);
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "Settings After:" + this.m_s2sSettings.ToString());
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "S2SSettings changed. Current Settings Version: " + this.m_version.ToString());
    }

    internal void OnJWTSigningStrongBoxDrawerChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "JWTSigningSettings changing. Current Settings Version: " + this.m_version.ToString());
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "Settings Before:" + (this.m_JWTSigningSettings?.ToString() ?? "JWTSigningSettings: Empty"));
      Volatile.Write<IJWTSigningSettings>(ref this.m_JWTSigningSettings, (IJWTSigningSettings) new OAuth2SettingsService.JWTSigningSettings(requestContext, this.m_JWTSigningSettings));
      Interlocked.Increment(ref this.m_version);
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "Settings After:" + this.m_JWTSigningSettings.ToString());
      requestContext.TraceAlways(5555501, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), (string[]) null, "JWTSigningSettings changed. Current Settings Version: " + this.m_version.ToString());
    }

    private static X509Certificate2 LoadCertFromStrongbox(
      IVssRequestContext requestContext,
      string drawerName,
      string thumbprint)
    {
      X509Certificate2 x509Certificate2 = (X509Certificate2) null;
      if (!string.IsNullOrEmpty(thumbprint))
      {
        ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
        Guid drawerId = service.UnlockDrawer(requestContext, drawerName, false);
        if (drawerId != Guid.Empty && service.GetItemInfo(requestContext, drawerId, thumbprint, false) != null)
          x509Certificate2 = service.RetrieveFileAsCertificate(requestContext, drawerId, thumbprint);
      }
      return x509Certificate2;
    }

    private static string[] SplitStringOrEmpty(string stringToSplit, char delimiter)
    {
      if (string.IsNullOrEmpty(stringToSplit))
        return Array.Empty<string>();
      return stringToSplit.Split(delimiter);
    }

    private class CommonSettings : IOAuth2CommonSettings
    {
      internal static readonly RegistryQuery[] CommonSettingsRegistryPaths = new RegistryQuery[2]
      {
        (RegistryQuery) OAuth2RegistryConstants.ClockSkewInSeconds,
        (RegistryQuery) OAuth2RegistryConstants.AllowedAudiences
      };
      private const int DefaultClockSkew = 300;
      private static readonly string s_registryPath = OAuth2RegistryConstants.Root + "/*";

      internal CommonSettings(IVssRequestContext requestContext)
      {
        RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) OAuth2SettingsService.CommonSettings.s_registryPath);
        this.ClockSkewInSeconds = registryEntryCollection.GetValueFromPath<int>(OAuth2RegistryConstants.ClockSkewInSeconds, 300);
        string valueFromPath = registryEntryCollection.GetValueFromPath<string>(OAuth2RegistryConstants.AllowedAudiences, (string) null);
        this.AllowedAudiences = OAuth2SettingsService.CommonSettings.InitializeAudiences(requestContext, valueFromPath);
      }

      public override string ToString() => "CommonSettings: ClockSkewInSeconds=" + this.ClockSkewInSeconds.ToString() + ", AllowedAudiences={" + string.Join(",", this.AllowedAudiences) + "}";

      public int ClockSkewInSeconds { get; private set; }

      public IEnumerable<string> AllowedAudiences { get; private set; }

      private static IEnumerable<string> InitializeAudiences(
        IVssRequestContext requestContext,
        string allowedAudiencesString)
      {
        string[] strArray = (string[]) null;
        if (!string.IsNullOrEmpty(allowedAudiencesString))
          strArray = allowedAudiencesString.Split('|');
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          string publicAccessMapping = new Uri(requestContext.GetService<ILocationService>().GetAccessMapping(requestContext, AccessMappingConstants.PublicAccessMappingMoniker).AccessPoint).Host;
          string str = requestContext.To(TeamFoundationHostType.Deployment).ServiceHost.InstanceId.ToString();
          if (strArray == null)
            strArray = new string[2]
            {
              publicAccessMapping,
              str
            };
          else if (!((IEnumerable<string>) strArray).Any<string>((Func<string, bool>) (x => string.Equals(x, publicAccessMapping))))
            strArray = ((IEnumerable<string>) strArray).Concat<string>((IEnumerable<string>) new string[2]
            {
              publicAccessMapping,
              str
            }).ToArray<string>();
          else
            strArray = ((IEnumerable<string>) strArray).Concat<string>((IEnumerable<string>) new string[1]
            {
              str
            }).ToArray<string>();
        }
        return (IEnumerable<string>) strArray;
      }
    }

    private class DelegatedAuthSettings : IDelegatedAuthSettings
    {
      internal static readonly RegistryQuery[] DelegatedAuthRegistryPaths = new RegistryQuery[3]
      {
        (RegistryQuery) OAuth2RegistryConstants.ClientAuthEnabled,
        (RegistryQuery) (OAuth2RegistryConstants.TrustedIssuers + "/..."),
        (RegistryQuery) OAuth2RegistryConstants.IsDeploymentLevelOnlyService
      };

      internal DelegatedAuthSettings(IVssRequestContext requestContext)
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        this.Enabled = service.GetValue<bool>(requestContext, (RegistryQuery) OAuth2RegistryConstants.ClientAuthEnabled, false, false);
        RegistryEntryCollection entries = service.ReadEntries(requestContext, OAuth2SettingsService.DelegatedAuthSettings.DelegatedAuthRegistryPaths[1]);
        this.TrustedIssuers = (IEnumerable<string>) OAuth2SettingsService.DelegatedAuthSettings.InitializeTrustedIssuers(requestContext, entries);
        this.IsDeploymentLevelOnlyService = service.GetValue<bool>(requestContext, (RegistryQuery) OAuth2RegistryConstants.IsDeploymentLevelOnlyService, false, false);
      }

      public override string ToString() => "DelegatedAuthSettings: TrustedIssuers={" + string.Join(",", this.TrustedIssuers) + "}";

      public bool Enabled { get; private set; }

      public bool IsDeploymentLevelOnlyService { get; private set; }

      public IEnumerable<string> TrustedIssuers { get; private set; }

      internal static IList<string> InitializeTrustedIssuers(
        IVssRequestContext requestContext,
        RegistryEntryCollection entries)
      {
        IList<string> source = (IList<string>) new List<string>();
        foreach (RegistryEntry entry in entries)
        {
          if (string.Equals(entry.Value, typeof (ClientAuthTokenValidator).FullName, StringComparison.Ordinal))
            source.Add(entry.Name);
        }
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          string publicAccessMapping = new Uri(requestContext.GetService<ILocationService>().GetAccessMapping(requestContext, AccessMappingConstants.PublicAccessMappingMoniker).AccessPoint).Host;
          if (source.Count == 0 || !source.Any<string>((Func<string, bool>) (x => string.Equals(x, publicAccessMapping))))
            source.Add(publicAccessMapping);
          string str = requestContext.To(TeamFoundationHostType.Deployment).ServiceHost.InstanceId.ToString();
          source.Add(str);
        }
        return source;
      }
    }

    private class UserAuthSettings : IUserAuthSettings
    {
      internal static readonly RegistryQuery[] UserAuthRegistryPaths = new RegistryQuery[2]
      {
        (RegistryQuery) "/Configuration/UserAuthentication/**",
        (RegistryQuery) (OAuth2RegistryConstants.TrustedIssuers + "/...")
      };

      internal UserAuthSettings(IVssRequestContext requestContext)
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        UserAuthenticationConfiguration configuration = vssRequestContext.GetService<IUserAuthenticationConfigurationService>().GetConfiguration(vssRequestContext);
        this.Enabled = true;
        this.Audience = configuration.Audience;
        RegistryEntryCollection entries = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, OAuth2SettingsService.UserAuthSettings.UserAuthRegistryPaths[1]);
        this.TrustedIssuers = (IEnumerable<string>) OAuth2SettingsService.DelegatedAuthSettings.InitializeTrustedIssuers(requestContext, entries);
      }

      public override string ToString() => "UserAuthSettings: Enabled=" + this.Enabled.ToString() + ", Audience=" + this.Audience + ", TrustedIssuers={" + string.Join(",", this.TrustedIssuers) + "}";

      public bool Enabled { get; private set; }

      public string Audience { get; private set; }

      public IEnumerable<string> TrustedIssuers { get; private set; }
    }

    private class SigningKeys : IDisposable
    {
      internal static readonly RegistryQuery SigningKeysRegistryPath = (RegistryQuery) (OAuth2RegistryConstants.SigningKeys + "/...");
      internal DateTime ExpirationTime;
      private const double DefaultSigningKeyExpiryTTL = 60.0;
      private bool disposedValue;
      internal readonly IDictionary<string, X509Certificate2> ValidationCertificatesFromRegistry;
      internal IDictionary<string, X509Certificate2> UserAuthValidationCertificates;

      internal SigningKeys(IVssRequestContext requestContext)
      {
        RegistryEntryCollection signingKeys = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, OAuth2SettingsService.SigningKeys.SigningKeysRegistryPath);
        this.ValidationCertificatesFromRegistry = OAuth2SettingsService.SigningKeys.InitializeCertificates(requestContext, (IEnumerable<RegistryEntry>) signingKeys);
        this.UpdateUserAuthCertificates(requestContext);
      }

      internal void UpdateUserAuthCertificates(IVssRequestContext requestContext)
      {
        this.UserAuthValidationCertificates = OAuth2SettingsService.SigningKeys.GetUserAuthCertificatesFromStrongBox(requestContext.To(TeamFoundationHostType.Deployment));
        this.ExpirationTime = DateTime.UtcNow.AddMinutes(this.GetSigningKeyTTLInMinutes(requestContext));
      }

      private double GetSigningKeyTTLInMinutes(IVssRequestContext requestContext)
      {
        RegistryEntryCollection source = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) OAuth2RegistryConstants.OAuthSigningKeyExpiryInMinutes);
        return source.Count <= 0 ? 60.0 : source.First<RegistryEntry>().GetValue<double>();
      }

      public override string ToString() => "SigningKey thumbprints: {" + string.Join(",", (IEnumerable<string>) this.GetValidationCertificates().Keys) + "}";

      public IDictionary<string, X509Certificate2> GetValidationCertificates() => (IDictionary<string, X509Certificate2>) this.ValidationCertificatesFromRegistry.Concat<KeyValuePair<string, X509Certificate2>>((IEnumerable<KeyValuePair<string, X509Certificate2>>) this.UserAuthValidationCertificates).ToLookup<KeyValuePair<string, X509Certificate2>, string, X509Certificate2>((Func<KeyValuePair<string, X509Certificate2>, string>) (x => x.Key), (Func<KeyValuePair<string, X509Certificate2>, X509Certificate2>) (x => x.Value)).ToDictionary<IGrouping<string, X509Certificate2>, string, X509Certificate2>((Func<IGrouping<string, X509Certificate2>, string>) (x => x.Key), (Func<IGrouping<string, X509Certificate2>, X509Certificate2>) (g => g.First<X509Certificate2>()));

      protected virtual void Dispose(bool disposing)
      {
        if (this.disposedValue)
          return;
        if (disposing)
        {
          foreach (X509Certificate x509Certificate in (IEnumerable<X509Certificate2>) this.ValidationCertificatesFromRegistry.Values)
            x509Certificate.Reset();
          this.ValidationCertificatesFromRegistry.Clear();
        }
        this.disposedValue = true;
      }

      public void Dispose() => this.Dispose(true);

      private static IDictionary<string, X509Certificate2> InitializeCertificates(
        IVssRequestContext requestContext,
        IEnumerable<RegistryEntry> signingKeys)
      {
        Dictionary<string, X509Certificate2> dictionary = new Dictionary<string, X509Certificate2>();
        Regex regex = new Regex("^([0-9A-F][0-9A-F])+$");
        foreach (RegistryEntry signingKey in signingKeys)
        {
          try
          {
            X509Certificate2 x509Certificate2 = new X509Certificate2(Convert.FromBase64String(signingKey.Value));
            string name = signingKey.Name;
            Match match = regex.Match(name);
            if (match.Success)
            {
              string base64StringNoPadding = match.Groups[1].Captures.OfType<System.Text.RegularExpressions.Capture>().Select<System.Text.RegularExpressions.Capture, byte>((Func<System.Text.RegularExpressions.Capture, byte>) (group => Convert.ToByte(group.Value, 16))).ToArray<byte>().ToBase64StringNoPadding();
              dictionary.Add(base64StringNoPadding, x509Certificate2);
            }
          }
          catch (Exception ex)
          {
            requestContext.Trace(5555512, TraceLevel.Error, "Authentication", nameof (OAuth2SettingsService), "Exception trying to load OAuth validation certificate {0} from registry", (object) signingKey.Name);
            requestContext.TraceException(5555512, "Authentication", nameof (OAuth2SettingsService), ex);
          }
        }
        return (IDictionary<string, X509Certificate2>) dictionary;
      }

      private static IDictionary<string, X509Certificate2> GetUserAuthCertificatesFromStrongBox(
        IVssRequestContext deploymentContext)
      {
        Dictionary<string, X509Certificate2> certificatesFromStrongBox = new Dictionary<string, X509Certificate2>();
        IVssRequestContext vssRequestContext = deploymentContext.Elevate();
        ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
        string layer = nameof (GetUserAuthCertificatesFromStrongBox);
        try
        {
          List<string> lookupKeys = new List<string>()
          {
            ServicingTokenConstants.UserAuthCookieSigningCertificateThumbprint,
            ServicingTokenConstants.SecondaryUserAuthCookieSigningCertificateThumbprint
          };
          foreach (X509Certificate2 certificate in (IEnumerable<X509Certificate2>) service.RetrieveFilesAsCertificate(vssRequestContext, "ConfigurationSecrets", (IList<string>) lookupKeys))
          {
            if (certificate != null)
            {
              string base64StringNoPadding = certificate.GetCertHash().ToBase64StringNoPadding();
              if (!certificatesFromStrongBox.ContainsKey(base64StringNoPadding))
                certificatesFromStrongBox.Add(base64StringNoPadding, new X509Certificate2((X509Certificate) certificate));
            }
          }
        }
        catch (StrongBoxDrawerNotFoundException ex)
        {
          vssRequestContext.TraceException(797994, "Authentication", layer, (Exception) ex);
        }
        catch (StrongBoxItemNotFoundException ex)
        {
          vssRequestContext.TraceException(905152, "Authentication", layer, (Exception) ex);
        }
        return (IDictionary<string, X509Certificate2>) certificatesFromStrongBox;
      }
    }

    private class S2SSettings : IS2SAuthSettings
    {
      internal static readonly RegistryQuery S2SSettingsRegistryPath = (RegistryQuery) (OAuth2RegistryConstants.S2SRoot + "/...");
      private bool m_useSecondarySigningCertificate;
      private X509Certificate2 m_certificate;
      private X509Certificate2 m_secondaryCertificate;
      private readonly List<string> _firstPartyServicePrincipals;
      private static readonly List<string> _hardcodedFirstPartyServicePrincipals = new List<string>()
      {
        OAuth2RegistryConstants.FirstPartyProdAADApplicationId,
        OAuth2RegistryConstants.FirstPartyIntAADApplicationId
      };

      internal S2SSettings(IVssRequestContext requestContext)
      {
        RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, OAuth2SettingsService.S2SSettings.S2SSettingsRegistryPath);
        this.Enabled = registryEntryCollection.GetValueFromPath<bool>(OAuth2RegistryConstants.S2SAuthEnabled, false);
        this.TenantDomain = registryEntryCollection.GetValueFromPath<string>(OAuth2RegistryConstants.S2STenantDomain, (string) null);
        this.FirstPartyTenantDomain = registryEntryCollection.GetValueFromPath<string>(OAuth2RegistryConstants.FirstPartyS2STenantDomain, (string) null);
        if (this.FirstPartyTenantDomain == null)
          this.FirstPartyTenantDomain = registryEntryCollection.GetValueFromPath<string>(OAuth2RegistryConstants.S2SRoot + "/FirstyPartyS2STenantDomain", (string) null);
        this.TenantId = registryEntryCollection.GetValueFromPath<string>(OAuth2RegistryConstants.S2STenantId, (string) null);
        this.FirstPartyTenantId = registryEntryCollection.GetValueFromPath<string>(OAuth2RegistryConstants.FirstPartyS2STenantId, (string) null);
        this.Issuer = registryEntryCollection.GetValueFromPath<string>(OAuth2RegistryConstants.S2SIssuer, string.Format(OAuth2RegistryConstants.S2SDefaultIssuer, (object) this.TenantId));
        this.FirstPartyIssuer = registryEntryCollection.GetValueFromPath<string>(OAuth2RegistryConstants.FirstPartyS2SIssuer, string.Format(OAuth2RegistryConstants.S2SDefaultIssuer, (object) this.FirstPartyTenantId));
        this.IssuanceEndpoint = new Uri(registryEntryCollection.GetValueFromPath<string>(OAuth2RegistryConstants.S2SIssuanceEndpoint, string.Format(OAuth2RegistryConstants.S2SDefaultIssuanceEndpoint, (object) this.TenantId)));
        this.PrimaryServicePrincipal = registryEntryCollection.GetValueFromPath<Guid>(OAuth2RegistryConstants.S2SPrimaryServicePrincipal, Guid.Empty);
        this.DisableAADTestSlice = registryEntryCollection.GetValueFromPath<bool>(OAuth2RegistryConstants.S2SDisableAADTestSlice, true);
        this.m_useSecondarySigningCertificate = registryEntryCollection.GetValueFromPath<bool>(OAuth2RegistryConstants.S2SUseSecondarySigningCertificate, false);
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && this.m_useSecondarySigningCertificate)
        {
          int valueFromPath = registryEntryCollection.GetValueFromPath<int>(OAuth2RegistryConstants.S2SSwitchToPrimaryAfter, 60);
          Guid guid = requestContext.GetService<ITeamFoundationJobService>().QueueOneTimeJob(requestContext, "Switch to Primary S2S Certificate Job", "Microsoft.VisualStudio.Services.Cloud.SwitchToPrimaryS2SCertificateJob", (XmlNode) null, JobPriorityLevel.Highest, JobPriorityClass.High, TimeSpan.FromMinutes((double) valueFromPath));
          requestContext.TraceAlways(6666664, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), "Job '{0}' will switch us back to using the primary S2S signing certificate in {1} minutes.", (object) guid, (object) valueFromPath);
        }
        List<string> stringList = new List<string>();
        foreach (string servicePrincipal in OAuth2SettingsService.S2SSettings._hardcodedFirstPartyServicePrincipals)
          stringList.Add(servicePrincipal);
        Guid result1;
        if (Guid.TryParse(this.FirstPartyTenantId, out result1))
        {
          IVssRequestContext context = requestContext.Elevate().To(TeamFoundationHostType.Deployment);
          IVssSecuritySubjectService securitySubjectService = (IVssSecuritySubjectService) null;
          try
          {
            securitySubjectService = context.GetService<IVssSecuritySubjectService>();
          }
          catch (HostShutdownException ex)
          {
            requestContext.TraceException(6666665, "Authentication", "Authentication", (Exception) ex);
          }
          if (securitySubjectService != null)
          {
            foreach (SecuritySubjectEntry securitySubjectEntry in securitySubjectService.GetSecuritySubjectEntries(requestContext))
            {
              if (securitySubjectEntry.SubjectType == SecuritySubjectType.ServicePrincipal)
              {
                string[] strArray = securitySubjectEntry.Identifier.Split('@');
                Guid result2;
                if (strArray.Length == 2 && Guid.TryParse(strArray[1], out result2) && result1 == result2)
                {
                  string str = securitySubjectEntry.Id.ToString();
                  stringList.Add(str);
                }
              }
            }
          }
        }
        this._firstPartyServicePrincipals = stringList;
      }

      public override string ToString() => "S2SSettings: Enabled=" + this.Enabled.ToString() + ", TenantDomain=" + this.TenantDomain + ", Issuer=" + this.Issuer + ", IssuanceEndpoint=" + this.IssuanceEndpoint.ToString() + ", PrimaryServicePrincipal=" + this.PrimaryServicePrincipal.ToString("D") + ", DisableAADTestSlice=" + this.DisableAADTestSlice.ToString();

      public bool Enabled { get; private set; }

      public string TenantDomain { get; private set; }

      public string FirstPartyTenantDomain { get; private set; }

      public string TenantId { get; private set; }

      public string FirstPartyTenantId { get; private set; }

      public string Issuer { get; private set; }

      public string FirstPartyIssuer { get; private set; }

      public Uri IssuanceEndpoint { get; private set; }

      public Guid PrimaryServicePrincipal { get; private set; }

      public bool DisableAADTestSlice { get; private set; }

      public X509Certificate2 GetSigningCertificate(IVssRequestContext requestContext)
      {
        if (this.m_certificate == null)
        {
          Interlocked.CompareExchange<X509Certificate2>(ref this.m_certificate, OAuth2SettingsService.LoadCertFromStrongbox(requestContext, "ConfigurationSecrets", ServicingTokenConstants.OAuthS2SSigningCertThumbprint), (X509Certificate2) null);
          Interlocked.CompareExchange<X509Certificate2>(ref this.m_secondaryCertificate, OAuth2SettingsService.LoadCertFromStrongbox(requestContext, "ConfigurationSecrets", ServicingTokenConstants.SecondaryOAuthS2SSigningCertThumbprint), (X509Certificate2) null);
        }
        if (this.m_useSecondarySigningCertificate)
        {
          if (this.m_secondaryCertificate != null)
          {
            requestContext.Trace(6666666, TraceLevel.Error, "Authentication", nameof (OAuth2SettingsService), "We are using the secondary S2S signing certificate. This happens when the primary certificate has been consistently failing. Do not rotate OAuthS2SSigningCertThumbprint until all scale units are using the primary certificate.");
            return this.m_secondaryCertificate;
          }
          requestContext.Trace(6666668, TraceLevel.Info, "Authentication", nameof (OAuth2SettingsService), "The secondary S2S signing certificate does not exist in '{0}'.", (object) "ConfigurationSecrets");
        }
        return this.m_certificate;
      }

      public IEnumerable<string> FirstPartyServicePrincipals => (IEnumerable<string>) this._firstPartyServicePrincipals;
    }

    private class AADSettings : IAADAuthSettings
    {
      internal static RegistryQuery[] AADSettingsRegistryPaths = new RegistryQuery[2]
      {
        (RegistryQuery) (OAuth2RegistryConstants.AADRoot + "/..."),
        (RegistryQuery) (OAuth2RegistryConstants.TrustedIssuers + "/...")
      };

      internal AADSettings(IVssRequestContext requestContext)
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        RegistryEntryCollection registryEntryCollection = service.ReadEntries(requestContext, OAuth2SettingsService.AADSettings.AADSettingsRegistryPaths[0]);
        this.Enabled = registryEntryCollection.GetValueFromPath<bool>(OAuth2RegistryConstants.AADAuthEnabled, false);
        this.Authority = registryEntryCollection.GetValueFromPath<string>(OAuth2RegistryConstants.AADAuthority, string.Empty);
        this.CreateAADTenant = registryEntryCollection.GetValueFromPath<bool>(OAuth2RegistryConstants.AADCreateTenants, false);
        this.AADGraphResource = registryEntryCollection.GetValueFromPath<string>(OAuth2RegistryConstants.AADGraphResource, OAuth2RegistryConstants.AADDefaultGraphResource);
        this.BlockedAADAppIds = (IEnumerable<string>) OAuth2SettingsService.SplitStringOrEmpty(registryEntryCollection.GetValueFromPath<string>(OAuth2RegistryConstants.BlockedAADAppIds, string.Empty), '|');
        this.MsaPassthroughBlockedAADAppIds = (IEnumerable<string>) OAuth2SettingsService.SplitStringOrEmpty(registryEntryCollection.GetValueFromPath<string>(OAuth2RegistryConstants.MsaPassthroughBlockedAADAppIds, string.Empty), '|');
        this.OnBehalfOfAllowedAADAppIds = (IEnumerable<string>) OAuth2SettingsService.SplitStringOrEmpty(registryEntryCollection.GetValueFromPath<string>(OAuth2RegistryConstants.OnBehalfOfAllowedAADAppIds, string.Empty), '|');
        foreach (RegistryEntry readEntry in service.ReadEntries(requestContext, OAuth2SettingsService.AADSettings.AADSettingsRegistryPaths[1]))
        {
          if (string.Equals(readEntry.Value, typeof (AADAuthTokenValidator).FullName, StringComparison.Ordinal))
            this.Issuer = readEntry.Name;
        }
      }

      public override string ToString() => "AADSettings: Enabled=" + this.Enabled.ToString() + ", Authority=" + this.Authority + ", Issuer=" + this.Issuer + ", CreateAADTenant=" + this.CreateAADTenant.ToString() + ", BlockedAADAppIds={" + string.Join(",", this.BlockedAADAppIds) + "}, MsaPassthroughBlockedAADAppIds={" + string.Join(",", this.MsaPassthroughBlockedAADAppIds) + "}, OnBehalfOfAllowedAADAppIds={" + string.Join(",", this.OnBehalfOfAllowedAADAppIds) + "}, AADGraphResource=" + this.AADGraphResource;

      public bool Enabled { get; private set; }

      public string Authority { get; private set; }

      public string Issuer { get; private set; }

      public bool CreateAADTenant { get; private set; }

      public IEnumerable<string> BlockedAADAppIds { get; private set; }

      public IEnumerable<string> MsaPassthroughBlockedAADAppIds { get; private set; }

      public IEnumerable<string> OnBehalfOfAllowedAADAppIds { get; private set; }

      public string AADGraphResource { get; private set; }
    }

    internal class JWTSigningSettings : IJWTSigningSettings
    {
      internal JWTSigningSettings(
        IVssRequestContext requestContext,
        IJWTSigningSettings oldSettings)
      {
        if (oldSettings == null)
          this.ReadJWTSettings(requestContext, false);
        else
          this.UpdateJWTSetings(requestContext, oldSettings);
      }

      protected string GetCertificateValues(
        IVssRequestContext requestContext,
        ITeamFoundationStrongBoxService strongBox,
        Guid drawerId,
        string lookupKey)
      {
        X509Certificate2 x509Certificate2 = strongBox.RetrieveFileAsCertificate(requestContext, drawerId, lookupKey);
        if (x509Certificate2 != null)
        {
          string base64String = Convert.ToBase64String(x509Certificate2.Export(X509ContentType.Cert));
          this.SigningKeys.Add(new KeyValuePair<string, string>(x509Certificate2.Thumbprint, base64String));
        }
        return x509Certificate2?.Thumbprint;
      }

      protected void ReadJWTSettings(IVssRequestContext requestContext, bool throwOnFailure)
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        this.JWTSigningThumbprint = service.GetValue(requestContext, (RegistryQuery) "/Service/DelegatedAuthorization/JWTSigningCertificateThumbprint", false, (string) null);
        this.SecondaryJWTSigningThumbrint = service.GetValue(requestContext, (RegistryQuery) "/Service/DelegatedAuthorization/SecondaryJWTSigningCertificateThumbprint", false, (string) null);
        this.SigningCertificateThumbprint = service.GetValue(requestContext, (RegistryQuery) "/Service/DelegatedAuthorization/SigningCertificateThumbprint", false, (string) null);
      }

      protected void ReadJWTSettingsFromStrongBox(IVssRequestContext requestContext)
      {
        ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
        Guid drawer = service.UnlockOrCreateDrawer(requestContext, "ConfigurationSecrets");
        this.JWTSigningThumbprint = this.GetCertificateValues(requestContext, service, drawer, "JWTSigningCertificateThumbprint");
        this.SecondaryJWTSigningThumbrint = this.GetCertificateValues(requestContext, service, drawer, "SecondaryJWTSigningCertificateThumbprint");
      }

      protected void UpdateJWTSetings(
        IVssRequestContext requestContext,
        IJWTSigningSettings oldSettings,
        bool throwOnFailure = true)
      {
        this.ReadJWTSettingsFromStrongBox(requestContext);
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        HashSet<string> signingThumbprints1 = oldSettings.JWTSigningThumbprints;
        HashSet<string> signingThumbprints2 = this.JWTSigningThumbprints;
        string signingThumbprint = this.JWTSigningThumbprint;
        string signingThumbrint = this.SecondaryJWTSigningThumbrint;
        this.ReadJWTSettings(requestContext, true);
        if (signingThumbprint != null && signingThumbprint == this.JWTSigningThumbprint && signingThumbrint != null && signingThumbrint == this.SecondaryJWTSigningThumbrint)
          return;
        IList<RegistryEntry> registryEntryList = (IList<RegistryEntry>) new List<RegistryEntry>();
        signingThumbprints2.ExceptWith((IEnumerable<string>) signingThumbprints1);
        if (signingThumbprints2.Count == 1)
          registryEntryList.Add(new RegistryEntry(OAuth2RegistryConstants.SigningKeys + "/" + signingThumbprints2.ElementAt<string>(0), this.SigningKeys[signingThumbprints2.ElementAt<string>(0)]));
        signingThumbprints1.ExceptWith((IEnumerable<string>) this.JWTSigningThumbprints);
        if (signingThumbprints1.Count == 1 && this.SigningCertificateThumbprint != signingThumbprints1.ElementAt<string>(0))
          registryEntryList.Add(new RegistryEntry(OAuth2RegistryConstants.SigningKeys + "/" + signingThumbprints1.ElementAt<string>(0), (string) null));
        if (signingThumbprint != null && signingThumbprint != oldSettings.JWTSigningThumbprint)
          registryEntryList.Add(new RegistryEntry("/Service/DelegatedAuthorization/JWTSigningCertificateThumbprint", signingThumbprint));
        if (signingThumbrint != null && signingThumbrint != oldSettings.SecondaryJWTSigningThumbrint)
          registryEntryList.Add(new RegistryEntry("/Service/DelegatedAuthorization/SecondaryJWTSigningCertificateThumbprint", signingThumbrint));
        if (registryEntryList.Count <= 0)
          return;
        service.UpdateOrDeleteEntries(requestContext, (IEnumerable<RegistryEntry>) registryEntryList);
      }

      public override string ToString() => "JWTSigningSettings: JWTSigningThumbprint=" + this.JWTSigningThumbprint + ", SecondaryJWTSigningThumbrint=" + this.SecondaryJWTSigningThumbrint;

      private IDictionary<string, string> SigningKeys { get; set; } = (IDictionary<string, string>) new Dictionary<string, string>();

      public string JWTSigningThumbprint { get; private set; }

      public string SecondaryJWTSigningThumbrint { get; private set; }

      private string SigningCertificateThumbprint { get; set; }

      public HashSet<string> JWTSigningThumbprints
      {
        get
        {
          HashSet<string> signingThumbprints = new HashSet<string>();
          if (this.JWTSigningThumbprint != null)
            signingThumbprints.Add(this.JWTSigningThumbprint);
          if (this.SecondaryJWTSigningThumbrint != null)
            signingThumbprints.Add(this.SecondaryJWTSigningThumbrint);
          return signingThumbprints;
        }
      }
    }
  }
}
