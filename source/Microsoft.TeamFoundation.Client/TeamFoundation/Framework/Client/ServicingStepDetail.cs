// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ServicingStepDetail
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public abstract class ServicingStepDetail
  {
    protected long m_detailId;
    protected DateTime m_detailTime = DateTime.MinValue;
    protected string m_servicingOperation;
    protected string m_servicingStepGroupId;
    protected string m_servicingStepId;

    public abstract string ToLogEntryLine();

    public long DetailId => this.m_detailId;

    public DateTime DetailTime => this.m_detailTime;

    public string ServicingOperation => this.m_servicingOperation;

    public string ServicingStepGroupId => this.m_servicingStepGroupId;

    public string ServicingStepId => this.m_servicingStepId;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ServicingStepDetail FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      switch (reader.GetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance"))
      {
        case "ServicingStepLogEntry":
          return (ServicingStepDetail) ServicingStepLogEntry.FromXml(serviceProvider, reader);
        case "ServicingStepStateChange":
          return (ServicingStepDetail) ServicingStepStateChange.FromXml(serviceProvider, reader);
        default:
          return (ServicingStepDetail) null;
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ServicingStepDetail instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  DetailId: " + this.m_detailId.ToString());
      stringBuilder.AppendLine("  DetailTime: " + this.m_detailTime.ToString());
      stringBuilder.AppendLine("  ServicingOperation: " + this.m_servicingOperation);
      stringBuilder.AppendLine("  ServicingStepGroupId: " + this.m_servicingStepGroupId);
      stringBuilder.AppendLine("  ServicingStepId: " + this.m_servicingStepId);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract void ToXml(XmlWriter writer, string element);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ServicingStepDetail obj) => obj.ToXml(writer, element);
  }
}
