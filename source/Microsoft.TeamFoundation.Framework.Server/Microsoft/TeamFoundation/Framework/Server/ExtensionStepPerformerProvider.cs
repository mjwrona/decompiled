// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ExtensionStepPerformerProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ExtensionStepPerformerProvider : IStepPerformerProvider
  {
    private Dictionary<string, IStepPerformer> m_stepPerformers;
    private Dictionary<string, IStepPerformer> m_extensibleStepPerformers;
    private static readonly object s_providersLock = new object();
    private static readonly ConcurrentDictionary<string, ExtensionStepPerformerProvider> s_providers = new ConcurrentDictionary<string, ExtensionStepPerformerProvider>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public static ExtensionStepPerformerProvider Get(string plugInDirectory, ITFLogger logger)
    {
      ExtensionStepPerformerProvider performerProvider = (ExtensionStepPerformerProvider) null;
      if (!ExtensionStepPerformerProvider.s_providers.TryGetValue(plugInDirectory, out performerProvider))
      {
        lock (ExtensionStepPerformerProvider.s_providersLock)
        {
          if (!ExtensionStepPerformerProvider.s_providers.TryGetValue(plugInDirectory, out performerProvider))
          {
            performerProvider = new ExtensionStepPerformerProvider(plugInDirectory, logger);
            ExtensionStepPerformerProvider.s_providers.TryAdd(plugInDirectory, performerProvider);
          }
        }
      }
      return performerProvider;
    }

    public ExtensionStepPerformerProvider(string plugInDirectory, ITFLogger logger) => this.Initialize(plugInDirectory, logger ?? (ITFLogger) new TraceLogger());

    public IStepPerformer GetStepPerformer(string name)
    {
      IStepPerformer stepPerformer;
      if (!this.TryGetStepPerformer(name, out stepPerformer))
        throw new StepPerformerNotFoundException(FrameworkResources.StepPerformerNotFoundException((object) name));
      return stepPerformer;
    }

    public bool TryGetStepPerformer(string name, out IStepPerformer stepPerformer)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      return this.m_extensibleStepPerformers.TryGetValue(name, out stepPerformer) || this.m_stepPerformers.TryGetValue(name, out stepPerformer);
    }

    private void Initialize(string plugInDirectory, ITFLogger logger)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(plugInDirectory, nameof (plugInDirectory));
      this.m_stepPerformers = new Dictionary<string, IStepPerformer>();
      this.m_extensibleStepPerformers = new Dictionary<string, IStepPerformer>();
      try
      {
        using (IDisposableReadOnlyList<IStepPerformer> extensionsRaw = VssExtensionManagementService.GetExtensionsRaw<IStepPerformer>(plugInDirectory))
        {
          foreach (IStepPerformer stepPerformer in (IEnumerable<IStepPerformer>) extensionsRaw)
          {
            if (string.IsNullOrEmpty(stepPerformer.Name))
            {
              TeamFoundationTrace.Error(TraceKeywordSets.General, "A step performer {0} does not provide a name.", (object) stepPerformer.GetType().FullName);
            }
            else
            {
              if (stepPerformer is IDisposable)
                throw new TeamFoundationServicingException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Step performer {0} is disposable, which is not allowed.", (object) stepPerformer.GetType().FullName));
              try
              {
                if (stepPerformer is IStepPerformerOverride)
                  this.m_extensibleStepPerformers.Add(stepPerformer.Name, stepPerformer);
                else
                  this.m_stepPerformers.Add(stepPerformer.Name, stepPerformer);
              }
              catch (ArgumentException ex)
              {
                string message = string.Format("More than one step performer with name '{0}' was found. Type1: {1}, Type2: {2}.", (object) stepPerformer.Name, (object) stepPerformer.GetType().FullName, (object) this.m_stepPerformers[stepPerformer.Name].GetType().FullName);
                logger.Error(message);
                throw new DuplicateStepPerformerException(FrameworkResources.StepPerformerHasBeenRegisteredError((object) stepPerformer.Name));
              }
            }
          }
        }
        foreach (KeyValuePair<string, IStepPerformer> extensibleStepPerformer in this.m_extensibleStepPerformers)
        {
          string key = extensibleStepPerformer.Key;
          IStepPerformerOverride performerOverride = extensibleStepPerformer.Value as IStepPerformerOverride;
          IStepPerformer baseStepPerformer;
          if (this.m_stepPerformers.TryGetValue(key, out baseStepPerformer))
          {
            performerOverride.SetBaseStepPerformer(baseStepPerformer);
          }
          else
          {
            string message = string.Format("Failed to find base step performer for step performer override with name '{0}'. Type: {1}.", (object) key, (object) performerOverride.GetType().FullName);
            logger.Error(message);
            throw new OverrideWithNoBaseStepPerformerException(FrameworkResources.StepPerformerOverrideWithNoBaseError((object) key));
          }
        }
      }
      catch (Exception ex)
      {
        logger.Error(ex);
        throw;
      }
    }
  }
}
