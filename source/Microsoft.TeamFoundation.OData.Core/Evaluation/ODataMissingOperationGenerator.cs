// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Evaluation.ODataMissingOperationGenerator
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Evaluation
{
  internal sealed class ODataMissingOperationGenerator
  {
    private readonly IODataMetadataContext metadataContext;
    private readonly IODataResourceMetadataContext resourceMetadataContext;
    private List<ODataAction> computedActions;
    private List<ODataFunction> computedFunctions;

    internal ODataMissingOperationGenerator(
      IODataResourceMetadataContext resourceMetadataContext,
      IODataMetadataContext metadataContext)
    {
      this.resourceMetadataContext = resourceMetadataContext;
      this.metadataContext = metadataContext;
    }

    internal IEnumerable<ODataAction> GetComputedActions()
    {
      this.ComputeMissingOperationsToResource();
      return (IEnumerable<ODataAction>) this.computedActions;
    }

    internal IEnumerable<ODataFunction> GetComputedFunctions()
    {
      this.ComputeMissingOperationsToResource();
      return (IEnumerable<ODataFunction>) this.computedFunctions;
    }

    private static HashSet<IEdmOperation> GetOperationsInEntry(
      ODataResourceBase resource,
      IEdmModel model,
      Uri metadataDocumentUri)
    {
      HashSet<IEdmOperation> operationsInEntry = new HashSet<IEdmOperation>((IEqualityComparer<IEdmOperation>) EqualityComparer<IEdmOperation>.Default);
      IEnumerable<ODataOperation> odataOperations = ODataUtilsInternal.ConcatEnumerables<ODataOperation>((IEnumerable<ODataOperation>) resource.NonComputedActions, (IEnumerable<ODataOperation>) resource.NonComputedFunctions);
      if (odataOperations != null)
      {
        foreach (ODataOperation odataOperation in odataOperations)
        {
          string propertyName = UriUtils.UriToString(odataOperation.Metadata);
          string referencePropertyName = ODataJsonLightUtils.GetUriFragmentFromMetadataReferencePropertyName(metadataDocumentUri, propertyName);
          IEnumerable<IEdmOperation> edmOperations = model.ResolveOperations(referencePropertyName);
          if (edmOperations != null)
          {
            foreach (IEdmOperation edmOperation in edmOperations)
              operationsInEntry.Add(edmOperation);
          }
        }
      }
      return operationsInEntry;
    }

    private void ComputeMissingOperationsToResource()
    {
      if (this.computedActions != null)
        return;
      this.computedActions = new List<ODataAction>();
      this.computedFunctions = new List<ODataFunction>();
      HashSet<IEdmOperation> operationsInEntry = ODataMissingOperationGenerator.GetOperationsInEntry(this.resourceMetadataContext.Resource, this.metadataContext.Model, this.metadataContext.MetadataDocumentUri);
      foreach (IEdmOperation bindableOperation in this.resourceMetadataContext.SelectedBindableOperations)
      {
        if (!operationsInEntry.Contains(bindableOperation))
        {
          bool isAction;
          ODataOperation odataOperation = ODataJsonLightUtils.CreateODataOperation(this.metadataContext.MetadataDocumentUri, "#" + ODataJsonLightUtils.GetMetadataReferenceName(this.metadataContext.Model, bindableOperation), bindableOperation, out isAction);
          if (bindableOperation.Parameters.Any<IEdmOperationParameter>() && this.resourceMetadataContext.ActualResourceTypeName != bindableOperation.Parameters.First<IEdmOperationParameter>().Type.FullName())
            odataOperation.BindingParameterTypeName = bindableOperation.Parameters.First<IEdmOperationParameter>().Type.FullName();
          odataOperation.SetMetadataBuilder(this.resourceMetadataContext.Resource.MetadataBuilder, this.metadataContext.MetadataDocumentUri);
          if (isAction)
            this.computedActions.Add((ODataAction) odataOperation);
          else
            this.computedFunctions.Add((ODataFunction) odataOperation);
        }
      }
    }
  }
}
