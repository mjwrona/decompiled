// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.ExtrasList
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs
{
  public class ExtrasList
  {
    public IEnumerable<Identifier> Extras { get; }

    public ExtrasList(IEnumerable<Identifier> extras) => this.Extras = extras;

    public string Dump(string indent, string newline) => "ExtrasList(" + string.Join(", ", this.Extras.Select<Identifier, string>((Func<Identifier, string>) (x => x.Dump(indent, newline)))) + ")";

    public override string ToString() => string.Join<Identifier>(", ", this.Extras);
  }
}
