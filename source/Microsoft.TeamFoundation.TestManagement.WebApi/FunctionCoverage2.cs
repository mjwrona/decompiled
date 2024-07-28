// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.FunctionCoverage2
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class FunctionCoverage2
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int CoverageId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int ModuleId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int FunctionId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string SourceFile { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Class { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Namespace { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int LinesCovered { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int LinesPartiallyCovered { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int LinesNotCovered { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int BlocksCovered { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int BlocksNotCovered { get; set; }
  }
}
