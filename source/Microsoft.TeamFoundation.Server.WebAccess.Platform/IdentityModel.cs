// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.IdentityModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class IdentityModel
  {
    public IdentityModel()
    {
    }

    public IdentityModel(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      this.CustomDisplayName = identity.CustomDisplayName;
      this.DisplayName = identity.DisplayName;
      this.Email = identity.GetProperty<string>("Mail", (string) null);
      this.Id = identity.Id;
      this.IsActive = identity.IsActive;
      this.IsContainer = identity.IsContainer;
      this.ProviderDisplayName = identity.ProviderDisplayName;
      this.UniqueName = IdentityHelper.GetUniqueName(identity);
    }

    [DataMember(EmitDefaultValue = false)]
    public string CustomDisplayName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DisplayName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Email { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsActive { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsContainer { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ProviderDisplayName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string UniqueName { get; set; }
  }
}
