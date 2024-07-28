// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.ReferenceLinksExtensions
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Pipelines.WebApi
{
  internal static class ReferenceLinksExtensions
  {
    public static string GetLink(this ReferenceLinks links, string key)
    {
      if (links == null)
        return (string) null;
      object source;
      if (links.Links.TryGetValue(key, out source))
      {
        switch (source)
        {
          case IList<ReferenceLink> _:
            return ((IEnumerable<ReferenceLink>) source).FirstOrDefault<ReferenceLink>()?.Href;
          case ReferenceLink _:
            return ((ReferenceLink) source).Href;
        }
      }
      return (string) null;
    }
  }
}
