// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationJobHistoryEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationJobHistoryEntry : TeamFoundationJobExecutionEntry
  {
    internal static readonly int s_ResultMessageMaxLength = int.MaxValue;

    [XmlAttribute("hid")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public long HistoryId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime EndTime { get; set; }

    [XmlIgnore]
    public TeamFoundationJobResult Result
    {
      get => (TeamFoundationJobResult) this.ResultValue;
      set => this.ResultValue = (int) value;
    }

    [XmlAttribute("result")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "Result")]
    public int ResultValue { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string ResultMessage { get; set; }

    [XmlAttribute("priority")]
    [DefaultValue(-1)]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "Priority")]
    public int Priority { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "[JobSource: {0}, JobId: {1}, QueueTime: {2}, ExecutionStartTime: {3}, EndTime: {4}, AgentId: {5}, QueuedReasons: {6}, Priority: {7}, HistoryId: {8}, Result: {9}, ResultMessage: {10}]", (object) this.JobSource, (object) this.JobId, (object) this.QueueTime, (object) this.ExecutionStartTime, (object) this.EndTime, (object) this.AgentId, (object) this.QueuedReasons, (object) this.Priority, (object) this.HistoryId, (object) this.Result, (object) this.ResultMessage);
  }
}
