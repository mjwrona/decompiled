// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Setting.SettingResult
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Setting
{
  [DataContract]
  public class SettingResult : SearchSecuredV2Object
  {
    public SettingResult(
      string title,
      string description,
      string routeId,
      string routeParameter,
      SearchScope scope,
      string icon)
    {
      this.Title = title;
      this.Description = description;
      this.RouteId = routeId;
      this.RouteParameterMapping = SettingResult.GetRouteParameterMappingDictionary(routeParameter);
      this.Scope = scope;
      this.Icon = icon;
    }

    [DataMember(Name = "title")]
    public string Title { get; set; }

    [DataMember(Name = "description")]
    public string Description { get; set; }

    [DataMember(Name = "routeId")]
    public string RouteId { get; set; }

    [DataMember(Name = "routeParameterMapping")]
    public Dictionary<string, string> RouteParameterMapping { get; set; }

    [DataMember(Name = "scope")]
    public SearchScope Scope { get; set; }

    [DataMember(Name = "icon")]
    public string Icon { get; set; }

    public static Dictionary<string, string> GetRouteParameterMappingDictionary(
      string routeParameterMapping)
    {
      Dictionary<string, string> mappingDictionary = new Dictionary<string, string>();
      if (!string.IsNullOrWhiteSpace(routeParameterMapping))
      {
        string str1 = routeParameterMapping;
        char[] chArray1 = new char[1]{ ',' };
        foreach (string str2 in str1.Split(chArray1))
        {
          char[] chArray2 = new char[1]{ ':' };
          string[] strArray = str2.Split(chArray2);
          if (strArray.Length >= 2)
          {
            string key = strArray[0].Trim();
            string str3 = strArray[1].Trim();
            if (!mappingDictionary.ContainsKey(key))
              mappingDictionary.Add(key, str3);
          }
        }
      }
      return mappingDictionary;
    }
  }
}
