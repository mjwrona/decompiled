// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IContentValidationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface IContentValidationService : IVssFrameworkService
  {
    bool IsEnabled(IVssRequestContext rc, string regionName = null);

    Task SubmitAsync(
      IVssRequestContext rc,
      Guid projectId,
      IEnumerable<ContentValidationKey> toScan,
      Microsoft.VisualStudio.Services.Identity.Identity contentCreator,
      string creatorIpAddress);

    Task SubmitAsync(
      IVssRequestContext rc,
      Guid projectId,
      string base64EncodedContent,
      ContentValidationScanType scanType,
      string fileName,
      Microsoft.VisualStudio.Services.Identity.Identity contentCreator,
      string creatorIpAddress);

    Task SubmitAsync(
      IVssRequestContext rc,
      Guid projectId,
      byte[] rawContent,
      Microsoft.VisualStudio.Services.Identity.Identity contentCreator,
      string creatorIpAddress,
      string fileName,
      ContentValidationScanType? scanType);

    Task SubmitProfileImageAsync(
      IVssRequestContext deploymentRc,
      byte[] profileImageContent,
      Microsoft.VisualStudio.Services.Identity.Identity profileIdentity,
      string lastUsedIpAddress,
      string regionName);

    Task SubmitImageAsync(
      IVssRequestContext deploymentRc,
      byte[] imageContent,
      Microsoft.VisualStudio.Services.Identity.Identity contentAuthor,
      string lastUsedIpAddress,
      string regionName,
      ContentValidationTakedownTarget takedownTarget);
  }
}
