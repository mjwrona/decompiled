// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ODataMessageWrapperHelper
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Formatter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;

namespace Microsoft.AspNet.OData
{
  internal static class ODataMessageWrapperHelper
  {
    internal static ODataMessageWrapper Create(Stream stream, HttpContentHeaders headers) => ODataMessageWrapperHelper.Create(stream, headers, (IDictionary<string, string>) null);

    internal static ODataMessageWrapper Create(
      Stream stream,
      HttpContentHeaders headers,
      IServiceProvider container)
    {
      return ODataMessageWrapperHelper.Create(stream, headers, (IDictionary<string, string>) null, container);
    }

    internal static ODataMessageWrapper Create(
      Stream stream,
      HttpContentHeaders headers,
      IDictionary<string, string> contentIdMapping,
      IServiceProvider container)
    {
      ODataMessageWrapper odataMessageWrapper = ODataMessageWrapperHelper.Create(stream, headers, contentIdMapping);
      odataMessageWrapper.Container = container;
      return odataMessageWrapper;
    }

    internal static ODataMessageWrapper Create(
      Stream stream,
      HttpContentHeaders headers,
      IDictionary<string, string> contentIdMapping)
    {
      return new ODataMessageWrapper(stream, headers.ToDictionary<KeyValuePair<string, IEnumerable<string>>, string, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, IEnumerable<string>>, string>) (kvp => string.Join(";", kvp.Value))), contentIdMapping);
    }
  }
}
