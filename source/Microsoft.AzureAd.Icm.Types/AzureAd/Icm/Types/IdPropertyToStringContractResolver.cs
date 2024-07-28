// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IdPropertyToStringContractResolver
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Microsoft.AzureAd.Icm.Types
{
  public class IdPropertyToStringContractResolver : DefaultContractResolver
  {
    public static readonly IdPropertyToStringContractResolver Instance = new IdPropertyToStringContractResolver();

    protected override JsonProperty CreateProperty(
      MemberInfo member,
      MemberSerialization memberSerialization)
    {
      JsonProperty property = base.CreateProperty(member, memberSerialization);
      if (property.PropertyType == typeof (long) && StringComparer.OrdinalIgnoreCase.Equals(property.PropertyName, "Id"))
      {
        property.PropertyType = typeof (string);
        property.Converter = (JsonConverter) new LongToStringConverter();
      }
      return property;
    }
  }
}
