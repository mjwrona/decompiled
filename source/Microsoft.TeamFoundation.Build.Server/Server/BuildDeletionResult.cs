// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildDeletionResult
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildDeletionResult
  {
    [ClientProperty(ClientVisibility.Private)]
    public Failure LabelFailure { get; set; }

    [ClientProperty(ClientVisibility.Private)]
    public Failure DropLocationFailure { get; set; }

    [ClientProperty(ClientVisibility.Private)]
    public Failure TestResultFailure { get; set; }

    [ClientProperty(ClientVisibility.Private)]
    public Failure SymbolsFailure { get; set; }

    internal BuildDetail Build { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildDeletionResult LabelFailure={0} DropLocationFailure={1} TestResultFailure={2} SymbolsFailure={3} BuildUri={4}]", (object) this.LabelFailure, (object) this.DropLocationFailure, (object) this.TestResultFailure, (object) this.SymbolsFailure, this.Build != null ? (object) this.Build.Uri : (object) string.Empty);
  }
}
