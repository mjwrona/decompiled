// Decompiled with JetBrains decompiler
// Type: Nest.RequestMetaDataExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;

namespace Nest
{
  internal static class RequestMetaDataExtensions
  {
    internal static void AddHelper(this RequestMetaData metaData, string helperValue)
    {
      if (!metaData.TryAddMetaData("helper", helperValue))
        throw new InvalidOperationException("A helper value has already been added.");
    }

    internal static void AddSnapshotHelper(this RequestMetaData metaData) => metaData.AddHelper("sn");

    internal static void AddScrollHelper(this RequestMetaData metaData) => metaData.AddHelper("s");

    internal static void AddReindexHelper(this RequestMetaData metaData) => metaData.AddHelper("r");

    internal static void AddBulkHelper(this RequestMetaData metaData) => metaData.AddHelper("b");

    internal static void AddRestoreHelper(this RequestMetaData metaData) => metaData.AddHelper("sr");
  }
}
