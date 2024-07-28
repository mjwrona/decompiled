// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ReferenceLinksExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class ReferenceLinksExtensions
  {
    public static bool TryAddLink(
      this ReferenceLinks links,
      string name,
      ISecuredObject securedObject,
      string href)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (links == null || links.Links.ContainsKey(name))
        return false;
      links.AddLink(name, href, securedObject);
      return true;
    }

    public static bool TryUnsecuredAddLink(this ReferenceLinks links, string name, string href)
    {
      if (links == null || links.Links.ContainsKey(name))
        return false;
      links.AddLink(name, href);
      return true;
    }

    public static bool TryAddLink(
      this ReferenceLinks links,
      string name,
      ISecuredObject securedObject,
      Func<string> href)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (links == null || links.Links.ContainsKey(name))
        return false;
      links.AddLink(name, href(), securedObject);
      return true;
    }

    public static bool TryGetUrl(this ReferenceLinks links, string name, out string href)
    {
      object obj;
      if (links?.Links != null && links.Links.TryGetValue(name, out obj) && obj is ReferenceLink referenceLink)
      {
        href = referenceLink.Href;
        return true;
      }
      href = (string) null;
      return false;
    }
  }
}
