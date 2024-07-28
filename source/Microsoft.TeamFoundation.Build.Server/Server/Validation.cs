// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Validation
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal static class Validation
  {
    internal static void CheckBuildAgentTag(
      string argumentName,
      string tag,
      bool allowNull,
      string errorMessage)
    {
      ArgumentValidation.Check(argumentName, (object) tag, allowNull);
      if (!string.IsNullOrEmpty(tag) && tag.Length > 256)
        throw new ArgumentException(AdministrationResources.InvalidBuildAgentTag((object) tag));
    }

    internal static void CheckDescription(string argumentName, string description, bool allowNull)
    {
      ArgumentValidation.Check(argumentName, description, allowNull, ResourceStrings.MissingDescription());
      if (!string.IsNullOrEmpty(description) && description.Length > 2048)
        throw new ArgumentException(ResourceStrings.InvalidDescription());
    }

    internal static void CheckIdentityName(
      string argumentName,
      string identityName,
      bool allowNull)
    {
      ArgumentValidation.Check(argumentName, identityName, allowNull, (string) null);
      if (!string.IsNullOrEmpty(identityName) && identityName.Length > 512)
        throw new ArgumentException(ResourceStrings.InvalidIdentityName((object) identityName));
    }

    internal static void CheckQuality(
      string argumentName,
      string quality,
      bool allowNull,
      string errorMessage)
    {
      ArgumentValidation.Check(argumentName, quality, allowNull, errorMessage);
      if (!string.IsNullOrEmpty(quality) && quality.Length > 256)
        throw new ArgumentException(ResourceStrings.InvalidQuality());
    }

    internal static void CheckValidatable(
      IVssRequestContext requestContext,
      string argumentName,
      IValidatable obj,
      bool allowNull,
      ValidationContext context)
    {
      ArgumentValidation.Check(argumentName, (object) obj, allowNull);
      obj?.Validate(requestContext, context);
    }

    internal static void CheckValidatable<T>(
      IVssRequestContext requestContext,
      string argumentName,
      IList<T> array,
      bool allowNull,
      ValidationContext context)
      where T : IValidatable
    {
      if (array == null)
      {
        if (!allowNull)
          throw new ArgumentException(BuildTypeResource.InvalidInputParameterNull((object) argumentName));
      }
      else
      {
        for (int index = 0; index < array.Count; ++index)
        {
          try
          {
            Validation.CheckValidatable(requestContext, argumentName, (IValidatable) array[index], allowNull, context);
          }
          catch (ArgumentException ex)
          {
            if (array.Count != 1)
              throw new ArgumentException(BuildTypeResource.InvalidInputAtIndex((object) argumentName, (object) index, (object) ex.Message));
            throw;
          }
        }
      }
    }

    internal static void CheckVersionControlPath(
      string argumentName,
      ref string argument,
      bool allowNull)
    {
      ArgumentValidation.Check(argumentName, argument, allowNull, (string) null);
      if (string.IsNullOrEmpty(argument))
        return;
      try
      {
        argument = VersionControlPath.GetFullPath(argument);
      }
      catch (Exception ex)
      {
        throw new ArgumentException(ex.Message, argumentName);
      }
    }

    internal static void CheckVersionControlPath(
      string argumentName,
      IList<string> array,
      bool allowNull)
    {
      if (array == null)
      {
        if (!allowNull)
          throw new ArgumentException(BuildTypeResource.InvalidInputParameterNull((object) argumentName));
      }
      else
      {
        for (int index = 0; index < array.Count; ++index)
        {
          try
          {
            string str = array[index];
            Validation.CheckVersionControlPath(argumentName, ref str, allowNull);
            array[index] = str;
          }
          catch (ArgumentException ex)
          {
            if (array.Count != 1)
              throw new ArgumentException(BuildTypeResource.InvalidInputAtIndex((object) argumentName, (object) index, (object) ex.Message));
            throw;
          }
        }
      }
    }

    public static TeamFoundationIdentity ResolveIdentity(
      IVssRequestContext requestContext,
      string identityName)
    {
      TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationIdentity foundationIdentity = (TeamFoundationIdentity) null;
      if (!string.IsNullOrEmpty(identityName))
      {
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && !UserNameUtil.IsComplete(identityName) && ArgumentUtility.IsValidEmailAddress(identityName))
        {
          string relative = "Windows Live ID";
          Guid organizationAadTenantId = requestContext.GetOrganizationAadTenantId();
          if (!organizationAadTenantId.Equals(Guid.Empty))
            relative = organizationAadTenantId.ToString();
          identityName = UserNameUtil.Complete(identityName, relative);
        }
        if (TFCommonUtil.IsLegalIdentity(identityName))
          foundationIdentity = service.ReadIdentity(requestContext, IdentitySearchFactor.AccountName, identityName, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null);
        if (foundationIdentity == null)
          foundationIdentity = service.ReadIdentity(requestContext, IdentitySearchFactor.General, identityName, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null);
        if (foundationIdentity == null)
          foundationIdentity = service.ReadIdentity(requestContext, new IdentityDescriptor("System:ServicePrincipal", identityName), MembershipQuery.None, ReadIdentityOptions.None);
      }
      return foundationIdentity;
    }
  }
}
