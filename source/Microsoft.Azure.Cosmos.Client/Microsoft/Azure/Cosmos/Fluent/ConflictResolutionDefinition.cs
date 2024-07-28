// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Fluent.ConflictResolutionDefinition
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.Fluent
{
  public class ConflictResolutionDefinition
  {
    private readonly ContainerBuilder parent;
    private readonly Action<ConflictResolutionPolicy> attachCallback;
    private string conflictResolutionPath;
    private string conflictResolutionProcedure;

    internal ConflictResolutionDefinition(
      ContainerBuilder parent,
      Action<ConflictResolutionPolicy> attachCallback)
    {
      this.parent = parent;
      this.attachCallback = attachCallback;
    }

    public ConflictResolutionDefinition WithLastWriterWinsResolution(string conflictResolutionPath)
    {
      this.conflictResolutionPath = !string.IsNullOrEmpty(conflictResolutionPath) ? conflictResolutionPath : throw new ArgumentNullException(nameof (conflictResolutionPath));
      return this;
    }

    public ConflictResolutionDefinition WithCustomStoredProcedureResolution(
      string conflictResolutionProcedure)
    {
      this.conflictResolutionProcedure = !string.IsNullOrEmpty(conflictResolutionProcedure) ? conflictResolutionProcedure : throw new ArgumentNullException(nameof (conflictResolutionProcedure));
      return this;
    }

    public ContainerBuilder Attach()
    {
      ConflictResolutionPolicy resolutionPolicy = new ConflictResolutionPolicy();
      if (this.conflictResolutionPath != null)
      {
        resolutionPolicy.Mode = ConflictResolutionMode.LastWriterWins;
        resolutionPolicy.ResolutionPath = this.conflictResolutionPath;
      }
      if (this.conflictResolutionProcedure != null)
      {
        resolutionPolicy.Mode = ConflictResolutionMode.Custom;
        resolutionPolicy.ResolutionProcedure = this.conflictResolutionProcedure;
      }
      this.attachCallback(resolutionPolicy);
      return this.parent;
    }
  }
}
