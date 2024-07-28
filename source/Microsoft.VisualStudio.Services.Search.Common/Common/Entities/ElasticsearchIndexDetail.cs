// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.ElasticsearchIndexDetail
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  public class ElasticsearchIndexDetail
  {
    public ElasticsearchIndexDetail()
    {
    }

    public ElasticsearchIndexDetail(string indexName, int documentCount, int deletedDocumentCount)
    {
      this.IndexName = !string.IsNullOrWhiteSpace(indexName) ? indexName : throw new ArgumentNullException(nameof (indexName));
      this.DocumentCount = (long) documentCount;
      this.DeletedDocumentCount = (long) deletedDocumentCount;
    }

    public string IndexName { get; set; }

    public long DocumentCount { get; set; }

    public long DeletedDocumentCount { get; set; }
  }
}
