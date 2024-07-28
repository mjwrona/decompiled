// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ArtifactCleanupConstants
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class ArtifactCleanupConstants
  {
    public static readonly Guid DeleteFilesTaskId = new Guid("B7E8B412-0437-4065-9371-EDC5881DE25B");
    public static readonly string DeleteFilesTaskVersionSpec = "1.*";
    public static readonly string DeleteArtifactSourceFolder = "SourceFolder";
    public static readonly string DeleteArtifactContentsStr = "Contents";
    public static readonly string DeleteArtifactContents = ".";
    public static readonly string RemoveArtifactSourceFolder = "RemoveSourceFolder";
    public static readonly Guid PublishSymbolsTaskId = new Guid("0675668A-7BBA-4CCB-901D-5AD6554CA653");
    public static readonly string PublishSymbolsTaskVersionSpec = "2.*";
    public static readonly string DeleteSymbols = "Delete";
    public static readonly string FileShareType = "FileShare";
    public static readonly string SymbolsPath = nameof (SymbolsPath);
    public static readonly string SymbolServerType = nameof (SymbolServerType);
    public static readonly string TransactionId = nameof (TransactionId);
    public static readonly string BuildCleanup = nameof (BuildCleanup);
    public static readonly string TrueString = "true";
    public const string BuildOrchestrationType = "BuildOrchestrationType";
    public const string BuildCleanupAgentCapability = "Build.Cleanup";
    public static readonly Version RequiredAgentVersion = new Version("1.97.0");
  }
}
