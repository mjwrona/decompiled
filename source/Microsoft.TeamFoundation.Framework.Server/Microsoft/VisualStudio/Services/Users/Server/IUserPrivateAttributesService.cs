// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.IUserPrivateAttributesService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Users.Server
{
  [DefaultServiceImplementation(typeof (FrameworkUserPrivateAttributesService))]
  public interface IUserPrivateAttributesService : IVssFrameworkService
  {
    UserAttribute GetPrivateAttribute(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      string attributeName);

    UserAttribute GetPrivateAttribute(
      IVssRequestContext requestContext,
      Guid userId,
      string attributeName);

    IList<UserAttribute> QueryPrivateAttributes(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      string queryPattern,
      DateTimeOffset? modifiedAfter = null);

    IList<UserAttribute> QueryPrivateAttributes(
      IVssRequestContext requestContext,
      Guid userId,
      string queryPattern,
      DateTimeOffset? modifiedAfter = null);

    IList<UserAttribute> SetPrivateAttributes(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      IList<SetUserAttributeParameters> attributeParametersList);

    IList<UserAttribute> SetPrivateAttributes(
      IVssRequestContext requestContext,
      Guid userId,
      IList<SetUserAttributeParameters> attributeParametersList);

    void DeletePrivateAttribute(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      string attributeName);

    void DeletePrivateAttribute(
      IVssRequestContext requestContext,
      Guid userId,
      string attributeName);
  }
}
