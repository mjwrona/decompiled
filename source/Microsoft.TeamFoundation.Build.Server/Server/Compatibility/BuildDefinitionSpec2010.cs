// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildDefinitionSpec2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType("BuildDefinitionSpec")]
  public sealed class BuildDefinitionSpec2010 : BuildGroupItemSpec2010
  {
    public BuildDefinitionSpec2010()
    {
      this.Options = QueryOptions2010.All;
      this.ContinuousIntegrationType = ContinuousIntegrationType.All;
    }

    internal BuildDefinitionSpec2010(string fullPath)
      : base(fullPath)
    {
      this.Options = QueryOptions2010.All;
      this.ContinuousIntegrationType = ContinuousIntegrationType.All;
    }

    [DefaultValue(ContinuousIntegrationType.All)]
    [ClientProperty(ClientVisibility.Public)]
    public ContinuousIntegrationType ContinuousIntegrationType { get; set; }

    [DefaultValue(QueryOptions2010.All)]
    [ClientProperty(ClientVisibility.Public)]
    public QueryOptions2010 Options { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildDefinitionSpec2010 FullPath={0} Options={1}]", (object) this.FullPath, (object) this.Options);
  }
}
