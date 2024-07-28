// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Behavior
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class Behavior : ProcessTypelet
  {
    public IReadOnlyDictionary<string, FieldEntry> BehaviorFields { get; protected set; }

    public Behavior ParentBehavior { get; protected set; }

    public string ParentBehaviorReferenceName { get; protected set; }

    public virtual int Rank { get; protected set; }

    public virtual bool Overridden { get; protected set; }

    public virtual bool Custom { get; protected set; }

    public virtual CustomizationType Customization { get; protected set; }

    public virtual IReadOnlyDictionary<string, string> Properties { get; protected set; }

    public IReadOnlyDictionary<string, FieldEntry> GetCombinedBehaviorFields(
      IVssRequestContext requestContext)
    {
      Dictionary<string, FieldEntry> combinedBehaviorFields = new Dictionary<string, FieldEntry>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      if (this.ParentBehavior != null)
      {
        foreach (KeyValuePair<string, FieldEntry> combinedBehaviorField in (IEnumerable<KeyValuePair<string, FieldEntry>>) this.ParentBehavior.GetCombinedBehaviorFields(requestContext))
          combinedBehaviorFields[combinedBehaviorField.Key] = combinedBehaviorField.Value;
      }
      foreach (KeyValuePair<string, FieldEntry> behaviorField in (IEnumerable<KeyValuePair<string, FieldEntry>>) this.BehaviorFields)
        combinedBehaviorFields[behaviorField.Key] = behaviorField.Value;
      return (IReadOnlyDictionary<string, FieldEntry>) combinedBehaviorFields;
    }

    public bool TryGetField(string behaviorFieldId, out FieldEntry field)
    {
      bool flag = this.BehaviorFields.TryGetValue(behaviorFieldId, out field);
      return !flag && this.ParentBehavior != null ? this.ParentBehavior.TryGetField(behaviorFieldId, out field) : flag;
    }

    public IReadOnlyDictionary<string, ProcessFieldDefinition> LegacyBehaviorFields { get; protected set; }

    public IDictionary<string, ProcessFieldDefinition> GetLegacyCombinedFields(
      IVssRequestContext requestContext)
    {
      Dictionary<string, ProcessFieldDefinition> legacyCombinedFields = new Dictionary<string, ProcessFieldDefinition>();
      if (this.ParentBehavior != null)
        legacyCombinedFields = new Dictionary<string, ProcessFieldDefinition>(this.ParentBehavior.GetLegacyCombinedFields(requestContext));
      foreach (KeyValuePair<string, ProcessFieldDefinition> legacyBehaviorField in (IEnumerable<KeyValuePair<string, ProcessFieldDefinition>>) this.LegacyBehaviorFields)
        legacyCombinedFields[legacyBehaviorField.Key] = legacyBehaviorField.Value;
      return (IDictionary<string, ProcessFieldDefinition>) legacyCombinedFields;
    }

    internal static Behavior Create(
      IVssRequestContext requestContext,
      WorkItemTypeletRecord record,
      WorkItemTrackingFieldService fieldDict)
    {
      bool flag = record.ProcessId == ProcessConstants.SystemProcessId || requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, record.ProcessId).IsSystem;
      Behavior behavior = new Behavior();
      behavior.Id = record.Id;
      behavior.Name = record.Name;
      behavior.Description = record.Description;
      behavior.LastChangedDate = record.LastChangeDate;
      behavior.Form = record.Form == null ? new Layout() : JsonConvert.DeserializeObject<Layout>(record.Form);
      behavior.ProcessId = record.ProcessId;
      behavior.ParentBehaviorReferenceName = record.ParentTypeRefName;
      behavior.ReferenceName = record.ReferenceName;
      behavior.IsDeleted = record.IsDeleted;
      behavior.Color = record.Color;
      behavior.IsAbstract = record.IsAbstract;
      behavior.Overridden = record.Overridden;
      behavior.Custom = !flag && !record.Overridden;
      behavior.Rank = record.Rank;
      behavior.Customization = flag || record.Overridden ? (!record.Overridden ? CustomizationType.System : CustomizationType.Inherited) : CustomizationType.Custom;
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (record.Properties != null)
      {
        foreach (WorkItemTypeExtensionPropertyRecord property in record.Properties)
          dictionary1[property.Name] = property.Value;
      }
      behavior.Properties = (IReadOnlyDictionary<string, string>) dictionary1;
      if (record.Fields != null)
      {
        IReadOnlyCollection<ProcessFieldDefinition> fieldDefinitions = requestContext.GetService<IProcessFieldService>().GetAllOutOfBoxFieldDefinitions(requestContext);
        Dictionary<string, FieldEntry> source = new Dictionary<string, FieldEntry>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
        Dictionary<string, ProcessFieldDefinition> dictionary2 = new Dictionary<string, ProcessFieldDefinition>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
        foreach (WorkItemTypeletFieldRecord field1 in record.Fields)
        {
          WorkItemTypeletFieldRecord field = field1;
          FieldEntry field2;
          if (fieldDict.TryGetField(requestContext, field.FieldReferenceName, out field2, new bool?(true)))
            source[field.BehaviorFieldId] = field2;
          ProcessFieldDefinition processFieldDefinition = fieldDefinitions.FirstOrDefault<ProcessFieldDefinition>((Func<ProcessFieldDefinition, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, field.FieldReferenceName)));
          if (processFieldDefinition != null)
            dictionary2[field.BehaviorFieldId] = processFieldDefinition;
        }
        behavior.BehaviorFields = (IReadOnlyDictionary<string, FieldEntry>) source;
        behavior.Fields = (IEnumerable<WorkItemTypeExtensionFieldEntry>) source.Select<KeyValuePair<string, FieldEntry>, WorkItemTypeExtensionFieldEntry>((Func<KeyValuePair<string, FieldEntry>, WorkItemTypeExtensionFieldEntry>) (bf => new WorkItemTypeExtensionFieldEntry(record.Id, bf.Value))).ToList<WorkItemTypeExtensionFieldEntry>();
        behavior.LegacyBehaviorFields = (IReadOnlyDictionary<string, ProcessFieldDefinition>) dictionary2;
      }
      else
      {
        behavior.BehaviorFields = (IReadOnlyDictionary<string, FieldEntry>) new Dictionary<string, FieldEntry>(0);
        behavior.Fields = Enumerable.Empty<WorkItemTypeExtensionFieldEntry>();
        behavior.LegacyBehaviorFields = (IReadOnlyDictionary<string, ProcessFieldDefinition>) new Dictionary<string, ProcessFieldDefinition>(0);
      }
      return behavior;
    }

    internal static Behavior Create(Behavior derivedBehavior, Behavior baseBehavior)
    {
      Behavior behavior = baseBehavior.Clone() as Behavior;
      if (!string.IsNullOrWhiteSpace(derivedBehavior.Name))
        behavior.Name = derivedBehavior.Name;
      if (!string.IsNullOrWhiteSpace(derivedBehavior.Color))
        behavior.Color = derivedBehavior.Color;
      if (derivedBehavior.LegacyBehaviorFields != null && derivedBehavior.LegacyBehaviorFields.Any<KeyValuePair<string, ProcessFieldDefinition>>())
      {
        Dictionary<string, ProcessFieldDefinition> dictionary = behavior.LegacyBehaviorFields.ToDictionary<KeyValuePair<string, ProcessFieldDefinition>, string, ProcessFieldDefinition>((Func<KeyValuePair<string, ProcessFieldDefinition>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, ProcessFieldDefinition>, ProcessFieldDefinition>) (kvp => kvp.Value), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
        foreach (KeyValuePair<string, ProcessFieldDefinition> legacyBehaviorField in (IEnumerable<KeyValuePair<string, ProcessFieldDefinition>>) derivedBehavior.LegacyBehaviorFields)
        {
          if (dictionary.ContainsKey(legacyBehaviorField.Key))
            dictionary[legacyBehaviorField.Key] = legacyBehaviorField.Value;
        }
        behavior.LegacyBehaviorFields = (IReadOnlyDictionary<string, ProcessFieldDefinition>) dictionary;
      }
      if (derivedBehavior.BehaviorFields != null && derivedBehavior.BehaviorFields.Any<KeyValuePair<string, FieldEntry>>())
      {
        Dictionary<string, FieldEntry> dictionary = behavior.BehaviorFields.ToDictionary<KeyValuePair<string, FieldEntry>, string, FieldEntry>((Func<KeyValuePair<string, FieldEntry>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, FieldEntry>, FieldEntry>) (kvp => kvp.Value), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
        foreach (KeyValuePair<string, FieldEntry> behaviorField in (IEnumerable<KeyValuePair<string, FieldEntry>>) derivedBehavior.BehaviorFields)
          dictionary[behaviorField.Key] = behaviorField.Value;
        behavior.BehaviorFields = (IReadOnlyDictionary<string, FieldEntry>) dictionary;
        behavior.Fields = (IEnumerable<WorkItemTypeExtensionFieldEntry>) behavior.BehaviorFields.Select<KeyValuePair<string, FieldEntry>, WorkItemTypeExtensionFieldEntry>((Func<KeyValuePair<string, FieldEntry>, WorkItemTypeExtensionFieldEntry>) (bf => new WorkItemTypeExtensionFieldEntry(derivedBehavior.Id, bf.Value))).ToList<WorkItemTypeExtensionFieldEntry>();
      }
      behavior.LastChangedDate = derivedBehavior.LastChangedDate;
      behavior.ProcessId = derivedBehavior.ProcessId;
      behavior.Overridden = derivedBehavior.Overridden;
      behavior.Id = derivedBehavior.Id;
      behavior.Customization = derivedBehavior.Customization;
      return behavior;
    }

    public override ProcessTypelet Clone()
    {
      Behavior behavior = (Behavior) this.MemberwiseClone();
      behavior.Properties = (IReadOnlyDictionary<string, string>) this.Properties.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, string>, string>) (kvp => kvp.Value));
      behavior.Fields = (IEnumerable<WorkItemTypeExtensionFieldEntry>) new List<WorkItemTypeExtensionFieldEntry>(this.Fields);
      behavior.BehaviorFields = (IReadOnlyDictionary<string, FieldEntry>) this.BehaviorFields.ToDictionary<KeyValuePair<string, FieldEntry>, string, FieldEntry>((Func<KeyValuePair<string, FieldEntry>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, FieldEntry>, FieldEntry>) (kvp => kvp.Value), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      behavior.LegacyBehaviorFields = (IReadOnlyDictionary<string, ProcessFieldDefinition>) this.LegacyBehaviorFields.ToDictionary<KeyValuePair<string, ProcessFieldDefinition>, string, ProcessFieldDefinition>((Func<KeyValuePair<string, ProcessFieldDefinition>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, ProcessFieldDefinition>, ProcessFieldDefinition>) (kvp => kvp.Value), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      return (ProcessTypelet) behavior;
    }

    public override void ResolveTypeReference(
      IReadOnlyCollection<ProcessTypelet> availableTypelets)
    {
      this.ParentBehavior = availableTypelets.OfType<Behavior>().FirstOrDefault<Behavior>((Func<Behavior, bool>) (t => TFStringComparer.WorkItemTypeReferenceName.Equals(t.ReferenceName, this.ParentBehaviorReferenceName)));
    }
  }
}
