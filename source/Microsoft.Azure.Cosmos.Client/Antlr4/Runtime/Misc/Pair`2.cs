// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Misc.Pair`2
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Antlr4.Runtime.Misc
{
  internal class Pair<A, B>
  {
    public readonly A a;
    public readonly B b;

    public Pair(A a, B b)
    {
      this.a = a;
      this.b = b;
    }

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      if (!(obj is Pair<A, B>))
        return false;
      Pair<A, B> pair = (Pair<A, B>) obj;
      if (((object) this.a == null ? ((object) pair.a == null ? 1 : 0) : (this.a.Equals((object) pair.a) ? 1 : 0)) == 0)
        return false;
      return (object) this.b != null ? this.b.Equals((object) pair.b) : (object) pair.b == null;
    }

    public override int GetHashCode() => MurmurHash.Finish(MurmurHash.Update(MurmurHash.Update(MurmurHash.Initialize(), (object) this.a), (object) this.b), 2);

    public override string ToString() => string.Format("({0}, {1})", (object) this.a, (object) this.b);
  }
}
