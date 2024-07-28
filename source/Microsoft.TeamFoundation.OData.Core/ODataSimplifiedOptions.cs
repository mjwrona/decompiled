// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataSimplifiedOptions
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;

namespace Microsoft.OData
{
  public sealed class ODataSimplifiedOptions
  {
    private bool enableWritingODataAnnotationWithoutPrefix;
    private bool omitODataPrefix40;
    private bool omitODataPrefix = true;

    public ODataSimplifiedOptions()
      : this(new ODataVersion?())
    {
    }

    public ODataSimplifiedOptions(ODataVersion? version)
    {
      this.EnableParsingKeyAsSegmentUrl = true;
      this.EnableWritingKeyAsSegment = false;
      this.EnableReadingKeyAsSegment = false;
      if (version.HasValue)
      {
        ODataVersion? nullable = version;
        ODataVersion odataVersion = ODataVersion.V401;
        if (!(nullable.GetValueOrDefault() < odataVersion & nullable.HasValue))
        {
          this.EnableReadingODataAnnotationWithoutPrefix = true;
          this.enableWritingODataAnnotationWithoutPrefix = this.omitODataPrefix;
          return;
        }
      }
      this.EnableReadingODataAnnotationWithoutPrefix = false;
      this.enableWritingODataAnnotationWithoutPrefix = this.omitODataPrefix40;
    }

    public bool EnableParsingKeyAsSegmentUrl { get; set; }

    public bool EnableReadingKeyAsSegment { get; set; }

    public bool EnableReadingODataAnnotationWithoutPrefix { get; set; }

    public bool EnableWritingKeyAsSegment { get; set; }

    [Obsolete("Deprecated. Use Get/SetOmitODataPrefix()")]
    public bool EnableWritingODataAnnotationWithoutPrefix
    {
      get => this.enableWritingODataAnnotationWithoutPrefix;
      set => this.enableWritingODataAnnotationWithoutPrefix = this.omitODataPrefix = this.omitODataPrefix40 = value;
    }

    public ODataSimplifiedOptions Clone()
    {
      ODataSimplifiedOptions simplifiedOptions = new ODataSimplifiedOptions();
      simplifiedOptions.CopyFrom(this);
      return simplifiedOptions;
    }

    public bool GetOmitODataPrefix() => this.enableWritingODataAnnotationWithoutPrefix;

    public bool GetOmitODataPrefix(ODataVersion version) => version >= ODataVersion.V401 ? this.omitODataPrefix : this.omitODataPrefix40;

    public void SetOmitODataPrefix(bool enabled) => this.enableWritingODataAnnotationWithoutPrefix = this.omitODataPrefix = this.omitODataPrefix40 = enabled;

    public void SetOmitODataPrefix(bool enabled, ODataVersion version)
    {
      if (version == ODataVersion.V4)
        this.omitODataPrefix40 = enabled;
      else
        this.omitODataPrefix = enabled;
    }

    internal static ODataSimplifiedOptions GetODataSimplifiedOptions(
      IServiceProvider container,
      ODataVersion? version = null)
    {
      return container == null ? new ODataSimplifiedOptions(version) : container.GetRequiredService<ODataSimplifiedOptions>();
    }

    private void CopyFrom(ODataSimplifiedOptions other)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataSimplifiedOptions>(other, nameof (other));
      this.EnableParsingKeyAsSegmentUrl = other.EnableParsingKeyAsSegmentUrl;
      this.EnableReadingKeyAsSegment = other.EnableReadingKeyAsSegment;
      this.EnableReadingODataAnnotationWithoutPrefix = other.EnableReadingODataAnnotationWithoutPrefix;
      this.EnableWritingKeyAsSegment = other.EnableWritingKeyAsSegment;
      this.enableWritingODataAnnotationWithoutPrefix = other.enableWritingODataAnnotationWithoutPrefix;
      this.omitODataPrefix40 = other.omitODataPrefix40;
      this.omitODataPrefix = other.omitODataPrefix;
    }
  }
}
