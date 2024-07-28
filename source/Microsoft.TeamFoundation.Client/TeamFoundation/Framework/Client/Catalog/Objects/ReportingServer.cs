// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Catalog.Objects.ReportingServer
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Client.Catalog.Objects
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

    public static class Fields
    {
      public const string DefaultItemPath = "DefaultItemPath";
      public const string Machine = "Machine";
      public const string ReportManagerUrl = "ReportManagerUrl";
      public const string ReportWebServiceUrl = "ReportWebServiceUrl";
    }
  }
}
