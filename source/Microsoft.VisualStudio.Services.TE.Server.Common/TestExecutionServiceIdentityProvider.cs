// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestExecutionServiceIdentityProvider
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class TestExecutionServiceIdentityProvider
  {
    private static ITestExecutionServiceIdentityHelper testExecutionIdentityHelper = (ITestExecutionServiceIdentityHelper) new TestExecutionServiceIdentityHelper();

    public TestExecutionServiceIdentityProvider(ITestExecutionServiceIdentityHelper identityHelper) => TestExecutionServiceIdentityProvider.testExecutionIdentityHelper = identityHelper;

    public static ServiceIdentityInfo EnsureTestExecutionServiceIdentitiyIsProvisioned(
      TestExecutionRequestContext context)
    {
      ServiceIdentityInfo serviceIdentityInfo = (ServiceIdentityInfo) null;
      IVssRequestContext requestContext = context.RequestContext;
      bool hostedDeployment = requestContext.ExecutionEnvironment.IsHostedDeployment;
      requestContext.Trace(6200081, TraceLevel.Info, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, "Provisioning TestExecutionService identity. HostedDeployment:{0}.", (object) hostedDeployment);
      if (hostedDeployment)
      {
        string serviceIdentityName = TestExecutionServiceResources.TestExecutionServiceIdentityName;
        ServiceIdentityInfo identityInfo = new ServiceIdentityInfo()
        {
          Name = serviceIdentityName
        };
        IdentityDescriptor foundationDescriptor = IdentityHelper.CreateTeamFoundationDescriptor(TestWellKnownGroups.TestServiceAccountsIdentifier);
        try
        {
          serviceIdentityInfo = TestExecutionServiceIdentityProvider.testExecutionIdentityHelper.ProvisionServiceIdentity(context, identityInfo, new IdentityDescriptor[1]
          {
            foundationDescriptor
          }).IdentityInfo;
          requestContext.Trace(6200086, TraceLevel.Info, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, "Provisioning TestExecutionService identity provisioned:{0}.", (object) serviceIdentityInfo.Name);
        }
        catch (AccessCheckException ex)
        {
          requestContext.Trace(6200087, TraceLevel.Info, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, "Provisioning TestExecutionService identity failed with the exception:{0}.", (object) ex.Message);
        }
      }
      return serviceIdentityInfo;
    }
  }
}
