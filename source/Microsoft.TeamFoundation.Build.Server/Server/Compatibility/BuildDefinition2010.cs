// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildDefinition2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [CallOnDeserialization("AfterDeserialize")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [RequiredClientService("BuildServer")]
  [XmlType("BuildDefinition")]
  public sealed class BuildDefinition2010 : BuildGroupItem2010, ICacheable, IValidatable
  {
    private int m_id = -1;
    private string m_securityToken;
    private List<Schedule2010> m_schedules = new List<Schedule2010>();
    private List<RetentionPolicy2010> m_retentionPolicies = new List<RetentionPolicy2010>();
    private static readonly Regex s_tfvcPathRegex = new Regex("\\$/[^\"<]*", RegexOptions.Compiled);

    public BuildDefinition2010() => this.ContinuousIntegrationType = ContinuousIntegrationType.None;

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public)]
    public string BuildControllerUri { get; set; }

    [XmlAttribute]
    [DefaultValue(ContinuousIntegrationType.None)]
    [ClientProperty(ClientVisibility.Public)]
    public ContinuousIntegrationType ContinuousIntegrationType { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public int ContinuousIntegrationQuietPeriod { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string DefaultDropLocation { get; set; }

    [ClientProperty(ClientVisibility.Public)]
    public string Description { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public bool Enabled { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public)]
    public string LastBuildUri { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public)]
    public string LastGoodBuildUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string LastGoodBuildLabel { get; set; }

    [ClientProperty(ClientVisibility.Public)]
    public ProcessTemplate2010 Process { get; set; }

    [ClientProperty(ClientVisibility.Public)]
    public string ProcessParameters { get; set; }

    [ClientProperty(ClientVisibility.Public)]
    public List<RetentionPolicy2010> RetentionPolicies => this.m_retentionPolicies;

    [ClientProperty(ClientVisibility.Public)]
    public List<Schedule2010> Schedules => this.m_schedules;

    [ClientProperty(ClientVisibility.Public)]
    public WorkspaceTemplate2010 WorkspaceTemplate { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public)]
    public string DefaultBuildAgentUri { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public)]
    public string ConfigurationFolderUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public int MaxTimeout { get; set; }

    internal int Id
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
        {
          string toolSpecificId = LinkingUtilities.DecodeUri(this.Uri).ToolSpecificId;
          this.m_securityToken = this.TeamProject.SecurityToken + (object) BuildSecurity.NamespaceSeparator + toolSpecificId;
        }
        return this.m_securityToken;
      }
    }

    internal int ProcessTemplateId { get; set; }

    internal Guid ScheduleJobId { get; set; }

    int ICacheable.GetCachedSize() => 700;

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      this.Validate(requestContext, context);
      if (string.IsNullOrEmpty(this.Name))
        throw new ArgumentException(BuildTypeResource.MissingDefinitionName());
      if (this.Name.Contains("@") || this.Name.Contains("$"))
        throw new ArgumentException(BuildTypeResource.InvalidDefinitionNameInvalidCharacters((object) this.Name));
      string defaultDropLocation = this.DefaultDropLocation;
      ArgumentValidation.CheckDropLocation("DropLocation", ref defaultDropLocation, true, !requestContext.ExecutionEnvironment.IsOnPremisesDeployment, ResourceStrings.MissingDefaultDropLocation());
      if (VersionControlPath.IsServerItem(defaultDropLocation))
      {
        string parent = VersionControlPath.Combine(VersionControlPath.PrependRootIfNeeded(this.TeamProject.Name), "Drops");
        if (VersionControlPath.GetFolderDepth(defaultDropLocation) < 2)
          throw new ArgumentException(BuildTypeResource.InvalidDropLocation((object) defaultDropLocation));
        if (!VersionControlPath.IsSubItem(defaultDropLocation, parent))
          throw new ArgumentException(ResourceStrings.InvalidVersionControlDefaultDropLocation((object) defaultDropLocation, (object) this.TeamProject.Name, (object) parent));
      }
      this.DefaultDropLocation = defaultDropLocation;
      ArgumentValidation.CheckBound("MaxTimeout", this.MaxTimeout, -1, int.MaxValue);
      ArgumentValidation.CheckBound("QuietPeriod", this.ContinuousIntegrationQuietPeriod, -1, int.MaxValue);
      Microsoft.TeamFoundation.Build.Server.Validation.CheckDescription("Description", this.Description, true);
      if (context == ValidationContext.Update)
        ArgumentValidation.CheckUri("Uri", this.Uri, "Definition", false, (string) null);
      ArgumentValidation.CheckUri("BuildControllerUri", this.BuildControllerUri, "Controller", false, ResourceStrings.MissingController());
      Dictionary<BuildStatus2010, BuildReason2010> dictionary = new Dictionary<BuildStatus2010, BuildReason2010>();
      foreach (RetentionPolicy2010 retentionPolicy in this.m_retentionPolicies)
      {
        if (retentionPolicy.BuildStatus != BuildStatus2010.Failed && retentionPolicy.BuildStatus != BuildStatus2010.PartiallySucceeded && retentionPolicy.BuildStatus != BuildStatus2010.Stopped && retentionPolicy.BuildStatus != BuildStatus2010.Succeeded)
          throw new ArgumentException(ResourceStrings.RetentionPolicyInvalidStatus((object) retentionPolicy.BuildStatus.ToString()));
        if (retentionPolicy.BuildReason != BuildReason2010.Triggered && retentionPolicy.BuildReason != BuildReason2010.ValidateShelveset)
          throw new ArgumentException(ResourceStrings.RetentionPolicyInvalidReason((object) retentionPolicy.BuildReason.ToString()));
        if (dictionary.ContainsKey(retentionPolicy.BuildStatus))
        {
          if ((dictionary[retentionPolicy.BuildStatus] & retentionPolicy.BuildReason) == retentionPolicy.BuildReason)
            throw new ArgumentException(ResourceStrings.DuplicateRetentionPolicyDefinition((object) retentionPolicy.BuildStatus.ToString(), (object) retentionPolicy.BuildReason.ToString()));
          dictionary[retentionPolicy.BuildStatus] |= retentionPolicy.BuildReason;
        }
        else
          dictionary.Add(retentionPolicy.BuildStatus, retentionPolicy.BuildReason);
        if ((retentionPolicy.DeleteOptions & DeleteOptions2010.Details) != DeleteOptions2010.Details)
          throw new ArgumentException(ResourceStrings.InvalidDeleteOptions((object) retentionPolicy.DeleteOptions));
      }
      if (this.m_schedules.Count > 0)
      {
        Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable<Schedule2010>(requestContext, "Schedules", (IList<Schedule2010>) this.m_schedules, false, context);
        if (this.m_schedules.Count > 1)
          throw new ArgumentException(ResourceStrings.ScheduleLimitExceeded());
      }
      if (this.Process != null)
        Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable(requestContext, "Process", (IValidatable) this.Process, false, context);
      else if (context == ValidationContext.Add)
        throw new ArgumentException(BuildTypeResource.InvalidInputParameterNull((object) "Process"));
      if (this.WorkspaceTemplate != null)
        Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable(requestContext, "WorkspaceTemplate", (IValidatable) this.WorkspaceTemplate, false, context);
      else if (context == ValidationContext.Add)
        throw new ArgumentException(BuildTypeResource.InvalidInputParameterNull((object) "WorkspaceTemplate"));
      if (this.ContinuousIntegrationType != ContinuousIntegrationType.None && this.ContinuousIntegrationType != ContinuousIntegrationType.Individual && this.ContinuousIntegrationType != ContinuousIntegrationType.Batch && this.ContinuousIntegrationType != ContinuousIntegrationType.Schedule && this.ContinuousIntegrationType != ContinuousIntegrationType.ScheduleForced && this.ContinuousIntegrationType != ContinuousIntegrationType.Gated)
        throw new ArgumentException(ResourceStrings.InvalidContinuousIntegrationType((object) this.ContinuousIntegrationType.ToString()));
      if (this.ContinuousIntegrationQuietPeriod != 0 && this.ContinuousIntegrationType != ContinuousIntegrationType.Batch)
        throw new ArgumentException(ResourceStrings.CannotSetQuietPeriodInvalidType((object) "Batch"));
      int itemDepth = BuildPath.GetItemDepth(this.FullPath);
      if (itemDepth < 2)
        throw new InvalidPathException(ResourceStrings.BuildGroupItemNameRequired((object) this.FullPath));
      if (itemDepth > 2)
        throw new InvalidPathException(ResourceStrings.BuildGroupItemInvalidGroup((object) this.Name));
    }

    internal static void ReadDatabaseResults(
      IEnumerable<BuildDefinition2010> definitions,
      ResultCollection dbResult)
    {
      dbResult.RequestContext.TraceEnter(0, "Build", "Database", nameof (ReadDatabaseResults));
      dbResult.NextResult();
      List<RetentionPolicy2010> items1 = dbResult.GetCurrent<RetentionPolicy2010>().Items;
      dbResult.NextResult();
      List<Schedule2010> items2 = dbResult.GetCurrent<Schedule2010>().Items;
      dbResult.NextResult();
      List<ProcessTemplate2010> items3 = dbResult.GetCurrent<ProcessTemplate2010>().Items;
      dbResult.NextResult();
      List<WorkspaceTemplate2010> items4 = dbResult.GetCurrent<WorkspaceTemplate2010>().Items;
      dbResult.NextResult();
      List<WorkspaceMapping2010> items5 = dbResult.GetCurrent<WorkspaceMapping2010>().Items;
      Dictionary<Tuple<Guid, string>, BuildDefinition2010> dictionary = definitions.ToDictionary<BuildDefinition2010, Tuple<Guid, string>>((Func<BuildDefinition2010, Tuple<Guid, string>>) (x => new Tuple<Guid, string>(x.TeamProject.Id, x.Uri)));
      Dictionary<Tuple<Guid, int>, ProcessTemplate2010> targetDictionary = new Dictionary<Tuple<Guid, int>, ProcessTemplate2010>();
      foreach (ProcessTemplate2010 processTemplate2010 in items3)
        targetDictionary[new Tuple<Guid, int>(processTemplate2010.TeamProjectObj.Id, processTemplate2010.Id)] = processTemplate2010;
      DBHelper.Match<WorkspaceTemplate2010, WorkspaceMapping2010, Tuple<Guid, int>>(items4, items5, (Func<WorkspaceTemplate2010, Tuple<Guid, int>>) (x => new Tuple<Guid, int>(x.ProjectId, x.WorkspaceId)), (Func<WorkspaceMapping2010, Tuple<Guid, int>>) (x => new Tuple<Guid, int>(x.ProjectId, x.WorkspaceId)), (Action<WorkspaceTemplate2010, WorkspaceMapping2010>) ((x, y) => x.Mappings.Add(y)), (Func<Tuple<Guid, int>, Tuple<Guid, int>, bool>) ((x, y) => x.Item1 == y.Item1 && x.Item2 == y.Item2));
      DBHelper.Match<BuildDefinition2010, Schedule2010, Tuple<Guid, string>>(dictionary, (IEnumerable<Schedule2010>) items2, (Func<Schedule2010, Tuple<Guid, string>>) (x => new Tuple<Guid, string>(x.ProjectId, x.DefinitionUri)), (Action<BuildDefinition2010, Schedule2010>) ((x, y) => x.Schedules.Add(y)));
      DBHelper.Match<BuildDefinition2010, RetentionPolicy2010, Tuple<Guid, string>>(dictionary, (IEnumerable<RetentionPolicy2010>) items1, (Func<RetentionPolicy2010, Tuple<Guid, string>>) (x => new Tuple<Guid, string>(x.ProjectId, x.DefinitionUri)), (Action<BuildDefinition2010, RetentionPolicy2010>) ((x, y) => x.RetentionPolicies.Add(y)));
      DBHelper.Match<ProcessTemplate2010, BuildDefinition2010, Tuple<Guid, int>>(targetDictionary, definitions, (Func<BuildDefinition2010, Tuple<Guid, int>>) (x => new Tuple<Guid, int>(x.TeamProject.Id, x.ProcessTemplateId)), (Action<ProcessTemplate2010, BuildDefinition2010>) ((x, y) => y.Process = x));
      DBHelper.Match<BuildDefinition2010, WorkspaceTemplate2010, Tuple<Guid, string>>(dictionary, (IEnumerable<WorkspaceTemplate2010>) items4, (Func<WorkspaceTemplate2010, Tuple<Guid, string>>) (x => new Tuple<Guid, string>(x.ProjectId, x.DefinitionUri)), (Action<BuildDefinition2010, WorkspaceTemplate2010>) ((x, y) => x.WorkspaceTemplate = y));
      dbResult.RequestContext.TraceLeave(0, "Build", "Database", nameof (ReadDatabaseResults));
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildDefinition2010 Name={0} BuildControllerUri={1} FullPath={2} ContinuousIntegrationType={3}]", (object) this.Name, (object) this.BuildControllerUri, (object) this.FullPath, (object) this.ContinuousIntegrationType);

    internal static void ConvertDataspacedValues(
      IVssRequestContext requestContext,
      IEnumerable<BuildDefinition2010> definitions)
    {
      foreach (BuildDefinition2010 definition in definitions)
        definition.ConvertProcessParametersToProjectName(requestContext);
    }

    internal void ConvertProcessParametersToProjectName(IVssRequestContext requestContext)
    {
      if (string.IsNullOrEmpty(this.ProcessParameters))
        return;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.ProcessParameters = BuildDefinition2010.s_tfvcPathRegex.Replace(this.ProcessParameters, (MatchEvaluator) (match => BuildDefinition2010.ConvertParameterValue(requestContext, match.Value, BuildDefinition2010.\u003C\u003EO.\u003C0\u003E__ConvertToPathWithProjectName ?? (BuildDefinition2010.\u003C\u003EO.\u003C0\u003E__ConvertToPathWithProjectName = new Func<IVssRequestContext, string, string>(TFVCPathHelper.ConvertToPathWithProjectName)))));
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
  }
}
