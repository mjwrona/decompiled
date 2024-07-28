// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2.WrappedExceptionErrorSerializer
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.Data.OData;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web.Http.OData.Formatter.Serialization;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2
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
        graph = (object) new ODataError()
        {
          Message = wrappedException.Message
        };
      base.WriteObject(graph, type, messageWriter, writeContext);
    }
  }
}
