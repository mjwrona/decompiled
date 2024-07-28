// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.BatchSchemaProvider
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using System;
using System.IO;
using System.Reflection;

namespace Microsoft.Azure.Cosmos
{
  internal static class BatchSchemaProvider
  {
    static BatchSchemaProvider()
    {
      BatchSchemaProvider.BatchSchemaNamespace = Namespace.Parse(BatchSchemaProvider.GetEmbeddedResource("Batch\\HybridRowBatchSchemas.json"));
      BatchSchemaProvider.BatchLayoutResolver = new LayoutResolverNamespace(BatchSchemaProvider.BatchSchemaNamespace);
      BatchSchemaProvider.BatchOperationLayout = BatchSchemaProvider.BatchLayoutResolver.Resolve(BatchSchemaProvider.BatchSchemaNamespace.Schemas.Find((Predicate<Schema>) (x => x.Name == "BatchOperation")).SchemaId);
      BatchSchemaProvider.BatchResultLayout = BatchSchemaProvider.BatchLayoutResolver.Resolve(BatchSchemaProvider.BatchSchemaNamespace.Schemas.Find((Predicate<Schema>) (x => x.Name == "BatchResult")).SchemaId);
    }

    public static Namespace BatchSchemaNamespace { get; private set; }

    public static LayoutResolverNamespace BatchLayoutResolver { get; private set; }

    public static Layout BatchOperationLayout { get; private set; }

    public static Layout BatchResultLayout { get; private set; }

    private static string GetEmbeddedResource(string resourceName)
    {
      Assembly assembly = Assembly.GetAssembly(typeof (BatchSchemaProvider));
      resourceName = BatchSchemaProvider.FormatResourceName(typeof (BatchSchemaProvider).Namespace, resourceName);
      string name = resourceName;
      using (Stream manifestResourceStream = assembly.GetManifestResourceStream(name))
      {
        if (manifestResourceStream == null)
          return (string) null;
        using (StreamReader streamReader = new StreamReader(manifestResourceStream))
          return streamReader.ReadToEnd();
      }
    }

    private static string FormatResourceName(string namespaceName, string resourceName) => namespaceName + "." + resourceName.Replace(" ", "_").Replace("\\", ".").Replace("/", ".");
  }
}
