// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ValidationHelpers
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents
{
  internal static class ValidationHelpers
  {
    public static bool ValidateConsistencyLevel(
      ConsistencyLevel backendConsistency,
      ConsistencyLevel desiredConsistency)
    {
      switch (backendConsistency)
      {
        case ConsistencyLevel.Strong:
          return desiredConsistency == ConsistencyLevel.Strong || desiredConsistency == ConsistencyLevel.BoundedStaleness || desiredConsistency == ConsistencyLevel.Session || desiredConsistency == ConsistencyLevel.Eventual || desiredConsistency == ConsistencyLevel.ConsistentPrefix;
        case ConsistencyLevel.BoundedStaleness:
          return desiredConsistency == ConsistencyLevel.BoundedStaleness || desiredConsistency == ConsistencyLevel.Session || desiredConsistency == ConsistencyLevel.Eventual || desiredConsistency == ConsistencyLevel.ConsistentPrefix;
        case ConsistencyLevel.Session:
        case ConsistencyLevel.Eventual:
        case ConsistencyLevel.ConsistentPrefix:
          return desiredConsistency == ConsistencyLevel.Session || desiredConsistency == ConsistencyLevel.Eventual || desiredConsistency == ConsistencyLevel.ConsistentPrefix;
        default:
          throw new ArgumentException(nameof (backendConsistency));
      }
    }
  }
}
