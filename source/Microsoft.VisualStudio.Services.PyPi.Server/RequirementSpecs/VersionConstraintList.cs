// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs
{
  public class VersionConstraintList
  {
    public IEnumerable<VersionConstraint> VersionConstraints { get; }

    public VersionConstraintList(IEnumerable<VersionConstraint> versionConstraints) => this.VersionConstraints = (IEnumerable<VersionConstraint>) versionConstraints.ToList<VersionConstraint>();

    public override string ToString() => string.Join<VersionConstraint>(",", this.VersionConstraints);

    public string Dump(string indent, string newline) => "VersionConstraintList(" + string.Join(", ", this.VersionConstraints.Select<VersionConstraint, string>((Func<VersionConstraint, string>) (x => x.Dump(indent, newline)))) + ")";
  }
}
