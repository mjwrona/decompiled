// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Data.Model.EnvironmentObject
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Data.Model
{
  [DataContract]
  public class EnvironmentObject
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "Resources")]
    private IList<EnvironmentResourceReferenceObject> resources;
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "DeploymentRecords")]
    private IList<DeploymentExecutionRecordObject> deploymentRecords;

    [DataMember]
    public int Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef CreatedBy { get; set; }

    [DataMember]
    public DateTime CreatedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef LastModifiedBy { get; set; }

    [DataMember]
    public DateTime LastModifiedOn { get; set; }

    public IList<EnvironmentResourceReferenceObject> Resources
    {
      get
      {
        if (this.resources == null)
          this.resources = (IList<EnvironmentResourceReferenceObject>) new List<EnvironmentResourceReferenceObject>();
        return this.resources;
      }
    }

    public PipelineResources ReferencedResources { get; set; }

    public IList<DeploymentExecutionRecordObject> DeploymentRecords
    {
      get
      {
        if (this.deploymentRecords == null)
          this.deploymentRecords = (IList<DeploymentExecutionRecordObject>) new List<DeploymentExecutionRecordObject>();
        return this.deploymentRecords;
      }
    }
  }
}
