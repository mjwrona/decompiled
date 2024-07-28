// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.CollectionAvatarService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class CollectionAvatarService : ICollectionAvatarService, IVssFrameworkService
  {
    private readonly SpsClientHelper m_spsClientHelper;
    private const string c_area = "CollectionAvatarService";
    private const string c_layer = "CollectionAvatarService";

    public CollectionAvatarService() => this.m_spsClientHelper = new SpsClientHelper();

    public CollectionAvatarService(SpsClientHelper spsClientHelper) => this.m_spsClientHelper = spsClientHelper;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckHostedDeployment();
      requestContext.CheckDeploymentRequestContext();
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public string CalculateCollectionAvatarEtag(Collection collection)
    {
      DateTime? nullable = collection.Properties.GetValue<DateTime?>("SystemProperty.AvatarTimestamp", new DateTime?());
      return AvatarUtils.GetETag(nullable.HasValue ? new DateTimeOffset(nullable.Value) : DateTimeOffset.MinValue).ToString();
    }

    public byte[] GetCollectionAvatarData(
      IVssRequestContext requestContext,
      Collection collection,
      ImageSize size = ImageSize.Small)
    {
      Collection collection1 = collection;
      if (!collection.Properties.ContainsKey("SystemProperty.AvatarData"))
        collection1 = this.GetCollectionForRequestIdentityByCollectionId(requestContext, collection.Id, new string[1]
        {
          "SystemProperty.AvatarData"
        });
      byte[] buffer = collection1.Properties.GetValue<byte[]>("SystemProperty.AvatarData", (byte[]) null);
      if (buffer == null)
        return AvatarUtils.GenerateAvatar(collection1.Name, PaletteAlgorithm.CollectionPalette, ImageSize.Small, AvatarImageFormat.Png);
      int avatarSizeInPixels = CollectionAvatarService.MapToAvatarSizeInPixels(size);
      using (MemoryStream imageDataToResize = new MemoryStream(buffer))
        return ImageResizeUtils.ResizeWhileMaintainingAspectRatio((Stream) imageDataToResize, avatarSizeInPixels, AvatarImageFormat.Png);
    }

    [TfsTraceFilter(505290, 505295)]
    public Collection GetCollectionForRequestIdentityByCollectionId(
      IVssRequestContext requestContext,
      Guid collectionId,
      string[] extendedProperties)
    {
      try
      {
        return this.m_spsClientHelper.CreateSpsClient(requestContext, collectionId).GetCollectionAsync("Me", (IEnumerable<string>) extendedProperties).SyncResult<Collection>();
      }
      catch (HostDoesNotExistException ex)
      {
        requestContext.Trace(505291, TraceLevel.Verbose, nameof (CollectionAvatarService), nameof (CollectionAvatarService), string.Format("Host {0} does not exist", (object) collectionId));
        return (Collection) null;
      }
      catch (IdentityNotFoundException ex)
      {
        requestContext.Trace(505292, TraceLevel.Verbose, nameof (CollectionAvatarService), nameof (CollectionAvatarService), string.Format("Identity has no access to collection {0}", (object) collectionId));
        return (Collection) null;
      }
    }

    private static int MapToAvatarSizeInPixels(ImageSize size)
    {
      switch (size)
      {
        case ImageSize.Small:
          return 34;
        case ImageSize.Medium:
          return 44;
        case ImageSize.Large:
          return 220;
        default:
          throw new ArgumentException("The argument is out of the supported enum range", nameof (size));
      }
    }
  }
}
