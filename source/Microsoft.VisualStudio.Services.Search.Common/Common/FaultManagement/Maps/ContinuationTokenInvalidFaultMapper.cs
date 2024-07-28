// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps.ContinuationTokenInvalidFaultMapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps
{
  public class ContinuationTokenInvalidFaultMapper : FaultMapper
  {
    private readonly HashSet<string> m_errorCodes = new HashSet<string>()
    {
      "VS403659"
    };

    public ContinuationTokenInvalidFaultMapper()
      : base("ContinuationTokenInvalid", IndexerFaultSource.TFS)
    {
    }

    public override bool IsMatch(Exception ex)
    {
      if (ex == null)
        return false;
      string errStr = ex.ToString();
      return this.m_errorCodes.Any<string>((Func<string, bool>) (errorCode => errStr.Contains(errorCode)));
    }
  }
}
