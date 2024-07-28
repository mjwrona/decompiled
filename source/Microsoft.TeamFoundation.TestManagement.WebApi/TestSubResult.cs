// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestSubResult
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class TestSubResult : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int Id;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int ParentId;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int SequenceId;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string DisplayName;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ResultGroupType ResultGroupType;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<TestSubResult> SubResults;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Outcome;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Comment;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ErrorMessage;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime StartedDate;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime CompletedDate;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public long DurationInMs;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Configuration;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime LastUpdatedDate;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ComputerName;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string StackTrace;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<CustomTestField> CustomFields;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Url;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestCaseResultIdentifier TestResult { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      this.TestResult?.InitializeSecureObject(securedObject);
      this.Configuration?.InitializeSecureObject(securedObject);
      if (this.SubResults != null)
      {
        foreach (TestManagementBaseSecuredObject subResult in this.SubResults)
          subResult.InitializeSecureObject((ISecuredObject) this);
      }
      if (this.CustomFields == null)
        return;
      foreach (TestManagementBaseSecuredObject customField in this.CustomFields)
        customField.InitializeSecureObject((ISecuredObject) this);
    }
  }
}
