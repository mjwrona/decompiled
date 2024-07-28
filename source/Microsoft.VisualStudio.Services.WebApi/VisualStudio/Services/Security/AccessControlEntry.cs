// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.AccessControlEntry
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Identity;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Security
{
  [DataContract]
  public sealed class AccessControlEntry
  {
    public AccessControlEntry()
    {
    }

    public AccessControlEntry(
      IdentityDescriptor descriptor,
      int allow,
      int deny,
      AceExtendedInformation extendedInfo)
    {
      this.Descriptor = descriptor;
      this.Allow = allow;
      this.Deny = deny;
      this.ExtendedInfo = extendedInfo;
    }

    [DataMember]
    public IdentityDescriptor Descriptor { get; set; }

    [DataMember]
    public int Allow { get; set; }

    [DataMember]
    public int Deny { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public AceExtendedInformation ExtendedInfo { get; set; }
  }
}
