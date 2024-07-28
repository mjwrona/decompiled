// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Store.RequestStoreService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Store, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 88C5F689-5CBE-419A-B234-7228E63B94DF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Store.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Microsoft.VisualStudio.Services.Search.Server.Store
{
  public class RequestStoreService : IRequestStoreService
  {
    [StaticSafe]
    private static volatile RequestStoreService s_instance;
    [StaticSafe]
    private static object s_syncRoot = new object();
    private const string RequestIdFormat = "{0}@{1}@{2}@{3}";
    private const string OldRequestIdFormat = "{0}@{1}@{2}";

    private RequestStoreService()
    {
    }

    [StaticSafe]
    public static RequestStoreService Instance
    {
      get
      {
        if (RequestStoreService.s_instance == null)
        {
          lock (RequestStoreService.s_syncRoot)
          {
            if (RequestStoreService.s_instance == null)
              RequestStoreService.s_instance = new RequestStoreService();
          }
        }
        return RequestStoreService.s_instance;
      }
    }

    public string AddRequest<T>(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      T requestContent,
      string branchName = null)
      where T : class
    {
      string requestId = Guid.NewGuid().ToString();
      this.ValidateArguments(requestContext, projectName, repositoryName, requestId);
      ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
      using (MemoryStream ms = new MemoryStream())
      {
        new DataContractJsonSerializer(typeof (T)).WriteObject((Stream) ms, (object) requestContent);
        if (branchName != null)
          service.UploadFile(requestContext, ms, this.GetRequestId(projectName, repositoryName, branchName, requestId));
        else
          service.UploadFile(requestContext, ms, this.GetRequestId(projectName, repositoryName, requestId));
      }
      return requestId;
    }

    public T GetRequest<T>(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string requestId,
      string branchName = null)
      where T : class
    {
      this.ValidateArguments(requestContext, projectName, repositoryName, requestId);
      ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
      byte[] hashValue;
      long contentLength;
      CompressionType compressionType;
      Stream stream = branchName == null ? service.RetrieveNamedFile(requestContext, OwnerId.CodeSense, this.GetRequestId(projectName, repositoryName, requestId), false, out hashValue, out contentLength, out compressionType) : service.RetrieveNamedFile(requestContext, OwnerId.CodeSense, this.GetRequestId(projectName, repositoryName, branchName, requestId), false, out hashValue, out contentLength, out compressionType);
      return stream != null && stream.CanRead ? (T) new DataContractJsonSerializer(typeof (T)).ReadObject(stream) : default (T);
    }

    public bool RequestExists(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string requestId,
      string branchName = null)
    {
      this.ValidateArguments(requestContext, projectName, repositoryName, requestId);
      ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
      bool flag = service.FileExists(requestContext, this.GetRequestId(projectName, repositoryName, branchName, requestId));
      if (!flag)
        flag = service.FileExists(requestContext, this.GetRequestId(projectName, repositoryName, requestId));
      return flag;
    }

    public void DeleteRequest(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string requestId,
      string branchName = null)
    {
      this.ValidateArguments(requestContext, projectName, repositoryName, requestId);
      ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
      if (branchName != null)
      {
        string requestId1 = this.GetRequestId(projectName, repositoryName, branchName, requestId);
        if (!service.FileExists(requestContext, requestId1))
          return;
        service.DeleteFile(requestContext, requestId1);
      }
      else
      {
        string requestId2 = this.GetRequestId(projectName, repositoryName, requestId);
        if (!service.FileExists(requestContext, requestId2))
          return;
        service.DeleteFile(requestContext, requestId2);
      }
    }

    private string GetRequestId(
      string projectName,
      string repositoryName,
      string branchName,
      string requestId)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}@{1}@{2}@{3}", (object) projectName, (object) repositoryName, (object) branchName, (object) requestId);
    }

    private string GetRequestId(string projectName, string repositoryName, string requestId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}@{1}@{2}", (object) projectName, (object) repositoryName, (object) requestId);

    private void ValidateArguments(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string requestId)
    {
      this.ValidateArguments(requestContext, projectName, repositoryName);
      if (string.IsNullOrWhiteSpace(requestId))
        throw new ArgumentNullException(nameof (requestId));
    }

    private void ValidateArguments(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (string.IsNullOrWhiteSpace(projectName))
        throw new ArgumentNullException(nameof (projectName));
      if (string.IsNullOrWhiteSpace(repositoryName))
        throw new ArgumentNullException(nameof (repositoryName));
    }
  }
}
