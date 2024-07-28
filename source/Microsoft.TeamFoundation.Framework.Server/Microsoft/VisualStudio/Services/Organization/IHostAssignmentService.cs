// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.IHostAssignmentService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Organization
{
  [DefaultServiceImplementation("Microsoft.VisualStudio.Services.Organization.HostAssignmentService, Microsoft.VisualStudio.Services.Organization")]
  public interface IHostAssignmentService : IVssFrameworkService
  {
    IDictionary<Guid, int> GetCountOfAssignableHostsPerPool(
      IVssRequestContext context,
      int timeLimitInMintutes = 60);

    IDictionary<AssignmentStatus, int> GetCountOfHostsPerAssignmentStatus(
      IVssRequestContext context,
      DateTime? startTime = null,
      DateTime? endTime = null);

    void AddHostCreationInfo(
      IVssRequestContext context,
      Guid hostId,
      Guid poolId,
      HostCreationType creationType);

    void UpdateHostAssignmentStatus(
      IVssRequestContext context,
      Guid hostId,
      AssignmentStatus status);

    HostCreationInfo GetHostCreationInfo(IVssRequestContext context, Guid hostId);

    Guid ReserveNextAssignableHost(IVssRequestContext context, Guid poolId);

    IList<HostCreationInfo> QueryHostCreationInfos(
      IVssRequestContext context,
      HostCreationType hostCreationType,
      AssignmentStatus assignmentStatus);

    void DeleteHostCreationInfo(IVssRequestContext context, Guid hostId);

    void ImportHostCreationInfo(IVssRequestContext context, HostCreationInfo hostCreationInfo);
  }
}
