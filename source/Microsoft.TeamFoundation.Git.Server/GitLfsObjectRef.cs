// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitLfsObjectRef
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DataContract]
  public class GitLfsObjectRef : IEquatable<GitLfsObjectRef>
  {
    public GitLfsObjectRef()
    {
    }

    public GitLfsObjectRef(string lfsObjectId, long sizeInBytes)
    {
      this.OidStr = lfsObjectId;
      this.Size = sizeInBytes;
    }

    public GitLfsObjectRef(Sha256Id lfsObjectId, long sizeInBytes)
      : this(lfsObjectId.ToString(), sizeInBytes)
    {
    }

    [DataMember(Name = "oid", EmitDefaultValue = false, IsRequired = true)]
    public string OidStr { get; private set; }

    public Sha256Id Oid => new Sha256Id(this.OidStr);

    [DataMember(Name = "size", EmitDefaultValue = true, IsRequired = true)]
    public long Size { get; set; }

    [DataMember(Name = "actions", EmitDefaultValue = false, IsRequired = false)]
    public IDictionary<string, GitLfsObjectAction> Actions { get; set; }

    [DataMember(Name = "error", EmitDefaultValue = false, IsRequired = false)]
    public GitLfsObjectError Error { get; set; }

    public override bool Equals(object obj) => this.Equals(obj as GitLfsObjectRef);

    public bool Equals(GitLfsObjectRef other)
    {
      if ((object) other == null)
        return false;
      if ((object) this == (object) other)
        return true;
      return !(other.GetType() != this.GetType()) && this.Size == other.Size && string.Equals(this.OidStr, other.OidStr);
    }

    public override int GetHashCode() => this.Size.GetHashCode() * 397 ^ this.OidStr.GetHashCode();

    public static bool operator ==(GitLfsObjectRef first, GitLfsObjectRef second) => (object) first == null ? (object) second == null : first.Equals(second);

    public static bool operator !=(GitLfsObjectRef first, GitLfsObjectRef second) => !(first == second);
  }
}
