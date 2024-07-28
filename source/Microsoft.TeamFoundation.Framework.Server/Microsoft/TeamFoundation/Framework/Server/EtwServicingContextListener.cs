// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.EtwServicingContextListener
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class EtwServicingContextListener : IServicingContextListener
  {
    private static readonly DateTime s_zeroDate = new DateTime(1601, 1, 1);

    public EtwServicingContextListener(
      ServicingEtwLogger logger,
      EtwServicingContextListenerConfiguration configuration)
    {
      this.Logger = logger;
      this.Configuration = configuration;
    }

    public ServicingEtwLogger Logger { get; }

    public EtwServicingContextListenerConfiguration Configuration { get; }

    public void OnOperationStarted(object sender, ServicingOperationStartedEventArgs args) => ServicingEtwLogger.Log.OperationStarted(this.NormalizeString(args.Name), this.NormalizeString(this.Configuration.ServiceName), this.NormalizeString(this.Configuration.BranchName), this.NormalizeString(this.Configuration.BuildNumber), this.NormalizeString(this.Configuration.DeploymentName), this.NormalizeString(this.Configuration.ReleaseDefinitionId), this.NormalizeString(this.Configuration.ReleaseId), this.NormalizeString(this.Configuration.AttemptNumber), this.NormalizeString(this.Configuration.JobId), this.NormalizeDateTime(args.Timestamp));

    public void OnOperationEnded(object sender, ServicingOperationEndedEventArgs args) => ServicingEtwLogger.Log.OperationEnded(this.NormalizeString(args.Name), args.Duration.TotalMilliseconds, this.NormalizeString(this.Configuration.ServiceName), this.NormalizeString(this.Configuration.BranchName), this.NormalizeString(this.Configuration.BuildNumber), this.NormalizeString(this.Configuration.DeploymentName), this.NormalizeString(this.Configuration.ReleaseDefinitionId), this.NormalizeString(this.Configuration.ReleaseId), this.NormalizeString(this.Configuration.AttemptNumber), this.NormalizeString(this.Configuration.JobId), this.NormalizeDateTime(args.Timestamp));

    public void OnStepGroupStarted(object sender, ServicingStepGroupStartedEventArgs args) => ServicingEtwLogger.Log.StepGroupStarted(this.NormalizeString(args.Operation), this.NormalizeString(args.Name), this.NormalizeString(this.Configuration.ServiceName), this.NormalizeString(this.Configuration.BranchName), this.NormalizeString(this.Configuration.BuildNumber), this.NormalizeString(this.Configuration.DeploymentName), this.NormalizeString(this.Configuration.ReleaseDefinitionId), this.NormalizeString(this.Configuration.ReleaseId), this.NormalizeString(this.Configuration.AttemptNumber), this.NormalizeString(this.Configuration.JobId), this.NormalizeDateTime(args.Timestamp));

    public void OnStepGroupEnded(object sender, ServicingStepGroupEndedEventArgs args) => ServicingEtwLogger.Log.StepGroupEnded(this.NormalizeString(args.Operation), this.NormalizeString(args.Name), args.Duration.TotalMilliseconds, this.NormalizeString(this.Configuration.ServiceName), this.NormalizeString(this.Configuration.BranchName), this.NormalizeString(this.Configuration.BuildNumber), this.NormalizeString(this.Configuration.DeploymentName), this.NormalizeString(this.Configuration.ReleaseDefinitionId), this.NormalizeString(this.Configuration.ReleaseId), this.NormalizeString(this.Configuration.AttemptNumber), this.NormalizeString(this.Configuration.JobId), this.NormalizeDateTime(args.Timestamp));

    public void OnStepStarted(object sender, ServicingStepStartedEventArgs args) => ServicingEtwLogger.Log.StepStarted(this.NormalizeString(args.Operation), this.NormalizeString(args.Group), this.NormalizeString(args.Name), this.NormalizeString(args.State.ToString()), this.NormalizeString(this.Configuration.ServiceName), this.NormalizeString(this.Configuration.BranchName), this.NormalizeString(this.Configuration.BuildNumber), this.NormalizeString(this.Configuration.DeploymentName), this.NormalizeString(this.Configuration.ReleaseDefinitionId), this.NormalizeString(this.Configuration.ReleaseId), this.NormalizeString(this.Configuration.AttemptNumber), this.NormalizeString(this.Configuration.JobId), this.NormalizeDateTime(args.Timestamp));

    public void OnStepEnded(object sender, ServicingStepEndedEventArgs args) => ServicingEtwLogger.Log.StepEnded(this.NormalizeString(args.Operation), this.NormalizeString(args.Group), this.NormalizeString(args.Name), this.NormalizeString(args.State.ToString()), args.Duration.TotalMilliseconds, this.NormalizeString(this.Configuration.ServiceName), this.NormalizeString(this.Configuration.BranchName), this.NormalizeString(this.Configuration.BuildNumber), this.NormalizeString(this.Configuration.DeploymentName), this.NormalizeString(this.Configuration.ReleaseDefinitionId), this.NormalizeString(this.Configuration.ReleaseId), this.NormalizeString(this.Configuration.AttemptNumber), this.NormalizeString(this.Configuration.JobId), this.NormalizeDateTime(args.Timestamp));

    private string NormalizeString(string arg) => !string.IsNullOrWhiteSpace(arg) ? arg : string.Empty;

    private DateTime NormalizeDateTime(DateTime date) => !(date < EtwServicingContextListener.s_zeroDate) ? date : EtwServicingContextListener.s_zeroDate;
  }
}
