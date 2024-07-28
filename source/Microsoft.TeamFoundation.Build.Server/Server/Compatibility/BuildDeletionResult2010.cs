// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildDeletionResult2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType("BuildDeletionResult")]
  public sealed class BuildDeletionResult2010
  {
    public Failure2010 LabelFailure { get; set; }

    public Failure2010 DropLocationFailure { get; set; }

    public Failure2010 TestResultFailure { get; set; }

    public Failure2010 SymbolsFailure { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildDeletionResult2010 LabelFailure={0} DropLocationFailure={1} TestResultFailure={2} SymbolsFailure={3}]", (object) this.LabelFailure, (object) this.DropLocationFailure, (object) this.TestResultFailure, (object) this.SymbolsFailure);
  }
}
