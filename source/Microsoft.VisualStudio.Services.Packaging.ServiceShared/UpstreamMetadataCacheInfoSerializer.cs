// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamMetadataCacheInfoSerializer
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class UpstreamMetadataCacheInfoSerializer : ISerializer<UpstreamMetadataCacheInfo>
  {
    private readonly IConverter<string, IPackageName> converter;

    public UpstreamMetadataCacheInfoSerializer(IConverter<string, IPackageName> converter) => this.converter = converter;

    private ISerializer<UpstreamMetadataCacheInfoSerializer.UpstreamPackageNameModel> InternalSerializer => (ISerializer<UpstreamMetadataCacheInfoSerializer.UpstreamPackageNameModel>) new JsonSerializer<UpstreamMetadataCacheInfoSerializer.UpstreamPackageNameModel>(new JsonSerializerSettings());

    public UpstreamMetadataCacheInfo Deserialize(Stream s)
    {
      ArgumentUtility.CheckForNull<Stream>(s, nameof (s));
      UpstreamMetadataCacheInfoSerializer.UpstreamPackageNameModel packageNameModel = this.InternalSerializer.Deserialize(s);
      SortedSet<string> packages = packageNameModel.Packages;
      return (packages != null ? (packages.Any<string>() ? 1 : 0) : 0) != 0 ? new UpstreamMetadataCacheInfo(this.converter.ConvertMultiple<string, IPackageName>((IEnumerable<string>) packageNameModel.Packages)) : (UpstreamMetadataCacheInfo) null;
    }

    public byte[] Serialize(UpstreamMetadataCacheInfo input)
    {
      UpstreamMetadataCacheInfoSerializer.UpstreamPackageNameModel input1 = new UpstreamMetadataCacheInfoSerializer.UpstreamPackageNameModel();
      bool? nullable;
      if (input == null)
      {
        nullable = new bool?();
      }
      else
      {
        ISet<IPackageName> packageNames = input.PackageNames;
        nullable = packageNames != null ? new bool?(packageNames.Any<IPackageName>()) : new bool?();
      }
      if (nullable.GetValueOrDefault())
        input1.Packages = new SortedSet<string>(input.PackageNames.Select<IPackageName, string>((Func<IPackageName, string>) (n => n.DisplayName)));
      return this.InternalSerializer.Serialize(input1);
    }

    [DataContract]
    private class UpstreamPackageNameModel
    {
      [DataMember(EmitDefaultValue = false)]
      public SortedSet<string> Packages;
    }
  }
}
