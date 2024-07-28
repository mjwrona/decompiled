// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.IPipeSshChannel
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using System;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  public interface IPipeSshChannel : IDisposable
  {
    PipeReader DataIn { get; }

    PipeReader ExtendedDataIn { get; }

    PipeWriter DataOut { get; }

    Task DataOutCompletion { get; }

    PipeWriter ExtendedDataOut { get; }

    Task ExtendedDataOutCompletion { get; }

    Task EofSent { get; }

    int ExitStatus { get; set; }
  }
}
