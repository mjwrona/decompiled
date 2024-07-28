// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.ShallowTestCaseResult
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class ShallowTestCaseResult : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int Id;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int RunId;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int RefId;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Outcome;
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int Priority;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AutomatedTestName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AutomatedTestStorage { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Owner { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsReRun { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string TestCaseTitle { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public double DurationInMs { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string[] Tags { get; set; }
  }
}
