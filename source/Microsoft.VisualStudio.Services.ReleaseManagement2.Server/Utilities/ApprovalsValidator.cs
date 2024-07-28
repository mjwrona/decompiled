// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ApprovalsValidator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public class ApprovalsValidator
  {
    public const string LocalScopeId = "LocalScopeId";
    private readonly IVssRequestContext context;
    private readonly Guid projectId;
    private readonly Microsoft.VisualStudio.Services.Identity.Identity requestor;

    public ApprovalsValidator(IVssRequestContext context, Guid projectId)
      : this(context, projectId, context.GetUserIdentity())
    {
    }

    protected ApprovalsValidator(IVssRequestContext context, Guid projectId, Microsoft.VisualStudio.Services.Identity.Identity requestor)
    {
      this.context = context;
      this.projectId = projectId;
      this.requestor = requestor;
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "To optimize db calls")]
    public bool IsRequestorApprover(int approvalId, ReleaseEnvironmentStep step)
    {
      if (step == null)
        throw new ReleaseManagementObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseEnvironmentStepNotFound, (object) approvalId));
      return step.ApproverId == this.requestor.Id;
    }

    public bool IsRequestorAuthorizedGroupMember(ReleaseEnvironmentStep releaseStep)
    {
      if (releaseStep == null)
        throw new ArgumentNullException(nameof (releaseStep));
      IdentityService service = this.context.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity = service.ReadIdentities(this.context, (IList<Guid>) new Guid[1]
      {
        releaseStep.ApproverId
      }, QueryMembership.None, (IEnumerable<string>) null).Single<Microsoft.VisualStudio.Services.Identity.Identity>();
      return identity != null && identity.IsContainer && service.IsMember(this.context, identity.Descriptor, this.requestor.Descriptor);
    }

    public bool IsRequestorReleaseManager(int releaseDefinitionId) => this.context.HasPermission(this.projectId, ReleaseManagementSecurityProcessor.GetFolderPath(this.context, this.projectId, releaseDefinitionId), releaseDefinitionId, ReleaseManagementSecurityPermissions.AdministerReleasePermissions);
  }
}
