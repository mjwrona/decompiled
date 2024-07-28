// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldDefinitionCollection
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class FieldDefinitionCollection : ReadOnlyCollection<FieldDefinition>
  {
    private Lazy<Dictionary<string, FieldDefinition>> lazyFieldsByName;
    private Lazy<Dictionary<int, FieldDefinition>> lazyFieldsById;

    private Dictionary<string, FieldDefinition> FieldsByName => this.lazyFieldsByName.Value;

    private Dictionary<int, FieldDefinition> FieldsById => this.lazyFieldsById.Value;

    protected FieldDefinitionCollection()
      : base((IList<FieldDefinition>) new List<FieldDefinition>())
    {
      this.PopulateMaps();
    }

    internal FieldDefinitionCollection(IEnumerable<FieldDefinition> fields)
      : base((IList<FieldDefinition>) fields.ToList<FieldDefinition>())
    {
      this.PopulateMaps();
    }

    private void PopulateMaps()
    {
      this.lazyFieldsById = new Lazy<Dictionary<int, FieldDefinition>>((Func<Dictionary<int, FieldDefinition>>) (() => this.Items.ToDictionary<FieldDefinition, int>((Func<FieldDefinition, int>) (fd => fd.Id))), true);
      this.lazyFieldsByName = new Lazy<Dictionary<string, FieldDefinition>>((Func<Dictionary<string, FieldDefinition>>) (() =>
      {
        Dictionary<string, FieldDefinition> dictionary = this.Items.ToDictionary<FieldDefinition, string>((Func<FieldDefinition, string>) (fd => fd.Name), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
        foreach (FieldDefinition fieldDefinition in (IEnumerable<FieldDefinition>) this.Items)
          dictionary[fieldDefinition.ReferenceName] = fieldDefinition;
        return dictionary;
      }), true);
    }

    public virtual FieldDefinition GetById(CoreField coreField) => this.GetById((int) coreField);

    public virtual FieldDefinition GetById(int fieldId) => this.FieldsById[fieldId];

    public virtual FieldDefinition TryGetById(int fieldId)
    {
      FieldDefinition field;
      return this.TryGetById(fieldId, out field) ? field : (FieldDefinition) null;
    }

    public virtual bool TryGetById(int fieldId, out FieldDefinition field) => this.FieldsById.TryGetValue(fieldId, out field);

    public virtual FieldDefinition GetByName(string fieldName) => this.FieldsByName[fieldName];

    public virtual FieldDefinition TryGetByName(string fieldName)
    {
      FieldDefinition field;
      return this.TryGetByName(fieldName, out field) ? field : (FieldDefinition) null;
    }

    public virtual bool TryGetByName(string fieldName, out FieldDefinition field) => this.FieldsByName.TryGetValue(fieldName, out field);
  }
}
