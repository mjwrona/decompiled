// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Misc.ArrayList`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;

namespace Antlr4.Runtime.Misc
{
  internal class ArrayList<T> : List<T>
  {
    public ArrayList()
    {
    }

    public ArrayList(int count)
      : base(count)
    {
    }

    public override int GetHashCode()
    {
      int hash = MurmurHash.Initialize(1);
      foreach (T obj in (List<T>) this)
        hash = MurmurHash.Update(hash, obj.GetHashCode());
      return MurmurHash.Finish(hash, this.Count);
    }

    public override bool Equals(object o)
    {
      if (o == this)
        return true;
      return o is List<T> && this.Equals((List<T>) o);
    }

    public bool Equals(List<T> o)
    {
      if (this.Count != o.Count)
        return false;
      IEnumerator<T> enumerator1 = (IEnumerator<T>) this.GetEnumerator();
      IEnumerator<T> enumerator2 = (IEnumerator<T>) o.GetEnumerator();
      while (enumerator1.MoveNext() && enumerator2.MoveNext())
      {
        if (!enumerator1.Current.Equals((object) enumerator2.Current))
          return false;
      }
      return true;
    }
  }
}
