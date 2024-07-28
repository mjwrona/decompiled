// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestExecutionServiceIdentityHelper
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class TestExecutionServiceIdentityHelper : ITestExecutionServiceIdentityHelper
  {
    public ServiceIdentity ProvisionServiceIdentity(
      TestExecutionRequestContext context,
      ServiceIdentityInfo identityInfo,
      IdentityDescriptor[] addToGroups)
    {
      IVssRequestContext requestContext = context.RequestContext;
      return requestContext.GetService<TeamFoundationAccessControlService>().ProvisionServiceIdentity(requestContext, identityInfo, addToGroups);
    }

    public bool IsUserIdentity(TestExecutionRequestContext requestContext, Guid userIdentifier)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = Utilities.ReadIdentityByAccountId(requestContext, userIdentifier);
      return identity != null && IdentityHelper.IsUserIdentity(requestContext.RequestContext, (IReadOnlyVssIdentity) identity);
    }
  }
}
