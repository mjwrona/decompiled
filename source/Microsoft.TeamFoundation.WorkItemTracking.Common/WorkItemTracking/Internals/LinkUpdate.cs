// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkUpdate
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class LinkUpdate
  {
    private LinkProperties m_supported;
    private LinkProperties m_mask;
    private string m_comment;
    private bool m_isLocked;

    public LinkProperties Mask => this.m_mask;

    public string Comment
    {
      get => this.m_comment;
      set
      {
        this.CheckSupport(LinkProperties.Comment);
        this.m_comment = value == null ? string.Empty : value;
        this.m_mask |= LinkProperties.Comment;
      }
    }

    public bool IsLocked
    {
      get => this.m_isLocked;
      set
      {
        if (value)
          this.CheckSupport(LinkProperties.IsLocked);
        this.m_isLocked = value;
        this.m_mask |= LinkProperties.IsLocked;
      }
    }

    public LinkUpdate(LinkProperties supported) => this.m_supported = supported;

    public void Submit(XmlElement element)
    {
      if ((this.m_mask & LinkProperties.Comment) != LinkProperties.None)
        element.SetAttribute("Comment", this.Comment);
      if ((this.m_mask & LinkProperties.IsLocked) == LinkProperties.None)
        return;
      element.SetAttribute("Lock", XmlConvert.ToString(this.m_isLocked));
    }

    public void Submit(LinkInfo li)
    {
      if ((this.m_mask & LinkProperties.Comment) != LinkProperties.None)
        li.Comment = this.Comment;
      if ((this.m_mask & LinkProperties.IsLocked) == LinkProperties.None)
        return;
      ((WorkItemLinkInfo) li).IsLocked = this.IsLocked;
    }

    private void CheckSupport(LinkProperties props)
    {
      if ((this.m_supported & props) != props)
        throw new NotSupportedException();
    }
  }
}
