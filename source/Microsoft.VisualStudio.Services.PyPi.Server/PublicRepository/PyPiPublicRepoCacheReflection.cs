// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository.PyPiPublicRepoCacheReflection
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using System;

namespace Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository
{
  public static class PyPiPublicRepoCacheReflection
  {
    private static FileDescriptor descriptor = FileDescriptor.FromGeneratedCode(Convert.FromBase64String("Ch1weV9waV9wdWJsaWNfcmVwb19jYWNoZS5wcm90bxITcHlwaXB1YmxpY3Jl" + "cG9jYWNoZRofZ29vZ2xlL3Byb3RvYnVmL3RpbWVzdGFtcC5wcm90bxoeZ29v" + "Z2xlL3Byb3RvYnVmL3dyYXBwZXJzLnByb3RvIoMHCidQeVBpUHViQ2FjaGVQ" + "YWNrYWdlVmVyc2lvbkZpbGVMZXZlbEluZm8SEAoIZmlsZW5hbWUYASABKAkS" + "CwoDdXJsGAIgASgJElgKBmhhc2hlcxgDIAMoCzJILnB5cGlwdWJsaWNyZXBv" + "Y2FjaGUuUHlQaVB1YkNhY2hlUGFja2FnZVZlcnNpb25GaWxlTGV2ZWxJbmZv" + "Lkhhc2hlc0VudHJ5EjUKD3JlcXVpcmVzX3B5dGhvbhgEIAEoCzIcLmdvb2ds" + "ZS5wcm90b2J1Zi5TdHJpbmdWYWx1ZRJrChNjb3JlX21ldGFkYXRhX3N0YXRl" + "GAUgASgLMk4ucHlwaXB1YmxpY3JlcG9jYWNoZS5QeVBpUHViQ2FjaGVQYWNr" + "YWdlVmVyc2lvbkZpbGVMZXZlbEluZm8uQ29yZU1ldGFkYXRhU3RhdGUSKwoH" + "Z3BnX3NpZxgGIAEoCzIaLmdvb2dsZS5wcm90b2J1Zi5Cb29sVmFsdWUSWgoK" + "eWFua19zdGF0ZRgHIAEoCzJGLnB5cGlwdWJsaWNyZXBvY2FjaGUuUHlQaVB1" + "YkNhY2hlUGFja2FnZVZlcnNpb25GaWxlTGV2ZWxJbmZvLllhbmtTdGF0ZRI0" + "Cgt1cGxvYWRfdGltZRgIIAEoCzIaLmdvb2dsZS5wcm90b2J1Zi5UaW1lc3Rh" + "bXBIAIgBARIMCgRzaXplGAkgASgEEi8KCWRpc3RfdHlwZRgKIAEoCzIcLmdv" + "b2dsZS5wcm90b2J1Zi5TdHJpbmdWYWx1ZRotCgtIYXNoZXNFbnRyeRILCgNr" + "ZXkYASABKAkSDQoFdmFsdWUYAiABKAw6AjgBGskBChFDb3JlTWV0YWRhdGFT" + "dGF0ZRIZChFoYXNfY29yZV9tZXRhZGF0YRgBIAEoCBJqCgZoYXNoZXMYAiAD" + "KAsyWi5weXBpcHVibGljcmVwb2NhY2hlLlB5UGlQdWJDYWNoZVBhY2thZ2VW" + "ZXJzaW9uRmlsZUxldmVsSW5mby5Db3JlTWV0YWRhdGFTdGF0ZS5IYXNoZXNF" + "bnRyeRotCgtIYXNoZXNFbnRyeRILCgNrZXkYASABKAkSDQoFdmFsdWUYAiAB" + "KAw6AjgBGjIKCVlhbmtTdGF0ZRIOCgZ5YW5rZWQYASABKAgSFQoNeWFua2Vk" + "X3JlYXNvbhgCIAEoCUIOCgxfdXBsb2FkX3RpbWUimgEKHFB5UGlQdWJDYWNo" + "ZVZlcnNpb25MZXZlbEluZm8SFAoMZGlzcGxheV9uYW1lGAEgASgJEhcKD2Rp" + "c3BsYXlfdmVyc2lvbhgCIAEoCRJLCgVmaWxlcxgDIAMoCzI8LnB5cGlwdWJs" + "aWNyZXBvY2FjaGUuUHlQaVB1YkNhY2hlUGFja2FnZVZlcnNpb25GaWxlTGV2" + "ZWxJbmZvIvgBChtQeVBpUHViQ2FjaGVQYWNrYWdlTmFtZUZpbGUSMQoNbW9k" + "aWZpZWRfZGF0ZRgBIAEoCzIaLmdvb2dsZS5wcm90b2J1Zi5UaW1lc3RhbXAS" + "FAoMZGlzcGxheV9uYW1lGAIgASgJEkMKCHZlcnNpb25zGAMgAygLMjEucHlw" + "aXB1YmxpY3JlcG9jYWNoZS5QeVBpUHViQ2FjaGVWZXJzaW9uTGV2ZWxJbmZv" + "EhgKEGRvY3VtZW50X3ZlcnNpb24YBCABKAMSMQoLbGFzdF9zZXJpYWwYBSAB" + "KAsyHC5nb29nbGUucHJvdG9idWYuVUludDY0VmFsdWVCP6oCPE1pY3Jvc29m" + "dC5WaXN1YWxTdHVkaW8uU2VydmljZXMuUHlQaS5TZXJ2ZXIuUHVibGljUmVw" + "b3NpdG9yeWIGcHJvdG8z"), new FileDescriptor[2]
    {
      TimestampReflection.Descriptor,
      WrappersReflection.Descriptor
    }, new GeneratedClrTypeInfo((System.Type[]) null, (Extension[]) null, new GeneratedClrTypeInfo[3]
    {
      new GeneratedClrTypeInfo(typeof (PyPiPubCachePackageVersionFileLevelInfo), (MessageParser) PyPiPubCachePackageVersionFileLevelInfo.Parser, new string[10]
      {
        "Filename",
        "Url",
        "Hashes",
        "RequiresPython",
        "CoreMetadataState",
        "GpgSig",
        "YankState",
        "UploadTime",
        "Size",
        "DistType"
      }, new string[1]{ "UploadTime" }, (System.Type[]) null, (Extension[]) null, new GeneratedClrTypeInfo[3]
      {
        null,
        new GeneratedClrTypeInfo(typeof (PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState), (MessageParser) PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState.Parser, new string[2]
        {
          "HasCoreMetadata",
          "Hashes"
        }, (string[]) null, (System.Type[]) null, (Extension[]) null, new GeneratedClrTypeInfo[1]),
        new GeneratedClrTypeInfo(typeof (PyPiPubCachePackageVersionFileLevelInfo.Types.YankState), (MessageParser) PyPiPubCachePackageVersionFileLevelInfo.Types.YankState.Parser, new string[2]
        {
          "Yanked",
          "YankedReason"
        }, (string[]) null, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null)
      }),
      new GeneratedClrTypeInfo(typeof (PyPiPubCacheVersionLevelInfo), (MessageParser) PyPiPubCacheVersionLevelInfo.Parser, new string[3]
      {
        "DisplayName",
        "DisplayVersion",
        "Files"
      }, (string[]) null, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null),
      new GeneratedClrTypeInfo(typeof (PyPiPubCachePackageNameFile), (MessageParser) PyPiPubCachePackageNameFile.Parser, new string[5]
      {
        "ModifiedDate",
        "DisplayName",
        "Versions",
        "DocumentVersion",
        "LastSerial"
      }, (string[]) null, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null)
    }));

    public static FileDescriptor Descriptor => PyPiPublicRepoCacheReflection.descriptor;
  }
}
