// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.WebApi.AnalyzerDescriptor
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F7D1B59D-FE5E-4B10-AAB1-4E05CDFBD17B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.ProjectAnalysis.WebApi
{
  [DataContract]
  public class AnalyzerDescriptor
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int MajorVersion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int MinorVersion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int PatchVersion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Description { get; set; }
  }
}
