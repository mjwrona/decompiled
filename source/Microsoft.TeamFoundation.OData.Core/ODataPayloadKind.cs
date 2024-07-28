// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataPayloadKind
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData
{
  public enum ODataPayloadKind
  {
    ResourceSet = 0,
    Resource = 1,
    Property = 2,
    EntityReferenceLink = 3,
    EntityReferenceLinks = 4,
    Value = 5,
    BinaryValue = 6,
    Collection = 7,
    ServiceDocument = 8,
    MetadataDocument = 9,
    Error = 10, // 0x0000000A
    Batch = 11, // 0x0000000B
    Parameter = 12, // 0x0000000C
    IndividualProperty = 13, // 0x0000000D
    Delta = 14, // 0x0000000E
    Asynchronous = 15, // 0x0000000F
    Unsupported = 2147483647, // 0x7FFFFFFF
  }
}
