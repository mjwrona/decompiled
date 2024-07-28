// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.IUserServiceInternal
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Users.Server
{
  [DefaultServiceImplementation(typeof (FrameworkUserService))]
  internal interface IUserServiceInternal : IUserService, IVssFrameworkService
  {
    User CreateUser(IVssRequestContext requestContext, InternalCreateUserParameters userParameters);

    User GetUser(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      bool createIfNotExists);

    User GetUser(IVssRequestContext requestContext, Guid userId);

    User UpdateUser(
      IVssRequestContext requestContext,
      Guid userId,
      UpdateUserParameters userParameters);

    UserAttribute GetAttribute(
      IVssRequestContext requestContext,
      Guid userId,
      string attributeName);

    IList<UserAttribute> QueryAttributes(
      IVssRequestContext requestContext,
      Guid userId,
      string queryPattern,
      DateTimeOffset? modifiedAfter = null);

    IList<UserAttribute> SetAttributes(
      IVssRequestContext requestContext,
      Guid userId,
      IList<SetUserAttributeParameters> attributeParametersList);

    void DeleteAttribute(IVssRequestContext requestContext, Guid userId, string attributeName);

    Avatar GetAvatar(IVssRequestContext requestContext, Guid userId, AvatarSize size = AvatarSize.Medium);

    void UpdateAvatar(IVssRequestContext requestContext, Guid userId, Avatar avatar);

    void DeleteAvatar(IVssRequestContext requestContext, Guid userId);

    Avatar CreateAvatarPreview(
      IVssRequestContext requestContext,
      Guid userId,
      Avatar avatar,
      AvatarSize size = AvatarSize.Medium,
      string displayName = null);
  }
}
