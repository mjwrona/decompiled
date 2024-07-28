// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ThrowWithBadInputMessageHandler`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class ThrowWithBadInputMessageHandler<TInputType, TOp> : 
    IAsyncHandler<TInputType, TOp>,
    IHaveInputType<TInputType>,
    IHaveOutputType<TOp>
    where TOp : ICommitOperationData
  {
    private readonly IEnumerable<string> parametersExpected;

    public ThrowWithBadInputMessageHandler(IEnumerable<string> parametersExpected) => this.parametersExpected = parametersExpected;

    public Task<TOp> Handle(TInputType request) => throw ExceptionHelper.ArgumentMissing(this.parametersExpected.Aggregate<string>((Func<string, string, string>) ((x, y) => x + " or " + y)));
  }
}
