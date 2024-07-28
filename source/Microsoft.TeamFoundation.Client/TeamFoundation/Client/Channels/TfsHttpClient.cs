// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Channels.TfsHttpClient
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Client;
using System;
using System.ComponentModel;
using System.Net;

namespace Microsoft.TeamFoundation.Client.Channels
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class TfsHttpClient : TfsHttpClientBase
  {
    protected TfsHttpClient(TfsConnection connection)
      : this(connection, (Uri) null)
    {
    }

    protected TfsHttpClient(TfsConnection connection, Uri serviceLocation)
      : base(serviceLocation, new Guid?(connection.SessionId), connection.Culture, connection.ClientCredentials, connection.IdentityToImpersonate)
    {
      this.Connection = connection;
      this.RemoteServerName = connection.Name;
    }

    public TfsConnection Connection { get; private set; }

    protected virtual Guid CollectionServiceIdentifier => Guid.Empty;

    protected virtual Guid ConfigurationServiceIdentifier => Guid.Empty;

    protected abstract string ServiceType { get; }

    public override object GetService(Type serviceType)
    {
      if (serviceType == typeof (TfsConnection))
        return (object) this.Connection;
      if (serviceType == typeof (TfsConfigurationServer) && typeof (TfsConfigurationServer).IsAssignableFrom(this.Connection.GetType()))
        return (object) this.Connection;
      return serviceType == typeof (TfsTeamProjectCollection) && typeof (TfsTeamProjectCollection).IsAssignableFrom(this.Connection.GetType()) ? (object) this.Connection : (object) null;
    }

    protected override ITfsRequestChannel OnCreateChannel(ITfsRequestChannel innerChannel) => this.Connection.ChannelFactory == null ? base.OnCreateChannel(innerChannel) : this.Connection.ChannelFactory.CreateChannel(innerChannel);

    protected override Uri GetServiceLocation() => this.Connection is TfsConfigurationServer ? this.GetServiceLocation(this.ServiceType, this.ConfigurationServiceIdentifier) : this.GetServiceLocation(this.ServiceType, this.CollectionServiceIdentifier);

    protected Uri GetServiceLocation(string serviceType, Guid serviceIdentifier)
    {
      Uri serviceLocation;
      if (!this.TryGetServiceLocation(serviceType, serviceIdentifier, out serviceLocation))
        throw new TeamFoundationServiceUnavailableException(TFCommonResources.UnableToRetrieveRegistrationInfo((object) serviceType), (Exception) null);
      return serviceLocation;
    }

    protected override void OnAfterReceiveReply(
      long requestId,
      string methodName,
      HttpWebResponse response)
    {
      TfsConnection.OnWebServiceCallEnd(this.Connection, requestId, methodName, this.ComponentName, response);
    }

    protected override void OnBeforeSendRequest(
      long requestId,
      string methodName,
      HttpWebRequest request)
    {
      TfsConnection.OnWebServiceCallBegin(this.Connection, requestId, methodName, this.ComponentName, request);
    }

    protected override void OnEndRequest(long requestId, Exception exception)
    {
      if (exception == null)
        this.Connection.ConnectivityFailureOnLastWebServiceCall = false;
      else
        this.Connection.ConnectivityFailureOnLastWebServiceCall = exception is TeamFoundationServiceUnavailableException;
    }

    protected bool TryGetServiceLocation(
      string serviceType,
      Guid serviceIdentifier,
      out Uri serviceLocation)
    {
      string uriString = this.Connection.GetService<ILocationService>().LocationForCurrentConnection(serviceType, serviceIdentifier);
      if (!string.IsNullOrEmpty(uriString))
      {
        serviceLocation = new Uri(uriString);
        return true;
      }
      serviceLocation = (Uri) null;
      return false;
    }
  }
}
