// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildDefinition
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [RequiredClientService("BuildServer")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [CallOnDeserialization("AfterDeserialize")]
  [DataContract]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  [GenerateInterface(false)]
  public sealed class BuildDefinition : ICacheable, IValidatable, IPropertyProvider
  {
    private int m_id = -1;
    private string m_name;
    private string m_fullPath;
    private string m_securityToken;
    private List<Schedule> m_schedules = new List<Schedule>();
    private List<BuildDefinitionSourceProvider> m_sourceProviders = new List<BuildDefinitionSourceProvider>();
    private List<RetentionPolicy> m_retentionPolicies = new List<RetentionPolicy>();
    private List<PropertyValue> m_properties = new List<PropertyValue>();
    internal static readonly int MaxQuietPeriod = 525600;
    private static readonly Regex s_tfvcPathRegex = new Regex("\\$/[^\"<\\\\]*", RegexOptions.Compiled);

    public BuildDefinition()
    {
      this.BatchSize = 1;
      this.TriggerType = DefinitionTriggerType.None;
    }

    [DataMember]
    [XmlAttribute]
    [DefaultValue(1)]
    [ClientProperty(ClientVisibility.Public)]
    public int BatchSize { get; set; }

    [DataMember]
    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public string BuildControllerUri { get; set; }

    [XmlAttribute]
    [DefaultValue(DefinitionTriggerType.None)]
    [ClientProperty(ClientVisibility.Public)]
    public DefinitionTriggerType TriggerType { get; set; }

    [DataMember]
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public int ContinuousIntegrationQuietPeriod { get; set; }

    [DataMember]
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string DefaultDropLocation { get; set; }

    [DataMember]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string Description { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public DefinitionQueueStatus QueueStatus { get; set; }

    [DataMember]
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string FullPath
    {
      get => this.m_fullPath;
      set => this.m_fullPath = value;
    }

    [DataMember]
    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string LastBuildUri { get; set; }

    [DataMember]
    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string LastGoodBuildUri { get; set; }

    [DataMember]
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string LastGoodBuildLabel { get; set; }

    [XmlIgnore]
    public string Name
    {
      get
      {
        if (this.m_name == null)
          this.m_name = BuildPath.GetItemName(this.FullPath);
        return this.m_name;
      }
    }

    [ClientProperty(ClientVisibility.Private)]
    public ProcessTemplate Process { get; set; }

    [DataMember]
    [ClientProperty(ClientVisibility.Private)]
    public string ProcessParameters { get; set; }

    [ClientProperty(ClientVisibility.Private)]
    public List<RetentionPolicy> RetentionPolicies => this.m_retentionPolicies;

    [ClientProperty(ClientVisibility.Private)]
    public List<Schedule> Schedules => this.m_schedules;

    [ClientProperty(ClientVisibility.Private)]
    public List<BuildDefinitionSourceProvider> SourceProviders => this.m_sourceProviders;

    [ClientProperty(ClientVisibility.Internal, PropertyName = "InternalProperties")]
    public List<PropertyValue> Properties => this.m_properties;

    [DataMember]
    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public string Uri { get; set; }

    [ClientProperty(ClientVisibility.Private)]
    public WorkspaceTemplate WorkspaceTemplate { get; set; }

    [DataMember]
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime DateCreated { get; set; }

    [XmlIgnore]
    public int Id
    {
      get
      {
        if (this.m_id == -1)
          this.m_id = int.Parse(DBHelper.ExtractDbId(this.Uri), (IFormatProvider) CultureInfo.InvariantCulture);
        return this.m_id;
      }
    }

    internal string SecurityToken
    {
      get
      {
        if (this.m_securityToken == null)
          this.m_securityToken = this.TeamProject.SecurityToken + (object) BuildSecurity.NamespaceSeparator + (object) this.Id;
        return this.m_securityToken;
      }
    }

    internal TeamProject TeamProject { get; set; }

    internal int ProcessTemplateId { get; set; }

    internal Guid ScheduleJobId { get; set; }

    internal TimeSpan? ScheduleJobDelay { get; set; }

    int ICacheable.GetCachedSize() => 700;

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      if (context == ValidationContext.Update)
        ArgumentValidation.CheckUri("Uri", this.Uri, "Definition", false, ResourceStrings.MissingUri());
      ArgumentValidation.CheckUri("ControllerUri", this.BuildControllerUri, "Controller", true, (string) null);
      ArgumentValidation.CheckItemPath("FullPath", ref this.m_fullPath, false, false);
      this.TeamProject = requestContext.GetService<IProjectService>().GetTeamProjectFromGuidOrName(requestContext, BuildPath.GetTeamProject(this.m_fullPath));
      if (string.IsNullOrEmpty(this.Name))
        throw new ArgumentException(BuildTypeResource.MissingDefinitionName());
      if (this.Name.Contains("@") || this.Name.Contains("$"))
        throw new ArgumentException(BuildTypeResource.InvalidDefinitionNameInvalidCharacters((object) this.Name));
      string path = this.DefaultDropLocation ?? string.Empty;
      ArgumentValidation.CheckDropLocation("DropLocation", ref path, true, !requestContext.ExecutionEnvironment.IsOnPremisesDeployment, ResourceStrings.MissingDefaultDropLocation());
      if (VersionControlPath.IsServerItem(path))
      {
        string parent = VersionControlPath.Combine(VersionControlPath.PrependRootIfNeeded(this.TeamProject.Name), "Drops");
        if (VersionControlPath.GetFolderDepth(path) < 2)
          throw new ArgumentException(BuildTypeResource.InvalidDropLocation((object) path));
        if (!VersionControlPath.IsSubItem(path, parent))
          throw new ArgumentException(ResourceStrings.InvalidVersionControlDefaultDropLocation((object) path, (object) this.TeamProject.Name, (object) parent));
      }
      this.DefaultDropLocation = path;
      ArgumentValidation.CheckBound("QuietPeriod", this.ContinuousIntegrationQuietPeriod, -1, BuildDefinition.MaxQuietPeriod);
      Validation.CheckDescription("Description", this.Description, true);
      ArgumentValidation.CheckUri("BuildControllerUri", this.BuildControllerUri, "Controller", false, ResourceStrings.MissingController());
      Dictionary<BuildStatus, BuildReason> dictionary = new Dictionary<BuildStatus, BuildReason>();
      foreach (RetentionPolicy retentionPolicy in this.m_retentionPolicies)
      {
        if (retentionPolicy.BuildStatus != BuildStatus.Failed && retentionPolicy.BuildStatus != BuildStatus.PartiallySucceeded && retentionPolicy.BuildStatus != BuildStatus.Stopped && retentionPolicy.BuildStatus != BuildStatus.Succeeded)
          throw new ArgumentException(ResourceStrings.RetentionPolicyInvalidStatus((object) retentionPolicy.BuildStatus.ToString()));
        if (retentionPolicy.BuildReason != BuildReason.Triggered && retentionPolicy.BuildReason != BuildReason.ValidateShelveset)
          throw new ArgumentException(ResourceStrings.RetentionPolicyInvalidReason((object) retentionPolicy.BuildReason.ToString()));
        if (dictionary.ContainsKey(retentionPolicy.BuildStatus))
        {
          if ((dictionary[retentionPolicy.BuildStatus] & retentionPolicy.BuildReason) == retentionPolicy.BuildReason)
            throw new ArgumentException(ResourceStrings.DuplicateRetentionPolicyDefinition((object) retentionPolicy.BuildStatus.ToString(), (object) retentionPolicy.BuildReason.ToString()));
          dictionary[retentionPolicy.BuildStatus] |= retentionPolicy.BuildReason;
        }
        else
          dictionary.Add(retentionPolicy.BuildStatus, retentionPolicy.BuildReason);
        if ((retentionPolicy.DeleteOptions & DeleteOptions.Details) != DeleteOptions.Details)
          throw new ArgumentException(ResourceStrings.InvalidDeleteOptions((object) retentionPolicy.DeleteOptions));
      }
      if (this.m_schedules.Count > 0)
      {
        Validation.CheckValidatable<Schedule>(requestContext, "Schedules", (IList<Schedule>) this.m_schedules, false, context);
        if (this.m_schedules.Count > 1)
          throw new ArgumentException(ResourceStrings.ScheduleLimitExceeded());
      }
      if (this.m_sourceProviders.Count > 0)
      {
        Validation.CheckValidatable<BuildDefinitionSourceProvider>(requestContext, "SourceProviders", (IList<BuildDefinitionSourceProvider>) this.m_sourceProviders, false, context);
        if (this.m_sourceProviders.Count > 1)
          throw new ArgumentException(ResourceStrings.SourceProviderLimitExceeded());
      }
      if (this.Process != null)
        Validation.CheckValidatable(requestContext, "Process", (IValidatable) this.Process, false, context);
      else if (context == ValidationContext.Add)
        throw new ArgumentException(BuildTypeResource.InvalidInputParameterNull((object) "Process"));
      if (this.WorkspaceTemplate != null)
        Validation.CheckValidatable(requestContext, "WorkspaceTemplate", (IValidatable) this.WorkspaceTemplate, false, context);
      else if (context == ValidationContext.Add)
        throw new ArgumentException(BuildTypeResource.InvalidInputParameterNull((object) "WorkspaceTemplate"));
      if (this.TriggerType != DefinitionTriggerType.None && this.TriggerType != DefinitionTriggerType.ContinuousIntegration && this.TriggerType != DefinitionTriggerType.BatchedContinuousIntegration && this.TriggerType != DefinitionTriggerType.Schedule && this.TriggerType != DefinitionTriggerType.ScheduleForced && this.TriggerType != DefinitionTriggerType.GatedCheckIn && this.TriggerType != DefinitionTriggerType.BatchedGatedCheckIn)
        throw new ArgumentException(ResourceStrings.InvalidContinuousIntegrationType((object) this.TriggerType.ToString()));
      if (this.ContinuousIntegrationQuietPeriod != 0 && this.TriggerType != DefinitionTriggerType.BatchedContinuousIntegration)
        throw new ArgumentException(ResourceStrings.CannotSetQuietPeriodInvalidType((object) "BatchedContinuousIntegration"));
      if (this.TriggerType == DefinitionTriggerType.BatchedGatedCheckIn)
        ArgumentValidation.CheckBound("BatchSize", this.BatchSize, 2, int.MaxValue);
      else if (this.BatchSize != 1)
        throw new ArgumentException(ResourceStrings.CannotSetBatchSizeInvalidType((object) "BatchedGatedCheckIn"));
      int itemDepth = BuildPath.GetItemDepth(this.FullPath);
      if (itemDepth < 2)
        throw new InvalidPathException(ResourceStrings.BuildGroupItemNameRequired((object) this.FullPath));
      if (itemDepth > 2)
        throw new InvalidPathException(ResourceStrings.BuildGroupItemInvalidGroup((object) this.Name));
    }

    internal TeamFoundationJobDefinition GetScheduleJobDefinition(bool isIgnoreDormancyPermitted = true)
    {
      TeamFoundationJobDefinition scheduleJobDefinition = (TeamFoundationJobDefinition) null;
      if (this.TriggerType == DefinitionTriggerType.Schedule || this.TriggerType == DefinitionTriggerType.ScheduleForced || this.TriggerType == DefinitionTriggerType.BatchedContinuousIntegration)
      {
        XmlDocument xmlDocument = new XmlDocument();
        XmlNode element1 = (XmlNode) xmlDocument.CreateElement(nameof (BuildDefinition));
        XmlNode element2 = (XmlNode) xmlDocument.CreateElement("Uri");
        element2.AppendChild((XmlNode) xmlDocument.CreateTextNode(this.Uri));
        XmlNode element3 = (XmlNode) xmlDocument.CreateElement("TriggerType");
        element3.AppendChild((XmlNode) xmlDocument.CreateTextNode(this.TriggerType.ToString("G")));
        XmlNode element4 = (XmlNode) xmlDocument.CreateElement("ProjectId");
        element4.AppendChild((XmlNode) xmlDocument.CreateTextNode(this.TeamProject.Id.ToString("D")));
        element1.AppendChild(element2);
        element1.AppendChild(element3);
        element1.AppendChild(element4);
        scheduleJobDefinition = new TeamFoundationJobDefinition()
        {
          Data = element1,
          EnabledState = TeamFoundationJobEnabledState.Enabled,
          ExtensionName = "Microsoft.TeamFoundation.Build.JobService.Extensions.ScheduleJob",
          JobId = this.ScheduleJobId,
          Name = ResourceStrings.ScheduleJobName(),
          IgnoreDormancy = isIgnoreDormancyPermitted && this.TriggerType == DefinitionTriggerType.ScheduleForced,
          PriorityClass = JobPriorityClass.High
        };
        scheduleJobDefinition.Schedule.AddRange((IEnumerable<TeamFoundationJobSchedule>) this.ConvertSchedules());
      }
      return scheduleJobDefinition;
    }

    private List<TeamFoundationJobSchedule> ConvertSchedules()
    {
      List<TeamFoundationJobSchedule> foundationJobScheduleList = new List<TeamFoundationJobSchedule>();
      if (this.m_schedules.Count == 0)
        return foundationJobScheduleList;
      int num = 604800;
      DateTime dateTime1;
      ref DateTime local = ref dateTime1;
      DateTime utcNow = DateTime.UtcNow;
      int year = utcNow.Year;
      utcNow = DateTime.UtcNow;
      int month = utcNow.Month;
      utcNow = DateTime.UtcNow;
      int day = utcNow.Day;
      local = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
      while (dateTime1.DayOfWeek != DayOfWeek.Sunday)
        dateTime1 = dateTime1.AddDays(-1.0);
      foreach (Schedule schedule in this.m_schedules)
      {
        DateTime dateTime2 = dateTime1.Add(TimeSpan.FromSeconds((double) schedule.UtcStartTime));
        if ((schedule.UtcDaysToBuild & ScheduleDays.Sunday) != ScheduleDays.None)
          foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
          {
            Interval = num,
            ScheduledTime = dateTime2,
            TimeZoneId = schedule.TimeZoneId,
            PriorityLevel = JobPriorityLevel.Highest
          });
        if ((schedule.UtcDaysToBuild & ScheduleDays.Monday) != ScheduleDays.None)
          foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
          {
            Interval = num,
            ScheduledTime = dateTime2.AddDays(1.0),
            TimeZoneId = schedule.TimeZoneId,
            PriorityLevel = JobPriorityLevel.Highest
          });
        if ((schedule.UtcDaysToBuild & ScheduleDays.Tuesday) != ScheduleDays.None)
          foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
          {
            Interval = num,
            ScheduledTime = dateTime2.AddDays(2.0),
            TimeZoneId = schedule.TimeZoneId,
            PriorityLevel = JobPriorityLevel.Highest
          });
        if ((schedule.UtcDaysToBuild & ScheduleDays.Wednesday) != ScheduleDays.None)
          foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
          {
            Interval = num,
            ScheduledTime = dateTime2.AddDays(3.0),
            TimeZoneId = schedule.TimeZoneId,
            PriorityLevel = JobPriorityLevel.Highest
          });
        if ((schedule.UtcDaysToBuild & ScheduleDays.Thursday) != ScheduleDays.None)
          foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
          {
            Interval = num,
            ScheduledTime = dateTime2.AddDays(4.0),
            TimeZoneId = schedule.TimeZoneId,
            PriorityLevel = JobPriorityLevel.Highest
          });
        if ((schedule.UtcDaysToBuild & ScheduleDays.Friday) != ScheduleDays.None)
          foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
          {
            Interval = num,
            ScheduledTime = dateTime2.AddDays(5.0),
            TimeZoneId = schedule.TimeZoneId,
            PriorityLevel = JobPriorityLevel.Highest
          });
        if ((schedule.UtcDaysToBuild & ScheduleDays.Saturday) != ScheduleDays.None)
          foundationJobScheduleList.Add(new TeamFoundationJobSchedule()
          {
            Interval = num,
            ScheduledTime = dateTime2.AddDays(6.0),
            TimeZoneId = schedule.TimeZoneId,
            PriorityLevel = JobPriorityLevel.Highest
          });
      }
      return foundationJobScheduleList;
    }

    internal void ScheduleRollingJob(IVssRequestContext requestContext)
    {
      if (!this.ScheduleJobDelay.HasValue || this.TriggerType != DefinitionTriggerType.BatchedContinuousIntegration)
        return;
      requestContext.GetService<TeamFoundationJobService>().QueueDelayedJobs(requestContext.Elevate(), (IEnumerable<Guid>) new Guid[1]
      {
        this.ScheduleJobId
      }, (int) this.ScheduleJobDelay.Value.TotalSeconds);
    }

    internal static void ReadDatabaseResults(
      IVssRequestContext requestContext,
      List<BuildDefinition> definitions,
      ResultCollection dbResult)
    {
      dbResult.RequestContext.TraceEnter(0, "Build", "Database", nameof (ReadDatabaseResults));
      dbResult.NextResult();
      List<RetentionPolicy> items1 = dbResult.GetCurrent<RetentionPolicy>().Items;
      dbResult.NextResult();
      List<Schedule> items2 = dbResult.GetCurrent<Schedule>().Items;
      dbResult.NextResult();
      List<BuildDefinitionSourceProvider> source;
      try
      {
        source = dbResult.GetCurrent<BuildDefinitionSourceProvider>().Items;
        dbResult.NextResult();
      }
      catch (InvalidCastException ex)
      {
        source = new List<BuildDefinitionSourceProvider>();
      }
      List<ProcessTemplate> items3 = dbResult.GetCurrent<ProcessTemplate>().Items;
      dbResult.NextResult();
      List<WorkspaceTemplate> items4 = dbResult.GetCurrent<WorkspaceTemplate>().Items;
      dbResult.NextResult();
      List<WorkspaceMapping> items5 = dbResult.GetCurrent<WorkspaceMapping>().Items;
      Dictionary<Tuple<Guid, string>, BuildDefinition> targetDictionary1 = new Dictionary<Tuple<Guid, string>, BuildDefinition>();
      foreach (BuildDefinition definition in definitions)
      {
        Tuple<Guid, string> key = new Tuple<Guid, string>(definition.TeamProject.Id, definition.Uri);
        targetDictionary1.Add(key, definition);
      }
      Dictionary<Tuple<Guid, int>, ProcessTemplate> targetDictionary2 = new Dictionary<Tuple<Guid, int>, ProcessTemplate>();
      foreach (ProcessTemplate processTemplate in items3)
        targetDictionary2[new Tuple<Guid, int>(processTemplate.TeamProjectObj.Id, processTemplate.Id)] = processTemplate;
      BuildIdentityResolver identityResolver = new BuildIdentityResolver();
      foreach (WorkspaceTemplate workspaceTemplate in items4)
        workspaceTemplate.LastModifiedBy = identityResolver.GetUniqueName(requestContext, workspaceTemplate.LastModifiedBy);
      DBHelper.Match<WorkspaceTemplate, WorkspaceMapping, Tuple<Guid, int>>(items4, items5, (Func<WorkspaceTemplate, Tuple<Guid, int>>) (x => new Tuple<Guid, int>(x.ProjectId, x.WorkspaceId)), (Func<WorkspaceMapping, Tuple<Guid, int>>) (x => new Tuple<Guid, int>(x.ProjectId, x.WorkspaceId)), (Action<WorkspaceTemplate, WorkspaceMapping>) ((x, y) => x.Mappings.Add(y)), (Func<Tuple<Guid, int>, Tuple<Guid, int>, bool>) ((x, y) => x.Item1 == y.Item1 && x.Item2 == y.Item2));
      DBHelper.Match<BuildDefinition, Schedule, Tuple<Guid, string>>(targetDictionary1, (IEnumerable<Schedule>) items2, (Func<Schedule, Tuple<Guid, string>>) (x => new Tuple<Guid, string>(x.ProjectId, x.DefinitionUri)), (Action<BuildDefinition, Schedule>) ((x, y) => x.Schedules.Add(y)));
      DBHelper.Match<BuildDefinition, BuildDefinitionSourceProvider, Tuple<Guid, string>>(targetDictionary1, (IEnumerable<BuildDefinitionSourceProvider>) source, (Func<BuildDefinitionSourceProvider, Tuple<Guid, string>>) (x => new Tuple<Guid, string>(x.ProjectId, x.DefinitionUri)), (Action<BuildDefinition, BuildDefinitionSourceProvider>) ((x, y) => x.SourceProviders.Add(y)));
      DBHelper.Match<BuildDefinition, RetentionPolicy, Tuple<Guid, string>>(targetDictionary1, (IEnumerable<RetentionPolicy>) items1, (Func<RetentionPolicy, Tuple<Guid, string>>) (x => new Tuple<Guid, string>(x.ProjectId, x.DefinitionUri)), (Action<BuildDefinition, RetentionPolicy>) ((x, y) => x.RetentionPolicies.Add(y)));
      DBHelper.Match<ProcessTemplate, BuildDefinition, Tuple<Guid, int>>(targetDictionary2, (IEnumerable<BuildDefinition>) definitions, (Func<BuildDefinition, Tuple<Guid, int>>) (x => new Tuple<Guid, int>(x.TeamProject.Id, x.ProcessTemplateId)), (Action<ProcessTemplate, BuildDefinition>) ((x, y) => y.Process = x));
      DBHelper.Match<BuildDefinition, WorkspaceTemplate, Tuple<Guid, string>>(targetDictionary1, (IEnumerable<WorkspaceTemplate>) items4, (Func<WorkspaceTemplate, Tuple<Guid, string>>) (x => new Tuple<Guid, string>(x.ProjectId, x.DefinitionUri)), (Action<BuildDefinition, WorkspaceTemplate>) ((x, y) => x.WorkspaceTemplate = y));
      definitions.ConvertObjectsToProjectName(requestContext);
      dbResult.RequestContext.TraceLeave(0, "Build", "Database", nameof (ReadDatabaseResults));
    }

    internal static void ConvertResultDefinitions(
      IVssRequestContext requestContext,
      List<BuildDefinition> definitions)
    {
      definitions.ConvertObjectsToProjectName(requestContext);
    }

    internal void CopyPropertiesFrom(BuildDefinition definition)
    {
      this.m_properties = new List<PropertyValue>();
      if (definition.Properties == null)
        return;
      foreach (PropertyValue property in definition.Properties)
        this.m_properties.Add(property);
    }

    internal void ConvertProcessParametersToProjectGuid(IVssRequestContext requestContext)
    {
      if (string.IsNullOrEmpty(this.ProcessParameters))
        return;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.ProcessParameters = BuildDefinition.s_tfvcPathRegex.Replace(this.ProcessParameters, (MatchEvaluator) (match => BuildDefinition.ConvertParameterValue(requestContext, match.Value, BuildDefinition.\u003C\u003EO.\u003C0\u003E__ConvertToPathWithProjectGuid ?? (BuildDefinition.\u003C\u003EO.\u003C0\u003E__ConvertToPathWithProjectGuid = new Func<IVssRequestContext, string, string>(TFVCPathHelper.ConvertToPathWithProjectGuid)))));
    }

    internal void ConvertProcessParametersToProjectName(IVssRequestContext requestContext)
    {
      if (string.IsNullOrEmpty(this.ProcessParameters))
        return;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.ProcessParameters = BuildDefinition.s_tfvcPathRegex.Replace(this.ProcessParameters, (MatchEvaluator) (match => BuildDefinition.ConvertParameterValue(requestContext, match.Value, BuildDefinition.\u003C\u003EO.\u003C1\u003E__ConvertToPathWithProjectName ?? (BuildDefinition.\u003C\u003EO.\u003C1\u003E__ConvertToPathWithProjectName = new Func<IVssRequestContext, string, string>(TFVCPathHelper.ConvertToPathWithProjectName)))));
    }

    private static string ConvertParameterValue(
      IVssRequestContext requestContext,
      string input,
      Func<IVssRequestContext, string, string> convertPath)
    {
      if (input.IndexOf(",$/", 0) < 0)
        return convertPath(requestContext, input);
      StringBuilder stringBuilder = new StringBuilder();
      string str1 = input;
      string[] separator = new string[1]{ ",$/" };
      foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        string str3 = str2;
        if (str3[0] != '$')
          str3 = "$/" + str2;
        if (stringBuilder.Length > 0)
          stringBuilder.Append(",");
        stringBuilder.Append(convertPath(requestContext, str3));
      }
      return stringBuilder.ToString();
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildDefinition Name={0} BuildControllerUri={1} FullPath={2} TriggerType={3}]", (object) this.Name, (object) this.BuildControllerUri, (object) this.FullPath, (object) this.TriggerType);
  }
}
