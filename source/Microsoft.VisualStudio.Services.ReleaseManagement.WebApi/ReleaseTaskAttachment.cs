// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTaskAttachment
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Utility;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class ReleaseTaskAttachment : ReleaseManagementSecuredObject
  {
    internal ReleaseTaskAttachment()
    {
    }

    public ReleaseTaskAttachment(string type, string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(type, nameof (type));
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      this.Type = type;
      this.Name = name;
    }

    [DataMember]
    public string Type { get; internal set; }

    [DataMember]
    public string Name { get; internal set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    [DataMember]
    public DateTime CreatedOn { get; internal set; }

    [DataMember]
    public DateTime ModifiedOn { get; internal set; }

    [DataMember]
    public IdentityRef ModifiedBy { get; internal set; }

    [DataMember]
    public Guid TimelineId { get; set; }

    [DataMember]
    public Guid RecordId { get; set; }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.Links = this.Links.GetSecuredReferenceLinks(token, requiredPermissions);
    }
  }
}
