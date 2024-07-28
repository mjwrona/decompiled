// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.PackageVersionCountsReflection
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  public static class PackageVersionCountsReflection
  {
    private static FileDescriptor descriptor = FileDescriptor.FromGeneratedCode(Convert.FromBase64String("ChxwYWNrYWdlX3ZlcnNpb25fY291bnRzLnByb3RvEhRwYWNrYWdldmVyc2lv" + "bmNvdW50cxofZ29vZ2xlL3Byb3RvYnVmL3RpbWVzdGFtcC5wcm90byJ2ChhW" + "ZXJzaW9uQ291bnRzUGFja2FnZVZpZXcSEgoKdmlld19pbmRleBgBIAEoBRIN" + "CgVjb3VudBgCIAEoBRI3CgVmbGFncxgDIAEoDjIoLnBhY2thZ2V2ZXJzaW9u" + "Y291bnRzLkxhdGVzdFZlcnNpb25GbGFncyK3AQoUVmVyc2lvbkNvdW50c1Bh" + "Y2thZ2USHwoXY29tcHJlc3NlZF9kaXNwbGF5X25hbWUYASABKAkSPQoFdmll" + "d3MYAiADKAsyLi5wYWNrYWdldmVyc2lvbmNvdW50cy5WZXJzaW9uQ291bnRz" + "UGFja2FnZVZpZXcSMQoNbGFzdF9tb2RpZmllZBgDIAEoCzIaLmdvb2dsZS5w" + "cm90b2J1Zi5UaW1lc3RhbXBSDGRpc3BsYXlfbmFtZSJmChFWZXJzaW9uQ291" + "bnRzRmlsZRITCgtrbm93bl92aWV3cxgBIAMoDBI8CghwYWNrYWdlcxgCIAMo" + "CzIqLnBhY2thZ2V2ZXJzaW9uY291bnRzLlZlcnNpb25Db3VudHNQYWNrYWdl" + "IpoBChpWZXJzaW9uTGlzdHNQYWNrYWdlVmVyc2lvbhIiChpjb21wcmVzc2Vk" + "X2Rpc3BsYXlfdmVyc2lvbhgBIAEoCRIUCgx2aWV3X2luZGljZXMYAiADKAUS" + "EwoLaXNfdW5saXN0ZWQYAyABKAgSEgoKaXNfZGVsZXRlZBgEIAEoCFIPZGlz" + "cGxheV92ZXJzaW9uUgh2aWV3X2lkcyK7AQoTVmVyc2lvbkxpc3RzUGFja2Fn" + "ZRIfChdjb21wcmVzc2VkX2Rpc3BsYXlfbmFtZRgBIAEoCRJCCgh2ZXJzaW9u" + "cxgCIAMoCzIwLnBhY2thZ2V2ZXJzaW9uY291bnRzLlZlcnNpb25MaXN0c1Bh" + "Y2thZ2VWZXJzaW9uEjEKDWxhc3RfbW9kaWZpZWQYAyABKAsyGi5nb29nbGUu" + "cHJvdG9idWYuVGltZXN0YW1wUgxkaXNwbGF5X25hbWUinQEKEFZlcnNpb25M" + "aXN0c0ZpbGUSEwoLa25vd25fdmlld3MYASADKAwSOwoIcGFja2FnZXMYAiAD" + "KAsyKS5wYWNrYWdldmVyc2lvbmNvdW50cy5WZXJzaW9uTGlzdHNQYWNrYWdl" + "EjEKDWxhc3RfbW9kaWZpZWQYAyABKAsyGi5nb29nbGUucHJvdG9idWYuVGlt" + "ZXN0YW1wSgQIDxAQKrcBChJMYXRlc3RWZXJzaW9uRmxhZ3MSGwoXTEFURVNU" + "VkVSU0lPTkZMQUdTX05PTkUQABIhCh1MQVRFU1RWRVJTSU9ORkxBR1NfSEFT" + "X0xBVEVTVBABEioKJkxBVEVTVFZFUlNJT05GTEFHU19IQVNfQUJTT0xVVEVf" + "TEFURVNUEAISNQoxTEFURVNUVkVSU0lPTkZMQUdTX0hBU19MQVRFU1RfQU5E" + "X0FCU09MVVRFX0xBVEVTVBADQl+qAlxNaWNyb3NvZnQuVmlzdWFsU3R1ZGlv" + "LlNlcnZpY2VzLk51R2V0LlNlcnZlci5CbG9iUHJvdG90eXBlLkFnZ3JlZ2F0" + "aW9ucy5QYWNrYWdlVmVyc2lvbkNvdW50c2IGcHJvdG8z"), new FileDescriptor[1]
    {
      TimestampReflection.Descriptor
    }, new GeneratedClrTypeInfo(new System.Type[1]
    {
      typeof (LatestVersionFlags)
    }, (Extension[]) null, new GeneratedClrTypeInfo[6]
    {
      new GeneratedClrTypeInfo(typeof (VersionCountsPackageView), (MessageParser) VersionCountsPackageView.Parser, new string[3]
      {
        "ViewIndex",
        "Count",
        "Flags"
      }, (string[]) null, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null),
      new GeneratedClrTypeInfo(typeof (VersionCountsPackage), (MessageParser) VersionCountsPackage.Parser, new string[3]
      {
        "CompressedDisplayName",
        "Views",
        "LastModified"
      }, (string[]) null, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null),
      new GeneratedClrTypeInfo(typeof (VersionCountsFile), (MessageParser) VersionCountsFile.Parser, new string[2]
      {
        "KnownViews",
        "Packages"
      }, (string[]) null, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null),
      new GeneratedClrTypeInfo(typeof (VersionListsPackageVersion), (MessageParser) VersionListsPackageVersion.Parser, new string[4]
      {
        "CompressedDisplayVersion",
        "ViewIndices",
        "IsUnlisted",
        "IsDeleted"
      }, (string[]) null, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null),
      new GeneratedClrTypeInfo(typeof (VersionListsPackage), (MessageParser) VersionListsPackage.Parser, new string[3]
      {
        "CompressedDisplayName",
        "Versions",
        "LastModified"
      }, (string[]) null, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null),
      new GeneratedClrTypeInfo(typeof (VersionListsFile), (MessageParser) VersionListsFile.Parser, new string[3]
      {
        "KnownViews",
        "Packages",
        "LastModified"
      }, (string[]) null, (System.Type[]) null, (Extension[]) null, (GeneratedClrTypeInfo[]) null)
    }));

    public static FileDescriptor Descriptor => PackageVersionCountsReflection.descriptor;
  }
}
