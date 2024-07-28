// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.BugFieldMapping
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class BugFieldMapping
  {
    private int m_revision;
    private string m_teamProjectName;

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

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string FieldMapping { get; set; }

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

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Bug Field Mapping Project = {0} ", (object) this.m_teamProjectName);

    internal UpdatedProperties Create(TestManagementRequestContext context, string teamProjectName)
    {
      this.m_teamProjectName = teamProjectName;
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      context.SecurityManager.CheckProjectWritePermission(context, projectFromName.String);
      Guid teamFoundationId = context.UserTeamFoundationId;
      UpdatedProperties property = (UpdatedProperties) null;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        property = planningDatabase.CreateBugFieldMapping(projectFromName.GuidId, this, teamFoundationId);
      return property.ResolveIdentity(context);
    }

    internal UpdatedProperties Update(TestManagementRequestContext context, string teamProjectName)
    {
      this.m_teamProjectName = teamProjectName;
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      context.SecurityManager.CheckProjectWritePermission(context, projectFromName.String);
      Guid teamFoundationId = context.UserTeamFoundationId;
      UpdatedProperties property = new UpdatedProperties();
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
      {
        try
        {
          property = planningDatabase.UpdateBugFieldMapping(projectFromName.GuidId, this, teamFoundationId);
        }
        catch (TestObjectUpdatedException ex)
        {
          property.Revision = -1;
          return property;
        }
      }
      return property.ResolveIdentity(context);
    }

    internal static BugFieldMapping Query(
      TestManagementRequestContext context,
      string teamProjectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      context.SecurityManager.CheckProjectReadPermission(context, projectFromName.String);
      context.TestManagementHost.Replicator.UpdateCss(context);
      BugFieldMapping bugFieldMapping = (BugFieldMapping) null;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        bugFieldMapping = planningDatabase.QueryBugFieldMapping(projectFromName.GuidId);
      if (bugFieldMapping != null)
      {
        Dictionary<Guid, string> dictionary = IdentityHelper.ResolveIdentities(context, new HashSet<Guid>()
        {
          bugFieldMapping.CreatedBy,
          bugFieldMapping.LastUpdatedBy
        }.ToArray<Guid>());
        if (dictionary.ContainsKey(bugFieldMapping.CreatedBy))
          bugFieldMapping.CreatedByName = dictionary[bugFieldMapping.CreatedBy];
        if (dictionary.ContainsKey(bugFieldMapping.LastUpdatedBy))
          bugFieldMapping.LastUpdatedByName = dictionary[bugFieldMapping.LastUpdatedBy];
      }
      return bugFieldMapping;
    }
  }
}
