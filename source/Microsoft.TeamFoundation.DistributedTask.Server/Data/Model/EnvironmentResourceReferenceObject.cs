// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Data.Model.EnvironmentResourceReferenceObject
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Data.Model
{
  [DataContract]
  public class EnvironmentResourceReferenceObject
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "DeploymentRecords")]
    private IList<DeploymentExecutionRecordObject> deploymentRecords;
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "Tags")]
    private IList<string> m_tags;

    [DataMember]
    public int Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public EnvironmentResourceType Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public EnvironmentResourceProviderData ProviderData { get; set; }

    public IList<string> Tags
    {
      get
      {
        if (this.m_tags == null)
          this.m_tags = (IList<string>) new List<string>();
        return this.m_tags;
      }
      set => this.m_tags = value;
    }

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
