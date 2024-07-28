// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.Version
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;

namespace YamlDotNet.Core
{
  [Serializable]
  public class Version
  {
    public int Major { get; private set; }

    public int Minor { get; private set; }

    public Version(int major, int minor)
    {
      this.Major = major;
      this.Minor = minor;
    }

    public override bool Equals(object obj) => obj is Version version && this.Major == version.Major && this.Minor == version.Minor;

    public override int GetHashCode()
    {
      int num = this.Major;
      int hashCode1 = num.GetHashCode();
      num = this.Minor;
      int hashCode2 = num.GetHashCode();
      return hashCode1 ^ hashCode2;
    }
  }
}
