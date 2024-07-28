// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityExtendedPropertyKeyBuilder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentityExtendedPropertyKeyBuilder
  {
    private Guid _identityId;
    private readonly string _propertyName;

    public IdentityExtendedPropertyKeyBuilder(Guid identityId, string propertyName)
    {
      this._identityId = identityId;
      this._propertyName = propertyName;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) this._identityId.ToString("N"), (object) this._propertyName);

    public static implicit operator string(IdentityExtendedPropertyKeyBuilder keyBuilder) => keyBuilder?.ToString();
  }
}
