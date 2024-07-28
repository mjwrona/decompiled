// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TeamFoundationAdministrationService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class TeamFoundationAdministrationService : IAdministrationService
  {
    private AdministrationWebService m_administrationWebService;

    internal TeamFoundationAdministrationService(TfsConfigurationServer tfsApplication) => this.m_administrationWebService = new AdministrationWebService(tfsApplication);

    public void CancelRequest(Guid hostId, long requestId, string reason) => this.m_administrationWebService.CancelRequest(hostId, requestId, reason);

    public TeamFoundationServiceHostActivity QueryActiveRequests(
      TfsConnection teamFoundationServer,
      bool includeDetails)
    {
      ArgumentUtility.CheckForNull<TfsConnection>(teamFoundationServer, nameof (teamFoundationServer));
      return this.QueryActiveRequests(teamFoundationServer.InstanceId, includeDetails);
    }

    public TeamFoundationServiceHostActivity QueryActiveRequests(Guid hostId, bool includeDetails)
    {
      TeamFoundationServiceHostActivity[] serviceHostActivityArray = this.m_administrationWebService.QueryActiveRequests((IEnumerable<Guid>) new Guid[1]
      {
        hostId
      }, (includeDetails ? 1 : 0) != 0);
      return serviceHostActivityArray.Length != 0 ? serviceHostActivityArray[0] : (TeamFoundationServiceHostActivity) null;
    }

    public ReadOnlyCollection<TeamFoundationServiceHostActivity> QueryActiveRequests(
      IEnumerable<Guid> hostIds,
      bool includeDetails)
    {
      TeamFoundationServiceHostActivity[] collection = this.m_administrationWebService.QueryActiveRequests(hostIds, includeDetails);
      List<TeamFoundationServiceHostActivity> serviceHostActivityList = new List<TeamFoundationServiceHostActivity>();
      if (hostIds != null && hostIds.Any<Guid>())
      {
        int index = 0;
        foreach (Guid hostId in hostIds)
        {
          TeamFoundationServiceHostActivity serviceHostActivity = (TeamFoundationServiceHostActivity) null;
          if (index < collection.Length && (collection[index] == null || collection[index].Id == hostId))
            serviceHostActivity = collection[index++];
          serviceHostActivityList.Add(serviceHostActivity);
        }
      }
      else
        serviceHostActivityList.AddRange((IEnumerable<TeamFoundationServiceHostActivity>) collection);
      return serviceHostActivityList.AsReadOnly();
    }
  }
}
