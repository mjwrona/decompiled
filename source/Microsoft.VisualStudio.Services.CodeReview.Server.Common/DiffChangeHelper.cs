// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.Common.DiffChangeHelper
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F16CDF2D-8103-4EAE-A2A8-4FA5B1C1BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.CodeReview.Server.Common
{
  public class DiffChangeHelper : IDisposable
  {
    private int m_originalCount;
    private int m_modifiedCount;
    private List<IDiffChange> m_changes;
    private int m_originalStart;
    private int m_modifiedStart;

    public DiffChangeHelper()
    {
      this.m_changes = new List<IDiffChange>();
      this.m_originalStart = int.MaxValue;
      this.m_modifiedStart = int.MaxValue;
    }

    public void Dispose()
    {
      if (this.m_changes != null)
        this.m_changes = (List<IDiffChange>) null;
      GC.SuppressFinalize((object) this);
    }

    public void MarkNextChange()
    {
      if (this.m_originalCount > 0 || this.m_modifiedCount > 0)
        this.m_changes.Add((IDiffChange) new DiffChange(this.m_originalStart, this.m_originalCount, this.m_modifiedStart, this.m_modifiedCount));
      this.m_originalCount = 0;
      this.m_modifiedCount = 0;
      this.m_originalStart = int.MaxValue;
      this.m_modifiedStart = int.MaxValue;
    }

    public void AddOriginalElement(int originalIndex, int modifiedIndex)
    {
      this.m_originalStart = Math.Min(this.m_originalStart, originalIndex);
      this.m_modifiedStart = Math.Min(this.m_modifiedStart, modifiedIndex);
      ++this.m_originalCount;
    }

    public void AddModifiedElement(int originalIndex, int modifiedIndex)
    {
      this.m_originalStart = Math.Min(this.m_originalStart, originalIndex);
      this.m_modifiedStart = Math.Min(this.m_modifiedStart, modifiedIndex);
      ++this.m_modifiedCount;
    }

    public IDiffChange[] Changes
    {
      get
      {
        if (this.m_originalCount > 0 || this.m_modifiedCount > 0)
          this.MarkNextChange();
        return this.m_changes.ToArray();
      }
    }

    public IDiffChange[] ReverseChanges
    {
      get
      {
        if (this.m_originalCount > 0 || this.m_modifiedCount > 0)
          this.MarkNextChange();
        this.m_changes.Reverse();
        return this.m_changes.ToArray();
      }
    }
  }
}
