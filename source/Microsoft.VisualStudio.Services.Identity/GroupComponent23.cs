// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupComponent23
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class GroupComponent23 : GroupComponent22
  {
    public override ResultCollection ReadAadGroups(bool readInactive = true)
    {
      try
      {
        this.TraceEnter(4704582, nameof (ReadAadGroups));
        this.PrepareStoredProcedure("prc_ReadAadGroups");
        this.BindBoolean("@readInactive", readInactive);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<GroupComponent.GroupIdentityData>((ObjectBinder<GroupComponent.GroupIdentityData>) this.GetGroupIdentitiesColumns());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(4704583, nameof (ReadAadGroups));
      }
    }
  }
}
