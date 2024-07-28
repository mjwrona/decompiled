// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Test.WebApi.RunProperties
// Assembly: Microsoft.TeamFoundation.Test.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17829F78-DAC0-47C1-AC4C-95D401C54448
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Test.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Test.WebApi
{
  [DataContract]
  public class RunProperties
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int MaxAgents { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsCustomSlicingEnabled { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int NumberOfTestCasesPerSlice { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsTimeBasedSlicing { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int SliceTime { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsTestImpactOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int BaseLineBuildId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsRerunEnabled { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int RerunFailedThreshold { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int RerunMaxAttempts { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int RerunFailedTestCasesMaxLimit { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TestPlanId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TestConfigurationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int[] TestSuiteIds { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string TestRunType { get; set; }
  }
}
