// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Test.WebApi.RerunProperties
// Assembly: Microsoft.TeamFoundation.Test.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17829F78-DAC0-47C1-AC4C-95D401C54448
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Test.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Test.WebApi
{
  [DataContract]
  public class RerunProperties
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int CurrentIteration { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int RerunIterationCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int RerunFailedThreshold { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int RerunFailedTestCasesMaxLimit { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool AllowRerunFailedTests { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int FailedTestCasesCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TotalTestCasesCount { get; set; }
  }
}
