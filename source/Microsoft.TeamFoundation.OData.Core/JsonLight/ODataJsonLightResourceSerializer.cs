// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightResourceSerializer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightResourceSerializer : ODataJsonLightPropertySerializer
  {
    internal ODataJsonLightResourceSerializer(ODataJsonLightOutputContext jsonLightOutputContext)
      : base(jsonLightOutputContext, true)
    {
    }

    private Uri MetadataDocumentBaseUri => this.JsonLightOutputContext.MessageWriterSettings.MetadataDocumentUri;

    internal void WriteResourceSetStartMetadataProperties(
      ODataResourceSet resourceSet,
      string propertyName,
      string expectedResourceTypeName,
      bool isUndeclared)
    {
      string typeNameForWriting = this.JsonLightOutputContext.TypeNameOracle.GetResourceSetTypeNameForWriting(expectedResourceTypeName, resourceSet, isUndeclared);
      if (typeNameForWriting == null || typeNameForWriting.Contains("Edm.Untyped"))
        return;
      if (propertyName == null)
        this.ODataAnnotationWriter.WriteODataTypeInstanceAnnotation(typeNameForWriting);
      else
        this.ODataAnnotationWriter.WriteODataTypePropertyAnnotation(propertyName, typeNameForWriting);
    }

    internal void WriteResourceStartMetadataProperties(
      IODataJsonLightWriterResourceState resourceState)
    {
      ODataResourceBase resource = resourceState.Resource;
      string typeNameForWriting = this.JsonLightOutputContext.TypeNameOracle.GetResourceTypeNameForWriting(!this.WritingResponse ? (resourceState.ResourceTypeFromMetadata != null ? resourceState.ResourceTypeFromMetadata.FullTypeName() : (resourceState.SerializationInfo == null ? (string) null : resourceState.SerializationInfo.ExpectedTypeName)) : resourceState.GetOrCreateTypeContext(this.WritingResponse).ExpectedResourceTypeName, resource, resourceState.IsUndeclared);
      if (typeNameForWriting != null && !string.Equals(typeNameForWriting, "Edm.Untyped", StringComparison.Ordinal))
        this.ODataAnnotationWriter.WriteODataTypeInstanceAnnotation(typeNameForWriting);
      Uri id;
      if (resource.MetadataBuilder.TryGetIdForSerialization(out id))
      {
        this.ODataAnnotationWriter.WriteInstanceAnnotationName("odata.id");
        if (id != (Uri) null && !resource.HasNonComputedId)
          id = this.MetadataDocumentBaseUri.MakeRelativeUri(id);
        this.JsonWriter.WriteValue(id == (Uri) null ? (string) null : this.UriToString(id));
      }
      string etag = resource.ETag;
      if (etag == null)
        return;
      this.ODataAnnotationWriter.WriteInstanceAnnotationName("odata.etag");
      this.JsonWriter.WriteValue(etag);
    }

    internal void WriteResourceMetadataProperties(IODataJsonLightWriterResourceState resourceState)
    {
      ODataResourceBase resource = resourceState.Resource;
      Uri editLink1 = resource.EditLink;
      if (editLink1 != (Uri) null && !resourceState.EditLinkWritten)
      {
        this.ODataAnnotationWriter.WriteInstanceAnnotationName("odata.editLink");
        this.JsonWriter.WriteValue(this.UriToString(resource.HasNonComputedEditLink || !editLink1.IsAbsoluteUri ? editLink1 : this.MetadataDocumentBaseUri.MakeRelativeUri(editLink1)));
        resourceState.EditLinkWritten = true;
      }
      Uri readLink1 = resource.ReadLink;
      if (readLink1 != (Uri) null && readLink1 != editLink1 && !resourceState.ReadLinkWritten)
      {
        this.ODataAnnotationWriter.WriteInstanceAnnotationName("odata.readLink");
        this.JsonWriter.WriteValue(this.UriToString(resource.HasNonComputedReadLink ? readLink1 : this.MetadataDocumentBaseUri.MakeRelativeUri(readLink1)));
        resourceState.ReadLinkWritten = true;
      }
      ODataStreamReferenceValue mediaResource = resource.MediaResource;
      if (mediaResource == null)
        return;
      Uri editLink2 = mediaResource.EditLink;
      if (editLink2 != (Uri) null && !resourceState.MediaEditLinkWritten)
      {
        this.ODataAnnotationWriter.WriteInstanceAnnotationName("odata.mediaEditLink");
        this.JsonWriter.WriteValue(this.UriToString(mediaResource.HasNonComputedEditLink ? editLink2 : this.MetadataDocumentBaseUri.MakeRelativeUri(editLink2)));
        resourceState.MediaEditLinkWritten = true;
      }
      Uri readLink2 = mediaResource.ReadLink;
      if (readLink2 != (Uri) null && readLink2 != editLink2 && !resourceState.MediaReadLinkWritten)
      {
        this.ODataAnnotationWriter.WriteInstanceAnnotationName("odata.mediaReadLink");
        this.JsonWriter.WriteValue(this.UriToString(mediaResource.HasNonComputedReadLink ? readLink2 : this.MetadataDocumentBaseUri.MakeRelativeUri(readLink2)));
        resourceState.MediaReadLinkWritten = true;
      }
      string contentType = mediaResource.ContentType;
      if (contentType != null && !resourceState.MediaContentTypeWritten)
      {
        this.ODataAnnotationWriter.WriteInstanceAnnotationName("odata.mediaContentType");
        this.JsonWriter.WriteValue(contentType);
        resourceState.MediaContentTypeWritten = true;
      }
      string etag = mediaResource.ETag;
      if (etag == null || resourceState.MediaETagWritten)
        return;
      this.ODataAnnotationWriter.WriteInstanceAnnotationName("odata.mediaEtag");
      this.JsonWriter.WriteValue(etag);
      resourceState.MediaETagWritten = true;
    }

    internal void WriteResourceEndMetadataProperties(
      IODataJsonLightWriterResourceState resourceState,
      IDuplicatePropertyNameChecker duplicatePropertyNameChecker)
    {
      ODataResourceBase resource = resourceState.Resource;
      for (ODataJsonLightReaderNestedResourceInfo unprocessedNavigationLink = resource.MetadataBuilder.GetNextUnprocessedNavigationLink(); unprocessedNavigationLink != null; unprocessedNavigationLink = resource.MetadataBuilder.GetNextUnprocessedNavigationLink())
      {
        unprocessedNavigationLink.NestedResourceInfo.MetadataBuilder = resource.MetadataBuilder;
        this.WriteNavigationLinkMetadata(unprocessedNavigationLink.NestedResourceInfo, duplicatePropertyNameChecker);
      }
      for (ODataProperty unprocessedStreamProperty = resource.MetadataBuilder.GetNextUnprocessedStreamProperty(); unprocessedStreamProperty != null; unprocessedStreamProperty = resource.MetadataBuilder.GetNextUnprocessedStreamProperty())
        this.WriteProperty(unprocessedStreamProperty, resourceState.ResourceType, false, false, duplicatePropertyNameChecker, (ODataResourceMetadataBuilder) null);
      IEnumerable<ODataAction> actions = resource.Actions;
      if (actions != null && actions.Any<ODataAction>())
        this.WriteOperations(actions.Cast<ODataOperation>(), true);
      IEnumerable<ODataFunction> functions = resource.Functions;
      if (functions == null || !functions.Any<ODataFunction>())
        return;
      this.WriteOperations(functions.Cast<ODataOperation>(), false);
    }

    internal void WriteNavigationLinkMetadata(
      ODataNestedResourceInfo nestedResourceInfo,
      IDuplicatePropertyNameChecker duplicatePropertyNameChecker)
    {
      Uri url = nestedResourceInfo.Url;
      string name = nestedResourceInfo.Name;
      Uri associationLinkUrl = nestedResourceInfo.AssociationLinkUrl;
      if (associationLinkUrl != (Uri) null)
      {
        duplicatePropertyNameChecker.ValidatePropertyOpenForAssociationLink(name);
        this.WriteAssociationLink(nestedResourceInfo.Name, associationLinkUrl);
      }
      if (!(url != (Uri) null))
        return;
      this.ODataAnnotationWriter.WritePropertyAnnotationName(name, "odata.navigationLink");
      this.JsonWriter.WriteValue(this.UriToString(url));
    }

    internal void WriteNestedResourceInfoContextUrl(
      ODataNestedResourceInfo nestedResourceInfo,
      ODataContextUrlInfo contextUrlInfo)
    {
      this.WriteContextUriProperty(ODataPayloadKind.Resource, (Func<ODataContextUrlInfo>) (() => contextUrlInfo), propertyName: nestedResourceInfo.Name);
    }

    internal void WriteOperations(IEnumerable<ODataOperation> operations, bool isAction)
    {
      foreach (IGrouping<string, ODataOperation> operations1 in operations.GroupBy<ODataOperation, string>((Func<ODataOperation, string>) (o =>
      {
        ValidationUtils.ValidateOperationNotNull(o, isAction);
        WriterValidationUtils.ValidateCanWriteOperation(o, this.JsonLightOutputContext.WritingResponse);
        ODataJsonLightValidationUtils.ValidateOperation(this.MetadataDocumentBaseUri, o);
        return this.GetOperationMetadataString(o);
      })))
        this.WriteOperationMetadataGroup(operations1);
    }

    internal ODataContextUrlInfo WriteDeltaContextUri(
      ODataResourceTypeContext typeContext,
      ODataDeltaKind kind,
      ODataContextUrlInfo parentContextUrlInfo = null)
    {
      ODataUri odataUri = this.JsonLightOutputContext.MessageWriterSettings.ODataUri;
      return this.WriteContextUriProperty(ODataPayloadKind.Delta, (Func<ODataContextUrlInfo>) (() => ODataContextUrlInfo.Create(typeContext, (ODataVersion) ((int) this.MessageWriterSettings.Version ?? 0), kind, odataUri)), parentContextUrlInfo);
    }

    internal ODataContextUrlInfo WriteResourceContextUri(
      ODataResourceTypeContext typeContext,
      ODataContextUrlInfo parentContextUrlInfo = null)
    {
      ODataUri odataUri = this.JsonLightOutputContext.MessageWriterSettings.ODataUri;
      return this.WriteContextUriProperty(ODataPayloadKind.Resource, (Func<ODataContextUrlInfo>) (() => ODataContextUrlInfo.Create(typeContext, (ODataVersion) ((int) this.MessageWriterSettings.Version ?? 0), true, odataUri)), parentContextUrlInfo);
    }

    internal ODataContextUrlInfo WriteResourceSetContextUri(ODataResourceTypeContext typeContext)
    {
      ODataUri odataUri = this.JsonLightOutputContext.MessageWriterSettings.ODataUri;
      return this.WriteContextUriProperty(ODataPayloadKind.ResourceSet, (Func<ODataContextUrlInfo>) (() => ODataContextUrlInfo.Create(typeContext, (ODataVersion) ((int) this.MessageWriterSettings.Version ?? 0), false, odataUri)));
    }

    private void WriteAssociationLink(string propertyName, Uri associationLinkUrl)
    {
      this.ODataAnnotationWriter.WritePropertyAnnotationName(propertyName, "odata.associationLink");
      this.JsonWriter.WriteValue(this.UriToString(associationLinkUrl));
    }

    private string GetOperationMetadataString(ODataOperation operation)
    {
      string propertyName = UriUtils.UriToString(operation.Metadata);
      return this.MetadataDocumentBaseUri == (Uri) null ? operation.Metadata.Fragment : "#" + ODataJsonLightUtils.GetUriFragmentFromMetadataReferencePropertyName(this.MetadataDocumentBaseUri, propertyName);
    }

    private string GetOperationTargetUriString(ODataOperation operation) => !(operation.Target == (Uri) null) ? this.UriToString(operation.Target) : (string) null;

    private void ValidateOperationMetadataGroup(IGrouping<string, ODataOperation> operations)
    {
      if (operations.Count<ODataOperation>() > 1 && operations.Any<ODataOperation>((Func<ODataOperation, bool>) (o => o.Target == (Uri) null)))
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceSerializer_ActionsAndFunctionsGroupMustSpecifyTarget((object) operations.Key));
      foreach (IGrouping<string, ODataOperation> source in operations.GroupBy<ODataOperation, string>(new Func<ODataOperation, string>(this.GetOperationTargetUriString)))
      {
        if (source.Count<ODataOperation>() > 1)
          throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceSerializer_ActionsAndFunctionsGroupMustNotHaveDuplicateTarget((object) operations.Key, (object) source.Key));
      }
    }

    private void WriteOperationMetadataGroup(IGrouping<string, ODataOperation> operations)
    {
      this.ValidateOperationMetadataGroup(operations);
      this.JsonLightOutputContext.JsonWriter.WriteName(operations.Key);
      bool flag = operations.Count<ODataOperation>() > 1;
      if (flag)
        this.JsonLightOutputContext.JsonWriter.StartArrayScope();
      foreach (ODataOperation operation in (IEnumerable<ODataOperation>) operations)
        this.WriteOperation(operation);
      if (!flag)
        return;
      this.JsonLightOutputContext.JsonWriter.EndArrayScope();
    }

    private void WriteOperation(ODataOperation operation)
    {
      this.JsonLightOutputContext.JsonWriter.StartObjectScope();
      if (operation.Title != null)
      {
        this.JsonLightOutputContext.JsonWriter.WriteName("title");
        this.JsonLightOutputContext.JsonWriter.WriteValue(operation.Title);
      }
      if (operation.Target != (Uri) null)
      {
        string operationTargetUriString = this.GetOperationTargetUriString(operation);
        this.JsonLightOutputContext.JsonWriter.WriteName("target");
        this.JsonLightOutputContext.JsonWriter.WriteValue(operationTargetUriString);
      }
      this.JsonLightOutputContext.JsonWriter.EndObjectScope();
    }
  }
}
