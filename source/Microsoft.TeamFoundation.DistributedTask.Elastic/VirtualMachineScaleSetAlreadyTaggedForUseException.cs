// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.VirtualMachineScaleSetAlreadyTaggedForUseException
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  public class VirtualMachineScaleSetAlreadyTaggedForUseException : ElasticPoolException
  {
    [Obsolete("All exceptions are supposed to have an empty constructor, but they're useless. Use one of the other constructors.")]
    public VirtualMachineScaleSetAlreadyTaggedForUseException()
    {
    }

    public VirtualMachineScaleSetAlreadyTaggedForUseException(
      string newPoolName,
      string currentPoolName)
      : base("The scale set can not be assigned to pool " + newPoolName + " as it is already in use by pool " + currentPoolName + ".")
    {
    }
  }
}
