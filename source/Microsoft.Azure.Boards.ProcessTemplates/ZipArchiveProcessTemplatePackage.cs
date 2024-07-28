// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ZipArchiveProcessTemplatePackage
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public class ZipArchiveProcessTemplatePackage : IProcessTemplatePackage, IDisposable
  {
    private const string c_ProcessTemplateFileName = "ProcessTemplate.xml";
    internal const string c_vcPluginXml = "<plugin name=\"Microsoft.ProjectCreationWizard.VersionControl\" wizardPage=\"true\" />";
    internal const string c_buildPluginXml = "<plugin name=\"Microsoft.ProjectCreationWizard.Build\" wizardPage=\"false\" />";
    internal const string c_labPluginXml = "<plugin name=\"Microsoft.ProjectCreationWizard.Lab\" wizardPage=\"false\" />";
    internal const string c_vcGroupXml = "<group id=\"VersionControl\" description=\"Creating version control.\" completionMessage=\"Version control task completed.\">\r\n          <dependencies>\r\n            <dependency groupId=\"Classification\" />\r\n            <dependency groupId=\"Groups\" />\r\n            <dependency groupId=\"WorkItemTracking\" />\r\n          </dependencies>\r\n          <taskList filename=\"Version Control\\VersionControl.xml\" />\r\n        </group>";
    internal const string c_buildGroupXml = "<group id=\"Build\" description=\"Build default processes uploading.\" completionMessage=\"Build default processes uploaded.\">\r\n          <dependencies>\r\n            <dependency groupId=\"VersionControl\" />\r\n            <dependency groupId=\"Groups\" />\r\n          </dependencies>\r\n          <taskList filename=\"Build\\Build.xml\" />\r\n        </group>";
    internal const string c_labGroupXml = "<group id=\"Lab\" description=\"Creating Lab.\" completionMessage=\"Lab task completed.\">\r\n          <dependencies>\r\n            <dependency groupId=\"Classification\" />\r\n            <dependency groupId=\"Groups\" />\r\n            <dependency groupId=\"WorkItemTracking\" />\r\n            <dependency groupId=\"Build\" />\r\n          </dependencies>\r\n          <taskList filename=\"Lab\\Lab.xml\" />\r\n        </group>";
    private static readonly IReadOnlyDictionary<string, ZipArchiveProcessTemplatePackage.PluginXml> s_defaultedPlugins = (IReadOnlyDictionary<string, ZipArchiveProcessTemplatePackage.PluginXml>) new Dictionary<string, ZipArchiveProcessTemplatePackage.PluginXml>()
    {
      {
        "Microsoft.ProjectCreationWizard.Groups",
        new ZipArchiveProcessTemplatePackage.PluginXml()
        {
          GroupXml = "<group id=\"Groups\" description=\"Create Groups and assign Permissions.\" completionMessage=\"Groups created and Permissions assigned.\">\r\n      <dependencies>\r\n        <dependency groupId=\"Classification\" />\r\n      </dependencies>\r\n      <taskList filename=\"Groups and Permissions\\GroupsandPermissions.xml\" />\r\n    </group>",
          PluginNameXml = "<plugin name=\"Microsoft.ProjectCreationWizard.Groups\" wizardPage=\"false\" />",
          TaskListFilePath = "Groups and Permissions\\GroupsandPermissions.xml"
        }
      },
      {
        "Microsoft.ProjectCreationWizard.VersionControl",
        new ZipArchiveProcessTemplatePackage.PluginXml()
        {
          GroupXml = "<group id=\"VersionControl\" description=\"Creating version control.\" completionMessage=\"Version control task completed.\">\r\n          <dependencies>\r\n            <dependency groupId=\"Classification\" />\r\n            <dependency groupId=\"Groups\" />\r\n            <dependency groupId=\"WorkItemTracking\" />\r\n          </dependencies>\r\n          <taskList filename=\"Version Control\\VersionControl.xml\" />\r\n        </group>",
          PluginNameXml = "<plugin name=\"Microsoft.ProjectCreationWizard.VersionControl\" wizardPage=\"true\" />",
          TaskListFilePath = "Version Control\\VersionControl.xml"
        }
      },
      {
        "Microsoft.ProjectCreationWizard.Build",
        new ZipArchiveProcessTemplatePackage.PluginXml()
        {
          GroupXml = "<group id=\"Build\" description=\"Build default processes uploading.\" completionMessage=\"Build default processes uploaded.\">\r\n          <dependencies>\r\n            <dependency groupId=\"VersionControl\" />\r\n            <dependency groupId=\"Groups\" />\r\n          </dependencies>\r\n          <taskList filename=\"Build\\Build.xml\" />\r\n        </group>",
          PluginNameXml = "<plugin name=\"Microsoft.ProjectCreationWizard.Build\" wizardPage=\"false\" />",
          TaskListFilePath = "Build\\Build.xml"
        }
      },
      {
        "Microsoft.ProjectCreationWizard.Lab",
        new ZipArchiveProcessTemplatePackage.PluginXml()
        {
          GroupXml = "<group id=\"Lab\" description=\"Creating Lab.\" completionMessage=\"Lab task completed.\">\r\n          <dependencies>\r\n            <dependency groupId=\"Classification\" />\r\n            <dependency groupId=\"Groups\" />\r\n            <dependency groupId=\"WorkItemTracking\" />\r\n            <dependency groupId=\"Build\" />\r\n          </dependencies>\r\n          <taskList filename=\"Lab\\Lab.xml\" />\r\n        </group>",
          PluginNameXml = "<plugin name=\"Microsoft.ProjectCreationWizard.Lab\" wizardPage=\"false\" />",
          TaskListFilePath = "Lab\\Lab.xml"
        }
      }
    };
    private static readonly HashSet<string> s_customizationSupportedPluginsOnPrem = new HashSet<string>()
    {
      "Microsoft.ProjectCreationWizard.Groups",
      "Microsoft.ProjectCreationWizard.VersionControl",
      "Microsoft.ProjectCreationWizard.Build",
      "Microsoft.ProjectCreationWizard.Lab"
    };
    private static readonly HashSet<string> s_notSupportedPluginsHosted = new HashSet<string>()
    {
      "Microsoft.ProjectCreationWizard.Portal"
    };
    private static readonly HashSet<string> s_notSupportedPluginsOnPrem = new HashSet<string>()
    {
      "Microsoft.ProjectCreationWizard.Portal"
    };
    private bool m_loaded;
    private XDocument m_rootDocument;
    private XElement m_metadataElement;
    private string m_name;
    private string m_description;
    private string m_pluginsXmlString;
    private ProcessVersion m_version;
    private Guid m_typeId;
    private ZipArchive m_archive;
    private bool m_disposed;

    public ZipArchiveProcessTemplatePackage(Stream stream, bool defaultEncoding = true)
      : this(stream, false, false, defaultEncoding)
    {
    }

    public ZipArchiveProcessTemplatePackage(Stream stream, bool leaveOpen, bool defaultEncoding = true)
      : this(stream, false, leaveOpen, defaultEncoding)
    {
    }

    public ZipArchiveProcessTemplatePackage(
      Stream stream,
      bool updateable,
      bool leaveOpen,
      bool defaultEncoding = true)
    {
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
      try
      {
        if (defaultEncoding)
          this.m_archive = new ZipArchive(stream, updateable ? ZipArchiveMode.Update : ZipArchiveMode.Read, leaveOpen);
        else
          this.m_archive = new ZipArchive(stream, updateable ? ZipArchiveMode.Update : ZipArchiveMode.Read, leaveOpen, ZipArchiveProcessTemplatePackage.ZipEntryNameEnconding);
      }
      catch (InvalidDataException ex)
      {
        throw new ProcessInvalidPackageFormatException((Exception) ex);
      }
      catch (ArgumentException ex)
      {
        throw new ProcessInvalidCultureException(CultureInfo.CurrentCulture.TextInfo.CultureName, CultureInfo.CurrentCulture.TextInfo.OEMCodePage, (Exception) ex);
      }
    }

    public XDocument RootDocument
    {
      get
      {
        this.EnsureTemplateLoaded();
        return this.m_rootDocument;
      }
    }

    public string Name
    {
      get
      {
        this.EnsureTemplateLoaded();
        return this.m_name;
      }
      set
      {
        this.EnsureTemplateLoaded();
        this.m_name = value;
      }
    }

    public string Description
    {
      get
      {
        this.EnsureTemplateLoaded();
        return this.m_description;
      }
      set
      {
        this.EnsureTemplateLoaded();
        this.m_description = value;
      }
    }

    public ProcessVersion Version
    {
      get
      {
        this.EnsureTemplateLoaded();
        return this.m_version;
      }
      set
      {
        this.EnsureTemplateLoaded();
        this.m_version = value;
      }
    }

    public Guid TypeId
    {
      get
      {
        this.EnsureTemplateLoaded();
        return this.m_typeId;
      }
      set
      {
        this.EnsureTemplateLoaded();
        this.m_typeId = value;
      }
    }

    public string PluginsXmlString
    {
      get
      {
        this.EnsureTemplateLoaded();
        return this.m_pluginsXmlString;
      }
      set
      {
        this.EnsureTemplateLoaded();
        this.m_pluginsXmlString = value;
      }
    }

    public bool TryGetDocument(string documentPath, out XDocument doc) => this.TryGetDocument(documentPath, false, out doc);

    public static Encoding ZipEntryNameEnconding => ZipArchiveProcessTemplatePackage.ComputeZipEntryNameEnconding(Thread.CurrentThread.CurrentCulture.TextInfo.OEMCodePage);

    public static Encoding ComputeZipEntryNameEnconding(int codePage)
    {
      if (codePage == 1 || codePage == 2 || codePage == 3 || codePage == 42)
        codePage = 437;
      try
      {
        return Encoding.GetEncoding(codePage);
      }
      catch (ArgumentException ex)
      {
        return Encoding.GetEncoding(437);
      }
    }

    private bool TryGetDocument(string documentPath, bool throwExceptions, out XDocument doc)
    {
      doc = (XDocument) null;
      ZipArchiveEntry entry;
      if (this.TryGetEntry(documentPath, out entry))
      {
        try
        {
          using (Stream stream = entry.Open())
            doc = XDocument.Load(stream, LoadOptions.SetLineInfo);
          return true;
        }
        catch (XmlException ex)
        {
          if (throwExceptions)
            throw new ProcessTemplateInvalidXmlException(ex.Message, documentPath, ex);
        }
      }
      return false;
    }

    public XDocument GetDocument(string packagePath)
    {
      XDocument doc;
      if (!this.TryGetDocument(packagePath, true, out doc))
        throw new ProcessTemplateFileNotFoundException(packagePath);
      return doc;
    }

    public void UpdateDocument(string packagePath, XDocument document)
    {
      using (Stream stream = this.GetEntry(packagePath).Open())
      {
        stream.SetLength(0L);
        document.Save(stream);
      }
    }

    public void CreateDocument(string packagePath, XDocument document)
    {
      using (Stream stream = this.m_archive.CreateEntry(packagePath).Open())
        document.Save(stream);
    }

    private ZipArchiveEntry GetEntry(string path)
    {
      ZipArchiveEntry entry;
      if (this.TryGetEntry(path, out entry))
        return entry;
      throw new ProcessTemplateFileNotFoundException(path);
    }

    private bool TryGetEntry(string path, out ZipArchiveEntry entry)
    {
      this.CheckDisposed();
      entry = this.m_archive.Entries.FirstOrDefault<ZipArchiveEntry>((Func<ZipArchiveEntry, bool>) (e => e.FullName.Equals(path, StringComparison.OrdinalIgnoreCase) || e.FullName.Equals(path.Replace("\\", "/"), StringComparison.OrdinalIgnoreCase)));
      return entry != null;
    }

    private void CheckDisposed()
    {
      if (this.m_disposed)
        throw new ObjectDisposedException(this.GetType().FullName);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (!this.m_disposed & disposing)
      {
        if (this.m_archive != null)
          this.m_archive.Dispose();
        this.m_archive = (ZipArchive) null;
      }
      this.m_disposed = true;
    }

    private void EnsureTemplateLoaded()
    {
      this.CheckDisposed();
      if (this.m_loaded)
        return;
      this.m_rootDocument = this.GetDocument("ProcessTemplate.xml");
      this.m_metadataElement = this.m_rootDocument.Descendants((XName) "metadata").FirstOrDefault<XElement>();
      if (this.m_metadataElement == null)
        throw new ProcessInvalidMetadataException();
      this.m_name = this.m_metadataElement.Element((XName) "name")?.Value;
      if (string.IsNullOrEmpty(this.m_name))
        throw new ProcessTemplateEmptyNameException();
      this.m_description = this.m_metadataElement.Element((XName) "description")?.Value;
      this.m_version = ProcessVersion.ParseLegacyMetadataXml(this.m_metadataElement, out this.m_typeId);
      this.m_pluginsXmlString = this.m_metadataElement.Element((XName) "plugins")?.ToString();
      this.m_loaded = true;
    }

    public void ReplacePluginsWithDefaults(
      Action<string> whenPluginReplaced = null,
      Action<string> whenPluginAdded = null,
      bool? isHosted = null)
    {
      this.EnsureTemplateLoaded();
      XElement content1 = this.m_metadataElement.Element((XName) "plugins");
      if (content1 == null)
      {
        content1 = new XElement((XName) "plugins");
        this.m_metadataElement.Add((object) content1);
      }
      XElement content2 = this.m_rootDocument.Root.Element((XName) "groups");
      if (content2 == null)
      {
        content2 = new XElement((XName) "groups");
        this.m_rootDocument.Root.Add((object) content2);
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (XContainer element1 in content2.Elements((XName) "group"))
      {
        string packagePath = element1.Element((XName) "taskList")?.Attribute((XName) "filename")?.Value;
        if (!string.IsNullOrEmpty(packagePath))
        {
          foreach (XElement element2 in this.GetDocument(packagePath).Root.Elements((XName) "task"))
          {
            string key = element2.Attribute((XName) "plugin")?.Value;
            if (!string.IsNullOrEmpty(key))
              dictionary[key] = packagePath;
          }
        }
      }
      bool flag = false;
      foreach (KeyValuePair<string, ZipArchiveProcessTemplatePackage.PluginXml> defaultedPlugin in (IEnumerable<KeyValuePair<string, ZipArchiveProcessTemplatePackage.PluginXml>>) ZipArchiveProcessTemplatePackage.s_defaultedPlugins)
      {
        XDocument taskListForPlugin = ZipArchiveProcessTemplatePackage.GetDefaultTaskListForPlugin(defaultedPlugin.Key);
        if (isHosted.GetValueOrDefault(true) || !ZipArchiveProcessTemplatePackage.s_customizationSupportedPluginsOnPrem.Contains(defaultedPlugin.Key) && !ZipArchiveProcessTemplatePackage.s_notSupportedPluginsOnPrem.Contains(defaultedPlugin.Key))
        {
          string packagePath;
          if (dictionary.TryGetValue(defaultedPlugin.Key, out packagePath))
          {
            if (whenPluginReplaced != null)
              whenPluginReplaced(defaultedPlugin.Key);
            this.UpdateDocument(packagePath, taskListForPlugin);
          }
          else
          {
            content1.Add((object) XElement.Parse(defaultedPlugin.Value.PluginNameXml));
            content2.Add((object) XElement.Parse(defaultedPlugin.Value.GroupXml));
            flag = true;
            if (this.TryGetDocument(defaultedPlugin.Value.TaskListFilePath, out XDocument _))
              this.UpdateDocument(defaultedPlugin.Value.TaskListFilePath, taskListForPlugin);
            else
              this.CreateDocument(defaultedPlugin.Value.TaskListFilePath, taskListForPlugin);
            if (whenPluginAdded != null)
              whenPluginAdded(defaultedPlugin.Key);
          }
        }
      }
      if (!flag)
        return;
      this.UpdateDocument("ProcessTemplate.xml", this.m_rootDocument);
    }

    public void RemovePluginsNotSupported(bool isHosted)
    {
      HashSet<string> notSupportedPlugins = isHosted ? ZipArchiveProcessTemplatePackage.s_notSupportedPluginsHosted : ZipArchiveProcessTemplatePackage.s_notSupportedPluginsOnPrem;
      this.RootDocument.Descendants((XName) "plugin").Where<XElement>((Func<XElement, bool>) (plugin => notSupportedPlugins.Contains(plugin.Attribute((XName) "name")?.Value))).Remove<XElement>();
      Dictionary<XElement, string> dictionary = this.RootDocument.Descendants((XName) "groups").Descendants<XElement>((XName) "group").ToDictionary<XElement, XElement, string>((Func<XElement, XElement>) (node => node), (Func<XElement, string>) (node => node.Descendants((XName) "taskList").First<XElement>().Attribute((XName) "filename").Value));
      List<string> toRemoveFolders = new List<string>();
      foreach (XElement key in dictionary.Keys)
      {
        string str = dictionary[key];
        XDocument doc;
        if (this.TryGetDocument(str, out doc) && notSupportedPlugins.Contains(doc.Descendants((XName) "task").First<XElement>().Attribute((XName) "plugin").Value))
        {
          key.Remove();
          toRemoveFolders.Add(Path.GetDirectoryName(str));
        }
      }
      this.m_archive.Entries.Where<ZipArchiveEntry>((Func<ZipArchiveEntry, bool>) (entry => toRemoveFolders.Any<string>((Func<string, bool>) (folder => entry.FullName.StartsWith(folder, StringComparison.CurrentCultureIgnoreCase))))).ToList<ZipArchiveEntry>().ForEach((Action<ZipArchiveEntry>) (e => e.Delete()));
      this.UpdateDocument("ProcessTemplate.xml", this.RootDocument);
    }

    private static XDocument GetDefaultTaskListForPlugin(string plugin)
    {
      string empty = string.Empty;
      string name;
      switch (plugin)
      {
        case "Microsoft.ProjectCreationWizard.Build":
          name = "Build.xml";
          break;
        case "Microsoft.ProjectCreationWizard.VersionControl":
          name = "VersionControl.xml";
          break;
        case "Microsoft.ProjectCreationWizard.Lab":
          name = "Lab.xml";
          break;
        case "Microsoft.ProjectCreationWizard.Groups":
          name = "GroupsAndPermissions.xml";
          break;
        default:
          throw new InvalidOperationException(string.Format("This Plugin {0} was not expected to be defaulted", (object) plugin));
      }
      using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
        return XDocument.Load(manifestResourceStream);
    }

    public static Stream FixupZipSingleRootFolder(Stream content)
    {
      try
      {
        Stream stream1;
        using (ZipArchive zipArchive1 = new ZipArchive(content, ZipArchiveMode.Read, true, ZipArchiveProcessTemplatePackage.ZipEntryNameEnconding))
        {
          List<string> list1 = zipArchive1.Entries.Select<ZipArchiveEntry, string>((Func<ZipArchiveEntry, string>) (e => e.FullName)).ToList<string>();
          List<string> list2 = list1.Select<string, string>((Func<string, string>) (s => ((IEnumerable<string>) s.Split('/', '\\')).First<string>())).Distinct<string>().ToList<string>();
          if (list2.Count == 1)
          {
            string str = list2.Single<string>();
            stream1 = (Stream) new MemoryStream();
            using (ZipArchive zipArchive2 = new ZipArchive(stream1, ZipArchiveMode.Create, true, ZipArchiveProcessTemplatePackage.ZipEntryNameEnconding))
            {
              foreach (string entryName1 in list1)
              {
                string entryName2 = entryName1.Substring(str.Length + 1);
                if (entryName2.Length != 0)
                {
                  ZipArchiveEntry entry1 = zipArchive2.CreateEntry(entryName2);
                  ZipArchiveEntry entry2 = zipArchive1.GetEntry(entryName1);
                  using (Stream destination = entry1.Open())
                  {
                    using (Stream stream2 = entry2.Open())
                      stream2.CopyTo(destination);
                  }
                }
              }
            }
          }
          else
            stream1 = content;
        }
        stream1.Seek(0L, SeekOrigin.Begin);
        return stream1;
      }
      catch (InvalidDataException ex)
      {
        throw new ProcessInvalidPackageFormatException((Exception) ex);
      }
    }

    private class PluginXml
    {
      public string PluginNameXml { get; set; }

      public string GroupXml { get; set; }

      public string TaskListFilePath { get; set; }
    }
  }
}
