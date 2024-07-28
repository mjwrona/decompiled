// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.FieldInfo
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class FieldInfo
  {
    public string PropertyName { get; set; }

    public string DisplayName { get; set; }

    public bool IsCommonField { get; set; }

    public bool IsExpansion { get; set; }

    public bool wrapInQuotes { get; set; }

    public string Type { get; set; }

    public bool DisallowFiltering { get; set; }

    public bool IsDefaultField { get; set; }
  }
}
