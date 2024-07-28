// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AadDirectoryRole
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Aad
{
  [DataContract]
  public class AadDirectoryRole : AadObject
  {
    [DataMember]
    private string description;
    [DataMember]
    private string roleTemplateId;

    protected AadDirectoryRole()
    {
    }

    internal AadDirectoryRole(
      Guid objectId,
      string roleTemplateId,
      string displayName,
      string description)
      : base(objectId, displayName)
    {
      this.description = description;
      this.roleTemplateId = roleTemplateId;
    }

    public string Description
    {
      get => this.description;
      set => this.description = value;
    }

    public string RoleTemplateId
    {
      get => this.roleTemplateId;
      set => this.roleTemplateId = value;
    }

    public class Factory
    {
      public AadDirectoryRole Create() => new AadDirectoryRole(this.ObjectId, this.RoleTemplateId, this.DisplayName, this.Description);

      public Guid ObjectId { get; set; }

      public string DisplayName { get; set; }

      public string Description { get; set; }

      public string RoleTemplateId { get; set; }
    }
  }
}
