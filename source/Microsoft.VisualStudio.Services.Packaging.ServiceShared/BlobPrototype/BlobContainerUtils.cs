// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.BlobContainerUtils
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public static class BlobContainerUtils
  {
    private static readonly string EmptyGuidString = Guid.Empty.ToString("N");

    public static string ContainerAddressToName(ContainerAddress containerAddress) => ((containerAddress.CollectionId?.Guid.ToString("N") ?? BlobContainerUtils.EmptyGuidString) + string.Join("-", (IEnumerable<string>) containerAddress.Path.PathSegments)).ToLowerInvariant();

    public static ContainerAddress? ContainerNameToAddress(string containerName)
    {
      string[] source = containerName.Split('-');
      if (source.Length < 2)
        return (ContainerAddress) null;
      string str = source[1];
      if (!containerName.StartsWith(CodeOnlyDeploymentsConstants.CodeOnlyContainerPrefix))
        return (ContainerAddress) null;
      Guid result;
      if (str.Length <= 32 || !Guid.TryParse(str.Remove(32), out result))
        return (ContainerAddress) null;
      CollectionId collectionId = result != Guid.Empty ? new CollectionId(result) : (CollectionId) null;
      Locator locator = new Locator(new string[1]
      {
        str.Remove(0, 32)
      });
      List<string> list = ((IEnumerable<string>) source).Skip<string>(2).ToList<string>();
      if (list.Any<string>())
        locator = new Locator(locator, new Locator((IEnumerable<string>) list));
      return new ContainerAddress(collectionId, locator);
    }
  }
}
