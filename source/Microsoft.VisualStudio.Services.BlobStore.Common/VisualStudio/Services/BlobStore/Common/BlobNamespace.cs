// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.BlobNamespace
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public static class BlobNamespace
  {
    public static readonly Guid NamespaceId = new Guid("19F9F97D-7CB7-45F7-8160-DD308A6BD48E");
    public static readonly string RootToken = "$";
    public static readonly char PathSeparator = '/';
    public static readonly string BlobsToken = BlobNamespace.RootToken + BlobNamespace.PathSeparator.ToString() + "blobs";
    public static readonly string ReferencesToken = BlobNamespace.RootToken + BlobNamespace.PathSeparator.ToString() + "references";

    [Flags]
    public enum Permissions
    {
      Read = 1,
      Delete = 2,
      Create = 4,
      All = Create | Delete | Read, // 0x00000007
    }
  }
}
