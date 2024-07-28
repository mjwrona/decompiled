// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ResourceTypeExtensions
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents
{
  internal static class ResourceTypeExtensions
  {
    private static Dictionary<int, string> resourceTypeNames = new Dictionary<int, string>();

    static ResourceTypeExtensions()
    {
      foreach (ResourceType key in Enum.GetValues(typeof (ResourceType)))
        ResourceTypeExtensions.resourceTypeNames[(int) key] = key.ToString();
    }

    public static string ToResourceTypeString(this ResourceType type) => ResourceTypeExtensions.resourceTypeNames[(int) type];

    public static bool IsPartitioned(this ResourceType type) => type == ResourceType.Document || type == ResourceType.Attachment || type == ResourceType.Conflict || type == ResourceType.PartitionKey || type == ResourceType.PartitionedSystemDocument || type == ResourceType.RetriableWriteCachedResponse;

    public static bool IsCollectionChild(this ResourceType type) => type == ResourceType.Document || type == ResourceType.Attachment || type == ResourceType.Conflict || type == ResourceType.Schema || type == ResourceType.PartitionKey || type == ResourceType.PartitionedSystemDocument || type == ResourceType.SystemDocument || type.IsScript();

    public static bool IsScript(this ResourceType type) => type == ResourceType.UserDefinedFunction || type == ResourceType.Trigger || type == ResourceType.StoredProcedure;
  }
}
