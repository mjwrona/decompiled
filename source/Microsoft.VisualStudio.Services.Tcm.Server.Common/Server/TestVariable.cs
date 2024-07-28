// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestVariable
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestVariable
  {
    private int m_id;
    private string m_name;
    private string m_description;
    private int m_revision;
    private string m_teamProjectUri;
    private List<string> m_values;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, UseClientDefinedProperty = true)]
    public int Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
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
    public string Description
    {
      get => this.m_description;
      set => this.m_description = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, UseClientDefinedProperty = true)]
    public int Revision
    {
      get => this.m_revision;
      set => this.m_revision = value;
    }

    [XmlArray]
    [XmlArrayItem(typeof (string))]
    [ClientProperty(ClientVisibility.Private)]
    public List<string> Values
    {
      get
      {
        if (this.m_values == null)
          this.m_values = new List<string>();
        return this.m_values;
      }
    }

    [XmlIgnore]
    internal string TeamProjectUri
    {
      get => this.m_teamProjectUri;
      set => this.m_teamProjectUri = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestVariable Id={0} Name={1} Description={2}", (object) this.m_id, (object) this.m_name, (object) this.m_description);

    internal int Create(TestManagementRequestContext context, string teamProjectName)
    {
      if (string.IsNullOrEmpty(this.m_name))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "Name"));
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      context.SecurityManager.CheckManageTestConfigurationsPermission(context, projectFromName.String);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        return managementDatabase.CreateTestVariable(this, projectFromName.GuidId);
    }

    internal int Update(TestManagementRequestContext context, string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckManageTestConfigurationsPermission(context, projectFromName.String);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
      {
        try
        {
          return managementDatabase.UpdateTestVariable(projectFromName.GuidId, this);
        }
        catch (TestObjectUpdatedException ex)
        {
          return -1;
        }
      }
    }

    internal static void Delete(
      TestManagementRequestContext context,
      int variableId,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckManageTestConfigurationsPermission(context, projectFromName.String);
      Guid teamFoundationId = context.UserTeamFoundationId;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.DeleteTestVariable(projectFromName.GuidId, variableId, teamFoundationId);
    }

    internal static TestVariable QueryById(
      TestManagementRequestContext context,
      int variableId,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return (TestVariable) null;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        return managementDatabase.QueryTestVariableById(variableId, projectFromName.GuidId);
    }

    internal static List<TestVariable> QueryTestVariables(
      TestManagementRequestContext context,
      string teamProjectName,
      int skip = 0,
      int topToFetch = 2147483647,
      int watermark = 0)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new List<TestVariable>();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        return managementDatabase.QueryTestVariables(projectFromName.GuidId, skip, topToFetch, watermark);
    }
  }
}
