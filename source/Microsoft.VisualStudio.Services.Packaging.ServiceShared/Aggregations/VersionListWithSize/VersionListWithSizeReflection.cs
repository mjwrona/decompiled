// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize.VersionListWithSizeReflection
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize
{
  public static class VersionListWithSizeReflection
  {
    private static FileDescriptor descriptor = FileDescriptor.FromGeneratedCode(Convert.FromBase64String("Chx2ZXJzaW9uX2xpc3Rfd2l0aF9zaXplLnByb3RvEhN2ZXJzaW9ubGlzdHdp" + "dGhzaXplGh9nb29nbGUvcHJvdG9idWYvdGltZXN0YW1wLnByb3RvImcKJVZl" + "cnNpb25MaXN0V2l0aFNpemVQYWNrYWdlVmVyc2lvbkZpbGUSHAoUY29tcHJl" + "c3NlZF9maWxlX25hbWUYASABKAkSFQoNc2l6ZV9pbl9ieXRlcxgCIAEoAVIJ" + "ZmlsZV9uYW1lIr8BCiFWZXJzaW9uTGlzdFdpdGhTaXplUGFja2FnZVZlcnNp" + "b24SIgoaY29tcHJlc3NlZF9kaXNwbGF5X3ZlcnNpb24YASABKAkSEgoKaXNf" + "ZGVsZXRlZBgCIAEoCBJRCg1wYWNrYWdlX2ZpbGVzGAMgAygLMjoudmVyc2lv" + "bmxpc3R3aXRoc2l6ZS5WZXJzaW9uTGlzdFdpdGhTaXplUGFja2FnZVZlcnNp" + "b25GaWxlUg9kaXNwbGF5X3ZlcnNpb24iyAEKGlZlcnNpb25MaXN0V2l0aFNp" + "emVQYWNrYWdlEh8KF2NvbXByZXNzZWRfZGlzcGxheV9uYW1lGAEgASgJEkgK" + "CHZlcnNpb25zGAIgAygLMjYudmVyc2lvbmxpc3R3aXRoc2l6ZS5WZXJzaW9u" + "TGlzdFdpdGhTaXplUGFja2FnZVZlcnNpb24SMQoNbGFzdF9tb2RpZmllZBgD" + "IAEoCzIaLmdvb2dsZS5wcm90b2J1Zi5UaW1lc3RhbXBSDGRpc3BsYXlfbmFt" + "ZSKVAQoXVmVyc2lvbkxpc3RXaXRoU2l6ZUZpbGUSQQoIcGFja2FnZXMYASAD" + "KAsyLy52ZXJzaW9ubGlzdHdpdGhzaXplLlZlcnNpb25MaXN0V2l0aFNpemVQ" + "YWNrYWdlEjEKDWxhc3RfbW9kaWZpZWQYAiABKAsyGi5nb29nbGUucHJvdG9i" + "dWYuVGltZXN0YW1wSgQIDxAQQluqAlhNaWNyb3NvZnQuVmlzdWFsU3R1ZGlv" + "LlNlcnZpY2VzLlBhY2thZ2luZy5TZXJ2aWNlU2hhcmVkLkFnZ3JlZ2F0aW9u" + "cy5WZXJzaW9uTGlzdFdpdGhTaXplYgZwcm90bzM="), new FileDescriptor[1]
    {
      TimestampReflection.Descriptor
    }, new GeneratedClrTypeInfo((System.Type[]) null, (Extension[]) null, new GeneratedClrTypeInfo[4]
    {
      new GeneratedClrTypeInfo(typeof (VersionListWithSizePackageVersionFile), (MessageParser) VersionListWithSizePackageVersionFile.Parser, new string[2]
      {
        "CompressedFileName",
        "SizeInBytes"
      }, (string[]) null, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null),
      new GeneratedClrTypeInfo(typeof (VersionListWithSizePackageVersion), (MessageParser) VersionListWithSizePackageVersion.Parser, new string[3]
      {
        "CompressedDisplayVersion",
        "IsDeleted",
        "PackageFiles"
      }, (string[]) null, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null),
      new GeneratedClrTypeInfo(typeof (VersionListWithSizePackage), (MessageParser) VersionListWithSizePackage.Parser, new string[3]
      {
        "CompressedDisplayName",
        "Versions",
        "LastModified"
      }, (string[]) null, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null),
      new GeneratedClrTypeInfo(typeof (VersionListWithSizeFile), (MessageParser) VersionListWithSizeFile.Parser, new string[2]
      {
        "Packages",
        "LastModified"
      }, (string[]) null, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null)
    }));

    public static FileDescriptor Descriptor => VersionListWithSizeReflection.descriptor;
  }
}
