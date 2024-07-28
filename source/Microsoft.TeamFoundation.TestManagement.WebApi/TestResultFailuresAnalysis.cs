// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestResultFailuresAnalysis
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class TestResultFailuresAnalysis : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestFailureDetails NewFailures { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestFailureDetails ExistingFailures { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestFailureDetails FixedTests { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      this.NewFailures?.InitializeSecureObject(securedObject);
      this.ExistingFailures?.InitializeSecureObject(securedObject);
      this.FixedTests?.InitializeSecureObject(securedObject);
    }
  }
}
