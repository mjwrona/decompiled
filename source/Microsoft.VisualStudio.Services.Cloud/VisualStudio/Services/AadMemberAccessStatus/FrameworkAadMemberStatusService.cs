// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.AadMemberAccessStatus.FrameworkAadMemberStatusService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories.DirectoryService.Client;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.AadMemberAccessStatus
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkAadMemberStatusService : AadMemberStatusServiceBase
  {
    protected new string TraceLayer = nameof (FrameworkAadMemberStatusService);

    protected internal override bool GetAadMemberStatusFromRemote(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor,
      Guid oid,
      Guid tenantId,
      out AadMemberStatus memberStatus)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(9002430, "AadUserStateService", this.TraceLayer, nameof (GetAadMemberStatusFromRemote));
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<IdentityDescriptor>(identityDescriptor, nameof (identityDescriptor));
        ArgumentUtility.CheckForEmptyGuid(tenantId, nameof (tenantId));
        ArgumentUtility.CheckForEmptyGuid(oid, nameof (oid));
        DirectoryHttpClient client = requestContext.Elevate().GetClient<DirectoryHttpClient>();
        try
        {
          memberStatus = client.GetAadMemberStatusAsync(identityDescriptor, oid, tenantId).SyncResult<AadMemberStatus>();
          return true;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(9002432, "AadUserStateService", this.TraceLayer, ex);
          memberStatus = new AadMemberStatus()
          {
            MemberState = AadMemberAccessState.Indeterminate,
            StatusValidUntil = new DateTimeOffset()
          };
          return false;
        }
      }
      finally
      {
        requestContext.TraceLeave(9002431, "AadUserStateService", this.TraceLayer, nameof (GetAadMemberStatusFromRemote));
      }
    }

    protected internal override bool GetAadMemberStatusFromRemote(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      Guid oid,
      Guid tenantId,
      out AadMemberStatus memberStatus)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(9002430, "AadUserStateService", this.TraceLayer, nameof (GetAadMemberStatusFromRemote));
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForEmptyGuid(tenantId, nameof (tenantId));
        ArgumentUtility.CheckForEmptyGuid(oid, nameof (oid));
        DirectoryHttpClient client = requestContext.Elevate().GetClient<DirectoryHttpClient>();
        try
        {
          memberStatus = client.GetAadMemberStatusAsync(subjectDescriptor, oid, tenantId).SyncResult<AadMemberStatus>();
          return true;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(9002432, "AadUserStateService", this.TraceLayer, ex);
          memberStatus = new AadMemberStatus()
          {
            MemberState = AadMemberAccessState.Indeterminate,
            StatusValidUntil = new DateTimeOffset()
          };
          return false;
        }
      }
      finally
      {
        requestContext.TraceLeave(9002431, "AadUserStateService", this.TraceLayer, nameof (GetAadMemberStatusFromRemote));
      }
    }
  }
}
