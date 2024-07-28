// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ReceivePackHandler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Protocol;
using Microsoft.TeamFoundation.Git.Server.Routing;
using Microsoft.TeamFoundation.Git.Server.Telemetry;
using Microsoft.TeamFoundation.Git.Server.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class ReceivePackHandler : GitHttpHandler
  {
    private const string ResponseContentType = "application/x-git-receive-pack-result";
    private static readonly TimeSpan s_timeout = TimeSpan.FromHours(72.0);

    public ReceivePackHandler()
    {
    }

    public ReceivePackHandler(HttpContextBase context)
      : base(context)
    {
    }

    protected override string Layer => nameof (ReceivePackHandler);

    internal override void Execute()
    {
      HttpContextBase handlerHttpContext = this.HandlerHttpContext;
      if (!handlerHttpContext.Request.HttpMethod.Equals(HttpMethod.Post.Method, StringComparison.OrdinalIgnoreCase))
      {
        handlerHttpContext.Response.StatusCode = 405;
        throw new InvalidOperationException("git-receive-pack requires the POST verb.");
      }
      handlerHttpContext.OverrideRequestTimeoutSeconds((int) ReceivePackHandler.s_timeout.TotalSeconds);
      handlerHttpContext.Response.ContentType = "application/x-git-receive-pack-result";
      handlerHttpContext.Response.BufferOutput = false;
      Stream inputStream = handlerHttpContext.Request.GetBufferlessInputStream(true);
      Stream outputStream = handlerHttpContext.Response.OutputStream;
      TimeMeasuredStream inputStream1 = (TimeMeasuredStream) null;
      TimeMeasuredStream outputStream1 = (TimeMeasuredStream) null;
      if (this.RequestContext.IsFeatureEnabled("Git.EnableIOWaitMetric"))
      {
        inputStream1 = new TimeMeasuredStream(handlerHttpContext.Request.GetBufferlessInputStream(true));
        outputStream1 = new TimeMeasuredStream(handlerHttpContext.Response.OutputStream);
        inputStream = (Stream) inputStream1;
        outputStream = (Stream) outputStream1;
      }
      ReceivePackParser receivePackParser = (ReceivePackParser) null;
      Stopwatch timer = Stopwatch.StartNew();
      try
      {
        MethodInformation methodInformation = new MethodInformation(this.Layer, MethodType.ReadWrite, EstimatedMethodCost.Moderate, ReceivePackHandler.s_timeout);
        string routeValue1 = handlerHttpContext.Request.RequestContext.RouteData.GetRouteValue<string>("GitRepositoryName");
        methodInformation.AddParameter("RepositoryName", (object) routeValue1);
        this.EnterMethod(methodInformation);
        string routeValue2 = handlerHttpContext.Request.RequestContext.RouteData.GetRouteValue<string>("project");
        ITeamFoundationGitRepositoryService service = this.RequestContext.GetService<ITeamFoundationGitRepositoryService>();
        using (ITfsGitRepository repository = service.FindRepositoryByName(this.RequestContext, routeValue2, routeValue1))
        {
          string contentEncoding = handlerHttpContext.Request.Headers["Content-Encoding"];
          if (repository.IsInMaintenance)
          {
            string maintenanceMessage = MaintenanceMessageUtils.GetSanitizedMaintenanceMessage(this.RequestContext);
            ProtocolHelper.WriteSideband(outputStream, SidebandChannel.Error, maintenanceMessage);
            throw new GitRepoInMaintenanceException(maintenanceMessage);
          }
          ClientTraceData eventData = new ClientTraceData();
          eventData.AddGitPushCtData(repository, (Action) (() =>
          {
            if (!string.IsNullOrEmpty(contentEncoding) && StringComparer.OrdinalIgnoreCase.Equals(contentEncoding, "gzip"))
              inputStream = (Stream) new GZipStream(inputStream, CompressionMode.Decompress, false);
            receivePackParser = new ReceivePackParser(this.RequestContext, repository, inputStream, outputStream, eventData, DefaultGitDependencyRoot.Instance, true);
            try
            {
              receivePackParser.ReceivePack();
            }
            catch (GitPackDeserializerException ex)
            {
              throw new GitObjectRejectedException(ex.Message);
            }
          }), timer);
          if (this.RequestContext.IsFeatureEnabled("Git.EnableIOWaitMetric"))
            eventData.AddIOWaitData(inputStream1, outputStream1);
          eventData.PublishProtocolCtData(this.RequestContext);
          eventData.PublishAppInsightsTelemetry(this.RequestContext);
          receivePackParser.CompletePushReport();
        }
      }
      catch (Exception ex)
      {
        if (this.HandleException(ex, receivePackParser))
          return;
        throw;
      }
      finally
      {
        byte[] buf = Array.Empty<byte>();
        GitStreamUtil.EnsureDrained(inputStream, buf);
        inputStream.Dispose();
      }
    }

    private bool HandleException(Exception ex, ReceivePackParser receivePackParser) => this.HandleException(ex, receivePackParser != null && receivePackParser.BodyStarted, (Action<string>) (message => receivePackParser.WriteError(message)));
  }
}
