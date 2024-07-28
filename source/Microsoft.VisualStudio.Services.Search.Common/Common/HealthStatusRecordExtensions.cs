// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HealthStatusRecordExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class HealthStatusRecordExtensions
  {
    public static List<Scenario> GetScenariosFromInputRecord(this HealthStatusRecord record)
    {
      if (record == null)
        throw new ArgumentNullException(nameof (record));
      List<Scenario> source = new List<Scenario>();
      if (record.Data?.Scenarios != null)
        source = record.Data.Scenarios;
      return source.Any<Scenario>() ? source : throw new SearchServiceException("No scenario(s) listed in the record");
    }
  }
}
