// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.Crs
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Spatial.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.Spatial
{
  [DataContract]
  [JsonConverter(typeof (CrsJsonConverter))]
  public abstract class Crs
  {
    protected Crs(CrsType type) => this.Type = type;

    public static Crs Default => (Crs) new NamedCrs("urn:ogc:def:crs:OGC:1.3:CRS84");

    public static Crs Unspecified => (Crs) new UnspecifiedCrs();

    [DataMember(Name = "type")]
    public CrsType Type { get; private set; }

    public static NamedCrs Named(string name) => new NamedCrs(name);

    public static LinkedCrs Linked(string href) => new LinkedCrs(href);

    public static LinkedCrs Linked(string href, string type) => new LinkedCrs(href, type);
  }
}
