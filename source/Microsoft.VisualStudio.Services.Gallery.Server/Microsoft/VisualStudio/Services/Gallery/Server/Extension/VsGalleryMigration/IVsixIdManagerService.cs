// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.VsGalleryMigration.IVsixIdManagerService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.VsGalleryMigration
{
  [DefaultServiceImplementation(typeof (VsixIdManagerService))]
  internal interface IVsixIdManagerService : IVssFrameworkService
  {
    bool IsExistingVsixId(IVssRequestContext requestContext, string vsixId);

    bool IsReservedVsixId(
      IVssRequestContext requestContext,
      string vsixId,
      ReservedVsixIdPurposeType purpose);

    void AddReservedVsixId(IVssRequestContext requestContext, ReservedVsixId reservedVsixId);

    void DeleteReservedVsixId(
      IVssRequestContext requestContext,
      string vsixId,
      ReservedVsixIdPurposeType purpose);

    IEnumerable<ReservedVsixId> GetReservedVsixIdsByUserId(
      IVssRequestContext requestContext,
      Guid userId);

    int AnonymizeReservedVsixIds(IVssRequestContext requestContext, Guid userId);
  }
}
