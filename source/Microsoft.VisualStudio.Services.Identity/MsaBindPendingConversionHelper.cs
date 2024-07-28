// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.MsaBindPendingConversionHelper
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class MsaBindPendingConversionHelper : BindPendingConversionHelper
  {
    private const string c_layer = "MsaBindPendingConversionHelper";

    public MsaBindPendingConversionHelper(
      IdentityConversionPayload identityConversionPayload)
      : base(identityConversionPayload)
    {
      this._prefix = "U_" + this._identityConversionPayload.EventId.ToString();
    }

    protected override void CheckAndFixCollision(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      IdentityDescriptor msaBindPendingDescriptor = BindPendingConversionHelper.GetMsaBindPendingDescriptor(identity.GetProperty<string>("Account", (string) null));
      if (!msaBindPendingDescriptor.Equals(identity.Descriptor))
        this._identityDescriptors.Add(msaBindPendingDescriptor);
      if (!this._identityConversionPayload.InactiveIdentities.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
        return;
      this._identityConversionPayload.InactiveIdentities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i.Descriptor.Equals(msaBindPendingDescriptor) || MsaBindPendingConversionHelper.WillIdentitiesCollideByUPN(identity, i))).ForEach<Microsoft.VisualStudio.Services.Identity.Identity>((Action<Microsoft.VisualStudio.Services.Identity.Identity>) (i => this.MungeIdentity(requestContext, i)));
    }

    private static bool WillIdentitiesCollideByUPN(Microsoft.VisualStudio.Services.Identity.Identity sourceIdentity, Microsoft.VisualStudio.Services.Identity.Identity targetIdentity)
    {
      string property1 = sourceIdentity.GetProperty<string>("Account", (string) null);
      string property2 = targetIdentity.GetProperty<string>("Account", (string) null);
      return targetIdentity.GetProperty<string>("Domain", (string) null).Equals("Windows Live ID", StringComparison.OrdinalIgnoreCase) && property1.Equals(property2, StringComparison.OrdinalIgnoreCase);
    }

    protected override void ConvertToBindPending(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      string property = identity.GetProperty<string>("Account", (string) null);
      identity.Descriptor = BindPendingConversionHelper.GetMsaBindPendingDescriptor(property);
      identity.SetProperty("Domain", (object) "Windows Live ID");
      identity.SetProperty("http://schemas.microsoft.com/identity/claims/objectidentifier", (object) null);
      DateTime utcNow;
      if (!identity.Properties.TryGetValue<DateTime>("MetadataUpdateDate", out utcNow))
        utcNow = DateTime.UtcNow;
      identity.SetProperty("MetadataUpdateDate", (object) utcNow.AddSeconds(1.0));
      identity.SetProperty("PUID", (object) string.Empty);
    }

    protected override void AddMungedCuid(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      Guid zero = GuidUtils.Change14thCharToZero(Guid.NewGuid());
      this._identityKeyMaps.Add(new IdentityKeyMap()
      {
        Cuid = zero,
        StorageKey = identity.Id,
        SubjectType = "msa"
      });
    }
  }
}
