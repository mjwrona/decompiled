// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility.AgileUtils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility
{
  internal static class AgileUtils
  {
    internal static T Deserialize<T>(TextReader sourceReader, bool validate, string schema)
    {
      XmlReader xmlReader;
      if (validate && schema != null)
      {
        XmlSchema schema1;
        using (TextReader input = (TextReader) new StringReader(schema))
        {
          XmlReaderSettings settings = new XmlReaderSettings()
          {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = (XmlResolver) null
          };
          XmlReader reader;
          using (reader = XmlReader.Create(input, settings))
            schema1 = XmlSchema.Read(reader, (ValidationEventHandler) null);
        }
        XmlReaderSettings settings1 = new XmlReaderSettings()
        {
          ValidationType = ValidationType.Schema,
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        };
        settings1.Schemas.Add(schema1);
        xmlReader = XmlReader.Create(sourceReader, settings1);
      }
      else
      {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        };
        xmlReader = XmlReader.Create(sourceReader, settings);
      }
      return AgileUtils.Deserialize<T>(xmlReader);
    }

    internal static T Deserialize<T>(XmlReader xmlReader) => (T) new XmlSerializer(typeof (T)).Deserialize(xmlReader);

    internal static string Serialize<T>(T obj)
    {
      using (StringWriter output = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        new XmlSerializer(typeof (T)).Serialize(XmlWriter.Create((TextWriter) output), (object) obj);
        return output.ToString();
      }
    }

    internal static bool TryExecute<TResult>(
      Func<TResult> function,
      string tfsTraceLayer,
      out TResult output)
    {
      try
      {
        output = function();
        return true;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", tfsTraceLayer, nameof (TryExecute), ex);
      }
      output = default (TResult);
      return false;
    }

    internal static DateTime GetTeamCapacityCollectionCurrentDate(IVssRequestContext requestContext) => new DateTime(TimeZoneInfo.ConvertTime(DateTime.Now, requestContext.GetCollectionTimeZone()).Date.Ticks, DateTimeKind.Utc);

    internal static bool TryGetAgileSettings(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project,
      WebApiTeam team,
      out IAgileSettings settings)
    {
      CommonStructureProjectInfo project1 = CommonStructureProjectInfo.ConvertProjectInfo(project);
      try
      {
        settings = (IAgileSettings) new AgileSettings(requestContext, project1, team);
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(290782, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, ex);
        settings = (IAgileSettings) null;
        return false;
      }
    }
  }
}
