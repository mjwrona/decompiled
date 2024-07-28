// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.PlanMetadata
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public class PlanMetadata
  {
    [DataMember(Name = "description", EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(Name = "modifiedDate")]
    public DateTime ModifiedDate { get; set; }

    [Obsolete("This property is deprecated and will not be populated. Use CreatedByIdentity property instead")]
    public Guid CreatedBy { get; set; }

    [DataMember(Name = "createdByIdentity", EmitDefaultValue = false)]
    public IdentityRef CreatedByIdentity { get; set; }

    [Obsolete("This property is deprecated and will not be populated. Use userPermissions property instead")]
    public PlanPermissions Permissions { get; set; }

    [DataMember(Name = "userPermissions")]
    public PlanUserPermissions UserPermissions { get; set; }
  }
}
