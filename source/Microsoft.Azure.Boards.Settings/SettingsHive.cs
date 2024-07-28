// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Settings.SettingsHive
// Assembly: Microsoft.Azure.Boards.Settings, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2AC3574E-9414-4605-BAB7-1F6B28A75804
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.Settings.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.Settings
{
  public abstract class SettingsHive : CachedRegistry, IDisposable, ISettingsHiveCore
  {
    protected SettingsHive()
    {
    }

    public SettingsHive(IVssRequestContext requestContext)
      : this(requestContext, (string) null)
    {
    }

    public SettingsHive(IVssRequestContext requestContext, string cachePattern)
      : base(requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (string.IsNullOrEmpty(cachePattern))
        return;
      this.Cache(cachePattern);
    }

    protected override IDictionary<string, string> Read(string pathPattern)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (RegistryEntry queryEntry in (IEnumerable<RegistryEntry>) this.QueryEntries(this.ToWebRegistryPath(pathPattern), false))
        dictionary[queryEntry.Path.Substring(this.Prefix.Length)] = queryEntry.Value;
      return (IDictionary<string, string>) dictionary;
    }

    protected override void Write(IDictionary<string, string> data)
    {
      if (data == null || data.Count <= 0)
        return;
      RegistryEntry[] entries = new RegistryEntry[data.Count];
      int num = 0;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) data)
        entries[num++] = new RegistryEntry(this.ToWebRegistryPath(keyValuePair.Key), keyValuePair.Value);
      this.UpdateEntries(entries);
    }

    protected override void Clear() => this.RemoveEntries(new string[1]
    {
      this.ToWebRegistryPath("/**")
    });

    public void Dispose()
    {
      this.Flush();
      GC.SuppressFinalize((object) this);
    }

    protected abstract string Prefix { get; }

    protected virtual string ToWebRegistryPath(string path) => this.Prefix + path;

    protected virtual string FromWebRegistryPath(string path) => !string.IsNullOrEmpty(this.Prefix) ? path.Substring(this.Prefix.Length) : path;

    protected abstract void UpdateEntries(RegistryEntry[] entries);

    protected abstract void RemoveEntries(string[] entries);

    protected abstract IList<RegistryEntry> QueryEntries(string pathPattern, bool includeFolders);

    public override IDictionary<string, object> QuerySettings(string pathPattern) => (IDictionary<string, object>) this.QueryEntries(this.ToWebRegistryPath(pathPattern), false).ToDictionary<RegistryEntry, string, object>((Func<RegistryEntry, string>) (re => this.FromWebRegistryPath(re.Path)), (Func<RegistryEntry, object>) (re => re.GetValue<object>()));

    string ISettingsHiveCore.Prefix => this.Prefix;

    string ISettingsHiveCore.ToWebRegistryPath(string path) => this.ToWebRegistryPath(path);

    void ISettingsHiveCore.UpdateEntries(RegistryEntry[] entries) => this.UpdateEntries(entries);

    void ISettingsHiveCore.RemoveEntries(string[] entries) => this.RemoveEntries(entries);

    IList<RegistryEntry> ISettingsHiveCore.QueryEntries(string pathPattern, bool includeFolders) => this.QueryEntries(pathPattern, includeFolders);
  }
}
