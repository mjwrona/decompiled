// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.TagsService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class TagsService : ITagsService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<TagItem> GetTagItemsByTagName(
      IVssRequestContext requestContext,
      string tagName,
      byte tagType)
    {
      ArgumentUtility.CheckForNull<string>(tagName, nameof (tagName));
      using (TagsComponent component = requestContext.CreateComponent<TagsComponent>())
        return component.GetTagItemsByTagName(tagName, tagType);
    }

    public int DeleteTagItems(IVssRequestContext requestContext, string tagName, byte tagType)
    {
      ArgumentUtility.CheckForNull<string>(tagName, nameof (tagName));
      using (TagsComponent component = requestContext.CreateComponent<TagsComponent>())
        return component.DeleteTagItems(tagName, tagType);
    }
  }
}
