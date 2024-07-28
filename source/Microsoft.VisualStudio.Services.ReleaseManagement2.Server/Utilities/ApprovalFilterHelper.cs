// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ApprovalFilterHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public class ApprovalFilterHelper
  {
    private readonly Func<IVssRequestContext, Guid, IEnumerable<Guid>, IEnumerable<Guid>> getMatchingGroupIds;
    private IVssRequestContext requestContext;

    public ApprovalFilterHelper(IVssRequestContext requestContext)
      : this(requestContext, ApprovalFilterHelper.\u003C\u003EO.\u003C0\u003E__GetMatchingGroupIds ?? (ApprovalFilterHelper.\u003C\u003EO.\u003C0\u003E__GetMatchingGroupIds = new Func<IVssRequestContext, Guid, IEnumerable<Guid>, IEnumerable<Guid>>(ApprovalFilterHelper.GetMatchingGroupIds)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is required only for testablity.")]
    protected ApprovalFilterHelper(
      IVssRequestContext requestContext,
      Func<IVssRequestContext, Guid, IEnumerable<Guid>, IEnumerable<Guid>> getMatchingGroupIds)
    {
      this.requestContext = requestContext;
      this.getMatchingGroupIds = getMatchingGroupIds;
    }

    public IEnumerable<ReleaseEnvironmentStep> ApplyFiltersPostSql(
      IEnumerable<ReleaseEnvironmentStep> approvals,
      ApprovalQueryParameters approvalQueryParameters)
    {
      if (approvalQueryParameters == null)
        throw new ArgumentNullException(nameof (approvalQueryParameters));
      List<ReleaseEnvironmentStep> source = approvalQueryParameters.QueryOrder != ReleaseQueryOrder.IdDescending ? approvals.OrderBy<ReleaseEnvironmentStep, int>((Func<ReleaseEnvironmentStep, int>) (a => a.Id)).ToList<ReleaseEnvironmentStep>() : approvals.OrderByDescending<ReleaseEnvironmentStep, int>((Func<ReleaseEnvironmentStep, int>) (a => a.Id)).ToList<ReleaseEnvironmentStep>();
      if (!approvalQueryParameters.IncludeMyGroupApprovals)
        return (IEnumerable<ReleaseEnvironmentStep>) source;
      Guid? approverId = approvalQueryParameters.ApproverIdFilter;
      if (approverId.HasValue)
      {
        Guid? nullable = approverId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
        {
          IEnumerable<Guid> guids = source.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (a => a.ApproverId != Guid.Empty)).Select<ReleaseEnvironmentStep, Guid>((Func<ReleaseEnvironmentStep, Guid>) (s => s.ApproverId));
          IEnumerable<Guid> matchingApproverIds = this.getMatchingGroupIds(this.requestContext, approverId.Value, guids);
          return source.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (a => a.ApproverId == approverId.Value || matchingApproverIds.Contains<Guid>(a.ApproverId))).Take<ReleaseEnvironmentStep>(approvalQueryParameters.Top);
        }
      }
      return (IEnumerable<ReleaseEnvironmentStep>) source;
    }

    private static IEnumerable<Guid> GetMatchingGroupIds(
      IVssRequestContext requestContext,
      Guid identity,
      IEnumerable<Guid> identitiesOrGroupIds)
    {
      return identity.GetGroupIds(requestContext, identitiesOrGroupIds);
    }
  }
}
