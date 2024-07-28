// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Common.IDiffChange
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 072F1303-F456-426E-A1CB-C0838641751B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Common.dll

namespace Microsoft.VisualStudio.Services.DevSecOps.Common
{
  public interface IDiffChange
  {
    DiffChangeType ChangeType { get; }

    int OriginalStart { get; }

    int OriginalLength { get; }

    int OriginalEnd { get; }

    int ModifiedStart { get; }

    int ModifiedLength { get; }

    int ModifiedEnd { get; }

    IDiffChange Add(IDiffChange diffChange);
  }
}
