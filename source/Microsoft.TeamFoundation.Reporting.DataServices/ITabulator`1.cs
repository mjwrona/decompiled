// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.ITabulator`1
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public interface ITabulator<RecordType>
  {
    TransformInstructions<RecordType> TabulationInstructions { get; set; }

    IInterpretTimedData<RecordType> RecordInterpreter { get; set; }

    void Tabulate(IEnumerable<RecordType> recordBuffer, IVssRequestContext requestContext);

    TransformResult PackResultsForLocalZoneTime(
      TimeZoneInfo localTimeZoneInfo,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
