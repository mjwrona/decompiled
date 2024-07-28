// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ResourceType
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents
{
  internal enum ResourceType
  {
    ControllerService = -6, // 0xFFFFFFFA
    Address = -5, // 0xFFFFFFFB
    ServiceFabricService = -4, // 0xFFFFFFFC
    Media = -3, // 0xFFFFFFFD
    Key = -2, // 0xFFFFFFFE
    Unknown = -1, // 0xFFFFFFFF
    Database = 0,
    Collection = 1,
    Document = 2,
    Attachment = 3,
    User = 4,
    Permission = 5,
    Progress = 6,
    Replica = 7,
    Tombstone = 8,
    Module = 9,
    SmallMaxInvalid = 10, // 0x0000000A
    LargeInvalid = 100, // 0x00000064
    ModuleCommand = 103, // 0x00000067
    Index = 104, // 0x00000068
    IndexBookmark = 105, // 0x00000069
    IndexSize = 106, // 0x0000006A
    Conflict = 107, // 0x0000006B
    Record = 108, // 0x0000006C
    StoredProcedure = 109, // 0x0000006D
    Trigger = 110, // 0x0000006E
    UserDefinedFunction = 111, // 0x0000006F
    BatchApply = 112, // 0x00000070
    Offer = 113, // 0x00000071
    PartitionSetInformation = 114, // 0x00000072
    XPReplicatorAddress = 115, // 0x00000073
    Timestamp = 117, // 0x00000075
    DatabaseAccount = 118, // 0x00000076
    MasterPartition = 120, // 0x00000078
    ServerPartition = 121, // 0x00000079
    Topology = 122, // 0x0000007A
    SchemaContainer = 123, // 0x0000007B
    Schema = 124, // 0x0000007C
    PartitionKeyRange = 125, // 0x0000007D
    LogStoreLogs = 126, // 0x0000007E
    RestoreMetadata = 127, // 0x0000007F
    PreviousImage = 128, // 0x00000080
    VectorClock = 129, // 0x00000081
    RidRange = 130, // 0x00000082
    ComputeGatewayCharges = 131, // 0x00000083
    UserDefinedType = 133, // 0x00000085
    Batch = 135, // 0x00000087
    PartitionKey = 136, // 0x00000088
    Snapshot = 137, // 0x00000089
    ClientEncryptionKey = 141, // 0x0000008D
  }
}
