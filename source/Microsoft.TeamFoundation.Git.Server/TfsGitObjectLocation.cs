// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitObjectLocation
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal struct TfsGitObjectLocation : 
    IEquatable<TfsGitObjectLocation>,
    IComparable<TfsGitObjectLocation>
  {
    public readonly ushort PackIntId;
    public readonly long Offset;
    public readonly long Length;

    public TfsGitObjectLocation(ushort packIntId, long offset, long length)
    {
      ArgumentUtility.CheckForOutOfRange(offset, nameof (offset), 0L);
      ArgumentUtility.CheckForOutOfRange(length, nameof (length), 0L);
      this.PackIntId = packIntId;
      this.Offset = offset;
      this.Length = length;
    }

    public bool IsValid => this.Length != 0L;

    public override bool Equals(object other)
    {
      TfsGitObjectLocation gitObjectLocation = this;
      TfsGitObjectLocation? nullable = other as TfsGitObjectLocation?;
      return nullable.HasValue && gitObjectLocation == nullable.GetValueOrDefault();
    }

    public bool Equals(TfsGitObjectLocation other) => this == other;

    public override int GetHashCode() => HashCodeUtil.GetHashCode<ushort, long, long>(this.PackIntId, this.Offset, this.Length);

    public int CompareTo(TfsGitObjectLocation other)
    {
      int num = (int) this.PackIntId - (int) other.PackIntId;
      return num == 0 ? this.Offset.CompareTo(other.Offset) : num;
    }

    public override string ToString() => "PackIntId: " + this.PackIntId.ToString() + " Offset: " + this.Offset.ToString() + " Length: " + this.Length.ToString();

    public static bool operator ==(TfsGitObjectLocation a, TfsGitObjectLocation b) => (int) a.PackIntId == (int) b.PackIntId && a.Offset == b.Offset && a.Length == b.Length;

    public static bool operator !=(TfsGitObjectLocation a, TfsGitObjectLocation b) => !(a == b);
  }
}
