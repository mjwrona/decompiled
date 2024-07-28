// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraint
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

namespace Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs
{
  public class VersionConstraint
  {
    public Operator Op { get; }

    public VersionIdentifier Version { get; }

    public VersionConstraint(Operator op, VersionIdentifier version)
    {
      this.Op = op;
      this.Version = version;
    }

    public override string ToString() => string.Format("{0}{1}", (object) this.Op, (object) this.Version);

    public string Dump(string indent, string newline) => "VersionConstraint(" + this.Op.Dump(indent, newline) + ", " + this.Version.Dump(indent, newline) + ")";
  }
}
