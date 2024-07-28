// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataMessageWriterSettings
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Buffers;
using Microsoft.OData.Evaluation;
using Microsoft.OData.UriParser;
using System;

namespace Microsoft.OData
{
  public sealed class ODataMessageWriterSettings
  {
    private string acceptCharSets;
    private string acceptMediaTypes;
    private Uri baseUri;
    private ODataFormat format;
    private ODataMessageQuotas messageQuotas;
    private ODataUri odataUri;
    private Func<string, bool> shouldIncludeAnnotation;
    private bool? useFormat;
    private ValidationKinds validations;

    public ODataMessageWriterSettings()
    {
      this.EnableMessageStreamDisposal = true;
      this.EnableCharactersCheck = false;
      this.Validations = ValidationKinds.All;
      this.Validator = (IWriterValidator) new WriterValidator(this);
      this.LibraryCompatibility = ODataLibraryCompatibility.Latest;
    }

    public ODataLibraryCompatibility LibraryCompatibility { get; set; }

    public ValidationKinds Validations
    {
      get => this.validations;
      set
      {
        this.validations = value;
        this.ThrowIfTypeConflictsWithMetadata = (this.validations & ValidationKinds.ThrowIfTypeConflictsWithMetadata) != 0;
        this.ThrowOnDuplicatePropertyNames = (this.validations & ValidationKinds.ThrowOnDuplicatePropertyNames) != 0;
        this.ThrowOnUndeclaredPropertyForNonOpenType = (this.validations & ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType) != 0;
      }
    }

    public Uri BaseUri
    {
      get => this.baseUri;
      set => this.baseUri = UriUtils.EnsureTaillingSlash(value);
    }

    public bool EnableMessageStreamDisposal { get; set; }

    public bool EnableCharactersCheck { get; set; }

    public string JsonPCallback { get; set; }

    public ICharArrayPool ArrayPool { get; set; }

    public ODataMessageQuotas MessageQuotas
    {
      get
      {
        if (this.messageQuotas == null)
          this.messageQuotas = new ODataMessageQuotas();
        return this.messageQuotas;
      }
      set => this.messageQuotas = value;
    }

    public ODataUri ODataUri
    {
      get => this.odataUri ?? (this.odataUri = new ODataUri());
      set => this.odataUri = value;
    }

    public ODataVersion? Version { get; set; }

    public bool OmitNullValues { get; set; }

    public ODataMetadataSelector MetadataSelector { get; set; }

    internal IWriterValidator Validator { get; private set; }

    internal bool ThrowIfTypeConflictsWithMetadata { get; private set; }

    internal bool ThrowOnDuplicatePropertyNames { get; private set; }

    internal bool ThrowOnUndeclaredPropertyForNonOpenType { get; private set; }

    internal string AcceptableMediaTypes => this.acceptMediaTypes;

    internal string AcceptableCharsets => this.acceptCharSets;

    internal ODataFormat Format => this.format;

    internal bool IsIndividualProperty => this.ODataUri.Path != null && this.ODataUri.Path.IsIndividualProperty();

    internal Uri MetadataDocumentUri => this.ODataUri.MetadataDocumentUri;

    internal bool? UseFormat => this.useFormat;

    internal SelectExpandClause SelectExpandClause => this.ODataUri.SelectAndExpand;

    internal SelectedPropertiesNode SelectedProperties => this.SelectExpandClause == null ? SelectedPropertiesNode.EntireSubtree : SelectedPropertiesNode.Create(this.SelectExpandClause, (ODataVersion) ((int) this.Version ?? 0));

    internal Func<string, bool> ShouldIncludeAnnotation
    {
      get => this.shouldIncludeAnnotation;
      set => this.shouldIncludeAnnotation = value;
    }

    public ODataMessageWriterSettings Clone()
    {
      ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings();
      messageWriterSettings.CopyFrom(this);
      return messageWriterSettings;
    }

    public void SetContentType(string acceptableMediaTypes, string acceptableCharSets)
    {
      this.acceptMediaTypes = string.Equals(acceptableMediaTypes, "json", StringComparison.OrdinalIgnoreCase) ? "application/json" : acceptableMediaTypes;
      this.acceptCharSets = acceptableCharSets;
      this.format = (ODataFormat) null;
      this.useFormat = new bool?(false);
    }

    public void SetContentType(ODataFormat payloadFormat)
    {
      this.acceptCharSets = (string) null;
      this.acceptMediaTypes = (string) null;
      this.format = payloadFormat;
      this.useFormat = new bool?(true);
    }

    internal static ODataMessageWriterSettings CreateWriterSettings(
      IServiceProvider container,
      ODataMessageWriterSettings other)
    {
      ODataMessageWriterSettings writerSettings = container != null ? container.GetRequiredService<ODataMessageWriterSettings>() : new ODataMessageWriterSettings();
      if (other != null)
        writerSettings.CopyFrom(other);
      return writerSettings;
    }

    internal void SetServiceDocumentUri(Uri serviceDocumentUri) => this.ODataUri.ServiceRoot = serviceDocumentUri;

    internal bool HasJsonPaddingFunction() => !string.IsNullOrEmpty(this.JsonPCallback);

    internal bool ShouldSkipAnnotation(string annotationName) => this.ShouldIncludeAnnotation == null || !this.ShouldIncludeAnnotation(annotationName);

    private void CopyFrom(ODataMessageWriterSettings other)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageWriterSettings>(other, nameof (other));
      this.acceptCharSets = other.acceptCharSets;
      this.acceptMediaTypes = other.acceptMediaTypes;
      this.BaseUri = other.BaseUri;
      this.EnableMessageStreamDisposal = other.EnableMessageStreamDisposal;
      this.EnableCharactersCheck = other.EnableCharactersCheck;
      this.format = other.format;
      this.JsonPCallback = other.JsonPCallback;
      this.messageQuotas = new ODataMessageQuotas(other.MessageQuotas);
      this.ODataUri = other.ODataUri.Clone();
      this.shouldIncludeAnnotation = other.shouldIncludeAnnotation;
      this.useFormat = other.useFormat;
      this.Version = other.Version;
      this.LibraryCompatibility = other.LibraryCompatibility;
      this.MetadataSelector = other.MetadataSelector;
      this.OmitNullValues = other.OmitNullValues;
      this.validations = other.validations;
      this.ThrowIfTypeConflictsWithMetadata = other.ThrowIfTypeConflictsWithMetadata;
      this.ThrowOnDuplicatePropertyNames = other.ThrowOnDuplicatePropertyNames;
      this.ThrowOnUndeclaredPropertyForNonOpenType = other.ThrowOnUndeclaredPropertyForNonOpenType;
      this.ArrayPool = other.ArrayPool;
    }
  }
}
