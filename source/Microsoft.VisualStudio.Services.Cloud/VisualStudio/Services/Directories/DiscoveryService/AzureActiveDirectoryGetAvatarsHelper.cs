// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.AzureActiveDirectoryGetAvatarsHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class AzureActiveDirectoryGetAvatarsHelper
  {
    internal static DirectoryInternalGetAvatarsResponse GetAvatars(
      IVssRequestContext context,
      DirectoryInternalGetAvatarsRequest request)
    {
      AadService aadService = context.GetService<AadService>();
      return new DirectoryInternalGetAvatarsResponse()
      {
        Results = (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetAvatarResult>) request.ObjectIds.ToDictionary<DirectoryEntityIdentifier, DirectoryEntityIdentifier, DirectoryInternalGetAvatarResult>((Func<DirectoryEntityIdentifier, DirectoryEntityIdentifier>) (objectId => objectId), (Func<DirectoryEntityIdentifier, DirectoryInternalGetAvatarResult>) (objectId =>
        {
          if (objectId.Version != 1)
            return (DirectoryInternalGetAvatarResult) null;
          if (!(objectId is DirectoryEntityIdentifierV1 entityIdentifierV1_2))
            return (DirectoryInternalGetAvatarResult) null;
          if (!"aad".Equals(entityIdentifierV1_2.Source))
            return (DirectoryInternalGetAvatarResult) null;
          if (!"user".Equals(entityIdentifierV1_2.Type))
            return (DirectoryInternalGetAvatarResult) null;
          Guid result;
          if (!Guid.TryParseExact(entityIdentifierV1_2.Id, "N", out result))
            return (DirectoryInternalGetAvatarResult) null;
          GetUserThumbnailResponse userThumbnail = aadService.GetUserThumbnail<Guid>(context, new GetUserThumbnailRequest<Guid>()
          {
            Identifier = result
          });
          return new DirectoryInternalGetAvatarResult()
          {
            Image = userThumbnail.Thumbnail
          };
        }))
      };
    }
  }
}
