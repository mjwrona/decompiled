// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GvfsSizesHandler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  [GvfsPublicProjectRequestRestrictions]
  internal class GvfsSizesHandler : GvfsHttpHandler<ISet<Sha1Id>>
  {
    public GvfsSizesHandler()
    {
    }

    public GvfsSizesHandler(HttpContextBase context)
      : base(context)
    {
    }

    protected override string Layer => nameof (GvfsSizesHandler);

    protected override TimeSpan Timeout => TimeSpan.FromMinutes(5.0);

    internal override void ProcessPost(RepoNameKey nameKey, ISet<Sha1Id> requestedObjectIds)
    {
      using (ITfsGitRepository repositoryByName = this.RequestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryByName(this.RequestContext, nameKey.ProjectName, nameKey.RepositoryName))
      {
        if (requestedObjectIds == null || requestedObjectIds.Count == 0)
        {
          this.WriteBadRequestResponse();
        }
        else
        {
          HttpResponseBase response = this.HandlerHttpContext.Response;
          List<ObjectIdAndSize> objectSizes;
          try
          {
            objectSizes = repositoryByName.ObjectMetadata.GetObjectSizes((IEnumerable<Sha1Id>) requestedObjectIds, "Gvfs");
          }
          catch (GitObjectDoesNotExistException ex)
          {
            this.RequestContext.Status = (Exception) ex;
            this.WriteTextResponse(HttpStatusCode.NotFound, ex.Message);
            return;
          }
          response.StatusCode = 200;
          response.ContentType = MediaTypeFormatUtility.AcceptHeaderToString(RequestMediaType.Json);
          using (StreamWriter streamWriter = new StreamWriter(response.OutputStream, GitEncodingUtil.SafeUtf8NoBom, GitStreamUtil.OptimalBufferSize, true))
          {
            using (JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter) streamWriter))
            {
              JsonSerializer jsonSerializer = new JsonSerializer();
              jsonTextWriter.WriteStartArray();
              foreach (ObjectIdAndSize objectIdAndSize in objectSizes)
                jsonSerializer.Serialize((JsonWriter) jsonTextWriter, (object) objectIdAndSize);
              jsonTextWriter.WriteEndArray();
            }
          }
        }
      }
    }
  }
}
