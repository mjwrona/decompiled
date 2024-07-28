// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.CoreDatabaseBuilder
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class CoreDatabaseBuilder
  {
    private const string c_operationPrefix = "Vssp";

    public static Dictionary<ServiceLevel, List<ServicingOperation>> GetOperationsByServiceLevel(
      ServicingOperationProvider operationProvider,
      ServiceLevel fromServiceLevel,
      ServiceLevel toServiceLevel,
      bool finalConfiguration)
    {
      Dictionary<ServiceLevel, List<ServicingOperation>> operationsByServiceLevel = new Dictionary<ServiceLevel, List<ServicingOperation>>();
      foreach (ServicingOperation servicingOperation in operationProvider.GetServicingOperations())
      {
        ServiceLevel level;
        if (!(finalConfiguration ^ servicingOperation.Name.EndsWith("FinalConfiguration", StringComparison.OrdinalIgnoreCase)) && ServiceLevel.TryGetServiceLevelFromOperation(servicingOperation.Name, "Vssp", out level) && level >= fromServiceLevel && (toServiceLevel == (ServiceLevel) null || level <= toServiceLevel))
        {
          if (operationsByServiceLevel.Keys.Contains<ServiceLevel>(level))
            operationsByServiceLevel[level].Add(servicingOperation);
          else
            operationsByServiceLevel.Add(level, new List<ServicingOperation>()
            {
              servicingOperation
            });
        }
      }
      if (fromServiceLevel != toServiceLevel)
        operationsByServiceLevel.Remove(fromServiceLevel);
      return operationsByServiceLevel;
    }
  }
}
