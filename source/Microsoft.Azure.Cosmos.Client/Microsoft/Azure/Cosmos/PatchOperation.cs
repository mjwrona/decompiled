// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.PatchOperation
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using System.IO;

namespace Microsoft.Azure.Cosmos
{
  public abstract class PatchOperation
  {
    [JsonProperty(PropertyName = "op")]
    public abstract PatchOperationType OperationType { get; }

    [JsonProperty(PropertyName = "path")]
    public abstract string Path { get; }

    public virtual bool TrySerializeValueParameter(
      CosmosSerializer cosmosSerializer,
      out Stream valueParam)
    {
      valueParam = (Stream) null;
      return false;
    }

    public static PatchOperation Add<T>(string path, T value) => (PatchOperation) new PatchOperationCore<T>(PatchOperationType.Add, path, value);

    public static PatchOperation Remove(string path) => (PatchOperation) new PatchOperationCore(PatchOperationType.Remove, path);

    public static PatchOperation Replace<T>(string path, T value) => (PatchOperation) new PatchOperationCore<T>(PatchOperationType.Replace, path, value);

    public static PatchOperation Set<T>(string path, T value) => (PatchOperation) new PatchOperationCore<T>(PatchOperationType.Set, path, value);

    public static PatchOperation Increment(string path, long value) => (PatchOperation) new PatchOperationCore<long>(PatchOperationType.Increment, path, value);

    public static PatchOperation Increment(string path, double value) => (PatchOperation) new PatchOperationCore<double>(PatchOperationType.Increment, path, value);
  }
}
