// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.Machine
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class Machine : CatalogObject
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.Machine;
    private static readonly Type[] KnownChildTypes = new Type[3]
    {
      typeof (SqlAnalysisInstance),
      typeof (SqlReportingInstance),
      typeof (TeamWebApplication)
    };
    private ICollection<SqlAnalysisInstance> m_SqlAnalysisInstances;
    private ICollection<SqlReportingInstance> m_SqlReportingInstances;
    private ICollection<TeamWebApplication> m_WebApplications;

    protected override void Reset()
    {
      base.Reset();
      this.m_SqlAnalysisInstances = (ICollection<SqlAnalysisInstance>) null;
      this.m_SqlReportingInstances = (ICollection<SqlReportingInstance>) null;
      this.m_WebApplications = (ICollection<TeamWebApplication>) null;
    }

    public override void Preload(CatalogBulkData bulkData)
    {
      base.Preload(bulkData);
      this.m_SqlAnalysisInstances = this.PreloadChildren<SqlAnalysisInstance>(bulkData);
      this.m_SqlReportingInstances = this.PreloadChildren<SqlReportingInstance>(bulkData);
      this.m_WebApplications = this.PreloadChildren<TeamWebApplication>(bulkData);
    }

    public string MachineName
    {
      get => this.GetProperty<string>(nameof (MachineName));
      set => this.SetProperty<string>(nameof (MachineName), value);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ICollection<SqlAnalysisInstance> SqlAnalysisInstances => this.GetChildCollection<SqlAnalysisInstance>(ref this.m_SqlAnalysisInstances);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ICollection<SqlReportingInstance> SqlReportingInstances => this.GetChildCollection<SqlReportingInstance>(ref this.m_SqlReportingInstances);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ICollection<TeamWebApplication> WebApplications => this.GetChildCollection<TeamWebApplication>(ref this.m_WebApplications);

    public static Machine Register(InfrastructureRoot root, string machineName)
    {
      ArgumentUtility.CheckForNull<InfrastructureRoot>(root, nameof (root));
      ArgumentUtility.CheckStringForNullOrEmpty(machineName, nameof (machineName));
      Machine machine = root.Machines.FirstOrDefault<Machine>((Func<Machine, bool>) (m => VssStringComparer.Hostname.Equals(m.MachineName, machineName)));
      if (machine == null)
      {
        machine = root.CreateChild<Machine>(machineName);
        machine.MachineName = machineName;
      }
      return machine;
    }

    public override string ToString() => this.MachineName;

    public static class Fields
    {
      public const string MachineName = "MachineName";
    }
  }
}
