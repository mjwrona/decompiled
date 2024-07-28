// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestRunStartedEvent
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [GeneratedCode("xsd", "4.0.30319.1")]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  [XmlRoot(Namespace = "", IsNullable = true)]
  [Serializable]
  public class TestRunStartedEvent
  {
    private string teamFoundationServerUrlField;
    private string teamProjectField;
    private string idField;
    private string urlField;
    private string testPlanIdField;
    private string isAutomatedField;
    private string titleField;
    private string creationDateField;
    private string ownerField;
    private RunStateType stateField;
    private string buildNumberField;
    private string totalTestsField;
    private string lastUpdatedField;
    private string lastUpdatedByField;
    private string subscriberField;
    private string timeZoneField;
    private string timeZoneOffsetField;

    [XmlElement(DataType = "anyURI")]
    public string TeamFoundationServerUrl
    {
      get => this.teamFoundationServerUrlField;
      set => this.teamFoundationServerUrlField = value;
    }

    public string TeamProject
    {
      get => this.teamProjectField;
      set => this.teamProjectField = value;
    }

    public string Id
    {
      get => this.idField;
      set => this.idField = value;
    }

    [XmlElement(DataType = "anyURI")]
    public string Url
    {
      get => this.urlField;
      set => this.urlField = value;
    }

    public string TestPlanId
    {
      get => this.testPlanIdField;
      set => this.testPlanIdField = value;
    }

    public string IsAutomated
    {
      get => this.isAutomatedField;
      set => this.isAutomatedField = value;
    }

    public string Title
    {
      get => this.titleField;
      set => this.titleField = value;
    }

    public string CreationDate
    {
      get => this.creationDateField;
      set => this.creationDateField = value;
    }

    public string Owner
    {
      get => this.ownerField;
      set => this.ownerField = value;
    }

    public RunStateType State
    {
      get => this.stateField;
      set => this.stateField = value;
    }

    public string BuildNumber
    {
      get => this.buildNumberField;
      set => this.buildNumberField = value;
    }

    public string TotalTests
    {
      get => this.totalTestsField;
      set => this.totalTestsField = value;
    }

    public string LastUpdated
    {
      get => this.lastUpdatedField;
      set => this.lastUpdatedField = value;
    }

    public string LastUpdatedBy
    {
      get => this.lastUpdatedByField;
      set => this.lastUpdatedByField = value;
    }

    public string Subscriber
    {
      get => this.subscriberField;
      set => this.subscriberField = value;
    }

    public string TimeZone
    {
      get => this.timeZoneField;
      set => this.timeZoneField = value;
    }

    public string TimeZoneOffset
    {
      get => this.timeZoneOffsetField;
      set => this.timeZoneOffsetField = value;
    }
  }
}
