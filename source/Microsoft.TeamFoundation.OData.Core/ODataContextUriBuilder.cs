// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataContextUriBuilder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData
{
  internal sealed class ODataContextUriBuilder
  {
    private readonly Uri baseContextUrl;
    private readonly bool throwIfMissingInfo;
    private static readonly Dictionary<ODataPayloadKind, Action<ODataContextUrlInfo>> ValidationDictionary = new Dictionary<ODataPayloadKind, Action<ODataContextUrlInfo>>((IEqualityComparer<ODataPayloadKind>) EqualityComparer<ODataPayloadKind>.Default)
    {
      {
        ODataPayloadKind.ServiceDocument,
        (Action<ODataContextUrlInfo>) null
      },
      {
        ODataPayloadKind.EntityReferenceLink,
        (Action<ODataContextUrlInfo>) null
      },
      {
        ODataPayloadKind.EntityReferenceLinks,
        (Action<ODataContextUrlInfo>) null
      },
      {
        ODataPayloadKind.IndividualProperty,
        new Action<ODataContextUrlInfo>(ODataContextUriBuilder.ValidateResourcePath)
      },
      {
        ODataPayloadKind.Collection,
        new Action<ODataContextUrlInfo>(ODataContextUriBuilder.ValidateCollectionType)
      },
      {
        ODataPayloadKind.Property,
        new Action<ODataContextUrlInfo>(ODataContextUriBuilder.ValidateType)
      },
      {
        ODataPayloadKind.Resource,
        new Action<ODataContextUrlInfo>(ODataContextUriBuilder.ValidateNavigationSource)
      },
      {
        ODataPayloadKind.ResourceSet,
        new Action<ODataContextUrlInfo>(ODataContextUriBuilder.ValidateNavigationSource)
      },
      {
        ODataPayloadKind.Delta,
        new Action<ODataContextUrlInfo>(ODataContextUriBuilder.ValidateDelta)
      }
    };

    private ODataContextUriBuilder(Uri baseContextUrl, bool throwIfMissingInfo)
    {
      this.baseContextUrl = baseContextUrl;
      this.throwIfMissingInfo = throwIfMissingInfo;
    }

    internal static ODataContextUriBuilder Create(Uri baseContextUrl, bool throwIfMissingInfo) => !(baseContextUrl == (Uri) null & throwIfMissingInfo) ? new ODataContextUriBuilder(baseContextUrl, throwIfMissingInfo) : throw new ODataException(Strings.ODataOutputContext_MetadataDocumentUriMissing);

    internal Uri BuildContextUri(ODataPayloadKind payloadKind, ODataContextUrlInfo contextInfo = null)
    {
      if (this.baseContextUrl == (Uri) null)
        return (Uri) null;
      Action<ODataContextUrlInfo> action;
      if (!ODataContextUriBuilder.ValidationDictionary.TryGetValue(payloadKind, out action))
        throw new ODataException(Strings.ODataContextUriBuilder_UnsupportedPayloadKind((object) payloadKind.ToString()));
      if (action != null && this.throwIfMissingInfo)
        action(contextInfo);
      switch (payloadKind)
      {
        case ODataPayloadKind.EntityReferenceLink:
          return new Uri(this.baseContextUrl, "#$ref");
        case ODataPayloadKind.EntityReferenceLinks:
          return new Uri(this.baseContextUrl, "#Collection($ref)");
        case ODataPayloadKind.ServiceDocument:
          return this.baseContextUrl;
        default:
          return this.CreateFromContextUrlInfo(contextInfo);
      }
    }

    private Uri CreateFromContextUrlInfo(ODataContextUrlInfo info)
    {
      StringBuilder builder = new StringBuilder();
      builder.Append('#');
      if (!string.IsNullOrEmpty(info.ResourcePath))
      {
        builder.Append(info.ResourcePath);
        if (info.DeltaKind == ODataDeltaKind.None)
          ODataContextUriBuilder.AppendTypeCastAndQueryClause(builder, info);
      }
      else if (!string.IsNullOrEmpty(info.NavigationPath))
      {
        builder.Append(info.NavigationPath);
        if (info.DeltaKind == ODataDeltaKind.None || info.DeltaKind == ODataDeltaKind.ResourceSet || info.DeltaKind == ODataDeltaKind.Resource)
          ODataContextUriBuilder.AppendTypeCastAndQueryClause(builder, info);
        switch (info.DeltaKind)
        {
          case ODataDeltaKind.None:
          case ODataDeltaKind.Resource:
            if (info.IncludeFragmentItemSelector)
            {
              builder.Append("/$entity");
              break;
            }
            break;
          case ODataDeltaKind.ResourceSet:
            builder.Append("/$delta");
            break;
          case ODataDeltaKind.DeletedEntry:
            builder.Append("/$deletedEntity");
            break;
          case ODataDeltaKind.Link:
            builder.Append("/$link");
            break;
          case ODataDeltaKind.DeletedLink:
            builder.Append("/$deletedLink");
            break;
        }
      }
      else
      {
        switch (info.DeltaKind)
        {
          case ODataDeltaKind.ResourceSet:
            return new Uri("#$delta", UriKind.Relative);
          case ODataDeltaKind.DeletedEntry:
            return new Uri("#$deletedEntity", UriKind.Relative);
          case ODataDeltaKind.Link:
            return new Uri("#$link", UriKind.Relative);
          case ODataDeltaKind.DeletedLink:
            return new Uri("#$deletedLink", UriKind.Relative);
          default:
            if (string.IsNullOrEmpty(info.TypeName))
              return (Uri) null;
            builder.Append(info.TypeName);
            break;
        }
      }
      return new Uri(this.baseContextUrl, builder.ToString());
    }

    private static void AppendTypeCastAndQueryClause(
      StringBuilder builder,
      ODataContextUrlInfo info)
    {
      if (!string.IsNullOrEmpty(info.TypeCast))
      {
        builder.Append('/');
        builder.Append(info.TypeCast);
      }
      if (string.IsNullOrEmpty(info.QueryClause))
        return;
      builder.Append(info.QueryClause);
    }

    private static void ValidateType(ODataContextUrlInfo contextUrlInfo)
    {
      if (string.IsNullOrEmpty(contextUrlInfo.TypeName))
        throw new ODataException(Strings.ODataContextUriBuilder_TypeNameMissingForProperty);
    }

    private static void ValidateCollectionType(ODataContextUrlInfo contextUrlInfo)
    {
      if (string.IsNullOrEmpty(contextUrlInfo.TypeName))
        throw new ODataException(Strings.ODataContextUriBuilder_TypeNameMissingForTopLevelCollection);
    }

    private static void ValidateNavigationSource(ODataContextUrlInfo contextUrlInfo)
    {
      if (!contextUrlInfo.HasNavigationSourceInfo)
      {
        if (string.IsNullOrEmpty(contextUrlInfo.TypeName))
          throw new ODataException(Strings.ODataContextUriBuilder_NavigationSourceOrTypeNameMissingForResourceOrResourceSet);
      }
      else if (!contextUrlInfo.IsUnknownEntitySet && string.IsNullOrEmpty(contextUrlInfo.NavigationPath) || contextUrlInfo.IsUnknownEntitySet && string.IsNullOrEmpty(contextUrlInfo.NavigationSource) && string.IsNullOrEmpty(contextUrlInfo.TypeName))
        throw new ODataException(Strings.ODataContextUriBuilder_NavigationSourceOrTypeNameMissingForResourceOrResourceSet);
    }

    private static void ValidateResourcePath(ODataContextUrlInfo contextUrlInfo)
    {
      if (string.IsNullOrEmpty(contextUrlInfo.ResourcePath))
        throw new ODataException(Strings.ODataContextUriBuilder_ODataUriMissingForIndividualProperty);
    }

    private static void ValidateDelta(ODataContextUrlInfo contextUrlInfo)
    {
    }
  }
}
