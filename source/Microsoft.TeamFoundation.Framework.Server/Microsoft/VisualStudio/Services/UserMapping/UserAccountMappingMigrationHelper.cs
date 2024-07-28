// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.UserMapping.UserAccountMappingMigrationHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.UserAccountMapping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.UserMapping
{
  public static class UserAccountMappingMigrationHelper
  {
    private const string c_area = "UserAccountMappingMigrationHelper";
    private const string c_layer = "UserAccountMappingMigrationHelper";

    public static IList<Guid> QueryAccountIds(
      IVssRequestContext context,
      Guid userId,
      UserRole userType,
      bool useEqualsCheckForUserTypeMatch = false,
      bool includeDeletedAccounts = false)
    {
      UserAccountMappingMigrationHelper.ValidateRequestContext(context);
      try
      {
        return context.GetService<Microsoft.VisualStudio.Services.UserAccountMapping.FrameworkUserAccountMappingService>().QueryAccountIds(context, (context.GetService<IdentityService>().ReadIdentities(context, (IList<Guid>) new List<Guid>()
        {
          userId
        }, QueryMembership.None, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>() ?? throw new IdentityNotFoundException(string.Format("No identity found for identity id {0}.", (object) userId))).SubjectDescriptor, userType, useEqualsCheckForUserTypeMatch, includeDeletedAccounts);
      }
      catch (Exception ex)
      {
        context.TraceException(7799100, nameof (UserAccountMappingMigrationHelper), nameof (UserAccountMappingMigrationHelper), ex);
        return context.GetService<FrameworkUserAccountMappingService>().QueryAccountIds(context, userId, (UserType) userType, useEqualsCheckForUserTypeMatch, includeDeletedAccounts);
      }
    }

    public static IList<Guid> QueryAccountIds(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      UserRole userType,
      bool useEqualsCheckForUserTypeMatch = false,
      bool includeDeletedAccounts = false)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(userIdentity, nameof (userIdentity));
      UserAccountMappingMigrationHelper.ValidateRequestContext(context);
      try
      {
        return context.GetService<Microsoft.VisualStudio.Services.UserAccountMapping.FrameworkUserAccountMappingService>().QueryAccountIds(context, userIdentity.SubjectDescriptor, userType, useEqualsCheckForUserTypeMatch, includeDeletedAccounts);
      }
      catch (Exception ex)
      {
        context.TraceException(7799102, nameof (UserAccountMappingMigrationHelper), nameof (UserAccountMappingMigrationHelper), ex);
        return context.GetService<FrameworkUserAccountMappingService>().QueryAccountIds(context, userIdentity.Id, (UserType) userType, useEqualsCheckForUserTypeMatch, includeDeletedAccounts);
      }
    }

    private static void ValidateRequestContext(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      context.CheckDeploymentRequestContext();
    }
  }
}
