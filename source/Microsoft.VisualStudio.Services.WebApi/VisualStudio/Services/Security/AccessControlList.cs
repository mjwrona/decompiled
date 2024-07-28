// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.AccessControlList
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Security
{
  [DataContract]
  public sealed class AccessControlList
  {
    public AccessControlList()
    {
    }

    public AccessControlList(
      string token,
      bool inherit,
      Dictionary<IdentityDescriptor, AccessControlEntry> acesDictionary,
      bool includeExtendedInfo)
    {
      this.Token = token;
      this.InheritPermissions = inherit;
      this.AcesDictionary = acesDictionary;
      this.IncludeExtendedInfo = includeExtendedInfo;
    }

    [DataMember]
    public bool InheritPermissions { get; set; }

    [DataMember]
    public string Token { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Dictionary<IdentityDescriptor, AccessControlEntry> AcesDictionary { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IncludeExtendedInfo { get; set; }
  }
}
