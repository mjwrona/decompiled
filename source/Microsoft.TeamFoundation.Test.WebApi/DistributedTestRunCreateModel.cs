// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Test.WebApi.DistributedTestRunCreateModel
// Assembly: Microsoft.TeamFoundation.Test.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17829F78-DAC0-47C1-AC4C-95D401C54448
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Test.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Test.WebApi
{
  [DataContract]
  public class DistributedTestRunCreateModel
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Build { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool? Automated { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string SourceFilter { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string TestCaseFilter { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string EnvironmentUrl { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AutEnvironmentUrl { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BuildDropLocation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference TestSettings { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BuildPlatform { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BuildFlavor { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string TestConfigurationsMapping { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ReleaseUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ReleaseEnvironmentUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int[] PointIds { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int[] ConfigurationIds { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public ShallowReference Plan { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TimeSpan RunTimeout { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public RunProperties RunProperties { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestReportingSettings TestReportingSettings { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TfsSpecificProperties TfsSpecificProperties { get; set; }
  }
}
