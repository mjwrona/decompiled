// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ExtensionOperationResultExtensions
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class ExtensionOperationResultExtensions
  {
    public static ICollection<ExtensionOperationResult> ToExtensionOperationResults(
      this IEnumerable<ExtensionOperationResultInternal> results)
    {
      return (ICollection<ExtensionOperationResult>) results.Select<ExtensionOperationResultInternal, ExtensionOperationResult>((Func<ExtensionOperationResultInternal, ExtensionOperationResult>) (x => x.ToExtensionOperationResult())).ToList<ExtensionOperationResult>();
    }
  }
}
