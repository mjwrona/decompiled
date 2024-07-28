// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ProjectSettings.TpcSettingsStore
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Client.ProjectSettings
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TpcSettingsStore : ITfsTeamProjectCollectionObject
  {
    private bool m_isInitialized;
    private TfsTeamProjectCollection m_teamProjectCollection;
    private ITpcSettingsAdapter m_settingsAdapter;
    private bool? m_supportsFramework;
    private ProjectCollectionSettings m_collectionSettings;
    private ConcurrentDictionary<Uri, TeamProjectSettings> m_projectSettingsCacheByUri;
    private ConcurrentDictionary<string, TeamProjectSettings> m_projectSettingsCacheByName;

    internal TpcSettingsStore()
    {
    }

    void ITfsTeamProjectCollectionObject.Initialize(TfsTeamProjectCollection teamProjectCollection)
    {
      ArgumentUtility.CheckForNull<TfsTeamProjectCollection>(teamProjectCollection, nameof (teamProjectCollection));
      this.m_teamProjectCollection = teamProjectCollection;
      this.InitializeInternal();
    }

    internal virtual void InitializeInternal()
    {
      this.m_supportsFramework = new bool?();
      this.m_settingsAdapter = !this.SupportsFramework ? (ITpcSettingsAdapter) new PreFrameworkSettingsProvider(this.m_teamProjectCollection) : (ITpcSettingsAdapter) new FrameworkSettingsProvider(this.m_teamProjectCollection);
      this.m_projectSettingsCacheByUri = new ConcurrentDictionary<Uri, TeamProjectSettings>();
      this.m_projectSettingsCacheByName = new ConcurrentDictionary<string, TeamProjectSettings>((IEqualityComparer<string>) TFStringComparer.TeamProjectName);
    }

    public bool SupportsFramework
    {
      get
      {
        if (!this.m_supportsFramework.HasValue)
          this.m_supportsFramework = this.m_teamProjectCollection.GetService<ILocationService>().LocationForCurrentConnection("SecurityService", FrameworkServiceIdentifiers.CollectionSecurity) == null ? new bool?(false) : new bool?(true);
        bool? supportsFramework = this.m_supportsFramework;
        bool flag = true;
        return supportsFramework.GetValueOrDefault() == flag & supportsFramework.HasValue;
      }
    }

    public ProjectCollectionSettings ProjectCollectionSettings
    {
      get
      {
        if (!this.m_isInitialized)
          this.InitializeSettingsCache();
        return this.m_collectionSettings;
      }
    }

    public virtual TeamProjectSettings GetTeamProjectSettings(Uri projectUri) => this.GetTeamProjectSettings(projectUri, false);

    public TeamProjectSettings GetTeamProjectSettings(Uri projectUri, bool forceRefresh)
    {
      TeamProjectSettings teamProjectSettings = (TeamProjectSettings) null;
      if (!forceRefresh && this.m_projectSettingsCacheByUri.TryGetValue(projectUri, out teamProjectSettings))
        return teamProjectSettings;
      this.InitializeSettingsCache();
      if (!this.m_projectSettingsCacheByUri.TryGetValue(projectUri, out teamProjectSettings))
        this.m_projectSettingsCacheByUri[projectUri] = (TeamProjectSettings) null;
      return teamProjectSettings;
    }

    public virtual TeamProjectSettings GetTeamProjectSettings(string projectName)
    {
      TeamProjectSettings teamProjectSettings1;
      if (this.m_projectSettingsCacheByName.TryGetValue(projectName, out teamProjectSettings1))
        return teamProjectSettings1;
      this.InitializeSettingsCache();
      TeamProjectSettings teamProjectSettings2;
      if (!this.m_projectSettingsCacheByName.TryGetValue(projectName, out teamProjectSettings2))
        this.m_projectSettingsCacheByName[projectName] = (TeamProjectSettings) null;
      return teamProjectSettings2;
    }

    public ICollection<TeamProjectSettings> GetTeamProjectSettings(ICollection<Uri> projectUris) => this.GetTeamProjectSettings(projectUris, false);

    public ICollection<TeamProjectSettings> GetTeamProjectSettings(
      ICollection<Uri> projectUris,
      bool refresh)
    {
      ICollection<TeamProjectSettings> collection = (ICollection<TeamProjectSettings>) new List<TeamProjectSettings>();
      if (!projectUris.Any<Uri>())
        return collection;
      if (refresh)
        this.InitializeSettingsCache();
      HashSet<Uri> desiredProjectUris = projectUris.ToHashSet<Uri, Uri>((Func<Uri, Uri>) (x => x));
      List<Uri> list = this.m_projectSettingsCacheByUri.Keys.Where<Uri>((Func<Uri, bool>) (t => desiredProjectUris.Contains(t))).ToList<Uri>();
      if (projectUris.Except<Uri>((IEnumerable<Uri>) list).ToHashSet<Uri, Uri>((Func<Uri, Uri>) (x => x)).Any<Uri>() && !refresh)
        this.InitializeSettingsCache();
      this.AddMissingUrisToCache((IEnumerable<Uri>) projectUris);
      IEnumerable<TeamProjectSettings> values = this.m_projectSettingsCacheByUri.Values.Where<TeamProjectSettings>((Func<TeamProjectSettings, bool>) (t => t != null && desiredProjectUris.Contains(t.ProjectUri)));
      collection.AddRange<TeamProjectSettings, ICollection<TeamProjectSettings>>(values);
      return collection;
    }

    public IEnumerable<Uri> TeamProjectsInCache => this.m_projectSettingsCacheByUri.Values.Where<TeamProjectSettings>((Func<TeamProjectSettings, bool>) (p => p != null)).Select<TeamProjectSettings, Uri>((Func<TeamProjectSettings, Uri>) (t => t.ProjectUri));

    public void RemoveTeamProjectSettings(string projectName)
    {
      if (!this.m_projectSettingsCacheByName.ContainsKey(projectName))
        return;
      TeamProjectSettings teamProjectSettings;
      this.m_projectSettingsCacheByName.TryRemove(projectName, out teamProjectSettings);
      if (teamProjectSettings == null)
        return;
      this.m_projectSettingsCacheByUri.TryRemove(teamProjectSettings.ProjectUri, out teamProjectSettings);
    }

    public void InitializeSettingsCache()
    {
      ProjectCollectionSettings projectCollectionSettings;
      ICollection<TeamProjectSettings> teamProjectSettings;
      this.m_settingsAdapter.GetAllSettings(out projectCollectionSettings, out teamProjectSettings);
      this.m_collectionSettings = projectCollectionSettings;
      this.AddToCache(teamProjectSettings, true);
      this.m_isInitialized = true;
    }

    private void AddMissingUrisToCache(IEnumerable<Uri> projectUris)
    {
      if (projectUris == null)
        return;
      foreach (Uri projectUri in projectUris)
      {
        if (!this.m_projectSettingsCacheByUri.ContainsKey(projectUri))
          this.m_projectSettingsCacheByUri.TryAdd(projectUri, (TeamProjectSettings) null);
      }
    }

    private void AddToCache(ICollection<TeamProjectSettings> tpSettings, bool clearCache)
    {
      if (clearCache)
      {
        this.m_projectSettingsCacheByUri.Clear();
        this.m_projectSettingsCacheByName.Clear();
      }
      foreach (TeamProjectSettings tpSetting in (IEnumerable<TeamProjectSettings>) tpSettings)
        this.AddToCache(tpSetting);
    }

    private void AddToCache(TeamProjectSettings tpSetting)
    {
      if (tpSetting.State == ProjectState.WellFormed)
      {
        this.m_projectSettingsCacheByUri[tpSetting.ProjectUri] = tpSetting;
        this.m_projectSettingsCacheByName[tpSetting.Name] = tpSetting;
      }
      else
      {
        if (tpSetting.State != ProjectState.Deleting)
          return;
        this.m_projectSettingsCacheByUri[tpSetting.ProjectUri] = (TeamProjectSettings) null;
        this.m_projectSettingsCacheByName[tpSetting.Name] = (TeamProjectSettings) null;
      }
    }
  }
}
