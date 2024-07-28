// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.Common.DiffChange
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F16CDF2D-8103-4EAE-A2A8-4FA5B1C1BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.CodeReview.Server.Common
{
  public class DiffChange : IDiffChange
  {
    private DiffChangeType m_changeType;
    private int m_originalStart;
    private int m_originalLength;
    private int m_modifiedStart;
    private int m_modifiedLength;
    private bool m_updateChangeType;

    public DiffChange(
      int originalStart,
      int originalLength,
      int modifiedStart,
      int modifiedLength)
    {
      this.m_originalStart = originalStart;
      this.m_originalLength = originalLength;
      this.m_modifiedStart = modifiedStart;
      this.m_modifiedLength = modifiedLength;
      this.UpdateChangeType();
    }

    private void UpdateChangeType()
    {
      if (this.m_originalLength > 0)
        this.m_changeType = this.m_modifiedLength <= 0 ? DiffChangeType.Delete : DiffChangeType.Change;
      else if (this.m_modifiedLength > 0)
        this.m_changeType = DiffChangeType.Insert;
      this.m_updateChangeType = false;
    }

    public DiffChangeType ChangeType
    {
      get
      {
        if (this.m_updateChangeType)
          this.UpdateChangeType();
        return this.m_changeType;
      }
    }

    public int OriginalStart
    {
      get => this.m_originalStart;
      set
      {
        this.m_originalStart = value;
        this.m_updateChangeType = true;
      }
    }

    public int OriginalLength
    {
      get => this.m_originalLength;
      set
      {
        this.m_originalLength = value;
        this.m_updateChangeType = true;
      }
    }

    public int OriginalEnd => this.OriginalStart + this.OriginalLength;

    public int ModifiedStart
    {
      get => this.m_modifiedStart;
      set
      {
        this.m_modifiedStart = value;
        this.m_updateChangeType = true;
      }
    }

    public int ModifiedLength
    {
      get => this.m_modifiedLength;
      set
      {
        this.m_modifiedLength = value;
        this.m_updateChangeType = true;
      }
    }

    public int ModifiedEnd => this.ModifiedStart + this.ModifiedLength;

    public IDiffChange Add(IDiffChange diffChange)
    {
      if (diffChange == null)
        return (IDiffChange) this;
      int originalStart = Math.Min(this.OriginalStart, diffChange.OriginalStart);
      int num1 = Math.Max(this.OriginalEnd, diffChange.OriginalEnd);
      int modifiedStart = Math.Min(this.ModifiedStart, diffChange.ModifiedStart);
      int num2 = Math.Max(this.ModifiedEnd, diffChange.ModifiedEnd);
      return (IDiffChange) new DiffChange(originalStart, num1 - originalStart, modifiedStart, num2 - modifiedStart);
    }
  }
}
