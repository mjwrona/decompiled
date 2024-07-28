// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DiagnosticComponent19
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DiagnosticComponent19 : DiagnosticComponent18
  {
    public override List<SpinlockInformation> QuerySpinlocks()
    {
      string str = "DIAGNOSTIC.prc_QuerySpinlocks";
      this.PrepareStoredProcedure(str);
      using (IDataReader reader = (IDataReader) this.ExecuteReader())
      {
        using (ResultCollection resultCollection = new ResultCollection(reader, str, this.RequestContext))
        {
          resultCollection.AddBinder<SpinlockInformation>((ObjectBinder<SpinlockInformation>) new SpinlockInformationBinder());
          return resultCollection.GetCurrent<SpinlockInformation>().Items;
        }
      }
    }
  }
}
