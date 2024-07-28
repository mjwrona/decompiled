// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageSerializers
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class CoverageSerializers
  {
    private static XmlSerializer MergeJobDataSerializer = new XmlSerializer(typeof (MergeJobData));
    private static XmlSerializer CoverageMonitorJobDataSerializer = new XmlSerializer(typeof (CoverageMonitorJobData));
    private static XmlSerializer CoverageMergeInvokerJobDataSerializer = new XmlSerializer(typeof (MergeInvokerJobData));
    private static XmlSerializer FileCoverageEvaluationJobDataSerializer = new XmlSerializer(typeof (FileCoverageEvaluationJobData));
    private static XmlSerializer PublishCoveragePRStatusJobDataSerializer = new XmlSerializer(typeof (PublishCoveragePRStatusJobData));
    private static XmlSerializer PipelineScopeLevelCoverageAggregationJobDataSerializer = new XmlSerializer(typeof (PipelineScopeLevelCoverageAggregationJobData));
    private static XmlSerializer PipelineCoverageEvaluationJobDataSerializer = new XmlSerializer(typeof (PipelineCoverageEvaluationJobData));
    private static XmlSerializer FolderViewGeneratorJobDataSerializer = new XmlSerializer(typeof (FolderViewGeneratorJobData));

    public static XmlNode Serialize<T>(
      TestManagementRequestContext tcmRequestContext,
      T jobDataObject)
    {
      ArgumentUtility.CheckGenericForNull((object) jobDataObject, nameof (jobDataObject));
      XmlSerializer xmlSerializer = CoverageSerializers.GetXmlSerializer(typeof (T));
      using (MemoryStream input = new MemoryStream())
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) input, Encoding.UTF8))
        {
          try
          {
            xmlSerializer.Serialize((TextWriter) streamWriter, (object) jobDataObject);
            input.Position = 0L;
            using (XmlReader reader = XmlReader.Create((Stream) input))
            {
              XmlDocument xmlDocument = new XmlDocument();
              xmlDocument.Load(reader);
              return (XmlNode) xmlDocument.DocumentElement;
            }
          }
          catch (InvalidOperationException ex)
          {
            tcmRequestContext.Logger.Error(1015129, string.Format("Type: {0}: Error serializing the data: {1}", (object) jobDataObject.GetType(), (object) ex));
          }
        }
      }
      return (XmlNode) null;
    }

    public static T Deserialize<T>(XmlNode xmlNode)
    {
      T obj = default (T);
      XmlSerializer xmlSerializer = CoverageSerializers.GetXmlSerializer(typeof (T));
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) memoryStream, Encoding.UTF8))
        {
          streamWriter.Write(xmlNode.OuterXml);
          streamWriter.Flush();
          memoryStream.Position = 0L;
          return (T) xmlSerializer.Deserialize((Stream) memoryStream);
        }
      }
    }

    private static XmlSerializer GetXmlSerializer(Type type)
    {
      ArgumentUtility.CheckForNull<Type>(type, nameof (type));
      string name = type.Name;
      if (name != null)
      {
        switch (name.Length)
        {
          case 12:
            if (name == "MergeJobData")
              return CoverageSerializers.MergeJobDataSerializer;
            break;
          case 19:
            if (name == "MergeInvokerJobData")
              return CoverageSerializers.CoverageMergeInvokerJobDataSerializer;
            break;
          case 22:
            if (name == "CoverageMonitorJobData")
              return CoverageSerializers.CoverageMonitorJobDataSerializer;
            break;
          case 26:
            if (name == "FolderViewGeneratorJobData")
              return CoverageSerializers.FolderViewGeneratorJobDataSerializer;
            break;
          case 29:
            if (name == "FileCoverageEvaluationJobData")
              return CoverageSerializers.FileCoverageEvaluationJobDataSerializer;
            break;
          case 30:
            if (name == "PublishCoveragePRStatusJobData")
              return CoverageSerializers.PublishCoveragePRStatusJobDataSerializer;
            break;
          case 33:
            if (name == "PipelineCoverageEvaluationJobData")
              return CoverageSerializers.PipelineCoverageEvaluationJobDataSerializer;
            break;
          case 44:
            if (name == "PipelineScopeLevelCoverageAggregationJobData")
              return CoverageSerializers.PipelineScopeLevelCoverageAggregationJobDataSerializer;
            break;
        }
      }
      throw new ArgumentException(string.Format("Job data object type {0} is not supported", (object) type));
    }
  }
}
