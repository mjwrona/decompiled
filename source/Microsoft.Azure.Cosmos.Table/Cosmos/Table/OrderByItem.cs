// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.OrderByItem
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

namespace Microsoft.Azure.Cosmos.Table
{
  internal class OrderByItem
  {
    public const string ASC = "asc";
    public const string DESC = "desc";

    public OrderByItem(string propertyName, string order = "asc")
    {
      this.PropertyName = propertyName;
      this.Order = order;
    }

    public string PropertyName { get; private set; }

    public string Order { get; private set; }
  }
}
