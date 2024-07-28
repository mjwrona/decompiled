// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.ProfileSyncFeatureDefaultValue
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using Microsoft.VisualStudio.Services.WebApi.Contracts.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins
{
  public class ProfileSyncFeatureDefaultValue : IContributedFeatureValuePlugin
  {
    private const string Area = "ContributedFeatureValuePlugin";
    private const string Layer = "ProfileSyncFeatureDefaultValue";

    public string Name => nameof (ProfileSyncFeatureDefaultValue);

    public ContributedFeatureEnabledValue ComputeEnabledValue(
      IVssRequestContext requestContext,
      ContributedFeature feature,
      object properties,
      IDictionary<string, string> scopeValues,
      out string reason)
    {
      SubjectDescriptor descriptor = requestContext.UserSubjectDescriptor();
      try
      {
        UserAttribute attribute = requestContext.GetService<IUserService>().GetAttribute(requestContext, descriptor, WellKnownUserAttributeNames.UserProfileSyncState);
        reason = "The attribute " + WellKnownUserAttributeNames.UserProfileSyncState + " is set to " + attribute.Value + ".";
        requestContext.Trace(10050103, TraceLevel.Info, "ContributedFeatureValuePlugin", nameof (ProfileSyncFeatureDefaultValue), string.Format("Profile sync enabled for {0}.", (object) descriptor));
        return EnumUtility.Parse<UserProfileSyncState>(attribute.Value) == UserProfileSyncState.Completed ? ContributedFeatureEnabledValue.Enabled : ContributedFeatureEnabledValue.Disabled;
      }
      catch (UserAttributeDoesNotExistException ex)
      {
        reason = "The attribute '" + WellKnownUserAttributeNames.UserProfileSyncState + "' is not set.";
        requestContext.Trace(10050104, TraceLevel.Error, "ContributedFeatureValuePlugin", nameof (ProfileSyncFeatureDefaultValue), string.Format("ProfileSync for {0}.\n {1}", (object) descriptor, (object) ex));
        return ContributedFeatureEnabledValue.Disabled;
      }
      catch (Exception ex)
      {
        reason = "Error while retrieving the actual state of ProfileSync.";
        requestContext.Trace(10050105, TraceLevel.Error, "ContributedFeatureValuePlugin", nameof (ProfileSyncFeatureDefaultValue), string.Format("ProfileSync for {0}.\n {1}", (object) descriptor, (object) ex));
        return ContributedFeatureEnabledValue.Undefined;
      }
    }

    public object ProcessProperties(IDictionary<string, object> properties) => new object();
  }
}
