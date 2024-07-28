// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.SearchTempStoreExtension
// Assembly: Microsoft.VisualStudio.Services.Search.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1D8FF195-304B-4BBA-9D1C-F4A6093CE2E1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Extensions.dll

using Microsoft.VisualStudio.Services.VssAzurePrep;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Extensions
{
  public class SearchTempStoreExtension : IVssAzurePrepExtension
  {
    private readonly string m_drive = "M:\\";

    public void Configure()
    {
      Trace.TraceInformation("SearchTempStoreExtension Configure started.");
      if (((IEnumerable<string>) Environment.GetLogicalDrives()).Contains<string>(this.m_drive, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        Environment.SetEnvironmentVariable("AZUREDEVOPS_SEARCH_TEMPSTOREPATH", this.m_drive, EnvironmentVariableTarget.Machine);
      Trace.TraceInformation("SearchTempStoreExtension Configure done.");
    }

    public void Unconfigure()
    {
    }
  }
}
