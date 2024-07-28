// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestRunModelBase
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public abstract class TestRunModelBase
  {
    public TestRunModelBase()
    {
    }

    public TestRunModelBase(TestRunBase testRunBase)
    {
      this.Title = testRunBase.Title;
      this.Owner = testRunBase.Owner;
      this.OwnerName = testRunBase.OwnerName;
      this.BuildUri = testRunBase.BuildUri;
      this.BuildNumber = testRunBase.BuildNumber;
      this.StartDate = testRunBase.StartDate;
      this.CompleteDate = testRunBase.CompleteDate <= DateTime.MinValue ? DateTime.Now : testRunBase.CompleteDate;
      this.TestPlanId = testRunBase.TestPlanId;
      this.TestSettingsId = testRunBase.TestSettingsId;
      this.PublicTestSettingsId = testRunBase.TestSettingsId;
      this.TestEnvironmentId = testRunBase.TestEnvironmentId;
    }

    [DataMember(Name = "title")]
    public string Title { get; set; }

    [DataMember(Name = "owner")]
    public Guid Owner { get; set; }

    [DataMember(Name = "ownerName")]
    public string OwnerName { get; set; }

    [DataMember(Name = "buildUri")]
    public string BuildUri { get; set; }

    [DataMember(Name = "buildNumber")]
    public string BuildNumber { get; set; }

    [DataMember(Name = "startDate")]
    public DateTime StartDate { get; set; }

    [DataMember(Name = "completeDate")]
    public DateTime CompleteDate { get; set; }

    [DataMember(Name = "testPlanId")]
    public int TestPlanId { get; set; }

    [DataMember(Name = "testSettingsId")]
    public int TestSettingsId { get; set; }

    [DataMember(Name = "publicTestSettingsId")]
    public int PublicTestSettingsId { get; set; }

    [DataMember(Name = "testEnvironmentId")]
    public Guid TestEnvironmentId { get; set; }

    internal void UpdateFromModel(TestRunBase run)
    {
      run.Title = this.Title;
      run.Owner = this.Owner;
      run.OwnerName = this.OwnerName;
      run.BuildUri = this.BuildUri;
      run.BuildNumber = this.BuildNumber;
      run.StartDate = this.StartDate;
      run.CompleteDate = this.CompleteDate;
      run.TestPlanId = this.TestPlanId;
      run.TestSettingsId = this.TestSettingsId;
      run.PublicTestSettingsId = this.TestSettingsId;
      run.TestEnvironmentId = this.TestEnvironmentId;
    }
  }
}
