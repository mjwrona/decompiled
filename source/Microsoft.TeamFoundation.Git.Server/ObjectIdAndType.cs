// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ObjectIdAndType
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal struct ObjectIdAndType : IEquatable<ObjectIdAndType>
  {
    public readonly Sha1Id ObjectId;
    public readonly GitPackObjectType ObjectType;
    internal static readonly int Size = sizeof (ObjectIdAndType);

    public ObjectIdAndType(Sha1Id objectId, GitPackObjectType objectType)
    {
      this.ObjectId = objectId;
      this.ObjectType = objectType;
    }

    public override bool Equals(object obj) => obj is ObjectIdAndType other && this.Equals(other);

    public bool Equals(ObjectIdAndType other) => this.ObjectId == other.ObjectId && this.ObjectType == other.ObjectType;

    public override int GetHashCode() => HashCodeUtil.GetHashCode<Sha1Id, GitPackObjectType>(this.ObjectId, this.ObjectType);
  }
}
