// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IVssServiceHost
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface IVssServiceHost : IVssServiceHostControl, IDisposable, IVssServiceHostProperties
  {
    IVssServiceHost ParentServiceHost { get; }

    IVssDeploymentServiceHost DeploymentServiceHost { get; }

    IVssServiceHost OrganizationServiceHost { get; }

    IVssServiceHost CollectionServiceHost { get; }

    bool Is(TeamFoundationHostType hostType);

    bool IsOnly(TeamFoundationHostType hostType);

    bool HasDatabaseAccess { get; }

    bool IsProduction { get; }

    int PartitionId { get; }

    bool SendWatsonReports { get; }

    ILockName CreateLockName(string lockName);

    ILockName CreateUniqueLockName(string lockName);

    IVssRequestContext CreateServicingContext();

    CultureInfo GetCulture(IVssRequestContext requestContext);

    void ReportException(
      string watsonReportingName,
      string eventCategory,
      Exception exception,
      string[] additionalInfo);

    event EventHandler<HostStatusChangedEventArgs> StatusChanged;
  }
}
