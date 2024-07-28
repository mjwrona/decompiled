// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.InfoRefsHandler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Protocol;
using Microsoft.TeamFoundation.Git.Server.Routing;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  [InfoRefsPublicProjectRequestRestrictions]
  [InfoRefsPublicProjectRequestRestrictions(AuthenticationMechanisms.Basic | AuthenticationMechanisms.Windows | AuthenticationMechanisms.Federated | AuthenticationMechanisms.OAuth, UserAgentFilterType.StartsWith, "git/")]
  [InfoRefsPublicProjectRequestRestrictions(AuthenticationMechanisms.Basic | AuthenticationMechanisms.Windows | AuthenticationMechanisms.Federated | AuthenticationMechanisms.OAuth, UserAgentFilterType.StartsWith, "git-lfs/")]
  [InfoRefsPublicProjectRequestRestrictions(AuthenticationMechanisms.Basic | AuthenticationMechanisms.Windows | AuthenticationMechanisms.Federated | AuthenticationMechanisms.OAuth, UserAgentFilterType.StartsWith, "xcode/")]
  [InfoRefsPublicProjectRequestRestrictions(AuthenticationMechanisms.Basic, UserAgentFilterType.StartsWith, "jgit/")]
  public class InfoRefsHandler : GitHttpHandler
  {
    private bool m_responseStarted;
    private const string c_plainTextContentType = "text/plain";
    private static readonly string s_serverVersion = "azure-repos/" + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
    internal const string ServiceQueryString = "service";
    internal const string UploadPackService = "git-upload-pack";
    private const string c_receivePackService = "git-receive-pack";
    private const string c_uploadPackAdvertisementContentType = "application/x-git-upload-pack-advertisement";
    private const string c_uploadPackServiceResponse = "# service=git-upload-pack";
    private const string c_receivePackAdvertisementContentType = "application/x-git-receive-pack-advertisement";
    private const string c_receivePackServiceResponse = "# service=git-receive-pack";

    public InfoRefsHandler()
    {
    }

    public InfoRefsHandler(HttpContextBase context)
      : base(context)
    {
    }

    protected override string Layer => nameof (InfoRefsHandler);

    internal override void Execute()
    {
      try
      {
        HttpRequestBase request = this.HandlerHttpContext.Request;
        string x = request.QueryString["service"];
        string routeValue1 = request.RequestContext.RouteData.GetRouteValue<string>("project");
        string routeValue2 = request.RequestContext.RouteData.GetRouteValue<string>("GitRepositoryName");
        MethodInformation methodInformation = new MethodInformation(this.Layer, MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter("RepositoryName", (object) routeValue2);
        if (x == null)
          this.EnterMethod(methodInformation);
        else if (StringComparer.Ordinal.Equals(x, "git-upload-pack"))
        {
          methodInformation.AddParameter("ServiceRequested", (object) "git-upload-pack");
          this.EnterMethod(methodInformation);
        }
        else if (StringComparer.Ordinal.Equals(x, "git-receive-pack"))
        {
          methodInformation.AddParameter("ServiceRequested", (object) "git-receive-pack");
          this.EnterMethod(methodInformation);
        }
        else
        {
          methodInformation.AddParameter("ServiceRequested", (object) "Unknown");
          this.EnterMethod(methodInformation);
        }
        using (ITfsGitRepository repositoryByName = this.RequestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryByName(this.RequestContext, routeValue1, routeValue2))
        {
          RequestedProtocolVersion protocolVersion = this.GetProtocolVersion();
          if (protocolVersion.RecognizedVersion != GitProtocolVersion.Unknown)
            this.RequestContext.TraceAlways(1013823, TraceLevel.Info, GitServerUtils.TraceArea, this.Layer, JsonConvert.SerializeObject((object) protocolVersion));
          this.m_responseStarted = true;
          if (protocolVersion.RecognizedVersion == GitProtocolVersion.Two && this.RequestContext.IsFeatureEnabled("Git.ProtocolVersionTwo"))
          {
            this.SendResponseHeader("application/x-git-compatibility-advertisement", "version 2", false);
            Stream outputStream = this.HandlerHttpContext.Response.OutputStream;
            ProtocolHelper.WriteLine(outputStream, FormattableString.Invariant(FormattableStringFactory.Create("agent={0}", (object) InfoRefsHandler.s_serverVersion)));
            ProtocolHelper.WriteLine(outputStream, "fetch");
            ProtocolHelper.WriteLine(outputStream, "ls-refs");
            ProtocolHelper.WriteLine(outputStream, (string) null);
          }
          else
          {
            RefAdvertiser refAdvertiser = new RefAdvertiser(this.RequestContext, repositoryByName, this.HandlerHttpContext.Response.OutputStream, GitServerUtils.IsRequestOptimized(this.HandlerHttpContext, repositoryByName.Settings));
            if (x == null)
            {
              this.HandlerHttpContext.Response.ContentType = "text/plain";
              refAdvertiser.AdvertiseHead();
            }
            else if (StringComparer.Ordinal.Equals(x, "git-upload-pack"))
            {
              this.SendResponseHeader("application/x-git-upload-pack-advertisement", "# service=git-upload-pack");
              refAdvertiser.AdvertiseUploadPack();
            }
            else
            {
              if (!StringComparer.Ordinal.Equals(x, "git-receive-pack"))
                return;
              this.SendResponseHeader("application/x-git-receive-pack-advertisement", "# service=git-receive-pack");
              refAdvertiser.AdvertiseReceivePack();
            }
          }
        }
      }
      catch (Exception ex)
      {
        if (this.HandleException(ex))
          return;
        throw;
      }
    }

    private bool HandleException(Exception ex) => this.HandleException(ex, this.m_responseStarted, (Action<string>) (message =>
    {
      ProtocolHelper.WriteLine(this.HandlerHttpContext.Response.OutputStream, (string) null);
      ProtocolHelper.WriteLine(this.HandlerHttpContext.Response.OutputStream, SecretUtility.ScrubSecrets(message));
    }));

    private void SendResponseHeader(
      string desiredContentType,
      string serviceResponse,
      bool flushPkt = true)
    {
      this.HandlerHttpContext.Response.ContentType = this.ShouldReturnPlainTextContentType() ? "text/plain" : desiredContentType;
      this.m_responseStarted = true;
      ProtocolHelper.WriteLine(this.HandlerHttpContext.Response.OutputStream, serviceResponse);
      if (!flushPkt)
        return;
      ProtocolHelper.WriteLine(this.HandlerHttpContext.Response.OutputStream, (string) null);
    }

    private bool ShouldReturnPlainTextContentType()
    {
      string header = this.HandlerHttpContext.Request.Headers["Referer"];
      string userAgent = this.HandlerHttpContext.Request.UserAgent;
      return !string.IsNullOrEmpty(header) && !string.IsNullOrEmpty(userAgent) && userAgent.StartsWith("Mozilla/", StringComparison.Ordinal);
    }
  }
}
