// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CommitLog.Operations.NpmDeprecateOperation
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server.CommitLog.Operations
{
  public class NpmDeprecateOperation : ProtocolOperation
  {
    private static readonly Lazy<NpmDeprecateOperation> LazyInstance = new Lazy<NpmDeprecateOperation>((Func<NpmDeprecateOperation>) (() => new NpmDeprecateOperation()));

    private NpmDeprecateOperation()
      : base("Npm", "Deprecate", "1.0")
    {
    }

    public static NpmDeprecateOperation Instance { get; } = NpmDeprecateOperation.LazyInstance.Value;
  }
}
