// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryService.Components.DirectoryMemberRequestStage
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Directories.DirectoryService.Components
{
  public class DirectoryMemberRequestStage
  {
    private readonly LinkedList<IDirectoryMemberRequestFilter> filters = new LinkedList<IDirectoryMemberRequestFilter>();

    public string Name { get; }

    public IEnumerable<IDirectoryMemberRequestFilter> Filters => (IEnumerable<IDirectoryMemberRequestFilter>) this.filters;

    public DirectoryMemberRequestStage(string name) => this.Name = name;

    public DirectoryMemberRequestStage Append(params IDirectoryMemberRequestFilter[] filters)
    {
      foreach (IDirectoryMemberRequestFilter filter in filters)
        this.filters.AddLast(filter);
      return this;
    }

    public DirectoryMemberRequestStage Prepend(params IDirectoryMemberRequestFilter[] filters)
    {
      for (int index = filters.Length - 1; index >= 0; --index)
        this.filters.AddFirst(filters[index]);
      return this;
    }

    public void Replace(
      Predicate<IDirectoryMemberRequestFilter> predicate,
      IDirectoryMemberRequestFilter replacementFilter)
    {
      for (LinkedListNode<IDirectoryMemberRequestFilter> node = this.filters.First; node != null; node = node.Next)
      {
        if (predicate(node.Value))
        {
          this.filters.AddAfter(node, replacementFilter);
          this.filters.Remove(node);
          break;
        }
      }
    }

    public void Process(IVssRequestContext requestContext, DirectoryMemberRequest request)
    {
      foreach (IDirectoryMemberRequestFilter filter in this.Filters)
        filter.Process(requestContext, request);
    }
  }
}
