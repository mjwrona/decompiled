// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitPackDeserializerSplitterTrait
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class GitPackDeserializerSplitterTrait : IGitPackDeserializerTrait
  {
    public GitPackDeserializerSplitterTrait(int packSoftCapSize) => this.PackSplitter = new GitPackSplitter(packSoftCapSize);

    public SplitPackResult Result => this.PackSplitter.ProcessPendingObjectInfo();

    public GitPackSplitter PackSplitter { get; }

    public void AddToDeserializer(GitPackDeserializer deserializer) => deserializer.ObjectInfo += (GitPackDeserializer.ObjectInfoHandler) ((progress, objectId, objectType, objectLength, offsetInPack, lengthInPack) => this.PackSplitter.AddPendingObject(objectId, objectType, offsetInPack, lengthInPack));
  }
}
