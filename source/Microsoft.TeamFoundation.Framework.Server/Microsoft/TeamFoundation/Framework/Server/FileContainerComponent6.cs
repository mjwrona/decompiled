// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerComponent6
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerComponent6 : FileContainerComponent5
  {
    protected override void BindDataspace(Guid dataspaceIdentifier) => this.BindInt("@dataspaceId", this.GetDataspaceId(dataspaceIdentifier));

    protected override void BindDataspace(Guid? dataspaceIdentifier)
    {
      if (!dataspaceIdentifier.HasValue)
        return;
      this.BindInt("@dataspaceId", this.GetDataspaceId(dataspaceIdentifier.Value));
    }

    internal override ContainerBinder GetFileContainerBinder() => (ContainerBinder) new ContainerBinder2((TeamFoundationSqlResourceComponent) this);

    internal override ContainerItemBinder GetFileContainerItemBinder() => (ContainerItemBinder) new ContainerItemBinder2((TeamFoundationSqlResourceComponent) this);
  }
}
