// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Serialization.ODataErrorSerializer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData;
using System;
using System.Runtime.Serialization;
using System.Web.Http;

namespace Microsoft.AspNet.OData.Formatter.Serialization
{
  public class ODataErrorSerializer : ODataSerializer
  {
    internal static bool IsHttpError(object error) => error is HttpError;

    internal static ODataError CreateODataError(object error) => (error as HttpError).CreateODataError();

    public ODataErrorSerializer()
      : base(ODataPayloadKind.Error)
    {
    }

    public override void WriteObject(
      object graph,
      Type type,
      ODataMessageWriter messageWriter,
      ODataSerializerContext writeContext)
    {
      if (graph == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (graph));
      if (messageWriter == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (messageWriter));
      if (!(graph is ODataError error))
        error = ODataErrorSerializer.IsHttpError(graph) ? ODataErrorSerializer.CreateODataError(graph) : throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.ErrorTypeMustBeODataErrorOrHttpError, (object) graph.GetType().FullName));
      bool includeDebugInformation = error.InnerError != null;
      messageWriter.WriteError(error, includeDebugInformation);
    }
  }
}
