// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitImportRepositoryValidationsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [ClientInclude(RestClientLanguages.TypeScript | RestClientLanguages.TypeScriptWebPlatform)]
  public class GitImportRepositoryValidationsController : GitApiController
  {
    [HttpPost]
    [ClientLocationId("D8C00958-DEDD-491F-93E6-73F3C06F5BBA")]
    [ClientResponseType(typeof (ImportRepositoryValidation), null, null)]
    public HttpResponseMessage ValidateRemoteRepository(ImportRepositoryValidation remoteRepository)
    {
      ArgumentUtility.CheckForNull<ImportRepositoryValidation>(remoteRepository, nameof (remoteRepository), this.TfsRequestContext.ServiceName);
      GitImportGitSource gitSource = remoteRepository.GitSource;
      GitImportTfvcSource tfvcSource = remoteRepository.TfvcSource;
      if (gitSource == null && tfvcSource == null)
        throw new InvalidArgumentValueException(nameof (remoteRepository), Resources.Get("InvalidParameters"));
      if (gitSource != null && gitSource.Url == null)
        throw new InvalidArgumentValueException("gitSource", Resources.Get("InvalidParameters"));
      if (tfvcSource != null)
      {
        if (tfvcSource.Path == null || tfvcSource.ImportHistoryDurationInDays < 0)
          throw new InvalidArgumentValueException("tfvcSource", Resources.Get("InvalidParameters"));
        if (tfvcSource.ImportHistoryDurationInDays > 180)
          throw new InvalidArgumentValueException("tfvcSource", Resources.Format("ImportHistoryDurationTooLarge", (object) 180));
      }
      ClientTraceData clientTraceData = new ClientTraceData();
      string errorMessage;
      if (!GitImportValidationUtility.ValidateImportParams(this.TfsRequestContext, remoteRepository, this.TraceArea, clientTraceData, out errorMessage))
      {
        this.TfsRequestContext.GetService<ClientTraceService>().Publish(this.TfsRequestContext, "Microsoft.TeamFoundation.Sourcecontrol.WebServer", "RemoteRepositoryValidation", clientTraceData);
        if (!string.IsNullOrWhiteSpace(errorMessage))
          throw new RemoteRepositoryValidationFailed(errorMessage);
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      }
      ImportRepositoryValidation repositoryValidation = remoteRepository;
      remoteRepository.Username = (string) null;
      remoteRepository.Password = (string) null;
      return this.Request.CreateResponse<ImportRepositoryValidation>(HttpStatusCode.Created, repositoryValidation);
    }
  }
}
