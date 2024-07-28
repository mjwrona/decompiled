// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.DefaultODataPathHandler
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Routing.Template;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Routing
{
  public class DefaultODataPathHandler : IODataPathHandler, IODataPathTemplateHandler
  {
    private const int MaxUriSchemeName = 1024;

    public ODataUrlKeyDelimiter UrlKeyDelimiter { get; set; }

    public virtual ODataPath Parse(
      string serviceRoot,
      string odataPath,
      IServiceProvider requestContainer)
    {
      if (serviceRoot == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (serviceRoot));
      if (odataPath == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (odataPath));
      if (requestContainer == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (requestContainer));
      return this.Parse(serviceRoot, odataPath, requestContainer, false);
    }

    public virtual ODataPathTemplate ParseTemplate(
      string odataPathTemplate,
      IServiceProvider requestContainer)
    {
      if (odataPathTemplate == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (odataPathTemplate));
      return requestContainer != null ? DefaultODataPathHandler.Templatify(this.Parse((string) null, odataPathTemplate, requestContainer, true), odataPathTemplate) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (requestContainer));
    }

    public virtual string Link(ODataPath path) => path != null ? path.ToString() : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (path));

    private ODataPath Parse(
      string serviceRoot,
      string odataPath,
      IServiceProvider requestContainer,
      bool template)
    {
      Uri uri1 = (Uri) null;
      Uri result = (Uri) null;
      IEdmModel requiredService = ServiceProviderServiceExtensions.GetRequiredService<IEdmModel>(requestContainer);
      ODataUriParser odataUriParser;
      if (template)
      {
        odataUriParser = new ODataUriParser(requiredService, new Uri(odataPath, UriKind.Relative), requestContainer);
        odataUriParser.EnableUriTemplateParsing = true;
      }
      else
      {
        uri1 = new Uri(serviceRoot.EndsWith("/", StringComparison.Ordinal) ? serviceRoot : serviceRoot + "/");
        if (!Uri.TryCreate(odataPath, UriKind.Absolute, out result))
        {
          Uri uri2 = new Uri(uri1?.ToString() + odataPath);
        }
        if (!Uri.IsWellFormedUriString(odataPath, UriKind.RelativeOrAbsolute) && odataPath.IndexOf(':') > 1024)
        {
          string uriString = odataPath.Replace(":", "%3A");
          if (Uri.IsWellFormedUriString(uriString, UriKind.Relative))
            odataPath = uriString;
        }
        result = new Uri(uri1, odataPath);
        odataUriParser = new ODataUriParser(requiredService, uri1, result, requestContainer);
      }
      odataUriParser.UrlKeyDelimiter = this.UrlKeyDelimiter == null ? ODataUrlKeyDelimiter.Slash : this.UrlKeyDelimiter;
      UnresolvedPathSegment unresolvedPathSegment = (UnresolvedPathSegment) null;
      KeySegment id = (KeySegment) null;
      Microsoft.OData.UriParser.ODataPath path;
      try
      {
        path = odataUriParser.ParsePath();
      }
      catch (ODataUnrecognizedPathException ex)
      {
        if (ex.ParsedSegments != null && ex.ParsedSegments.Any<ODataPathSegment>() && (ex.ParsedSegments.Last<ODataPathSegment>().EdmType is IEdmComplexType || ex.ParsedSegments.Last<ODataPathSegment>().EdmType is IEdmEntityType) && ex.CurrentSegment != "$count")
        {
          path = !ex.UnparsedSegments.Any<string>() ? new Microsoft.OData.UriParser.ODataPath(ex.ParsedSegments) : throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.InvalidPathSegment, (object) ex.UnparsedSegments.First<string>(), (object) ex.CurrentSegment));
          unresolvedPathSegment = new UnresolvedPathSegment(ex.CurrentSegment);
        }
        else
          throw;
      }
      if (!template && path.LastSegment is NavigationPropertyLinkSegment && path.LastSegment.EdmType is IEdmCollectionType edmType)
      {
        EntityIdSegment entityIdSegment = (EntityIdSegment) null;
        bool flag = false;
        try
        {
          entityIdSegment = odataUriParser.ParseEntityId();
          if (entityIdSegment != null)
            id = new ODataUriParser(requiredService, uri1, entityIdSegment.Id, requestContainer).ParsePath().LastSegment as KeySegment;
        }
        catch (ODataException ex)
        {
          flag = true;
        }
        if (flag || entityIdSegment != null && (id == null || !id.EdmType.IsOrInheritsFrom(edmType.ElementType.Definition) && !edmType.ElementType.Definition.IsOrInheritsFrom(id.EdmType)))
        {
          string str1 = result.Query;
          string str2 = "$id=";
          int startIndex = str1.IndexOf(str2, StringComparison.OrdinalIgnoreCase);
          int num = startIndex >= 0 ? str1.IndexOf("&", startIndex, StringComparison.OrdinalIgnoreCase) : throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.InvalidDollarId, (object) str1));
          str1 = num < 0 ? str1.Substring(startIndex + str2.Length) : str1.Substring(startIndex + str2.Length, num - 1);
        }
      }
      path.WalkWith((PathSegmentHandler) new DefaultODataPathValidator(requiredService));
      List<ODataPathSegment> list = ODataPathSegmentTranslator.Translate(requiredService, path, odataUriParser.ParameterAliasNodes).ToList<ODataPathSegment>();
      if (unresolvedPathSegment != null)
        list.Add((ODataPathSegment) unresolvedPathSegment);
      if (!template)
        DefaultODataPathHandler.AppendIdForRef((IList<ODataPathSegment>) list, id);
      return new ODataPath((IEnumerable<ODataPathSegment>) list)
      {
        Path = path
      };
    }

    private static void AppendIdForRef(IList<ODataPathSegment> segments, KeySegment id)
    {
      if (id == null || !(segments.Last<ODataPathSegment>() is NavigationPropertyLinkSegment))
        return;
      segments.Add((ODataPathSegment) id);
    }

    private static ODataPathTemplate Templatify(ODataPath path, string pathTemplate)
    {
      if (path == null)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.InvalidODataPathTemplate, (object) pathTemplate));
      ODataPathSegmentTemplateTranslator translator = new ODataPathSegmentTemplateTranslator();
      return new ODataPathTemplate(path.Segments.Select<ODataPathSegment, ODataPathSegmentTemplate>((Func<ODataPathSegment, ODataPathSegmentTemplate>) (e =>
      {
        return !(e is UnresolvedPathSegment unresolvedPathSegment2) ? e.TranslateWith<ODataPathSegmentTemplate>((PathSegmentTranslator<ODataPathSegmentTemplate>) translator) : throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.UnresolvedPathSegmentInTemplate, (object) unresolvedPathSegment2, (object) pathTemplate));
      })));
    }
  }
}
