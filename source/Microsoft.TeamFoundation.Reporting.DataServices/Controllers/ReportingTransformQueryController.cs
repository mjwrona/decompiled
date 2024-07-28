// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Controllers.ReportingTransformQueryController
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices.Services;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Controllers
{
  public class ReportingTransformQueryController : ChartingProjectControllerBase
  {
    private static readonly Dictionary<Type, HttpStatusCode> s_HttpExceptions = ControllerHelpers.getTransformHttpExceptions();

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) ReportingTransformQueryController.s_HttpExceptions;

    public override string TraceArea => "TransformQuery";

    [TraceFilter(1017300, 1017310)]
    [HttpPost]
    [ClientResponseType(typeof (IEnumerable<TransformResult>), null, null)]
    [PublicProjectRequestRestrictions(false, false, null)]
    public HttpResponseMessage RunTransformQuery(string scope, TransformOptions[] transformOptions)
    {
      IDataTransformService service = this.TfsRequestContext.GetService<IDataTransformService>();
      HttpResponseMessage response = this.Request.CreateResponse();
      Guid? projectId = this.ProjectId == Guid.Empty ? new Guid?() : new Guid?(this.ProjectId);
      List<TransformResult> list = service.GetResults(this.TfsRequestContext, scope, projectId, (IEnumerable<TransformOptions>) transformOptions).ToList<TransformResult>();
      IDataServicesWriter writer = service.GetWriter(this.TfsRequestContext);
      writer.QueryInterpreter = service.GetQueryTextInterpreter(this.TfsRequestContext, scope);
      response.Content = (HttpContent) this.WrapRecordStream(response, writer, (IEnumerable<TransformResult>) list);
      return response;
    }

    private PushStreamContent WrapRecordStream(
      HttpResponseMessage response,
      IDataServicesWriter writer,
      IEnumerable<TransformResult> transformResults)
    {
      return (PushStreamContent) new VssServerPushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, content, context) =>
      {
        try
        {
          writer.WriteResultsToStream(stream, transformResults);
        }
        catch (Exception ex)
        {
          response.StatusCode = this.MapException(ex);
        }
        finally
        {
          stream.Close();
        }
      }), writer.ContentType, (object) transformResults);
    }
  }
}
