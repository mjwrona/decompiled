// Decompiled with JetBrains decompiler
// Type: Nest.RuntimeField
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class RuntimeField : IRuntimeField
  {
    public string Format { get; set; }

    public IInlineScript Script { get; set; }

    public FieldType Type { get; set; }
  }
}
