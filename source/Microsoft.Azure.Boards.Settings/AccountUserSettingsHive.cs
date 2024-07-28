// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Settings.AccountUserSettingsHive
// Assembly: Microsoft.Azure.Boards.Settings, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2AC3574E-9414-4605-BAB7-1F6B28A75804
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.Settings.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Boards.Settings
{
  public class AccountUserSettingsHive : IAccountUserSettingsHive, IDisposable
  {
    private SettingsHive m_userHive;
    private SettingsHive m_fallbackHive;
    private bool m_canUseFallbackHive;

    public AccountUserSettingsHive(IVssRequestContext requestContext)
      : this(requestContext, Guid.Empty)
    {
    }

    public AccountUserSettingsHive(IVssRequestContext requestContext, Guid userId)
    {
      IVssRequestContext hiveContext;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        requestContext.CheckProjectCollectionRequestContext();
        hiveContext = requestContext;
        this.m_canUseFallbackHive = requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) "/Configuration/WebAccess/CanUseFallbackHive", true) && !requestContext.To(TeamFoundationHostType.Application).IsVirtualServiceHost();
      }
      else
      {
        hiveContext = requestContext.To(TeamFoundationHostType.Application);
        if (userId != Guid.Empty)
        {
          Microsoft.VisualStudio.Services.Identity.Identity readIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
          {
            userId
          }, QueryMembership.None, (IEnumerable<string>) null)[0];
          if (readIdentity != null)
            userId = readIdentity.MasterId;
        }
      }
      this.m_userHive = AccountUserSettingsHive.CreateSettingsHive(hiveContext, userId);
      if (!this.m_canUseFallbackHive)
        return;
      this.m_fallbackHive = AccountUserSettingsHive.CreateSettingsHive(hiveContext.To(TeamFoundationHostType.Application), userId);
    }

    public void Dispose()
    {
      if (this.m_userHive != null)
      {
        this.m_userHive.Dispose();
        this.m_userHive = (SettingsHive) null;
      }
      if (this.m_fallbackHive == null)
        return;
      this.m_fallbackHive.Dispose();
      this.m_fallbackHive = (SettingsHive) null;
    }

    public virtual T ReadSetting<T>(string path, T defaultValue)
    {
      string str = this.m_userHive.ReadSetting<string>(path, (string) null);
      if (str == null && this.m_canUseFallbackHive)
        str = this.m_fallbackHive.ReadSetting<string>(path, (string) null);
      return ConvertUtility.FromString<T>(str, defaultValue);
    }

    public virtual void WriteSetting<T>(string path, T value) => this.m_userHive.WriteSetting<T>(path, value);

    public virtual IEnumerable<T> ReadEnumerableSetting<T>(string path, IEnumerable<T> defaultValue)
    {
      string str = this.ReadSetting<string>(path, (string) null);
      return str == null ? defaultValue : str.Split<T>(';');
    }

    public virtual void WriteEnumerableSetting<T>(string path, IEnumerable<T> value)
    {
      if (value == null)
        this.WriteSetting<string>(path, (string) null);
      else
        this.WriteSetting<string>(path, value.StringJoin<T>(';'));
    }

    private static SettingsHive CreateSettingsHive(IVssRequestContext hiveContext, Guid userId) => userId == Guid.Empty ? (SettingsHive) new WebUserSettingsHive(hiveContext) : (SettingsHive) new WebImpersonatedUserSettingsHive(hiveContext, userId);
  }
}
