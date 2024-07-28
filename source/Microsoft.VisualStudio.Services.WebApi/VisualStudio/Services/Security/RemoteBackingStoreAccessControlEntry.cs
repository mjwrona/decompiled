// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.RemoteBackingStoreAccessControlEntry
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Security
{
  [DataContract(Name = "AccessControlEntryDetails")]
  public sealed class RemoteBackingStoreAccessControlEntry
  {
    public RemoteBackingStoreAccessControlEntry()
    {
    }

    public RemoteBackingStoreAccessControlEntry(
      string subject,
      string token,
      int allow,
      int deny,
      bool isDeleted)
    {
      this.Subject = subject;
      this.Token = token;
      this.Allow = allow;
      this.Deny = deny;
      this.IsDeleted = isDeleted;
    }

    [DataMember]
    public string Token { get; set; }

    [DataMember(Name = "IdentityId")]
    public string Subject { get; set; }

    [DataMember]
    public int Allow { get; set; }

    [DataMember]
    public int Deny { get; set; }

    [DataMember]
    public bool IsDeleted { get; set; }
  }
}
