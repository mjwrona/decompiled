// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.FeedPermission
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  [DataContract]
  public class FeedPermission : FeedSecuredObject, IEquatable<FeedPermission>
  {
    [DataMember]
    public FeedRole Role { get; set; }

    [DataMember]
    public IdentityDescriptor IdentityDescriptor { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public Guid? IdentityId { get; set; }

    [DataMember]
    public string DisplayName { get; set; }

    [DataMember]
    public bool IsInheritedRole { get; set; }

    bool IEquatable<FeedPermission>.Equals(FeedPermission other) => this.Role == other.Role && this.IdentityDescriptor.Identifier.Equals(other.IdentityDescriptor.Identifier) && this.IdentityDescriptor.IdentityType.Equals(other.IdentityDescriptor.IdentityType);
  }
}
