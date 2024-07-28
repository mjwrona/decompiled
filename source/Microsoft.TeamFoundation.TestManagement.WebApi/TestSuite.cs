// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestSuite
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
  public sealed class TestSuite : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public int Id { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Project { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public ShallowReference Plan { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Parent { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string QueryString { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int RequirementId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int Revision { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestCaseCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string SuiteType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<ShallowReference> Suites { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string TestCasesUrl { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool InheritDefaultConfigurations { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<ShallowReference> DefaultConfigurations { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<ShallowReference> DefaultTesters { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime LastPopulatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string LastError { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string State { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityRef LastUpdatedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    public DateTime LastUpdatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AreaUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Text { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<TestSuite> Children { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      this.Project?.InitializeSecureObject(securedObject);
      this.Plan?.InitializeSecureObject(securedObject);
      this.Parent?.InitializeSecureObject(securedObject);
      this.SecureIdentityRef(this.LastUpdatedBy);
      if (this.Suites != null)
      {
        foreach (TestManagementBaseSecuredObject suite in this.Suites)
          suite.InitializeSecureObject(securedObject);
      }
      if (this.DefaultConfigurations != null)
      {
        foreach (TestManagementBaseSecuredObject defaultConfiguration in this.DefaultConfigurations)
          defaultConfiguration.InitializeSecureObject(securedObject);
      }
      if (this.DefaultTesters != null)
      {
        foreach (TestManagementBaseSecuredObject defaultTester in this.DefaultTesters)
          defaultTester.InitializeSecureObject(securedObject);
      }
      if (this.Children == null)
        return;
      foreach (TestManagementBaseSecuredObject child in this.Children)
        child.InitializeSecureObject(securedObject);
    }
  }
}
