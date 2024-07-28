// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Test.WebApi.TestAutomationRunSlice
// Assembly: Microsoft.TeamFoundation.Test.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17829F78-DAC0-47C1-AC4C-95D401C54448
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Test.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Test.WebApi
{
  [DataContract]
  public sealed class TestAutomationRunSlice
  {
    private const string EmptyRunSettings = "<RunSettings></RunSettings>";

    public TestAutomationRunSlice()
    {
      this.Type = AutomatedTestRunSliceType.Execution;
      this.TestRunInformation = new TestRunInformation()
      {
        Filters = new RunFilter()
        {
          SourceFilter = "*test*.dll"
        },
        RunSettings = "<RunSettings></RunSettings>",
        TestDropPath = Environment.CurrentDirectory,
        TcmRun = new ShallowReference(),
        ProjectReference = new TeamProjectReference(),
        IsTestRunComplete = false
      };
    }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public AutomatedTestRunSliceType Type { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<SlicedTestData> LastPhaseResults { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestRunInformation TestRunInformation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Results { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public AutomatedTestRunSliceStatus Status { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<Message> Messages { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public int TestConfigId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string TestConfigurationsMapping { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Requirements { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int AssignmentOrder { get; set; }
  }
}
