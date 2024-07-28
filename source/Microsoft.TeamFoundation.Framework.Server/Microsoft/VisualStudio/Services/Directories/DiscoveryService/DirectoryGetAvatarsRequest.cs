// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.DirectoryGetAvatarsRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  public class DirectoryGetAvatarsRequest : DirectoryRequest
  {
    public IEnumerable<string> ObjectIds { get; set; }

    internal override DirectoryResponse Execute(
      IVssRequestContext context,
      IEnumerable<IDirectory> directories)
    {
      string[] strArray = this.SanitizeAndFilterDirectories(context);
      IList<KeyValuePair<string, DirectoryEntityIdentifier>> source = DirectoryEntityIdentifier.TryParse(this.GetObjectIds());
      Dictionary<DirectoryEntityIdentifier, DirectoryInternalGetAvatarResult> decodedIdToResult = source.Where<KeyValuePair<string, DirectoryEntityIdentifier>>((Func<KeyValuePair<string, DirectoryEntityIdentifier>, bool>) (kvp => kvp.Value != null)).ToDictionary<KeyValuePair<string, DirectoryEntityIdentifier>, DirectoryEntityIdentifier, DirectoryInternalGetAvatarResult>((Func<KeyValuePair<string, DirectoryEntityIdentifier>, DirectoryEntityIdentifier>) (kvp => kvp.Value), (Func<KeyValuePair<string, DirectoryEntityIdentifier>, DirectoryInternalGetAvatarResult>) (kvp => (DirectoryInternalGetAvatarResult) null));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      IDirectory directory1 = directories.Where<IDirectory>(DirectoryGetAvatarsRequest.\u003C\u003EO.\u003C0\u003E__IsVisualStudioDirectory ?? (DirectoryGetAvatarsRequest.\u003C\u003EO.\u003C0\u003E__IsVisualStudioDirectory = new Func<IDirectory, bool>(DirectoryUtils.IsVisualStudioDirectory))).First<IDirectory>();
      IVssRequestContext context1 = context;
      DirectoryInternalGetAvatarsRequest request1 = new DirectoryInternalGetAvatarsRequest();
      request1.Directories = (IEnumerable<string>) strArray;
      request1.ObjectIds = (IEnumerable<DirectoryEntityIdentifier>) decodedIdToResult.Select<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetAvatarResult>, DirectoryEntityIdentifier>((Func<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetAvatarResult>, DirectoryEntityIdentifier>) (kvp => kvp.Key)).ToList<DirectoryEntityIdentifier>();
      DirectoryInternalGetAvatarsResponse avatars1 = directory1.GetAvatars(context1, request1);
      if (avatars1.Results != null)
      {
        foreach (KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetAvatarResult> result in (IEnumerable<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetAvatarResult>>) avatars1.Results)
        {
          if (result.Value != null)
            decodedIdToResult[result.Key] = result.Value;
        }
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      foreach (IDirectory directory2 in directories.Where<IDirectory>(DirectoryGetAvatarsRequest.\u003C\u003EO.\u003C1\u003E__IsNotVisualStudioDirectory ?? (DirectoryGetAvatarsRequest.\u003C\u003EO.\u003C1\u003E__IsNotVisualStudioDirectory = new Func<IDirectory, bool>(DirectoryUtils.IsNotVisualStudioDirectory))))
      {
        IVssRequestContext context2 = context;
        DirectoryInternalGetAvatarsRequest request2 = new DirectoryInternalGetAvatarsRequest();
        request2.Directories = (IEnumerable<string>) strArray;
        request2.ObjectIds = (IEnumerable<DirectoryEntityIdentifier>) decodedIdToResult.Where<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetAvatarResult>>((Func<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetAvatarResult>, bool>) (kvp => kvp.Value == null)).Select<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetAvatarResult>, DirectoryEntityIdentifier>((Func<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetAvatarResult>, DirectoryEntityIdentifier>) (kvp => kvp.Key)).ToList<DirectoryEntityIdentifier>();
        DirectoryInternalGetAvatarsResponse avatars2 = directory2.GetAvatars(context2, request2);
        if (avatars2.Results != null)
        {
          foreach (KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetAvatarResult> result in (IEnumerable<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetAvatarResult>>) avatars2.Results)
          {
            if (result.Value != null && result.Value.Image != null)
              decodedIdToResult[result.Key] = result.Value;
          }
        }
      }
      return (DirectoryResponse) new DirectoryGetAvatarsResponse()
      {
        Results = (IDictionary<string, byte[]>) source.ToDictionary<KeyValuePair<string, DirectoryEntityIdentifier>, string, byte[]>((Func<KeyValuePair<string, DirectoryEntityIdentifier>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, DirectoryEntityIdentifier>, byte[]>) (kvp =>
        {
          DirectoryInternalGetAvatarResult internalGetAvatarResult = (DirectoryInternalGetAvatarResult) null;
          if (kvp.Value == null || !decodedIdToResult.TryGetValue(kvp.Value, out internalGetAvatarResult))
            return (byte[]) null;
          return internalGetAvatarResult?.Image;
        }))
      };
    }

    private IEnumerable<string> GetObjectIds() => this.ObjectIds == null ? (IEnumerable<string>) Array.Empty<string>() : (IEnumerable<string>) new HashSet<string>(this.ObjectIds);
  }
}
