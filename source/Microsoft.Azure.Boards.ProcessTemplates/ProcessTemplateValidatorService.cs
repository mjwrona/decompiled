// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessTemplateValidatorService
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public class ProcessTemplateValidatorService : 
    IProcessTemplateValidatorService,
    IVssFrameworkService
  {
    private IDisposableReadOnlyList<IProcessTemplatePlugin> m_processTemplatePluginExtensions;
    internal const string c_ProcessTemplateXmlFile = "ProcessTemplate.xml";
    private static readonly string[] s_requiredPlugins = new string[4]
    {
      "Microsoft.ProjectCreationWizard.Classification",
      "Microsoft.ProjectCreationWizard.Groups",
      "Microsoft.ProjectCreationWizard.WorkItemTracking",
      "Microsoft.ProjectCreationWizard.TestManagement"
    };
    private static readonly string[] s_requiredPluginsOnPrem = new string[3]
    {
      "Microsoft.ProjectCreationWizard.Classification",
      "Microsoft.ProjectCreationWizard.Groups",
      "Microsoft.ProjectCreationWizard.WorkItemTracking"
    };
    private static readonly HashSet<string> s_vitalPlugins = new HashSet<string>()
    {
      "Microsoft.ProjectCreationWizard.Classification",
      "Microsoft.ProjectCreationWizard.Groups"
    };

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => this.m_processTemplatePluginExtensions = requestContext.GetExtensions<IProcessTemplatePlugin>();

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
      if (this.m_processTemplatePluginExtensions == null)
        return;
      this.m_processTemplatePluginExtensions.Dispose();
      this.m_processTemplatePluginExtensions = (IDisposableReadOnlyList<IProcessTemplatePlugin>) null;
    }

    public void ValidateTemplateFileSizeLimit(
      IVssRequestContext requestContext,
      Stream zipContentStream)
    {
      long maxSize = requestContext.GetService<IVssRegistryService>().GetValue<long>(requestContext, (RegistryQuery) "/Service/Process/Settings/MaxProcessTemplateFileSize", 20000000L);
      if (zipContentStream.Length > maxSize)
        throw new ProcessPackageFileTooLargeException(zipContentStream.Length, maxSize);
      try
      {
        long uploadSize = new ZipArchive(zipContentStream, ZipArchiveMode.Read, true, ZipArchiveProcessTemplatePackage.ZipEntryNameEnconding).Entries.Sum<ZipArchiveEntry>((Func<ZipArchiveEntry, long>) (e => e.Length));
        if (uploadSize > maxSize)
          throw new ProcessPackageFileTooLargeException(uploadSize, maxSize);
      }
      catch (InvalidDataException ex)
      {
        throw new ProcessInvalidPackageFormatException((Exception) ex);
      }
    }

    public ProcessTemplateValidatorResult Validate(
      IVssRequestContext requestContext,
      Stream zipTemplateContent,
      bool isTfsMigratorValidationContext = false,
      Dictionary<string, object> optionalParameters = null)
    {
      ProcessTemplateValidatorResult results = new ProcessTemplateValidatorResult();
      try
      {
        using (ZipArchiveProcessTemplatePackage processTemplatePackage = new ZipArchiveProcessTemplatePackage(zipTemplateContent, true, true))
        {
          XDocument document;
          this.LoadFile((IProcessTemplatePackage) processTemplatePackage, "ProcessTemplate.xml", out document);
          this.ValidateEmbeddedImages(requestContext, document);
          this.ValidateMethodologyFileSchema(document);
          ZipArchiveProcessTemplatePackage package = processTemplatePackage;
          XDocument processTemplateDoc = document;
          TeamFoundationExecutionEnvironment executionEnvironment = requestContext.ExecutionEnvironment;
          int num1 = executionEnvironment.IsHostedDeployment ? 1 : 0;
          OrderedDictionary orderedDictionary = this.OrderedPlugins(this.ValidateAndLoadTasks((IProcessTemplatePackage) package, processTemplateDoc, num1 != 0));
          Dictionary<string, object> templateContext = new Dictionary<string, object>();
          executionEnvironment = requestContext.ExecutionEnvironment;
          if (!executionEnvironment.IsHostedDeployment)
          {
            ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
            Guid result;
            if (!Guid.TryParse(document.Descendants((XName) "metadata").Descendants<XElement>((XName) "version").First<XElement>().Attribute((XName) "type").Value, out result))
              throw new ProcessTemplateTypeInvalidException("ProcessTemplate.xml");
            IVssRequestContext requestContext1 = requestContext;
            Guid processSpecificId = result;
            ProcessDescriptor processDescriptor;
            ref ProcessDescriptor local = ref processDescriptor;
            service.TryGetSpecificProcessDescriptor(requestContext1, processSpecificId, out local);
            templateContext[TemplateContextKeys.IsOverridingExistingTemplate] = (object) (processDescriptor != null);
          }
          foreach (DictionaryEntry dictionaryEntry in orderedDictionary)
          {
            ProcessTemplateValidatorService.PluginData pluginData = dictionaryEntry.Value as ProcessTemplateValidatorService.PluginData;
            ProcessTemplateValidatorResult pluginResults = pluginData.Plugin.Validate(requestContext, (IDictionary<string, object>) templateContext, processTemplatePackage.TypeId, (IProcessTemplatePackage) processTemplatePackage, pluginData.TaskListPath, pluginData.TaskListXDocument, isTfsMigratorValidationContext, optionalParameters);
            this.AddResults(results, pluginResults);
            if (ProcessTemplateValidatorService.s_vitalPlugins.Contains<string>(dictionaryEntry.Key as string, (IEqualityComparer<string>) TFStringComparer.ProcessTemplatePluginName) && pluginResults != null)
            {
              int? count = pluginResults.Errors?.Count;
              int num2 = 0;
              if (count.GetValueOrDefault() > num2 & count.HasValue)
                break;
            }
          }
        }
        zipTemplateContent.Seek(0L, SeekOrigin.Begin);
        ZipArchive zipArchive = new ZipArchive(zipTemplateContent, ZipArchiveMode.Read, true, ZipArchiveProcessTemplatePackage.ZipEntryNameEnconding);
        HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.FilePath);
        foreach (ZipArchiveEntry entry in zipArchive.Entries)
        {
          string str = Path.GetFullPath(entry.FullName).ToString();
          if (stringSet.Contains(str))
            results.Errors.Add(new ProcessTemplateValidatorMessage()
            {
              File = entry.FullName,
              LineNumber = new int?(0),
              Message = Resources.DuplicateFile((object) entry.FullName)
            });
          else
            stringSet.Add(str);
        }
      }
      catch (ProcessTemplateValidatorException ex)
      {
        results.Errors.Add(new ProcessTemplateValidatorMessage()
        {
          Message = ex.Message ?? ex.ToString(),
          File = ex.TemplateFile,
          LineNumber = new int?(0)
        });
      }
      catch (Exception ex)
      {
        results.Errors.Add(new ProcessTemplateValidatorMessage()
        {
          Message = ex.ToString(),
          LineNumber = new int?(0)
        });
      }
      finally
      {
        zipTemplateContent.Seek(0L, SeekOrigin.Begin);
      }
      return results;
    }

    private OrderedDictionary OrderedPlugins(
      IReadOnlyDictionary<string, ProcessTemplateValidatorService.PluginData> plugins)
    {
      OrderedDictionary orderedDictionary = new OrderedDictionary();
      foreach (KeyValuePair<string, ProcessTemplateValidatorService.PluginData> plugin in (IEnumerable<KeyValuePair<string, ProcessTemplateValidatorService.PluginData>>) plugins)
      {
        if (TFStringComparer.ProcessTemplatePluginName.Equals(plugin.Key, "Microsoft.ProjectCreationWizard.Classification"))
          orderedDictionary.Insert(0, (object) plugin.Key, (object) plugin.Value);
        else
          orderedDictionary.Add((object) plugin.Key, (object) plugin.Value);
      }
      return orderedDictionary;
    }

    private void AddResults(
      ProcessTemplateValidatorResult results,
      ProcessTemplateValidatorResult pluginResults)
    {
      if (pluginResults == null)
        return;
      if (pluginResults.Errors != null)
        (results.Errors as List<ProcessTemplateValidatorMessage>).AddRange((IEnumerable<ProcessTemplateValidatorMessage>) pluginResults.Errors);
      if (pluginResults.ConfirmationsNeeded != null)
        (results.ConfirmationsNeeded as List<ProcessTemplateValidatorMessage>).AddRange((IEnumerable<ProcessTemplateValidatorMessage>) pluginResults.ConfirmationsNeeded);
      results.AddDetails((IEnumerable<object>) pluginResults.Details);
    }

    private IReadOnlyDictionary<string, ProcessTemplateValidatorService.PluginData> ValidateAndLoadTasks(
      IProcessTemplatePackage package,
      XDocument processTemplateDoc,
      bool isHosted)
    {
      Dictionary<string, ProcessTemplateValidatorService.PluginData> dictionary = new Dictionary<string, ProcessTemplateValidatorService.PluginData>();
      IEnumerable<XElement> xelements = processTemplateDoc.Descendants((XName) "group");
      string[] array = processTemplateDoc.Descendants((XName) "plugin").Attributes((XName) "name").Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value)).ToArray<string>();
      foreach (XElement xelement in xelements)
      {
        XName name = (XName) "taskList";
        string str1 = xelement.Element(name).Attribute((XName) "filename").Value;
        XDocument document;
        this.LoadFile(package, str1, out document);
        XmlSchemaSet schemaSet = ProcessTemplateSchema.GetSchemaSet(ProcessTemplateSchema.Schema.Tasks);
        string message = (string) null;
        document.Validate(schemaSet, (ValidationEventHandler) ((o, e) => message = e.Message));
        if (!string.IsNullOrEmpty(message))
          throw new ProcessTemplateSchemaValidationException(message, str1);
        foreach (XElement descendant in document.Descendants((XName) "task"))
        {
          string str2 = descendant.Attribute((XName) "plugin").Value;
          IProcessTemplatePlugin matchingPlugin = this.GetMatchingPlugin(str2);
          if (matchingPlugin == null)
            throw new ProcessTemplatePluginNotFoundException(str2, str1);
          ProcessTemplateValidatorService.PluginData pluginData;
          if (dictionary.TryGetValue(str2, out pluginData) && !VssStringComparer.FilePath.Equals(str1, pluginData.TaskListPath))
            throw new ProcessTemplateValidatorMultipleTaskListException(str2, pluginData.TaskListPath, str1);
          dictionary[str2] = new ProcessTemplateValidatorService.PluginData()
          {
            Plugin = matchingPlugin,
            TaskListPath = str1,
            TaskListXDocument = document
          };
        }
      }
      this.ValidatePlugins((IEnumerable<string>) array, (IEnumerable<string>) dictionary.Keys, isHosted);
      return (IReadOnlyDictionary<string, ProcessTemplateValidatorService.PluginData>) dictionary;
    }

    private IProcessTemplatePlugin GetMatchingPlugin(string pluginName)
    {
      if (this.m_processTemplatePluginExtensions != null && this.m_processTemplatePluginExtensions.Any<IProcessTemplatePlugin>())
      {
        foreach (IProcessTemplatePlugin templatePluginExtension in (IEnumerable<IProcessTemplatePlugin>) this.m_processTemplatePluginExtensions)
        {
          if (templatePluginExtension.IsMatch(pluginName))
            return templatePluginExtension;
        }
      }
      return (IProcessTemplatePlugin) null;
    }

    private void ValidateMethodologyFileSchema(XDocument processTemplateDoc)
    {
      XmlSchemaSet schemaSet = ProcessTemplateSchema.GetSchemaSet(ProcessTemplateSchema.Schema.Methodology);
      string message = (string) null;
      processTemplateDoc.Validate(schemaSet, (ValidationEventHandler) ((o, e) => message = e.Message));
      if (!string.IsNullOrEmpty(message))
        throw new ProcessTemplateSchemaValidationException(message, "ProcessTemplate.xml");
    }

    private void ValidateEmbeddedImages(
      IVssRequestContext requestContext,
      XDocument processTemplateDoc)
    {
      foreach (string document in processTemplateDoc.Descendants().SelectMany<XElement, XAttribute>((Func<XElement, IEnumerable<XAttribute>>) (d => d.Attributes())).Where<XAttribute>((Func<XAttribute, bool>) (a => string.Equals("value", a.Name.LocalName, StringComparison.OrdinalIgnoreCase))).Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value)).ToList<string>())
      {
        if (ContentValidationUtil.ExtractBase64DataUriEmbeddedContent(document).Any<DataUriEmbeddedContent>())
          throw new ProcessTemplateEmbeddedImageValidationException("ProcessTemplate.xml");
      }
    }

    private void LoadFile(IProcessTemplatePackage package, string path, out XDocument document) => document = package.GetDocument(path);

    private void ValidatePlugins(
      IEnumerable<string> plugins,
      IEnumerable<string> pluginsFromTasks,
      bool isHosted)
    {
      foreach (string pluginsFromTask in pluginsFromTasks)
      {
        if (!plugins.Contains<string>(pluginsFromTask, (IEqualityComparer<string>) TFStringComparer.ProcessTemplatePluginName))
          throw new ProcessTemplateValidatorInvalidPluginException(pluginsFromTask, "ProcessTemplate.xml");
      }
      foreach (string plugin in plugins)
      {
        if (!pluginsFromTasks.Contains<string>(plugin, (IEqualityComparer<string>) TFStringComparer.ProcessTemplatePluginName))
          throw new ProcessTemplateValidatorInvalidPluginException(plugin, "ProcessTemplate.xml");
      }
      foreach (string pluginName in isHosted ? ProcessTemplateValidatorService.s_requiredPlugins : ProcessTemplateValidatorService.s_requiredPluginsOnPrem)
      {
        if (!pluginsFromTasks.Contains<string>(pluginName, (IEqualityComparer<string>) TFStringComparer.ProcessTemplatePluginName))
          throw new ProcessTemplateValidatorRequiredPluginMissingException(pluginName, "ProcessTemplate.xml");
      }
    }

    private class PluginData
    {
      public IProcessTemplatePlugin Plugin { get; set; }

      public string TaskListPath { get; set; }

      public XDocument TaskListXDocument { get; set; }
    }

    internal static class PluginNames
    {
      public const string Classification = "Microsoft.ProjectCreationWizard.Classification";
      public const string Groups = "Microsoft.ProjectCreationWizard.Groups";
      public const string WorkItemTracking = "Microsoft.ProjectCreationWizard.WorkItemTracking";
      public const string VersionControl = "Microsoft.ProjectCreationWizard.VersionControl";
      public const string Build = "Microsoft.ProjectCreationWizard.Build";
      public const string Lab = "Microsoft.ProjectCreationWizard.Lab";
      public const string TestManagement = "Microsoft.ProjectCreationWizard.TestManagement";
      public const string Reporting = "Microsoft.ProjectCreationWizard.Reporting";
      public const string Portal = "Microsoft.ProjectCreationWizard.Portal";
    }
  }
}
