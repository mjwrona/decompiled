// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.ProcessTemplate
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FileContainer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [RequiredClientService("BuildServer")]
  [ClassVisibility(ClientVisibility.Internal)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class ProcessTemplate : IValidatable
  {
    private Microsoft.TeamFoundation.Build.Server.TeamProject m_teamProjectObj;
    private string m_teamProject;
    private const string SupportedReasonsPropertyName = "SupportedReasons";
    private const string BuildProcessVersionPropertyName = "BuildProcessVersion";
    private readonly string c_headsRefPath = "refs/heads";
    private static readonly char[] c_pathSeparators = new char[2]
    {
      '/',
      '\\'
    };

    public ProcessTemplate()
    {
      this.SupportedReasons = BuildReason.Manual | BuildReason.IndividualCI | BuildReason.BatchedCI | BuildReason.Schedule | BuildReason.ScheduleForced | BuildReason.UserCreated | BuildReason.BuildCompletion;
      this.TemplateType = ProcessTemplateType.Custom;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string TeamProject
    {
      get => this.m_teamProjectObj != null ? this.m_teamProjectObj.Name : this.m_teamProject;
      set => this.m_teamProject = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string ServerPath { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string Description { get; set; }

    [XmlAttribute]
    [DefaultValue(BuildReason.Manual | BuildReason.IndividualCI | BuildReason.BatchedCI | BuildReason.Schedule | BuildReason.ScheduleForced | BuildReason.UserCreated | BuildReason.BuildCompletion)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public BuildReason SupportedReasons { get; set; }

    [XmlAttribute]
    [DefaultValue(ProcessTemplateType.Custom)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public ProcessTemplateType TemplateType { get; set; }

    [XmlAttribute]
    [DefaultValue(-1)]
    [ClientProperty(ClientVisibility.Private)]
    public int Id { get; set; }

    [XmlElement]
    [ClientProperty(ClientVisibility.Private)]
    public string Parameters { get; set; }

    [ClientProperty(ClientVisibility.Private)]
    public bool FileExists { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string Version { get; set; }

    internal Microsoft.TeamFoundation.Build.Server.TeamProject TeamProjectObj
    {
      get => this.m_teamProjectObj;
      set
      {
        this.m_teamProjectObj = value;
        if (this.m_teamProjectObj == null)
          return;
        this.m_teamProject = this.m_teamProjectObj.Name;
      }
    }

    internal ProcessTemplate Clone() => new ProcessTemplate()
    {
      Id = this.Id,
      Version = this.Version,
      FileExists = this.FileExists,
      Parameters = this.Parameters,
      ServerPath = this.ServerPath,
      Description = this.Description,
      TeamProject = this.TeamProject,
      TemplateType = this.TemplateType,
      TeamProjectObj = this.TeamProjectObj,
      SupportedReasons = this.SupportedReasons
    };

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      if (string.IsNullOrEmpty(this.ServerPath))
        throw new ArgumentException(BuildTypeResource.InvalidInputParameterNull((object) "ServerPath"));
      if (VersionControlPath.IsServerItem(this.ServerPath))
        this.ServerPath = VersionControlPath.GetFullPath(this.ServerPath);
      this.m_teamProjectObj = requestContext.GetService<IProjectService>().GetTeamProjectFromGuidOrName(requestContext, this.m_teamProject);
    }

    public static void UpdateCachedProcessParametersForAllTemplates(
      IVssRequestContext requestContext,
      FileContainerItem containerItem)
    {
      List<string> paths = new List<string>()
      {
        BuildContainerPath.Combine(containerItem.ContainerId.ToString((IFormatProvider) CultureInfo.InvariantCulture), containerItem.Path)
      };
      TeamFoundationBuildService service = requestContext.GetService<TeamFoundationBuildService>();
      List<ProcessTemplate> processTemplateList = service.QueryProcessTemplatesByPath(requestContext, (string) null, (IList<string>) paths, true);
      if (processTemplateList.Count <= 0)
        return;
      List<ProcessTemplate> processTemplates1 = new List<ProcessTemplate>();
      List<ProcessTemplate> processTemplates2 = new List<ProcessTemplate>();
      foreach (ProcessTemplate processTemplate in processTemplateList)
      {
        if (processTemplate.FileExists)
          processTemplates2.Add(processTemplate);
        else
          processTemplates1.Add(processTemplate);
      }
      if (processTemplates2.Count > 0)
        service.UpdateProcessTemplates(requestContext, (IList<ProcessTemplate>) processTemplates2);
      if (processTemplates1.Count <= 0)
        return;
      service.AddProcessTemplates(requestContext, (IList<ProcessTemplate>) processTemplates1);
    }

    internal void UpdateCachedProcessParameters(
      IVssRequestContext requestContext,
      VersionSpec versionSpec)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (UpdateCachedProcessParameters));
      string tempFileName = Path.GetTempFileName();
      try
      {
        string processWorkflow = string.Empty;
        if (VersionControlPath.IsServerItem(this.ServerPath))
        {
          requestContext.GetService<TeamFoundationVersionControlService>().DownloadFile(requestContext, this.ServerPath, 0, versionSpec, tempFileName);
          processWorkflow = File.ReadAllText(tempFileName);
        }
        else
        {
          string projectName;
          string repositoryName;
          string branchAndPath;
          if (BuildSourceProviders.GitProperties.ParseGitPath(this.ServerPath, out projectName, out repositoryName, out branchAndPath))
          {
            processWorkflow = this.GetGitTemplate(requestContext, projectName, repositoryName, branchAndPath, this.ServerPath);
          }
          else
          {
            using (Stream stream = this.DownloadFileFromFileContainerService(requestContext, this.ServerPath))
            {
              using (StreamReader streamReader = new StreamReader(stream))
                processWorkflow = streamReader.ReadToEnd();
            }
          }
        }
        IDictionary<string, string> parameters;
        this.Parameters = ProcessParameterHelper.ExtractProcessParameters(processWorkflow, out parameters);
        string str1;
        if (parameters.TryGetValue("SupportedReasons", out str1))
        {
          try
          {
            this.SupportedReasons = (BuildReason) Enum.Parse(typeof (BuildReason), str1);
            requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Parsed supported reasons: {0}", (object) this.SupportedReasons);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(0, "Build", "Service", ex);
          }
        }
        else
          requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Unable to find value for property '{0}'", (object) "SupportedReasons");
        string str2;
        if (parameters.TryGetValue("BuildProcessVersion", out str2))
        {
          if (!string.IsNullOrEmpty(str2))
          {
            try
            {
              System.Version version = (System.Version) null;
              if (!str2.Contains("."))
              {
                this.Version = new System.Version(int.Parse(str2, (IFormatProvider) CultureInfo.InvariantCulture), 0).ToString();
              }
              else
              {
                version = new System.Version(str2);
                this.Version = str2;
              }
              requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Parsed template version: {0}", (object) this.Version);
              goto label_26;
            }
            catch (Exception ex)
            {
              requestContext.TraceException(0, "Build", "Service", ex);
              goto label_26;
            }
          }
        }
        this.Version = (string) null;
        requestContext.Trace(0, TraceLevel.Warning, "Build", "Service", "Unable to find value for property '{0}'", (object) "BuildProcessVersion");
label_26:
        this.FileExists = true;
      }
      catch (ServerException ex)
      {
        requestContext.TraceException(0, "Build", "Service", (Exception) ex);
        this.FileExists = false;
      }
      catch (InvalidGitUriException ex)
      {
        requestContext.TraceException(0, "Build", "Service", (Exception) ex);
        this.FileExists = false;
      }
      finally
      {
        FileSpec.DeleteFile(tempFileName);
      }
      requestContext.TraceLeave(0, "Build", "Service", nameof (UpdateCachedProcessParameters));
    }

    private string GetGitTemplate(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string branchAndPath,
      string fullPath)
    {
      using (ITfsGitRepository repositoryByName = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryByName(requestContext, projectName, repositoryName))
      {
        TfsGitRef tfsGitRef = repositoryByName.Refs.MatchingNames((IEnumerable<string>) new string[1]
        {
          "refs/heads/"
        }, GitRefSearchType.StartsWith).FirstOrDefault<TfsGitRef>((Func<TfsGitRef, bool>) (x => branchAndPath.StartsWith(x.Name.Substring(this.c_headsRefPath.Length + 1), StringComparison.OrdinalIgnoreCase)));
        if (tfsGitRef == null)
          throw new InvalidGitUriException(fullPath);
        string path = branchAndPath.Substring(tfsGitRef.Name.Length - this.c_headsRefPath.Length);
        TfsGitObject tfsGitObject = repositoryByName.LookupObject(tfsGitRef.ObjectId);
        TfsGitTree tree = tfsGitObject != null && tfsGitObject.ObjectType == GitObjectType.Commit ? ((TfsGitCommit) tfsGitObject).GetTree() : throw new InvalidGitUriException(fullPath);
        TfsGitObject member = this.GitFindMember(requestContext, tree, ref path, out TfsGitTreeEntry _);
        if (member == null || member.ObjectType != GitObjectType.Blob)
          throw new InvalidGitUriException(fullPath);
        using (Stream content = member.GetContent())
        {
          using (StreamReader streamReader = new StreamReader(content))
            return streamReader.ReadToEnd();
        }
      }
    }

    private Stream DownloadFileFromFileContainerService(
      IVssRequestContext requestContext,
      string serverPath)
    {
      TeamFoundationFileContainerService service1 = requestContext.GetService<TeamFoundationFileContainerService>();
      TeamFoundationFileService service2 = requestContext.GetService<TeamFoundationFileService>();
      long containerId1 = long.MinValue;
      string itemPath;
      BuildContainerPath.GetContainerIdAndPath(serverPath, out containerId1, out itemPath);
      IVssRequestContext requestContext1 = requestContext;
      long containerId2 = containerId1;
      string path = itemPath;
      Guid empty = Guid.Empty;
      List<FileContainerItem> fileContainerItemList = service1.QueryItems(requestContext1, containerId2, path, empty, false, false);
      CompressionType compressionType = CompressionType.None;
      long contentLength = long.MinValue;
      return service2.RetrieveFile(requestContext, (long) fileContainerItemList[0].FileId, false, out byte[] _, out contentLength, out compressionType);
    }

    internal static void ApplyUpgradeProcessTemplate(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      string configurationFolderPath)
    {
      ProcessTemplate.ApplyUpgradeProcessTemplate(requestContext, definition.TeamProject.Name, configurationFolderPath, (Action<ProcessTemplate>) (x => definition.Process = x), (Action<string>) (x => definition.ProcessParameters = x));
    }

    internal static void ApplyUpgradeProcessTemplate(
      IVssRequestContext requestContext,
      string teamProject,
      string configurationFolderPath,
      Action<ProcessTemplate> setProcess,
      Action<string> setProcessParameters)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (ApplyUpgradeProcessTemplate));
      List<ProcessTemplate> processTemplateList = requestContext.GetService<TeamFoundationBuildService>().QueryProcessTemplates(requestContext, teamProject, (IList<ProcessTemplateType>) new ProcessTemplateType[1]
      {
        ProcessTemplateType.Upgrade
      });
      if (processTemplateList.Count > 0)
      {
        setProcess(processTemplateList[0]);
        if (string.IsNullOrEmpty(processTemplateList[0].Parameters))
        {
          requestContext.Trace(0, TraceLevel.Verbose, "Build", "Service", "Empty parameters for process template '{0}' of team project '{1}'", (object) processTemplateList[0].Id, (object) teamProject);
          setProcessParameters((string) null);
        }
        else
        {
          try
          {
            setProcessParameters(ProcessParameterHelper.CreateProcessParameters((IDictionary<string, string>) new Dictionary<string, string>()
            {
              {
                "ConfigurationFolderPath",
                configurationFolderPath
              }
            }));
            requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Set process parameters for process template '{0}' of team project '{1}'", (object) processTemplateList[0].Id, (object) teamProject);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(0, "Build", "Service", ex);
            setProcessParameters((string) null);
          }
        }
      }
      else
        requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "No upgrade process templates found for team project '{0}'", (object) teamProject);
      requestContext.TraceLeave(0, "Build", "Service", nameof (ApplyUpgradeProcessTemplate));
    }

    public static string GetTemplate_AzureContinuousDeployment11() => ProcessTemplate.GetProcessTemplateFromResources("AzureContinuousDeployment.11.xaml");

    public static string GetTemplate_DefaultTemplate11() => ProcessTemplate.GetProcessTemplateFromResources("DefaultTemplate.11.1.xaml");

    public static string GetTemplate_GitContinuousDeploymentTemplate12() => ProcessTemplate.GetProcessTemplateFromResources("GitContinuousDeploymentTemplate.12.xaml");

    public static string GetTemplate_GitTemplate12() => ProcessTemplate.GetProcessTemplateFromResources("GitTemplate.12.xaml");

    public static string GetTemplate_GitTemplate11() => ProcessTemplate.GetProcessTemplateFromResources("GitTemplate.xaml");

    public static string GetTemplate_GitUpgradeTemplate() => ProcessTemplate.GetProcessTemplateFromResources("GitUpgradeTemplate.xaml");

    public static string GetTemplate_TfvcContinuousDeploymentTemplate12() => ProcessTemplate.GetProcessTemplateFromResources("TfvcContinuousDeploymentTemplate.12.xaml");

    public static string GetTemplate_TfvcTemplate12() => ProcessTemplate.GetProcessTemplateFromResources("TfvcTemplate.12.xaml");

    public static string GetTemplate_UpgradeTemplate() => ProcessTemplate.GetProcessTemplateFromResources("UpgradeTemplate.xaml");

    public static string GetProcessTemplateFromResources(string processTemplateName)
    {
      string processTemplateName1 = "Microsoft.TeamFoundation.Build.Server.Resources." + processTemplateName;
      string templateFromResources = ProcessTemplate.GetProcessTemplateFromResources(Assembly.GetExecutingAssembly(), processTemplateName1);
      return !string.IsNullOrEmpty(templateFromResources) ? templateFromResources : throw new ArgumentException(ResourceStrings.UnknownTemplate((object) templateFromResources));
    }

    public static string GetProcessTemplateFromResources(
      Assembly assembly,
      string processTemplateName)
    {
      for (CultureInfo culture = CultureInfo.CurrentUICulture; culture != null; culture = culture.Parent)
      {
        if (culture != CultureInfo.InvariantCulture)
        {
          try
          {
            string templateFromAssembly = ProcessTemplate.GetProcessTemplateFromAssembly(assembly.GetSatelliteAssembly(culture), processTemplateName);
            if (templateFromAssembly != null)
              return templateFromAssembly;
          }
          catch (FileNotFoundException ex)
          {
          }
          catch (FileLoadException ex)
          {
          }
          catch (BadImageFormatException ex)
          {
          }
        }
        else
          break;
      }
      return ProcessTemplate.GetProcessTemplateFromAssembly(assembly, processTemplateName);
    }

    private static string GetProcessTemplateFromAssembly(
      Assembly assembly,
      string processTemplateName)
    {
      try
      {
        using (Stream manifestResourceStream = assembly.GetManifestResourceStream(processTemplateName))
        {
          if (manifestResourceStream != null)
            return ProcessTemplate.GetProcessTemplateFromStream(manifestResourceStream);
        }
      }
      catch (FileNotFoundException ex)
      {
      }
      catch (FileLoadException ex)
      {
      }
      catch (BadImageFormatException ex)
      {
      }
      catch (XmlException ex)
      {
      }
      catch (XmlSchemaValidationException ex)
      {
      }
      return (string) null;
    }

    private static string GetProcessTemplateFromStream(Stream stream)
    {
      XmlDocument xmlDocument = new XmlDocument();
      Encoding utF8 = Encoding.UTF8;
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      };
      using (StreamReader input = new StreamReader(stream, true))
      {
        using (XmlReader reader = XmlReader.Create((TextReader) input, settings))
        {
          Encoding currentEncoding = input.CurrentEncoding;
          xmlDocument.Load(reader);
        }
      }
      ProcessTemplate.ClearLocTagsFromXml((XmlNode) xmlDocument);
      StringBuilder stringBuilder = new StringBuilder();
      StringBuilder output = stringBuilder;
      using (XmlWriter w = XmlWriter.Create(output, new XmlWriterSettings()
      {
        OmitXmlDeclaration = true,
        Indent = true,
        IndentChars = "  "
      }))
        xmlDocument.Save(w);
      return stringBuilder.ToString();
    }

    private static void ClearLocTagsFromXml(XmlNode xmlNode)
    {
      foreach (XmlNode childNode in xmlNode.ChildNodes)
      {
        if (childNode.NodeType == XmlNodeType.Element)
        {
          if (childNode.Attributes["_locID"] != null)
            childNode.Attributes.RemoveNamedItem("_locID");
          if (childNode.Attributes["_locAttrData"] != null)
            childNode.Attributes.RemoveNamedItem("_locAttrData");
        }
        ProcessTemplate.ClearLocTagsFromXml(childNode);
      }
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[ProcessTemplate Id={0} ServerPath={1} TeamProject={2} TemplateType={3}]", (object) this.Id, (object) this.ServerPath, (object) this.TeamProject, (object) this.TemplateType);

    private TfsGitObject GitFindMember(
      IVssRequestContext requestContext,
      TfsGitTree tree,
      ref string path,
      out TfsGitTreeEntry treeEntry)
    {
      ArgumentUtility.CheckForNull<string>(path, nameof (path));
      string str = (string) null;
      string path1 = (string) null;
      TfsGitObject tree1 = (TfsGitObject) null;
      treeEntry = (TfsGitTreeEntry) null;
      int num;
      do
      {
        num = path.IndexOfAny(ProcessTemplate.c_pathSeparators);
        if (num == 0)
          path = path.Substring(1);
        else if (num < 0)
          str = path;
        else if (num == path.Length - 1)
        {
          path = path.Substring(0, path.Length - 1);
          num = 0;
        }
        else
        {
          str = path.Substring(0, num);
          path1 = path.Substring(num);
        }
      }
      while (num == 0);
      List<TfsGitTreeEntry> tfsGitTreeEntryList = new List<TfsGitTreeEntry>();
      foreach (TfsGitTreeEntry treeEntry1 in tree.GetTreeEntries())
      {
        if (treeEntry1.Name.Equals(str, StringComparison.Ordinal))
        {
          if (treeEntry1.ObjectType != GitObjectType.Commit)
            tree1 = treeEntry1.Object;
          treeEntry = treeEntry1;
          break;
        }
        if (treeEntry1.Name.Equals(str, StringComparison.CurrentCultureIgnoreCase))
          tfsGitTreeEntryList.Add(treeEntry1);
      }
      if (treeEntry == null && tfsGitTreeEntryList.Count == 1)
      {
        TfsGitTreeEntry tfsGitTreeEntry = tfsGitTreeEntryList[0];
        if (tfsGitTreeEntry.ObjectType == GitObjectType.Commit)
        {
          path = tfsGitTreeEntry.Name;
        }
        else
        {
          tree1 = tfsGitTreeEntry.Object;
          str = tfsGitTreeEntry.Name;
        }
        treeEntry = tfsGitTreeEntry;
      }
      if (tree1 != null)
      {
        if (path1 == null)
        {
          path = str;
          return tree1;
        }
        if (tree1 is TfsGitTree && path1 != null)
        {
          TfsGitObject member = this.GitFindMember(requestContext, (TfsGitTree) tree1, ref path1, out treeEntry);
          path = str + "/" + path1;
          return member;
        }
      }
      return (TfsGitObject) null;
    }
  }
}
