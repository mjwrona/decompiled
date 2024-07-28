// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Utils.PublicSourceValidator
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server.Utils
{
  public class PublicSourceValidator : UpstreamSourceValidator
  {
    private static readonly IReadOnlyList<string> SupportedCustomUpstreamProtocols = (IReadOnlyList<string>) new string[1]
    {
      "npm"
    };

    internal PublicSourceValidator(IVssRequestContext requestContext, UpstreamSource source)
      : base(requestContext, source)
    {
    }

    public override string GetNormalizedLocation()
    {
      try
      {
        Uri uri = new Uri(this.Source.Location);
        return uri.IsWellFormedOriginalString() ? uri.AbsoluteUri : throw new UriFormatException();
      }
      catch (UriFormatException ex)
      {
        throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_UpstreamSourceLocationNotParseable((object) this.Source.Location));
      }
    }

    protected override async Task ValidateUpstreamLocationAsync()
    {
      PublicSourceValidator publicSourceValidator = this;
      publicSourceValidator.GetNormalizedLocation();
      WellKnownUpstreamSource knownSourceOrDefault = WellKnownSourceProvider.Instance.GetWellKnownSourceOrDefault(publicSourceValidator.Source.Location);
      if ((object) knownSourceOrDefault != null)
      {
        (UpstreamStatus Status, IEnumerable<UpstreamStatusDetail> StatusDetails) = knownSourceOrDefault.ValidateUpstreamSourceMatches(publicSourceValidator.Source);
        int num = (int) Status;
        IEnumerable<UpstreamStatusDetail> source = StatusDetails;
        if (num != 0)
          throw new InvalidUpstreamSourceException(string.Join("\n", source.Select<UpstreamStatusDetail, string>((Func<UpstreamStatusDetail, string>) (x => x.Reason))));
      }
      else
      {
        if (!publicSourceValidator.requestContext.IsFeatureEnabled("Packaging.Feed.CustomPublicUpstreams"))
          throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_UpstreamSourceNotAllowed());
        await publicSourceValidator.ValidateCustomLocationAsync();
      }
    }

    private async Task ValidateCustomLocationHelperAsync(string requestUrl)
    {
      PublicSourceValidator publicSourceValidator = this;
      if (!PublicSourceValidator.SupportedCustomUpstreamProtocols.Contains<string>(publicSourceValidator.Source.Protocol, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_UpstreamSourceNotAllowed());
      Uri requestUri = new Uri(requestUrl);
      HttpWebRequest request = (HttpWebRequest) null;
      try
      {
        request = WebRequest.CreateHttp(requestUri);
      }
      catch (NotSupportedException ex)
      {
        throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_UpstreamSourceLocationNotHttp((object) publicSourceValidator.Source.Location));
      }
      HttpWebResponse webResponse = (HttpWebResponse) null;
      Exception exception = (Exception) null;
      DateTime start = DateTime.UtcNow;
      DateTime utcNow1 = DateTime.UtcNow;
      try
      {
        webResponse = (HttpWebResponse) await request.GetResponseAsync();
      }
      catch (WebException ex)
      {
        exception = (Exception) ex;
        if (ex.Response is HttpWebResponse response && response.StatusCode == HttpStatusCode.ServiceUnavailable)
          throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_UpstreamSourceLocationUnavailable((object) publicSourceValidator.Source.Location));
        throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_UpstreamSourceLocationInvalid((object) publicSourceValidator.Source.Location));
      }
      finally
      {
        DateTime utcNow2 = DateTime.UtcNow;
        string afdRefInfo = string.Empty;
        int responseCode = -1;
        if (webResponse != null)
        {
          string[] values = webResponse.Headers?.GetValues("X-MSEdge-Ref");
          if (values != null && ((IEnumerable<string>) values).Any<string>())
            afdRefInfo = ((IEnumerable<string>) values).First<string>();
          responseCode = (int) webResponse.StatusCode;
          webResponse.Dispose();
        }
        DateTime dateTime = start;
        TeamFoundationTracingService.TraceHttpOutgoingRequest(start, (int) (utcNow2 - dateTime).TotalMilliseconds, "Feed.UpstreamSourceHelper", request.Method, request.Host, request.RequestUri.AbsolutePath, responseCode, exception?.ToString() ?? "", publicSourceValidator.requestContext.E2EId, afdRefInfo, "", Guid.Empty, "", "");
      }
      request = (HttpWebRequest) null;
      webResponse = (HttpWebResponse) null;
      exception = (Exception) null;
    }

    private async Task ValidateCustomLocationAsync()
    {
      PublicSourceValidator publicSourceValidator = this;
      string requestUrl = publicSourceValidator.Source.Location;
      try
      {
        await publicSourceValidator.ValidateCustomLocationHelperAsync(requestUrl);
      }
      catch (object ex)
      {
        string requestUrl1 = publicSourceValidator.Source.Location.TrimEnd('/');
        if (requestUrl1.Equals(requestUrl))
          throw;
        await publicSourceValidator.ValidateCustomLocationHelperAsync(requestUrl1);
      }
      requestUrl = (string) null;
    }
  }
}
