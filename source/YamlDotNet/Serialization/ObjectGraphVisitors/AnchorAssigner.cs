// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.ObjectGraphVisitors.AnchorAssigner
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
  public sealed class AnchorAssigner : PreProcessingPhaseObjectGraphVisitorSkeleton, IAliasProvider
  {
    private readonly IDictionary<object, AnchorAssigner.AnchorAssignment> assignments = (IDictionary<object, AnchorAssigner.AnchorAssignment>) new Dictionary<object, AnchorAssigner.AnchorAssignment>();
    private uint nextId;

    public AnchorAssigner(IEnumerable<IYamlTypeConverter> typeConverters)
      : base(typeConverters)
    {
    }

    protected override bool Enter(IObjectDescriptor value)
    {
      AnchorAssigner.AnchorAssignment anchorAssignment;
      if (value.Value == null || !this.assignments.TryGetValue(value.Value, out anchorAssignment))
        return true;
      if (anchorAssignment.Anchor == null)
      {
        anchorAssignment.Anchor = "o" + this.nextId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        ++this.nextId;
      }
      return false;
    }

    protected override bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value) => true;

    protected override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value) => true;

    protected override void VisitScalar(IObjectDescriptor scalar)
    {
    }

    protected override void VisitMappingStart(
      IObjectDescriptor mapping,
      Type keyType,
      Type valueType)
    {
      this.VisitObject(mapping);
    }

    protected override void VisitMappingEnd(IObjectDescriptor mapping)
    {
    }

    protected override void VisitSequenceStart(IObjectDescriptor sequence, Type elementType) => this.VisitObject(sequence);

    protected override void VisitSequenceEnd(IObjectDescriptor sequence)
    {
    }

    private void VisitObject(IObjectDescriptor value)
    {
      if (value.Value == null)
        return;
      this.assignments.Add(value.Value, new AnchorAssigner.AnchorAssignment());
    }

    string IAliasProvider.GetAlias(object target)
    {
      AnchorAssigner.AnchorAssignment anchorAssignment;
      return target != null && this.assignments.TryGetValue(target, out anchorAssignment) ? anchorAssignment.Anchor : (string) null;
    }

    private class AnchorAssignment
    {
      public string Anchor;
    }
  }
}
