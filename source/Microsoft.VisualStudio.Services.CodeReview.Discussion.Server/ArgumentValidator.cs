// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.ArgumentValidator
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  internal static class ArgumentValidator
  {
    internal static void CheckNull(object obj, string argument)
    {
      if (obj == null)
        throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArgumentNullError, (object) argument));
    }

    internal static void CheckNull(string str, string argument)
    {
      if (string.IsNullOrWhiteSpace(str))
        throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArgumentNullError, (object) argument));
    }

    internal static void Check(bool condition, string errorMessage)
    {
      if (!condition)
        throw new ArgumentException(errorMessage);
    }

    internal static void CheckNullCollection<T>(
      IEnumerable<T> objs,
      string argument,
      bool allowsEmpty = true)
    {
      foreach (T obj in objs)
        ArgumentValidator.CheckNull((object) obj, argument);
      if (objs.Count<T>() == 0 && !allowsEmpty)
        throw new ArgumentException(argument);
    }

    internal static void ValidateInput(DiscussionThread thread, int commentId)
    {
      ArgumentUtility.CheckForNull<DiscussionThread>(thread, nameof (thread));
      ArgumentUtility.CheckForOutOfRange(thread.DiscussionId, "DiscussionId", 1);
      ArgumentUtility.CheckStringForNullOrEmpty(thread.ArtifactUri, "ArtifactUri");
      ArgumentUtility.CheckForOutOfRange(commentId, nameof (commentId), 1);
    }

    internal static void ValidateUsers(IVssRequestContext requestContext, List<IdentityRef> users)
    {
      if (users == null)
        return;
      ArgumentValidator.CheckNullCollection<IdentityRef>((IEnumerable<IdentityRef>) users, nameof (users));
      List<Guid> uniqueUsers = new List<Guid>();
      foreach (IdentityRef user in users)
      {
        Guid result;
        if (string.IsNullOrEmpty(user.Id) || !Guid.TryParse(user.Id, out result))
          throw new ArgumentException(Resources.UserIdMustBeAGuid);
        if (uniqueUsers.Contains(result))
          throw new ArgumentException(Resources.UserIdentitiesMustBeUnique);
        uniqueUsers.Add(result);
      }
      ArgumentValidator.ValidateUserIdentities(requestContext, uniqueUsers);
    }

    internal static void ValidateComment(DiscussionComment comment)
    {
      ArgumentValidator.CheckNull((object) comment, nameof (comment));
      ArgumentValidator.CheckNull(comment.Content, "comment.Content");
      if (comment.Content.Length > 150000)
        throw new ArgumentOutOfRangeException("comment.Content", string.Format(Resources.DiscussionCommentWithTooMuchContent, (object) 150000));
    }

    internal static void ValidateUserIdentities(
      IVssRequestContext requestContext,
      List<Guid> uniqueUsers)
    {
      IList<Guid> invalidIdentities = IdentityHelper.GetInvalidIdentities(requestContext, (IList<Guid>) uniqueUsers);
      if (invalidIdentities.Count > 0)
        throw new ArgumentException(string.Format(Resources.InvalidVSOIdentitiesAreFound, (object) string.Join<Guid>(",", (IEnumerable<Guid>) invalidIdentities)), "users");
    }
  }
}
