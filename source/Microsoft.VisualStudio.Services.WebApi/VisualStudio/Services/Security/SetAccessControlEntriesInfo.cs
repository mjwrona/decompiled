// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.SetAccessControlEntriesInfo
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Security
{
  [DataContract]
  public sealed class SetAccessControlEntriesInfo
  {
    public SetAccessControlEntriesInfo(
      string token,
      IEnumerable<AccessControlEntry> accessControlEntries,
      bool merge)
    {
      this.Token = token;
      this.Merge = merge;
      this.AccessControlEntries = new AccessControlEntriesCollection(accessControlEntries);
    }

    [DataMember]
    public string Token { get; private set; }

    [DataMember]
    public bool Merge { get; private set; }

    [DataMember]
    public AccessControlEntriesCollection AccessControlEntries { get; private set; }
  }
}
