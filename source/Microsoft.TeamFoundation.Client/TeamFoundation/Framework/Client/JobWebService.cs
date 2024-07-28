// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.JobWebService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class JobWebService : TfsHttpClient
  {
    public JobWebService(TfsConnection connection)
      : base(connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("22da67e4-dbe5-4e09-9bd3-02ff4b620aa7");

    protected override string ComponentName => "Framework";

    protected override Guid ConfigurationServiceIdentifier => new Guid("da1c0184-14fe-4e13-b7fc-6eaa07d84be8");

    protected override string ServiceType => "JobService";

    protected override Exception ConvertException(SoapException exception) => TeamFoundationServiceException.ConvertException(exception);

    public bool PauseJob(Guid jobId) => (bool) this.Invoke((TfsClientOperation) new JobWebService.PauseJobClientOperation(), new object[1]
    {
      (object) jobId
    });

    public TeamFoundationJobDefinition[] QueryJobDefinitions(IEnumerable<Guid> jobIds) => (TeamFoundationJobDefinition[]) this.Invoke((TfsClientOperation) new JobWebService.QueryJobDefinitionsClientOperation(), new object[1]
    {
      (object) jobIds
    });

    public TeamFoundationJobHistoryEntry[] QueryJobHistory(IEnumerable<Guid> jobIds) => (TeamFoundationJobHistoryEntry[]) this.Invoke((TfsClientOperation) new JobWebService.QueryJobHistoryClientOperation(), new object[1]
    {
      (object) jobIds
    });

    public TeamFoundationJobHistoryEntry[] QueryLatestJobHistory(IEnumerable<Guid> jobIds) => (TeamFoundationJobHistoryEntry[]) this.Invoke((TfsClientOperation) new JobWebService.QueryLatestJobHistoryClientOperation(), new object[1]
    {
      (object) jobIds
    });

    public int QueueJobs(IEnumerable<Guid> jobIds, bool highPriority, int maxDelaySeconds) => (int) this.Invoke((TfsClientOperation) new JobWebService.QueueJobsClientOperation(), new object[3]
    {
      (object) jobIds,
      (object) highPriority,
      (object) maxDelaySeconds
    });

    public Guid QueueOneTimeJob(
      string jobName,
      string extensionName,
      XmlNode jobData,
      bool highPriority)
    {
      return (Guid) this.Invoke((TfsClientOperation) new JobWebService.QueueOneTimeJobClientOperation(), new object[4]
      {
        (object) jobName,
        (object) extensionName,
        (object) jobData,
        (object) highPriority
      });
    }

    public bool ResumeJob(Guid jobId) => (bool) this.Invoke((TfsClientOperation) new JobWebService.ResumeJobClientOperation(), new object[1]
    {
      (object) jobId
    });

    public bool StopJob(Guid jobId) => (bool) this.Invoke((TfsClientOperation) new JobWebService.StopJobClientOperation(), new object[1]
    {
      (object) jobId
    });

    public void UpdateJobDefinitions(
      IEnumerable<Guid> jobsToDelete,
      IEnumerable<TeamFoundationJobDefinition> jobUpdates)
    {
      this.Invoke((TfsClientOperation) new JobWebService.UpdateJobDefinitionsClientOperation(), new object[2]
      {
        (object) jobsToDelete,
        (object) jobUpdates
      });
    }

    internal sealed class PauseJobClientOperation : TfsClientOperation
    {
      public override string BodyName => "PauseJob";

      public override bool HasOutputs => true;

      public override string ResultName => "PauseJobResult";

      public override string SoapAction => "http://microsoft.com/webservices/PauseJob";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) false;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.BooleanFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter = (Guid) parameters[0];
        if (!(parameter != Guid.Empty))
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "jobId", parameter);
      }
    }

    internal sealed class QueryJobDefinitionsClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryJobDefinitions";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryJobDefinitionsResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueryJobDefinitions";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfTeamFoundationJobDefinition;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfTeamFoundationJobDefinitionFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        IEnumerable<Guid> parameter = (IEnumerable<Guid>) parameters[0];
        Helper.ToXml((XmlWriter) writer, "jobIds", parameter, false, false);
      }
    }

    internal sealed class QueryJobHistoryClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryJobHistory";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryJobHistoryResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueryJobHistory";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfTeamFoundationJobHistoryEntry;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfTeamFoundationJobHistoryEntryFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        IEnumerable<Guid> parameter = (IEnumerable<Guid>) parameters[0];
        Helper.ToXml((XmlWriter) writer, "jobIds", parameter, false, false);
      }
    }

    internal sealed class QueryLatestJobHistoryClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryLatestJobHistory";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryLatestJobHistoryResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueryLatestJobHistory";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfTeamFoundationJobHistoryEntry;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfTeamFoundationJobHistoryEntryFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        IEnumerable<Guid> parameter = (IEnumerable<Guid>) parameters[0];
        Helper.ToXml((XmlWriter) writer, "jobIds", parameter, false, false);
      }
    }

    internal sealed class QueueJobsClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueueJobs";

      public override bool HasOutputs => true;

      public override string ResultName => "QueueJobsResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueueJobs";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) 0;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.Int32FromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        IEnumerable<Guid> parameter1 = (IEnumerable<Guid>) parameters[0];
        Helper.ToXml((XmlWriter) writer, "jobIds", parameter1, false, false);
        bool parameter2 = (bool) parameters[1];
        if (parameter2)
          XmlUtility.ToXmlElement((XmlWriter) writer, "highPriority", parameter2);
        int parameter3 = (int) parameters[2];
        if (parameter3 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "maxDelaySeconds", parameter3);
      }
    }

    internal sealed class QueueOneTimeJobClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueueOneTimeJob";

      public override bool HasOutputs => true;

      public override string ResultName => "QueueOneTimeJobResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueueOneTimeJob";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Guid.Empty;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.GuidFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "jobName", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "extensionName", parameter2);
        XmlNode parameter3 = (XmlNode) parameters[2];
        if (parameter3 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "jobData", parameter3);
        bool parameter4 = (bool) parameters[3];
        if (!parameter4)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "highPriority", parameter4);
      }
    }

    internal sealed class ResumeJobClientOperation : TfsClientOperation
    {
      public override string BodyName => "ResumeJob";

      public override bool HasOutputs => true;

      public override string ResultName => "ResumeJobResult";

      public override string SoapAction => "http://microsoft.com/webservices/ResumeJob";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) false;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.BooleanFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter = (Guid) parameters[0];
        if (!(parameter != Guid.Empty))
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "jobId", parameter);
      }
    }

    internal sealed class StopJobClientOperation : TfsClientOperation
    {
      public override string BodyName => "StopJob";

      public override bool HasOutputs => true;

      public override string ResultName => "StopJobResult";

      public override string SoapAction => "http://microsoft.com/webservices/StopJob";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) false;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.BooleanFromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter = (Guid) parameters[0];
        if (!(parameter != Guid.Empty))
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "jobId", parameter);
      }
    }

    internal sealed class UpdateJobDefinitionsClientOperation : TfsClientOperation
    {
      public override string BodyName => "UpdateJobDefinitions";

      public override string SoapAction => "http://microsoft.com/webservices/UpdateJobDefinitions";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        IEnumerable<Guid> parameter1 = (IEnumerable<Guid>) parameters[0];
        Helper.ToXml((XmlWriter) writer, "jobsToDelete", parameter1, false, false);
        IEnumerable<TeamFoundationJobDefinition> parameter2 = (IEnumerable<TeamFoundationJobDefinition>) parameters[1];
        Helper.ToXml((XmlWriter) writer, "jobUpdates", parameter2, false, false);
      }
    }
  }
}
