// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSqlComponent5
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceSqlComponent5 : CommerceSqlComponent4
  {
    public override int CleanupAzureResourceAccounts()
    {
      int num = 0;
      try
      {
        this.TraceEnter(5106500, nameof (CleanupAzureResourceAccounts));
        this.Trace(5106501, TraceLevel.Info, string.Format("{0}: calling {1}. at Utc {2}", (object) nameof (CleanupAzureResourceAccounts), (object) "prc_CleanupAzureResourceAccount", (object) DateTime.UtcNow));
        this.PrepareStoredProcedure("prc_CleanupAzureResourceAccount");
        num = (int) this.ExecuteNonQuery(true);
        return num;
      }
      catch (Exception ex)
      {
        this.ComponentRequestContext.TraceException(5106502, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.Trace(5106503, TraceLevel.Info, string.Format("{0} completed. Deleted {1} rows.", (object) nameof (CleanupAzureResourceAccounts), (object) num));
        this.TraceLeave(5106504, nameof (CleanupAzureResourceAccounts));
      }
    }
  }
}
