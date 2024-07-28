// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionValidatorController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi.Commerce;
using Microsoft.VisualStudio.Services.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "extensionValidator")]
  public class ExtensionValidatorController : GalleryController
  {
    private static readonly string _operationId = Guid.NewGuid().ToString();
    [StaticSafe]
    private static readonly JsonSerializerSettings MediaTypeFormatterSettings = new JsonSerializerSettings()
    {
      NullValueHandling = NullValueHandling.Ignore,
      Formatting = Newtonsoft.Json.Formatting.Indented,
      ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver(),
      Converters = (IList<JsonConverter>) new List<JsonConverter>()
      {
        (JsonConverter) new StringEnumConverter()
      }
    };
    [StaticSafe]
    private static readonly MediaTypeFormatter MediaTypeFormatter;

    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("05E8A5E1-8C59-4C2C-8856-0FF087D1A844")]
    public HttpResponseMessage ExtensionValidator([FromBody] AzureRestApiRequestModel azureRestApiRequestModel)
    {
      try
      {
        if (this.TfsRequestContext.GetService<IPublishedExtensionService>().IsValidAzurePublisherAndExtension(this.TfsRequestContext, azureRestApiRequestModel, ExtensionQueryFlags.None))
          return this.GetResponseMessage(HttpStatusCode.OK, "Success", this.Request);
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case ExtensionDoesNotExistException _:
            return this.GetResponseMessage(HttpStatusCode.NotFound, ex.Message, this.Request);
          case AccessCheckException _:
          case SecurityException _:
            return this.GetResponseMessage(HttpStatusCode.Forbidden, ex.Message, this.Request);
          case ArgumentNullException _:
          case ArgumentException _:
            return this.GetResponseMessage(HttpStatusCode.BadRequest, ex.Message, this.Request);
          default:
            return this.GetResponseMessage(HttpStatusCode.InternalServerError, ex.Message, this.Request);
        }
      }
      return this.GetResponseMessage(HttpStatusCode.BadRequest, "BadRequest", this.Request);
    }

    private HttpResponseMessage GetResponseMessage(
      HttpStatusCode statusCode,
      string message,
      HttpRequestMessage httpRequestMessage)
    {
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new ObjectContent<AzureRestApiResponseModel>(new AzureRestApiResponseModel()
      {
        OperationStatus = new RestApiResponseStatusModel()
        {
          Status = statusCode == HttpStatusCode.OK ? RestApiResponseStatus.Completed : RestApiResponseStatus.Failed,
          StatusMessage = message,
          OperationId = ExtensionValidatorController._operationId,
          PercentageCompleted = statusCode == HttpStatusCode.OK ? 100 : 0
        }
      }, ExtensionValidatorController.MediaTypeFormatter);
      response.Content.Headers.Add("x-ms-client-request-id", httpRequestMessage.Headers.GetValues("x-ms-client-request-id"));
      return response;
    }

    static ExtensionValidatorController()
    {
      JsonMediaTypeFormatter mediaTypeFormatter = new JsonMediaTypeFormatter();
      mediaTypeFormatter.SerializerSettings = ExtensionValidatorController.MediaTypeFormatterSettings;
      mediaTypeFormatter.UseDataContractJsonSerializer = false;
      ExtensionValidatorController.MediaTypeFormatter = (MediaTypeFormatter) mediaTypeFormatter;
    }
  }
}
