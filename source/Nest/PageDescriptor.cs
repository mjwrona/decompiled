// Decompiled with JetBrains decompiler
// Type: Nest.PageDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class PageDescriptor : DescriptorBase<PageDescriptor, IPage>, IPage
  {
    int? IPage.From { get; set; }

    int? IPage.Size { get; set; }

    public PageDescriptor From(int? from) => this.Assign<int?>(from, (Action<IPage, int?>) ((a, v) => a.From = v));

    public PageDescriptor Size(int? size) => this.Assign<int?>(size, (Action<IPage, int?>) ((a, v) => a.Size = v));
  }
}
