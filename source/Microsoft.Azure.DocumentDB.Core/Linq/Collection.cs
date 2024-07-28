// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.Collection
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Sql;
using System;

namespace Microsoft.Azure.Documents.Linq
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
      if (collection == null)
        throw new ArgumentNullException(nameof (collection));
      this.isOuter = false;
      this.inner = collection;
    }
  }
}
