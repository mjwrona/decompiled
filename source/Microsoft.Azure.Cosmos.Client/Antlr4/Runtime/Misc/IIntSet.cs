// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Misc.IIntSet
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;

namespace Antlr4.Runtime.Misc
{
  internal interface IIntSet
  {
    void Add(int el);

    [return: NotNull]
    IIntSet AddAll(IIntSet set);

    [return: Nullable]
    IIntSet And(IIntSet a);

    [return: Nullable]
    IIntSet Complement(IIntSet elements);

    [return: Nullable]
    IIntSet Or(IIntSet a);

    [return: Nullable]
    IIntSet Subtract(IIntSet a);

    int Count { get; }

    bool IsNil { get; }

    bool Equals(object obj);

    int SingleElement { get; }

    bool Contains(int el);

    void Remove(int el);

    [return: NotNull]
    IList<int> ToList();

    string ToString();
  }
}
