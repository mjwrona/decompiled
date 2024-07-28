// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.OperationType
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

namespace Microsoft.Azure.Documents
{
  internal enum OperationType
  {
    ExecuteJavaScript = -2, // 0xFFFFFFFE
    Invalid = -1, // 0xFFFFFFFF
    Create = 0,
    Patch = 1,
    Read = 2,
    ReadFeed = 3,
    Delete = 4,
    Replace = 5,
    Execute = 9,
    BatchApply = 13, // 0x0000000D
    SqlQuery = 14, // 0x0000000E
    Query = 15, // 0x0000000F
    Head = 18, // 0x00000012
    HeadFeed = 19, // 0x00000013
    Upsert = 20, // 0x00000014
    AddComputeGatewayRequestCharges = 37, // 0x00000025
    Batch = 40, // 0x00000028
    QueryPlan = 41, // 0x00000029
    CompleteUserTransaction = 52, // 0x00000034
    MetadataCheckAccess = 54, // 0x00000036
    CollectionTruncate = 57, // 0x00000039
  }
}
