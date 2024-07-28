// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.Collection
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects;
using System;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal sealed class Collection
  {
    public bool isOuter;
    public SqlCollection inner;
    public string Name;

    public Collection(string name)
    {
      this.isOuter = true;
      this.Name = name;
    }

    public Collection(SqlCollection collection)
    {
      if ((SqlObject) collection == (SqlObject) null)
        throw new ArgumentNullException(nameof (collection));
      this.isOuter = false;
      this.inner = collection;
    }
  }
}
