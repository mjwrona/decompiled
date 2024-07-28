// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ObjectIdAndGitPackIndexEntry
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class ObjectIdAndGitPackIndexEntry : 
    IEquatable<ObjectIdAndGitPackIndexEntry>,
    IComparable<ObjectIdAndGitPackIndexEntry>
  {
    public readonly Sha1Id ObjectId;
    public readonly GitPackObjectType ObjectType;
    public readonly TfsGitObjectLocation Location;

    public ObjectIdAndGitPackIndexEntry(
      Sha1Id objectId,
      GitPackObjectType objectType,
      TfsGitObjectLocation location)
    {
      this.ObjectId = objectId;
      this.ObjectType = objectType;
      this.Location = location;
    }

    public override string ToString() => FormattableString.Invariant(FormattableStringFactory.Create("{0}: {1} {2}: {3} {4}: ({5})", (object) "ObjectId", (object) this.ObjectId, (object) "ObjectType", (object) this.ObjectType, (object) "Location", (object) this.Location));

    public override bool Equals(object other) => this == other as ObjectIdAndGitPackIndexEntry;

    public bool Equals(ObjectIdAndGitPackIndexEntry other) => this == other;

    public override int GetHashCode() => this.ObjectId.GetHashCode();

    public static bool operator ==(ObjectIdAndGitPackIndexEntry a, ObjectIdAndGitPackIndexEntry b)
    {
      if ((object) a == (object) b)
        return true;
      return (object) a != null && (object) b != null && a.ObjectId == b.ObjectId && a.ObjectType == b.ObjectType && a.Location == b.Location;
    }

    public static bool operator !=(ObjectIdAndGitPackIndexEntry a, ObjectIdAndGitPackIndexEntry b) => !(a == b);

    public int CompareTo(ObjectIdAndGitPackIndexEntry other) => this.Location.CompareTo(other.Location);
  }
}
