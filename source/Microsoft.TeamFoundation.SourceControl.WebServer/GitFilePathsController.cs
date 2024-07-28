// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitFilePathsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ClientInclude(RestClientLanguages.CSharp | RestClientLanguages.TypeScript | RestClientLanguages.TypeScriptWebPlatform)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [ClientInternalUseOnly(false)]
  public class GitFilePathsController : GitApiController
  {
    [HttpGet]
    [ClientLocationId("E74B530C-EDFA-402B-88E2-8D04671134F7")]
    [ClientResponseType(typeof (GitFilePathsCollection), null, null)]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetFilePaths(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      [ClientQueryParameter] string scopePath = null,
      [ModelBinder(typeof (GitVersionDescriptorModelBinder))] GitVersionDescriptor versionDescriptor = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        GitFilePathsCollection filePathsCollection = GitItemUtility.RetrieveFilePaths(this.TfsRequestContext, this.Url, tfsGitRepository, scopePath, versionDescriptor);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("Action", nameof (GetFilePaths));
        properties.Add("RepositoryId", tfsGitRepository.Key.RepoId.ToString());
        properties.Add("FilePathsCount", (double) filePathsCollection.Paths.Count);
        properties.Add("ElapsedTimeMs", (double) stopwatch.ElapsedMilliseconds);
        this.TfsRequestContext.GetService<CustomerIntelligenceService>().Publish(this.TfsRequestContext, "Microsoft.TeamFoundation.Sourcecontrol.WebServer", "GitFilePaths", properties);
        ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
        filePathsCollection.SetSecuredObject(repositoryReadOnly);
        return this.Request.CreateResponse<GitFilePathsCollection>(HttpStatusCode.OK, filePathsCollection);
      }
    }
  }
}
