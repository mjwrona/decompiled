// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.StableRemoteSettingsProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class StableRemoteSettingsProvider : 
    RemoteSettingsProviderBase,
    IStableRemoteSettingsProvider,
    IRemoteSettingsProvider,
    ISettingsCollection,
    IDisposable
  {
    private readonly HashSet<string> stableSettingRootSubCollections;

    public StableRemoteSettingsProvider(RemoteSettingsInitializer initializer)
      : base(initializer.LiveRemoteSettingsStorageHandlerFactory())
    {
      this.stableSettingRootSubCollections = new HashSet<string>(initializer.StableSettingRootSubCollections);
    }

    public override string Name => "StableProvider";

    public bool IsStable(string collectionPath) => this.stableSettingRootSubCollections.Contains(collectionPath.GetRootSubCollectionOfPath());

    public void MakeStable<T>(string collectionPath, string key, T value) => this.currentStorageHandler.SaveNonScopedSetting(new RemoteSetting(collectionPath, key, (object) value, (string) null));

    public override Task<GroupedRemoteSettings> Start() => this.startTask;
  }
}
