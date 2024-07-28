// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.WebApi.PipelineProcessResources
// Assembly: Microsoft.Azure.Pipelines.Authorization.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4807FD31-F2A4-4329-AA76-35B262BDA671
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Authorization.WebApi
{
  [DataContract]
  public sealed class PipelineProcessResources
  {
    [DataMember(Name = "resources", EmitDefaultValue = false)]
    private List<PipelineResourceReference> m_resources;

    public void Add(PipelineResourceReference resourceReference)
    {
      if (resourceReference == null)
        return;
      this.Resources.Add(resourceReference);
    }

    public PipelineProcessResources GetAuthorizedResources()
    {
      PipelineProcessResources authorizedResources = new PipelineProcessResources();
      authorizedResources.Resources.AddRange(this.Resources.Where<PipelineResourceReference>((Func<PipelineResourceReference, bool>) (e => e.Authorized)));
      return authorizedResources;
    }

    public PipelineProcessResources GetUnauthorizedResources()
    {
      PipelineProcessResources unauthorizedResources = new PipelineProcessResources();
      unauthorizedResources.Resources.AddRange(this.Resources.Where<PipelineResourceReference>((Func<PipelineResourceReference, bool>) (e => !e.Authorized)));
      return unauthorizedResources;
    }

    public List<PipelineResourceReference> Resources
    {
      get
      {
        if (this.m_resources == null)
          this.m_resources = new List<PipelineResourceReference>();
        return this.m_resources;
      }
    }
  }
}
