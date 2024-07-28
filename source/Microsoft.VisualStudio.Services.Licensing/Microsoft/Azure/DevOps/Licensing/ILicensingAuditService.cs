// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Licensing.ILicensingAuditService
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Licensing;

namespace Microsoft.Azure.DevOps.Licensing
{
  [DefaultServiceImplementation(typeof (PlatformLicensingAuditService))]
  internal interface ILicensingAuditService : IVssFrameworkService
  {
    void LogLicenseAssigned(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      License license,
      AssignmentSource assignmentSource);

    void LogLicenseAssigned(
      IVssRequestContext requestContext,
      SubjectDescriptor userSubjectDescriptor,
      License license,
      AssignmentSource assignmentSource);

    void LogLicenseModified(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      License previousLicense,
      License newLicense,
      AssignmentSource assignmentSource);

    void LogLicenseModified(
      IVssRequestContext requestContext,
      SubjectDescriptor userSubjectDescriptor,
      License previousLicense,
      License newLicense,
      AssignmentSource assignmentSource);

    void LogLicenseRemoved(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      License license);

    void LogLicenseRemoved(
      IVssRequestContext requestContext,
      SubjectDescriptor userSubjectDescriptor,
      License license);

    void LogGroupRuleCreated(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity groupIdentity,
      License license);

    void LogGroupRuleCreated(
      IVssRequestContext requestContext,
      SubjectDescriptor groupSubjectDescriptor,
      License license);

    void LogGroupRuleModified(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity groupIdentity,
      License previousLicense,
      License newLicense);

    void LogGroupRuleModified(
      IVssRequestContext requestContext,
      SubjectDescriptor groupSubjectDescriptor,
      License previousLicense,
      License newLicense);

    void LogGroupRuleDeleted(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity groupIdentity,
      License license);

    void LogGroupRuleDeleted(
      IVssRequestContext requestContext,
      SubjectDescriptor groupSubjectDescriptor,
      License license);
  }
}
