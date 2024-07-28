// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.QueuedBuildUpdateOptions2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType("QueuedBuildUpdateOptions")]
  public sealed class QueuedBuildUpdateOptions2010 : IValidatable
  {
    public QueuedBuildUpdateOptions2010() => this.Priority = QueuePriority2010.Normal;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public int QueueId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public QueuedBuildUpdate2010 Fields { get; set; }

    [XmlAttribute]
    [DefaultValue(QueuePriority2010.Normal)]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public QueuePriority2010 Priority { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public bool Postponed { get; set; }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context) => ArgumentValidation.CheckBound("QueueId", this.QueueId, 0, int.MaxValue);

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[QueuedBuildUpdate2010 QueueId={0} Fields={1}]", (object) this.QueueId, (object) this.Fields);
  }
}
