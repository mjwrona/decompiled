// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Test.WebApi.TestRunExecutionConfiguration
// Assembly: Microsoft.TeamFoundation.Test.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17829F78-DAC0-47C1-AC4C-95D401C54448
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Test.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Test.WebApi
{
  [DataContract]
  public class TestRunExecutionConfiguration
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ProjectName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TestRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public RerunProperties RerunProperties { get; set; }
  }
}
