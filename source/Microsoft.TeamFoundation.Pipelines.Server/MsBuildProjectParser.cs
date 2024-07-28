// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.MsBuildProjectParser
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  internal class MsBuildProjectParser
  {
    private IVssRequestContext m_requestContext;
    private IFileContentsProvider m_fileContentsProvider;

    public MsBuildProjectParser(
      IVssRequestContext requestContext,
      IFileContentsProvider fileContentsProvider)
    {
      this.m_requestContext = requestContext;
      this.m_fileContentsProvider = fileContentsProvider;
    }

    public IEnumerable<VsProjectFile> ParseSolutionFile(
      FilePath solutionPath,
      bool skipLoadingProjects)
    {
      string contents;
      if (this.m_fileContentsProvider.TryGetFileContents(this.m_requestContext, solutionPath.ToString(), out contents))
      {
        string pattern = "Project\\(\"([^\"]+)\"\\)\\s*=\\s*\"([^\"]+)\",\\s*\"([^\"]+)\",\\s*\"([^\"]+)\"([\\s\\S]*?)EndProject\\s";
        foreach (Match match in Regex.Matches(contents, pattern, RegexOptions.Singleline))
        {
          string input = match.Groups[1].Value;
          string nameOverride = match.Groups[2].Value;
          string projectLocation = match.Groups[3].Value;
          string idOverride = match.Groups[4].Value;
          string projectSection = match.Groups[5].Value;
          Guid result;
          if (Guid.TryParse(input, out result))
          {
            VsProjectType additionalProjectType = VsProjectType.FromGuid(result);
            if (!(additionalProjectType.Type == VsProjectType.WellKnownTypes.SolutionFolder))
            {
              FilePath projectPath = this.GetProjectPath(solutionPath, projectLocation, projectSection, additionalProjectType.Type);
              bool skipLoadingProject = additionalProjectType.Type == VsProjectType.WellKnownTypes.WebSite | skipLoadingProjects;
              yield return this.ParseProjectFileInternal(projectPath, nameOverride, idOverride, additionalProjectType, skipLoadingProject);
            }
          }
          else
            this.m_requestContext.TraceError(TracePoints.BuildFrameworkDetection.MsBuildTryDetectError, nameof (MsBuildProjectParser), "Failed to the parse the project type '" + input + "' in one of the solution files");
        }
      }
    }

    public VsProjectFile ParseProjectFile(FilePath projectPath) => this.ParseProjectFileInternal(projectPath);

    private VsProjectFile ParseProjectFileInternal(
      FilePath projectLocation,
      string nameOverride = null,
      string idOverride = null,
      VsProjectType additionalProjectType = null,
      bool skipLoadingProject = false)
    {
      bool fileLoaded;
      XDocument projectDocument = this.GetProjectDocument(projectLocation, skipLoadingProject, out fileLoaded);
      string input = idOverride ?? this.GetElementValue(projectDocument, "PropertyGroup/ProjectGuid");
      string name = nameOverride ?? this.GetElementValue(projectDocument, "PropertyGroup/AssemblyName");
      string elementValue1 = this.GetElementValue(projectDocument, "PropertyGroup/TargetFramework");
      string elementValue2 = this.GetElementValue(projectDocument, "PropertyGroup/TargetFrameworks");
      string elementValue3 = this.GetElementValue(projectDocument, "PropertyGroup/TargetFrameworkVersion");
      string attributeValue = this.GetAttributeValue(projectDocument, "Project", "ToolsVersion");
      List<string> list1 = this.GetPackageReferences(projectDocument).ToList<string>();
      List<string> list2 = this.GetProjectReferences(projectDocument).ToList<string>();
      Guid result = Guid.Empty;
      if (input != null)
        Guid.TryParse(input, out result);
      return new VsProjectFile(result, name, projectLocation, elementValue2 ?? elementValue1 ?? elementValue3, attributeValue, (IReadOnlyList<VsProjectType>) this.GetProjectTypes(projectDocument, additionalProjectType, (IReadOnlyList<string>) list1).ToList<VsProjectType>(), (IReadOnlyList<string>) list2, (IReadOnlyList<string>) list1, fileLoaded);
    }

    private FilePath GetProjectPath(
      FilePath solutionPath,
      string projectLocation,
      string projectSection,
      Guid projectTypeFromSolution)
    {
      if (!(projectTypeFromSolution == VsProjectType.WellKnownTypes.WebSite))
        return solutionPath.Folder.AppendPath(projectLocation);
      string siteProjectLocation = this.GetWebSiteProjectLocation(projectLocation, projectSection);
      return solutionPath.Folder.AppendPath(siteProjectLocation);
    }

    private XDocument GetProjectDocument(
      FilePath fullPath,
      bool skipLoadingProject,
      out bool fileLoaded)
    {
      fileLoaded = false;
      try
      {
        if (!skipLoadingProject)
        {
          string contents;
          if (this.m_fileContentsProvider.TryGetFileContents(this.m_requestContext, fullPath.ToString(), out contents))
          {
            if (!string.IsNullOrEmpty(contents))
            {
              fileLoaded = true;
              using (StringReader stringReader = new StringReader(contents))
                return XDocument.Load((TextReader) stringReader);
            }
          }
        }
      }
      catch (Exception ex)
      {
        this.m_requestContext.TraceException(TracePoints.BuildFrameworkDetection.MsBuildTryDetectError, nameof (MsBuildProjectParser), ex);
      }
      return new XDocument();
    }

    private IEnumerable<VsProjectType> GetProjectTypes(
      XDocument projectDocument,
      VsProjectType solutionType,
      IReadOnlyList<string> packageReferences)
    {
      List<VsProjectType> projectTypes = new List<VsProjectType>();
      if (solutionType != null)
        projectTypes.Add(solutionType);
      foreach (Guid projectTypeGuid in this.GetProjectTypeGuids(projectDocument, packageReferences))
      {
        if (solutionType == null || projectTypeGuid != solutionType.Type)
          projectTypes.Add(VsProjectType.FromGuid(projectTypeGuid));
      }
      return (IEnumerable<VsProjectType>) projectTypes;
    }

    private IEnumerable<Guid> GetProjectTypeGuids(
      XDocument projectDocument,
      IReadOnlyList<string> packageReferences)
    {
      List<Guid> projectTypeGuids = new List<Guid>();
      string attributeValue = this.GetAttributeValue(projectDocument, "Project", "Sdk");
      if (!string.IsNullOrEmpty(attributeValue) && string.Equals(attributeValue, "Microsoft.NET.Sdk", StringComparison.OrdinalIgnoreCase))
        projectTypeGuids.Add(VsProjectType.WellKnownTypes.DOTNETCORE);
      else if (!string.IsNullOrEmpty(attributeValue) && string.Equals(attributeValue, "Microsoft.NET.Sdk.Web", StringComparison.OrdinalIgnoreCase))
        projectTypeGuids.Add(VsProjectType.WellKnownTypes.DOTNETCORE_WEB);
      IEnumerable<Guid> projectTypes;
      if (this.TryGetProjectTypes(projectDocument, out projectTypes))
        projectTypeGuids.AddRange(projectTypes);
      IEnumerable<Guid> outputTypes;
      if (this.TryGetOutputTypes(projectDocument, out outputTypes))
        projectTypeGuids.AddRange(outputTypes);
      if (this.HasMatchingPackageReference(packageReferences, "Microsoft.NET.Sdk.Functions"))
        projectTypeGuids.Add(VsProjectType.WellKnownTypes.FUNCTION);
      if (this.HasMatchingPackageReference(packageReferences, "Microsoft.AspNetCore") || this.HasMatchingPackageReference(packageReferences, "Microsoft.AspNetCore.All") || this.HasMatchingPackageReference(packageReferences, "Microsoft.AspNetCore.App"))
        projectTypeGuids.Add(VsProjectType.WellKnownTypes.ASPNET_CORE);
      if (this.HasMatchingPackageReference(packageReferences, "Microsoft.NET.Test.Sdk"))
        projectTypeGuids.Add(VsProjectType.WellKnownTypes.DOTNETCORE_TEST);
      return (IEnumerable<Guid>) projectTypeGuids;
    }

    private bool TryGetProjectTypes(XDocument projectDocument, out IEnumerable<Guid> projectTypes)
    {
      string elementValue = this.GetElementValue(projectDocument, "ProjectTypeGuids");
      if (elementValue != null)
      {
        projectTypes = ((IEnumerable<string>) elementValue.Split(';')).Select<string, Guid>((Func<string, Guid>) (x => new Guid(x)));
        return true;
      }
      projectTypes = Enumerable.Empty<Guid>();
      return false;
    }

    private bool TryGetOutputTypes(XDocument projectDocument, out IEnumerable<Guid> outputTypes)
    {
      string elementValue = this.GetElementValue(projectDocument, "OutputType");
      if (!string.IsNullOrEmpty(elementValue) && elementValue.IndexOf("exe", StringComparison.OrdinalIgnoreCase) >= 0)
      {
        outputTypes = (IEnumerable<Guid>) new Guid[1]
        {
          VsProjectType.WellKnownTypes.EXE
        };
        return true;
      }
      outputTypes = Enumerable.Empty<Guid>();
      return false;
    }

    private string GetWebSiteProjectLocation(string projectLocation, string projectSection)
    {
      if (!Uri.TryCreate(projectLocation, UriKind.Absolute, out Uri _))
        return projectLocation;
      string str1 = (string) null;
      string str2 = (string) null;
      string pattern = "(Debug|Release)\\.AspNetCompiler\\.PhysicalPath\\s*=\\s*\"([^\"]+)\"";
      foreach (Match match in Regex.Matches(projectSection, pattern, RegexOptions.Singleline))
      {
        string str3 = match.Groups[1].Value;
        string str4 = match.Groups[2].Value;
        if (str3.Equals("Debug", StringComparison.OrdinalIgnoreCase))
          str1 = str4;
        else if (str3.Equals("Release", StringComparison.OrdinalIgnoreCase))
          str2 = str4;
      }
      return str2 ?? str1 ?? string.Empty;
    }

    private bool HasMatchingPackageReference(
      IReadOnlyList<string> packageReferences,
      string reference)
    {
      return packageReferences.Any<string>((Func<string, bool>) (r => string.Equals(r, reference, StringComparison.OrdinalIgnoreCase)));
    }

    private IReadOnlyList<string> GetPackageReferences(XDocument projectDocument) => this.GetReferences(projectDocument, "PackageReference");

    private IReadOnlyList<string> GetProjectReferences(XDocument projectDocument) => this.GetReferences(projectDocument, "ProjectReference");

    private IReadOnlyList<string> GetReferences(XDocument projectDocument, string referenceType)
    {
      List<string> references = new List<string>();
      IEnumerable<XElement> elements = this.GetElements(projectDocument, referenceType);
      if (elements != null)
      {
        foreach (XElement element in elements)
        {
          string attributeValue = this.GetAttributeValue(element, "Include");
          if (!string.IsNullOrEmpty(attributeValue))
            references.Add(attributeValue);
        }
      }
      return (IReadOnlyList<string>) references;
    }

    private string GetElementValue(XDocument projectDocument, string xPathFragment) => this.GetElement(projectDocument, xPathFragment)?.Value;

    private string GetAttributeValue(
      XDocument projectDocument,
      string xPathFragment,
      string attributeName)
    {
      return this.GetAttributeValue(this.GetElement(projectDocument, xPathFragment), attributeName);
    }

    private string GetAttributeValue(XElement element, string attributeName) => element?.Attribute(XName.Get(attributeName))?.Value;

    private XElement GetElement(XDocument projectDocument, string xPathFragment)
    {
      IEnumerable<XElement> elements = this.GetElements(projectDocument, xPathFragment);
      return elements == null ? (XElement) null : elements.FirstOrDefault<XElement>();
    }

    private IEnumerable<XElement> GetElements(XDocument projectDocument, string xPathFragment)
    {
      string namespaceName = projectDocument?.Root?.GetDefaultNamespace()?.NamespaceName;
      XmlNamespaceManager resolver = new XmlNamespaceManager((XmlNameTable) new NameTable());
      if (namespaceName != null)
        resolver.AddNamespace("none", namespaceName);
      IEnumerable<XElement> xelements1;
      if (projectDocument == null)
      {
        xelements1 = (IEnumerable<XElement>) null;
      }
      else
      {
        XElement root = projectDocument.Root;
        xelements1 = root != null ? root.XPathSelectElements("//" + this.AddNamespaces(xPathFragment, "none"), (IXmlNamespaceResolver) resolver) : (IEnumerable<XElement>) null;
      }
      IEnumerable<XElement> source = xelements1;
      if (source == null || !source.Any<XElement>())
      {
        IEnumerable<XElement> xelements2;
        if (projectDocument == null)
        {
          xelements2 = (IEnumerable<XElement>) null;
        }
        else
        {
          XElement root = projectDocument.Root;
          xelements2 = root != null ? root.XPathSelectElements("//" + xPathFragment) : (IEnumerable<XElement>) null;
        }
        source = xelements2;
      }
      return source;
    }

    private string AddNamespaces(string xPathFragment, string namespaceToAdd)
    {
      if (string.IsNullOrEmpty(xPathFragment) || string.IsNullOrEmpty(namespaceToAdd))
        return xPathFragment;
      StringBuilder stringBuilder = new StringBuilder(namespaceToAdd + ":");
      for (int index = 0; index < xPathFragment.Length; ++index)
      {
        stringBuilder.Append(xPathFragment[index]);
        if (xPathFragment[index] == '/' && index + 1 < xPathFragment.Length && xPathFragment[index + 1] != '@')
          stringBuilder.Append(namespaceToAdd + ":");
      }
      return stringBuilder.ToString();
    }
  }
}
