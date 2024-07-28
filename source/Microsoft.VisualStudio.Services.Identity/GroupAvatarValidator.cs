// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupAvatarValidator
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class GroupAvatarValidator
  {
    private const string c_area = "IdentityService";
    private const string c_layer = "GroupAvatarValidator";
    private const string GroupAvatarContentValidation = "VisualStudio.Services.Identity.GroupAvatarContentValidation";

    internal static void ValidateGroupAvatarsAsync(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.GroupAvatarContentValidation"))
        return;
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) identities, nameof (identities));
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => IdentityValidation.IsTeamFoundationType(identity.Descriptor))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>())
      {
        object obj;
        int num = identity.TryGetProperty("Microsoft.TeamFoundation.Identity.Image.Data", out obj) ? 1 : 0;
        byte[] imageData = obj as byte[];
        if (num != 0 && imageData != null)
          GroupAvatarValidator.ValidateGroupAvatarAsync(requestContext, identity, imageData);
      }
    }

    internal static void ValidateGroupAvatarAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      byte[] imageData)
    {
      string ipAddress = requestContext.RemoteIPAddress();
      requestContext.Fork((Func<IVssRequestContext, Task>) (async rc =>
      {
        try
        {
          string hostRegion = rc.GetService<IHostRegionService>().GetHostRegion(rc);
          requestContext.TraceAlways(80275, TraceLevel.Info, "IdentityService", nameof (GroupAvatarValidator), "Avatar will be validated for Identity: {0} Region: {1}", (object) identity.Id, (object) hostRegion);
          IVssRequestContext vssRequestContext = rc.To(TeamFoundationHostType.Deployment);
          await vssRequestContext.GetService<IContentValidationService>().SubmitProfileImageAsync(vssRequestContext, imageData, identity, ipAddress, hostRegion);
        }
        catch (Exception ex)
        {
          rc.TraceException(80276, "IdentityService", nameof (GroupAvatarValidator), ex);
        }
      }), nameof (ValidateGroupAvatarAsync));
    }
  }
}
