// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionIdentifier
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

namespace Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs
{
  public abstract class VersionIdentifier
  {
    protected VersionIdentifier(string versionString) => this.VersionString = versionString;

    public string VersionString { get; }

    public override string ToString() => this.VersionString;

    public string Dump(string indent, string newline) => this.GetType().Name + "(" + this.VersionString + ")";
  }
}
