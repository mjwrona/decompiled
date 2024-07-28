// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.BatchOperationType
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9764DF62-33FE-41B6-9E79-DE201B497BE0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.dll

namespace Microsoft.VisualStudio.Services.Packaging.Shared.WebApi
{
  public class BatchOperationType : IBatchOperationType
  {
    public static readonly IBatchOperationType Promote = (IBatchOperationType) new BatchOperationType(nameof (Promote));
    public static readonly IBatchOperationType PermanentDelete = (IBatchOperationType) new BatchOperationType(nameof (PermanentDelete));
    public static readonly IBatchOperationType RestoreToFeed = (IBatchOperationType) new BatchOperationType(nameof (RestoreToFeed));
    public static readonly IBatchOperationType Delete = (IBatchOperationType) new BatchOperationType(nameof (Delete));

    public BatchOperationType(string name) => this.Name = name;

    public string Name { get; }

    public override string ToString() => this.Name;
  }
}
