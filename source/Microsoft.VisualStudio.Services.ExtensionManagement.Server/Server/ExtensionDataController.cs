// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionDataController
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  [VersionedApiControllerCustomName(Area = "ExtensionManagement", ResourceName = "Data")]
  [ClientInternalUseOnly(false)]
  public class ExtensionDataController : TfsApiController
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<DocumentDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<DocumentCollectionDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<DocumentExistsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidDocumentVersionException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ExtensionInvalidAuthorizationException>(HttpStatusCode.Unauthorized);
      exceptionMap.AddStatusCode<TokenMissingHostAuthorizationClaimException>(HttpStatusCode.Unauthorized);
    }

    [HttpGet]
    [ClientLocationId("BBE06C18-1C8B-4FCD-B9C6-1535AAAB8749")]
    public JObject GetDocumentByName(
      string publisherName,
      string extensionName,
      string scopeType,
      string scopeValue,
      string collectionName,
      string documentId)
    {
      return this.TfsRequestContext.GetService<ExtensionDataService>().GetDocument(this.TfsRequestContext, collectionName, scopeType, scopeValue, documentId, publisherName, extensionName);
    }

    [HttpGet]
    [ClientResponseType(typeof (List<JObject>), null, null)]
    [ClientLocationId("BBE06C18-1C8B-4FCD-B9C6-1535AAAB8749")]
    public HttpResponseMessage GetDocumentsByName(
      string publisherName,
      string extensionName,
      string scopeType,
      string scopeValue,
      string collectionName)
    {
      ExtensionDataService service = this.TfsRequestContext.GetService<ExtensionDataService>();
      if (!this.TfsRequestContext.IsFeatureEnabled(ExtensionManagementFeatureFlags.EnableDocumentsResponseStreaming))
        return this.Request.CreateResponse<List<JObject>>(HttpStatusCode.OK, service.GetDocuments(this.TfsRequestContext, collectionName, scopeType, scopeValue, publisherName, extensionName));
      IEnumerable<string> document = service.GetDocumentsAsString(this.TfsRequestContext, collectionName, scopeType, scopeValue, publisherName, extensionName);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      UTF8Encoding encoding = new UTF8Encoding();
      response.Content = (HttpContent) new PushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, content, context) =>
      {
        try
        {
          StreamWriter streamWriter = new StreamWriter(stream, (Encoding) encoding, 80000);
          streamWriter.Write("[");
          bool flag = true;
          foreach (string str in document)
          {
            if (!flag)
              streamWriter.Write(",");
            streamWriter.Write(str);
            flag = false;
          }
          streamWriter.Write("]");
          streamWriter.Flush();
        }
        catch (Exception ex)
        {
          this.TfsRequestContext.TraceException(10013596, "GetDocumentsAsStream", "ExtensionsArea", ex);
        }
        finally
        {
          stream.Dispose();
        }
      }), "application/json");
      this.TfsRequestContext.UpdateTimeToFirstPage();
      return response;
    }

    [HttpPut]
    [ClientLocationId("BBE06C18-1C8B-4FCD-B9C6-1535AAAB8749")]
    public JObject SetDocumentByName(
      string publisherName,
      string extensionName,
      string scopeType,
      string scopeValue,
      string collectionName,
      JObject doc)
    {
      long? contentLength = this.Request.Content.Headers.ContentLength;
      return this.TfsRequestContext.GetService<ExtensionDataService>().SetDocument(this.TfsRequestContext, collectionName, scopeType, scopeValue, doc, publisherName, extensionName, contentLength);
    }

    [HttpPost]
    [ClientResponseType(typeof (JObject), null, null)]
    [ClientLocationId("BBE06C18-1C8B-4FCD-B9C6-1535AAAB8749")]
    public HttpResponseMessage CreateDocumentByName(
      string publisherName,
      string extensionName,
      string scopeType,
      string scopeValue,
      string collectionName,
      JObject doc)
    {
      long? contentLength = this.Request.Content.Headers.ContentLength;
      return this.Request.CreateResponse<JObject>(HttpStatusCode.Created, this.TfsRequestContext.GetService<ExtensionDataService>().CreateDocument(this.TfsRequestContext, collectionName, scopeType, scopeValue, doc, publisherName, extensionName, contentLength));
    }

    [HttpPatch]
    [ClientLocationId("BBE06C18-1C8B-4FCD-B9C6-1535AAAB8749")]
    public JObject UpdateDocumentByName(
      string publisherName,
      string extensionName,
      string scopeType,
      string scopeValue,
      string collectionName,
      JObject doc)
    {
      long? contentLength = this.Request.Content.Headers.ContentLength;
      return this.TfsRequestContext.GetService<ExtensionDataService>().UpdateDocument(this.TfsRequestContext, collectionName, scopeType, scopeValue, doc, publisherName, extensionName, contentLength);
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("BBE06C18-1C8B-4FCD-B9C6-1535AAAB8749")]
    public HttpResponseMessage DeleteDocumentByName(
      string publisherName,
      string extensionName,
      string scopeType,
      string scopeValue,
      string collectionName,
      string documentId)
    {
      this.TfsRequestContext.GetService<ExtensionDataService>().DeleteDocument(this.TfsRequestContext, collectionName, scopeType, scopeValue, documentId, publisherName, extensionName);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }
  }
}
