// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing.ExceptionBlackListMarker
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing
{
  public class ExceptionBlackListMarker : IShouldMark
  {
    private readonly IReadOnlyList<ExceptionFilter> exceptionFilters;

    public ExceptionBlackListMarker(IReadOnlyList<ExceptionFilter> exceptionFilters) => this.exceptionFilters = exceptionFilters;

    public bool ShouldMark(Exception e)
    {
      foreach (ExceptionFilter exceptionFilter in (IEnumerable<ExceptionFilter>) this.exceptionFilters)
      {
        if (exceptionFilter.ExceptionType == e.GetType() && (exceptionFilter.ExceptionMessage == null || e.Message.Contains(exceptionFilter.ExceptionMessage)))
          return true;
      }
      return false;
    }
  }
}
