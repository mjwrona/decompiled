// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.LayoutNodeContainer`1
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  public abstract class LayoutNodeContainer<T> : LayoutNode, ILayoutNodeContainer where T : LayoutNode
  {
    public LayoutNodeContainer() => this.Children = (IList<T>) new List<T>();

    public abstract IList<T> Children { get; protected set; }

    public bool TryGetChild(string childId, out T child)
    {
      child = this.Children.FirstOrDefault<T>((Func<T, bool>) (x => string.Compare(x.Id, childId, StringComparison.OrdinalIgnoreCase) == 0));
      return (object) child != null;
    }

    public DescendantType FindDescendant<DescendantType>(string id) where DescendantType : LayoutNode => this.GetDescendants<DescendantType>().Cast<DescendantType>().FirstOrDefault<DescendantType>((Func<DescendantType, bool>) (x => string.Compare(x.Id, id, StringComparison.OrdinalIgnoreCase) == 0));

    public ParentType FindAncestorOf<DecendentType, ParentType>(string id)
      where DecendentType : LayoutNode
      where ParentType : LayoutNode
    {
      if (!typeof (ILayoutNodeContainer).IsAssignableFrom(typeof (ParentType)))
        throw new ArgumentException("Parent type must be a container");
      return this.GetDescendants<ParentType>().Cast<ParentType>().Where<ParentType>((Func<ParentType, bool>) (p => ((ILayoutNodeContainer) (object) p).GetDescendants<DecendentType>().Any<DecendentType>((Func<DecendentType, bool>) (d => string.Compare(d.Id, id, StringComparison.OrdinalIgnoreCase) == 0)))).FirstOrDefault<ParentType>();
    }

    public IEnumerable<DescendantType> GetDescendants<DescendantType>() where DescendantType : LayoutNode
    {
      bool isCurrentChildTypeAContainer = typeof (ILayoutNodeContainer).IsAssignableFrom(typeof (T));
      foreach (T child1 in (IEnumerable<T>) this.Children)
      {
        T child = child1;
        if ((object) child is DescendantType)
          yield return (object) child as DescendantType;
        if (isCurrentChildTypeAContainer)
        {
          foreach (DescendantType descendant in ((ILayoutNodeContainer) (object) child).GetDescendants<DescendantType>())
            yield return descendant;
        }
        child = default (T);
      }
    }
  }
}
