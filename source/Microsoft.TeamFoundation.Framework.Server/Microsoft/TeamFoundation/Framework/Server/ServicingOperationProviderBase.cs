// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingOperationProviderBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class ServicingOperationProviderBase : IServicingOperationProvider
  {
    private bool m_throwOnDuplicates = true;
    private ServicingOperation[] m_operations;
    private readonly IServicingStepGroupProvider m_stepGroupProvider;
    private readonly ServicingOperationProviderBase m_fallbackProvider;
    private readonly ITFLogger m_logger;
    private readonly object m_lock = new object();
    private static readonly char[] s_wildcards = new char[2]
    {
      '*',
      '?'
    };

    public ServicingOperationProviderBase(
      IServicingStepGroupProvider stepGroupProvider,
      ServicingOperationProviderBase fallbackProvider,
      ServicingOperationTarget target,
      bool hostedDeployment,
      ITFLogger logger)
    {
      ArgumentUtility.CheckForNull<IServicingStepGroupProvider>(stepGroupProvider, nameof (stepGroupProvider));
      this.m_stepGroupProvider = stepGroupProvider;
      this.m_fallbackProvider = fallbackProvider;
      this.Target = target;
      this.HostedDeployment = hostedDeployment;
      this.m_logger = logger ?? (ITFLogger) new NullLogger();
    }

    public bool ThrowOnDuplicates
    {
      get => this.m_throwOnDuplicates;
      protected set => this.m_throwOnDuplicates = value;
    }

    public ServicingOperationTarget Target { get; private set; }

    public bool HostedDeployment { get; private set; }

    public ServicingOperation GetServicingOperation(string servicingOperation)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(servicingOperation, nameof (servicingOperation));
      if (this.m_operations == null)
        this.LoadOperations();
      int num1 = 0;
      int num2 = this.m_operations.Length;
      while (num1 < num2)
      {
        int index = (num1 + num2) / 2;
        ServicingOperation operation = this.m_operations[index];
        int num3 = string.Compare(servicingOperation, operation.Name, StringComparison.OrdinalIgnoreCase);
        if (num3 > 0)
        {
          num1 = index + 1;
        }
        else
        {
          if (num3 >= 0)
            return operation;
          num2 = index;
        }
      }
      this.Logger.Warning("The following servicing operation was not found: {0}", (object) servicingOperation);
      return (ServicingOperation) null;
    }

    public string[] GetServicingOperationNames()
    {
      ServicingOperation[] servicingOperations = this.GetServicingOperations();
      string[] servicingOperationNames = new string[servicingOperations.Length];
      for (int index = 0; index < servicingOperationNames.Length; ++index)
        servicingOperationNames[index] = servicingOperations[index].Name;
      return servicingOperationNames;
    }

    public ServicingOperation[] GetServicingOperations()
    {
      this.LoadOperations();
      return this.m_operations;
    }

    private void LoadOperations()
    {
      if (this.m_operations != null)
        return;
      lock (this.m_lock)
      {
        if (this.m_operations != null)
          return;
        this.Logger.Info("Loading servicing operations.");
        Dictionary<string, ServicingOperation> dictionary1 = new Dictionary<string, ServicingOperation>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        Dictionary<string, string> dictionary2 = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        string[] operationResourceNames = this.GetServicingOperationResourceNames();
        this.Logger.Info("Found {0} resources.", (object) operationResourceNames.Length);
        for (int index = 0; index < operationResourceNames.Length; ++index)
        {
          using (Stream operationStream = this.OpenServicingOperationSteam(operationResourceNames[index]))
          {
            ServicingOperation servicingOperation = this.LoadServicingOperation(operationStream, operationResourceNames[index]);
            servicingOperation.Resource = operationResourceNames[index];
            try
            {
              dictionary1.Add(servicingOperation.Name, servicingOperation);
              dictionary2.Add(servicingOperation.Name, operationResourceNames[index]);
            }
            catch (ArgumentException ex)
            {
              throw new TeamFoundationServicingException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "'{0}' servicing operation is defined more than once. First resource: {1}, Second resource: {2}.", (object) servicingOperation.Name, (object) dictionary2[servicingOperation.Name], (object) operationResourceNames[index]));
            }
          }
        }
        if (this.m_fallbackProvider != null)
        {
          foreach (ServicingOperation servicingOperation in this.m_fallbackProvider.GetServicingOperations())
          {
            if (!dictionary1.ContainsKey(servicingOperation.Name))
            {
              dictionary1.Add(servicingOperation.Name, servicingOperation);
            }
            else
            {
              string message = string.Format("Duplicate operation loaded.  Name: '{0}'. Loaded from '{1}' and '{2}'.", (object) servicingOperation.Name, (object) dictionary1[servicingOperation.Name].Resource, (object) servicingOperation.Resource);
              this.Logger.Info(message);
              if (this.ThrowOnDuplicates)
                throw new TeamFoundationServicingException(message);
              this.Logger.Info("Resource for operation '{0}' loaded from '{1}' is ignored.", (object) servicingOperation.Name, (object) servicingOperation.Resource);
            }
          }
        }
        this.m_operations = dictionary1.Values.OrderBy<ServicingOperation, string>((Func<ServicingOperation, string>) (op => op.Name), (IComparer<string>) StringComparer.OrdinalIgnoreCase).ToArray<ServicingOperation>();
      }
    }

    private ServicingOperation LoadServicingOperation(
      Stream operationStream,
      string operationResourceName)
    {
      ServicingOperation servicingOperation = new ServicingOperation()
      {
        Target = this.Target
      };
      try
      {
        XPathDocument xpathDocument = new XPathDocument(operationStream);
        servicingOperation.Name = (xpathDocument.CreateNavigator().SelectSingleNode("/ServicingOperation/@name") ?? throw new TeamFoundationServicingException(FrameworkResources.ServicingOperationFileDoesNotDefineOperationNameError((object) operationResourceName))).Value;
        foreach (XPathNavigator xpathNavigator in xpathDocument.CreateNavigator().Select("/ServicingOperation/ServicingStepGroup"))
        {
          string attribute = xpathNavigator.GetAttribute("name", string.Empty);
          ServicingOperationProviderBase.ValidateGroupName(attribute);
          bool flag1 = string.Equals(xpathNavigator.GetAttribute("optional", string.Empty), "true", StringComparison.OrdinalIgnoreCase);
          int num1 = string.Equals(xpathNavigator.GetAttribute("onPremOnly", string.Empty), "true", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
          bool flag2 = string.Equals(xpathNavigator.GetAttribute("hostedOnly", string.Empty), "true", StringComparison.OrdinalIgnoreCase);
          if ((num1 == 0 || !this.HostedDeployment) && (!flag2 || this.HostedDeployment))
          {
            int num2 = attribute.IndexOfAny(ServicingOperationProviderBase.s_wildcards) >= 0 ? 1 : 0;
            List<ServicingStepGroup> collection = new List<ServicingStepGroup>();
            if (num2 == 0)
            {
              ServicingStepGroup servicingStepGroup = this.m_stepGroupProvider.GetServicingStepGroup(attribute);
              if (servicingStepGroup != null)
                collection.Add(servicingStepGroup);
            }
            else
            {
              Regex regex = new Regex("^" + Regex.Escape(attribute).Replace("\\*", ".*").Replace('?', '.') + "$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
              foreach (ServicingStepGroup servicingStepGroup in this.m_stepGroupProvider.GetServicingStepGroups())
              {
                if (regex.Match(servicingStepGroup.Name).Success)
                {
                  if (string.Equals(servicingStepGroup.Name, "VsspFeatureAvailability", StringComparison.Ordinal))
                    collection.Insert(0, servicingStepGroup);
                  else
                    collection.Add(servicingStepGroup);
                }
              }
            }
            if (collection.Count > 0)
              servicingOperation.Groups.AddRange((IEnumerable<ServicingStepGroup>) collection);
            else if (!flag1)
              throw new TeamFoundationServicingException(FrameworkResources.ServicingOperationGroupUnknownError((object) operationResourceName, (object) xpathNavigator.GetAttribute("name", "")));
          }
        }
        XPathNodeIterator source = xpathDocument.CreateNavigator().Select("/ServicingOperation/ExecutionHandlers/ExecutionHandler/@type");
        servicingOperation.ExecutionHandlers.AddRange(source.OfType<XPathNavigator>().Select<XPathNavigator, ServicingExecutionHandlerData>((Func<XPathNavigator, ServicingExecutionHandlerData>) (node => new ServicingExecutionHandlerData(node.Value))));
      }
      catch (Exception ex)
      {
        throw new TeamFoundationServicingException(FrameworkResources.ServicingResourcesLoadException((object) operationResourceName), ex);
      }
      return servicingOperation;
    }

    private static void ValidateGroupName(string groupName)
    {
      foreach (char ch in groupName)
      {
        if ((ch < 'A' || ch > 'Z') && (ch < 'a' || ch > 'z') && (ch < '0' || ch > '9') && ch != '.' && ch != '_' && ch != '-' && ch != '?' && ch != '*')
          throw new ArgumentException("'" + groupName + "' is an invalid group name.");
      }
    }

    protected abstract string[] GetServicingOperationResourceNames();

    protected abstract Stream OpenServicingOperationSteam(string resourceName);

    protected ITFLogger Logger => this.m_logger;
  }
}
