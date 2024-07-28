// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataResourceBase
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Evaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.OData
{
  [DebuggerDisplay("Id: {Id} TypeName: {TypeName}")]
  public abstract class ODataResourceBase : ODataItem
  {
    private ODataResourceMetadataBuilder metadataBuilder;
    private string etag;
    private bool hasNonComputedETag;
    private Uri id;
    private bool hasNonComputedId;
    private Uri editLink;
    private bool hasNonComputedEditLink;
    private Uri readLink;
    private bool hasNonComputedReadLink;
    private ODataStreamReferenceValue mediaResource;
    private IEnumerable<ODataProperty> properties;
    private List<ODataAction> actions = new List<ODataAction>();
    private List<ODataFunction> functions = new List<ODataFunction>();
    private ODataResourceSerializationInfo serializationInfo;

    public string ETag
    {
      get => this.MetadataBuilder.GetETag();
      set
      {
        this.etag = value;
        this.hasNonComputedETag = true;
      }
    }

    public Uri Id
    {
      get => this.MetadataBuilder.GetId();
      set
      {
        this.id = value;
        this.hasNonComputedId = true;
      }
    }

    public Uri EditLink
    {
      get => this.MetadataBuilder.GetEditLink();
      set
      {
        this.editLink = value;
        this.hasNonComputedEditLink = true;
      }
    }

    public bool IsTransient { get; set; }

    public Uri ReadLink
    {
      get => this.MetadataBuilder.GetReadLink();
      set
      {
        this.readLink = value;
        this.hasNonComputedReadLink = true;
      }
    }

    public ODataStreamReferenceValue MediaResource
    {
      get => this.MetadataBuilder.GetMediaResource();
      set => this.mediaResource = value;
    }

    public IEnumerable<ODataAction> Actions => this.MetadataBuilder.GetActions();

    public IEnumerable<ODataFunction> Functions => this.MetadataBuilder.GetFunctions();

    public IEnumerable<ODataProperty> Properties
    {
      get => this.MetadataBuilder.GetProperties(this.properties);
      set
      {
        ODataResourceBase.VerifyProperties(value);
        this.properties = value;
      }
    }

    public string TypeName { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "We want to allow the same instance annotation collection instance to be shared across ODataLib OM instances.")]
    public ICollection<ODataInstanceAnnotation> InstanceAnnotations
    {
      get => this.GetInstanceAnnotations();
      set => this.SetInstanceAnnotations(value);
    }

    internal ODataResourceMetadataBuilder MetadataBuilder
    {
      get
      {
        if (this.metadataBuilder == null)
          this.metadataBuilder = (ODataResourceMetadataBuilder) new NoOpResourceMetadataBuilder(this);
        return this.metadataBuilder;
      }
      set => this.metadataBuilder = value;
    }

    internal Uri NonComputedId => this.id;

    internal bool HasNonComputedId => this.hasNonComputedId;

    internal Uri NonComputedEditLink => this.editLink;

    internal bool HasNonComputedEditLink => this.hasNonComputedEditLink;

    internal Uri NonComputedReadLink => this.readLink;

    internal bool HasNonComputedReadLink => this.hasNonComputedReadLink;

    internal string NonComputedETag => this.etag;

    internal bool HasNonComputedETag => this.hasNonComputedETag;

    internal ODataStreamReferenceValue NonComputedMediaResource => this.mediaResource;

    internal IEnumerable<ODataProperty> NonComputedProperties => this.properties;

    internal IEnumerable<ODataAction> NonComputedActions => this.actions != null ? (IEnumerable<ODataAction>) new ReadOnlyEnumerable<ODataAction>((IList<ODataAction>) this.actions) : (IEnumerable<ODataAction>) null;

    internal IEnumerable<ODataFunction> NonComputedFunctions => this.functions != null ? (IEnumerable<ODataFunction>) new ReadOnlyEnumerable<ODataFunction>((IList<ODataFunction>) this.functions) : (IEnumerable<ODataFunction>) null;

    internal virtual ODataResourceSerializationInfo SerializationInfo
    {
      get => this.serializationInfo;
      set => this.serializationInfo = value;
    }

    public void AddAction(ODataAction action)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataAction>(action, nameof (action));
      if (this.actions.Contains(action))
        return;
      this.actions.Add(action);
    }

    public void AddFunction(ODataFunction function)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataFunction>(function, nameof (function));
      if (this.functions.Contains(function))
        return;
      this.functions.Add(function);
    }

    private static void VerifyProperties(IEnumerable<ODataProperty> properties)
    {
      if (properties == null)
        return;
      foreach (ODataProperty property in properties)
      {
        if (property.Value is ODataResourceValue)
          throw new ODataException(Strings.ODataResource_PropertyValueCannotBeODataResourceValue((object) property.Name));
        if (property.Value is ODataCollectionValue odataCollectionValue && odataCollectionValue != null && odataCollectionValue.Items != null && odataCollectionValue.Items.Any<object>((Func<object, bool>) (t => t is ODataResourceValue)))
          throw new ODataException(Strings.ODataResource_PropertyValueCannotBeODataResourceValue((object) property.Name));
      }
    }
  }
}
