// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileServiceContentValidationResult
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class FileServiceContentValidationResult
  {
    public int ProcessedFiles { get; internal set; }

    public int SubmittedFiles { get; internal set; }

    public int ProcessedBatches { get; internal set; }

    public int CallsToCVS { get; internal set; }

    public int ResolvedIdentities { get; internal set; }

    public int UnresolvedIdentities { get; internal set; }

    public int ScanTypeQueries { get; internal set; }

    public long SubmissionTime { get; internal set; }

    public long TotalTime { get; internal set; }

    public string ErrorMessage { get; internal set; }

    public bool IsSuccessful => this.ErrorMessage == null;
  }
}
