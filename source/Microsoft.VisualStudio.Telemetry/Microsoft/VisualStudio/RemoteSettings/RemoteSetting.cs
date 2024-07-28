// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.RemoteSetting
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal sealed class RemoteSetting
  {
    public const char Separator = ':';

    public string Path { get; }

    public string Name { get; }

    public string ScopeString { get; set; }

    public object Value { get; }

    public string Origin { get; set; }

    public bool HasScope => this.ScopeString != null;

    public RemoteSetting(string path, string name, object value, string scopeString)
    {
      this.Path = path;
      this.Name = name;
      this.Value = value;
      this.ScopeString = scopeString;
      this.Origin = string.Empty;
    }

    public override string ToString() => string.Format("{0}: {1} {2}{3} {4}", (object) this.Origin, (object) this.Path, (object) this.Name, (object) (this.HasScope ? ":" + this.ScopeString : string.Empty), this.Value);
  }
}
