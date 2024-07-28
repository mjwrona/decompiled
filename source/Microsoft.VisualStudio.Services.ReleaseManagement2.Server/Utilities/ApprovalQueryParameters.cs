// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ApprovalQueryParameters
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public class ApprovalQueryParameters
  {
    public ApprovalQueryParameters(
      IList<int> releaseIdsFilter,
      ReleaseEnvironmentStepStatus statusFilter,
      EnvironmentStepType typeFilter,
      Guid? approverIdFilter,
      Guid? actualApproverIdFilter,
      int top,
      int continuationToken,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder queryOrder,
      bool includeMyGroupApprovals,
      DateTime? minModifiedTime,
      DateTime? maxModifiedTime)
    {
      this.ReleaseIdsFilter = releaseIdsFilter;
      this.StatusFilter = statusFilter;
      this.TypeFilter = typeFilter;
      this.Top = top;
      this.ContinuationToken = continuationToken;
      this.QueryOrder = queryOrder;
      this.IncludeMyGroupApprovals = includeMyGroupApprovals;
      this.ApproverIdFilter = approverIdFilter;
      this.ActualApproverIdFilter = actualApproverIdFilter;
      this.MinModifiedTime = minModifiedTime;
      this.MaxModifiedTime = maxModifiedTime;
    }

    public IList<int> ReleaseIdsFilter { get; private set; }

    public ReleaseEnvironmentStepStatus StatusFilter { get; private set; }

    public EnvironmentStepType TypeFilter { get; private set; }

    public Guid? ApproverIdFilter { get; private set; }

    public Guid? ActualApproverIdFilter { get; private set; }

    public int Top { get; private set; }

    public int ContinuationToken { get; private set; }

    public Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder QueryOrder { get; private set; }

    public bool IncludeMyGroupApprovals { get; private set; }

    public DateTime? MinModifiedTime { get; private set; }

    public DateTime? MaxModifiedTime { get; private set; }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "Compat is a correct word in the context")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Temporary fix")]
    public static ApprovalQueryParameters GetQueryParameters(
      IVssRequestContext requestContext,
      string assignedToFilter,
      ApprovalStatus statusFilter,
      string releaseIdsFilter,
      ApprovalType typeFilter,
      int top,
      int continuationToken,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseQueryOrder releaseQueryOrder,
      bool includeMyGroupApprovals,
      bool backCompat = false,
      DateTime? minModifiedTime = null,
      DateTime? maxModifiedTime = null)
    {
      IEnumerable<int> intList = ServerModelUtility.ToIntList(releaseIdsFilter);
      Guid? assignedTo;
      if (!ApprovalQueryParameters.TryGetValidAssignedTo(requestContext, assignedToFilter, out assignedTo))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidAssignedToFilter, (object) assignedToFilter));
      if (includeMyGroupApprovals && (intList == null || !assignedTo.HasValue || statusFilter != ApprovalStatus.Pending))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.IncludeMyGroupApprovalsNotSupported));
      if (backCompat && intList != null && assignedTo.HasValue && statusFilter == ApprovalStatus.Pending)
        includeMyGroupApprovals = true;
      List<int> list = intList == null ? (List<int>) null : intList.Distinct<int>().ToList<int>();
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder releaseQueryOrder1 = releaseQueryOrder == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseQueryOrder.Ascending ? Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder.IdAscending : Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder.IdDescending;
      ReleaseEnvironmentStepStatus environmentStepStatus = statusFilter.FromWebApi();
      EnvironmentStepType environmentStepType = typeFilter.ToEnvironmentStepType();
      Guid? nullable1 = new Guid?();
      Guid? nullable2 = new Guid?();
      if (!statusFilter.IsCompletedStatus())
        nullable1 = assignedTo;
      else
        nullable2 = assignedTo;
      int statusFilter1 = (int) environmentStepStatus;
      int typeFilter1 = (int) environmentStepType;
      Guid? approverIdFilter = nullable1;
      Guid? actualApproverIdFilter = nullable2;
      int top1 = top;
      int continuationToken1 = continuationToken;
      int queryOrder = (int) releaseQueryOrder1;
      int num = includeMyGroupApprovals ? 1 : 0;
      DateTime? minModifiedTime1 = minModifiedTime;
      DateTime? maxModifiedTime1 = maxModifiedTime;
      return new ApprovalQueryParameters((IList<int>) list, (ReleaseEnvironmentStepStatus) statusFilter1, (EnvironmentStepType) typeFilter1, approverIdFilter, actualApproverIdFilter, top1, continuationToken1, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder) queryOrder, num != 0, minModifiedTime1, maxModifiedTime1);
    }

    private static bool TryGetValidAssignedTo(
      IVssRequestContext requestContext,
      string assignedToFilter,
      out Guid? assignedTo)
    {
      assignedTo = new Guid?();
      if (string.IsNullOrWhiteSpace(assignedToFilter))
        return true;
      List<Guid> list = requestContext.GetIdentityIds(assignedToFilter).ToList<Guid>();
      if (list.Count<Guid>() != 1 || list.Single<Guid>() == Guid.Empty)
        return false;
      assignedTo = new Guid?(list.Single<Guid>());
      return true;
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Method is more appropriate")]
    public ApprovalQueryParameters GetSqlFilters()
    {
      ApprovalQueryParameters sqlFilters = this.Clone();
      if (sqlFilters.IncludeMyGroupApprovals)
      {
        sqlFilters.Top = 0;
        sqlFilters.ApproverIdFilter = new Guid?();
      }
      return sqlFilters;
    }

    public ApprovalQueryParameters Clone() => new ApprovalQueryParameters(this.ReleaseIdsFilter, this.StatusFilter, this.TypeFilter, this.ApproverIdFilter, this.ActualApproverIdFilter, this.Top, this.ContinuationToken, this.QueryOrder, this.IncludeMyGroupApprovals, this.MinModifiedTime, this.MaxModifiedTime);
  }
}
