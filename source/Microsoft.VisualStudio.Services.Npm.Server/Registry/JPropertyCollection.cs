// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Registry.JPropertyCollection
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;

namespace Microsoft.VisualStudio.Services.Npm.Server.Registry
{
  public class JPropertyCollection : KeyedCollection<string, JProperty>
  {
    private static IsoDateTimeConverter isoDateTimeConverter = new IsoDateTimeConverter();

    protected override string GetKeyForItem(JProperty item) => item.Name;

    public void Replace(JProperty oldValue, JProperty newValue)
    {
      int index = this.IndexOf(oldValue);
      if (index < 0)
        throw new ArgumentException("The object was not found in the collection", nameof (oldValue));
      this.SetItem(index, newValue);
    }

    public JProperty GetOrDefault(string key)
    {
      JProperty jproperty;
      return !this.Dictionary.TryGetValue(key, out jproperty) ? (JProperty) null : jproperty;
    }

    public string ToJsonString()
    {
      JObject jobject = new JObject();
      foreach (JProperty content in (Collection<JProperty>) this)
        jobject.Add((object) content);
      return jobject.ToString(Formatting.None, (JsonConverter) JPropertyCollection.isoDateTimeConverter);
    }
  }
}
