// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.IdentityHelper
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  internal class IdentityHelper
  {
    private readonly Microsoft.VisualStudio.Services.Identity.Identity m_identity;

    public IdentityHelper(Microsoft.VisualStudio.Services.Identity.Identity identity) => this.m_identity = identity;

    public string GetDistinctDisplayName()
    {
      string displayName = this.GetDisplayName();
      string disambiguationPart = this.GetDisambiguationPart();
      if (!string.IsNullOrWhiteSpace(disambiguationPart))
        return FormattableString.Invariant(FormattableStringFactory.Create("{0} <{1}>", (object) displayName, (object) disambiguationPart));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081421, "Query Pipeline", "Corrector", "Disambiguation part of identity is null or whitespace.");
      return displayName;
    }

    public string GetDisplayName()
    {
      if (!string.IsNullOrWhiteSpace(this.m_identity.CustomDisplayName))
        return this.m_identity.CustomDisplayName;
      return this.m_identity.UniqueUserId != 0 && (this.m_identity.Descriptor.IdentityType.Equals("Microsoft.TeamFoundation.BindPendingIdentity", StringComparison.OrdinalIgnoreCase) || this.m_identity.Descriptor.IdentityType.Equals("Microsoft.IdentityModel.Claims.ClaimsIdentity", StringComparison.OrdinalIgnoreCase)) && this.m_identity.ProviderDisplayName.IndexOf('@') > 0 ? Microsoft.TeamFoundation.Framework.Server.IdentityHelper.GetUniqueName(string.Empty, this.m_identity.ProviderDisplayName, this.m_identity.UniqueUserId) : this.m_identity.ProviderDisplayName;
    }

    public string GetDisambiguationPart()
    {
      string attribute1 = this.GetAttribute("Account", string.Empty);
      if (!this.m_identity.IsContainer && (this.m_identity.IsExternalUser || this.IsMsaDomain))
        return attribute1;
      string attribute2 = this.GetAttribute("LocalScopeId", string.Empty);
      string attribute3 = this.GetAttribute("Domain", string.Empty);
      return !string.IsNullOrEmpty(attribute2) ? FormattableString.Invariant(FormattableStringFactory.Create("{0}{1}", (object) "id:", (object) this.m_identity.Id)) : (string.IsNullOrEmpty(attribute3) ? attribute1 : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}", (object) attribute3, (object) attribute1));
    }

    private bool IsMsaDomain => this.GetAttribute("Domain", string.Empty).Equals("Windows Live ID", StringComparison.OrdinalIgnoreCase);

    private string GetAttribute(string name, string defaultValue)
    {
      object obj;
      return this.m_identity.TryGetProperty(name, out obj) && obj != null ? obj.ToString() : defaultValue;
    }
  }
}
