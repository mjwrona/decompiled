// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataImport.DataImportPreparationRunner
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server.DataImport
{
  public class DataImportPreparationRunner : IDisposable
  {
    public ImportValidationOutcome Outcome;
    private IDisposableReadOnlyList<IImportPreparation> m_preparations;
    private DataImportValidationContext m_validationContext;

    public IDisposableReadOnlyList<IImportPreparation> Preparations => this.m_preparations;

    public DataImportValidationType Skip { get; set; }

    public IList<string> Errors { get; private set; }

    public IList<string> Warnings { get; private set; }

    public event EventHandler<DataImportActivityCompletedEventArgs> PreparationCompleted;

    public DataImportPreparationRunner(DataImportValidationContext context)
    {
      ArgumentUtility.CheckForNull<DataImportValidationContext>(context, nameof (context));
      this.m_preparations = (IDisposableReadOnlyList<IImportPreparation>) null;
      this.m_validationContext = context;
      this.Warnings = (IList<string>) new List<string>();
      this.Errors = (IList<string>) new List<string>();
    }

    public void LoadExtensions(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      this.m_preparations = vssRequestContext.GetService<IVssExtensionManagementService>().GetExtensions<IImportPreparation>(vssRequestContext);
    }

    public void Dispose()
    {
      if (this.m_preparations == null)
        return;
      this.m_preparations.Dispose();
    }

    public void Run()
    {
      foreach (IImportPreparation preparation in (IEnumerable<IImportPreparation>) this.Preparations)
      {
        ITFLogger logger = this.m_validationContext.Logger;
        string message = (string) null;
        FileLogger fileLogger = (FileLogger) null;
        ImportValidationOutcome val2;
        try
        {
          fileLogger = new FileLogger(Path.Combine(this.m_validationContext.GetItem<string>("Output", "."), preparation.GetType().Name + ".log"), FileMode.Append);
          this.m_validationContext.SetLogger((ITFLogger) new AggregateLogger(new ITFLogger[2]
          {
            logger,
            (ITFLogger) fileLogger
          }));
          this.m_validationContext.Logger.Info("========================================================");
          this.m_validationContext.Logger.Info("Starting '" + preparation.Name + "' (" + preparation.GetType().ToString() + ")");
          val2 = preparation.Run(this.m_validationContext, out message);
        }
        catch (Exception ex)
        {
          val2 = ImportValidationOutcome.Failed;
          if (string.IsNullOrEmpty(message))
            message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, string.Format("{0} {1}", (object) preparation.Name, (object) val2));
          this.m_validationContext.Logger.Error(ex.ToReadableStackTrace());
        }
        finally
        {
          this.m_validationContext.SetLogger(logger);
          fileLogger?.Dispose();
        }
        switch (val2)
        {
          case ImportValidationOutcome.Warning:
            this.Warnings.Add(message);
            break;
          case ImportValidationOutcome.Failed:
            this.Errors.Add(message);
            break;
        }
        this.Outcome = (ImportValidationOutcome) Math.Max((int) this.Outcome, (int) val2);
        this.OnPreparationCompleted(new DataImportActivityCompletedEventArgs()
        {
          Name = preparation.Name,
          Outcome = val2
        });
      }
    }

    public virtual void OnPreparationCompleted(DataImportActivityCompletedEventArgs e)
    {
      EventHandler<DataImportActivityCompletedEventArgs> preparationCompleted = this.PreparationCompleted;
      if (preparationCompleted == null)
        return;
      preparationCompleted((object) this, e);
    }
  }
}
