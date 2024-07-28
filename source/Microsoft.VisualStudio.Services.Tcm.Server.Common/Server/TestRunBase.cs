// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestRunBase
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public abstract class TestRunBase
  {
    private string m_title;
    private string m_buildUri;
    private string m_buildNumber;
    private string m_buildPlatform;
    private string m_buildFlavor;
    private string m_teamProjectUri;
    private DateTime m_creationDate;
    private DateTime m_startDate;
    private DateTime m_completeDate;
    private string m_controller;
    private int m_testPlanId;
    private int m_revision;
    private string m_teamProject;

    [XmlIgnore]
    internal string TeamProjectUri
    {
      get => this.m_teamProjectUri;
      set => this.m_teamProjectUri = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping("TeamProject", "DataspaceId", DataType.String)]
    public string TeamProject
    {
      get => this.m_teamProject;
      set => this.m_teamProject = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string Title
    {
      get => this.m_title;
      set => this.m_title = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public Guid Owner { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string OwnerName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string BuildUri
    {
      get => this.m_buildUri;
      set => this.m_buildUri = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string BuildNumber
    {
      get => this.m_buildNumber;
      set => this.m_buildNumber = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int BuildConfigurationId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string BuildPlatform
    {
      get => this.m_buildPlatform;
      set => this.m_buildPlatform = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string BuildFlavor
    {
      get => this.m_buildFlavor;
      set => this.m_buildFlavor = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public DateTime CreationDate
    {
      get => this.m_creationDate;
      set => this.m_creationDate = value;
    }

    [XmlAttribute]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public DateTime LastUpdated { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public DateTime StartDate
    {
      get => this.m_startDate;
      set => this.m_startDate = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public DateTime CompleteDate
    {
      get => this.m_completeDate;
      set => this.m_completeDate = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string Controller
    {
      get => this.m_controller;
      set => this.m_controller = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int TestPlanId
    {
      get => this.m_testPlanId;
      set => this.m_testPlanId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [DefaultValue(0)]
    public int TestSettingsId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(WiqlFieldName = "TestSettingsId", SqlFieldName = "PublicTestSettingsId")]
    [DefaultValue(0)]
    public int PublicTestSettingsId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public Guid TestEnvironmentId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, UseClientDefinedProperty = true)]
    [QueryMapping]
    public int Revision
    {
      get => this.m_revision;
      set => this.m_revision = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public Guid LastUpdatedBy { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string LastUpdatedByName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string Comment { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int BugsCount { get; set; }

    [XmlIgnore]
    internal byte[] RowVersion { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string ServiceVersion { get; set; }

    internal static void ResolveUserNames<T>(TestManagementRequestContext context, List<T> runs) where T : TestRunBase
    {
      HashSet<Guid> source = new HashSet<Guid>();
      foreach (T run in runs)
      {
        source.Add(run.Owner);
        source.Add(run.LastUpdatedBy);
      }
      Guid[] array = source != null ? ((IEnumerable<Guid>) source.ToArray<Guid>()).Where<Guid>((Func<Guid, bool>) (id => id != Guid.Empty)).ToArray<Guid>() : (Guid[]) null;
      Dictionary<Guid, string> dictionary = IdentityHelper.ResolveIdentities(context, array);
      foreach (T run in runs)
      {
        if (dictionary.ContainsKey(run.Owner))
          run.OwnerName = dictionary[run.Owner];
        if (dictionary.ContainsKey(run.LastUpdatedBy))
          run.LastUpdatedByName = dictionary[run.LastUpdatedBy];
      }
    }

    protected static void PopulateVersion(TestRunBase ret)
    {
      if (ret == null)
        return;
      ret.ServiceVersion = TestManagementHost.ServerVersion.ToString();
    }

    protected static void PopulateVersion<T>(IList<T> ret) where T : TestRunBase
    {
      if (ret == null || ret.Count <= 0)
        return;
      TestRunBase.PopulateVersion((TestRunBase) ret[0]);
    }
  }
}
