// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitAnnotatedTagsHelper
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Protocol;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class GitAnnotatedTagsHelper
  {
    public static Sha1Id CreateTag(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      string tagName,
      Sha1Id taggedObjectId,
      string message)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ITeamFoundationIdentityService service = requestContext.GetService<ITeamFoundationIdentityService>();
      IdentityAndDate identityAndDate = IdentityHelper.Instance.GetIdentityAndDate(requestContext, service, requestContext.GetUserId());
      return GitAnnotatedTagsHelper.CreateTag(requestContext, repository, tagName, taggedObjectId, identityAndDate, message);
    }

    public static Sha1Id CreateTag(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      string tagName,
      Sha1Id taggedObjectId,
      IdentityAndDate userIdentityDate,
      string message)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repository, nameof (repository));
      ArgumentUtility.CheckForNull<string>(tagName, nameof (tagName));
      ArgumentUtility.CheckForNull<IdentityAndDate>(userIdentityDate, nameof (userIdentityDate));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(message, nameof (message));
      string str1 = "refs/tags/" + tagName;
      if (!RefUtil.IsValidRefName(str1, true))
        throw new InvalidGitRefNameException(str1);
      if (repository.Refs.MatchingName(str1) != null)
        throw new GitRefNameAlreadyExistsException(str1);
      GitObjectType objectType = repository.LookupObjectType(taggedObjectId);
      Stream tempPackStream;
      PackAndRefIngester packAndRefIngester = repository.CreatePackAndRefIngester(out tempPackStream);
      using (tempPackStream)
      {
        byte[] tagBytes;
        Sha1Id annotatedTag = TagBuilder.CreateAnnotatedTag(taggedObjectId, objectType.GetPackType(), tagName, IdentityAndDate.CreateIdentityString(userIdentityDate.Name, userIdentityDate.Email, userIdentityDate.Time, userIdentityDate.BaseUtcOffset), message, out tagBytes);
        using (GitPackSerializer gitPackSerializer = new GitPackSerializer(tempPackStream, 1, true))
        {
          using (MemoryStream sourceStream = new MemoryStream(tagBytes))
            gitPackSerializer.AddInflatedStreamWithTypeAndSize((Stream) sourceStream, GitPackObjectType.Tag, (long) tagBytes.Length);
          gitPackSerializer.Complete();
        }
        tempPackStream.Seek(0L, SeekOrigin.Begin);
        packAndRefIngester.AddRefUpdateRequest(str1, Sha1Id.Empty, annotatedTag);
        TfsGitRefUpdateResultSet refUpdateResultSet = packAndRefIngester.Ingest();
        if (refUpdateResultSet.CountSucceeded == 0)
        {
          TfsGitRefUpdateResult result = refUpdateResultSet.Results[0];
          object[] objArray = new object[4]
          {
            (object) result.Name,
            (object) Resources.Get("RefRejectedByPlugin"),
            null,
            null
          };
          string str2;
          if (!string.IsNullOrEmpty(result.RejectedBy))
            str2 = Resources.Format("PluginName", (object) result.RejectedBy);
          else
            str2 = string.Empty;
          objArray[2] = (object) str2;
          string str3;
          if (!string.IsNullOrEmpty(result.CustomMessage))
            str3 = Resources.Format("CustomMessage", (object) result.CustomMessage);
          else
            str3 = string.Empty;
          objArray[3] = (object) str3;
          throw new GitFailedToCreateTagException(string.Format("{0} {1}. {2} {3}", objArray));
        }
        return annotatedTag;
      }
    }
  }
}
