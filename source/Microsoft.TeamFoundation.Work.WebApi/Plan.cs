// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.Plan
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public class Plan
  {
    [DataMember(Name = "id", EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(Name = "revision")]
    public int Revision { get; set; }

    [DataMember(Name = "name", EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember]
    public PlanType Type { get; set; }

    [DataMember(Name = "properties", EmitDefaultValue = false)]
    public object Properties { get; set; }

    [DataMember(Name = "createdDate", EmitDefaultValue = false)]
    public DateTime CreatedDate { get; set; }

    [DataMember(Name = "createdByIdentity", EmitDefaultValue = false)]
    public IdentityRef CreatedByIdentity { get; set; }

    [DataMember(Name = "modifiedDate", EmitDefaultValue = false)]
    public DateTime ModifiedDate { get; set; }

    [DataMember(Name = "modifiedByIdentity", EmitDefaultValue = false)]
    public IdentityRef ModifiedByIdentity { get; set; }

    [DataMember(Name = "url", EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(Name = "description", EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(Name = "userPermissions")]
    public PlanUserPermissions UserPermissions { get; set; }

    [DataMember(Name = "lastAccessed", EmitDefaultValue = false)]
    public DateTime? LastAccessed { get; set; }

    [Obsolete("This property is obsolete and will not be populated")]
    public Guid OwnerId { get; set; }

    [Obsolete("This property is deprecated and will not be populated. Use CreatedByIdentity property instead")]
    public Guid CreatedBy { get; set; }

    [Obsolete("This property is deprecated and will not be populated. Use ModifiedByIdentity property instead")]
    public Guid ModifiedBy { get; set; }

    [Obsolete("This property is deprecated and will not be populated. Use Properties.CardSettings instead")]
    public CardSettings CardSettings { get; set; }

    [Obsolete("This property is deprecated and will not be populated. Use userPermissions property instead")]
    public PlanPermissions Permissions { get; set; }
  }
}
