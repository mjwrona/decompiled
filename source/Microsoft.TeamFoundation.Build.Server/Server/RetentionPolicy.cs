// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.RetentionPolicy
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class RetentionPolicy
  {
    public RetentionPolicy()
    {
      this.BuildReason = BuildReason.All;
      this.DeleteOptions = DeleteOptions.All;
    }

    public RetentionPolicy(BuildStatus status)
      : this()
    {
      this.BuildStatus = status;
    }

    public RetentionPolicy(BuildReason reason, BuildStatus status, int numberToKeep)
      : this(status)
    {
      this.BuildReason = reason;
      this.NumberToKeep = numberToKeep;
    }

    [XmlAttribute]
    [DefaultValue(BuildReason.All)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public BuildReason BuildReason { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public BuildStatus BuildStatus { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public int NumberToKeep { get; set; }

    [XmlAttribute]
    [DefaultValue(DeleteOptions.All)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public DeleteOptions DeleteOptions { get; set; }

    internal string DefinitionUri { get; set; }

    internal Guid ProjectId { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[RetentionPolicy DefinitionUri={0} BuildReason={1} BuildStatus={2} NumberToKeep={3} DeleteOptions={4}]", (object) this.DefinitionUri, (object) this.BuildReason, (object) this.BuildStatus, (object) this.NumberToKeep, (object) this.DeleteOptions);
  }
}
