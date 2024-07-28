// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsExperience.MonitoringAccountExperienceManager
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Configuration;
using Microsoft.Cloud.Metrics.Client.Utility;
using Microsoft.Online.Metrics.Serialization.MetricsExperience;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.MetricsExperience
{
  public sealed class MonitoringAccountExperienceManager : IMonitoringAccountExperienceManager
  {
    private readonly ConnectionInfo connectionInfo;
    private readonly HttpClient httpClient;
    private readonly string authTypeUrlPrefix;
    private readonly string subscribeUrlPrefix;
    private readonly string monitoringAccountUrlPrefix;
    private readonly string experienceUrlPrefix;
    private readonly string quotaCheckAccountUrlPrefix;
    private bool updatedExistingExperiences;
    private List<string> existingExperiences;
    private string experience;

    public MonitoringAccountExperienceManager(ConnectionInfo connectionInfo, HttpClient client = null)
    {
      this.connectionInfo = connectionInfo ?? throw new ArgumentNullException(nameof (connectionInfo));
      this.authTypeUrlPrefix = this.connectionInfo.GetAuthRelativeUrl("v1/metrics-experiences");
      this.subscribeUrlPrefix = this.authTypeUrlPrefix + "/subscribe/monitoringAccount";
      this.monitoringAccountUrlPrefix = this.authTypeUrlPrefix + "/monitoringAccount";
      this.experienceUrlPrefix = nameof (experience);
      this.quotaCheckAccountUrlPrefix = this.authTypeUrlPrefix + "quotaCheck/monitoringAccount";
      this.httpClient = client == null ? HttpClientHelper.CreateHttpClientWithAuthInfo(connectionInfo) : client;
      this.updatedExistingExperiences = false;
    }

    public async Task BuildExperienceAsync(
      IMonitoringAccount monitoringAccount,
      string experience,
      CancellationToken cancellationToken,
      bool skipLimitValidation = false,
      bool subscribe = false,
      Guid? traceId = null)
    {
      string query = "traceId=" + this.AddTracePrefix(traceId) + string.Format("&{0}={1}", (object) nameof (skipLimitValidation), (object) skipLimitValidation) + string.Format("&{0}={1}", (object) nameof (subscribe), (object) subscribe);
      Uri uri = await this.BuildUriForExperienceRequest(this.monitoringAccountUrlPrefix, monitoringAccount, experience, query, cancellationToken, traceId).ConfigureAwait(false);
      string str = await this.SendRequestAsync(monitoringAccount.Name, uri, cancellationToken, HttpMethod.Post).ConfigureAwait(false);
    }

    public async Task<ExperienceConfigurationState> RemoveExperienceAsync(
      IMonitoringAccount monitoringAccount,
      string experience,
      CancellationToken cancellationToken,
      bool shouldCommit = false,
      Guid? traceId = null)
    {
      if (string.IsNullOrWhiteSpace(traceId.ToString()))
        traceId = new Guid?(Guid.NewGuid());
      string modifiedTraceId = this.AddTracePrefix(traceId);
      if (shouldCommit)
        await this.UnsubscribeFromExperienceAsync(monitoringAccount, experience, traceId).ConfigureAwait(false);
      string query = "traceId=" + modifiedTraceId + string.Format("&{0}={1}", (object) nameof (shouldCommit), (object) shouldCommit);
      Uri uri = await this.BuildUriForExperienceRequest(this.monitoringAccountUrlPrefix, monitoringAccount, experience, query, cancellationToken, traceId).ConfigureAwait(false);
      ExperienceConfigurationState configurationState = JsonConvert.DeserializeObject<ExperienceConfigurationState>(await this.SendRequestAsync(monitoringAccount.Name, uri, cancellationToken, HttpMethod.Delete).ConfigureAwait(false));
      modifiedTraceId = (string) null;
      return configurationState;
    }

    public async Task<ExperienceConfigurationState> GetStateOfExperienceAsync(
      IMonitoringAccount monitoringAccount,
      string experience,
      CancellationToken cancellationToken,
      Guid? traceId = null)
    {
      string query = "traceId=" + this.AddTracePrefix(traceId);
      Uri uri = await this.BuildUriForExperienceRequest(this.monitoringAccountUrlPrefix, monitoringAccount, experience, query, cancellationToken, traceId).ConfigureAwait(false);
      return JsonConvert.DeserializeObject<ExperienceConfigurationState>(await this.SendRequestAsync(monitoringAccount.Name, uri, cancellationToken, HttpMethod.Get).ConfigureAwait(false));
    }

    public async Task<IEnumerable<string>> GetListOfExperienceNamesAsync(
      IMonitoringAccount monitoringAccount,
      CancellationToken cancellationToken,
      Guid? traceId = null)
    {
      IEnumerable<ExperienceInformationState> informationStates = await this.GetListOfExperiences(monitoringAccount, cancellationToken, traceId).ConfigureAwait(false);
      List<string> experienceNamesAsync = new List<string>();
      foreach (ExperienceInformationState informationState in informationStates)
        experienceNamesAsync.Add(informationState.Name);
      return (IEnumerable<string>) experienceNamesAsync;
    }

    public async Task<IEnumerable<ExperienceInformationState>> GetListOfExperiencesAsync(
      IMonitoringAccount monitoringAccount,
      CancellationToken cancellationToken,
      Guid? traceId = null)
    {
      return await this.GetListOfExperiences(monitoringAccount, cancellationToken, traceId).ConfigureAwait(false);
    }

    public async Task<bool> CheckExperienceTypeIsValid(
      IMonitoringAccount monitoringAccount,
      string experience,
      CancellationToken cancellationToken,
      Guid? traceId = null)
    {
      await this.PopulateExistingExperiencesList(monitoringAccount, cancellationToken, traceId).ConfigureAwait(false);
      return !string.IsNullOrWhiteSpace(experience) && this.IsValidExperience(experience);
    }

    public async Task SubscribeToExperienceAsync(
      IMonitoringAccount monitoringAccount,
      string experience,
      Guid? traceId = null)
    {
      string query = "traceId=" + this.AddTracePrefix(traceId);
      Uri uri = await this.BuildUriForExperienceRequest(this.subscribeUrlPrefix, monitoringAccount, experience, query, CancellationToken.None, traceId).ConfigureAwait(false);
      string str = await this.SendRequestAsync(monitoringAccount.Name, uri, HttpMethod.Put).ConfigureAwait(false);
    }

    public async Task UnsubscribeFromExperienceAsync(
      IMonitoringAccount monitoringAccount,
      string experience,
      Guid? traceId = null)
    {
      string query = "traceId=" + this.AddTracePrefix(traceId);
      Uri uri = await this.BuildUriForExperienceRequest(this.subscribeUrlPrefix, monitoringAccount, experience, query, CancellationToken.None, traceId).ConfigureAwait(false);
      string str = await this.SendRequestAsync(monitoringAccount.Name, uri, HttpMethod.Delete).ConfigureAwait(false);
    }

    public async Task<IEnumerable<string>> GetSubscribedExperiencesAsync(
      IMonitoringAccount monitoringAccount,
      Guid? traceId = null)
    {
      string query = "traceId=" + this.AddTracePrefix(traceId);
      Uri uri = await this.BuildUriForGetRequest(this.subscribeUrlPrefix, monitoringAccount, query).ConfigureAwait(false);
      return JsonConvert.DeserializeObject<IEnumerable<string>>(await this.SendRequestAsync(monitoringAccount.Name, uri, HttpMethod.Get).ConfigureAwait(false));
    }

    private async Task<IEnumerable<ExperienceInformationState>> GetListOfExperiences(
      IMonitoringAccount monitoringAccount,
      CancellationToken cancellationToken,
      Guid? traceId = null)
    {
      string query = "traceId=" + this.AddTracePrefix(traceId);
      Uri uri = await this.BuildUriForGetRequest(this.monitoringAccountUrlPrefix, monitoringAccount, query).ConfigureAwait(false);
      return JsonConvert.DeserializeObject<IEnumerable<ExperienceInformationState>>(await this.SendRequestAsync(monitoringAccount.Name, uri, cancellationToken, HttpMethod.Get).ConfigureAwait(false));
    }

    private async Task PopulateExistingExperiencesList(
      IMonitoringAccount monitoringAccount,
      CancellationToken cancellationToken,
      Guid? traceId = null)
    {
      if (this.updatedExistingExperiences)
        return;
      this.existingExperiences = (await this.GetListOfExperienceNamesAsync(monitoringAccount, cancellationToken, traceId).ConfigureAwait(false)).ToList<string>();
      this.updatedExistingExperiences = true;
    }

    private async Task<string> SendRequestAsync(
      string monitoringAccountName,
      Uri uri,
      HttpMethod method)
    {
      return await this.SendRequestAsync(monitoringAccountName, uri, CancellationToken.None, method).ConfigureAwait(false);
    }

    private async Task<string> SendRequestAsync(
      string monitoringAccountName,
      Uri uri,
      CancellationToken cancellationToken,
      HttpMethod method)
    {
      string str;
      try
      {
        str = await HttpClientHelper.GetJsonResponse(uri, method, this.httpClient, monitoringAccountName, this.authTypeUrlPrefix).ConfigureAwait(false);
      }
      catch (MetricsClientException ex)
      {
        if (ex.ResponseStatusCode.HasValue && ex.ResponseStatusCode.Value == HttpStatusCode.NotFound)
          throw new AccountNotFoundException(string.Format("Account [{0}] not found. TraceId: [{1}]", (object) monitoringAccountName, (object) ex.TraceId), (Exception) ex);
        if (ex.ResponseStatusCode.HasValue && ex.ResponseStatusCode.Value != HttpStatusCode.OK)
          throw new MetricsClientException(string.Format("Received status code {0} from server.", (object) ex.ResponseStatusCode.Value), (Exception) ex);
        throw;
      }
      return str;
    }

    private async Task AreValidExperienceRequestArgs(
      IMonitoringAccount monitoringAccount,
      string experience,
      CancellationToken cancellationToken,
      Guid? traceId = null)
    {
      if (monitoringAccount == null)
        throw new ArgumentNullException("monitoringAccount is invalid.");
      await this.PopulateExistingExperiencesList(monitoringAccount, cancellationToken, traceId).ConfigureAwait(false);
      if (string.IsNullOrWhiteSpace(experience) || !this.IsValidExperience(experience))
        throw new ArgumentException("experience is invalid.");
    }

    private bool IsValidExperience(string experience)
    {
      bool flag = false;
      StringComparer caseInsensitiveComparer = StringComparer.OrdinalIgnoreCase;
      int index = this.existingExperiences.FindIndex((Predicate<string>) (i => caseInsensitiveComparer.Equals(i, experience)));
      if (index >= 0)
      {
        this.experience = this.existingExperiences.ElementAt<string>(index);
        flag = true;
      }
      return flag;
    }

    private string AddTracePrefix(Guid? traceId = null) => !string.IsNullOrWhiteSpace(traceId.ToString()) ? string.Format("MetricsSDK-{0}", (object) traceId) : string.Format("MetricsSDK-{0}", (object) Guid.NewGuid());

    private async Task<Uri> BuildUriForExperienceRequest(
      string urlRequestPrefix,
      IMonitoringAccount monitoringAccount,
      string experience,
      string query,
      CancellationToken cancellationToken,
      Guid? traceId = null)
    {
      await this.AreValidExperienceRequestArgs(monitoringAccount, experience, cancellationToken, traceId).ConfigureAwait(false);
      string name = monitoringAccount.Name;
      string path = urlRequestPrefix + "/" + name + "/" + this.experienceUrlPrefix + "/" + this.experience;
      return await this.BuildUri(name, path, query).ConfigureAwait(false);
    }

    private async Task<Uri> BuildUriForGetRequest(
      string urlRequestPrefix,
      IMonitoringAccount monitoringAccount,
      string query)
    {
      string monitoringAccountName = monitoringAccount != null ? monitoringAccount.Name : throw new ArgumentNullException("monitoringAccount is invalid.");
      string path = urlRequestPrefix + "/" + monitoringAccountName;
      return await this.BuildUri(monitoringAccountName, path, query).ConfigureAwait(false);
    }

    private async Task<Uri> BuildUri(string monitoringAccountName, string path, string query)
    {
      Uri uri;
      try
      {
        uri = new UriBuilder(await this.connectionInfo.GetEndpointAsync(monitoringAccountName).ConfigureAwait(false))
        {
          Path = path,
          Query = query
        }.Uri;
      }
      catch (MetricsClientException ex)
      {
        if (ex.ResponseStatusCode.HasValue && ex.ResponseStatusCode.Value == HttpStatusCode.NotFound)
          throw new AccountNotFoundException(string.Format("Account [{0}] not found. TraceId: [{1}]", (object) monitoringAccountName, (object) ex.TraceId), (Exception) ex);
        throw;
      }
      return uri;
    }
  }
}
