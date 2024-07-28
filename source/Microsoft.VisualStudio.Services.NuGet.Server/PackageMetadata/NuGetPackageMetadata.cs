// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata.NuGetPackageMetadata
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata
{
  public class NuGetPackageMetadata
  {
    public NuGetPackageMetadata(
      byte[] nuspecBytes,
      bool areBytesCompressed,
      MetadataReadOptions options = MetadataReadOptions.None,
      bool listed = true)
      : this(NuGetPackageMetadata.GetNuspecFromBytes(nuspecBytes, areBytesCompressed), options, listed)
    {
    }

    public NuGetPackageMetadata(XDocument nuspec, MetadataReadOptions options = MetadataReadOptions.None, bool listed = true)
    {
      ArgumentUtility.CheckForNull<XDocument>(nuspec, nameof (nuspec));
      this.StripNamespaces(nuspec);
      XElement requiredElement = this.GetRequiredElement((XContainer) this.GetRequiredElement((XContainer) nuspec, "package"), "metadata");
      string elementStringValue = this.GetRequiredElementStringValue((XContainer) requiredElement, "id");
      NuGetVersion version;
      if (!NuGetVersion.TryParse(this.GetRequiredElementStringValue((XContainer) requiredElement, "version"), out version))
        throw InvalidPackageExceptionHelper.InvalidVersionFormat();
      this.Identity = new VssNuGetPackageIdentity(new VssNuGetPackageName(elementStringValue), new VssNuGetPackageVersion(version));
      this.Copyright = (string) requiredElement.Element((XName) "copyright");
      this.Description = (string) requiredElement.Element((XName) "description");
      this.Language = (string) requiredElement.Element((XName) "language");
      this.MinClientVersion = (string) requiredElement.Attribute((XName) "minClientVersion");
      this.ProjectUrl = (string) requiredElement.Element((XName) "projectUrl");
      this.ReleaseNotes = (string) requiredElement.Element((XName) "releaseNotes");
      this.Summary = (string) requiredElement.Element((XName) "summary");
      this.Title = (string) requiredElement.Element((XName) "title");
      this.IconUrl = (string) requiredElement.Element((XName) "iconUrl");
      this.IconFile = (string) requiredElement.Element((XName) "icon");
      this.LicenseUrl = (string) requiredElement.Element((XName) "licenseUrl");
      XElement xelement = requiredElement.Element((XName) "license");
      if (xelement != null)
      {
        string str = (string) xelement.Attribute((XName) "type");
        if (!string.IsNullOrWhiteSpace(str))
        {
          if (str.Equals("expression", StringComparison.OrdinalIgnoreCase))
            this.LicenseExpression = (string) xelement;
          else if (str.Equals("file", StringComparison.OrdinalIgnoreCase))
            this.LicenseFile = (string) xelement;
        }
      }
      string str1 = (string) requiredElement.Element((XName) "requireLicenseAcceptance");
      bool result;
      if (!string.IsNullOrWhiteSpace(str1) && bool.TryParse(str1, out result))
        this.RequireLicenseAcceptance = new bool?(result);
      string input = (string) requiredElement.Element((XName) "tags");
      if (input != null)
        this.Tags = ((IEnumerable<string>) Regex.Split(input, "\\s+")).Where<string>((Func<string, bool>) (x => !string.IsNullOrWhiteSpace(x))).ToImmutableList<string>();
      this.Authors = (string) requiredElement.Element((XName) "authors");
      this.DependencyGroups = this.LoadDependencyGroups(requiredElement, options);
      this.Listed = listed;
    }

    public ImmutableList<NuGetDependencyGroup> DependencyGroups { get; }

    public VssNuGetPackageIdentity Identity { get; }

    public string Authors { get; }

    public string Copyright { get; }

    public string Description { get; }

    public string IconUrl { get; }

    public string IconFile { get; }

    public string Language { get; }

    public string LicenseExpression { get; }

    public string LicenseUrl { get; }

    public string LicenseFile { get; }

    public string MinClientVersion { get; }

    public string ProjectUrl { get; }

    public string ReleaseNotes { get; }

    public bool? RequireLicenseAcceptance { get; }

    public string Summary { get; }

    public ImmutableList<string> Tags { get; }

    public string Title { get; }

    public bool Listed { get; }

    private static XDocument GetNuspecFromBytes(byte[] nuSpecBytes, bool areBytesCompressed)
    {
      using (MemoryStream memoryStream = new MemoryStream(nuSpecBytes))
      {
        if (areBytesCompressed)
        {
          using (DeflateStream deflateStream = new DeflateStream((Stream) memoryStream, CompressionMode.Decompress))
          {
            using (StreamReader streamReader = new StreamReader((Stream) deflateStream))
              return XDocument.Load((TextReader) streamReader);
          }
        }
        else
        {
          using (StreamReader streamReader = new StreamReader((Stream) memoryStream))
            return XDocument.Load((TextReader) streamReader);
        }
      }
    }

    private ImmutableList<NuGetDependencyGroup> LoadDependencyGroups(
      XElement metadataElement,
      MetadataReadOptions options)
    {
      XElement groupElement = metadataElement.Element((XName) "dependencies");
      if (groupElement == null)
        return ImmutableList<NuGetDependencyGroup>.Empty;
      ImmutableList<NuGetDependencyGroup> immutableList = ImmutableList<NuGetDependencyGroup>.Empty;
      NuGetDependencyGroup getDependencyGroup = this.LoadOneDependencyGroup(groupElement, true, options);
      if (getDependencyGroup.Dependencies.Count > 0)
        immutableList = immutableList.Add(getDependencyGroup);
      IEnumerable<XElement> source = groupElement.Elements((XName) "group");
      return immutableList.AddRange(source.Select<XElement, NuGetDependencyGroup>((Func<XElement, NuGetDependencyGroup>) (x => this.LoadOneDependencyGroup(x, false, options))));
    }

    private NuGetDependencyGroup LoadOneDependencyGroup(
      XElement groupElement,
      bool isOldStyleDependencyList,
      MetadataReadOptions options)
    {
      string targetFramework = (string) groupElement.Attribute((XName) "targetFramework");
      IEnumerable<NuGetDependency> dependencies = groupElement.Elements((XName) "dependency").Select<XElement, NuGetDependency>((Func<XElement, NuGetDependency>) (arg => this.LoadOneDependency(arg, targetFramework, options))).Where<NuGetDependency>((Func<NuGetDependency, bool>) (x => x != null));
      return new NuGetDependencyGroup(targetFramework, isOldStyleDependencyList, dependencies);
    }

    private NuGetDependency LoadOneDependency(
      XElement dependencyElement,
      string targetFramework,
      MetadataReadOptions options)
    {
      string str1 = (string) dependencyElement.Attribute((XName) "id");
      string str2 = (string) dependencyElement.Attribute((XName) "version");
      if (string.IsNullOrWhiteSpace(str1))
      {
        if (options.HasFlag((Enum) MetadataReadOptions.IgnoreNonCriticalErrors))
          return (NuGetDependency) null;
        throw InvalidPackageExceptionHelper.DependencyIdNotSpecified();
      }
      VersionRange versionRange;
      if (string.IsNullOrWhiteSpace(str2))
        versionRange = VersionRange.All;
      else if (!VersionRange.TryParse(str2, out versionRange))
      {
        if (options.HasFlag((Enum) MetadataReadOptions.IgnoreNonCriticalErrors))
          return (NuGetDependency) null;
        throw InvalidPackageExceptionHelper.DependencyVersionFormat(targetFramework, str1);
      }
      return new NuGetDependency(new VssNuGetPackageName(str1), versionRange);
    }

    private XElement GetRequiredElement(XContainer container, string elementName) => container.Element((XName) elementName) ?? throw InvalidPackageExceptionHelper.MissingNuspecElement(elementName);

    private string GetRequiredElementStringValue(XContainer container, string elementName)
    {
      string str = (string) container.Element((XName) elementName);
      return !string.IsNullOrWhiteSpace(str) ? str : throw InvalidPackageExceptionHelper.MissingNuspecElement(elementName);
    }

    private void StripNamespaces(XDocument nuspec)
    {
      foreach (XElement descendant in nuspec.Descendants())
      {
        descendant.Name = XNamespace.None.GetName(descendant.Name.LocalName);
        foreach (XAttribute attribute in descendant.Attributes())
        {
          if (attribute.IsNamespaceDeclaration)
            attribute.Remove();
          else if (attribute.Name.Namespace != XNamespace.None)
          {
            attribute.Remove();
            descendant.SetAttributeValue(XNamespace.None.GetName(attribute.Name.LocalName), (object) attribute.Value);
          }
        }
      }
    }
  }
}
