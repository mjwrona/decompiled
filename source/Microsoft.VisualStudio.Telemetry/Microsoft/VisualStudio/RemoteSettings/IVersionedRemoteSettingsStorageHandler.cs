// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.IVersionedRemoteSettingsStorageHandler
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal interface IVersionedRemoteSettingsStorageHandler : 
    IRemoteSettingsStorageHandler,
    ISettingsCollection,
    IScopesStorageHandler
  {
    string FileVersion { get; }

    void DeleteSettingsForFileVersion(string fileVersion);

    bool DoSettingsNeedToBeUpdated(string newFileVersion);

    void SaveSettings(VersionedDeserializedRemoteSettings remoteSettings);

    void InvalidateFileVersion();

    void CleanUpOldFileVersions(string newFileVersion);
  }
}
