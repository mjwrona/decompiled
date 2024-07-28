// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.AadBindPendingConversionHelper
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
  internal class AadBindPendingConversionHelper : BindPendingConversionHelper
  {
    private const string c_layer = "AadBindPendingConversionHelper";

    public AadBindPendingConversionHelper(
      IdentityConversionPayload identityConversionPayload)
      : base(identityConversionPayload)
    {
      this._prefix = "L_" + this._identityConversionPayload.EventId.ToString();
    }

    protected override void CheckAndFixCollision(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      string property = identity.GetProperty<string>("Account", (string) null);
      IdentityDescriptor aadDescriptor = BindPendingConversionHelper.GetAadDescriptor(property, this._identityConversionPayload.TenantId.ToString());
      IdentityDescriptor aadBindPendingDescriptor = BindPendingConversionHelper.GetAadBindPendingDescriptor(property, this._identityConversionPayload.TenantId.ToString());
      if ((aadDescriptor.Equals(identity.Descriptor) ? 1 : (aadBindPendingDescriptor.Equals(identity.Descriptor) ? 1 : 0)) == 0)
      {
        this._identityDescriptors.Add(aadDescriptor);
        this._identityDescriptors.Add(aadBindPendingDescriptor);
      }
      if (!this._identityConversionPayload.InactiveIdentities.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
        return;
      this._identityConversionPayload.InactiveIdentities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i.Descriptor.Equals(aadDescriptor) || i.Descriptor.Equals(aadBindPendingDescriptor))).ForEach<Microsoft.VisualStudio.Services.Identity.Identity>((Action<Microsoft.VisualStudio.Services.Identity.Identity>) (i => this.MungeIdentity(requestContext, i)));
    }

    protected override void ConvertToBindPending(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      string property = identity.GetProperty<string>("Account", (string) null);
      identity.Descriptor = BindPendingConversionHelper.GetAadBindPendingDescriptor(property, this._identityConversionPayload.TenantId.ToString());
      identity.SetProperty("Domain", (object) this._identityConversionPayload.TenantId.ToString());
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
        SubjectType = "aad"
      });
    }
  }
}
