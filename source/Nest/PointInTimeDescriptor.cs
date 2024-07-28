// Decompiled with JetBrains decompiler
// Type: Nest.PointInTimeDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class PointInTimeDescriptor : 
    DescriptorBase<PointInTimeDescriptor, IPointInTime>,
    IPointInTime
  {
    public PointInTimeDescriptor(string id) => this.Self.Id = id;

    string IPointInTime.Id { get; set; }

    Time IPointInTime.KeepAlive { get; set; }

    public PointInTimeDescriptor KeepAlive(Time id) => this.Assign<Time>(id, (Action<IPointInTime, Time>) ((a, v) => a.KeepAlive = v));
  }
}
