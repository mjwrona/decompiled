// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationJobDefinitionTemplate
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationJobDefinitionTemplate : ITeamFoundationJobDefinitionTemplate
  {
    [XmlIgnore]
    public TeamFoundationHostType HostType { get; set; }

    [XmlAttribute]
    public Guid JobId { get; set; }

    [XmlAttribute]
    public string JobName { get; set; }

    [XmlAttribute]
    public string PluginType { get; set; }

    [XmlElement]
    public XmlNode JobData { get; set; }

    [XmlAttribute]
    public TeamFoundationJobEnabledState EnabledState { get; set; }

    [XmlIgnore]
    public TeamFoundationJobDefinitionTemplateFlags Flags { get; set; }

    [XmlAttribute]
    public JobPriorityClass PriorityClass { get; set; } = TeamFoundationJobDefinition.DefaultPriorityClass;

    [XmlIgnore]
    public TimeSpan InitialStagger { get; set; }

    [XmlIgnore]
    internal bool PendingStaggering { get; set; }

    [XmlArray]
    [XmlArrayItem("ScheduleTemplate")]
    public List<TeamFoundationJobScheduleTemplate> Schedules { get; } = new List<TeamFoundationJobScheduleTemplate>(1);

    IEnumerable<ITeamFoundationJobScheduleTemplate> ITeamFoundationJobDefinitionTemplate.Schedules => (IEnumerable<ITeamFoundationJobScheduleTemplate>) this.Schedules;

    public override string ToString() => string.Format("[HostType={0}, JobId={1}, JobName={2}, PluginType={3}, InitialStagger={4}]", (object) this.HostType, (object) this.JobId, (object) this.JobName, (object) this.PluginType, (object) this.InitialStagger);

    [XmlAttribute("Flags")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string __InternalFlags__
    {
      get => this.Flags.ToString();
      set => this.Flags = (TeamFoundationJobDefinitionTemplateFlags) TeamFoundationJobDefinitionTemplate.ParseEnum<TeamFoundationJobDefinitionTemplateFlags>(value);
    }

    [XmlAttribute("HostType")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string __InternalHostType__
    {
      get => this.HostType.ToString();
      set => this.HostType = (TeamFoundationHostType) TeamFoundationJobDefinitionTemplate.ParseEnum<TeamFoundationHostType>(value);
    }

    [XmlAttribute("InitialStagger")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string __InternalInitialStagger__
    {
      get => this.InitialStagger.ToString();
      set => this.InitialStagger = TimeSpan.Parse(value);
    }

    private static int ParseEnum<T>(string str)
    {
      int num = 0;
      string str1 = str;
      char[] separator = new char[3]{ '|', ' ', ',' };
      foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
        num |= (int) Enum.Parse(typeof (T), str2, true);
      return num;
    }
  }
}
