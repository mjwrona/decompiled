// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Charts.Cache.ChartSeriesBase
// Assembly: Microsoft.Azure.Boards.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EABADF19-3537-403E-8E3C-4185CE6D1F3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Charts.dll

using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Microsoft.Azure.Boards.Charts.Cache
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class ChartSeriesBase
  {
    [DataMember(Name = "endDate", EmitDefaultValue = false)]
    public DateTime EndDate { get; set; }

    [DataMember(Name = "cacheVersion", EmitDefaultValue = false)]
    public int CacheVersion { get; protected set; }

    internal string Serialize()
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        new DataContractJsonSerializer(this.GetType()).WriteObject((Stream) memoryStream, (object) this);
        return Encoding.UTF8.GetString(memoryStream.ToArray()).ToString();
      }
    }

    internal static T Deserialize<T>(string value) where T : ChartSeriesBase
    {
      using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
        return new DataContractJsonSerializer(typeof (T)).ReadObject((Stream) memoryStream) as T;
    }
  }
}
