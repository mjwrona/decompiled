// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.CssNode
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class CssNode
  {
    private string m_name;
    private object m_tag;
    private bool m_hasChildren;
    private int m_imageIndex;
    private int m_imageIndexExpanded;

    public CssNode(string name, object tag, bool hasChildren)
      : this(name, tag, hasChildren, -1)
    {
    }

    public CssNode(string name)
      : this(name, (object) null, false, -1)
    {
    }

    public CssNode(string name, object tag, bool hasChildren, int imageIndex)
      : this(name, tag, hasChildren, imageIndex, -1)
    {
    }

    public CssNode(
      string name,
      object tag,
      bool hasChildren,
      int imageIndex,
      int imageIndexExpanded)
    {
      this.m_name = name;
      this.m_tag = tag;
      this.m_hasChildren = hasChildren;
      this.m_imageIndex = imageIndex;
      this.m_imageIndexExpanded = imageIndexExpanded;
    }

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    public object Tag
    {
      get => this.m_tag;
      set => this.m_tag = value;
    }

    public bool HasChildren
    {
      get => this.m_hasChildren;
      set => this.m_hasChildren = value;
    }

    public int ImageIndex
    {
      get => this.m_imageIndex;
      set => this.m_imageIndex = value;
    }

    public int ImageIndexExpanded
    {
      get => this.m_imageIndexExpanded;
      set => this.m_imageIndexExpanded = value;
    }

    public event EventHandler<Exception> Populated;

    public void RaisePopulated(Exception error)
    {
      if (this.Populated == null)
        return;
      this.Populated((object) this, error);
    }
  }
}
