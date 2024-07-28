// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryPermissionsEntry
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Directories
{
  [DataContract]
  public class DirectoryPermissionsEntry
  {
    [DataMember]
    private readonly Guid namespaceId;
    [DataMember]
    private readonly string token;
    [DataMember]
    private readonly int allow;
    [DataMember]
    private readonly int deny;
    [DataMember]
    private readonly bool merge;

    public Guid NamespaceId => this.namespaceId;

    public string Token => this.token;

    public int Allow => this.allow;

    public int Deny => this.deny;

    public bool Merge => this.merge;

    public DirectoryPermissionsEntry(
      Guid namespaceId,
      string token,
      int allow = 0,
      int deny = 0,
      bool merge = true)
    {
      this.namespaceId = namespaceId;
      this.token = token;
      this.allow = allow;
      this.deny = deny;
      this.merge = merge;
    }
  }
}
