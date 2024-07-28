// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.BindPendingConversionHelper
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Graph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal abstract class BindPendingConversionHelper : IBindPendingCoversionHelper
  {
    protected const string c_area = "Identity";
    private const string c_layer = "BindPendingConversionHelper";
    protected IdentityConversionPayload _identityConversionPayload;
    protected IList<IdentityKeyMap> _identityKeyMaps;
    protected HashSet<IdentityDescriptor> _identityDescriptors;
    protected string _prefix;

    protected BindPendingConversionHelper(
      IdentityConversionPayload identityConversionPayload)
    {
      this._identityConversionPayload = identityConversionPayload;
      this._identityKeyMaps = (IList<IdentityKeyMap>) new List<IdentityKeyMap>();
      this._identityDescriptors = new HashSet<IdentityDescriptor>();
    }

    public void ConvertIdentitiesToBindPending(IVssRequestContext requestContext)
    {
      HashSet<IdentityDescriptor> hashSet = this._identityConversionPayload.ActiveIdentities.Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (i => i.Descriptor)).ToHashSet<IdentityDescriptor>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity activeIdentity in this._identityConversionPayload.ActiveIdentities)
      {
        this.CheckAndFixCollision(requestContext, activeIdentity);
        this.ConvertToBindPending(activeIdentity);
        this.AddMungedCuid(activeIdentity);
      }
      List<IdentityDescriptor> conflictDescriptors = hashSet.Intersect<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) this._identityDescriptors).ToList<IdentityDescriptor>();
      if (conflictDescriptors.Any<IdentityDescriptor>())
      {
        requestContext.TraceDataConditionally(9281905, TraceLevel.Error, "Identity", nameof (BindPendingConversionHelper), string.Format("[{0}]-[{1}]: Following conflicts found during bind pending conversion", (object) requestContext.ServiceHost.InstanceId, (object) this._identityConversionPayload.EventId), (Func<object>) (() => (object) new List<IdentityDescriptor>[1]
        {
          conflictDescriptors
        }), nameof (ConvertIdentitiesToBindPending));
        throw new AccountStateNotValidException("Active conflict identities in collection");
      }
      this.UpdateIdentities(requestContext);
      this.MungeCuids(requestContext);
    }

    private void UpdateIdentities(IVssRequestContext requestContext)
    {
      requestContext.CheckOrganizationRequestContext();
      IdentityService service = requestContext.GetService<IdentityService>();
      if (this._identityConversionPayload.InactiveIdentities.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
      {
        requestContext.Trace(9281906, TraceLevel.Info, "Identity", nameof (BindPendingConversionHelper), string.Format("[{0}]-[{1}]: Munge inactive Identities: {2}.", (object) requestContext.ServiceHost.InstanceId, (object) this._identityConversionPayload.EventId, (object) BindPendingConversionHelper.GetTraceMessageForIdentities((IList<Microsoft.VisualStudio.Services.Identity.Identity>) this._identityConversionPayload.InactiveIdentities, this._identityConversionPayload.IsFullTracing)));
        service.UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this._identityConversionPayload.InactiveIdentities, true);
      }
      requestContext.Trace(9281907, TraceLevel.Info, "Identity", nameof (BindPendingConversionHelper), string.Format("[{0}]-[{1}]: Bind pending active Identities: {2}.", (object) requestContext.ServiceHost.InstanceId, (object) this._identityConversionPayload.EventId, (object) BindPendingConversionHelper.GetTraceMessageForIdentities((IList<Microsoft.VisualStudio.Services.Identity.Identity>) this._identityConversionPayload.ActiveIdentities, this._identityConversionPayload.IsFullTracing)));
      service.UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this._identityConversionPayload.ActiveIdentities, true);
    }

    private void MungeCuids(IVssRequestContext requestContext)
    {
      using (IdentityKeyMapComponent component = requestContext.CreateComponent<IdentityKeyMapComponent>())
        component.UpdateIdentityKeyMaps(this._identityKeyMaps);
    }

    protected void MungeIdentity(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      string property = identity.GetProperty<string>("Account", (string) null);
      string str1 = string.Empty;
      IdentityDescriptor identityDescriptor;
      if (identity.IsMsa())
      {
        identityDescriptor = BindPendingConversionHelper.GetMungedMsaDescriptor(requestContext, identity, this._prefix);
        if (!string.IsNullOrEmpty(property))
          str1 = string.Format("{0}_{1}_{2}", (object) this._prefix, (object) identity.Id, (object) property);
      }
      else
      {
        identityDescriptor = BindPendingConversionHelper.GetMungedAadDescriptor(requestContext, identity, this._prefix);
        string str2 = identity.GetProperty<Guid>("http://schemas.microsoft.com/identity/claims/objectidentifier", new Guid()).ToString();
        Guid zero = GuidUtils.Change14thCharToZero(Guid.NewGuid());
        identity.SetProperty("http://schemas.microsoft.com/identity/claims/objectidentifier", (object) zero);
        if (!string.IsNullOrEmpty(property))
          str1 = this._prefix + "_" + str2 + "_" + property;
      }
      DateTime utcNow;
      if (!identity.Properties.TryGetValue<DateTime>("MetadataUpdateDate", out utcNow))
        utcNow = DateTime.UtcNow;
      identity.SetProperty("MetadataUpdateDate", (object) utcNow.AddSeconds(1.0));
      identity.SetProperty("PUID", (object) string.Empty);
      identity.Descriptor = identityDescriptor;
      identity.SetProperty("Account", (object) str1);
    }

    private static string GetTraceMessageForIdentities(IList<Microsoft.VisualStudio.Services.Identity.Identity> identities, bool fullTracing = false)
    {
      if (identities == null || identities.Count == 0)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("{");
      stringBuilder.AppendFormat("Total count: {0}. ", (object) identities.Count);
      int num = 0;
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
      {
        if (num > 0)
          stringBuilder.Append(",");
        stringBuilder.Append(BindPendingConversionHelper.GetTraceMessageForIdentity(identity));
        if (num++ > 10)
        {
          if (!fullTracing)
            break;
        }
      }
      stringBuilder.Append("}");
      return stringBuilder.ToString();
    }

    private static string GetTraceMessageForIdentity(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (identity == null)
        return string.Empty;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "(id: {0}; mid: {1}; cuid: {2}; sid: {3}; domain: {4}; accountName: {5})", (object) identity.Id, (object) identity.MasterId, (object) identity.GetProperty<Guid>("CUID", Guid.Empty), (object) identity.Descriptor.Identifier, (object) identity.GetProperty<string>("Domain", string.Empty), (object) identity.GetProperty<string>("Account", string.Empty));
    }

    protected static IdentityDescriptor GetAadDescriptor(string accountName, string tenantId) => IdentityHelper.CreateDescriptorFromAccountName(tenantId, accountName);

    protected static IdentityDescriptor GetAadBindPendingDescriptor(
      string accountName,
      string tenantId)
    {
      return IdentityHelper.CreateDescriptorFromAccountName(tenantId, accountName, true);
    }

    protected static IdentityDescriptor GetMsaBindPendingDescriptor(string accountName) => IdentityHelper.CreateDescriptorFromAccountName("Windows Live ID", accountName, true);

    private static IdentityDescriptor GetMungedMsaDescriptor(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string prefix)
    {
      string[] strArray = identity.Descriptor.Identifier.Split('\\');
      if (strArray.Length <= 1)
        return new IdentityDescriptor("Microsoft.TeamFoundation.ImportedIdentity", string.Format("{0}_{1}_{2}", (object) prefix, (object) identity.Id, (object) identity.Descriptor.Identifier));
      string str1 = strArray[0];
      string str2 = strArray[1];
      return new IdentityDescriptor("Microsoft.TeamFoundation.ImportedIdentity", string.Format("{0}\\{1}_{2}_{3}", (object) str1, (object) prefix, (object) identity.Id, (object) str2));
    }

    private static IdentityDescriptor GetMungedAadDescriptor(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string prefix)
    {
      string[] strArray = identity.Descriptor.Identifier.Split('\\');
      if (strArray.Length <= 1)
        return new IdentityDescriptor("Microsoft.TeamFoundation.ImportedIdentity", string.Format("{0}_{1}_{2}", (object) prefix, (object) identity.Id, (object) identity.Descriptor.Identifier));
      string str1 = strArray[0];
      string str2 = strArray[1];
      string str3 = identity.GetProperty<Guid>("http://schemas.microsoft.com/identity/claims/objectidentifier", new Guid()).ToString();
      return new IdentityDescriptor("Microsoft.TeamFoundation.ImportedIdentity", str1 + "\\" + prefix + "_" + str3 + "_" + str2);
    }

    protected abstract void CheckAndFixCollision(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity);

    protected abstract void ConvertToBindPending(Microsoft.VisualStudio.Services.Identity.Identity identity);

    protected abstract void AddMungedCuid(Microsoft.VisualStudio.Services.Identity.Identity identity);
  }
}
