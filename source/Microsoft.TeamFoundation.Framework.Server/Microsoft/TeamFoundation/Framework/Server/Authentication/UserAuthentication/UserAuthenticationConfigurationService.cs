// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.UserAuthentication.UserAuthenticationConfigurationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication.UserAuthentication
{
  public class UserAuthenticationConfigurationService : 
    IUserAuthenticationConfigurationService,
    IVssFrameworkService
  {
    private const string s_area = "UserAuthentication";
    private const string s_layer = "IUserAuthenticationConfigurationService";
    private const int DefaultTimeSpan = 604800;
    private const string DefaultCookieName = "UserAuthentication";
    private const string DefaultAudience = "e5c86676-d063-4da3-bb96-c8e0504bdba5";
    private const string DefaultCurrentVersion = "1.0";
    public const string RegistryPath = "/Configuration/UserAuthentication/**";
    private UserAuthenticationConfiguration m_configuration;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Configuration/UserAuthentication/**");
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    public VssSigningCredentials GetSigningCredentials(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return VssSigningCredentials.Create(vssRequestContext.GetService<TeamFoundationStrongBoxService>().RetrieveFilesAsCertificate(vssRequestContext, "ConfigurationSecrets", (IList<string>) new List<string>()
      {
        ServicingTokenConstants.UserAuthCookieSigningCertificateThumbprint
      })[0]);
    }

    public UserAuthenticationConfiguration GetConfiguration(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      return this.EnsureInitialized(requestContext);
    }

    private UserAuthenticationConfiguration EnsureInitialized(IVssRequestContext requestContext)
    {
      UserAuthenticationConfiguration authenticationConfiguration = this.m_configuration;
      if (authenticationConfiguration == null)
      {
        RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Configuration/UserAuthentication/**");
        bool flag = registryEntryCollection["UseSlidingExpiration"].GetValue<bool>(true);
        int num1 = registryEntryCollection["ExpireTimeSpanSeconds"].GetValue<int>(604800);
        int num2 = registryEntryCollection["ReIssueTimeSpanSeconds"].GetValue<int>(num1 / 2);
        string str1 = registryEntryCollection["CookieName"].GetValue<string>("UserAuthentication");
        string str2 = registryEntryCollection["Audience"].GetValue<string>("e5c86676-d063-4da3-bb96-c8e0504bdba5");
        string str3 = registryEntryCollection["TrustedIssuer"].GetValue<string>((string) null);
        string str4 = registryEntryCollection["CurrentVersion"].GetValue<string>("1.0");
        authenticationConfiguration = new UserAuthenticationConfiguration()
        {
          UseSlidingExpiration = flag,
          ExpireTimeSpan = TimeSpan.FromSeconds((double) num1),
          ReIssueTimeSpan = new TimeSpan?(TimeSpan.FromSeconds((double) num2)),
          CookieName = str1,
          Audience = str2,
          Issuer = str3,
          CurrentVersion = str4,
          TimeProvider = (ITimeProvider) new UserAuthenticationConfigurationService.DefaultTimeProvider()
        };
        this.m_configuration = authenticationConfiguration;
      }
      return authenticationConfiguration;
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.m_configuration = (UserAuthenticationConfiguration) null;
    }

    private class DefaultTimeProvider : ITimeProvider
    {
      public DateTime Now => DateTime.UtcNow;
    }
  }
}
