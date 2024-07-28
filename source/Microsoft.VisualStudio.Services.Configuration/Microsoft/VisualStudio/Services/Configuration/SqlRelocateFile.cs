// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SqlRelocateFile
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.VisualStudio.Services.Common;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Configuration
{
  [DebuggerDisplay("{LogicalFileName} - {PhysicalFileName}")]
  public class SqlRelocateFile
  {
    public SqlRelocateFile(string logicalFileName, string physicalFileName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(logicalFileName, nameof (logicalFileName));
      ArgumentUtility.CheckStringForNullOrEmpty(physicalFileName, nameof (physicalFileName));
      this.LogicalFileName = logicalFileName;
      this.PhysicalFileName = physicalFileName;
    }

    public string LogicalFileName { get; private set; }

    public string PhysicalFileName { get; private set; }
  }
}
