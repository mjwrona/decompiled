// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildReference
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class BuildReference : BaseSecuredObject
  {
    [DataMember(Name = "_links", EmitDefaultValue = false)]
    private ReferenceLinks m_links;

    public BuildReference()
    {
    }

    internal BuildReference(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(EmitDefaultValue = false)]
    [Key]
    public int Id { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public string BuildNumber { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public BuildStatus? Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public BuildResult? Result { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? QueueTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FinishTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef RequestedFor { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public bool Deleted { get; set; }

    public ReferenceLinks Links
    {
      get
      {
        if (this.m_links == null)
          this.m_links = new ReferenceLinks();
        return this.m_links;
      }
    }
  }
}
