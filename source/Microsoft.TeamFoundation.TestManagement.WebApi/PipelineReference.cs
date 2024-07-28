// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.PipelineReference
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class PipelineReference : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int PipelineId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public StageReference StageReference { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public PhaseReference PhaseReference { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public JobReference JobReference { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int PipelineDefinitionId { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      this.StageReference?.InitializeSecureObject(securedObject);
      this.PhaseReference?.InitializeSecureObject(securedObject);
      this.JobReference?.InitializeSecureObject(securedObject);
    }
  }
}
