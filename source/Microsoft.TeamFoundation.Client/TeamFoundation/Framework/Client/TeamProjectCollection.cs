// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TeamProjectCollection
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public class TeamProjectCollection
  {
    private TeamProjectCollectionWebService m_service;
    private TeamProjectCollectionProperties m_properties;
    private static readonly int s_jobPollingInterval = 1000;

    internal TeamProjectCollection(
      TeamProjectCollectionWebService service,
      TeamProjectCollectionProperties properties)
    {
      this.m_service = service;
      this.m_properties = properties;
    }

    public Guid Id => this.m_properties.Id;

    public string Name
    {
      get => this.m_properties.Name;
      set => this.m_properties.Name = value;
    }

    public string Description
    {
      get => this.m_properties.Description;
      set => this.m_properties.Description = value;
    }

    public bool IsDefault
    {
      get => this.m_properties.IsDefault;
      set => this.m_properties.IsDefault = value;
    }

    public bool Registered
    {
      get => this.m_properties.Registered;
      set => this.m_properties.Registered = false;
    }

    public string VirtualDirectory
    {
      get => this.m_properties.VirtualDirectory;
      set => this.m_properties.VirtualDirectory = value;
    }

    public TeamFoundationServiceHostStatus State => this.m_properties.State;

    public bool IsBeingServiced
    {
      get
      {
        foreach (ServicingJobDetail servicingDetail in this.m_properties.ServicingDetails)
        {
          if (servicingDetail.JobStatus == ServicingJobStatus.Queued || servicingDetail.JobStatus == ServicingJobStatus.Running)
            return true;
        }
        return false;
      }
    }

    public ServicingJobDetail[] ServicingDetails => this.m_properties.ServicingDetails;

    public void Refresh() => this.m_properties = this.m_service.GetCollectionProperties((IEnumerable<Guid>) new Guid[1]
    {
      this.m_properties.Id
    }, ServiceHostFilterFlags.IncludeAllServicingDetails)[0] ?? throw new CollectionDoesNotExistException(TFCommonResources.CollectionDoesNotExist((object) this.m_properties.Id));

    public void Save()
    {
      if (!this.m_properties.Registered)
        throw new InvalidOperationException(ClientResources.UnregisteredCollectionInvalidMethodError());
      this.WaitForServicingToComplete(this.m_service.QueueUpdateCollection(this.m_properties));
    }

    public void Delete()
    {
      if (!this.m_properties.Registered)
        throw new InvalidOperationException(ClientResources.UnregisteredCollectionInvalidMethodError());
      this.WaitForServicingToComplete(this.m_service.QueueDeleteCollection(this.m_properties.Id));
      this.m_properties.Registered = false;
    }

    private void WaitForServicingToComplete(ServicingJobDetail queuedDeleteDetails)
    {
      ServicingJobDetail servicingJobDetail = (ServicingJobDetail) null;
      do
      {
        Thread.Sleep(TeamProjectCollection.s_jobPollingInterval);
        this.Refresh();
        foreach (ServicingJobDetail servicingDetail in this.ServicingDetails)
        {
          if (servicingDetail.JobId == queuedDeleteDetails.JobId)
          {
            servicingJobDetail = servicingDetail;
            break;
          }
        }
      }
      while (servicingJobDetail != null && (servicingJobDetail.JobStatus == ServicingJobStatus.Queued || servicingJobDetail.JobStatus == ServicingJobStatus.Running));
      if (servicingJobDetail.Result == ServicingJobResult.Failed || servicingJobDetail.JobStatus == ServicingJobStatus.Failed)
        throw new CollectionServicingJobDidNotSucceedException(ClientResources.CollectionServicingJobDidNotSucceed());
    }
  }
}
