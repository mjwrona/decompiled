// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightContextUriParser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightContextUriParser
  {
    private static readonly Regex KeyPattern = new Regex("^(?:-{0,1}\\d+?|\\w*'.+?'|[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}|.+?=.+?)$", RegexOptions.IgnoreCase);
    private readonly IEdmModel model;
    private readonly ODataJsonLightContextUriParseResult parseResult;

    private ODataJsonLightContextUriParser(IEdmModel model, Uri contextUriFromPayload)
    {
      this.model = model.IsUserModel() ? model : throw new ODataException(Microsoft.OData.Strings.ODataJsonLightContextUriParser_NoModel);
      this.parseResult = new ODataJsonLightContextUriParseResult(contextUriFromPayload);
    }

    internal static ODataJsonLightContextUriParseResult Parse(
      IEdmModel model,
      string contextUriFromPayload,
      ODataPayloadKind payloadKind,
      Func<IEdmType, string, IEdmType> clientCustomTypeResolver,
      bool needParseFragment,
      bool throwIfMetadataConflict = true)
    {
      if (contextUriFromPayload == null)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightContextUriParser_NullMetadataDocumentUri);
      Uri result;
      if (!Uri.TryCreate(contextUriFromPayload, UriKind.Absolute, out result))
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightContextUriParser_TopLevelContextUrlShouldBeAbsolute((object) contextUriFromPayload));
      ODataJsonLightContextUriParser contextUriParser = new ODataJsonLightContextUriParser(model, result);
      contextUriParser.TokenizeContextUri();
      if (needParseFragment)
        contextUriParser.ParseContextUri(payloadKind, clientCustomTypeResolver, throwIfMetadataConflict);
      return contextUriParser.parseResult;
    }

    private static string ExtractSelectQueryOption(string fragment) => fragment;

    private void TokenizeContextUri()
    {
      Uri contextUri = this.parseResult.ContextUri;
      this.parseResult.MetadataDocumentUri = new UriBuilder(contextUri)
      {
        Fragment = ((string) null)
      }.Uri;
      this.parseResult.Fragment = contextUri.GetComponents(UriComponents.Fragment, UriFormat.SafeUnescaped);
    }

    private void ParseContextUri(
      ODataPayloadKind expectedPayloadKind,
      Func<IEdmType, string, IEdmType> clientCustomTypeResolver,
      bool throwIfMetadataConflict)
    {
      bool isUndeclared;
      ODataPayloadKind contextUriFragment = this.ParseContextUriFragment(this.parseResult.Fragment, clientCustomTypeResolver, throwIfMetadataConflict, out isUndeclared);
      bool flag = contextUriFragment == expectedPayloadKind || expectedPayloadKind == ODataPayloadKind.Unsupported;
      IEdmType edmType = this.parseResult.EdmType;
      if (edmType != null && edmType.TypeKind == EdmTypeKind.Untyped)
      {
        if (string.Equals(edmType.FullTypeName(), "Edm.Untyped", StringComparison.Ordinal))
        {
          this.parseResult.DetectedPayloadKinds = (IEnumerable<ODataPayloadKind>) new ODataPayloadKind[4]
          {
            ODataPayloadKind.ResourceSet,
            ODataPayloadKind.Property,
            ODataPayloadKind.Collection,
            ODataPayloadKind.Resource
          };
          flag = true;
        }
        else if (expectedPayloadKind == ODataPayloadKind.Property || expectedPayloadKind == ODataPayloadKind.Resource)
        {
          this.parseResult.DetectedPayloadKinds = (IEnumerable<ODataPayloadKind>) new ODataPayloadKind[2]
          {
            ODataPayloadKind.Property,
            ODataPayloadKind.Resource
          };
          flag = true;
        }
      }
      else if (edmType != null && edmType.TypeKind == EdmTypeKind.Collection && ((IEdmCollectionType) edmType).ElementType.TypeKind() == EdmTypeKind.Untyped)
      {
        this.parseResult.DetectedPayloadKinds = (IEnumerable<ODataPayloadKind>) new ODataPayloadKind[3]
        {
          ODataPayloadKind.ResourceSet,
          ODataPayloadKind.Property,
          ODataPayloadKind.Collection
        };
        if (expectedPayloadKind == ODataPayloadKind.ResourceSet || expectedPayloadKind == ODataPayloadKind.Property || expectedPayloadKind == ODataPayloadKind.Collection)
          flag = true;
      }
      else if (contextUriFragment == ODataPayloadKind.ResourceSet && edmType.IsODataComplexTypeKind())
      {
        this.parseResult.DetectedPayloadKinds = (IEnumerable<ODataPayloadKind>) new ODataPayloadKind[3]
        {
          ODataPayloadKind.ResourceSet,
          ODataPayloadKind.Property,
          ODataPayloadKind.Collection
        };
        if (expectedPayloadKind == ODataPayloadKind.Property || expectedPayloadKind == ODataPayloadKind.Collection)
          flag = true;
      }
      else if (contextUriFragment == ODataPayloadKind.Resource && edmType.IsODataComplexTypeKind())
      {
        this.parseResult.DetectedPayloadKinds = (IEnumerable<ODataPayloadKind>) new ODataPayloadKind[2]
        {
          ODataPayloadKind.Resource,
          ODataPayloadKind.Property
        };
        if (expectedPayloadKind == ODataPayloadKind.Property || expectedPayloadKind == ODataPayloadKind.Delta)
          flag = true;
      }
      else
      {
        switch (contextUriFragment)
        {
          case ODataPayloadKind.Resource:
            this.parseResult.DetectedPayloadKinds = (IEnumerable<ODataPayloadKind>) new ODataPayloadKind[2]
            {
              ODataPayloadKind.Resource,
              ODataPayloadKind.Delta
            };
            if (expectedPayloadKind == ODataPayloadKind.Delta)
            {
              this.parseResult.DeltaKind = ODataDeltaKind.Resource;
              flag = true;
              break;
            }
            break;
          case ODataPayloadKind.Collection:
            this.parseResult.DetectedPayloadKinds = (IEnumerable<ODataPayloadKind>) new ODataPayloadKind[2]
            {
              ODataPayloadKind.Collection,
              ODataPayloadKind.Property
            };
            if (expectedPayloadKind == ODataPayloadKind.Property)
            {
              flag = true;
              break;
            }
            break;
          default:
            if (contextUriFragment == ODataPayloadKind.Property & isUndeclared && (expectedPayloadKind == ODataPayloadKind.Resource || expectedPayloadKind == ODataPayloadKind.ResourceSet))
            {
              this.parseResult.DetectedPayloadKinds = (IEnumerable<ODataPayloadKind>) new ODataPayloadKind[2]
              {
                expectedPayloadKind,
                ODataPayloadKind.Property
              };
              flag = true;
              break;
            }
            this.parseResult.DetectedPayloadKinds = (IEnumerable<ODataPayloadKind>) new ODataPayloadKind[1]
            {
              contextUriFragment
            };
            break;
        }
      }
      if (!flag)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightContextUriParser_ContextUriDoesNotMatchExpectedPayloadKind((object) UriUtils.UriToString(this.parseResult.ContextUri), (object) expectedPayloadKind.ToString()));
      if (this.parseResult.SelectQueryOption != null && contextUriFragment != ODataPayloadKind.ResourceSet && contextUriFragment != ODataPayloadKind.Resource && contextUriFragment != ODataPayloadKind.Delta)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightContextUriParser_InvalidPayloadKindWithSelectQueryOption((object) expectedPayloadKind.ToString()));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "Will be moving to non case statements later, no point in investing in reducing this now")]
    private ODataPayloadKind ParseContextUriFragment(
      string fragment,
      Func<IEdmType, string, IEdmType> clientCustomTypeResolver,
      bool throwIfMetadataConflict,
      out bool isUndeclared)
    {
      bool flag = false;
      ODataDeltaKind odataDeltaKind = ODataDeltaKind.None;
      isUndeclared = false;
      if (fragment.EndsWith("/$entity", StringComparison.Ordinal))
      {
        flag = true;
        fragment = fragment.Substring(0, fragment.Length - "/$entity".Length);
      }
      else if (fragment.EndsWith("/$delta", StringComparison.Ordinal))
      {
        odataDeltaKind = ODataDeltaKind.ResourceSet;
        fragment = fragment.Substring(0, fragment.Length - "/$delta".Length);
      }
      else if (fragment.EndsWith("/$deletedEntity", StringComparison.Ordinal))
      {
        odataDeltaKind = ODataDeltaKind.DeletedEntry;
        fragment = fragment.Substring(0, fragment.Length - "/$deletedEntity".Length);
      }
      else if (fragment.EndsWith("/$link", StringComparison.Ordinal))
      {
        odataDeltaKind = ODataDeltaKind.Link;
        fragment = fragment.Substring(0, fragment.Length - "/$link".Length);
      }
      else if (fragment.EndsWith("/$deletedLink", StringComparison.Ordinal))
      {
        odataDeltaKind = ODataDeltaKind.DeletedLink;
        fragment = fragment.Substring(0, fragment.Length - "/$deletedLink".Length);
      }
      this.parseResult.DeltaKind = odataDeltaKind;
      if (fragment.EndsWith(")", StringComparison.Ordinal))
      {
        int index1 = fragment.Length - 2;
        for (int index2 = 1; index2 > 0 && index1 > 0; --index1)
        {
          switch (fragment[index1])
          {
            case '(':
              --index2;
              break;
            case ')':
              ++index2;
              break;
          }
        }
        string str1 = index1 != 0 ? fragment.Substring(0, index1 + 1) : throw new ODataException(Microsoft.OData.Strings.ODataJsonLightContextUriParser_InvalidContextUrl((object) UriUtils.UriToString(this.parseResult.ContextUri)));
        if (!str1.Equals("Collection"))
        {
          string str2 = fragment.Substring(index1 + 2);
          string str3 = str2.Substring(0, str2.Length - 1);
          this.parseResult.SelectQueryOption = !ODataJsonLightContextUriParser.KeyPattern.IsMatch(str3) ? ODataJsonLightContextUriParser.ExtractSelectQueryOption(str3) : throw new ODataException(Microsoft.OData.Strings.ODataJsonLightContextUriParser_LastSegmentIsKeySegment((object) UriUtils.UriToString(this.parseResult.ContextUri)));
          fragment = str1;
        }
      }
      EdmTypeResolver edmTypeResolver = (EdmTypeResolver) new EdmTypeReaderResolver(this.model, clientCustomTypeResolver);
      ODataPayloadKind contextUriFragment;
      if (!fragment.Contains("/") && !flag && odataDeltaKind == ODataDeltaKind.None)
      {
        switch (fragment)
        {
          case "":
            contextUriFragment = ODataPayloadKind.ServiceDocument;
            break;
          case "Collection($ref)":
            contextUriFragment = ODataPayloadKind.EntityReferenceLinks;
            break;
          case "$ref":
            contextUriFragment = ODataPayloadKind.EntityReferenceLink;
            break;
          default:
            IEdmNavigationSource navigationSource = this.model.FindDeclaredNavigationSource(fragment);
            if (navigationSource != null)
            {
              this.parseResult.NavigationSource = navigationSource;
              this.parseResult.EdmType = (IEdmType) edmTypeResolver.GetElementType(navigationSource);
              contextUriFragment = navigationSource is IEdmSingleton ? ODataPayloadKind.Resource : ODataPayloadKind.ResourceSet;
              break;
            }
            contextUriFragment = this.ResolveType(fragment, clientCustomTypeResolver, throwIfMetadataConflict);
            break;
        }
      }
      else
      {
        string str = UriUtils.UriToString(this.parseResult.MetadataDocumentUri);
        Uri uri = str.EndsWith("$metadata", StringComparison.Ordinal) ? new Uri(str.Substring(0, str.Length - "$metadata".Length)) : throw new ODataException(Microsoft.OData.Strings.ODataJsonLightContextUriParser_InvalidContextUrl((object) UriUtils.UriToString(this.parseResult.ContextUri)));
        ODataUriParser odataUriParser = new ODataUriParser(this.model, uri, new Uri(uri, fragment));
        ODataPath path;
        try
        {
          path = odataUriParser.ParsePath();
        }
        catch (ODataException ex)
        {
          throw new ODataException(Microsoft.OData.Strings.ODataJsonLightContextUriParser_InvalidContextUrl((object) UriUtils.UriToString(this.parseResult.ContextUri)));
        }
        this.parseResult.Path = path.Count != 0 ? path : throw new ODataException(Microsoft.OData.Strings.ODataJsonLightContextUriParser_InvalidContextUrl((object) UriUtils.UriToString(this.parseResult.ContextUri)));
        this.parseResult.NavigationSource = path.NavigationSource();
        this.parseResult.EdmType = path.LastSegment.EdmType;
        switch (path.TrimEndingTypeSegment().LastSegment)
        {
          case EntitySetSegment _:
          case NavigationPropertySegment _:
            contextUriFragment = odataDeltaKind == ODataDeltaKind.None ? (flag ? ODataPayloadKind.Resource : ODataPayloadKind.ResourceSet) : ODataPayloadKind.Delta;
            if (this.parseResult.EdmType is IEdmCollectionType)
            {
              IEdmCollectionTypeReference type = this.parseResult.EdmType.ToTypeReference().AsCollection();
              if (type != null)
              {
                this.parseResult.EdmType = type.ElementType().Definition;
                break;
              }
              break;
            }
            break;
          case SingletonSegment _:
            contextUriFragment = ODataPayloadKind.Resource;
            break;
          default:
            isUndeclared = path.IsIndividualProperty() ? path.IsUndeclared() : throw new ODataException(Microsoft.OData.Strings.ODataJsonLightContextUriParser_InvalidContextUrl((object) UriUtils.UriToString(this.parseResult.ContextUri)));
            contextUriFragment = ODataPayloadKind.Property;
            if (this.parseResult.EdmType is IEdmComplexType)
            {
              contextUriFragment = ODataPayloadKind.Resource;
              break;
            }
            if (this.parseResult.EdmType is IEdmCollectionType edmType)
            {
              if (edmType.ElementType.IsComplex())
              {
                this.parseResult.EdmType = edmType.ElementType.Definition;
                contextUriFragment = ODataPayloadKind.ResourceSet;
                break;
              }
              contextUriFragment = ODataPayloadKind.Collection;
              break;
            }
            break;
        }
      }
      return contextUriFragment;
    }

    private ODataPayloadKind ResolveType(
      string typeName,
      Func<IEdmType, string, IEdmType> clientCustomTypeResolver,
      bool throwIfMetadataConflict)
    {
      string typeName1 = EdmLibraryExtensions.GetCollectionItemTypeName(typeName) ?? typeName;
      bool isCollection = typeName1 != typeName;
      IEdmType type = MetadataUtils.ResolveTypeNameForRead(this.model, (IEdmType) null, typeName1, clientCustomTypeResolver, out EdmTypeKind _);
      if (type == null && !throwIfMetadataConflict)
      {
        string namespaceName;
        string typeName2;
        TypeUtils.ParseQualifiedTypeName(typeName, out namespaceName, out typeName2, out isCollection);
        type = (IEdmType) new EdmUntypedStructuredType(namespaceName, typeName2);
      }
      if (type == null || type.TypeKind != EdmTypeKind.Primitive && type.TypeKind != EdmTypeKind.Enum && type.TypeKind != EdmTypeKind.Complex && type.TypeKind != EdmTypeKind.Entity && type.TypeKind != EdmTypeKind.TypeDefinition && type.TypeKind != EdmTypeKind.Untyped)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightContextUriParser_InvalidEntitySetNameOrTypeName((object) UriUtils.UriToString(this.parseResult.ContextUri), (object) typeName));
      if (type.TypeKind == EdmTypeKind.Entity || type.TypeKind == EdmTypeKind.Complex)
      {
        this.parseResult.EdmType = type;
        return !isCollection ? ODataPayloadKind.Resource : ODataPayloadKind.ResourceSet;
      }
      this.parseResult.EdmType = isCollection ? (IEdmType) EdmLibraryExtensions.GetCollectionType(type.ToTypeReference(true)) : type;
      return !isCollection ? ODataPayloadKind.Property : ODataPayloadKind.Collection;
    }
  }
}
