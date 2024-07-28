// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.ReportingServer
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ReportingServer : CatalogObject
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.ReportingServer;

    public string DefaultItemPath
    {
      get => this.GetProperty<string>(nameof (DefaultItemPath));
      set => this.SetProperty<string>(nameof (DefaultItemPath), value);
    }

    public ServiceDefinition ReportsManagerService
    {
      get => this.GetServiceReference("ReportManagerUrl");
      set => this.SetServiceRefence("ReportManagerUrl", value);
    }

    public Uri ReportsManagerServiceLocation => this.LocationAsUrl(this.ReportsManagerService);

    public ServiceDefinition ReportServerService
    {
      get => this.GetServiceReference("ReportWebServiceUrl");
      set => this.SetServiceRefence("ReportWebServiceUrl", value);
    }

    public Uri ReportServerServiceLocation => this.LocationAsUrl(this.ReportServerService);

    public Machine Machine
    {
      get => this.GetDependency<Machine>(nameof (Machine));
      set => this.SetDependency<Machine>(nameof (Machine), value);
    }

    public void ResetServiceDefinitions(
      IVssRequestContext requestContext,
      string reportServer,
      string reportManager)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(reportServer, nameof (reportServer));
      ArgumentUtility.CheckStringForNullOrEmpty(reportManager, nameof (reportManager));
      ServiceDefinition reportServerService = this.ReportServerService;
      this.UpdateOrCreateReportingService(ref reportServerService, "ReportWebServiceUrl", reportServer, requestContext);
      this.ReportServerService = reportServerService;
      ServiceDefinition reportsManagerService = this.ReportsManagerService;
      this.UpdateOrCreateReportingService(ref reportsManagerService, "ReportManagerUrl", reportManager, requestContext);
      this.ReportsManagerService = reportsManagerService;
    }

    private void UpdateOrCreateReportingService(
      ref ServiceDefinition serviceDef,
      string serviceType,
      string location,
      IVssRequestContext requestContext)
    {
      IEnumerable<ServiceDefinition> source = this.Context.GetLocationService(requestContext).FindServiceDefinitionsByToolId(requestContext, "Reports").Where<ServiceDefinition>((Func<ServiceDefinition, bool>) (s => VssStringComparer.ServiceInterface.Equals(s.ServiceType, serviceType)));
      if (source.Any<ServiceDefinition>())
      {
        List<ServiceDefinition> list = source.ToList<ServiceDefinition>();
        serviceDef = list[0];
        list.Remove(serviceDef);
        if (list.Count > 0)
          this.Context.GetLocationService(requestContext).RemoveServiceDefinitions(requestContext, (IEnumerable<ServiceDefinition>) list);
      }
      if (serviceDef == null)
      {
        string displayName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1})", (object) FrameworkResources.ReportingServer(), (object) serviceType);
        serviceDef = new ServiceDefinition(serviceType, Guid.NewGuid(), displayName, (string) null, Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified, FrameworkResources.ReportingServerDescription(), "Reports");
      }
      else
        serviceDef.LocationMappings.Clear();
      serviceDef.AddLocationMapping(this.Context.PublicAccessMapping, location);
    }

    public ICollection<ReportingFolder> FindReportingFolders(string itemPath)
    {
      ICollection<ReportingFolder> reportingFolders = (ICollection<ReportingFolder>) new HashSet<ReportingFolder>();
      foreach (ReportingFolder queryDependent in (IEnumerable<ReportingFolder>) this.QueryDependents<ReportingFolder>(true, true))
      {
        string fullPath = queryDependent.FullPath;
        if (!string.IsNullOrEmpty(fullPath) && TFStringComparer.ReportItemPath.Equals(fullPath, itemPath))
          reportingFolders.Add(queryDependent);
      }
      return reportingFolders;
    }

    public static class Fields
    {
      public const string DefaultItemPath = "DefaultItemPath";
      public const string Machine = "Machine";
      public const string ReportManagerUrl = "ReportManagerUrl";
      public const string ReportWebServiceUrl = "ReportWebServiceUrl";
    }
  }
}
