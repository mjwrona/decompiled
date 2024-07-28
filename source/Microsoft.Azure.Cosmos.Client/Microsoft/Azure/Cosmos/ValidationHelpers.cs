// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ValidationHelpers
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;

namespace Microsoft.Azure.Cosmos
{
  internal static class ValidationHelpers
  {
    public static bool IsValidConsistencyLevelOverwrite(
      ConsistencyLevel backendConsistency,
      ConsistencyLevel desiredConsistency,
      bool isLocalQuorumConsistency,
      OperationType operationType,
      ResourceType resourceType)
    {
      return ValidationHelpers.IsValidConsistencyLevelOverwrite((Microsoft.Azure.Documents.ConsistencyLevel) backendConsistency, (Microsoft.Azure.Documents.ConsistencyLevel) desiredConsistency, isLocalQuorumConsistency, new OperationType?(operationType), new ResourceType?(resourceType));
    }

    public static bool IsValidConsistencyLevelOverwrite(
      Microsoft.Azure.Documents.ConsistencyLevel backendConsistency,
      Microsoft.Azure.Documents.ConsistencyLevel desiredConsistency,
      bool isLocalQuorumConsistency,
      OperationType? operationType,
      ResourceType? resourceType)
    {
      return isLocalQuorumConsistency && ValidationHelpers.IsLocalQuorumConsistency(backendConsistency, desiredConsistency, operationType, resourceType) || ValidationHelpers.IsValidConsistencyLevelOverwrite(backendConsistency, desiredConsistency);
    }

    private static bool IsValidConsistencyLevelOverwrite(
      Microsoft.Azure.Documents.ConsistencyLevel backendConsistency,
      Microsoft.Azure.Documents.ConsistencyLevel desiredConsistency)
    {
      switch (backendConsistency)
      {
        case Microsoft.Azure.Documents.ConsistencyLevel.Strong:
          return desiredConsistency == Microsoft.Azure.Documents.ConsistencyLevel.Strong || desiredConsistency == Microsoft.Azure.Documents.ConsistencyLevel.BoundedStaleness || desiredConsistency == Microsoft.Azure.Documents.ConsistencyLevel.Session || desiredConsistency == Microsoft.Azure.Documents.ConsistencyLevel.Eventual || desiredConsistency == Microsoft.Azure.Documents.ConsistencyLevel.ConsistentPrefix;
        case Microsoft.Azure.Documents.ConsistencyLevel.BoundedStaleness:
          return desiredConsistency == Microsoft.Azure.Documents.ConsistencyLevel.BoundedStaleness || desiredConsistency == Microsoft.Azure.Documents.ConsistencyLevel.Session || desiredConsistency == Microsoft.Azure.Documents.ConsistencyLevel.Eventual || desiredConsistency == Microsoft.Azure.Documents.ConsistencyLevel.ConsistentPrefix;
        case Microsoft.Azure.Documents.ConsistencyLevel.Session:
        case Microsoft.Azure.Documents.ConsistencyLevel.Eventual:
        case Microsoft.Azure.Documents.ConsistencyLevel.ConsistentPrefix:
          return desiredConsistency == Microsoft.Azure.Documents.ConsistencyLevel.Session || desiredConsistency == Microsoft.Azure.Documents.ConsistencyLevel.Eventual || desiredConsistency == Microsoft.Azure.Documents.ConsistencyLevel.ConsistentPrefix;
        default:
          throw new ArgumentException("Invalid Backend Consistency i.e. " + backendConsistency.ToString());
      }
    }

    private static bool IsLocalQuorumConsistency(
      Microsoft.Azure.Documents.ConsistencyLevel backendConsistency,
      Microsoft.Azure.Documents.ConsistencyLevel desiredConsistency,
      OperationType? operationType,
      ResourceType? resourceType)
    {
      if (backendConsistency != Microsoft.Azure.Documents.ConsistencyLevel.Eventual || desiredConsistency != Microsoft.Azure.Documents.ConsistencyLevel.Strong || !resourceType.HasValue)
        return false;
      if (resourceType.HasValue)
      {
        ResourceType? nullable = resourceType;
        ResourceType resourceType1 = ResourceType.Document;
        if (!(nullable.GetValueOrDefault() == resourceType1 & nullable.HasValue))
          return false;
      }
      if (operationType.HasValue)
      {
        if (operationType.HasValue)
        {
          OperationType? nullable = operationType;
          OperationType operationType1 = OperationType.Read;
          if (!(nullable.GetValueOrDefault() == operationType1 & nullable.HasValue))
          {
            nullable = operationType;
            OperationType operationType2 = OperationType.ReadFeed;
            if (!(nullable.GetValueOrDefault() == operationType2 & nullable.HasValue))
              goto label_9;
          }
        }
        return true;
      }
label_9:
      return false;
    }
  }
}
