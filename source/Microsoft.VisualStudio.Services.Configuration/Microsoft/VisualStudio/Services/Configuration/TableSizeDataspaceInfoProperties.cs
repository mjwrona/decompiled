// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.TableSizeDataspaceInfoProperties
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class TableSizeDataspaceInfoProperties
  {
    public string Schema { get; set; }

    public string TableName { get; set; }

    public long SizePages { get; set; }

    public int DataspaceColumnCount { get; set; }

    public long SizeMB => this.SizePages * 8L / 1024L;
  }
}
