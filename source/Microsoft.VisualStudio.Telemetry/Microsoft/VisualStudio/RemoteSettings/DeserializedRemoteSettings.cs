// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.DeserializedRemoteSettings
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Collections.ObjectModel;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class DeserializedRemoteSettings
  {
    public ReadOnlyCollection<Scope> Scopes { get; }

    public ReadOnlyCollection<RemoteSetting> Settings { get; }

    public string Error { get; }

    public bool Successful => this.Error == null;

    public DeserializedRemoteSettings(
      ReadOnlyCollection<Scope> scopes = null,
      ReadOnlyCollection<RemoteSetting> settings = null,
      string error = null)
    {
      this.Scopes = scopes;
      this.Settings = settings;
      this.Error = error;
    }
  }
}
