// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogComponent5
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.VisualStudio.Services.Common;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class CatalogComponent5 : CatalogComponent4
  {
    public void DeleteMachineFromCatalog(string machineName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(machineName, nameof (machineName));
      this.PrepareStoredProcedure("prc_DeleteMachineFromCatalog");
      this.BindString("@machineName", machineName, 256, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public void DeleteWebApplicationFromCatalog(string machineName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(machineName, nameof (machineName));
      this.PrepareStoredProcedure("prc_DeleteWebApplicationFromCatalog");
      this.BindString("@machineName", machineName, 256, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }
  }
}
