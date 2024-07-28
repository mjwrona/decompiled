// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FeatureMappingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class FeatureMappingService : IVssFrameworkService
  {
    private SparseTree<string> m_commandFeatureMapping;
    private SparseTree<string> m_jobFeatureMapping;
    private const char c_separator = '.';
    private const string s_tracingArea = "FeatureMapping";
    private const string s_tracingLayer = "Service";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      try
      {
        systemRequestContext.CheckDeploymentRequestContext();
        this.LoadCommandMapping(systemRequestContext);
        this.LoadJobMapping(systemRequestContext);
      }
      catch (Exception ex)
      {
        systemRequestContext.Trace(15040000, TraceLevel.Error, "FeatureMapping", "Service", "Caught Exception {0} while attempting start", (object) ex);
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    internal void LoadCommandMapping(IVssRequestContext requestContext)
    {
      using (StreamReader streamReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.TeamFoundation.Framework.Server.Telemetry.Tracing.FeatureMappingCommands.json")))
        this.LoadCommandMapping(requestContext, streamReader.ReadToEnd());
    }

    internal void LoadCommandMapping(IVssRequestContext requestContext, string commandMappingJson)
    {
      this.m_commandFeatureMapping = new SparseTree<string>('.', StringComparison.OrdinalIgnoreCase);
      ServiceCommandMappingSet commandMappingSet;
      if (!JsonUtilities.TryDeserialize<ServiceCommandMappingSet>(commandMappingJson, out commandMappingSet))
      {
        requestContext.Trace(15040002, TraceLevel.Error, "FeatureMapping", "Service", "Failed to deserialize feature command mapping json");
        throw new ArgumentException("Failed to deserialize feature command mapping json");
      }
      foreach (FeatureCommand featureCommand in commandMappingSet.CommandMapping)
      {
        string token;
        if (this.TryGetCommandMappingToken(requestContext, featureCommand.Application, featureCommand.Command, out token))
        {
          try
          {
            this.m_commandFeatureMapping.Add(token, featureCommand.FeatureName);
          }
          catch (ArgumentException ex)
          {
            requestContext.Trace(15040000, TraceLevel.Error, "FeatureMapping", "Service", "Caught Exception while adding mapping for Token '{0}': {1}", (object) token, (object) ex);
          }
        }
        else
          requestContext.Trace(15040002, TraceLevel.Error, "FeatureMapping", "Service", "Error getting a feature mapping token value for Application = {0}, Command = {1}", (object) featureCommand.Application, (object) featureCommand.Command);
      }
      requestContext.Trace(15040001, TraceLevel.Verbose, "FeatureMapping", "Service", "Initial load: loaded {0} command mappings", (object) this.m_commandFeatureMapping.Count);
    }

    internal void LoadJobMapping(IVssRequestContext requestContext)
    {
      using (StreamReader streamReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.TeamFoundation.Framework.Server.Telemetry.Tracing.FeatureMappingJobs.json")))
        this.LoadJobMapping(requestContext, streamReader.ReadToEnd());
    }

    internal void LoadJobMapping(IVssRequestContext requestContext, string jobMappingJson)
    {
      this.m_jobFeatureMapping = new SparseTree<string>('.', StringComparison.OrdinalIgnoreCase);
      ServiceJobMappingSet serviceJobMappingSet;
      if (!JsonUtilities.TryDeserialize<ServiceJobMappingSet>(jobMappingJson, out serviceJobMappingSet))
      {
        requestContext.Trace(15040002, TraceLevel.Error, "FeatureMapping", "Service", "Failed to deserialize feature job mapping json");
        throw new ArgumentException("Failed to deserialize feature job mapping json");
      }
      foreach (FeatureJob featureJob in serviceJobMappingSet.JobMapping)
      {
        if (!string.IsNullOrEmpty(featureJob.JobName))
          this.m_jobFeatureMapping.Add(featureJob.JobName, featureJob.FeatureName);
        else
          requestContext.Trace(15040002, TraceLevel.Error, "FeatureMapping", "Service", "Error getting a feature mapping token value for JobName {0}", (object) featureJob.JobName);
      }
      requestContext.Trace(15040001, TraceLevel.Verbose, "FeatureMapping", "Service", "Initial load: loaded {0} job mappings", (object) this.m_jobFeatureMapping.Count);
    }

    public bool TryGetCommandFeatureMapping(
      IVssRequestContext requestContext,
      string application,
      string command,
      out string feature)
    {
      try
      {
        string token;
        if (this.TryGetCommandMappingToken(requestContext, application, command, out token))
        {
          if (this.m_commandFeatureMapping != null)
          {
            string referencedObject;
            if (this.m_commandFeatureMapping.TryGetValue(token, false, out referencedObject))
            {
              feature = referencedObject;
              return true;
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(15040003, TraceLevel.Error, "FeatureMapping", "Service", "Caught Exception {0} while getting feature mapping", (object) ex);
        throw;
      }
      feature = string.Empty;
      return false;
    }

    public bool TryGetJobFeatureMapping(
      IVssRequestContext requestContext,
      string jobName,
      out string feature)
    {
      try
      {
        if (this.m_jobFeatureMapping != null)
        {
          string referencedObject;
          if (this.m_jobFeatureMapping.TryGetValue(jobName, false, out referencedObject))
          {
            feature = referencedObject;
            return true;
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(15040003, TraceLevel.Error, "FeatureMapping", "Service", "Caught Exception {0} while getting feature mapping", (object) ex);
        throw;
      }
      feature = string.Empty;
      return false;
    }

    private bool TryGetCommandMappingToken(
      IVssRequestContext requestContext,
      string application,
      string command,
      out string token)
    {
      token = string.Empty;
      if (string.IsNullOrEmpty(application) && string.IsNullOrEmpty(command))
      {
        requestContext.Trace(15040004, TraceLevel.Error, "FeatureMapping", "Service", "Application and Command cannot both be empty for command feature mapping");
        return false;
      }
      token = application + "." + command;
      return true;
    }
  }
}
