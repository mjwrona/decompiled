// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.VsGalleryMigration.VsixIdManagerService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.VsGalleryMigration
{
  internal class VsixIdManagerService : IVsixIdManagerService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool IsExistingVsixId(IVssRequestContext requestContext, string vsixId)
    {
      using (VsixIdManagerComponent component = requestContext.CreateComponent<VsixIdManagerComponent>())
        return component.DoesMetadataPairExist("VsixId", vsixId);
    }

    public bool IsReservedVsixId(
      IVssRequestContext requestContext,
      string vsixId,
      ReservedVsixIdPurposeType purpose)
    {
      List<ReservedVsixId> reservedVsixIdList;
      using (VsixIdManagerComponent component = requestContext.CreateComponent<VsixIdManagerComponent>())
        reservedVsixIdList = component.QueryReservedVsixIds(vsixId, purpose);
      return !reservedVsixIdList.IsNullOrEmpty<ReservedVsixId>() && reservedVsixIdList.Any<ReservedVsixId>((Func<ReservedVsixId, bool>) (x => x.VsixId.Equals(vsixId, StringComparison.OrdinalIgnoreCase) && x.Purpose == purpose));
    }

    public void AddReservedVsixId(IVssRequestContext requestContext, ReservedVsixId reservedVsixId)
    {
      using (VsixIdManagerComponent component = requestContext.CreateComponent<VsixIdManagerComponent>())
        component.AddReservedVsixId(reservedVsixId.VsixId, reservedVsixId.Purpose, reservedVsixId.UserId);
    }

    public void DeleteReservedVsixId(
      IVssRequestContext requestContext,
      string vsixId,
      ReservedVsixIdPurposeType purpose)
    {
      using (VsixIdManagerComponent component = requestContext.CreateComponent<VsixIdManagerComponent>())
        component.DeleteReservedVsixId(vsixId, purpose);
    }

    public IEnumerable<ReservedVsixId> GetReservedVsixIdsByUserId(
      IVssRequestContext requestContext,
      Guid userId)
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      IEnumerable<ReservedVsixId> reservedVsixIdsByUserId = (IEnumerable<ReservedVsixId>) null;
      using (VsixIdManagerComponent component = requestContext.CreateComponent<VsixIdManagerComponent>())
      {
        if (component is VsixIdManagerComponent2 managerComponent2)
          reservedVsixIdsByUserId = managerComponent2.GetReservedVsixIdsByUserId(userId.ToString("D").ToUpperInvariant());
      }
      return reservedVsixIdsByUserId;
    }

    public int AnonymizeReservedVsixIds(IVssRequestContext requestContext, Guid userId)
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      int num = 0;
      using (VsixIdManagerComponent component = requestContext.CreateComponent<VsixIdManagerComponent>())
      {
        if (component is VsixIdManagerComponent2 managerComponent2)
          num = managerComponent2.AnonymizeReservedVsixIds(userId.ToString("D").ToUpperInvariant());
      }
      return num;
    }
  }
}
