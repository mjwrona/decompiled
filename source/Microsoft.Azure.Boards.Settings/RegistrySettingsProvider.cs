// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Settings.RegistrySettingsProvider
// Assembly: Microsoft.Azure.Boards.Settings, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2AC3574E-9414-4605-BAB7-1F6B28A75804
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.Settings.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Boards.Settings
{
  public class RegistrySettingsProvider : ISettingsProvider, IDisposable
  {
    private ISettingsHive m_settingsHive;

    public RegistrySettingsProvider(IVssRequestContext requestContext)
      : this(requestContext, Guid.Empty)
    {
    }

    public RegistrySettingsProvider(IVssRequestContext requestContext, ISettingsHive settingsHive)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ISettingsHive>(settingsHive, nameof (settingsHive));
      this.m_settingsHive = settingsHive;
    }

    public RegistrySettingsProvider(IVssRequestContext requestContext, Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (projectId != Guid.Empty)
        this.m_settingsHive = (ISettingsHive) new WebProjectSettingsHive(requestContext, projectId);
      else
        this.m_settingsHive = (ISettingsHive) new WebUserSettingsHive(requestContext);
    }

    public T GetSetting<T>(string path, T defaultValue)
    {
      ArgumentUtility.CheckForNull<string>(path, nameof (path));
      return this.m_settingsHive.ReadSetting<T>(path, defaultValue);
    }

    public IDictionary<string, object> QueryEntries(string pathPattern) => this.m_settingsHive.QuerySettings(pathPattern);

    public void SetSetting<T>(string path, T value)
    {
      ArgumentUtility.CheckForNull<string>(path, nameof (path));
      this.m_settingsHive.WriteSetting<T>(path, value);
    }

    public void Flush() => this.m_settingsHive.Flush();

    void IDisposable.Dispose() => this.Flush();

    public void RemoveSetting(string path)
    {
      ArgumentUtility.CheckForNull<string>(path, nameof (path));
      this.m_settingsHive.WriteValue(path, (string) null);
    }
  }
}
