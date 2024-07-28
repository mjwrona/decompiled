// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationJobDefinition
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [CallOnSerialization("BeforeSerialize")]
  [CallOnDeserialization("AfterDeserialize")]
  public class TeamFoundationJobDefinition
  {
    private List<TeamFoundationJobSchedule> m_schedules = new List<TeamFoundationJobSchedule>();
    private bool m_isTemplateJobDefinition;
    internal static readonly int s_NameMaxLength = 128;
    internal static readonly int s_ExtensionNameMaxLength = 128;
    private static readonly XmlReaderSettings s_readerSettings = new XmlReaderSettings()
    {
      ConformanceLevel = ConformanceLevel.Fragment,
      DtdProcessing = DtdProcessing.Prohibit,
      XmlResolver = (XmlResolver) null
    };
    private bool m_queried;

    public TeamFoundationJobDefinition() => this.PriorityClass = TeamFoundationJobDefinition.DefaultPriorityClass;

    public TeamFoundationJobDefinition(Guid jobId, string name, string extensionName)
      : this(jobId, name, extensionName, (XmlNode) null)
    {
    }

    public TeamFoundationJobDefinition(
      Guid jobId,
      string name,
      string extensionName,
      XmlNode data)
      : this(jobId, name, extensionName, data, TeamFoundationJobEnabledState.Enabled, false, false, TeamFoundationJobDefinition.DefaultPriorityClass)
    {
    }

    public TeamFoundationJobDefinition(
      Guid jobId,
      string name,
      string extensionName,
      XmlNode data,
      TeamFoundationJobEnabledState enabledState)
      : this(jobId, name, extensionName, data, enabledState, false, false, TeamFoundationJobDefinition.DefaultPriorityClass)
    {
    }

    public TeamFoundationJobDefinition(
      Guid jobId,
      string name,
      string extensionName,
      XmlNode data,
      TeamFoundationJobEnabledState enabledState,
      bool ignoreDormancy)
      : this(jobId, name, extensionName, data, enabledState, false, ignoreDormancy, TeamFoundationJobDefinition.DefaultPriorityClass)
    {
    }

    public TeamFoundationJobDefinition(
      Guid jobId,
      string name,
      string extensionName,
      XmlNode data,
      TeamFoundationJobEnabledState enabledState,
      bool ignoreDormancy,
      JobPriorityClass priorityClass)
      : this(jobId, name, extensionName, data, enabledState, false, ignoreDormancy, priorityClass)
    {
    }

    internal TeamFoundationJobDefinition(
      Guid jobId,
      string name,
      string extensionName,
      XmlNode data,
      TeamFoundationJobEnabledState enabledState,
      bool runOnce,
      bool ignoreDormancy,
      JobPriorityClass priorityClass)
    {
      this.JobId = jobId;
      this.Name = name;
      this.ExtensionName = extensionName;
      this.Data = data;
      this.EnabledState = enabledState;
      this.RunOnce = runOnce;
      this.IgnoreDormancy = ignoreDormancy;
      if (priorityClass == JobPriorityClass.None)
        priorityClass = TeamFoundationJobDefinition.DefaultPriorityClass;
      this.PriorityClass = priorityClass;
      ArgumentUtility.CheckForEmptyGuid(jobId, nameof (JobId));
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

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public Guid JobId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public string Name { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public string ExtensionName { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public XmlNode Data { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public List<TeamFoundationJobSchedule> Schedule => this.m_schedules;

    [XmlIgnore]
    public TeamFoundationJobEnabledState EnabledState
    {
      get => (TeamFoundationJobEnabledState) this.EnabledStateValue;
      set => this.EnabledStateValue = (int) value;
    }

    [XmlAttribute("enabledState")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "EnabledState")]
    public int EnabledStateValue { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public bool IgnoreDormancy
    {
      get => (this.Flags & TeamFoundationJobDefinitionFlags.IgnoreDormancy) > TeamFoundationJobDefinitionFlags.None;
      set
      {
        if (value)
          this.Flags |= TeamFoundationJobDefinitionFlags.IgnoreDormancy;
        else
          this.Flags &= ~TeamFoundationJobDefinitionFlags.IgnoreDormancy;
      }
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public bool AllowDelete
    {
      get => (this.Flags & TeamFoundationJobDefinitionFlags.AllowDelete) > TeamFoundationJobDefinitionFlags.None;
      set
      {
        if (value)
          this.Flags |= TeamFoundationJobDefinitionFlags.AllowDelete;
        else
          this.Flags &= ~TeamFoundationJobDefinitionFlags.AllowDelete;
      }
    }

    internal bool RunOnce
    {
      get => (this.Flags & TeamFoundationJobDefinitionFlags.RunOnce) > TeamFoundationJobDefinitionFlags.None;
      set
      {
        if (value)
          this.Flags |= TeamFoundationJobDefinitionFlags.RunOnce;
        else
          this.Flags &= ~TeamFoundationJobDefinitionFlags.RunOnce;
      }
    }

    internal bool Queried
    {
      get => this.m_queried;
      set => this.m_queried |= value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public bool DisableDuringUpgrade
    {
      get => (this.Flags & TeamFoundationJobDefinitionFlags.DisableDuringUpgrade) > TeamFoundationJobDefinitionFlags.None;
      set
      {
        if (value)
          this.Flags |= TeamFoundationJobDefinitionFlags.DisableDuringUpgrade;
        else
          this.Flags &= ~TeamFoundationJobDefinitionFlags.DisableDuringUpgrade;
      }
    }

    internal bool UseServicingContext
    {
      get => (this.Flags & TeamFoundationJobDefinitionFlags.UseServicingContext) > TeamFoundationJobDefinitionFlags.None;
      set
      {
        if (value)
          this.Flags |= TeamFoundationJobDefinitionFlags.UseServicingContext;
        else
          this.Flags &= ~TeamFoundationJobDefinitionFlags.UseServicingContext;
      }
    }

    internal bool SelfService
    {
      get => (this.Flags & TeamFoundationJobDefinitionFlags.SelfService) > TeamFoundationJobDefinitionFlags.None;
      set
      {
        if (value)
          this.Flags |= TeamFoundationJobDefinitionFlags.SelfService;
        else
          this.Flags &= ~TeamFoundationJobDefinitionFlags.SelfService;
      }
    }

    internal DateTime? LastExecutionTime { get; set; }

    internal TeamFoundationJobDefinitionFlags Flags { get; set; }

    internal bool QueueAsDormant { get; set; }

    internal DateTime OverrideQueueTime { get; set; }

    [XmlIgnore]
    public bool IsTemplateJob
    {
      get => this.m_isTemplateJobDefinition;
      internal set => this.m_isTemplateJobDefinition |= value;
    }

    [XmlIgnore]
    public JobPriorityClass PriorityClass
    {
      get => (JobPriorityClass) this.PriorityClassValue;
      set => this.PriorityClassValue = (int) value;
    }

    [XmlAttribute("priorityClass")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "PriorityClass")]
    public int PriorityClassValue { get; set; }

    public TeamFoundationJobReference ToJobReference() => new TeamFoundationJobReference(this.JobId, this.PriorityClass);

    public TeamFoundationJobDefinition Clone()
    {
      TeamFoundationJobDefinition foundationJobDefinition = (TeamFoundationJobDefinition) this.MemberwiseClone();
      foundationJobDefinition.m_schedules = new List<TeamFoundationJobSchedule>(this.m_schedules.Select<TeamFoundationJobSchedule, TeamFoundationJobSchedule>((Func<TeamFoundationJobSchedule, TeamFoundationJobSchedule>) (x => x.Clone())));
      return foundationJobDefinition;
    }

    public override string ToString() => string.Format("JobId {0} Name {1} ExtensionName {2}", (object) this.JobId, (object) this.Name, (object) this.ExtensionName);

    internal void Validate(
      IVssRequestContext requestContext,
      string topLevelParamName,
      int minimumJobInterval,
      bool allowIgnoreDormancy)
    {
      PropertyValidation.CheckPropertyLength(this.Name, false, 1, TeamFoundationJobDefinition.s_NameMaxLength, "Name", this.GetType(), topLevelParamName);
      PropertyValidation.CheckPropertyLength(this.ExtensionName, false, 1, TeamFoundationJobDefinition.s_ExtensionNameMaxLength, "ExtensionName", this.GetType(), topLevelParamName);
      if (this.IgnoreDormancy && !allowIgnoreDormancy)
        throw new IgnoreDormancyProhibitedException(FrameworkResources.IgnoreDormancyProhibitedError((object) this));
      if (this.Data != null)
      {
        try
        {
          TeamFoundationJobDefinition.StringToJobDataNode(this.JobDataToString());
        }
        catch (Exception ex)
        {
          TeamFoundationTrace.TraceException(ex);
          throw new TeamFoundationValidationException(FrameworkResources.JobDataIsInvalid(), "Data", ex);
        }
      }
      if (this.Schedule == null)
        return;
      foreach (TeamFoundationJobSchedule foundationJobSchedule in this.Schedule)
        foundationJobSchedule.Validate(requestContext, topLevelParamName, minimumJobInterval);
    }

    internal string JobDataToString() => this.Data.OuterXml;

    internal static XmlNode StringToJobDataNode(string jobData)
    {
      XmlNode jobDataNode = (XmlNode) null;
      if (!string.IsNullOrEmpty(jobData))
      {
        using (StringReader input = new StringReader(jobData))
        {
          using (XmlReader reader = XmlReader.Create((TextReader) input, TeamFoundationJobDefinition.s_readerSettings))
            jobDataNode = new XmlDocument()
            {
              XmlResolver = ((XmlResolver) null)
            }.ReadNode(reader);
        }
      }
      return jobDataNode;
    }

    internal static JobPriorityClass DefaultPriorityClass => JobPriorityClass.Normal;
  }
}
