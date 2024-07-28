// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ServicingJobDetail
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class ServicingJobDetail
  {
    private short m_completedStepCount;
    private DateTime m_endTime = DateTime.MinValue;
    private Guid m_hostId = Guid.Empty;
    private Guid m_jobId = Guid.Empty;
    private int m_jobStatusValue;
    private string m_operationClass;
    private string m_operationString;
    internal string[] m_operations = Helper.ZeroLengthArrayOfString;
    private int m_queuePosition;
    private DateTime m_queueTime = DateTime.MinValue;
    private int m_resultValue;
    private DateTime m_startTime = DateTime.MinValue;
    private short m_totalStepCount;

    public ServicingJobStatus JobStatus
    {
      get => (ServicingJobStatus) this.JobStatusValue;
      set => this.JobStatusValue = (int) value;
    }

    public ServicingJobResult Result
    {
      get => (ServicingJobResult) this.ResultValue;
      set => this.ResultValue = (int) value;
    }

    public ReadOnlyCollection<string> Operations => new ReadOnlyCollection<string>((IList<string>) this.m_operations);

    private ServicingJobDetail()
    {
    }

    public short CompletedStepCount
    {
      get => this.m_completedStepCount;
      set => this.m_completedStepCount = value;
    }

    public DateTime EndTime
    {
      get => this.m_endTime;
      set => this.m_endTime = value;
    }

    public Guid HostId
    {
      get => this.m_hostId;
      set => this.m_hostId = value;
    }

    public Guid JobId
    {
      get => this.m_jobId;
      set => this.m_jobId = value;
    }

    public int JobStatusValue
    {
      get => this.m_jobStatusValue;
      set => this.m_jobStatusValue = value;
    }

    public string OperationClass
    {
      get => this.m_operationClass;
      set => this.m_operationClass = value;
    }

    public string OperationString
    {
      get => this.m_operationString;
      set => this.m_operationString = value;
    }

    public int QueuePosition
    {
      get => this.m_queuePosition;
      set => this.m_queuePosition = value;
    }

    public DateTime QueueTime
    {
      get => this.m_queueTime;
      set => this.m_queueTime = value;
    }

    public int ResultValue
    {
      get => this.m_resultValue;
      set => this.m_resultValue = value;
    }

    public DateTime StartTime
    {
      get => this.m_startTime;
      set => this.m_startTime = value;
    }

    public short TotalStepCount
    {
      get => this.m_totalStepCount;
      set => this.m_totalStepCount = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ServicingJobDetail FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ServicingJobDetail servicingJobDetail = new ServicingJobDetail();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name = reader.Name;
          if (name != null)
          {
            switch (name.Length)
            {
              case 1:
                switch (name[0])
                {
                  case 'o':
                    servicingJobDetail.m_operationString = XmlUtility.StringFromXmlAttribute(reader);
                    continue;
                  case 'r':
                    servicingJobDetail.m_resultValue = XmlUtility.Int32FromXmlAttribute(reader);
                    continue;
                  default:
                    continue;
                }
              case 2:
                switch (name[0])
                {
                  case 'e':
                    if (name == "et")
                    {
                      servicingJobDetail.m_endTime = XmlUtility.DateTimeFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'j':
                    if (name == "js")
                    {
                      servicingJobDetail.m_jobStatusValue = XmlUtility.Int32FromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'o':
                    if (name == "oc")
                    {
                      servicingJobDetail.m_operationClass = XmlUtility.StringFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'q':
                    switch (name)
                    {
                      case "qp":
                        servicingJobDetail.m_queuePosition = XmlUtility.Int32FromXmlAttribute(reader);
                        continue;
                      case "qt":
                        servicingJobDetail.m_queueTime = XmlUtility.DateTimeFromXmlAttribute(reader);
                        continue;
                      default:
                        continue;
                    }
                  case 's':
                    if (name == "st")
                    {
                      servicingJobDetail.m_startTime = XmlUtility.DateTimeFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 3:
                switch (name[0])
                {
                  case 'c':
                    if (name == "csc")
                    {
                      servicingJobDetail.m_completedStepCount = XmlUtility.Int16FromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'h':
                    if (name == "hid")
                    {
                      servicingJobDetail.m_hostId = XmlUtility.GuidFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'j':
                    if (name == "jid")
                    {
                      servicingJobDetail.m_jobId = XmlUtility.GuidFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 't':
                    if (name == "tsc")
                    {
                      servicingJobDetail.m_totalStepCount = XmlUtility.Int16FromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              default:
                continue;
            }
          }
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.Name == "Operations")
            servicingJobDetail.m_operations = Helper.ArrayOfStringFromXml(reader, false);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return servicingJobDetail;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ServicingJobDetail instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  CompletedStepCount: " + this.m_completedStepCount.ToString());
      stringBuilder.AppendLine("  EndTime: " + this.m_endTime.ToString());
      stringBuilder.AppendLine("  HostId: " + this.m_hostId.ToString());
      stringBuilder.AppendLine("  JobId: " + this.m_jobId.ToString());
      stringBuilder.AppendLine("  JobStatusValue: " + this.m_jobStatusValue.ToString());
      stringBuilder.AppendLine("  OperationClass: " + this.m_operationClass);
      stringBuilder.AppendLine("  OperationString: " + this.m_operationString);
      stringBuilder.AppendLine("  Operations: " + Helper.ArrayToString<string>(this.m_operations));
      stringBuilder.AppendLine("  QueuePosition: " + this.m_queuePosition.ToString());
      stringBuilder.AppendLine("  QueueTime: " + this.m_queueTime.ToString());
      stringBuilder.AppendLine("  ResultValue: " + this.m_resultValue.ToString());
      stringBuilder.AppendLine("  StartTime: " + this.m_startTime.ToString());
      stringBuilder.AppendLine("  TotalStepCount: " + this.m_totalStepCount.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_completedStepCount != (short) 0)
        XmlUtility.ToXmlAttribute(writer, "csc", this.m_completedStepCount);
      if (this.m_endTime != DateTime.MinValue)
        XmlUtility.ToXmlAttribute(writer, "et", this.m_endTime);
      if (this.m_hostId != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "hid", this.m_hostId);
      if (this.m_jobId != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "jid", this.m_jobId);
      if (this.m_jobStatusValue != 0)
        XmlUtility.ToXmlAttribute(writer, "js", this.m_jobStatusValue);
      if (this.m_operationClass != null)
        XmlUtility.ToXmlAttribute(writer, "oc", this.m_operationClass);
      if (this.m_operationString != null)
        XmlUtility.ToXmlAttribute(writer, "o", this.m_operationString);
      if (this.m_queuePosition != 0)
        XmlUtility.ToXmlAttribute(writer, "qp", this.m_queuePosition);
      if (this.m_queueTime != DateTime.MinValue)
        XmlUtility.ToXmlAttribute(writer, "qt", this.m_queueTime);
      if (this.m_resultValue != 0)
        XmlUtility.ToXmlAttribute(writer, "r", this.m_resultValue);
      if (this.m_startTime != DateTime.MinValue)
        XmlUtility.ToXmlAttribute(writer, "st", this.m_startTime);
      if (this.m_totalStepCount != (short) 0)
        XmlUtility.ToXmlAttribute(writer, "tsc", this.m_totalStepCount);
      Helper.ToXml(writer, "Operations", this.m_operations, false, false);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ServicingJobDetail obj) => obj.ToXml(writer, element);
  }
}
