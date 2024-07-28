// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Interfaces.IWebApiRequestMessage
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Formatter.Deserialization;
using Microsoft.AspNet.OData.Routing;
using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace Microsoft.AspNet.OData.Interfaces
{
  internal interface IWebApiRequestMessage
  {
    IWebApiContext Context { get; }

    bool IsCountRequest();

    ODataRequestMethod Method { get; }

    IWebApiOptions Options { get; }

    IWebApiHeaders Headers { get; }

    IServiceProvider RequestContainer { get; }

    Uri RequestUri { get; }

    ODataDeserializerProvider DeserializerProvider { get; }

    string CreateETag(IDictionary<string, object> properties);

    ETag GetETag(EntityTagHeaderValue etagHeaderValue);

    ETag GetETag<TEntity>(EntityTagHeaderValue etagHeaderValue);

    Uri GetNextPageLink(int pageSize, object instance, Func<object, string> objToSkipTokenValue);

    IDictionary<string, string> ODataContentIdMapping { get; }

    IODataPathHandler PathHandler { get; }

    IDictionary<string, string> QueryParameters { get; }

    ODataMessageReaderSettings ReaderSettings { get; }

    ODataMessageWriterSettings WriterSettings { get; }
  }
}
