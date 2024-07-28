// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestConfiguration
  {
    protected int m_id;
    protected string m_name;
    protected string m_areaPath;
    protected string m_description;
    protected string m_teamProjectUri;
    protected byte m_state;
    protected int m_revision;
    protected List<NameValuePair> m_values;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, UseClientDefinedProperty = true)]
    [QueryMapping(SqlFieldName = "ConfigurationId")]
    [DefaultValue(0)]
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
    public string Description
    {
      get => this.m_description;
      set => this.m_description = value;
    }

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
    public bool IsDefault { get; set; }

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

    [XmlArray]
    [XmlArrayItem(typeof (NameValuePair))]
    [ClientProperty(ClientVisibility.Private)]
    public List<NameValuePair> Values
    {
      get
      {
        if (this.m_values == null)
          this.m_values = new List<NameValuePair>();
        return this.m_values;
      }
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(EnumType = typeof (TestConfigurationState))]
    public byte State
    {
      get => this.m_state;
      set => this.m_state = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestConfiguration Id={0} Name={1} Description={2}", (object) this.m_id, (object) this.m_name, (object) this.m_description);

    internal TestConfiguration Create(TestManagementRequestContext context, string teamProjectName)
    {
      if (string.IsNullOrEmpty(this.m_name))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "Name"));
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      context.SecurityManager.CheckManageTestConfigurationsPermission(context, projectFromName.String);
      this.AreaId = context.AreaPathsCache.GetIdAndThrow(context, this.AreaPath).Id;
      TestConfiguration testConfiguration = (TestConfiguration) null;
      Guid teamFoundationId = context.UserTeamFoundationId;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        testConfiguration = managementDatabase.CreateTestConfiguration(this, teamFoundationId, projectFromName.GuidId);
      testConfiguration.LastUpdatedByName = context.UserTeamFoundationName;
      TestConfiguration.FireNotification(context, testConfiguration.Id, teamProjectName);
      return testConfiguration;
    }

    internal UpdatedProperties Update(
      TestManagementRequestContext context,
      string projectName,
      bool updateInUse,
      bool unchangedValues)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckManageTestConfigurationsPermission(context, projectFromName.String);
      if (!string.IsNullOrEmpty(this.AreaPath))
        this.AreaId = context.AreaPathsCache.GetIdAndThrow(context, this.AreaPath).Id;
      UpdatedProperties property = (UpdatedProperties) null;
      try
      {
        Guid teamFoundationId = context.UserTeamFoundationId;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          property = managementDatabase.UpdateTestConfiguration(projectFromName.GuidId, this, teamFoundationId, updateInUse, unchangedValues);
      }
      catch (TestObjectUpdatedException ex)
      {
        return new UpdatedProperties() { Revision = -1 };
      }
      TestConfiguration.FireNotification(context, this.Id, projectName);
      return property.ResolveIdentity(context);
    }

    internal static void Delete(
      TestManagementRequestContext context,
      int configurationId,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckManageTestConfigurationsPermission(context, projectFromName.String);
      Guid teamFoundationId = context.UserTeamFoundationId;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.DeleteTestConfiguration(projectFromName.GuidId, configurationId, teamFoundationId);
      TestConfiguration.FireNotification(context, configurationId, projectName);
    }

    internal static List<TestConfiguration> Query(
      TestManagementRequestContext context,
      ResultsStoreQuery query,
      int planId)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestConfiguration.Query"))
      {
        ArgumentUtility.CheckForNull<ResultsStoreQuery>(query, nameof (query), context.RequestContext.ServiceName);
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, query.TeamProjectName);
        if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
          return new List<TestConfiguration>();
        List<TestConfiguration> source = (List<TestConfiguration>) null;
        AreaPathQueryTranslator pathQueryTranslator = new AreaPathQueryTranslator(context, query);
        query.QueryText = pathQueryTranslator.TranslateQuery();
        List<KeyValuePair<string, TestConfiguration>> areaUris;
        if (!context.IsFeatureEnabled("TestManagement.Server.UseStaticSprocInsteadOfDynamic"))
        {
          int lazyInitialization;
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            lazyInitialization = managementDatabase.GetDataspaceIdWithLazyInitialization(projectFromName.GuidId);
          string whereClause = pathQueryTranslator.GenerateWhereClause(lazyInitialization);
          string orderClause = pathQueryTranslator.GenerateOrderClause();
          List<KeyValuePair<int, string>> valueLists = pathQueryTranslator.GenerateValueLists();
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            source = managementDatabase.QueryTestConfigurations(whereClause, orderClause, valueLists, planId, out areaUris);
        }
        else
        {
          Dictionary<string, List<object>> parametersMap = new TcmQueryParser(query.QueryText).GetParametersMap();
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            source = managementDatabase.QueryTestConfigurations2(projectFromName.GuidId, parametersMap, planId, out areaUris);
        }
        if (areaUris != null && areaUris.Any<KeyValuePair<string, TestConfiguration>>())
        {
          List<TcmCommonStructureNodeInfo> nodes = context.CSSHelper.GetNodes(areaUris.Select<KeyValuePair<string, TestConfiguration>, string>((Func<KeyValuePair<string, TestConfiguration>, string>) (a => a.Key)).Distinct<string>().ToList<string>());
          foreach (KeyValuePair<string, TestConfiguration> keyValuePair in areaUris)
          {
            KeyValuePair<string, TestConfiguration> areaUri = keyValuePair;
            if (nodes != null && nodes.Count > 0)
            {
              TcmCommonStructureNodeInfo structureNodeInfo = nodes.Find((Predicate<TcmCommonStructureNodeInfo>) (a => string.Equals(a.Uri, areaUri.Key, StringComparison.OrdinalIgnoreCase)));
              if (structureNodeInfo != null)
                areaUri.Value.AreaPath = structureNodeInfo.Path;
            }
          }
        }
        Dictionary<Guid, string> dictionary = IdentityHelper.ResolveIdentities(context, source.Select<TestConfiguration, Guid>((Func<TestConfiguration, Guid>) (c => c.LastUpdatedBy)));
        foreach (TestConfiguration testConfiguration in source)
        {
          if (dictionary.ContainsKey(testConfiguration.LastUpdatedBy))
            testConfiguration.LastUpdatedByName = dictionary[testConfiguration.LastUpdatedBy];
        }
        return source;
      }
    }

    internal static List<TestConfiguration> QueryWithPaging(
      TestManagementRequestContext context,
      string teamProjectName,
      int skip,
      int top,
      int watermark)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new List<TestConfiguration>();
      List<TestConfiguration> source = new List<TestConfiguration>();
      List<KeyValuePair<string, TestConfiguration>> areaUris;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        source = managementDatabase.QueryTestConfigurationsWithPaging(projectFromName.GuidId, skip, top, watermark, out areaUris);
      if (areaUris != null)
      {
        List<TcmCommonStructureNodeInfo> nodes = context.CSSHelper.GetNodes(areaUris.Select<KeyValuePair<string, TestConfiguration>, string>((Func<KeyValuePair<string, TestConfiguration>, string>) (a => a.Key)).Distinct<string>().ToList<string>());
        foreach (KeyValuePair<string, TestConfiguration> keyValuePair in areaUris)
        {
          KeyValuePair<string, TestConfiguration> areaUri = keyValuePair;
          if (nodes != null && nodes.Count > 0)
          {
            TcmCommonStructureNodeInfo structureNodeInfo = nodes.Find((Predicate<TcmCommonStructureNodeInfo>) (a => string.Equals(a.Uri, areaUri.Key, StringComparison.OrdinalIgnoreCase)));
            if (structureNodeInfo != null)
              areaUri.Value.AreaPath = structureNodeInfo.Path;
          }
        }
      }
      Dictionary<Guid, string> dictionary = IdentityHelper.ResolveIdentities(context, source.Select<TestConfiguration, Guid>((Func<TestConfiguration, Guid>) (c => c.LastUpdatedBy)));
      foreach (TestConfiguration testConfiguration in source)
      {
        if (dictionary.ContainsKey(testConfiguration.LastUpdatedBy))
          testConfiguration.LastUpdatedByName = dictionary[testConfiguration.LastUpdatedBy];
      }
      return source;
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
        return managementDatabase.QueryObjectsCount(whereClause, valueLists, "vw_Configuration");
    }

    internal static TestConfiguration QueryById(
      TestManagementRequestContext context,
      int configurationId,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return (TestConfiguration) null;
      context.TestManagementHost.Replicator.UpdateCss(context);
      TestConfiguration testConfiguration = (TestConfiguration) null;
      string str = (string) null;
      using (TestManagementDatabase managementDatabase1 = TestManagementDatabase.Create(context))
      {
        TestManagementDatabase managementDatabase2 = managementDatabase1;
        List<int> configurationIds = new List<int>();
        configurationIds.Add(configurationId);
        Guid guidId = projectFromName.GuidId;
        List<Tuple<TestConfiguration, string>> source = managementDatabase2.QueryTestConfigurationById(configurationIds, guidId, true);
        if (source.Any<Tuple<TestConfiguration, string>>())
        {
          testConfiguration = source.First<Tuple<TestConfiguration, string>>().Item1;
          str = source.First<Tuple<TestConfiguration, string>>().Item2;
        }
      }
      if (str != null)
      {
        List<TcmCommonStructureNodeInfo> nodes = context.CSSHelper.GetNodes(new List<string>()
        {
          str
        });
        if (nodes != null && nodes.Count > 0)
          testConfiguration.AreaPath = nodes.First<TcmCommonStructureNodeInfo>().Path;
      }
      if (testConfiguration != null)
      {
        IdAndString idAndThrow = context.AreaPathsCache.GetIdAndThrow(context, testConfiguration.AreaPath);
        testConfiguration.AreaId = idAndThrow.Id;
        testConfiguration.LastUpdatedByName = IdentityHelper.ResolveIdentityToName(context, testConfiguration.LastUpdatedBy);
      }
      return testConfiguration;
    }

    private static void FireNotification(
      TestManagementRequestContext context,
      int configurationId,
      string projectName)
    {
      context.EventService.PublishNotification(context.RequestContext, (object) new TestConfigurationChangedNotification(configurationId, projectName));
    }
  }
}
