// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestResultModelBase
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public abstract class TestResultModelBase
  {
    public TestResultModelBase()
    {
    }

    public TestResultModelBase(TestResult result)
    {
      this.Id = new TestCaseResultIdentifierModel(result.Id);
      this.Outcome = result.Outcome;
      this.ErrorMessage = result.ErrorMessage;
      this.Comment = result.Comment;
      this.DateStarted = result.DateStarted <= DateTime.MinValue ? DateTime.Now : result.DateStarted;
      this.DateCompleted = result.DateCompleted <= DateTime.MinValue ? DateTime.Now : result.DateCompleted;
      this.Duration = result.Duration;
    }

    [DataMember(Name = "id")]
    public TestCaseResultIdentifierModel Id { get; set; }

    [DataMember(Name = "outcome", EmitDefaultValue = false)]
    public byte Outcome { get; set; }

    [DataMember(Name = "errorMessage", EmitDefaultValue = false)]
    public string ErrorMessage { get; set; }

    [DataMember(Name = "comment", EmitDefaultValue = false)]
    public string Comment { get; set; }

    [DataMember(Name = "dateStarted", EmitDefaultValue = false)]
    public DateTime DateStarted { get; set; }

    [DataMember(Name = "dateCompleted", EmitDefaultValue = false)]
    public DateTime DateCompleted { get; set; }

    [DataMember(Name = "duration")]
    public long Duration { get; set; }

    internal void UpdateFromModel(TestResult result)
    {
      result.Outcome = this.Outcome;
      result.ErrorMessage = this.ErrorMessage;
      result.Comment = this.Comment;
      result.DateStarted = this.DateStarted;
      result.DateCompleted = this.DateCompleted;
      result.Duration = this.Duration;
    }
  }
}
