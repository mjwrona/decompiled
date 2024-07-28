// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultUpdateModel
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
  public class TestCaseResultUpdateModel
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<CustomTestField> CustomFields;

    public TestCaseResultUpdateModel()
    {
    }

    public TestCaseResultUpdateModel(TestCaseResultUpdateModel _model)
    {
      this.Outcome = _model.Outcome;
      this.ErrorMessage = _model.ErrorMessage;
      this.Comment = _model.Comment;
      this.StartedDate = _model.StartedDate;
      this.CompletedDate = _model.CompletedDate;
      this.DurationInMs = _model.DurationInMs;
      if (_model.TestResult != null)
        this.TestResult = new ShallowReference(_model.TestResult);
      this.State = _model.State;
      this.ComputerName = _model.ComputerName;
      if (_model.Owner != null && !string.IsNullOrEmpty(_model.Owner.DisplayName))
      {
        this.Owner = new IdentityRef();
        this.Owner.DisplayName = _model.Owner.DisplayName;
      }
      if (_model.RunBy != null && !string.IsNullOrEmpty(_model.RunBy.DisplayName))
      {
        this.RunBy = new IdentityRef();
        this.RunBy.DisplayName = _model.RunBy.DisplayName;
      }
      this.ResolutionState = _model.ResolutionState;
      this.TestCasePriority = _model.TestCasePriority;
      this.FailureType = _model.FailureType;
      this.AutomatedTestTypeId = _model.AutomatedTestTypeId;
      if (_model.AssociatedWorkItems == null || _model.AssociatedWorkItems.Length == 0)
        return;
      Array.Copy((Array) this.AssociatedWorkItems, (Array) _model.AssociatedWorkItems, _model.AssociatedWorkItems.Length);
    }

    [DataMember(Name = "testResult")]
    public ShallowReference TestResult { get; set; }

    [DataMember(Name = "state")]
    public string State { get; set; }

    [DataMember(Name = "computerName")]
    public string ComputerName { get; set; }

    [DataMember(Name = "owner")]
    public IdentityRef Owner { get; set; }

    [DataMember(Name = "runBy")]
    public IdentityRef RunBy { get; set; }

    [DataMember(Name = "resolutionState")]
    public string ResolutionState { get; set; }

    [DataMember(Name = "testCasePriority")]
    public string TestCasePriority { get; set; }

    [DataMember(Name = "failureType")]
    public string FailureType { get; set; }

    [DataMember(Name = "automatedTestTypeId")]
    public string AutomatedTestTypeId { get; set; }

    [DataMember(Name = "associatedWorkItems")]
    public int[] AssociatedWorkItems { get; set; }

    [DataMember(Name = "outcome")]
    public string Outcome { get; set; }

    [DataMember(Name = "errorMessage")]
    public string ErrorMessage { get; set; }

    [DataMember(Name = "comment")]
    public string Comment { get; set; }

    [DataMember(Name = "startedDate")]
    public string StartedDate { get; set; }

    [DataMember(Name = "completedDate")]
    public string CompletedDate { get; set; }

    [DataMember(Name = "durationInMs")]
    public string DurationInMs { get; set; }

    [DataMember(Name = "stackTrace")]
    public string StackTrace { get; set; }
  }
}
