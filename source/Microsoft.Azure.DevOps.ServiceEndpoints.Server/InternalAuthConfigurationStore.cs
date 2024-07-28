// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.InternalAuthConfigurationStore
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  public class InternalAuthConfigurationStore : IVssFrameworkService
  {
    private string c_layer = nameof (InternalAuthConfigurationStore);
    private IList<AuthConfiguration> _allInternalAuthConfigurations = (IList<AuthConfiguration>) new List<AuthConfiguration>();
    private IList<AuthConfiguration> _shownInternalAuthConfigurationsAndBitBucket = (IList<AuthConfiguration>) new List<AuthConfiguration>();
    private IList<AuthConfiguration> _shownInternalAuthConfigurationsAndBitBucketBackup = (IList<AuthConfiguration>) new List<AuthConfiguration>();

    public InternalAuthConfigurationStore(IVssRequestContext requestContext)
    {
      try
      {
        this.LoadConfigurations(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(TraceConstants.InternalAuthConfigurationStore, "ServiceEndpoints", this.c_layer, ex);
      }
      finally
      {
        requestContext.TraceLeave(TraceConstants.InternalAuthConfigurationStore, "ServiceEndpoints", this.c_layer, "ServiceLeave. InternalAuthConfigurationLoadedCount= " + this._allInternalAuthConfigurations?.Count.ToString());
      }
    }

    public IList<AuthConfiguration> GetInternalAuthConfigurations() => this._allInternalAuthConfigurations;

    private IList<AuthConfiguration> ShownInternalAuthConfigurations(
      IVssRequestContext requestContext)
    {
      return !requestContext.IsFeatureEnabled(BitbucketFeatureFlags.BitbucketAzurePipelinesBackupOAuthClient) ? this._shownInternalAuthConfigurationsAndBitBucket : this._shownInternalAuthConfigurationsAndBitBucketBackup;
    }

    public List<AuthConfiguration> GetInternalAuthConfigurations(
      IVssRequestContext requestContext,
      string endpointType)
    {
      return this.ShownInternalAuthConfigurations(requestContext).Where<AuthConfiguration>((Func<AuthConfiguration, bool>) (item => item != null && item.EndpointType.Equals(endpointType, StringComparison.OrdinalIgnoreCase))).ToList<AuthConfiguration>();
    }

    public bool IsInternalAuthConfiguration(Guid configurationId) => this._allInternalAuthConfigurations.Any<AuthConfiguration>((Func<AuthConfiguration, bool>) (item => item.Id == configurationId));

    internal IDictionary<string, Parameter> ReadInternalAuthConfigurationSecrets(
      IVssRequestContext requestContext,
      Guid configurationId)
    {
      IDictionary<string, Parameter> collection = (IDictionary<string, Parameter>) new Dictionary<string, Parameter>();
      Guid guid = configurationId;
      if (guid == InternalAuthConfigurationConstants.AzurePipelinesOAuthAppId)
      {
        AuthConfiguration authConfiguration = AzurePipelinesOAuthApp.Initialize(requestContext, true);
        collection.Add("ClientSecret", (Parameter) authConfiguration.ClientSecret);
        collection.AddRange<KeyValuePair<string, Parameter>, IDictionary<string, Parameter>>((IEnumerable<KeyValuePair<string, Parameter>>) authConfiguration.Parameters);
        return collection;
      }
      if (guid == InternalAuthConfigurationConstants.AzureBoardsOAuthAppId)
      {
        AuthConfiguration authConfiguration = AzureBoardsOAuthApp.Initialize(requestContext, true);
        collection.Add("ClientSecret", (Parameter) authConfiguration.ClientSecret);
        collection.AddRange<KeyValuePair<string, Parameter>, IDictionary<string, Parameter>>((IEnumerable<KeyValuePair<string, Parameter>>) authConfiguration.Parameters);
        return collection;
      }
      if (guid == InternalAuthConfigurationConstants.AzurePipelinesMarketplaceAppId)
      {
        AuthConfiguration authConfiguration = AzurePipelinesMarketplaceApp.Initialize(requestContext, true);
        collection.Add("ClientSecret", (Parameter) authConfiguration.ClientSecret);
        collection.AddRange<KeyValuePair<string, Parameter>, IDictionary<string, Parameter>>((IEnumerable<KeyValuePair<string, Parameter>>) authConfiguration.Parameters);
        return collection;
      }
      if (guid == InternalAuthConfigurationConstants.BitbucketVstsAppId)
      {
        AuthConfiguration authConfiguration = BitbucketOAuthApp.Initialize(requestContext, true);
        collection.Add("ClientSecret", (Parameter) authConfiguration.ClientSecret);
        collection.AddRange<KeyValuePair<string, Parameter>, IDictionary<string, Parameter>>((IEnumerable<KeyValuePair<string, Parameter>>) authConfiguration.Parameters);
        return collection;
      }
      if (guid == InternalAuthConfigurationConstants.BitbucketAzurePipelinesOAuthAppId)
      {
        AuthConfiguration authConfiguration = BitbucketAzurePipelinesOAuthApp.Initialize(requestContext, true);
        collection.Add("ClientSecret", (Parameter) authConfiguration.ClientSecret);
        collection.AddRange<KeyValuePair<string, Parameter>, IDictionary<string, Parameter>>((IEnumerable<KeyValuePair<string, Parameter>>) authConfiguration.Parameters);
        return collection;
      }
      if (!(guid == InternalAuthConfigurationConstants.BitbucketAzurePipelinesBackupOAuthAppId))
        return (IDictionary<string, Parameter>) null;
      AuthConfiguration authConfiguration1 = BitbucketAzurePipelinesBackupOAuthApp.Initialize(requestContext, true);
      collection.Add("ClientSecret", (Parameter) authConfiguration1.ClientSecret);
      collection.AddRange<KeyValuePair<string, Parameter>, IDictionary<string, Parameter>>((IEnumerable<KeyValuePair<string, Parameter>>) authConfiguration1.Parameters);
      return collection;
    }

    public AuthConfiguration GetInternalAuthConfiguration(Guid configurationId) => this._allInternalAuthConfigurations.SingleOrDefault<AuthConfiguration>((Func<AuthConfiguration, bool>) (item => item.Id.Equals(configurationId)));

    public void LoadConfigurations(IVssRequestContext requestContext)
    {
      List<AuthConfiguration> values = new List<AuthConfiguration>();
      this._allInternalAuthConfigurations = (IList<AuthConfiguration>) new List<AuthConfiguration>();
      this._shownInternalAuthConfigurationsAndBitBucket = (IList<AuthConfiguration>) new List<AuthConfiguration>();
      this._shownInternalAuthConfigurationsAndBitBucketBackup = (IList<AuthConfiguration>) new List<AuthConfiguration>();
      AuthConfiguration authConfiguration1 = AzurePipelinesOAuthApp.Initialize(requestContext);
      if (authConfiguration1 != null)
        values.Add(authConfiguration1);
      AuthConfiguration authConfiguration2 = AzureBoardsOAuthApp.Initialize(requestContext);
      if (authConfiguration2 != null)
        values.Add(authConfiguration2);
      AuthConfiguration authConfiguration3 = AzurePipelinesMarketplaceApp.Initialize(requestContext);
      if (authConfiguration3 != null)
        values.Add(authConfiguration3);
      AuthConfiguration authConfiguration4 = AzureBoardsMarketplaceApp.Initialize(requestContext);
      if (authConfiguration4 != null)
        values.Add(authConfiguration4);
      this._allInternalAuthConfigurations.AddRange<AuthConfiguration, IList<AuthConfiguration>>((IEnumerable<AuthConfiguration>) values);
      AuthConfiguration authConfiguration5 = BitbucketOAuthApp.Initialize(requestContext);
      if (authConfiguration5 != null)
        this._allInternalAuthConfigurations.Add(authConfiguration5);
      AuthConfiguration authConfiguration6 = BitbucketAzurePipelinesOAuthApp.Initialize(requestContext);
      if (authConfiguration6 != null)
      {
        this._shownInternalAuthConfigurationsAndBitBucket.AddRange<AuthConfiguration, IList<AuthConfiguration>>((IEnumerable<AuthConfiguration>) values);
        this._shownInternalAuthConfigurationsAndBitBucket.Add(authConfiguration6);
        this._allInternalAuthConfigurations.Add(authConfiguration6);
      }
      AuthConfiguration authConfiguration7 = BitbucketAzurePipelinesBackupOAuthApp.Initialize(requestContext);
      if (authConfiguration7 == null)
        return;
      this._shownInternalAuthConfigurationsAndBitBucketBackup.AddRange<AuthConfiguration, IList<AuthConfiguration>>((IEnumerable<AuthConfiguration>) values);
      this._shownInternalAuthConfigurationsAndBitBucketBackup.Add(authConfiguration7);
      this._allInternalAuthConfigurations.Add(authConfiguration7);
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
