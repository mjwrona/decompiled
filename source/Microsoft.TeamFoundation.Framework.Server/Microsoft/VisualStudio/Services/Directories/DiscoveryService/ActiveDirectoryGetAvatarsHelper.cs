// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.ActiveDirectoryGetAvatarsHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class ActiveDirectoryGetAvatarsHelper
  {
    internal static DirectoryInternalGetAvatarsResponse GetAvatars(
      IVssRequestContext context,
      DirectoryInternalGetAvatarsRequest request)
    {
      if (!ActiveDirectoryRequestFilter.AllowGetAvatarsRequest(context, (DirectoryInternalRequest) request))
        return new DirectoryInternalGetAvatarsResponse();
      DirectoryInternalGetAvatarsResponse avatars = new DirectoryInternalGetAvatarsResponse()
      {
        Results = (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetAvatarResult>) request.ObjectIds.ToDictionary<DirectoryEntityIdentifier, DirectoryEntityIdentifier, DirectoryInternalGetAvatarResult>((Func<DirectoryEntityIdentifier, DirectoryEntityIdentifier>) (id => id), (Func<DirectoryEntityIdentifier, DirectoryInternalGetAvatarResult>) (id => (DirectoryInternalGetAvatarResult) null))
      };
      ActiveDirectoryGetAvatarsHelper.GetAvatars(context, avatars.Results);
      return avatars;
    }

    private static void GetAvatars(
      IVssRequestContext context,
      IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetAvatarResult> results)
    {
      Dictionary<DirectoryEntityIdentifier, string> source = new Dictionary<DirectoryEntityIdentifier, string>((IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer);
      foreach (DirectoryEntityIdentifier entityIdentifier in results.Keys.ToList<DirectoryEntityIdentifier>())
      {
        string objectSid;
        if (ActiveDirectoryHelper.TryGetObjectSid(entityIdentifier, out objectSid) && DirectoryEntityHelper.IsActiveDirectoryUser(entityIdentifier))
          source.Add(entityIdentifier, objectSid);
      }
      if (source.Count == 0)
        return;
      IDictionary<string, byte[]> thumbnailPhotos = ActiveDirectoryHelper.Instance.GetThumbnailPhotos(context, (IList<string>) source.Select<KeyValuePair<DirectoryEntityIdentifier, string>, string>((Func<KeyValuePair<DirectoryEntityIdentifier, string>, string>) (kvp => kvp.Value)).ToList<string>());
      foreach (KeyValuePair<DirectoryEntityIdentifier, string> keyValuePair in source)
      {
        DirectoryEntityIdentifier key1 = keyValuePair.Key;
        string key2 = keyValuePair.Value;
        byte[] numArray = (byte[]) null;
        if (thumbnailPhotos.TryGetValue(key2, out numArray))
          results[key1] = new DirectoryInternalGetAvatarResult()
          {
            Image = numArray
          };
      }
    }
  }
}
