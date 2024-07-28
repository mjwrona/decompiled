// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Test.WebApi.SlicedTestData
// Assembly: Microsoft.TeamFoundation.Test.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17829F78-DAC0-47C1-AC4C-95D401C54448
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Test.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Test.WebApi
{
  [DataContract]
  public sealed class SlicedTestData
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string FullyQualifiedName { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Source { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ExecutorUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public double AnticipatedExecutionTime { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int BaseDefinitionRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TestPointId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TestCaseId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TestConfigurationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string TestConfigurationName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int AttemptId { get; set; }
  }
}
