// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification.ContentValidationFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification
{
  public class ContentValidationFacade : IContentValidationFacade
  {
    private readonly IVssRequestContext requestContext;
    private readonly ITracerService tracerService;
    private const int MaxAllowedBase64ContentSizeInMB = 3;

    public ContentValidationFacade(IVssRequestContext requestContext, ITracerService tracerService)
    {
      this.requestContext = requestContext;
      this.tracerService = tracerService;
    }

    public async Task<NullResult> SubmitFilesAsync(
      FeedCore feed,
      IEnumerable<ContentValidationKey> toScan,
      string userId,
      string creatorIpAddress)
    {
      ContentValidationFacade sendInTheThisObject = this;
      NullResult nullResult;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (SubmitFilesAsync)))
      {
        try
        {
          IContentValidationService validationService = sendInTheThisObject.ContentValidationService;
          IVssRequestContext requestContext = sendInTheThisObject.requestContext;
          ProjectReference project = feed.Project;
          Guid projectId = (object) project != null ? project.Id : Guid.NewGuid();
          IEnumerable<ContentValidationKey> toScan1 = toScan;
          Microsoft.VisualStudio.Services.Identity.Identity contentCreatorIdentity = sendInTheThisObject.GetContentCreatorIdentity(userId);
          string creatorIpAddress1 = creatorIpAddress;
          await validationService.SubmitAsync(requestContext, projectId, toScan1, contentCreatorIdentity, creatorIpAddress1);
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          throw;
        }
        nullResult = await Task.FromResult<NullResult>((NullResult) null);
      }
      return nullResult;
    }

    public async Task<NullResult> ScanBase64ContentInStream(
      FeedCore feed,
      string userId,
      string creatorIpAddress,
      string content,
      string fileName)
    {
      ContentValidationFacade validationFacade = this;
      List<DataUriEmbeddedContent> list = ContentValidationUtil.ExtractBase64DataUriEmbeddedContent(content).ToList<DataUriEmbeddedContent>();
      // ISSUE: reference to a compiler-generated method
      list.ForEach(new Action<DataUriEmbeddedContent>(validationFacade.\u003CScanBase64ContentInStream\u003Eb__4_0));
      if (list.Count > 0)
      {
        IContentValidationService validationService = validationFacade.ContentValidationService;
        IVssRequestContext requestContext = validationFacade.requestContext;
        ProjectReference project = feed.Project;
        Guid projectId = (object) project != null ? project.Id : Guid.NewGuid();
        Microsoft.VisualStudio.Services.Identity.Identity contentCreatorIdentity = validationFacade.GetContentCreatorIdentity(userId);
        string creatorIpAddress1 = creatorIpAddress;
        string sourceFileName = fileName;
        List<DataUriEmbeddedContent> embeddedContents = list;
        await validationService.FireAndForgetSubmitEmbeddedContentAsync(requestContext, projectId, contentCreatorIdentity, creatorIpAddress1, sourceFileName, (IEnumerable<DataUriEmbeddedContent>) embeddedContents);
      }
      return await Task.FromResult<NullResult>((NullResult) null);
    }

    private void ValidateSize(string content, int maxSizeInMb)
    {
      if (Encoding.Unicode.GetByteCount(content) / 1048576 > maxSizeInMb)
        throw new Exception(string.Format("Base64 encoded img/video size greater than allowed limited: {0} MB", (object) maxSizeInMb));
    }

    private Microsoft.VisualStudio.Services.Identity.Identity GetContentCreatorIdentity(
      string userId)
    {
      using (ITracerBlock tracerBlock = this.tracerService.Enter((object) this, nameof (GetContentCreatorIdentity)))
      {
        try
        {
          return this.identityService.ReadIdentities(this.requestContext, (IList<Guid>) new List<Guid>()
          {
            new Guid(userId)
          }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        }
        catch (Exception ex)
        {
          tracerBlock.TraceException(new Exception("Exception resolving identity for commit.UserId: " + userId + ") Exception: " + ex.StackTrace));
          return (Microsoft.VisualStudio.Services.Identity.Identity) null;
        }
      }
    }

    private IContentValidationService ContentValidationService => this.requestContext.GetService<IContentValidationService>();

    private IdentityService identityService => this.requestContext.GetService<IdentityService>();
  }
}
