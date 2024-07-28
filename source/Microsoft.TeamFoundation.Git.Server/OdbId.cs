// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.OdbId
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server
{
  public struct OdbId : IEquatable<OdbId>
  {
    public readonly Guid Value;

    public OdbId(Guid value) => this.Value = value;

    public void CheckValid()
    {
      if (this.Value == Guid.Empty)
        throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("The {0} {1} is invalid.", (object) nameof (OdbId), (object) this)));
    }

    public static bool operator ==(OdbId a, OdbId b) => a.Value == b.Value;

    public static bool operator !=(OdbId a, OdbId b) => a.Value != b.Value;

    public bool Equals(OdbId other) => this == other;

    public override bool Equals(object other)
    {
      OdbId odbId = this;
      OdbId? nullable = other as OdbId?;
      return nullable.HasValue && odbId == nullable.GetValueOrDefault();
    }

    public override int GetHashCode() => this.Value.GetHashCode();

    public override string ToString() => this.Value.ToString();

    public static OdbId Parse(string input) => new OdbId(Guid.Parse(input));
  }
}
