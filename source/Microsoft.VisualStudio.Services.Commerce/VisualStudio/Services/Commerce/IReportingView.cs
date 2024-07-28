// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.IReportingView
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [InheritedExport]
  public interface IReportingView
  {
    void ProcessEvents(IVssRequestContext requestContext, AzureReportingEventStore eventStore);

    IEnumerable<T> GetProcessedEvents<T>(
      IVssRequestContext requestContext,
      AzureReportingEventStore eventStore,
      string resourceName,
      DateTime startDateInclusive,
      DateTime endDateInclusive,
      string filter)
      where T : ITableEntity, new();

    string ViewName { get; }

    string TableName { get; }
  }
}
