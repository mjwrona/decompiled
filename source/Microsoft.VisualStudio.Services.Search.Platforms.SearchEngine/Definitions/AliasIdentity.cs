// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.AliasIdentity
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions
{
  public class AliasIdentity
  {
    public static AliasIdentity CreateAliasIdentity(string aliasIdentity) => new AliasIdentity()
    {
      Name = aliasIdentity
    };

    public string Name { get; private set; }

    public override string ToString() => this.Name;

    public override bool Equals(object obj) => obj != null && (obj.GetType() == this.GetType() || obj.GetType().IsSubclassOf(this.GetType())) && this.Name.Equals(((AliasIdentity) obj).Name, StringComparison.Ordinal);

    public override int GetHashCode() => this.Name.GetHashCode();
  }
}
