// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.UploadPackHandler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack;
using Microsoft.TeamFoundation.Git.Server.Routing;
using Microsoft.TeamFoundation.Git.Server.Telemetry;
using Microsoft.TeamFoundation.Git.Server.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  [GitPublicProjectRequestRestrictions]
  [GitPublicProjectRequestRestrictions(AuthenticationMechanisms.Basic | AuthenticationMechanisms.Windows | AuthenticationMechanisms.Federated | AuthenticationMechanisms.OAuth, UserAgentFilterType.StartsWith, "git/")]
  [GitPublicProjectRequestRestrictions(AuthenticationMechanisms.Basic | AuthenticationMechanisms.Windows | AuthenticationMechanisms.Federated | AuthenticationMechanisms.OAuth, UserAgentFilterType.StartsWith, "git-lfs/")]
  [GitPublicProjectRequestRestrictions(AuthenticationMechanisms.Basic | AuthenticationMechanisms.Windows | AuthenticationMechanisms.Federated | AuthenticationMechanisms.OAuth, UserAgentFilterType.StartsWith, "xcode/")]
  [GitPublicProjectRequestRestrictions(AuthenticationMechanisms.Basic, UserAgentFilterType.StartsWith, "jgit/")]
  public class UploadPackHandler : GitHttpHandler
  {
    private const string ResponseContentType = "application/x-git-upload-pack-result";
    private static readonly TimeSpan s_timeout = TimeSpan.FromHours(72.0);

    public UploadPackHandler()
    {
    }

    protected UploadPackHandler(HttpContextBase context)
      : base(context)
    {
    }

    protected override string Layer => nameof (UploadPackHandler);

    internal override void Execute()
    {
      HttpContextBase context = this.HandlerHttpContext;
      this.HandlerHttpContext.OverrideRequestTimeoutSeconds((int) UploadPackHandler.s_timeout.TotalSeconds);
      UploadPackParser uploadPackParser = (UploadPackParser) null;
      Stopwatch timer = Stopwatch.StartNew();
      ClientTraceData eventData = new ClientTraceData();
      context.Response.ContentType = "application/x-git-upload-pack-result";
      context.Response.BufferOutput = false;
      Stream inputStream = context.Request.InputStream;
      Stream outputStream = context.Response.OutputStream;
      TimeMeasuredStream inputStream1 = (TimeMeasuredStream) null;
      TimeMeasuredStream outputStream1 = (TimeMeasuredStream) null;
      if (this.RequestContext.IsFeatureEnabled("Git.EnableIOWaitMetric"))
      {
        inputStream1 = new TimeMeasuredStream(context.Request.InputStream);
        outputStream1 = new TimeMeasuredStream(context.Response.OutputStream);
        inputStream = (Stream) inputStream1;
        outputStream = (Stream) outputStream1;
      }
      try
      {
        string routeValue1 = context.Request.RequestContext.RouteData.GetRouteValue<string>("project");
        string routeValue2 = context.Request.RequestContext.RouteData.GetRouteValue<string>("GitRepositoryName");
        MethodInformation methodInformation = new MethodInformation(this.Layer, MethodType.Normal, EstimatedMethodCost.Moderate, UploadPackHandler.s_timeout);
        methodInformation.AddParameter("RepositoryName", (object) routeValue2);
        this.EnterMethod(methodInformation);
        ITeamFoundationGitRepositoryService service = this.RequestContext.GetService<ITeamFoundationGitRepositoryService>();
        using (ITfsGitRepository repository = service.FindRepositoryByName(this.RequestContext, routeValue1, routeValue2))
        {
          HashSet<Sha1Id> wants = new HashSet<Sha1Id>();
          eventData.AddGitOperationCtData(repository.Key, repository.Name, (Action) (() =>
          {
            string header = context.Request.Headers["Content-Encoding"];
            if (!string.IsNullOrEmpty(header) && StringComparer.OrdinalIgnoreCase.Equals(header, "gzip"))
              inputStream = (Stream) new GZipStream(inputStream, CompressionMode.Decompress, false);
            uploadPackParser = this.GetProtocolVersion().RecognizedVersion != GitProtocolVersion.Two || !this.RequestContext.IsFeatureEnabled("Git.ProtocolVersionTwo") ? (UploadPackParser) new UploadPackParserV1(this.RequestContext, repository, inputStream, outputStream, GitServerUtils.IsRequestOptimized(this.HandlerHttpContext, repository.Settings), true, eventData, wants) : (UploadPackParser) new UploadPackParserV2(this.RequestContext, repository, inputStream, outputStream, GitServerUtils.IsRequestOptimized(this.HandlerHttpContext, repository.Settings), eventData, wants);
            uploadPackParser.UploadPack();
          }), timer);
          WebhookUtil.PublishWebHook(this.RequestContext, repository, eventData, wants);
        }
      }
      catch (Exception ex)
      {
        if (this.HandleException(ex, uploadPackParser))
          return;
        throw;
      }
      finally
      {
        try
        {
          if (this.RequestContext.IsFeatureEnabled("Git.EnableIOWaitMetric"))
            eventData.AddIOWaitData(inputStream1, outputStream1);
          if (this.RequestContext.IsCanceled)
            this.RequestContext.TraceAlways(1013922, TraceLevel.Info, GitServerUtils.TraceArea, nameof (UploadPackHandler), JsonConvert.SerializeObject((object) eventData.GetData()));
          else
            eventData.PublishProtocolCtData(this.RequestContext);
        }
        catch (Exception ex)
        {
          this.RequestContext.TraceException(1013908, TraceLevel.Error, GitServerUtils.TraceArea, nameof (UploadPackHandler), ex);
        }
        byte[] buf = Array.Empty<byte>();
        GitStreamUtil.EnsureDrained(inputStream, buf);
        inputStream.Dispose();
      }
    }

    private bool HandleException(Exception ex, UploadPackParser packParser) => this.HandleException(ex, packParser != null && packParser.BodyStarted, (Action<string>) (message => packParser.WriteError(message)));
  }
}
