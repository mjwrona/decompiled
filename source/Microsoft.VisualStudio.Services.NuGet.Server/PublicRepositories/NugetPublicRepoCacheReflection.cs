// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories.NugetPublicRepoCacheReflection
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories
{
  [ExcludeFromCodeCoverage]
  public static class NugetPublicRepoCacheReflection
  {
    private static FileDescriptor descriptor = FileDescriptor.FromGeneratedCode(Convert.FromBase64String("Ch1udWdldF9wdWJsaWNfcmVwb19jYWNoZS5wcm90bxIUbnVnZXRwdWJsaWNy" + "ZXBvY2FjaGUaH2dvb2dsZS9wcm90b2J1Zi90aW1lc3RhbXAucHJvdG8iSQod" + "TnVHZXRQdWJDYWNoZUFsdGVybmF0ZVBhY2thZ2USCgoCaWQYASABKAkSEgoF" + "cmFuZ2UYAiABKAlIAIgBAUIICgZfcmFuZ2UivwEKH051R2V0UHViQ2FjaGVQ" + "YWNrYWdlRGVwcmVjYXRpb24SDwoHcmVhc29ucxgBIAMoCRIUCgdtZXNzYWdl" + "GAIgASgJSACIAQESUwoRYWx0ZXJuYXRlX3BhY2thZ2UYAyABKAsyMy5udWdl" + "dHB1YmxpY3JlcG9jYWNoZS5OdUdldFB1YkNhY2hlQWx0ZXJuYXRlUGFja2Fn" + "ZUgBiAEBQgoKCF9tZXNzYWdlQhQKEl9hbHRlcm5hdGVfcGFja2FnZSJEChpO" + "dUdldFB1YkNhY2hlVnVsbmVyYWJpbGl0eRIUCgxhZHZpc29yeV91cmwYASAB" + "KAkSEAoIc2V2ZXJpdHkYAiABKAkipQMKH051R2V0UHViQ2FjaGVWZXJzaW9u" + "TXV0YWJsZUluZm8SLgoKZmV0Y2hfZGF0ZRgBIAEoCzIaLmdvb2dsZS5wcm90" + "b2J1Zi5UaW1lc3RhbXASDgoGbGlzdGVkGAIgASgIEjUKDHB1Ymxpc2hfZGF0" + "ZRgDIAEoCzIaLmdvb2dsZS5wcm90b2J1Zi5UaW1lc3RhbXBIAIgBARJPCgtk" + "ZXByZWNhdGlvbhgEIAEoCzI1Lm51Z2V0cHVibGljcmVwb2NhY2hlLk51R2V0" + "UHViQ2FjaGVQYWNrYWdlRGVwcmVjYXRpb25IAYgBARJJCg92dWxuZXJhYmls" + "aXRpZXMYBSADKAsyMC5udWdldHB1YmxpY3JlcG9jYWNoZS5OdUdldFB1YkNh" + "Y2hlVnVsbmVyYWJpbGl0eRI5ChBjb21taXRfdGltZXN0YW1wGAYgASgLMhou" + "Z29vZ2xlLnByb3RvYnVmLlRpbWVzdGFtcEgCiAEBQg8KDV9wdWJsaXNoX2Rh" + "dGVCDgoMX2RlcHJlY2F0aW9uQhMKEV9jb21taXRfdGltZXN0YW1wImQKGk51" + "R2V0UHViQ2FjaGVWZXJzaW9uTnVzcGVjEi4KCmZldGNoX2RhdGUYASABKAsy" + "Gi5nb29nbGUucHJvdG9idWYuVGltZXN0YW1wEhYKDmRlZmxhdGVkX2J5dGVz" + "GAIgASgMIoMCCh1OdUdldFB1YkNhY2hlVmVyc2lvbkxldmVsSW5mbxIUCgxk" + "aXNwbGF5X25hbWUYASABKAkSFwoPZGlzcGxheV92ZXJzaW9uGAIgASgJElAK" + "DG11dGFibGVfaW5mbxgDIAEoCzI1Lm51Z2V0cHVibGljcmVwb2NhY2hlLk51" + "R2V0UHViQ2FjaGVWZXJzaW9uTXV0YWJsZUluZm9IAIgBARJFCgZudXNwZWMY" + "BCABKAsyMC5udWdldHB1YmxpY3JlcG9jYWNoZS5OdUdldFB1YkNhY2hlVmVy" + "c2lvbk51c3BlY0gBiAEBQg8KDV9tdXRhYmxlX2luZm9CCQoHX251c3BlYyKY" + "AgocTnVHZXRQdWJDYWNoZVBhY2thZ2VOYW1lRmlsZRIxCg1tb2RpZmllZF9k" + "YXRlGAEgASgLMhouZ29vZ2xlLnByb3RvYnVmLlRpbWVzdGFtcBIUCgxkaXNw" + "bGF5X25hbWUYAiABKAkSRQoIdmVyc2lvbnMYAyADKAsyMy5udWdldHB1Ymxp" + "Y3JlcG9jYWNoZS5OdUdldFB1YkNhY2hlVmVyc2lvbkxldmVsSW5mbxIYChBk" + "b2N1bWVudF92ZXJzaW9uGAQgASgDEjkKEGNvbW1pdF90aW1lc3RhbXAYBSAB" + "KAsyGi5nb29nbGUucHJvdG9idWYuVGltZXN0YW1wSACIAQFCEwoRX2NvbW1p" + "dF90aW1lc3RhbXBCQqoCP01pY3Jvc29mdC5WaXN1YWxTdHVkaW8uU2Vydmlj" + "ZXMuTnVHZXQuU2VydmVyLlB1YmxpY1JlcG9zaXRvcmllc2IGcHJvdG8z"), new FileDescriptor[1]
    {
      TimestampReflection.Descriptor
    }, new GeneratedClrTypeInfo((System.Type[]) null, (Extension[]) null, new GeneratedClrTypeInfo[7]
    {
      new GeneratedClrTypeInfo(typeof (NuGetPubCacheAlternatePackage), (MessageParser) NuGetPubCacheAlternatePackage.Parser, new string[2]
      {
        "Id",
        "Range"
      }, new string[1]{ "Range" }, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null),
      new GeneratedClrTypeInfo(typeof (NuGetPubCachePackageDeprecation), (MessageParser) NuGetPubCachePackageDeprecation.Parser, new string[3]
      {
        "Reasons",
        "Message",
        "AlternatePackage"
      }, new string[2]{ "Message", "AlternatePackage" }, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null),
      new GeneratedClrTypeInfo(typeof (NuGetPubCacheVulnerability), (MessageParser) NuGetPubCacheVulnerability.Parser, new string[2]
      {
        "AdvisoryUrl",
        "Severity"
      }, (string[]) null, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null),
      new GeneratedClrTypeInfo(typeof (NuGetPubCacheVersionMutableInfo), (MessageParser) NuGetPubCacheVersionMutableInfo.Parser, new string[6]
      {
        "FetchDate",
        "Listed",
        "PublishDate",
        "Deprecation",
        "Vulnerabilities",
        "CommitTimestamp"
      }, new string[3]
      {
        "PublishDate",
        "Deprecation",
        "CommitTimestamp"
      }, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null),
      new GeneratedClrTypeInfo(typeof (NuGetPubCacheVersionNuspec), (MessageParser) NuGetPubCacheVersionNuspec.Parser, new string[2]
      {
        "FetchDate",
        "DeflatedBytes"
      }, (string[]) null, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null),
      new GeneratedClrTypeInfo(typeof (NuGetPubCacheVersionLevelInfo), (MessageParser) NuGetPubCacheVersionLevelInfo.Parser, new string[4]
      {
        "DisplayName",
        "DisplayVersion",
        "MutableInfo",
        "Nuspec"
      }, new string[2]{ "MutableInfo", "Nuspec" }, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null),
      new GeneratedClrTypeInfo(typeof (NuGetPubCachePackageNameFile), (MessageParser) NuGetPubCachePackageNameFile.Parser, new string[5]
      {
        "ModifiedDate",
        "DisplayName",
        "Versions",
        "DocumentVersion",
        "CommitTimestamp"
      }, new string[1]{ "CommitTimestamp" }, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null)
    }));

    public static FileDescriptor Descriptor => NugetPublicRepoCacheReflection.descriptor;
  }
}
