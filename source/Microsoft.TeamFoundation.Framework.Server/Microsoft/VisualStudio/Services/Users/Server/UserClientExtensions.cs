// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.UserClientExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Users.Server
{
  public static class UserClientExtensions
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IList<UserAttribute> QueryAttributes(
      this FrameworkUserHttpClient client,
      Guid userId,
      string queryPattern,
      DateTimeOffset? modifiedAfter = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      UserAttributes userAttributes = client.QueryAttributesAsync(userId, (string) null, queryPattern, modifiedAfter, cancellationToken: cancellationToken).SyncResult<UserAttributes>();
      IList<UserAttribute> attributes = userAttributes.Attributes;
      while (userAttributes.ContinuationToken != null)
      {
        userAttributes = client.QueryAttributesAsync(userId, userAttributes.ContinuationToken, queryPattern, modifiedAfter, cancellationToken: cancellationToken).SyncResult<UserAttributes>();
        attributes.AddRange<UserAttribute, IList<UserAttribute>>((IEnumerable<UserAttribute>) userAttributes.Attributes);
      }
      return attributes;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IList<UserAttribute> QueryAttributes(
      this FrameworkUserHttpClient client,
      SubjectDescriptor descriptor,
      string queryPattern,
      DateTimeOffset? modifiedAfter = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      UserAttributes userAttributes = client.QueryAttributesAsync(descriptor, (string) null, queryPattern, modifiedAfter, cancellationToken: cancellationToken).SyncResult<UserAttributes>();
      IList<UserAttribute> attributes = userAttributes.Attributes;
      while (userAttributes.ContinuationToken != null)
      {
        userAttributes = client.QueryAttributesAsync(descriptor, userAttributes.ContinuationToken, queryPattern, modifiedAfter, cancellationToken: cancellationToken).SyncResult<UserAttributes>();
        attributes.AddRange<UserAttribute, IList<UserAttribute>>((IEnumerable<UserAttribute>) userAttributes.Attributes);
      }
      return attributes;
    }
  }
}
