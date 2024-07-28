// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.EnvironmentInstance
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public class EnvironmentInstance
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "Resources")]
    private IList<EnvironmentResourceReference> resources;

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

    public IList<EnvironmentResourceReference> Resources
    {
      get
      {
        if (this.resources == null)
          this.resources = (IList<EnvironmentResourceReference>) new List<EnvironmentResourceReference>();
        return this.resources;
      }
    }

    public PipelineResources ReferencedResources { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ProjectReference Project { get; internal set; }
  }
}
