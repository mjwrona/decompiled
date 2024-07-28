// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.RetentionPolicy2010
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
  [XmlType("RetentionPolicy")]
  public sealed class RetentionPolicy2010
  {
    public RetentionPolicy2010()
    {
      this.BuildReason = BuildReason2010.All;
      this.DeleteOptions = DeleteOptions2010.All;
    }

    public RetentionPolicy2010(BuildStatus2010 status)
      : this()
    {
      this.BuildStatus = status;
    }

    public RetentionPolicy2010(BuildReason2010 reason, BuildStatus2010 status, int numberToKeep)
      : this(status)
    {
      this.BuildReason = reason;
      this.NumberToKeep = numberToKeep;
    }

    [XmlAttribute]
    [DefaultValue(BuildReason2010.All)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public BuildReason2010 BuildReason { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public BuildStatus2010 BuildStatus { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public int NumberToKeep { get; set; }

    [XmlAttribute]
    [DefaultValue(DeleteOptions2010.All)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public DeleteOptions2010 DeleteOptions { get; set; }

    internal string DefinitionUri { get; set; }

    internal Guid ProjectId { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[RetentionPolicy2010 DefinitionUri={0} BuildReason={1} BuildStatus={2} NumberToKeep={3} DeleteOptions={4}]", (object) this.DefinitionUri, (object) this.BuildReason, (object) this.BuildStatus, (object) this.NumberToKeep, (object) this.DeleteOptions);
  }
}
