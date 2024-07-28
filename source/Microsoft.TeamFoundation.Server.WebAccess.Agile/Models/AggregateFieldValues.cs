// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.AggregateFieldValues
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  [DataContract]
  public class AggregateFieldValues : Dictionary<string, Dictionary<string, double>>
  {
    internal void SetFields(IEnumerable<string> aggregateFieldIds)
    {
      foreach (string aggregateFieldId in aggregateFieldIds)
        this.Add(aggregateFieldId, new Dictionary<string, double>());
    }

    internal void AddCapacity(string fieldName, string value, double capacity)
    {
      Dictionary<string, double> dictionary;
      if (!this.TryGetValue(fieldName, out dictionary))
        this[fieldName] = dictionary = new Dictionary<string, double>();
      double num;
      dictionary.TryGetValue(value, out num);
      capacity += num;
      dictionary[value] = capacity;
    }
  }
}
