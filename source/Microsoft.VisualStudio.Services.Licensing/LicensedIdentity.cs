// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensedIdentity
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.VisualStudio.Services.Identity;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class LicensedIdentity : IEquatable<LicensedIdentity>, ICloneable
  {
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "email")]
    public string Email { get; set; }

    [JsonProperty(PropertyName = "userType")]
    public IdentityMetaType UserType { get; set; }

    public LicensedIdentity()
    {
    }

    public LicensedIdentity(string name, string email, IdentityMetaType userType)
    {
      this.Name = name;
      this.Email = email;
      this.UserType = userType;
    }

    public override bool Equals(object obj)
    {
      LicensedIdentity licensedIdentity = obj as LicensedIdentity;
      return obj != null && licensedIdentity != null && this.Equals(licensedIdentity);
    }

    public bool Equals(LicensedIdentity licensedIdentity) => licensedIdentity != null && string.Equals(this.Name, licensedIdentity.Name) && string.Equals(this.Email, licensedIdentity.Email) && this.UserType == licensedIdentity.UserType;

    public override int GetHashCode() => 23 * (23 * (this.Name == null ? 0 : this.Name.GetHashCode()) + (this.Email == null ? 0 : this.Email.GetHashCode())) + this.UserType.GetHashCode();

    public bool IsEmpty() => string.IsNullOrEmpty(this.Name) && string.IsNullOrEmpty(this.Email);

    public object Clone() => (object) new LicensedIdentity()
    {
      Name = this.Name,
      Email = this.Email,
      UserType = this.UserType
    };
  }
}
