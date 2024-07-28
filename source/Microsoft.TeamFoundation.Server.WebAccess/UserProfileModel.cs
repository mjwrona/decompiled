// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.UserProfileModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  internal class UserProfileModel
  {
    private IVssRequestContext m_requestContext;
    private List<CultureInfoModel> m_allCultures = new List<CultureInfoModel>();
    private List<ThemeModel> m_allThemes = new List<ThemeModel>();

    public UserProfileModel(TfsWebContext webContext)
    {
      this.m_requestContext = webContext.TfsRequestContext;
      IVssRequestContext vssRequestContext = this.m_requestContext.To(TeamFoundationHostType.Deployment);
      this.UserPreferences = new UserPreferencesModel(this.m_requestContext.GetService<IUserPreferencesService>().GetUserPreferences(this.m_requestContext));
      TeamFoundationIdentityService service1 = this.m_requestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationIdentity readIdentity = service1.ReadIdentities(this.m_requestContext, new IdentityDescriptor[1]
      {
        this.m_requestContext.UserContext
      }, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[2]
      {
        "CustomNotificationAddresses",
        "ConfirmedNotificationAddress"
      })[0];
      this.Identity = IdentityImageUtility.GetIdentityViewModel(readIdentity);
      this.ProviderDisplayName = readIdentity.ProviderDisplayName;
      this.DefaultMailAddress = readIdentity.GetAttribute("Mail", string.Empty);
      if (!vssRequestContext.ExecutionEnvironment.IsHostedDeployment)
        this.SshEnabled = vssRequestContext.GetService<IVssRegistryService>().GetValue<bool>(vssRequestContext, (RegistryQuery) "/Configuration/SshServer/Enabled", false);
      this.UserPreferences.CustomDisplayName = readIdentity.CustomDisplayName;
      object obj = (object) null;
      if (readIdentity.TryGetProperty("CustomNotificationAddresses", out obj))
      {
        this.UserPreferences.PreferredEmail = obj as string;
        this.UserPreferences.IsEmailConfirmationPending = service1.IsEmailConfirmationPending(this.m_requestContext, readIdentity.TeamFoundationId);
      }
      if (this.m_requestContext.ExecutionEnvironment.IsHostedDeployment && string.IsNullOrEmpty(this.UserPreferences.PreferredEmail) && readIdentity.TryGetProperty("ConfirmedNotificationAddress", out obj))
        this.UserPreferences.PreferredEmail = obj as string;
      IInstalledLanguageService service2 = vssRequestContext.GetService<IInstalledLanguageService>();
      DateTime local = this.m_requestContext.GetTimeZone().ConvertToLocal(DateTime.UtcNow);
      foreach (int installedLanguage in (IEnumerable<int>) service2.GetAllInstalledLanguages(vssRequestContext))
        this.m_allCultures.Add(new CultureInfoModel(CultureInfo.GetCultureInfo(installedLanguage), local));
      this.m_allCultures.Sort((Comparison<CultureInfoModel>) ((a, b) => a.DisplayName.CompareTo(b.DisplayName)));
      CultureInfo preferredCulture;
      TeamFoundationApplicationCore.GetPreferredCulture(webContext.RequestContext.HttpContext.Request.UserLanguages, service2.GetInstalledLanguages(vssRequestContext), service2.GetUserInstalledLanguages(vssRequestContext), out preferredCulture);
      this.m_allCultures.Insert(0, (CultureInfoModel) new DefaultCultureInfoModel(preferredCulture ?? CultureInfo.CurrentCulture, local));
      this.m_allThemes.AddRange((IEnumerable<ThemeModel>) ThemesUtility.GetThemes(StaticResources.Versioned.Themes.GetPhysicalLocation(string.Empty), webContext.RequestContext.HttpContext));
    }

    public string ProviderDisplayName { get; private set; }

    public string DefaultMailAddress { get; private set; }

    public IdentityViewModelBase Identity { get; private set; }

    public UserPreferencesModel UserPreferences { get; private set; }

    public bool BasicAuthenticationEnabled { get; private set; }

    public bool SshEnabled { get; private set; }

    public bool TypeAheadDisabled { get; private set; }

    public bool WorkItemFormChromeBorder { get; private set; }

    public ReadOnlyCollection<ThemeModel> AllThemes => this.m_allThemes.AsReadOnly();

    public ReadOnlyCollection<CultureInfoModel> AllCultures => this.m_allCultures.AsReadOnly();

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["identity"] = (object) this.Identity.ToJson();
      json["providerDisplayName"] = (object) this.ProviderDisplayName;
      json["defaultMailAddress"] = (object) this.DefaultMailAddress;
      json["basicAuthenticationEnabled"] = (object) this.BasicAuthenticationEnabled;
      json["sshEnabled"] = (object) this.SshEnabled;
      json["userPreferences"] = (object) this.UserPreferences;
      json["allThemes"] = (object) this.AllThemes;
      json["typeAheadDisabled"] = (object) this.TypeAheadDisabled;
      json["workItemFormChromeBorder"] = (object) this.WorkItemFormChromeBorder;
      List<TimeZoneInfoModel> timeZoneInfoModelList = new List<TimeZoneInfoModel>();
      TimeZoneInfoModel timeZoneInfoModel = new TimeZoneInfoModel(this.m_requestContext.GetCollectionTimeZone());
      timeZoneInfoModel.DisplayName = string.Format(WACommonResources.AccountTimeZone, (object) timeZoneInfoModel.DisplayName);
      timeZoneInfoModelList.Add(timeZoneInfoModel);
      timeZoneInfoModelList.AddRange((IEnumerable<TimeZoneInfoModel>) CommonUtility.AllTimeZones);
      json["allTimeZones"] = (object) timeZoneInfoModelList;
      json["allCultures"] = (object) this.AllCultures;
      return json;
    }
  }
}
