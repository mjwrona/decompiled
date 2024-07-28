// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingOperationSet
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ServicingOperationSet
  {
    private List<ServicingOperation> m_operations;

    public ServicingOperationSet(
      IServicingOperationProvider operationProvider,
      params string[] servicingOperations)
    {
      ArgumentUtility.CheckForNull<IServicingOperationProvider>(operationProvider, nameof (operationProvider));
      this.OperationProvider = operationProvider;
      this.ServicingOperations = servicingOperations;
    }

    public string[] ServicingOperations { get; private set; }

    public IServicingOperationProvider OperationProvider { get; private set; }

    public List<ServicingOperation> GetServicingOperations()
    {
      if (this.m_operations == null)
      {
        this.m_operations = new List<ServicingOperation>(this.ServicingOperations.Length);
        foreach (string servicingOperation in this.ServicingOperations)
        {
          if (!string.IsNullOrWhiteSpace(servicingOperation))
            this.m_operations.Add(this.OperationProvider.GetServicingOperation(servicingOperation) ?? throw new TeamFoundationServicingException(string.Format("Operation not found: '{0}'. Target: '{1}'", (object) servicingOperation, (object) this.OperationProvider.Target)));
        }
      }
      return this.m_operations;
    }
  }
}
