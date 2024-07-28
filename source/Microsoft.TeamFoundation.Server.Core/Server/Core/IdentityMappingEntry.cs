// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.IdentityMappingEntry
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.Core
{
  [DataContract]
  public class IdentityMappingEntry
  {
    public static readonly string NoMappingToken = "[NOMAPPING]";

    public IdentityMappingEntry()
    {
    }

    public IdentityMappingEntry(string cloudIdentity, string displayName, string localIdentity)
    {
      this.CloudIdentity = cloudIdentity;
      this.DisplayName = displayName;
      this.LocalIdentity = localIdentity;
    }

    [DataMember]
    public string CloudIdentity { get; set; }

    [DataMember]
    public string DisplayName { get; set; }

    [DataMember]
    public string LocalIdentity { get; set; }
  }
}
