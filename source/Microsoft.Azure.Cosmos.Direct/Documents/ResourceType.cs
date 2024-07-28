// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ResourceType
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

namespace Microsoft.Azure.Documents
{
  internal enum ResourceType
  {
    ControllerService = -6, // 0xFFFFFFFA
    Address = -5, // 0xFFFFFFFB
    Media = -3, // 0xFFFFFFFD
    Key = -2, // 0xFFFFFFFE
    Unknown = -1, // 0xFFFFFFFF
    Database = 0,
    Collection = 1,
    Document = 2,
    Attachment = 3,
    User = 4,
    Permission = 5,
    Conflict = 107, // 0x0000006B
    Record = 108, // 0x0000006C
    StoredProcedure = 109, // 0x0000006D
    Trigger = 110, // 0x0000006E
    UserDefinedFunction = 111, // 0x0000006F
    BatchApply = 112, // 0x00000070
    Offer = 113, // 0x00000071
    DatabaseAccount = 118, // 0x00000076
    SchemaContainer = 123, // 0x0000007B
    Schema = 124, // 0x0000007C
    PartitionKeyRange = 125, // 0x0000007D
    ComputeGatewayCharges = 131, // 0x00000083
    UserDefinedType = 133, // 0x00000085
    Batch = 135, // 0x00000087
    PartitionKey = 136, // 0x00000088
    Snapshot = 137, // 0x00000089
    PartitionedSystemDocument = 138, // 0x0000008A
    ClientEncryptionKey = 141, // 0x0000008D
    Transaction = 145, // 0x00000091
    RoleDefinition = 146, // 0x00000092
    RoleAssignment = 147, // 0x00000093
    SystemDocument = 148, // 0x00000094
    InteropUser = 149, // 0x00000095
    AuthPolicyElement = 150, // 0x00000096
    RetriableWriteCachedResponse = 153, // 0x00000099
    EncryptionScope = 156, // 0x0000009C
    Telemetry = 1001, // 0x000003E9
  }
}
