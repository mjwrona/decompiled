// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataMessageReaderSettings
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Buffers;
using Microsoft.OData.Edm;
using System;

namespace Microsoft.OData
{
  public sealed class ODataMessageReaderSettings
  {
    private Uri baseUri;
    private ODataMessageQuotas messageQuotas;
    private ValidationKinds validations;

    public ODataMessageReaderSettings()
      : this(ODataVersion.V4)
    {
    }

    public ODataMessageReaderSettings(ODataVersion odataVersion)
    {
      this.ClientCustomTypeResolver = (Func<IEdmType, string, IEdmType>) null;
      this.PrimitiveTypeResolver = (Func<object, string, IEdmTypeReference>) null;
      this.EnablePrimitiveTypeConversion = true;
      this.EnableMessageStreamDisposal = true;
      this.EnableCharactersCheck = false;
      this.Version = new ODataVersion?(odataVersion);
      this.LibraryCompatibility = ODataLibraryCompatibility.Latest;
      this.Validator = (IReaderValidator) new ReaderValidator(this);
      if (odataVersion < ODataVersion.V401)
      {
        this.Validations = ValidationKinds.All;
        this.ReadUntypedAsString = true;
        this.MaxProtocolVersion = ODataVersion.V4;
      }
      else
      {
        this.Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;
        this.ReadUntypedAsString = false;
        this.MaxProtocolVersion = odataVersion;
      }
    }

    public ODataLibraryCompatibility LibraryCompatibility { get; set; }

    public ODataVersion? Version { get; set; }

    public ICharArrayPool ArrayPool { get; set; }

    public ValidationKinds Validations
    {
      get => this.validations;
      set
      {
        this.validations = value;
        this.ThrowOnDuplicatePropertyNames = (this.validations & ValidationKinds.ThrowOnDuplicatePropertyNames) != 0;
        this.ThrowIfTypeConflictsWithMetadata = (this.validations & ValidationKinds.ThrowIfTypeConflictsWithMetadata) != 0;
        this.ThrowOnUndeclaredPropertyForNonOpenType = (this.validations & ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType) != 0;
      }
    }

    public Uri BaseUri
    {
      get => this.baseUri;
      set => this.baseUri = UriUtils.EnsureTaillingSlash(value);
    }

    public Func<IEdmType, string, IEdmType> ClientCustomTypeResolver { get; set; }

    public Func<object, string, IEdmTypeReference> PrimitiveTypeResolver { get; set; }

    public bool EnablePrimitiveTypeConversion { get; set; }

    public bool EnableMessageStreamDisposal { get; set; }

    public bool EnableCharactersCheck { get; set; }

    public ODataVersion MaxProtocolVersion { get; set; }

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

    public bool ReadUntypedAsString { get; set; }

    public Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> ReadAsStreamFunc { get; set; }

    public Func<string, bool> ShouldIncludeAnnotation { get; set; }

    internal IReaderValidator Validator { get; private set; }

    internal bool ThrowOnDuplicatePropertyNames { get; private set; }

    internal bool ThrowIfTypeConflictsWithMetadata { get; private set; }

    internal bool ThrowOnUndeclaredPropertyForNonOpenType { get; private set; }

    public ODataMessageReaderSettings Clone()
    {
      ODataMessageReaderSettings messageReaderSettings = new ODataMessageReaderSettings();
      messageReaderSettings.CopyFrom(this);
      return messageReaderSettings;
    }

    internal static ODataMessageReaderSettings CreateReaderSettings(
      IServiceProvider container,
      ODataMessageReaderSettings other)
    {
      ODataMessageReaderSettings readerSettings = container != null ? container.GetRequiredService<ODataMessageReaderSettings>() : new ODataMessageReaderSettings();
      if (other != null)
        readerSettings.CopyFrom(other);
      return readerSettings;
    }

    internal bool ShouldSkipAnnotation(string annotationName) => this.ShouldIncludeAnnotation == null || !this.ShouldIncludeAnnotation(annotationName);

    private void CopyFrom(ODataMessageReaderSettings other)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageReaderSettings>(other, nameof (other));
      this.BaseUri = other.BaseUri;
      this.ClientCustomTypeResolver = other.ClientCustomTypeResolver;
      this.PrimitiveTypeResolver = other.PrimitiveTypeResolver;
      this.EnableMessageStreamDisposal = other.EnableMessageStreamDisposal;
      this.EnablePrimitiveTypeConversion = other.EnablePrimitiveTypeConversion;
      this.EnableCharactersCheck = other.EnableCharactersCheck;
      this.messageQuotas = new ODataMessageQuotas(other.MessageQuotas);
      this.MaxProtocolVersion = other.MaxProtocolVersion;
      this.ReadUntypedAsString = other.ReadUntypedAsString;
      this.ShouldIncludeAnnotation = other.ShouldIncludeAnnotation;
      this.validations = other.validations;
      this.ThrowOnDuplicatePropertyNames = other.ThrowOnDuplicatePropertyNames;
      this.ThrowIfTypeConflictsWithMetadata = other.ThrowIfTypeConflictsWithMetadata;
      this.ThrowOnUndeclaredPropertyForNonOpenType = other.ThrowOnUndeclaredPropertyForNonOpenType;
      this.LibraryCompatibility = other.LibraryCompatibility;
      this.Version = other.Version;
      this.ReadAsStreamFunc = other.ReadAsStreamFunc;
      this.ArrayPool = other.ArrayPool;
    }
  }
}
