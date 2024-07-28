// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.ProcessPackageFormTransformer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormExtensions;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  public static class ProcessPackageFormTransformer
  {
    public static Stream GetProcessPackageContentWithTransformedWebLayout(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor)
    {
      ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
      try
      {
        using (Stream processPackageContent = service.GetLegacyProcessPackageContent(requestContext, processDescriptor))
        {
          MemoryStream destination = new MemoryStream();
          processPackageContent.CopyTo((Stream) destination);
          processPackageContent.Close();
          using (ZipArchive zipArchive = new ZipArchive((Stream) destination, ZipArchiveMode.Update, true, ZipArchiveProcessTemplatePackage.ZipEntryNameEnconding))
          {
            if (zipArchive.Entries.FirstOrDefault<ZipArchiveEntry>((Func<ZipArchiveEntry, bool>) (entry => StringComparer.OrdinalIgnoreCase.Equals("ProcessTemplate.xml", entry.Name))) == null)
            {
              requestContext.Trace(1300601, TraceLevel.Info, "FormLayout", nameof (ProcessPackageFormTransformer), string.Format("ProcessTemplate.xml not found in process {0} ({1}).", (object) processDescriptor.Name, (object) processDescriptor.TypeId));
            }
            else
            {
              ZipArchiveEntry zipArchiveEntry1 = zipArchive.Entries.FirstOrDefault<ZipArchiveEntry>((Func<ZipArchiveEntry, bool>) (entry => StringComparer.OrdinalIgnoreCase.Equals("WorkItem Tracking\\WorkItems.xml", entry.FullName.Replace('/', '\\'))));
              if (zipArchiveEntry1 == null)
              {
                requestContext.Trace(1300602, TraceLevel.Info, "FormLayout", nameof (ProcessPackageFormTransformer), string.Format("WorkItem Tracking\\WorkItems.xml not found in process {0} ({1}).", (object) processDescriptor.Name, (object) processDescriptor.TypeId));
              }
              else
              {
                string[] array;
                using (Stream stream = zipArchiveEntry1.Open())
                {
                  using (StreamReader streamReader = new StreamReader(stream))
                    array = XDocument.Parse(streamReader.ReadToEnd()).Root.XPathSelectElements("//task[@id=\"WITs\"]/taskXml/WORKITEMTYPES/WORKITEMTYPE").Select<XElement, string>((Func<XElement, string>) (type => type.Attribute((XName) "fileName")?.Value.Replace('/', '\\'))).Where<string>((Func<string, bool>) (fileName => !string.IsNullOrEmpty(fileName))).ToArray<string>();
                }
                IFieldTypeDictionary fieldTypeDictionary = requestContext.WitContext().FieldDictionary;
                FormTransformer formTransformer = new FormTransformer(requestContext.RequestTracer);
                foreach (string str in array)
                {
                  string witTypeFileName = str;
                  ZipArchiveEntry zipArchiveEntry2 = zipArchive.Entries.FirstOrDefault<ZipArchiveEntry>((Func<ZipArchiveEntry, bool>) (entry => StringComparer.OrdinalIgnoreCase.Equals(witTypeFileName, entry.FullName.Replace('/', '\\'))));
                  if (zipArchiveEntry2 == null)
                  {
                    requestContext.Trace(1300603, TraceLevel.Info, "FormLayout", nameof (ProcessPackageFormTransformer), string.Format("{0} not found in process {1} ({2}).", (object) witTypeFileName, (object) processDescriptor.Name, (object) processDescriptor.TypeId));
                  }
                  else
                  {
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.DtdProcessing = DtdProcessing.Prohibit;
                    settings.XmlResolver = (XmlResolver) null;
                    XmlDocument xmlDocument;
                    using (Stream input = zipArchiveEntry2.Open())
                    {
                      using (XmlReader reader = XmlReader.Create(input, settings))
                      {
                        xmlDocument = new XmlDocument();
                        xmlDocument.Load(reader);
                        XmlElement formElement = xmlDocument.SelectSingleNode("//WORKITEMTYPE/FORM") as XmlElement;
                        Lazy<IEnumerable<Contribution>> formContributions = new Lazy<IEnumerable<Contribution>>((Func<IEnumerable<Contribution>>) (() => FormExtensionsUtility.GetFilteredContributions(requestContext)));
                        FieldEntry field;
                        formTransformer.TransformForExport(formElement, formContributions, WellKnownProcessLayout.GetAgileBugLayout(requestContext), (ControlLabelResolver) (controlId => fieldTypeDictionary.TryGetField(controlId, out field) ? field.Name : (string) null), false);
                      }
                    }
                    zipArchiveEntry2.Delete();
                    using (Stream w1 = zipArchive.CreateEntry(witTypeFileName).Open())
                    {
                      using (XmlTextWriter w2 = new XmlTextWriter(w1, Encoding.UTF8))
                      {
                        w2.Formatting = Formatting.Indented;
                        xmlDocument.Save((XmlWriter) w2);
                      }
                    }
                  }
                }
              }
            }
          }
          destination.Seek(0L, SeekOrigin.Begin);
          return (Stream) destination;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1300604, "FormLayout", nameof (ProcessPackageFormTransformer), ex);
        return service.GetLegacyProcessPackageContent(requestContext, processDescriptor);
      }
    }
  }
}
