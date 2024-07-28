// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Interop.Common.Schema.Edm.TableEntityWriterContext
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

namespace Microsoft.Azure.Documents.Interop.Common.Schema.Edm
{
  internal sealed class TableEntityWriterContext
  {
    private bool hasElements;

    public TableEntityWriterContext(bool hasElements) => this.hasElements = hasElements;

    public TableEntityWriterContext()
      : this(false)
    {
    }

    public bool HasElements
    {
      get => this.hasElements;
      set => this.hasElements = value;
    }
  }
}
