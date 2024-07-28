// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestSettings
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestSettings
  {
    protected int m_id;
    protected string m_name;
    protected string m_areaPath;
    protected XmlElement m_settings;
    protected string m_teamProjectUri;
    protected int m_revision;
    protected bool m_isPublic;
    protected bool m_isAutomated;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, UseClientDefinedProperty = true)]
    [DefaultValue(0)]
    [QueryMapping(SqlFieldName = "SettingsId")]
    public int Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string Name
    {
      get => this.m_name;
      set
      {
        Validator.CheckAndTrimString(ref value, nameof (Name), 256);
        this.m_name = value;
      }
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string Description { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public Guid CreatedBy { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string CreatedByName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public DateTime CreatedDate { get; set; }

    [XmlArray]
    [ClientProperty(ClientVisibility.Private)]
    public TestSettingsMachineRole[] MachineRoles { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public bool IsPublic
    {
      get => this.m_isPublic;
      set => this.m_isPublic = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public bool IsAutomated
    {
      get => this.m_isAutomated;
      set => this.m_isAutomated = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string Settings
    {
      get => this.m_settings != null ? this.m_settings.OuterXml : (string) null;
      set
      {
        if (value != null)
          this.m_settings = XmlUtility.LoadXmlDocumentFromString(value).DocumentElement;
        else
          this.m_settings = (XmlElement) null;
      }
    }

    public XmlElement SettingsXml => this.m_settings;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string AreaPath
    {
      get => this.m_areaPath;
      set
      {
        Validator.CheckAndTrimString(ref value, nameof (AreaPath), 4000);
        this.m_areaPath = value;
      }
    }

    [XmlIgnore]
    internal int AreaId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public Guid LastUpdatedBy { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string LastUpdatedByName { get; set; }

    [XmlAttribute]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public DateTime LastUpdated { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, UseClientDefinedProperty = true)]
    [QueryMapping]
    [DefaultValue(0)]
    public int Revision
    {
      get => this.m_revision;
      set => this.m_revision = value;
    }

    [XmlIgnore]
    [QueryMapping("TeamProject", "DataspaceId", DataType.String)]
    internal string TeamProjectUri
    {
      get => this.m_teamProjectUri;
      set => this.m_teamProjectUri = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestSettings Id={0} Name={1}", (object) this.m_id, (object) this.m_name);

    internal UpdatedProperties Create(TestManagementRequestContext context, string teamProjectName)
    {
      context.TraceEnter("BusinessLayer", "TestSettings.Create");
      if (string.IsNullOrEmpty(this.m_name))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "Name"));
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      if (this.IsPublic)
        context.SecurityManager.CheckManageTestConfigurationsPermission(context, projectFromName.String);
      else
        context.SecurityManager.CheckPublishTestResultsPermission(context, projectFromName.String);
      this.AreaId = context.AreaPathsCache.GetIdAndThrow(context, this.AreaPath).Id;
      context.TraceVerbose("BusinessLayer", "TestSettings.Create:: Creating test settings. projectName = {0} ", (object) teamProjectName);
      Guid teamFoundationId = context.UserTeamFoundationId;
      UpdatedProperties property = new UpdatedProperties();
      if (context.IsFeatureEnabled("TestManagement.Server.CreateIfNotExistsTestSettings"))
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          property = managementDatabase.CreateIfNotExistsTestSettings(context, projectFromName.GuidId, this, teamFoundationId);
      }
      else
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          property = managementDatabase.CreateTestSettings(context, projectFromName.GuidId, this, teamFoundationId);
      }
      return property.ResolveIdentity(context);
    }

    internal UpdatedProperties Update(TestManagementRequestContext context, string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (this.IsPublic)
        context.SecurityManager.CheckManageTestConfigurationsPermission(context, projectFromName.String);
      else
        context.SecurityManager.CheckPublishTestResultsPermission(context, projectFromName.String);
      if (!string.IsNullOrEmpty(this.AreaPath))
        this.AreaId = context.AreaPathsCache.GetIdAndThrow(context, this.AreaPath).Id;
      Guid teamFoundationId = context.UserTeamFoundationId;
      UpdatedProperties property = new UpdatedProperties();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
      {
        try
        {
          property = managementDatabase.UpdateTestSettings(projectFromName.GuidId, this, teamFoundationId);
        }
        catch (TestObjectUpdatedException ex)
        {
          property.Revision = -1;
          return property;
        }
      }
      return property.ResolveIdentity(context);
    }

    internal static void Delete(
      TestManagementRequestContext context,
      int settingsId,
      string projectName)
    {
      context.TraceEnter("BusinessLayer", "TestSettings.Delete");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckManageTestConfigurationsPermission(context, projectFromName.String);
      context.TraceVerbose("BusinessLayer", "TestSettings.Delete:: Deleting testSettings. TestSettingsId = {0}, projectName = {1} ", (object) settingsId, (object) projectName);
      Guid teamFoundationId = context.UserTeamFoundationId;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.DeleteTestSettings(projectFromName.GuidId, settingsId, teamFoundationId);
    }

    internal static List<TestSettings> Query(
      TestManagementRequestContext context,
      ResultsStoreQuery query,
      bool omitSettings)
    {
      ArgumentUtility.CheckForNull<ResultsStoreQuery>(query, nameof (query), context.RequestContext.ServiceName);
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, query.TeamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new List<TestSettings>();
      AreaPathQueryTranslator pathQueryTranslator = new AreaPathQueryTranslator(context, query);
      query.QueryText = pathQueryTranslator.TranslateQuery();
      List<KeyValuePair<string, TestSettings>> areaUris = new List<KeyValuePair<string, TestSettings>>();
      List<TestSettings> testSettingsList;
      if (context.IsFeatureEnabled("TestManagement.Server.UseStaticSprocInsteadOfDynamic"))
      {
        Dictionary<string, List<object>> parametersMap = new TcmQueryParser(query.QueryText).GetParametersMap();
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          testSettingsList = managementDatabase.QueryTestSettings2(projectFromName.GuidId, parametersMap, omitSettings, out areaUris);
      }
      else
      {
        int lazyInitialization;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          lazyInitialization = managementDatabase.GetDataspaceIdWithLazyInitialization(projectFromName.GuidId);
        string whereClause = pathQueryTranslator.GenerateWhereClause(lazyInitialization);
        string orderClause = pathQueryTranslator.GenerateOrderClause();
        List<KeyValuePair<int, string>> valueLists = pathQueryTranslator.GenerateValueLists();
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          testSettingsList = managementDatabase.QueryTestSettings(whereClause, orderClause, valueLists, omitSettings, out areaUris);
      }
      if (areaUris.Count > 0)
      {
        List<TcmCommonStructureNodeInfo> nodes = context.CSSHelper.GetNodes(areaUris.Select<KeyValuePair<string, TestSettings>, string>((Func<KeyValuePair<string, TestSettings>, string>) (a => a.Key)).Distinct<string>().ToList<string>());
        foreach (KeyValuePair<string, TestSettings> keyValuePair in areaUris)
        {
          KeyValuePair<string, TestSettings> areaUri = keyValuePair;
          if (nodes != null && nodes.Count > 0)
          {
            TcmCommonStructureNodeInfo structureNodeInfo = nodes.Find((Predicate<TcmCommonStructureNodeInfo>) (a => string.Equals(a.Uri, areaUri.Key, StringComparison.OrdinalIgnoreCase)));
            if (structureNodeInfo != null)
              areaUri.Value.AreaPath = structureNodeInfo.Path;
          }
        }
      }
      HashSet<Guid> source = new HashSet<Guid>();
      foreach (TestSettings testSettings in testSettingsList)
      {
        source.Add(testSettings.CreatedBy);
        source.Add(testSettings.LastUpdatedBy);
      }
      Dictionary<Guid, string> dictionary = IdentityHelper.ResolveIdentities(context, source.ToArray<Guid>());
      foreach (TestSettings testSettings in testSettingsList)
      {
        if (dictionary.ContainsKey(testSettings.CreatedBy))
          testSettings.CreatedByName = dictionary[testSettings.CreatedBy];
        if (dictionary.ContainsKey(testSettings.LastUpdatedBy))
          testSettings.LastUpdatedByName = dictionary[testSettings.LastUpdatedBy];
      }
      return testSettingsList;
    }

    internal static int QueryCount(TestManagementRequestContext context, ResultsStoreQuery query)
    {
      ArgumentUtility.CheckForNull<ResultsStoreQuery>(query, nameof (query), context.RequestContext.ServiceName);
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, query.TeamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return 0;
      AreaPathQueryTranslator pathQueryTranslator = new AreaPathQueryTranslator(context, query);
      query.QueryText = pathQueryTranslator.TranslateQuery();
      int lazyInitialization;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        lazyInitialization = managementDatabase.GetDataspaceIdWithLazyInitialization(projectFromName.GuidId);
      string whereClause = pathQueryTranslator.GenerateWhereClause(lazyInitialization);
      List<KeyValuePair<int, string>> valueLists = pathQueryTranslator.GenerateValueLists();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        return managementDatabase.QueryObjectsCount(whereClause, valueLists, "vw_TestSettings");
    }

    internal static TestSettings QueryById(
      TestManagementRequestContext context,
      int settingsId,
      string projectName)
    {
      context.TraceEnter("BusinessLayer", "TestSettings.QueryById");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return (TestSettings) null;
      context.TestManagementHost.Replicator.UpdateCss(context);
      context.TraceVerbose("BusinessLayer", "TestSettings.QueryById:: Querying test settings by test settings id. testSettingsId = {0}, projectName = {1} ", (object) settingsId, (object) projectName);
      TestSettings setting = (TestSettings) null;
      string areaUri;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        setting = managementDatabase.QueryTestSettingsById(projectFromName.GuidId, settingsId, out areaUri);
      if (setting != null)
      {
        if (!string.IsNullOrEmpty(areaUri))
        {
          List<TcmCommonStructureNodeInfo> nodes = context.CSSHelper.GetNodes(new List<string>()
          {
            areaUri
          });
          if (nodes != null && nodes.Count > 0)
            setting.AreaPath = nodes.First<TcmCommonStructureNodeInfo>().Path;
        }
        Dictionary<Guid, string> dictionary = IdentityHelper.ResolveIdentities(context, new HashSet<Guid>()
        {
          setting.CreatedBy,
          setting.LastUpdatedBy
        }.ToArray<Guid>());
        if (dictionary.ContainsKey(setting.CreatedBy))
          setting.CreatedByName = dictionary[setting.CreatedBy];
        if (dictionary.ContainsKey(setting.LastUpdatedBy))
          setting.LastUpdatedByName = dictionary[setting.LastUpdatedBy];
        TestSettings.ExpandVsLatestMacroInTestSettingsXmlIfRequired(context, setting);
      }
      return setting;
    }

    private static void ExpandVsLatestMacroInTestSettingsXmlIfRequired(
      TestManagementRequestContext context,
      TestSettings setting)
    {
      if (!context.RequestContext.ExecutionEnvironment.IsHostedDeployment || setting.SettingsXml == null)
        return;
      if (!setting.SettingsXml.HasChildNodes)
        return;
      try
      {
        foreach (XmlNode xmlNode in setting.SettingsXml.GetElementsByTagName("DataCollector"))
        {
          XmlAttribute attribute = xmlNode.Attributes["assemblyQualifiedName"];
          if (attribute != null && attribute.Value.IndexOf("$(VSLATEST)", StringComparison.InvariantCultureIgnoreCase) >= 0)
            attribute.Value = Regex.Replace(attribute.Value, Regex.Escape("$(VSLATEST)"), "19.0.0.0", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }
      }
      catch (Exception ex)
      {
        context.RequestContext.TraceException(1015004, "TestManagement", "BusinessLayer", ex);
      }
    }
  }
}
