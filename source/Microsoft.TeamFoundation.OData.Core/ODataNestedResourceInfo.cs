// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataNestedResourceInfo
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Evaluation;
using System;
using System.Diagnostics;

namespace Microsoft.OData
{
  [DebuggerDisplay("{Name}")]
  public sealed class ODataNestedResourceInfo : ODataItem
  {
    private ODataResourceMetadataBuilder metadataBuilder;
    private Uri url;
    private bool hasNavigationLink;
    private Uri associationLinkUrl;
    private bool hasAssociationUrl;

    public bool? IsCollection { get; set; }

    public string Name { get; set; }

    public Uri Url
    {
      get
      {
        if (this.metadataBuilder != null && !this.IsComplex)
        {
          this.url = this.metadataBuilder.GetNavigationLinkUri(this.Name, this.url, this.hasNavigationLink);
          this.hasNavigationLink = true;
        }
        return this.url;
      }
      set
      {
        this.url = value;
        this.hasNavigationLink = true;
      }
    }

    public Uri AssociationLinkUrl
    {
      get
      {
        if (this.metadataBuilder != null && !this.IsComplex)
        {
          this.associationLinkUrl = this.metadataBuilder.GetAssociationLinkUri(this.Name, this.associationLinkUrl, this.hasAssociationUrl);
          this.hasAssociationUrl = true;
        }
        return this.associationLinkUrl;
      }
      set
      {
        this.associationLinkUrl = value;
        this.hasAssociationUrl = true;
      }
    }

    internal Uri ContextUrl { get; set; }

    internal ODataResourceMetadataBuilder MetadataBuilder
    {
      get => this.metadataBuilder;
      set => this.metadataBuilder = value;
    }

    internal ODataNestedResourceInfoSerializationInfo SerializationInfo { get; set; }

    internal bool IsComplex { get; set; }
  }
}
