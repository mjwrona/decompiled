// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataUrlKeyDelimiter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;

namespace Microsoft.OData
{
  public sealed class ODataUrlKeyDelimiter
  {
    private static readonly ODataUrlKeyDelimiter slashDelimiter = new ODataUrlKeyDelimiter(true);
    private static readonly ODataUrlKeyDelimiter parenthesesDelimiter = new ODataUrlKeyDelimiter(false);
    private readonly bool enableKeyAsSegment;

    private ODataUrlKeyDelimiter(bool enablekeyAsSegment) => this.enableKeyAsSegment = enablekeyAsSegment;

    public static ODataUrlKeyDelimiter Parentheses => ODataUrlKeyDelimiter.parenthesesDelimiter;

    public static ODataUrlKeyDelimiter Slash => ODataUrlKeyDelimiter.slashDelimiter;

    internal bool EnableKeyAsSegment => this.enableKeyAsSegment;

    internal static ODataUrlKeyDelimiter GetODataUrlKeyDelimiter(IServiceProvider container) => !ODataSimplifiedOptions.GetODataSimplifiedOptions(container).EnableParsingKeyAsSegmentUrl ? ODataUrlKeyDelimiter.Parentheses : ODataUrlKeyDelimiter.Slash;
  }
}
