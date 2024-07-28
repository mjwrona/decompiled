// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.WebSessionToken
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebPlatform
{
  [DataContract]
  public class WebSessionToken
  {
    [DataMember(Order = 1)]
    public Guid? AppId { get; set; }

    [DataMember(Order = 10)]
    public string Token { get; set; }

    [DataMember(Order = 20, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Order = 30, EmitDefaultValue = false)]
    public bool Force { get; set; }

    [DataMember(Order = 40, EmitDefaultValue = false)]
    public DelegatedAppTokenType? TokenType { get; set; }

    [DataMember(Order = 50, EmitDefaultValue = false)]
    public DateTime? ValidTo { get; set; }

    [DataMember(Order = 60, EmitDefaultValue = false)]
    public string NamedTokenId { get; set; }

    [DataMember(Order = 70, EmitDefaultValue = false, IsRequired = false)]
    public string PublisherName { get; set; }

    [DataMember(Order = 80, EmitDefaultValue = false, IsRequired = false)]
    public string ExtensionName { get; set; }
  }
}
