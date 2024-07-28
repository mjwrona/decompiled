// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.PackageFileSerialization
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public static class PackageFileSerialization
  {
    public static void SerializeInnerFileReferences(

      #nullable disable
      JsonSerializer serializer,
      JsonWriter writer,
      IEnumerable<InnerFileReference> innerFiles)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      serializer.Serialize(writer, (object) innerFiles.Select<InnerFileReference, PackageFileSerialization.SerializedInnerFileReference>(PackageFileSerialization.\u003C\u003EO.\u003C0\u003E__ToSerializedInnerFileReference ?? (PackageFileSerialization.\u003C\u003EO.\u003C0\u003E__ToSerializedInnerFileReference = new Func<InnerFileReference, PackageFileSerialization.SerializedInnerFileReference>(PackageFileSerialization.ToSerializedInnerFileReference))));
    }

    public static string SerializeInnerFileReferences(IEnumerable<InnerFileReference> innerFiles)
    {
      using (StringWriter stringWriter = new StringWriter())
      {
        using (JsonTextWriter writer = new JsonTextWriter((TextWriter) stringWriter))
        {
          PackageFileSerialization.SerializeInnerFileReferences(JsonSerializer.CreateDefault(), (JsonWriter) writer, innerFiles);
          return stringWriter.ToString();
        }
      }
    }

    public static ImmutableArray<InnerFileReference> DeserializeInnerFileReferences(
      JsonSerializer serializer,
      JsonReader reader)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return ImmutableArray.CreateRange<InnerFileReference>(serializer.Deserialize<IEnumerable<PackageFileSerialization.SerializedInnerFileReference>>(reader).Select<PackageFileSerialization.SerializedInnerFileReference, InnerFileReference>(PackageFileSerialization.\u003C\u003EO.\u003C1\u003E__FromSerializedInnerFileReference ?? (PackageFileSerialization.\u003C\u003EO.\u003C1\u003E__FromSerializedInnerFileReference = new Func<PackageFileSerialization.SerializedInnerFileReference, InnerFileReference>(PackageFileSerialization.FromSerializedInnerFileReference))));
    }

    public static ImmutableArray<InnerFileReference> DeserializeInnerFileReferences(
      string innerFilesJson)
    {
      using (StringReader reader1 = new StringReader(innerFilesJson))
      {
        using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
          return PackageFileSerialization.DeserializeInnerFileReferences(JsonSerializer.CreateDefault(), (JsonReader) reader2);
      }
    }

    private static PackageFileSerialization.SerializedInnerFileReference ToSerializedInnerFileReference(
      InnerFileReference x)
    {
      return new PackageFileSerialization.SerializedInnerFileReference(x.Path, x.StorageId.ValueString, x.SizeInBytes);
    }

    private static InnerFileReference FromSerializedInnerFileReference(
      PackageFileSerialization.SerializedInnerFileReference x)
    {
      return new InnerFileReference(x.Path, StorageId.Parse(x.StorageId), x.SizeInBytes);
    }

    private record SerializedInnerFileReference(string Path, string StorageId, long SizeInBytes);
  }
}
