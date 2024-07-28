// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Types.Person
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Types
{
  [DataContract]
  public class Person
  {
    [DataMember(EmitDefaultValue = false, Name = "name")]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "email")]
    public string Email { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "url")]
    public string Url { get; set; }

    public string FullName() => this.Name;

    public bool IsEmpty() => string.IsNullOrEmpty(this.Name) && string.IsNullOrEmpty(this.Url) && string.IsNullOrEmpty(this.Email);
  }
}
