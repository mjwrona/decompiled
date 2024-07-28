// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.WrappedExceptionErrorSerializer
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.OData;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class WrappedExceptionErrorSerializer : ODataErrorSerializer
  {
    public override void WriteObject(
      object graph,
      Type type,
      ODataMessageWriter messageWriter,
      ODataSerializerContext writeContext)
    {
      if (graph is WrappedException wrappedException)
      {
        Exception exception = wrappedException.Unwrap((IDictionary<string, Type>) null);
        string str1 = (wrappedException.Message ?? "").Replace("{", "{{").Replace("}", "}}");
        string str2 = wrappedException.ErrorCode.ToString();
        if (exception is ODataException)
          str1 = string.Format(AnalyticsResources.URI_QUERY_STRING_INVALID((object) str1));
        graph = (object) new ODataError()
        {
          Message = str1,
          ErrorCode = str2,
          InnerError = new ODataInnerError(exception)
        };
      }
      base.WriteObject(graph, type, messageWriter, writeContext);
    }
  }
}
