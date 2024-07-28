// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Utility.ReferenceLinksExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Utility
{
  public static class ReferenceLinksExtensions
  {
    public static ReferenceLinks GetSecuredReferenceLinks(
      this ReferenceLinks referenceLinks,
      string token,
      int requiredPermissions)
    {
      if (referenceLinks == null)
        return (ReferenceLinks) null;
      ReferenceLinks securedReferenceLinks = new ReferenceLinks();
      ReleaseManagementSecuredObject managementSecuredObject = new ReleaseManagementSecuredObject(token, requiredPermissions);
      foreach (KeyValuePair<string, object> link in (IEnumerable<KeyValuePair<string, object>>) referenceLinks.Links)
      {
        if (link.Value is IList<ReferenceLink>)
        {
          if (link.Value is IList<ReferenceLink> referenceLinkList)
          {
            foreach (ReferenceLink referenceLink in (IEnumerable<ReferenceLink>) referenceLinkList)
              securedReferenceLinks.AddLink(link.Key, referenceLink.Href, (ISecuredObject) managementSecuredObject);
          }
        }
        else if (link.Value is ReferenceLink && link.Value is ReferenceLink referenceLink1)
          securedReferenceLinks.AddLink(link.Key, referenceLink1.Href, (ISecuredObject) managementSecuredObject);
      }
      return securedReferenceLinks;
    }
  }
}
