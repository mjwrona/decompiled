// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps.TfvcItemNotFoundFaultMapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps
{
  public class TfvcItemNotFoundFaultMapper : FaultMapper
  {
    private readonly HashSet<string> m_errorCodes = new HashSet<string>()
    {
      "VS403404",
      "VS403405",
      "VS403420"
    };

    public TfvcItemNotFoundFaultMapper()
      : base("TfvcItemItemNotFound", IndexerFaultSource.TFS)
    {
    }

    public override bool IsMatch(Exception ex)
    {
      if (ex == null)
        return false;
      string str = ex.ToString();
      bool flag = (str.Contains("Microsoft.VisualStudio.Services.Common.VssServiceException") || str.Contains("Microsoft.TeamFoundation.VersionControl.Server.ItemNotFoundException")) && (str.Contains("does not exist at the specified version, or you do not have permission to access it") || str.Contains("does not exist on the server, or you do not have permission to access it"));
      if (!flag)
      {
        foreach (string errorCode in this.m_errorCodes)
        {
          if (str.Contains(errorCode))
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }
  }
}
