// Decompiled with JetBrains decompiler
// Type: Nest.ClearSqlCursorDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SqlApi;
using System;

namespace Nest
{
  public class ClearSqlCursorDescriptor : 
    RequestDescriptorBase<ClearSqlCursorDescriptor, ClearSqlCursorRequestParameters, IClearSqlCursorRequest>,
    IClearSqlCursorRequest,
    IRequest<ClearSqlCursorRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SqlClearCursor;

    string IClearSqlCursorRequest.Cursor { get; set; }

    public ClearSqlCursorDescriptor Cursor(string cursor) => this.Assign<string>(cursor, (Action<IClearSqlCursorRequest, string>) ((a, v) => a.Cursor = v));
  }
}
