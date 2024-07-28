// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.UserServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Users.Server
{
  public static class UserServiceExtensions
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static User GetUser(
      this IUserService userService,
      IVssRequestContext requestContext,
      Guid userId)
    {
      return userService.UserServiceInternal().GetUser(requestContext, userId);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static User UpdateUser(
      this IUserService userService,
      IVssRequestContext requestContext,
      Guid userId,
      UpdateUserParameters userParameters)
    {
      return userService.UserServiceInternal().UpdateUser(requestContext, userId, userParameters);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static UserAttribute GetAttribute(
      this IUserService userService,
      IVssRequestContext requestContext,
      Guid userId,
      string attributeName)
    {
      return userService.UserServiceInternal().GetAttribute(requestContext, userId, attributeName);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IList<UserAttribute> QueryAttributes(
      this IUserService userService,
      IVssRequestContext requestContext,
      Guid userId,
      string queryPattern,
      DateTimeOffset? modifiedAfter = null)
    {
      return userService.UserServiceInternal().QueryAttributes(requestContext, userId, queryPattern, modifiedAfter);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static UserAttribute SetAttribute(
      this IUserService userService,
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      string attributeName,
      string attributeValue)
    {
      return userService.SetAttributes(requestContext, descriptor, (IList<SetUserAttributeParameters>) new SetUserAttributeParameters[1]
      {
        new SetUserAttributeParameters()
        {
          Name = attributeName,
          Value = attributeValue
        }
      })[0];
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static UserAttribute SetAttribute(
      this IUserService userService,
      IVssRequestContext requestContext,
      Guid userId,
      string attributeName,
      string attributeValue)
    {
      return userService.UserServiceInternal().SetAttributes(requestContext, userId, (IList<SetUserAttributeParameters>) new SetUserAttributeParameters[1]
      {
        new SetUserAttributeParameters()
        {
          Name = attributeName,
          Value = attributeValue
        }
      })[0];
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static UserAttribute SetPrivateAttribute(
      this IUserPrivateAttributesService userService,
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      string attributeName,
      string attributeValue)
    {
      return userService.SetPrivateAttributes(requestContext, descriptor, (IList<SetUserAttributeParameters>) new SetUserAttributeParameters[1]
      {
        new SetUserAttributeParameters()
        {
          Name = attributeName,
          Value = attributeValue
        }
      })[0];
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static UserAttribute SetPrivateAttribute(
      this IUserPrivateAttributesService userService,
      IVssRequestContext requestContext,
      Guid userId,
      string attributeName,
      string attributeValue)
    {
      return userService.SetPrivateAttributes(requestContext, userId, (IList<SetUserAttributeParameters>) new SetUserAttributeParameters[1]
      {
        new SetUserAttributeParameters()
        {
          Name = attributeName,
          Value = attributeValue
        }
      })[0];
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IList<UserAttribute> SetAttributes(
      this IUserService userService,
      IVssRequestContext requestContext,
      Guid userId,
      IList<SetUserAttributeParameters> attributeParametersList)
    {
      return userService.UserServiceInternal().SetAttributes(requestContext, userId, attributeParametersList);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void DeleteAttribute(
      this IUserService userService,
      IVssRequestContext requestContext,
      Guid userId,
      string attributeName)
    {
      userService.UserServiceInternal().DeleteAttribute(requestContext, userId, attributeName);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Avatar GetAvatar(
      this IUserService userService,
      IVssRequestContext requestContext,
      Guid userId,
      AvatarSize size = AvatarSize.Medium)
    {
      return userService.UserServiceInternal().GetAvatar(requestContext, userId, size);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void UpdateAvatar(
      this IUserService userService,
      IVssRequestContext requestContext,
      Guid userId,
      Avatar avatar)
    {
      userService.UserServiceInternal().UpdateAvatar(requestContext, userId, avatar);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void DeleteAvatar(
      this IUserService userService,
      IVssRequestContext requestContext,
      Guid userId)
    {
      userService.UserServiceInternal().DeleteAvatar(requestContext, userId);
    }

    private static IUserServiceInternal UserServiceInternal(this IUserService userService) => userService is IUserServiceInternal userServiceInternal ? userServiceInternal : throw new InvalidCastException("Attempt to cast IUserService to IUserServiceInternal failed. This exception will only occur in misbehaved or incomplete test code. You need to fix your test like this:\r\nvar userService = new Mock<IUserService>();\r\nmockProfileService = userService.As<IUserServiceInternal>();\r\nrequestContext.SetupService<IUserService>(() => userService.Object);");
  }
}
