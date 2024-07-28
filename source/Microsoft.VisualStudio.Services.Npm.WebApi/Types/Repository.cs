// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Types.Repository
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Types
{
  [DataContract]
  public class Repository
  {
    [DataMember(EmitDefaultValue = false, Name = "type")]
    public string RepoType { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "url")]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "ShortcutSyntax")]
    public string ShortcutSyntax { get; set; }

    public bool IsEmpty() => string.IsNullOrEmpty(this.Url) && string.IsNullOrEmpty(this.RepoType) && string.IsNullOrEmpty(this.ShortcutSyntax);
  }
}
