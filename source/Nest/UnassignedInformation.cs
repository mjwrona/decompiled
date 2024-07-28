// Decompiled with JetBrains decompiler
// Type: Nest.UnassignedInformation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class UnassignedInformation
  {
    [DataMember(Name = "at")]
    public DateTime At { get; set; }

    [DataMember(Name = "last_allocation_status")]
    public string LastAllocationStatus { get; set; }

    [DataMember(Name = "reason")]
    public UnassignedInformationReason Reason { get; set; }
  }
}
