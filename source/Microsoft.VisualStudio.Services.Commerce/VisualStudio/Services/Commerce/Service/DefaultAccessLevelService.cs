// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Service.DefaultAccessLevelService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.Migration.Utilities;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce.Service
{
  internal class DefaultAccessLevelService : IDefaultAccessLevelService, IVssFrameworkService
  {
    private const string Area = "Commerce";
    private const string Layer = "DefaultAccessLevelService";

    public DefaultAccessLevel GetDefaultAccessLevel(IVssRequestContext collectionContext)
    {
      try
      {
        collectionContext.CheckProjectCollectionRequestContext();
        collectionContext.TraceAlways(5109330, TraceLevel.Info, "Commerce", nameof (DefaultAccessLevelService), "Enter Getdefault licence level for organization {0}", (object) collectionContext.ServiceHost.InstanceId);
        if (collectionContext.GetAuthenticatedIdentity() == null)
          throw new InvalidAccessException(string.Format("User {0} not a valid collection user", (object) collectionContext.GetAuthenticatedIdentity()));
        DefaultAccessLevel defaultAccessLevel = DefaultAccessLevel.Stakeholder;
        if (collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableBasicLicenceLevelByDefault"))
          defaultAccessLevel = DefaultAccessLevel.Basic;
        collectionContext.TraceAlways(5109332, TraceLevel.Info, "Commerce", nameof (DefaultAccessLevelService), "Default licence level for organization {0} is {1}", (object) collectionContext.ServiceHost.InstanceId, (object) defaultAccessLevel);
        return defaultAccessLevel;
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5109331, TraceLevel.Info, "Commerce", nameof (DefaultAccessLevelService), ex);
        throw;
      }
    }

    public void SetDefaultAccessLevel(
      IVssRequestContext collectionContext,
      DefaultAccessLevel accessLevel)
    {
      try
      {
        collectionContext.CheckProjectCollectionRequestContext();
        collectionContext.TraceAlways(5109335, TraceLevel.Info, "Commerce", nameof (DefaultAccessLevelService), "Enter set default licence level {0} for organization {1}", (object) accessLevel, (object) collectionContext.ServiceHost.InstanceId);
        IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
        SubjectDescriptor subjectDescriptor = vssRequestContext.GetAuthenticatedDescriptor().ToSubjectDescriptor(collectionContext);
        if (!CommerceDeploymentHelper.IsProjectCollectionAdmin(collectionContext, subjectDescriptor, 5109335, "Commerce", nameof (DefaultAccessLevelService)))
          throw new InvalidAccessException(string.Format("User {0} does not have permissions to access organization information.", (object) subjectDescriptor));
        FeatureAvailabilityState state = FeatureAvailabilityState.Off;
        if (accessLevel == DefaultAccessLevel.Basic)
          state = FeatureAvailabilityState.On;
        collectionContext.TraceAlways(5109336, TraceLevel.Info, "Commerce", nameof (DefaultAccessLevelService), "FF state we are trying to set for request is {0}", (object) state);
        collectionContext.Elevate().GetService<ITeamFoundationFeatureAvailabilityService>().SetFeatureState(collectionContext.Elevate(), "VisualStudio.Services.Commerce.EnableBasicLicenceLevelByDefault", state);
        int accessLevel1 = accessLevel == DefaultAccessLevel.Basic ? 2 : 5;
        MigrationUtilities.DualWriteDefaultAccessLevel(vssRequestContext, collectionContext.ServiceHost.InstanceId, accessLevel1);
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5109337, TraceLevel.Info, "Commerce", nameof (DefaultAccessLevelService), ex);
        throw;
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckProjectCollectionRequestContext();
  }
}
