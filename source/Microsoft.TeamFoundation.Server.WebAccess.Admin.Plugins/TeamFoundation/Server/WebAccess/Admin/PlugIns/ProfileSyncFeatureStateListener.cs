// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.ProfileSyncFeatureStateListener
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Settings;
using Microsoft.VisualStudio.Services.Users.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins
{
  public class ProfileSyncFeatureStateListener : IContributedFeatureStateChangedListener
  {
    private const string Area = "ContributedFeatureValueListener";
    private const string Layer = "ProfileSyncFeatureStateListener";

    public string Name => nameof (ProfileSyncFeatureStateListener);

    public void OnFeatureStateChanged(
      IVssRequestContext requestContext,
      string featureId,
      ContributedFeatureEnabledValue state,
      SettingsUserScope userScope,
      string scopeName,
      string scopeValue,
      IDictionary<string, object> properties)
    {
      requestContext.TraceEnter(10050106, "ContributedFeatureValueListener", nameof (ProfileSyncFeatureStateListener), nameof (OnFeatureStateChanged));
      if (AadIdentityHelper.IsAadUser((IReadOnlyVssIdentity) requestContext.GetUserIdentity()))
      {
        switch (state)
        {
          case ContributedFeatureEnabledValue.Disabled:
            this.HandleDisabledFeatureState(requestContext);
            break;
          case ContributedFeatureEnabledValue.Enabled:
            this.HandleEnabledFeatureState(requestContext);
            break;
        }
      }
      requestContext.TraceLeave(10050107, "ContributedFeatureValueListener", nameof (ProfileSyncFeatureStateListener), nameof (OnFeatureStateChanged));
    }

    private void HandleEnabledFeatureState(IVssRequestContext requestContext)
    {
      try
      {
        requestContext.GetService<IUserService>().EnableUserProfileDataSync(requestContext, requestContext.UserSubjectDescriptor());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10050108, "ContributedFeatureValueListener", nameof (ProfileSyncFeatureStateListener), ex);
        throw new ContributedFeatureListenerCallbackException("Error during execution of ProfileSyncFeatureStateListener enabling action!", ex);
      }
    }

    private void HandleDisabledFeatureState(IVssRequestContext requestContext)
    {
      try
      {
        requestContext.GetService<IUserService>().DisableUserProfileDataSync(requestContext, requestContext.UserSubjectDescriptor());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10050109, "ContributedFeatureValueListener", nameof (ProfileSyncFeatureStateListener), ex);
        throw new ContributedFeatureListenerCallbackException("Error during execution of ProfileSyncFeatureStateListener disabling action!", ex);
      }
    }
  }
}
