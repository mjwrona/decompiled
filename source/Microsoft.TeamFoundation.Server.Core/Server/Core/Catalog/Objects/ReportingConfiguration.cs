// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.ReportingConfiguration
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ReportingConfiguration : CatalogObject
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.ReportingConfiguration;

    public WarehouseDatabase ReportingWarehouse
    {
      get => this.GetDependency<WarehouseDatabase>(nameof (ReportingWarehouse));
      set => this.SetDependency<WarehouseDatabase>(nameof (ReportingWarehouse), value);
    }

    public SqlAnalysisDatabase ReportingCube
    {
      get => this.GetDependency<SqlAnalysisDatabase>(nameof (ReportingCube));
      set => this.SetDependency<SqlAnalysisDatabase>(nameof (ReportingCube), value);
    }

    public ReportingServer ReportServer
    {
      get => this.GetDependency<ReportingServer>(nameof (ReportServer));
      set => this.SetDependency<ReportingServer>(nameof (ReportServer), value);
    }

    public static class Fields
    {
      public const string ReportingWarehouse = "ReportingWarehouse";
      public const string ReportingCube = "ReportingCube";
      public const string ReportServer = "ReportServer";
    }
  }
}
