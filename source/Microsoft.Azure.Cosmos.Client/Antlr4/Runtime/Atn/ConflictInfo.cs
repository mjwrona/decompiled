// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.ConflictInfo
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Sharpen;

namespace Antlr4.Runtime.Atn
{
  internal class ConflictInfo
  {
    private readonly BitSet conflictedAlts;
    private readonly bool exact;

    public ConflictInfo(BitSet conflictedAlts, bool exact)
    {
      this.conflictedAlts = conflictedAlts;
      this.exact = exact;
    }

    public BitSet ConflictedAlts => this.conflictedAlts;

    public bool IsExact => this.exact;

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      if (!(obj is ConflictInfo))
        return false;
      ConflictInfo conflictInfo = (ConflictInfo) obj;
      return this.IsExact == conflictInfo.IsExact && object.Equals((object) this.ConflictedAlts, (object) conflictInfo.ConflictedAlts);
    }

    public override int GetHashCode() => this.ConflictedAlts.GetHashCode();
  }
}
