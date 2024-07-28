// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.ResourceExpressionType
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal enum ResourceExpressionType
  {
    RootResourceSet = 10000, // 0x00002710
    ResourceNavigationProperty = 10001, // 0x00002711
    ResourceNavigationPropertySingleton = 10002, // 0x00002712
    TakeQueryOption = 10003, // 0x00002713
    SkipQueryOption = 10004, // 0x00002714
    OrderByQueryOption = 10005, // 0x00002715
    FilterQueryOption = 10006, // 0x00002716
    InputReference = 10007, // 0x00002717
    ProjectionQueryOption = 10008, // 0x00002718
    RequestOptions = 10009, // 0x00002719
    OperationContext = 10010, // 0x0000271A
    Resolver = 10011, // 0x0000271B
  }
}
