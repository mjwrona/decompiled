// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestResultCreateModel
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
  public class TestResultCreateModel
  {
    [DataMember(Name = "testCase")]
    public ShallowReference TestCase;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<CustomTestField> CustomFields;

    private ShallowReference ValidateAndGetParameter(
      ShallowReference inputRef,
      string parameterName)
    {
      int result = 0;
      if (string.IsNullOrEmpty(inputRef.Id))
        return (ShallowReference) null;
      if (int.TryParse(inputRef.Id, out result) && result > 0)
        return inputRef;
      throw new ArgumentException(parameterName);
    }

    private IdentityRef ValidateAndGetParameter(IdentityRef inputRef, string parameterName)
    {
      Guid result = Guid.Empty;
      if (string.IsNullOrEmpty(inputRef.Id))
        return (IdentityRef) null;
      if (Guid.TryParse(inputRef.Id, out result) && result != Guid.Empty)
        return inputRef;
      throw new ArgumentException(parameterName);
    }

    [DataMember(Name = "configuration")]
    public ShallowReference Configuration { get; set; }

    [DataMember(Name = "testPoint")]
    public ShallowReference TestPoint { get; set; }

    [DataMember(Name = "state")]
    public string State { get; set; }

    [DataMember(Name = "computerName")]
    public string ComputerName { get; set; }

    [DataMember(Name = "resolutionState")]
    public string ResolutionState { get; set; }

    [DataMember(Name = "testCasePriority")]
    public string TestCasePriority { get; set; }

    [DataMember(Name = "failureType")]
    public string FailureType { get; set; }

    [DataMember(Name = "automatedTestName")]
    public string AutomatedTestName { get; set; }

    [DataMember(Name = "automatedTestStorage")]
    public string AutomatedTestStorage { get; set; }

    [DataMember(Name = "automatedTestType")]
    public string AutomatedTestType { get; set; }

    [DataMember(Name = "automatedTestTypeId")]
    public string AutomatedTestTypeId { get; set; }

    [DataMember(Name = "automatedTestId")]
    public string AutomatedTestId { get; set; }

    [DataMember(Name = "area")]
    public ShallowReference Area { get; set; }

    [DataMember(Name = "owner")]
    public IdentityRef Owner { get; set; }

    [DataMember(Name = "runBy")]
    public IdentityRef RunBy { get; set; }

    [DataMember(Name = "testCaseTitle")]
    public string TestCaseTitle { get; set; }

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
