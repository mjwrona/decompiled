// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteControl.RemoteControlUri
// Assembly: Microsoft.VisualStudio.RemoteControl, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4D9D0761-3208-49DD-A9E2-BF705DBE6B5D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.RemoteControl.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.IO;

namespace Microsoft.VisualStudio.RemoteControl
{
  internal sealed class RemoteControlUri
  {
    private readonly Uri uri;

    private RemoteControlUri(Uri uri, string hostId)
    {
      this.uri = uri;
      this.IsLocalFile = this.uri.IsFile;
      this.FullUrl = this.uri.IsFile ? Path.GetFullPath(this.uri.LocalPath) : this.uri.AbsoluteUri;
      this.HostId = hostId;
    }

    public bool IsLocalFile { get; }

    public string FullUrl { get; }

    public string HostId { get; }

    public static RemoteControlUri Create(
      IRegistryTools registryTools,
      string hostId,
      string baseUrl,
      string relativePath)
    {
      registryTools.RequiresArgumentNotNull<IRegistryTools>(nameof (registryTools));
      hostId.RequiresArgumentNotNullAndNotEmpty(nameof (hostId));
      baseUrl.RequiresArgumentNotNullAndNotEmpty(nameof (baseUrl));
      relativePath.RequiresArgumentNotNullAndNotEmpty(nameof (relativePath));
      Uri uri = new Uri(baseUrl).AddSegment(hostId).AddSegment(relativePath);
      Tuple<Uri, string> tuple = uri.SplitLastSegment();
      string absoluteUri = tuple.Item1.AbsoluteUri;
      object fromCurrentUserRoot = registryTools.GetRegistryValueFromCurrentUserRoot("Software\\Microsoft\\VisualStudio\\RemoteControl\\TestUrlMapping", absoluteUri);
      if (fromCurrentUserRoot != null)
        uri = new Uri(fromCurrentUserRoot.ToString()).AddSegment(tuple.Item2);
      return new RemoteControlUri(uri, hostId);
    }
  }
}
