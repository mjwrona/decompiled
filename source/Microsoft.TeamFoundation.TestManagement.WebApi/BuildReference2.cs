// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.BuildReference2
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class BuildReference2
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid ProjectId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int BuildConfigurationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int BuildId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string BuildUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string BuildNumber { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string BuildPlatform { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string BuildFlavor { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int BuildDefinitionId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime? CreatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string BranchName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string SourceVersion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string BuildSystem { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int? CoverageId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool BuildDeleted { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string RepoId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string RepoType { get; set; }
  }
}
