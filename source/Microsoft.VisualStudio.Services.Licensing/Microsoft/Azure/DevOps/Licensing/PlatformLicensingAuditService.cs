// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Licensing.PlatformLicensingAuditService
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.Licensing
{
  internal class PlatformLicensingAuditService : ILicensingAuditService, IVssFrameworkService
  {
    private const string _licensingAuditServiceFeatureFlag = "AzureDevOps.Services.Licensing.EnableLicensingAuditService";
    private const string _area = "Licensing";
    private const string _layer = "PlatformLicensingAuditService";

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public void LogLicenseAssigned(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      License license,
      AssignmentSource assignmentSource)
    {
      ArgumentUtility.CheckForDefault<Microsoft.VisualStudio.Services.Identity.Identity>(userIdentity, nameof (userIdentity));
      if (!requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.EnableLicensingAuditService"))
        return;
      IVssRequestContext requestContext1 = requestContext;
      string assigned = LicensingAuditConstants.Assigned;
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add(LicensingAuditConstants.UserIdentifier, (object) userIdentity.Id);
      data.Add(LicensingAuditConstants.AccessLevel, (object) license.ToAccessLevel().LicenseDisplayName);
      data.Add(LicensingAuditConstants.Reason, assignmentSource == AssignmentSource.GroupRule ? (object) LicensingResources.AuditGroupRuleReason() : (object) (string) null);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      requestContext1.LogAuditEvent(assigned, data, targetHostId, projectId);
    }

    public void LogLicenseModified(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      License previousLicense,
      License newLicense,
      AssignmentSource assignmentSource)
    {
      ArgumentUtility.CheckForDefault<Microsoft.VisualStudio.Services.Identity.Identity>(userIdentity, nameof (userIdentity));
      if (!requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.EnableLicensingAuditService"))
        return;
      IVssRequestContext requestContext1 = requestContext;
      string modified = LicensingAuditConstants.Modified;
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add(LicensingAuditConstants.UserIdentifier, (object) userIdentity.Id);
      data.Add(LicensingAuditConstants.PreviousAccessLevel, (object) previousLicense.ToAccessLevel().LicenseDisplayName);
      data.Add(LicensingAuditConstants.AccessLevel, (object) newLicense.ToAccessLevel().LicenseDisplayName);
      data.Add(LicensingAuditConstants.Reason, assignmentSource == AssignmentSource.GroupRule ? (object) LicensingResources.AuditGroupRuleReason() : (object) (string) null);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      requestContext1.LogAuditEvent(modified, data, targetHostId, projectId);
    }

    public void LogLicenseRemoved(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      License license)
    {
      ArgumentUtility.CheckForDefault<Microsoft.VisualStudio.Services.Identity.Identity>(userIdentity, nameof (userIdentity));
      if (!requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.EnableLicensingAuditService"))
        return;
      IVssRequestContext requestContext1 = requestContext;
      string removed = LicensingAuditConstants.Removed;
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add(LicensingAuditConstants.UserIdentifier, (object) userIdentity.Id);
      data.Add(LicensingAuditConstants.AccessLevel, (object) license.ToAccessLevel().LicenseDisplayName);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      requestContext1.LogAuditEvent(removed, data, targetHostId, projectId);
    }

    public void LogGroupRuleCreated(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity groupIdentity,
      License license)
    {
      ArgumentUtility.CheckForDefault<Microsoft.VisualStudio.Services.Identity.Identity>(groupIdentity, nameof (groupIdentity));
      if (!requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.EnableLicensingAuditService"))
        return;
      IVssRequestContext requestContext1 = requestContext;
      string groupRuleCreated = LicensingAuditConstants.GroupRuleCreated;
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add(LicensingAuditConstants.GroupIdentifier, (object) groupIdentity.Id);
      data.Add(LicensingAuditConstants.AccessLevel, (object) license.ToAccessLevel().LicenseDisplayName);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      requestContext1.LogAuditEvent(groupRuleCreated, data, targetHostId, projectId);
    }

    public void LogGroupRuleModified(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity groupIdentity,
      License previousLicense,
      License newLicense)
    {
      ArgumentUtility.CheckForDefault<Microsoft.VisualStudio.Services.Identity.Identity>(groupIdentity, nameof (groupIdentity));
      if (!requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.EnableLicensingAuditService"))
        return;
      IVssRequestContext requestContext1 = requestContext;
      string groupRuleModified = LicensingAuditConstants.GroupRuleModified;
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add(LicensingAuditConstants.GroupIdentifier, (object) groupIdentity.Id);
      data.Add(LicensingAuditConstants.PreviousAccessLevel, (object) previousLicense.ToAccessLevel().LicenseDisplayName);
      data.Add(LicensingAuditConstants.AccessLevel, (object) newLicense.ToAccessLevel().LicenseDisplayName);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      requestContext1.LogAuditEvent(groupRuleModified, data, targetHostId, projectId);
    }

    public void LogGroupRuleDeleted(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity groupIdentity,
      License license)
    {
      ArgumentUtility.CheckForDefault<Microsoft.VisualStudio.Services.Identity.Identity>(groupIdentity, nameof (groupIdentity));
      if (!requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.EnableLicensingAuditService"))
        return;
      IVssRequestContext requestContext1 = requestContext;
      string groupRuleDeleted = LicensingAuditConstants.GroupRuleDeleted;
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add(LicensingAuditConstants.GroupIdentifier, (object) groupIdentity.Id);
      data.Add(LicensingAuditConstants.AccessLevel, (object) license.ToAccessLevel().LicenseDisplayName);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      requestContext1.LogAuditEvent(groupRuleDeleted, data, targetHostId, projectId);
    }

    public void LogGroupRuleCreated(
      IVssRequestContext requestContext,
      SubjectDescriptor groupSubjectDescriptor,
      License license)
    {
      this.LogGroupRuleCreated(requestContext, this.ReadIdentity(requestContext, groupSubjectDescriptor), license);
    }

    public void LogGroupRuleModified(
      IVssRequestContext requestContext,
      SubjectDescriptor groupSubjectDescriptor,
      License previousLicense,
      License newLicense)
    {
      this.LogGroupRuleModified(requestContext, this.ReadIdentity(requestContext, groupSubjectDescriptor), previousLicense, newLicense);
    }

    public void LogGroupRuleDeleted(
      IVssRequestContext requestContext,
      SubjectDescriptor groupSubjectDescriptor,
      License license)
    {
      this.LogGroupRuleDeleted(requestContext, this.ReadIdentity(requestContext, groupSubjectDescriptor), license);
    }

    public void LogLicenseAssigned(
      IVssRequestContext requestContext,
      SubjectDescriptor userSubjectDescriptor,
      License license,
      AssignmentSource assignmentSource)
    {
      this.LogLicenseAssigned(requestContext, this.ReadIdentity(requestContext, userSubjectDescriptor), license, assignmentSource);
    }

    public void LogLicenseModified(
      IVssRequestContext requestContext,
      SubjectDescriptor userSubjectDescriptor,
      License previousLicense,
      License newLicense,
      AssignmentSource assignmentSource)
    {
      this.LogLicenseModified(requestContext, this.ReadIdentity(requestContext, userSubjectDescriptor), previousLicense, newLicense, assignmentSource);
    }

    public void LogLicenseRemoved(
      IVssRequestContext requestContext,
      SubjectDescriptor userSubjectDescriptor,
      License license)
    {
      this.LogLicenseRemoved(requestContext, this.ReadIdentity(requestContext, userSubjectDescriptor), license);
    }

    private Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor)
    {
      return requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<SubjectDescriptor>) new List<SubjectDescriptor>()
      {
        subjectDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null).Single<Microsoft.VisualStudio.Services.Identity.Identity>();
    }
  }
}
