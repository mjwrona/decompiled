// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.TupleInternal`2
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm
{
  internal class TupleInternal<T1, T2>
  {
    public TupleInternal(T1 item1, T2 item2)
    {
      this.Item1 = item1;
      this.Item2 = item2;
    }

    public T1 Item1 { get; private set; }

    public T2 Item2 { get; private set; }
  }
}
