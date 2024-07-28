// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupMembership
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [DataContract]
  public sealed class GroupMembership
  {
    private Guid m_id;

    public GroupMembership(Guid queriedId, Guid id, IdentityDescriptor descriptor)
    {
      this.QueriedId = queriedId;
      this.Id = id;
      this.Descriptor = descriptor;
      this.Active = true;
    }

    [DataMember]
    public Guid QueriedId { get; set; }

    [DataMember]
    public Guid Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    [DataMember]
    public IdentityDescriptor Descriptor { get; set; }

    [DataMember]
    public bool Active { get; set; }

    public GroupMembership Clone() => new GroupMembership(this.QueriedId, this.Id, this.Descriptor == (IdentityDescriptor) null ? (IdentityDescriptor) null : new IdentityDescriptor(this.Descriptor))
    {
      Active = this.Active
    };

    public override string ToString() => string.Format("[Id = {0}, Descriptor = {1}, Active = {2}, QueriedId = {3}]", (object) this.Id, (object) this.Descriptor, (object) this.Active, (object) this.QueriedId);
  }
}
