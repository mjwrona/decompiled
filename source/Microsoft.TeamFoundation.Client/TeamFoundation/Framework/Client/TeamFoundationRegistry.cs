// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TeamFoundationRegistry
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal sealed class TeamFoundationRegistry : ITeamFoundationRegistry
  {
    private RegistryWebService m_registry;

    public TeamFoundationRegistry(TfsConnection server) => this.m_registry = new RegistryWebService(server);

    public T GetValue<T>(string path) => this.ReadEntries(path, false)[path].GetValue<T>();

    public T GetValue<T>(string path, T defaultValue) => this.ReadEntries(path, false)[path].GetValue<T>(defaultValue);

    public string GetValue(string path) => this.ReadEntries(path, false)[path].Value;

    public string GetValue(string path, string defaultValue) => this.ReadEntries(path, false)[path].GetValue(defaultValue);

    public T GetUserValue<T>(string path) => this.ReadUserEntries(path, false)[path].GetValue<T>();

    public T GetUserValue<T>(string path, T defaultValue) => this.ReadUserEntries(path, false)[path].GetValue<T>(defaultValue);

    public string GetUserValue(string path) => this.ReadUserEntries(path, false)[path].Value;

    public string GetUserValue(string path, string defaultValue) => this.ReadUserEntries(path, false)[path].GetValue(defaultValue);

    public void SetValue(string path, string value) => this.WriteEntriesInternal((IEnumerable<RegistryEntry>) new RegistryEntry[1]
    {
      new RegistryEntry(path, value)
    }, false);

    public void SetValue<T>(string path, T value) => this.SetValue(path, RegistryUtility.ToString<T>(value));

    public void SetUserValue(string path, string value) => this.WriteEntriesInternal((IEnumerable<RegistryEntry>) new RegistryEntry[1]
    {
      new RegistryEntry(path, value)
    }, true);

    public void SetUserValue<T>(string path, T value) => this.SetUserValue(path, RegistryUtility.ToString<T>(value));

    public int DeleteEntries(params string[] registryPathPatterns)
    {
      if (registryPathPatterns == null)
        throw new ArgumentNullException(nameof (registryPathPatterns));
      return registryPathPatterns.Length == 0 ? 0 : this.m_registry.RemoveRegistryEntries(registryPathPatterns);
    }

    public int DeleteUserEntries(params string[] registryPathPatterns)
    {
      if (registryPathPatterns == null)
        throw new ArgumentNullException(nameof (registryPathPatterns));
      return registryPathPatterns.Length == 0 ? 0 : this.m_registry.RemoveUserEntries(registryPathPatterns);
    }

    public IEnumerable<RegistryAuditEntry> QueryAuditLog(int changeIndex, bool returnOlder) => (IEnumerable<RegistryAuditEntry>) this.m_registry.QueryAuditLog(changeIndex, returnOlder);

    public RegistryEntryCollection ReadEntries(string registryPath) => this.ReadEntries(registryPath, false);

    public RegistryEntryCollection ReadEntries(string registryPath, bool includeContainerHints)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(registryPath, nameof (registryPath));
      string path;
      RegistryUtility.Parse(registryPath, out path, out string _, out int _);
      RegistryEntry[] entries = this.m_registry.QueryRegistryEntries(registryPath, includeContainerHints);
      return new RegistryEntryCollection(path, (ICollection<RegistryEntry>) entries);
    }

    public RegistryEntryCollection ReadUserEntries(string registryPath) => this.ReadUserEntries(registryPath, false);

    public RegistryEntryCollection ReadUserEntries(string registryPath, bool includeContainerHints)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(registryPath, nameof (registryPath));
      RegistryEntry[] entries = this.m_registry.QueryUserEntries(registryPath, includeContainerHints);
      string path;
      RegistryUtility.Parse(registryPath, out path, out string _, out int _);
      return new RegistryEntryCollection(path, (ICollection<RegistryEntry>) entries);
    }

    public void WriteEntries(IEnumerable<RegistryEntry> registryEntries) => this.WriteEntriesInternal(registryEntries, false);

    public void WriteUserEntries(IEnumerable<RegistryEntry> registryEntries) => this.WriteEntriesInternal(registryEntries, true);

    private void WriteEntriesInternal(
      IEnumerable<RegistryEntry> registryEntries,
      bool writeToUserHive)
    {
      if (registryEntries == null)
        throw new ArgumentNullException(nameof (registryEntries));
      using (IEnumerator<RegistryEntry> enumerator = registryEntries.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return;
      }
      foreach (RegistryEntry registryEntry in registryEntries)
      {
        if (registryEntry.Value == null)
          throw new ArgumentNullException(nameof (registryEntries), "registryEntries.entry.Value");
      }
      if (!writeToUserHive)
        this.m_registry.UpdateRegistryEntries(registryEntries);
      else
        this.m_registry.UpdateUserEntries(registryEntries);
    }
  }
}
