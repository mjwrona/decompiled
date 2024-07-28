// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsErrorReporting.BucketFilter
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Telemetry.WindowsErrorReporting
{
  [DataContract]
  public sealed class BucketFilter
  {
    private static string[] bucketParameterNames = new string[10]
    {
      "P1",
      "P2",
      "P3",
      "P4",
      "P5",
      "P6",
      "P7",
      "P8",
      "P9",
      "P10"
    };

    public static int IndexOfBucketParameter(string bucketParameterName) => Array.IndexOf<string>(BucketFilter.bucketParameterNames, bucketParameterName);

    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string WatsonEventType { get; set; }

    [DataMember]
    public string[] BucketParameterFilters { get; set; } = new string[10];

    [DataMember]
    public Dictionary<string, string> AdditionalProperties { get; set; } = new Dictionary<string, string>();

    public BucketFilter(Guid id, string watsonEventType)
    {
      this.Id = id;
      this.WatsonEventType = watsonEventType;
    }
  }
}
