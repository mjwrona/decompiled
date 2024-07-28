// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping.GitHubInstallationIdRouter
// Assembly: Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D6C9F672-10C9-4D5D-90D5-E07E56C1D4C0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Web;

namespace Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping
{
  public class GitHubInstallationIdRouter : IHostIdMappingRouter
  {
    private const int c_maxPostRequestSize = 26214400;

    protected virtual string Layer => nameof (GitHubInstallationIdRouter);

    protected virtual string Area => TracingPoints.ExternalServiceEventUrlHostRoutingArea;

    protected virtual string QualifierName => "full_name";

    public virtual bool OverrideOnDeletedOrganization(IVssRequestContext requestContext) => false;

    public bool TryExtractMappingData(
      IVssRequestContext requestContext,
      HttpRequest request,
      out HostIdMappingData mappingData)
    {
      string str = request.ReadBodyAsString(26214400);
      if (this.IsHostRoutingRequired(request, str))
        return this.TryExtractMappingData(requestContext, str, out mappingData);
      requestContext.Trace(TracingPoints.EventsRouting.HostRoutingNotRequired, TraceLevel.Info, this.Area, this.Layer, "TryExtractMappingData - github");
      mappingData = (HostIdMappingData) null;
      return false;
    }

    public bool TryExtractMappingData(
      IVssRequestContext requestContext,
      string requestBodyString,
      out HostIdMappingData mappingData)
    {
      try
      {
        mappingData = (HostIdMappingData) null;
        JObject httpRequestBody = JsonConvert.DeserializeObject<JObject>(requestBodyString ?? string.Empty);
        if (httpRequestBody == null)
        {
          requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Error, this.Area, this.Layer, "Unable to deserialize payload.");
          return false;
        }
        JToken jtoken = httpRequestBody["installation"];
        if (jtoken == null)
        {
          requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Error, this.Area, this.Layer, "Installation object not found in payload.");
          return false;
        }
        string str = jtoken[(object) "id"]?.ToString();
        if (string.IsNullOrEmpty(str))
        {
          requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Error, this.Area, this.Layer, "Id token is missing in installation object in payload.");
          return false;
        }
        string mappingDataQualifier = this.GetMappingDataQualifier(httpRequestBody);
        mappingData = new HostIdMappingData()
        {
          PropertyName = GitHubConstants.InstallationId,
          Id = str.Trim(),
          Qualifier = mappingDataQualifier
        };
        requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Verbose, this.Area, this.Layer, string.Format("MappingData: {0}", (object) mappingData));
        return long.TryParse(mappingData.Id, out long _);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(TracingPoints.EventsRouting.Mapping, this.Area, this.Layer, ex);
        mappingData = (HostIdMappingData) null;
        return false;
      }
    }

    private string GetMappingDataQualifier(JObject httpRequestBody)
    {
      JToken jtoken1 = httpRequestBody["repository"];
      if (jtoken1 == null || jtoken1.ToString() == string.Empty)
        jtoken1 = httpRequestBody["repo"];
      if (jtoken1 == null)
        return string.Empty;
      JToken jtoken2 = jtoken1[(object) this.QualifierName];
      return jtoken2 == null ? string.Empty : jtoken2.ToString().Trim();
    }

    public HostIdMappingData GetMappingData(
      IVssRequestContext requestContext,
      string installationId,
      string qualifier)
    {
      return new HostIdMappingData()
      {
        PropertyName = GitHubConstants.InstallationId,
        Id = installationId,
        Qualifier = qualifier
      };
    }

    public void AddHostIdMapping(
      IVssRequestContext requestContext,
      string providerId,
      string installationId,
      string qualifier,
      bool overrideExisting = false)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(installationId, nameof (installationId));
      HostIdMappingData mappingData = new HostIdMappingData()
      {
        PropertyName = GitHubConstants.InstallationId,
        Id = installationId,
        Qualifier = qualifier
      };
      requestContext.GetService<IHostIdMappingService>().AddHostIdMapping(requestContext, providerId, mappingData, requestContext.ServiceHost.InstanceId, overrideExisting);
    }

    public void RemoveHostIdMapping(
      IVssRequestContext requestContext,
      string providerId,
      string installationId,
      string qualifier)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(installationId, nameof (installationId));
      HostIdMappingData mappingData = new HostIdMappingData()
      {
        PropertyName = GitHubConstants.InstallationId,
        Id = installationId,
        Qualifier = qualifier
      };
      if (string.IsNullOrEmpty(qualifier))
        requestContext.GetService<IHostIdMappingService>().RemoveHostIdMappings(requestContext, providerId, mappingData);
      else
        requestContext.GetService<IHostIdMappingService>().RemoveHostIdMapping(requestContext, providerId, mappingData);
    }

    private bool IsHostRoutingRequired(HttpRequest request, string requestBody)
    {
      if (string.Equals(request.GetSimpleHeaderValue("X-GitHub-Event"), "installation", StringComparison.OrdinalIgnoreCase))
      {
        string fragmentExcludingTokens = JsonExtractor.GetFragmentExcludingTokens(requestBody, "\"action\":", ",");
        string a;
        if (fragmentExcludingTokens == null)
        {
          a = (string) null;
        }
        else
        {
          string str = fragmentExcludingTokens.Trim();
          if (str == null)
            a = (string) null;
          else
            a = str.Trim('"', '\'');
        }
        if (string.Equals(a, "created", StringComparison.OrdinalIgnoreCase))
          return false;
      }
      return true;
    }
  }
}
