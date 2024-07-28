// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.PackageFile
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  [DataContract]
  public class PackageFile : FeedSecuredObject
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ProtocolMetadata ProtocolMetadata { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<PackageFile> Children { get; set; }

    public override string ToString() => !string.IsNullOrWhiteSpace(this.Name) ? this.Name : base.ToString();

    public T Data<T>(bool validate = false) where T : class
    {
      if (this.ProtocolMetadata?.Data is JObject data1)
        this.ProtocolMetadata.Data = (object) data1.ToObject<T>();
      T data2 = (T) this.ProtocolMetadata?.Data;
      if (validate)
        ArgumentUtility.CheckForNull<T>(data2, "ProtocolMetadata");
      if ((object) data2 == null)
        this.ProtocolMetadata = new ProtocolMetadata()
        {
          SchemaVersion = 1,
          Data = (object) Activator.CreateInstance<T>()
        };
      return (T) this.ProtocolMetadata.Data;
    }
  }
}
