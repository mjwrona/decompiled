// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.SecureFileController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.2)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "securefiles")]
  public class SecureFileController : DistributedTaskProjectApiController
  {
    public override string ActivityLogArea => "SecureFile";

    private ISecureFileService SecureFileService => this.TfsRequestContext.GetService<ISecureFileService>();

    [HttpPost]
    [ClientResponseType(typeof (SecureFile), null, null)]
    [ClientRequestContentIsRawData]
    [ClientRequestContentMediaType("application/octet-stream")]
    [ClientRequestBodyIsStream]
    public HttpResponseMessage UploadSecureFile([ClientQueryParameter] string name, bool authorizePipelines = false)
    {
      HttpContent content = this.Request.Content;
      if (content == null)
        return this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, Microsoft.TeamFoundation.DistributedTask.Server.TaskResources.SecureFileWithNoContentError());
      if (!content.Headers.ContentLength.HasValue)
        return this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, Microsoft.TeamFoundation.DistributedTask.Server.TaskResources.SecureFileWithNoContentLengthError());
      SecureFile secureFile = this.SecureFileService.UploadSecureFile(this.TfsRequestContext, this.ProjectId, name, content.ReadAsStreamAsync().Result, authorizePipelines);
      if (secureFile != null)
      {
        string feature = string.Format("{0}.{1}", (object) "SecureFile", (object) "Upload");
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("FileExtension", Path.GetExtension(name));
        properties.Add("FileContentLength", (object) content.Headers.ContentLength);
        this.TfsRequestContext.GetService<CustomerIntelligenceService>().Publish(this.TfsRequestContext, "DistributedTask", feature, properties);
      }
      return this.Request.CreateResponse<SecureFile>(HttpStatusCode.OK, secureFile);
    }

    [HttpGet]
    [ClientResponseType(typeof (Stream), "DownloadSecureFile", "application/octet-stream")]
    public HttpResponseMessage DownloadSecureFile(
      Guid secureFileId,
      string ticket,
      bool download = false,
      [FromUri(Name = "$format"), ClientIgnore] string format = null)
    {
      return this.CreateDownloadResponse(secureFileId, ticket, download, format);
    }

    [HttpGet]
    public async Task<SecureFile> GetSecureFile(
      Guid secureFileId,
      bool includeDownloadTicket = false,
      SecureFileActionFilter actionFilter = SecureFileActionFilter.None)
    {
      SecureFileController secureFileController = this;
      return await secureFileController.SecureFileService.GetSecureFileAsync(secureFileController.TfsRequestContext, secureFileController.ProjectId, secureFileId, includeDownloadTicket, actionFilter) ?? throw new SecureFileNotFoundException(Microsoft.TeamFoundation.DistributedTask.Server.TaskResources.SecureFileNotFound((object) secureFileId));
    }

    [HttpGet]
    public Task<IList<SecureFile>> GetSecureFilesByIds(
      [ClientParameterAsIEnumerable(typeof (Guid), ',')] string secureFileIds,
      bool includeDownloadTickets = false,
      SecureFileActionFilter actionFilter = SecureFileActionFilter.None)
    {
      return this.SecureFileService.GetSecureFilesAsync(this.TfsRequestContext, this.ProjectId, (IEnumerable<Guid>) this.ParseIdsAsGuidArray(secureFileIds), includeDownloadTickets, actionFilter);
    }

    [HttpGet]
    public Task<IList<SecureFile>> GetSecureFilesByNames(
      [ClientParameterAsIEnumerable(typeof (string), ',')] string secureFileNames,
      bool includeDownloadTickets = false,
      SecureFileActionFilter actionFilter = SecureFileActionFilter.None)
    {
      return this.SecureFileService.GetSecureFilesAsync(this.TfsRequestContext, this.ProjectId, (IEnumerable<string>) ArtifactPropertyKinds.AsPropertyFilters(secureFileNames), includeDownloadTickets, actionFilter);
    }

    [HttpGet]
    public IList<SecureFile> GetSecureFiles(
      [ClientQueryParameter] string namePattern = null,
      bool includeDownloadTickets = false,
      SecureFileActionFilter actionFilter = SecureFileActionFilter.None)
    {
      return this.SecureFileService.GetSecureFiles(this.TfsRequestContext, this.ProjectId, namePattern, includeDownloadTickets, actionFilter);
    }

    [HttpDelete]
    public void DeleteSecureFile(Guid secureFileId) => this.SecureFileService.DeleteSecureFile(this.TfsRequestContext, this.ProjectId, secureFileId);

    [HttpPatch]
    public SecureFile UpdateSecureFile(Guid secureFileId, SecureFile secureFile) => this.SecureFileService.UpdateSecureFile(this.TfsRequestContext, this.ProjectId, secureFileId, secureFile);

    [HttpPatch]
    public IList<SecureFile> UpdateSecureFiles(IList<SecureFile> secureFiles) => this.SecureFileService.UpdateSecureFiles(this.TfsRequestContext, this.ProjectId, secureFiles);

    [HttpPost]
    [ClientRequestContentMediaType("application/json")]
    public IList<SecureFile> QuerySecureFilesByProperties([FromBody] string condition, [ClientQueryParameter] string namePattern = null) => this.SecureFileService.QuerySecureFilesByProperties(this.TfsRequestContext, this.ProjectId, condition, namePattern);

    protected HttpResponseMessage CreateDownloadResponse(
      Guid secureFileId,
      string ticket = null,
      bool download = false,
      [FromUri(Name = "$format"), ClientIgnore] string format = null)
    {
      int num = (int) MediaTypeFormatUtility.GetPrioritizedAcceptHeaders(this.Request, new List<RequestMediaType>()
      {
        RequestMediaType.OctetStream,
        RequestMediaType.None
      }).First<RequestMediaType>();
      SecureFile secureFile = this.SecureFileService.GetSecureFile(this.TfsRequestContext, this.ProjectId, secureFileId);
      if (secureFile == null)
        throw new SecureFileNotFoundException(Microsoft.TeamFoundation.DistributedTask.Server.TaskResources.SecureFileNotFound((object) secureFileId));
      if (num != 4)
        return this.Request.CreateResponse<SecureFile>(HttpStatusCode.OK, secureFile);
      return this.CreateDownloadStreamResponse(secureFile, this.SecureFileService.DownloadSecureFile(this.TfsRequestContext, this.ProjectId, secureFileId, ticket) ?? throw new SecureFileNotFoundException(Microsoft.TeamFoundation.DistributedTask.Server.TaskResources.SecureFileNotFound((object) secureFileId)), download);
    }

    private HttpResponseMessage CreateDownloadStreamResponse(
      SecureFile secureFile,
      Stream secureContent,
      bool download = false)
    {
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StreamContent(secureContent);
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      if (download)
        response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(secureFile.Name);
      return response;
    }

    private List<Guid> ParseIdsAsGuidArray(string idString)
    {
      List<Guid> idsAsGuidArray = (List<Guid>) null;
      IList<string> stringList = ArtifactPropertyKinds.AsPropertyFilters(idString);
      if (stringList != null)
      {
        idsAsGuidArray = new List<Guid>(stringList.Count);
        List<string> values = new List<string>();
        foreach (string input in (IEnumerable<string>) stringList)
        {
          Guid result;
          if (Guid.TryParse(input, out result))
            idsAsGuidArray.Add(result);
          else
            values.Add(input);
        }
        if (values.Count > 0)
          throw new ArgumentException(string.Format("{0}, secureFileIds: {1}", (object) TFCommonResources.EntityModel_BadGuidFormat(), (object) string.Join(", ", (IEnumerable<string>) values)));
      }
      return idsAsGuidArray;
    }
  }
}
