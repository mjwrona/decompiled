// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.NewDomainUrlMigrationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.NameResolution;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class NewDomainUrlMigrationService : IVssFrameworkService
  {
    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual bool UseDevOpsDomainUrls(IVssRequestContext requestContext, Guid hostId)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return false;
      if (requestContext.ServiceHost.InstanceId == hostId)
        return requestContext.GetService<ILocationService>().GetAccessMapping(requestContext, AccessMappingConstants.RootDomainMappingMoniker) == null;
      IHostUriData hostUriData = requestContext.GetService<IInternalUrlHostResolutionService>().ResolveUriData(requestContext, hostId);
      return hostUriData is DevOpsCollectionHostUriData || hostUriData is DevOpsOrganizationHostUriData;
    }

    public void ConfigureDevOpsDomainUrls(
      IVssRequestContext requestContext,
      bool enableDevOpsDomainUrls,
      bool withOrganization,
      ITFLogger logger = null)
    {
      requestContext.CheckHostedDeployment();
      requestContext.CheckProjectCollectionRequestContext();
      this.CheckPermissions(requestContext);
      logger = logger ?? (ITFLogger) new NullLogger();
      List<NameResolutionEntry> resolutionEntries = this.ComputeNameResolutionEntries(requestContext, enableDevOpsDomainUrls, logger);
      if (withOrganization)
        resolutionEntries.AddRange((IEnumerable<NameResolutionEntry>) this.ComputeNameResolutionEntries(requestContext.To(TeamFoundationHostType.Application), enableDevOpsDomainUrls, logger));
      logger.Info("Updating {0} resolution entries", (object) resolutionEntries.Count);
      if (resolutionEntries.Count <= 0)
        return;
      resolutionEntries.ForEach((Action<NameResolutionEntry>) (x => logger.Info("{0}", (object) x)));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<INameResolutionService>().SetEntries(vssRequestContext, (IEnumerable<NameResolutionEntry>) resolutionEntries);
    }

    public void ResetRoutingData(IVssRequestContext requestContext, ITFLogger logger = null)
    {
      requestContext.CheckHostedDeployment();
      requestContext.CheckProjectCollectionOrOrganizationRequestContext();
      logger = logger ?? (ITFLogger) new NullLogger();
      IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
      this.ResetRoutingData(deploymentContext, requestContext.ServiceHost.InstanceId, logger);
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        this.ResetRoutingData(deploymentContext, requestContext.To(TeamFoundationHostType.Application).ServiceHost.InstanceId, logger);
      this.FlushLocationCache(requestContext, logger);
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      this.FlushLocationCache(requestContext.To(TeamFoundationHostType.Application), logger);
    }

    private void ResetRoutingData(
      IVssRequestContext deploymentContext,
      Guid hostId,
      ITFLogger logger)
    {
      logger.Info(string.Format("Reseting routing data for {0}", (object) hostId));
      IList<NameResolutionEntry> entriesForHost = deploymentContext.GetService<UrlHostResolutionService>().ComputeEntriesForHost(deploymentContext, (HostProperties) (deploymentContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(deploymentContext, hostId) ?? throw new HostDoesNotExistException(hostId)));
      if (entriesForHost.Count <= 0)
        return;
      entriesForHost.ForEach<NameResolutionEntry>((Action<NameResolutionEntry>) (x => logger.Info("Saving {0}", (object) x)));
      deploymentContext.GetService<NameResolutionStore>().SetEntries(deploymentContext, (IEnumerable<NameResolutionEntry>) entriesForHost, true);
    }

    private void FlushLocationCache(IVssRequestContext requestContext, ITFLogger logger)
    {
      logger.Info(string.Format("Flushing location cache for {0}", (object) requestContext.ServiceHost.InstanceId));
      requestContext.GetService<IInternalLocationService>().OnLocationDataChanged(requestContext, LocationDataKind.All);
    }

    private List<NameResolutionEntry> ComputeNameResolutionEntries(
      IVssRequestContext requestContext,
      bool enableDevOpsDomainUrls,
      ITFLogger logger)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IList<NameResolutionEntry> nameResolutionEntryList = vssRequestContext.GetService<IInternalNameResolutionService>().QueryEntriesForValue(vssRequestContext, requestContext.ServiceHost.InstanceId, QueryOptions.None);
      if (nameResolutionEntryList == null || nameResolutionEntryList.Count == 0)
        throw new InvalidOperationException(string.Format("No name resolution entries found for host {0}", (object) requestContext.ServiceHost.InstanceId));
      logger.Info("Found {0} name resolution entries for host {1}", (object) nameResolutionEntryList.Count, (object) requestContext.ServiceHost.InstanceId);
      nameResolutionEntryList.ForEach<NameResolutionEntry>((Action<NameResolutionEntry>) (x => logger.Info("{0}", (object) x)));
      List<NameResolutionEntry> resolutionEntries = new List<NameResolutionEntry>();
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
      {
        IList<NameResolutionEntry> list = (IList<NameResolutionEntry>) nameResolutionEntryList.Where<NameResolutionEntry>((Func<NameResolutionEntry, bool>) (x => x.IsPrimary)).ToList<NameResolutionEntry>();
        logger.Info("Processing namespace Organization");
        NameResolutionEntry nameResolutionEntry = list.Single<NameResolutionEntry>((Func<NameResolutionEntry, bool>) (x => x.Namespace == "Organization")).Clone();
        if (nameResolutionEntry.IsEnabled != enableDevOpsDomainUrls)
        {
          nameResolutionEntry.IsEnabled = enableDevOpsDomainUrls;
          resolutionEntries.Add(nameResolutionEntry);
        }
      }
      else
      {
        IList<NameResolutionEntry> list = (IList<NameResolutionEntry>) nameResolutionEntryList.Where<NameResolutionEntry>((Func<NameResolutionEntry, bool>) (x => x.IsEnabled)).ToList<NameResolutionEntry>();
        foreach (CollectionNamespaceReservation namespaceReservation in (IEnumerable<CollectionNamespaceReservation>) new NameAvailabilityHelper().GetCollectionNamespaceReservations(enableDevOpsDomainUrls))
        {
          CollectionNamespaceReservation reservation = namespaceReservation;
          logger.Info(string.Format("Processing namespace {0}", (object) reservation));
          NameResolutionEntry nameResolutionEntry = list.Single<NameResolutionEntry>((Func<NameResolutionEntry, bool>) (x => x.Namespace == reservation.Namespace)).Clone();
          if (nameResolutionEntry.IsPrimary != reservation.IsPrimary)
          {
            nameResolutionEntry.IsPrimary = reservation.IsPrimary;
            resolutionEntries.Add(nameResolutionEntry);
          }
        }
      }
      resolutionEntries.ForEach((Action<NameResolutionEntry>) (x => x.TTL = new int?()));
      return resolutionEntries;
    }

    private void CheckPermissions(IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      (vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, OrganizationSecurity.NamespaceId) ?? throw new ConfigurationErrorsException(string.Format("Could not find security namespace {0}", (object) OrganizationSecurity.NamespaceId))).CheckPermission(vssRequestContext, OrganizationSecurity.GenerateCollectionToken(new Guid?(requestContext.ServiceHost.InstanceId)), 4, false);
    }
  }
}
