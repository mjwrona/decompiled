// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.UserAttribute
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Users
{
  [DataContract]
  public class UserAttribute
  {
    [DataMember(IsRequired = true)]
    public string Name { get; set; }

    [DataMember(IsRequired = true)]
    public string Value { get; set; }

    [DataMember(IsRequired = false)]
    public DateTimeOffset LastModified { get; internal set; }

    [DataMember(IsRequired = false)]
    public int Revision { get; internal set; }

    public static implicit operator SetUserAttributeParameters(UserAttribute attribute) => new SetUserAttributeParameters()
    {
      Name = attribute.Name,
      Value = attribute.Value,
      LastModified = attribute.LastModified,
      Revision = attribute.Revision
    };

    public override string ToString() => string.Format("{0} @ {1}", (object) this.Name, (object) this.LastModified);
  }
}
