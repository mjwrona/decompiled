// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.InstalledLanguageService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class InstalledLanguageService : 
    VssBaseService,
    IInternalInstalledLanguageService,
    IInstalledLanguageService,
    IVssFrameworkService
  {
    private HashSet<int> m_installedLanguages;
    private HashSet<int> m_userInstalledLanguages;
    private int m_cacheVersion = -1;
    private bool m_cacheFresh;
    private ILockName m_userInstalledLanguagesLockName;
    private INotificationRegistration m_installedLanguageRegistration;
    private const string c_userInstalledLanguagesLock = "UserInstalledLanguagesLock";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      this.m_userInstalledLanguagesLockName = this.CreateLockName(systemRequestContext, "userInstalledLanguages");
      this.m_installedLanguageRegistration = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.UserInstalledLanguageChanged, new SqlNotificationCallback(this.OnUserInstalledLanguageChanged), true, false);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.m_installedLanguageRegistration.Unregister(systemRequestContext);

    public ISet<int> GetInstalledLanguages(IVssRequestContext requestContext)
    {
      if (this.m_installedLanguages == null)
      {
        HashSet<int> list = new HashSet<int>();
        this.LoadLanguages(requestContext, list, FrameworkServerConstants.InstalledUICulture, FrameworkServerConstants.InstalledUICultureSeparator);
        using (requestContext.Lock(this.m_userInstalledLanguagesLockName))
        {
          if (this.m_installedLanguages == null)
            this.m_installedLanguages = list;
        }
      }
      return (ISet<int>) new HashSet<int>((IEnumerable<int>) this.m_installedLanguages);
    }

    public ISet<int> GetUserInstalledLanguages(IVssRequestContext requestContext)
    {
      if (this.m_cacheFresh)
        return (ISet<int>) new HashSet<int>((IEnumerable<int>) this.m_userInstalledLanguages);
      int cacheVersion = this.m_cacheVersion;
      HashSet<int> list = new HashSet<int>();
      this.LoadLanguages(requestContext, list, FrameworkServerConstants.UserInstalledUICulture, FrameworkServerConstants.InstalledUICultureSeparator);
      using (requestContext.AcquireWriterLock(this.m_userInstalledLanguagesLockName))
      {
        if (this.m_cacheFresh)
          return (ISet<int>) new HashSet<int>((IEnumerable<int>) this.m_userInstalledLanguages);
        this.m_userInstalledLanguages = list;
        this.m_cacheFresh = this.m_cacheVersion == cacheVersion;
      }
      return (ISet<int>) new HashSet<int>((IEnumerable<int>) this.m_userInstalledLanguages);
    }

    public ISet<int> GetAllInstalledLanguages(IVssRequestContext requestContext)
    {
      ISet<int> installedLanguages = this.GetInstalledLanguages(requestContext);
      installedLanguages.UnionWith((IEnumerable<int>) this.GetUserInstalledLanguages(requestContext));
      return installedLanguages;
    }

    public void CheckLanguageInstalled(
      IVssRequestContext requestContext,
      int lcid,
      string argumentName)
    {
      if (!this.GetInstalledLanguages(requestContext).Contains(lcid))
        throw new ArgumentException(FrameworkResources.LcidMustBeInstalledLanguageError((object) lcid), argumentName);
    }

    public void CheckAnyLanguageInstalled(
      IVssRequestContext requestContext,
      int lcid,
      string argumentName)
    {
      if (!this.GetAllInstalledLanguages(requestContext).Contains(lcid))
        throw new ArgumentException(FrameworkResources.LcidMustBeInstalledLanguageError((object) lcid), argumentName);
    }

    private void LoadLanguages(
      IVssRequestContext requestContext,
      HashSet<int> list,
      string key,
      string separator)
    {
      List<RegistryEntry> registryEntryList = requestContext.GetService<SecuredRegistryManager>().QueryRegistryEntries(requestContext.Elevate(), key, false);
      if (registryEntryList.Count == 0)
        return;
      string str1 = registryEntryList[0].Value;
      string[] separator1 = new string[1]{ separator };
      foreach (string str2 in str1.Split(separator1, StringSplitOptions.RemoveEmptyEntries))
      {
        try
        {
          int int32 = Convert.ToInt32(str2, (IFormatProvider) CultureInfo.InvariantCulture);
          CultureInfo.GetCultureInfo(int32);
          list.Add(int32);
        }
        catch (Exception ex)
        {
          TeamFoundationTrace.TraceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Exception thrown while loading languages from {0}", (object) key), nameof (LoadLanguages), ex);
        }
      }
    }

    private void OnUserInstalledLanguageChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      using (requestContext.AcquireWriterLock(this.m_userInstalledLanguagesLockName))
      {
        ++this.m_cacheVersion;
        this.m_cacheFresh = false;
      }
    }
  }
}
