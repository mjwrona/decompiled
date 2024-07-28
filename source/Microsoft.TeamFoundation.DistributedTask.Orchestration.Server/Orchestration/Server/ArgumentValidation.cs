// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.ArgumentValidation
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal static class ArgumentValidation
  {
    public const int TimelineRecordNameMaxLength = 512;
    public const int TimelineRecordTypeMaxLength = 512;
    public const int TimelineRecordCurrentOperationMaxLength = 256;
    public const int TimelineRecordWorkNameMaxLength = 512;

    public static void Validate(IEnumerable<TimelineRecord> records, string variableName)
    {
      ArgumentUtility.CheckForNull<IEnumerable<TimelineRecord>>(records, variableName, "DistributedTask");
      int index = 0;
      foreach (TimelineRecord record in records)
      {
        if (record == null)
          ArgumentUtility.CheckForNull<TimelineRecord>(record, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}[{1}]", (object) variableName, (object) index), "DistributedTask");
        ArgumentValidation.Validate(record, (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}[{1}]", (object) variableName, (object) index)));
        index++;
      }
    }

    private static void Validate(TimelineRecord record, Func<string> getVariableName)
    {
      if (record.Name != null && record.Name.Length > 512)
        record.Name = record.Name.Substring(0, 509) + "...";
      if (record.RecordType != null && record.RecordType.Length > 512)
        ArgumentUtility.CheckStringLength(record.RecordType, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) getVariableName(), (object) "RecordType"), 512, expectedServiceArea: "DistributedTask");
      if (record.CurrentOperation != null && record.CurrentOperation.Length > 256)
        record.CurrentOperation = record.CurrentOperation.Substring(0, 253) + "...";
      if (record.WorkerName == null || record.WorkerName.Length <= 512)
        return;
      ArgumentUtility.CheckStringLength(record.WorkerName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) getVariableName(), (object) "WorkerName"), 512, expectedServiceArea: "DistributedTask");
    }
  }
}
