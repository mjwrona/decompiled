// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.CollectionRefExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Organization
{
  public static class CollectionRefExtensions
  {
    public static Microsoft.VisualStudio.Services.Organization.Client.Collection ToClient(
      this CollectionRef x)
    {
      if (x == null)
        return (Microsoft.VisualStudio.Services.Organization.Client.Collection) null;
      return new Microsoft.VisualStudio.Services.Organization.Client.Collection() { Id = x.Id, Name = x.Name };
    }

    public static IList<Microsoft.VisualStudio.Services.Organization.Client.Collection> ToClient(
      this IEnumerable<CollectionRef> collections)
    {
      return collections == null ? (IList<Microsoft.VisualStudio.Services.Organization.Client.Collection>) null : (IList<Microsoft.VisualStudio.Services.Organization.Client.Collection>) collections.Select<CollectionRef, Microsoft.VisualStudio.Services.Organization.Client.Collection>((Func<CollectionRef, Microsoft.VisualStudio.Services.Organization.Client.Collection>) (x => x.ToClient())).Where<Microsoft.VisualStudio.Services.Organization.Client.Collection>((Func<Microsoft.VisualStudio.Services.Organization.Client.Collection, bool>) (x => x != null)).ToList<Microsoft.VisualStudio.Services.Organization.Client.Collection>();
    }
  }
}
