// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.ServerTestSuite
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class ServerTestSuite
  {
    private List<TestSuiteEntry> m_entries = new List<TestSuiteEntry>();
    private int m_id;
    private int m_planId;
    private int m_parentId;
    private string m_title;
    private string m_description;
    private string m_query;
    private int m_requirementId;
    private int m_revision;
    private DateTime m_lastPopulated;
    private string m_lastError;
    private List<int> m_defaultConfigurations;
    private List<string> m_defaultConfigurationNames;

    [XmlAttribute]
    public int Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    [XmlAttribute]
    public int PlanId
    {
      get => this.m_planId;
      set => this.m_planId = value;
    }

    [XmlAttribute]
    public int ParentId
    {
      get => this.m_parentId;
      set => this.m_parentId = value;
    }

    [XmlAttribute]
    public string Title
    {
      get => this.m_title;
      set => this.m_title = value;
    }

    [XmlAttribute]
    public string Description
    {
      get => this.m_description;
      set => this.m_description = value;
    }

    [XmlAttribute]
    public string QueryString
    {
      get => this.m_query;
      set => this.m_query = value;
    }

    [XmlAttribute]
    public int RequirementId
    {
      get => this.m_requirementId;
      set => this.m_requirementId = value;
    }

    [XmlAttribute]
    public int Revision
    {
      get => this.m_revision;
      set => this.m_revision = value;
    }

    [XmlAttribute]
    public byte SuiteType { get; set; }

    [XmlArray]
    [XmlArrayItem(Type = typeof (TestSuiteEntry))]
    public List<TestSuiteEntry> ServerEntries => this.m_entries;

    [XmlAttribute]
    public bool InheritDefaultConfigurations { get; set; }

    [XmlArray]
    [XmlArrayItem(Type = typeof (int))]
    public List<int> DefaultConfigurations
    {
      get
      {
        if (this.m_defaultConfigurations == null)
          this.m_defaultConfigurations = new List<int>();
        return this.m_defaultConfigurations;
      }
    }

    [XmlArray]
    [XmlArrayItem(Type = typeof (string))]
    public List<string> DefaultConfigurationNames
    {
      get
      {
        if (this.m_defaultConfigurationNames == null)
          this.m_defaultConfigurationNames = new List<string>();
        return this.m_defaultConfigurationNames;
      }
    }

    [XmlAttribute]
    public DateTime LastPopulated
    {
      get => this.m_lastPopulated;
      set => this.m_lastPopulated = value;
    }

    [XmlAttribute]
    public string LastError
    {
      get => this.m_lastError;
      set => this.m_lastError = value;
    }

    [XmlAttribute]
    public byte State { get; set; }

    [XmlAttribute]
    public Guid LastUpdatedBy { get; set; }

    [XmlAttribute]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    public DateTime LastUpdated { get; set; }
  }
}
