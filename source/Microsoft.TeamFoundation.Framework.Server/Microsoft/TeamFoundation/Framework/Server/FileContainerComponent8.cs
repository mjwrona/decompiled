// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerComponent8
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerComponent8 : FileContainerComponent7
  {
    public override void DeleteContainers(IList<long> containerIds, Guid dataspaceIdentifier)
    {
      this.TraceEnter(0, nameof (DeleteContainers));
      this.PrepareStoredProcedure("prc_DeleteContainers");
      this.BindInt64WithIndexTable("@containerIdTable", (IEnumerable<long>) containerIds);
      this.BindDataspace(dataspaceIdentifier);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DeleteContainers));
    }
  }
}
