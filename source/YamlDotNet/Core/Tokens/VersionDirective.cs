// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.Tokens.VersionDirective
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;

namespace YamlDotNet.Core.Tokens
{
  [Serializable]
  public class VersionDirective : Token
  {
    private readonly YamlDotNet.Core.Version version;

    public YamlDotNet.Core.Version Version => this.version;

    public VersionDirective(YamlDotNet.Core.Version version)
      : this(version, Mark.Empty, Mark.Empty)
    {
    }

    public VersionDirective(YamlDotNet.Core.Version version, Mark start, Mark end)
      : base(start, end)
    {
      this.version = version;
    }

    public override bool Equals(object obj) => obj is VersionDirective versionDirective && this.version.Equals((object) versionDirective.version);

    public override int GetHashCode() => this.version.GetHashCode();
  }
}
