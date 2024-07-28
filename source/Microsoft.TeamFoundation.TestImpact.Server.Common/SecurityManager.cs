// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.SecurityManager
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestImpact.Server.Common.Properties;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [CLSCompliant(false)]
  public sealed class SecurityManager : IVssFrameworkService
  {
    private static long LogThreshold = DateTime.MinValue.Ticks;

    internal IVssSecurityNamespace ProjectSecurity { get; private set; }

    internal IVssSecurityNamespace BuildSecurity { get; private set; }

    void IVssFrameworkService.ServiceStart(IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      TeamFoundationSecurityService service = context.GetService<TeamFoundationSecurityService>();
      this.ProjectSecurity = service.GetSecurityNamespace(context, FrameworkSecurity.TeamProjectNamespaceId);
      this.BuildSecurity = service.GetSecurityNamespace(context, Microsoft.TeamFoundation.Build.Common.BuildSecurity.BuildNamespaceId);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext context) => ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));

    public bool HasProjectReadAccess(IVssRequestContext context, Uri projectUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<Uri>(projectUri, nameof (projectUri), context.ServiceName);
      return this.ProjectSecurity.HasPermission(context, this.ProjectSecurity.NamespaceExtension.HandleIncomingToken(context, this.ProjectSecurity, projectUri.ToString()), TeamProjectPermissions.GenericRead, false);
    }

    public void DemandProjectReadAccess(IVssRequestContext context, Uri projectUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<Uri>(projectUri, nameof (projectUri), context.ServiceName);
      if (!this.HasProjectReadAccess(context, projectUri))
        throw new AccessDeniedException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.AccessDenied, (object) context.DomainUserName));
    }

    public bool HasDeleteProjectAccess(IVssRequestContext context, Uri projectUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<Uri>(projectUri, nameof (projectUri), context.ServiceName);
      return this.ProjectSecurity.HasPermission(context, this.ProjectSecurity.NamespaceExtension.HandleIncomingToken(context, this.ProjectSecurity, projectUri.ToString()), TeamProjectPermissions.Delete, false);
    }

    public void DemandDeleteProjectAccess(IVssRequestContext context, Uri projectUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<Uri>(projectUri, nameof (projectUri), context.ServiceName);
      if (!this.HasDeleteProjectAccess(context, projectUri))
        throw new AccessDeniedException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.AccessDenied, (object) context.DomainUserName));
    }

    public bool HasPublishTestResultsAccess(IVssRequestContext context, Uri projectUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<Uri>(projectUri, nameof (projectUri), context.ServiceName);
      return this.ProjectSecurity.HasPermission(context, this.ProjectSecurity.NamespaceExtension.HandleIncomingToken(context, this.ProjectSecurity, projectUri.ToString()), TeamProjectPermissions.PublishTestResults, false);
    }

    public void DemandPublishTestResultsAccess(IVssRequestContext context, Uri projectUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<Uri>(projectUri, nameof (projectUri), context.ServiceName);
      if (!this.HasPublishTestResultsAccess(context, projectUri))
        throw new AccessDeniedException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.AccessDenied, (object) context.DomainUserName));
    }

    public bool HasDeleteTestResultsAccess(IVssRequestContext context, Uri projectUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<Uri>(projectUri, nameof (projectUri), context.ServiceName);
      return this.ProjectSecurity.HasPermission(context, this.ProjectSecurity.NamespaceExtension.HandleIncomingToken(context, this.ProjectSecurity, projectUri.ToString()), TeamProjectPermissions.DeleteTestResults, false);
    }

    public void DemandDeleteTestResultsAccess(IVssRequestContext context, Uri projectUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<Uri>(projectUri, nameof (projectUri), context.ServiceName);
      if (!this.HasDeleteTestResultsAccess(context, projectUri))
        throw new AccessDeniedException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.AccessDenied, (object) context.DomainUserName));
    }

    public bool HasUpdateBuildAccess(
      IVssRequestContext context,
      Uri projectUri,
      Uri buildDefinitionUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<Uri>(projectUri, nameof (projectUri), context.ServiceName);
      ArgumentUtility.CheckForNull<Uri>(buildDefinitionUri, nameof (buildDefinitionUri), context.ServiceName);
      string definitionSecurityToken = this.GetBuildDefinitionSecurityToken(projectUri, buildDefinitionUri);
      bool flag = this.BuildSecurity.HasPermission(context, definitionSecurityToken, BuildPermissions.UpdateBuildInformation, false);
      if (flag && context.IsFeatureEnabled("TestManagement.Server.LogSuspiciousAccessToBuildApi"))
      {
        DateTime now = DateTime.Now;
        long ticks = now.Ticks;
        string str = "skipped";
        if (SecurityManager.LogThreshold < ticks && Interlocked.Exchange(ref SecurityManager.LogThreshold, now.AddSeconds(5.0).Ticks) < ticks)
          str = Environment.StackTrace;
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = context.GetUserIdentity();
        if (!context.IsCollectionAdministrator() && IdentityHelper.IsUserIdentity(context, (IReadOnlyVssIdentity) userIdentity) && SecurityManager.IsMemberOfProjectContributorsGroup(context, userIdentity))
          context.TraceAlways(12030499, TraceLevel.Info, TestImpactServiceCIArea.TestImpactService, "BuildSecurity", "User {0} accessed {1} with requested permissions {2}. Stack: {3}", (object) userIdentity?.Id, (object) definitionSecurityToken, (object) BuildPermissions.UpdateBuildInformation, (object) str);
      }
      return flag;
    }

    private static bool IsMemberOfProjectContributorsGroup(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = service.ReadIdentities(requestContext, IdentitySearchFilter.LocalGroupName, "Contributors", QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      return identity1 != null && service.IsMember(requestContext, identity1.Descriptor, identity.Descriptor);
    }

    public void DemandUpdateBuildAccess(
      IVssRequestContext context,
      Uri projectUri,
      Uri buildDefinitionUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<Uri>(projectUri, nameof (projectUri), context.ServiceName);
      ArgumentUtility.CheckForNull<Uri>(buildDefinitionUri, nameof (buildDefinitionUri), context.ServiceName);
      if (!this.HasUpdateBuildAccess(context, projectUri, buildDefinitionUri))
        throw new AccessDeniedException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.AccessDenied, (object) context.DomainUserName));
    }

    public bool HasDeleteBuildsAccess(
      IVssRequestContext context,
      Uri projectUri,
      Uri buildDefinitionUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<Uri>(projectUri, nameof (projectUri), context.ServiceName);
      ArgumentUtility.CheckForNull<Uri>(buildDefinitionUri, nameof (buildDefinitionUri), context.ServiceName);
      return this.BuildSecurity.HasPermission(context, this.GetBuildDefinitionSecurityToken(projectUri, buildDefinitionUri), BuildPermissions.DeleteBuilds, false);
    }

    public void DemandDeleteBuildsAccess(
      IVssRequestContext context,
      Uri projectUri,
      Uri buildDefinitionUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<Uri>(projectUri, nameof (projectUri), context.ServiceName);
      ArgumentUtility.CheckForNull<Uri>(buildDefinitionUri, nameof (buildDefinitionUri), context.ServiceName);
      if (!this.HasDeleteBuildsAccess(context, projectUri, buildDefinitionUri))
        throw new AccessDeniedException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.AccessDenied, (object) context.DomainUserName));
    }

    private string GetBuildDefinitionSecurityToken(Uri projectUri, Uri buildDefinitionUri) => LinkingUtilities.DecodeUri(projectUri.ToString()).ToolSpecificId + (object) Microsoft.TeamFoundation.Build.Common.BuildSecurity.NamespaceSeparator + LinkingUtilities.DecodeUri(buildDefinitionUri.ToString()).ToolSpecificId;
  }
}
