// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestRun
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class TestRun : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Build { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public BuildConfiguration BuildConfiguration { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public bool IsAutomated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Iteration { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityRef Owner { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Project { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime StartedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime CompletedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string State { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestEnvironment TestEnvironment { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Plan { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string PostProcessState { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ErrorMessage { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime DueDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TotalTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int IncompleteTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int NotApplicableTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int PassedTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int UnanalyzedTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime CreatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    public DateTime LastUpdatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityRef LastUpdatedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Controller { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int Revision { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Comment { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string DropLocation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TestMessageLogId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference TestSettings { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference DtlEnvironment { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference DtlAutEnvironment { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DtlEnvironmentDetails DtlEnvironmentCreationDetails { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public RunFilter Filter { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Phase { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestRunSubstate Substate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ReleaseUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ReleaseEnvironmentUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ReleaseReference Release { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<RunStatistic> RunStatistics { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string WebAccessUrl { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<CustomTestField> CustomFields { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public PipelineReference PipelineReference { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IList<TestTag> Tags { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      this.Build?.InitializeSecureObject(securedObject);
      this.BuildConfiguration?.InitializeSecureObject(securedObject);
      this.Project?.InitializeSecureObject(securedObject);
      this.TestEnvironment?.InitializeSecureObject(securedObject);
      this.Plan?.InitializeSecureObject(securedObject);
      this.TestSettings?.InitializeSecureObject(securedObject);
      this.DtlAutEnvironment?.InitializeSecureObject(securedObject);
      this.DtlEnvironment?.InitializeSecureObject(securedObject);
      this.DtlEnvironmentCreationDetails?.InitializeSecureObject(securedObject);
      this.Filter?.InitializeSecureObject(securedObject);
      this.Release?.InitializeSecureObject(securedObject);
      this.PipelineReference?.InitializeSecureObject(securedObject);
      this.SecureIdentityRef(this.LastUpdatedBy);
      this.SecureIdentityRef(this.Owner);
      if (this.CustomFields != null)
      {
        foreach (TestManagementBaseSecuredObject customField in this.CustomFields)
          customField.InitializeSecureObject(securedObject);
      }
      if (this.RunStatistics != null)
      {
        foreach (TestManagementBaseSecuredObject runStatistic in this.RunStatistics)
          runStatistic.InitializeSecureObject(securedObject);
      }
      if (this.Tags == null)
        return;
      foreach (TestManagementBaseSecuredObject tag in (IEnumerable<TestTag>) this.Tags)
        tag.InitializeSecureObject(securedObject);
    }
  }
}
