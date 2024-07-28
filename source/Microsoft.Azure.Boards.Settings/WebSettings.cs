// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Settings.WebSettings
// Assembly: Microsoft.Azure.Boards.Settings, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2AC3574E-9414-4605-BAB7-1F6B28A75804
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.Settings.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Boards.Settings
{
  public class WebSettings : IDisposable, ISettingsProvider
  {
    private const string SettingIdentifier = "Settings";
    private const string ControllerSetting = "Controllers";
    private const string TeamSetting = "Teams";
    private const string SettingSeparator = "/";
    private static readonly char[] SettingSeparatorChars = "/".ToCharArray();
    private ISettingsProvider m_settingsProvider;
    private string m_prefix;

    public static ISettingsProvider GetWebSettings(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      WebApiTeam team,
      WebSettingsScope scope)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(tfsRequestContext, nameof (tfsRequestContext));
      string prefix = string.Empty;
      ISettingsProvider provider;
      switch (scope)
      {
        case WebSettingsScope.User:
          provider = (ISettingsProvider) new RegistrySettingsProvider(tfsRequestContext);
          break;
        case WebSettingsScope.UserAndTeam:
          if (team == null)
            throw new InvalidOperationException(WACommonResources.TeamContextNotFound);
          provider = (ISettingsProvider) new RegistrySettingsProvider(tfsRequestContext);
          prefix = WebSettings.GetUserTeamSettingPrefix(team);
          break;
        case WebSettingsScope.Project:
          provider = (ISettingsProvider) new RegistrySettingsProvider(tfsRequestContext, projectId);
          break;
        case WebSettingsScope.Collection:
          provider = (ISettingsProvider) new RegistrySettingsProvider(tfsRequestContext, (ISettingsHive) new WebGlobalSettingsHive(tfsRequestContext));
          break;
        case WebSettingsScope.Root:
          provider = (ISettingsProvider) new RegistrySettingsProvider(tfsRequestContext, (ISettingsHive) new RootSettingsHive(tfsRequestContext));
          break;
        default:
          return (ISettingsProvider) null;
      }
      return (ISettingsProvider) new WebSettings(provider, prefix);
    }

    public virtual void SetSetting<T>(string path, T value)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      this.m_settingsProvider.SetSetting<T>(this.ToFullPath(path), value);
    }

    public virtual T GetSetting<T>(string path, T defaultValue)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      return this.m_settingsProvider.GetSetting<T>(this.ToFullPath(path), defaultValue);
    }

    public IDictionary<string, object> QueryEntries(string pathPattern) => (IDictionary<string, object>) this.m_settingsProvider.QueryEntries(this.ToFullPath(pathPattern)).ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (kvp => this.FromFullPath(kvp.Key)), (Func<KeyValuePair<string, object>, object>) (kvp => kvp.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public virtual void RemoveSetting(string path)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      this.m_settingsProvider.RemoveSetting(this.ToFullPath(path));
    }

    public virtual void Dispose() => this.m_settingsProvider.Flush();

    void ISettingsProvider.Flush() => this.Dispose();

    public static string CombineKeyPaths(params string[] parts)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) parts, nameof (parts));
      List<string> stringList = new List<string>(parts.Length * 2);
      for (int index = 0; index < parts.Length; ++index)
      {
        string str = parts[index].Trim(WebSettings.SettingSeparatorChars);
        if (!string.IsNullOrWhiteSpace(str))
        {
          stringList.Add("/");
          stringList.Add(str);
        }
      }
      return string.Concat(stringList.ToArray());
    }

    protected WebSettings()
    {
    }

    private WebSettings(ISettingsProvider provider, string prefix)
    {
      this.m_settingsProvider = provider;
      this.m_prefix = prefix;
    }

    private string ToFullPath(string path) => WebSettings.CombineKeyPaths(this.m_prefix, path);

    private string FromFullPath(string path) => !string.IsNullOrWhiteSpace(this.m_prefix) ? path.Substring("/".Length + this.m_prefix.Trim(WebSettings.SettingSeparatorChars).Length) : path;

    private static string GetUserTeamSettingPrefix(WebApiTeam team) => WebSettings.CombineKeyPaths("Teams", team.Id.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture));
  }
}
