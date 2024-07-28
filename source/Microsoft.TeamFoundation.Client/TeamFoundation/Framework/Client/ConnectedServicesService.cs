// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ConnectedServicesService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class ConnectedServicesService : ITfsTeamProjectCollectionObject
  {
    private ConnectedServicesWebService m_connectedServicesService;
    private TfsTeamProjectCollection m_tfs;

    private ConnectedServicesService()
    {
    }

    void ITfsTeamProjectCollectionObject.Initialize(TfsTeamProjectCollection server) => this.m_tfs = server;

    public ConnectedServiceMetadata CreateConnectedService(
      string name,
      string teamProject,
      ConnectedServiceKind kind,
      string friendlyName,
      string description,
      Uri serviceUri,
      Uri endpoint,
      string credentialsXml)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      ArgumentUtility.CheckStringForNullOrEmpty(teamProject, nameof (teamProject));
      ArgumentUtility.CheckStringForNullOrEmpty(endpoint.ToString(), nameof (endpoint));
      ArgumentUtility.CheckStringForNullOrEmpty(credentialsXml, "CredentialsXml");
      return this.connectedServices.CreateConnectedService(new ConnectedServiceCreationData(name, teamProject, kind, friendlyName, description, serviceUri, endpoint, credentialsXml));
    }

    public List<ConnectedServiceMetadata> QueryConnectedServices(string teamProject) => new List<ConnectedServiceMetadata>((IEnumerable<ConnectedServiceMetadata>) this.connectedServices.QueryConnectedServices(teamProject));

    public ConnectedService GetConnectedService(string name, string teamProject)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      ArgumentUtility.CheckStringForNullOrEmpty(teamProject, "projectName");
      return this.connectedServices.GetConnectedService(name, teamProject);
    }

    public bool DoesConnectedServiceExist(string name, string teamProject)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      ArgumentUtility.CheckStringForNullOrEmpty(teamProject, nameof (teamProject));
      return this.connectedServices.DoesConnectedServiceExist(name, teamProject);
    }

    public void DeleteConnectedService(string name, string teamProject)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      ArgumentUtility.CheckStringForNullOrEmpty(teamProject, nameof (teamProject));
      this.connectedServices.DeleteConnectedService(name, teamProject);
    }

    private ConnectedServicesWebService connectedServices
    {
      get
      {
        if (this.m_connectedServicesService == null)
          this.m_connectedServicesService = new ConnectedServicesWebService(this.m_tfs);
        return this.m_connectedServicesService;
      }
    }
  }
}
