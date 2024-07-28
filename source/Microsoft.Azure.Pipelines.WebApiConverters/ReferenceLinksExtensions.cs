// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApiConverters.ReferenceLinksExtensions
// Assembly: Microsoft.Azure.Pipelines.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 42DDCCD8-4E0C-44F8-A5D2-AEF894388AED
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApiConverters.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.Azure.Pipelines.WebApiConverters
{
  public static class ReferenceLinksExtensions
  {
    public static void AddLink(
      this ReferenceLinks links,
      string name,
      ISecuredObject securedObject,
      Func<string> href)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (links == null || links.Links.ContainsKey(name))
        return;
      string href1 = href();
      if (string.IsNullOrEmpty(href1))
        return;
      links.AddLink(name, href1, securedObject);
    }

    public static void AddLink(
      this ReferenceLinks links,
      string name,
      ISecuredObject securedObject,
      string href)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (links == null || links.Links.ContainsKey(name) || string.IsNullOrEmpty(href))
        return;
      links.AddLink(name, href, securedObject);
    }
  }
}
