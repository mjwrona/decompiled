// Decompiled with JetBrains decompiler
// Type: Nest.RequestMetaDataFactory
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;

namespace Nest
{
  internal static class RequestMetaDataFactory
  {
    internal static RequestMetaData ReindexHelperRequestMetaData()
    {
      RequestMetaData metaData = new RequestMetaData();
      metaData.AddReindexHelper();
      return metaData;
    }

    internal static RequestMetaData ScrollHelperRequestMetaData()
    {
      RequestMetaData metaData = new RequestMetaData();
      metaData.AddScrollHelper();
      return metaData;
    }

    internal static RequestMetaData BulkHelperRequestMetaData()
    {
      RequestMetaData metaData = new RequestMetaData();
      metaData.AddBulkHelper();
      return metaData;
    }

    internal static RequestMetaData SnapshotHelperRequestMetaData()
    {
      RequestMetaData metaData = new RequestMetaData();
      metaData.AddSnapshotHelper();
      return metaData;
    }

    internal static RequestMetaData RestoreHelperRequestMetaData()
    {
      RequestMetaData metaData = new RequestMetaData();
      metaData.AddRestoreHelper();
      return metaData;
    }
  }
}
