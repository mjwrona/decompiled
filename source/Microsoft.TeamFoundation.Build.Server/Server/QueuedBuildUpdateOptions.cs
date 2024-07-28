// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.QueuedBuildUpdateOptions
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class QueuedBuildUpdateOptions : IValidatable
  {
    public QueuedBuildUpdateOptions()
    {
      this.BatchId = BuildWellKnownBatchIds.DynamicBatch;
      this.Priority = QueuePriority.Normal;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public int QueueId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public QueuedBuildUpdate Fields { get; set; }

    [XmlAttribute]
    [DefaultValue(typeof (Guid), "00000000-0000-0000-0000-000000000000")]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public Guid BatchId { get; set; }

    [XmlAttribute]
    [DefaultValue(QueuePriority.Normal)]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public QueuePriority Priority { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public bool Postponed { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public bool Retry { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public QueuedBuildRetryOption RetryOption { get; set; }

    internal Guid ProjectId { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[QueuedBuildUpdate QueueId={0} BatchId={1} Fields={2}]", (object) this.QueueId, (object) this.BatchId, (object) this.Fields);

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context) => ArgumentValidation.CheckBound("QueueId", this.QueueId, 0, int.MaxValue);
  }
}
