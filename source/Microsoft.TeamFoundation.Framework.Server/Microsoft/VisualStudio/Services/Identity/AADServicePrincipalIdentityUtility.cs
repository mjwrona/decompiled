// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.AADServicePrincipalIdentityUtility
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class AADServicePrincipalIdentityUtility
  {
    internal static Microsoft.VisualStudio.Services.Identity.Identity CreateAADServicePrincipalIdentity(
      IVssRequestContext requestContext,
      string oid,
      string tid,
      string appid,
      string identifier,
      string metaType,
      string givenName,
      string surname,
      IdentityDescriptor descriptor,
      string customName = null)
    {
      Guid cuid = IdentityCuidHelper.ComputeCuid(requestContext, Guid.Parse(tid), oid);
      string str = identifier;
      if (!string.IsNullOrEmpty(givenName) && !string.IsNullOrEmpty(surname))
        str = givenName + " " + surname;
      else if (!string.IsNullOrEmpty(givenName))
        str = givenName;
      Microsoft.VisualStudio.Services.Identity.Identity identity = new Microsoft.VisualStudio.Services.Identity.Identity();
      identity.CustomDisplayName = customName;
      identity.Descriptor = descriptor;
      identity.ProviderDisplayName = str;
      identity.IsActive = true;
      identity.IsContainer = false;
      identity.UniqueUserId = 0;
      identity.Members = (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
      identity.MemberOf = (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
      identity.SubjectDescriptor = new SubjectDescriptor("aadsp", cuid.ToString());
      Microsoft.VisualStudio.Services.Identity.Identity principalIdentity = identity;
      IdentityMetaType result;
      if (Enum.TryParse<IdentityMetaType>(metaType, out result))
        principalIdentity.MetaType = result;
      principalIdentity.SetProperty("SchemaClassName", (object) "User");
      principalIdentity.SetProperty("Description", (object) string.Empty);
      principalIdentity.SetProperty("Domain", (object) tid);
      principalIdentity.SetProperty("Account", (object) identifier);
      principalIdentity.SetProperty("DN", (object) string.Empty);
      principalIdentity.SetProperty("Mail", (object) string.Empty);
      principalIdentity.SetProperty("Alias", (object) string.Empty);
      principalIdentity.SetProperty("http://schemas.microsoft.com/identity/claims/objectidentifier", (object) oid);
      principalIdentity.SetProperty("ApplicationId", (object) appid);
      principalIdentity.ResetModifiedProperties();
      return principalIdentity;
    }
  }
}
