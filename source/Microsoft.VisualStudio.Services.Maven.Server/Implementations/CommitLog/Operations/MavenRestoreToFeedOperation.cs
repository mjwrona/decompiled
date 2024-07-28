// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.Operations.MavenRestoreToFeedOperation
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;

namespace Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.Operations
{
  public class MavenRestoreToFeedOperation : ProtocolOperation
  {
    private MavenRestoreToFeedOperation()
      : base("maven", "RestoreToFeed", "1.0")
    {
    }

    public static MavenRestoreToFeedOperation Instance { get; } = new MavenRestoreToFeedOperation();
  }
}
