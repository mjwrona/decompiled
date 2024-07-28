// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference2
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class ReleaseReference2
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid ProjectId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int ReleaseRefId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ReleaseUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ReleaseEnvUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int ReleaseId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int ReleaseEnvId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int ReleaseDefId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int ReleaseEnvDefId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int Attempt { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ReleaseName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ReleaseEnvName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime? ReleaseCreationDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime? EnvironmentCreationDate { get; set; }
  }
}
