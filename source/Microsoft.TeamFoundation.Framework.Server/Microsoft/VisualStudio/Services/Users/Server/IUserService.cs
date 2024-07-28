// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.IUserService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Users.Server
{
  [DefaultServiceImplementation(typeof (IUserServiceInternal))]
  public interface IUserService : IVssFrameworkService
  {
    User GetUser(IVssRequestContext requestContext, SubjectDescriptor descriptor);

    User CreateUser(IVssRequestContext requestContext, CreateUserParameters userParameters);

    User UpdateUser(IVssRequestContext requestContext, UpdateUserParameters userParameters);

    UserAttribute GetAttribute(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      string attributeName);

    IList<UserAttribute> QueryAttributes(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      string queryPattern,
      DateTimeOffset? modifiedAfter = null);

    IList<UserAttribute> SetAttributes(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      IList<SetUserAttributeParameters> attributeParametersList);

    void DeleteAttribute(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      string attributeName);

    Avatar GetAvatar(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      AvatarSize size = AvatarSize.Medium);

    void UpdateAvatar(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      Avatar avatar);

    void DeleteAvatar(IVssRequestContext requestContext, SubjectDescriptor descriptor);

    IList<AccessedHost> GetMostRecentlyAccessedHosts(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor);

    void UpdateMostRecentlyAccessedHosts(
      IVssRequestContext requestContext,
      IList<AccessedHostsParameters> parametersList);

    void EnableUserProfileDataSync(IVssRequestContext requestContext, SubjectDescriptor descriptor);

    void DisableUserProfileDataSync(IVssRequestContext requestContext, SubjectDescriptor descriptor);
  }
}
