// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.LocationContext
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation
{
  public sealed class LocationContext : IJsonSerializable
  {
    private readonly IDictionary<string, string> tags;

    internal LocationContext(IDictionary<string, string> tags) => this.tags = tags;

    public string Ip
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.LocationIp);
      set
      {
        if (value == null || !this.IsIpV4(value))
          return;
        this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.LocationIp, value);
      }
    }

    void IJsonSerializable.Serialize(IJsonWriter writer)
    {
      writer.WriteStartObject();
      writer.WriteProperty("ip", this.Ip);
      writer.WriteEndObject();
    }

    private bool IsIpV4(string ip)
    {
      if (ip.Length > 15 || ip.Length < 7 || ip.Cast<char>().Any<char>((Func<char, bool>) (c => (c < '0' || c > '9') && c != '.')))
        return false;
      string[] strArray = ip.Split('.');
      if (strArray.Length != 4)
        return false;
      foreach (string s in strArray)
      {
        if (!byte.TryParse(s, out byte _))
          return false;
      }
      return true;
    }
  }
}
