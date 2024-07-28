// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.ReferenceEqualityComparer
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System.Collections;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal class ReferenceEqualityComparer : IEqualityComparer
  {
    protected ReferenceEqualityComparer()
    {
    }

    bool IEqualityComparer.Equals(object x, object y) => x == y;

    int IEqualityComparer.GetHashCode(object obj) => obj == null ? 0 : obj.GetHashCode();
  }
}
