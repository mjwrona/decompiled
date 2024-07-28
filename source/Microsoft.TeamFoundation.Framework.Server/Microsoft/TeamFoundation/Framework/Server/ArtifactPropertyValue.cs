// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ArtifactPropertyValue
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  public class ArtifactPropertyValue : ICacheable
  {
    internal ArtifactPropertyValue()
    {
    }

    public ArtifactPropertyValue(
      ArtifactSpec artifactSpec,
      IEnumerable<PropertyValue> propertyValues)
    {
      this.Spec = artifactSpec;
      this.PropertyValues = new StreamingCollection<PropertyValue>(propertyValues);
    }

    public ArtifactPropertyValue(
      Guid kind,
      int id,
      int version,
      IEnumerable<PropertyValue> propertyValues)
      : this(new ArtifactSpec(kind, id, version), propertyValues)
    {
    }

    public ArtifactPropertyValue(
      Guid kind,
      string moniker,
      int version,
      IEnumerable<PropertyValue> propertyValues)
      : this(new ArtifactSpec(kind, moniker, version), propertyValues)
    {
    }

    internal ArtifactPropertyValue(
      ArtifactSpec artifactSpec,
      int sequenceId,
      IEnumerable<PropertyValue> propertyValues)
      : this(artifactSpec, propertyValues)
    {
      this.SequenceId = sequenceId;
    }

    public int GetCachedSize() => this.Spec.GetCachedSize();

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public ArtifactSpec Spec { get; set; }

    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal, PropertyName = "InternalPropertyValues", Direction = ClientPropertySerialization.Bidirectional)]
    public StreamingCollection<PropertyValue> PropertyValues { get; set; }

    [XmlIgnore]
    public int SequenceId { get; internal set; }
  }
}
