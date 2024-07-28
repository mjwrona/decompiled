// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.PatchConstants
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos
{
  internal static class PatchConstants
  {
    public static string ToEnumMemberString(this PatchOperationType patchOperationType)
    {
      switch (patchOperationType)
      {
        case PatchOperationType.Add:
          return "add";
        case PatchOperationType.Remove:
          return "remove";
        case PatchOperationType.Replace:
          return "replace";
        case PatchOperationType.Set:
          return "set";
        case PatchOperationType.Increment:
          return "incr";
        default:
          throw new ArgumentException(string.Format("Unknown Patch operation type '{0}'.", (object) patchOperationType));
      }
    }

    public static class PropertyNames
    {
      public const string OperationType = "op";
      public const string Path = "path";
      public const string Value = "value";
    }

    public static class PatchSpecAttributes
    {
      public const string Operations = "operations";
      public const string Condition = "condition";
    }

    public static class OperationTypeNames
    {
      public const string Add = "add";
      public const string Remove = "remove";
      public const string Replace = "replace";
      public const string Set = "set";
      public const string Increment = "incr";
    }
  }
}
