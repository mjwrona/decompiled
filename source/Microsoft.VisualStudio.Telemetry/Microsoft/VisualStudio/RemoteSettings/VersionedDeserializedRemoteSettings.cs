// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.VersionedDeserializedRemoteSettings
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Collections.ObjectModel;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal sealed class VersionedDeserializedRemoteSettings : DeserializedRemoteSettings
  {
    public string FileVersion { get; }

    public string ChangesetId { get; }

    public VersionedDeserializedRemoteSettings(
      ReadOnlyCollection<Scope> scopes = null,
      ReadOnlyCollection<RemoteSetting> settings = null,
      string fileVersion = null,
      string changesetId = null,
      string error = null)
      : base(scopes, settings, error)
    {
      this.FileVersion = fileVersion;
      this.ChangesetId = changesetId;
    }

    public VersionedDeserializedRemoteSettings(
      DeserializedRemoteSettings remoteSettings,
      string fileVersion = null,
      string changesetId = null)
      : this(remoteSettings.Scopes, remoteSettings.Settings, fileVersion, changesetId, remoteSettings.Error)
    {
    }
  }
}
