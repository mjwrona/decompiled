// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceMeteringComponent7
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceMeteringComponent7 : CommerceMeteringComponent6
  {
    protected override SqlCommand PrepareStoredProcedure(string storedProcedure) => base.PrepareStoredProcedure("Commerce." + storedProcedure);
  }
}
