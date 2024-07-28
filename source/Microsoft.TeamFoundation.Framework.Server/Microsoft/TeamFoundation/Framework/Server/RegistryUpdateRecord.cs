// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistryUpdateRecord
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public readonly struct RegistryUpdateRecord
  {
    public readonly long SequenceId;
    public readonly RegistryItem Item;

    public RegistryUpdateRecord(long sequenceId, RegistryItem item)
    {
      this.SequenceId = sequenceId;
      this.Item = item;
    }

    public RegistryUpdateRecord(long sequenceId, string path, string value)
      : this(sequenceId, new RegistryItem(path, value))
    {
    }
  }
}
