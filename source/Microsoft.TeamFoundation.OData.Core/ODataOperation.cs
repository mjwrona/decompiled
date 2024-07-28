// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataOperation
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Evaluation;
using Microsoft.OData.JsonLight;
using System;

namespace Microsoft.OData
{
  public abstract class ODataOperation : ODataAnnotatable
  {
    private ODataResourceMetadataBuilder metadataBuilder;
    private string title;
    private bool hasNonComputedTitle;
    private string computedTitle;
    private Uri target;
    private bool hasNonComputedTarget;
    private Uri computedTarget;
    private string operationFullName;
    private string parameterNames;

    public Uri Metadata { get; set; }

    public string Title
    {
      get
      {
        if (this.hasNonComputedTitle)
          return this.title;
        string computedTitle = this.computedTitle;
        if (computedTitle != null)
          return computedTitle;
        return this.metadataBuilder != null ? (this.computedTitle = this.metadataBuilder.GetOperationTitle(this.operationFullName)) : (string) null;
      }
      set
      {
        this.title = value;
        this.hasNonComputedTitle = true;
      }
    }

    public Uri Target
    {
      get
      {
        if (this.hasNonComputedTarget)
          return this.target;
        Uri computedTarget = this.computedTarget;
        if ((object) computedTarget != null)
          return computedTarget;
        return this.metadataBuilder != null ? (this.computedTarget = this.metadataBuilder.GetOperationTargetUri(this.operationFullName, this.BindingParameterTypeName, this.parameterNames)) : (Uri) null;
      }
      set
      {
        this.target = value;
        this.hasNonComputedTarget = true;
      }
    }

    internal string BindingParameterTypeName { get; set; }

    internal void SetMetadataBuilder(ODataResourceMetadataBuilder builder, Uri metadataDocumentUri)
    {
      ODataJsonLightValidationUtils.ValidateOperation(metadataDocumentUri, this);
      this.metadataBuilder = builder;
      this.operationFullName = ODataJsonLightUtils.GetFullyQualifiedOperationName(metadataDocumentUri, UriUtils.UriToString(this.Metadata), out this.parameterNames);
      this.computedTitle = (string) null;
      this.computedTarget = (Uri) null;
    }

    internal ODataResourceMetadataBuilder GetMetadataBuilder() => this.metadataBuilder;
  }
}
