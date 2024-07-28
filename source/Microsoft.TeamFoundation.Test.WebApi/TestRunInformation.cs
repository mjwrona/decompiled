// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Test.WebApi.TestRunInformation
// Assembly: Microsoft.TeamFoundation.Test.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17829F78-DAC0-47C1-AC4C-95D401C54448
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Test.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Test.WebApi
{
  [DataContract]
  public sealed class TestRunInformation
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string TestDropPath { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public ShallowReference TcmRun { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public RunFilter Filters { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string RunSettings { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = true)]
    public TeamProjectReference ProjectReference { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsTestRunComplete { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsTestPlanRun { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TestPlanId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int BuildConfigurationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BuildPlatform { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BuildFlavor { get; set; }
  }
}
