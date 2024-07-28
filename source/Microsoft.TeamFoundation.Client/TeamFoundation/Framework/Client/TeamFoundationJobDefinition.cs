// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TeamFoundationJobDefinition
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class TeamFoundationJobDefinition
  {
    private List<TeamFoundationJobSchedule> m_schedulesList = new List<TeamFoundationJobSchedule>();
    private XmlNode m_data;
    private int m_enabledState;
    private string m_extensionName;
    private bool m_ignoreDormancy;
    private bool m_allowDelete;
    private bool m_disableDuringUpgrade;
    private Guid m_jobId = Guid.Empty;
    private string m_name;
    private int m_priorityClass;
    internal TeamFoundationJobSchedule[] m_schedule = Helper.ZeroLengthArrayOfTeamFoundationJobSchedule;

    public TeamFoundationJobDefinition(
      Guid jobId,
      string name,
      string extensionName,
      XmlNode data)
      : this(jobId, name, extensionName, data, TeamFoundationJobEnabledState.Enabled)
    {
    }

    public TeamFoundationJobDefinition(
      Guid jobId,
      string name,
      string extensionName,
      XmlNode data,
      TeamFoundationJobEnabledState enabledState)
      : this(jobId, name, extensionName, data, enabledState, false)
    {
    }

    public TeamFoundationJobDefinition(
      Guid jobId,
      string name,
      string extensionName,
      XmlNode data,
      TeamFoundationJobEnabledState enabledState,
      bool ignoreDormancy)
    {
      this.m_jobId = jobId;
      this.m_name = name;
      this.m_extensionName = extensionName;
      this.m_data = data;
      this.m_enabledState = (int) enabledState;
      this.m_ignoreDormancy = ignoreDormancy;
      ArgumentUtility.CheckStringForNullOrEmpty(this.Name, nameof (Name));
      ArgumentUtility.CheckStringForNullOrEmpty(this.ExtensionName, nameof (ExtensionName));
    }

    public TeamFoundationJobDefinition(string name, string extensionName, XmlNode data)
      : this(Guid.NewGuid(), name, extensionName, data)
    {
    }

    public TeamFoundationJobDefinition(string name, string extensionName)
      : this(name, extensionName, (XmlNode) null)
    {
    }

    public IList<TeamFoundationJobSchedule> Schedule => (IList<TeamFoundationJobSchedule>) this.m_schedulesList;

    public string Name
    {
      get => this.m_name;
      set
      {
        ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (Name));
        this.m_name = value;
      }
    }

    public string ExtensionName
    {
      get => this.m_extensionName;
      set
      {
        ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (ExtensionName));
        this.m_extensionName = value;
      }
    }

    public TeamFoundationJobEnabledState EnabledState
    {
      get => (TeamFoundationJobEnabledState) this.m_enabledState;
      set => this.m_enabledState = (int) value;
    }

    public XmlNode Data
    {
      get => this.m_data;
      set => throw new InvalidOperationException(ClientResources.JobDefinitionPropertiesCannotSet());
    }

    private void BeforeSerialize() => this.m_schedule = this.m_schedulesList.ToArray();

    private void AfterDeserialize() => this.m_schedulesList.AddRange((IEnumerable<TeamFoundationJobSchedule>) this.m_schedule);

    private TeamFoundationJobDefinition()
    {
    }

    public bool IgnoreDormancy
    {
      get => this.m_ignoreDormancy;
      set => this.m_ignoreDormancy = value;
    }

    public Guid JobId => this.m_jobId;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TeamFoundationJobDefinition FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition();
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
              case 4:
                if (name == "Name")
                {
                  foundationJobDefinition.m_name = XmlUtility.StringFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 5:
                if (name == "JobId")
                {
                  foundationJobDefinition.m_jobId = XmlUtility.GuidFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 11:
                if (name == "AllowDelete")
                {
                  foundationJobDefinition.m_allowDelete = XmlUtility.BooleanFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 12:
                if (name == "enabledState")
                {
                  foundationJobDefinition.m_enabledState = XmlUtility.Int32FromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 13:
                switch (name[0])
                {
                  case 'E':
                    if (name == "ExtensionName")
                    {
                      foundationJobDefinition.m_extensionName = XmlUtility.StringFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'p':
                    if (name == "priorityClass")
                    {
                      foundationJobDefinition.m_priorityClass = XmlUtility.Int32FromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 14:
                if (name == "IgnoreDormancy")
                {
                  foundationJobDefinition.m_ignoreDormancy = XmlUtility.BooleanFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 20:
                if (name == "DisableDuringUpgrade")
                {
                  foundationJobDefinition.m_disableDuringUpgrade = XmlUtility.BooleanFromXmlAttribute(reader);
                  continue;
                }
                continue;
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
          switch (reader.Name)
          {
            case "Data":
              foundationJobDefinition.m_data = XmlUtility.XmlNodeFromXmlElement(reader);
              continue;
            case "Schedule":
              foundationJobDefinition.m_schedule = Helper.ArrayOfTeamFoundationJobScheduleFromXml(serviceProvider, reader, false);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      foundationJobDefinition.AfterDeserialize();
      return foundationJobDefinition;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("TeamFoundationJobDefinition instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Data: " + this.m_data?.ToString());
      stringBuilder.AppendLine("  EnabledState: " + this.m_enabledState.ToString());
      stringBuilder.AppendLine("  ExtensionName: " + this.m_extensionName);
      stringBuilder.AppendLine("  IgnoreDormancy: " + this.m_ignoreDormancy.ToString());
      stringBuilder.AppendLine("  JobId: " + this.m_jobId.ToString());
      stringBuilder.AppendLine("  Name: " + this.m_name);
      stringBuilder.AppendLine("  PriorityClass: " + this.m_priorityClass.ToString());
      stringBuilder.AppendLine("  Schedule: " + Helper.ArrayToString<TeamFoundationJobSchedule>(this.m_schedule));
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      this.BeforeSerialize();
      writer.WriteStartElement(element);
      if (this.m_enabledState != 0)
        XmlUtility.ToXmlAttribute(writer, "enabledState", this.m_enabledState);
      if (this.m_extensionName != null)
        XmlUtility.ToXmlAttribute(writer, "ExtensionName", this.m_extensionName);
      if (this.m_ignoreDormancy)
        XmlUtility.ToXmlAttribute(writer, "IgnoreDormancy", this.m_ignoreDormancy);
      if (this.m_allowDelete)
        XmlUtility.ToXmlAttribute(writer, "AllowDelete", this.m_allowDelete);
      if (this.m_disableDuringUpgrade)
        XmlUtility.ToXmlAttribute(writer, "DisableDuringUpgrade", this.m_disableDuringUpgrade);
      if (this.m_jobId != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "JobId", this.m_jobId);
      if (this.m_name != null)
        XmlUtility.ToXmlAttribute(writer, "Name", this.m_name);
      if (this.m_priorityClass != 0)
        XmlUtility.ToXmlAttribute(writer, "priorityClass", this.m_priorityClass);
      if (this.m_data != null)
        XmlUtility.ToXmlElement(writer, "Data", this.m_data);
      Helper.ToXml(writer, "Schedule", this.m_schedule, false, false);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, TeamFoundationJobDefinition obj) => obj.ToXml(writer, element);
  }
}
