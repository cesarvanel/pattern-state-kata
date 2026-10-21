using LoanCase.Application.Command.Approve;
using LoanCase.Application.Command.Close;
using LoanCase.Application.Command.Create;
using LoanCase.Application.Command.Reject;
using LoanCase.Application.Command.Restore;
using LoanCase.Application.Command.SendToCommittee;
using LoanCase.Application.Command.Submit;
using LoanCase.Application.Services;
using LoanCase.Tests.Repository;

namespace LoanCase.Tests;

public sealed class LoanCaseTestContext
{
    public LoanCaseTestContext()
    {
        LoanCaseRepository = new InMemoryLoanRepository();
        SnapshotRepository = new InMemoryLoanCaseSnapshotRepository();
        GenerateLoanCaseNumberService = new InMemoryGenerateLoanCaseNumberService(null);

        SnapshotService = new LoanCaseSnapshotService(SnapshotRepository);

        CreateHandler = new CreateLoanCaseHandler(LoanCaseRepository,
            GenerateLoanCaseNumberService,
            SnapshotService);
        SubmitHandler = new SubmitLoanCaseHandler(LoanCaseRepository, SnapshotService);
        SendToCommitteeHandler = new SendLoanCaseToCommitteeHandler(LoanCaseRepository,
            SnapshotService);
        ApproveHandler = new ApproveLoanCaseHandler(LoanCaseRepository, SnapshotService);
        RejectHandler = new RejectLoanCaseHandler(LoanCaseRepository, SnapshotService);
        CloseHandler = new CloseLoanCaseHandler(LoanCaseRepository, SnapshotService);
        RestoreHandler = new RestoreLoanCaseStateHandler(LoanCaseRepository, SnapshotService);
    }

    public InMemoryLoanRepository LoanCaseRepository { get; }
    public InMemoryLoanCaseSnapshotRepository SnapshotRepository { get; }
    public InMemoryGenerateLoanCaseNumberService GenerateLoanCaseNumberService { get; }
    public ILoanCaseSnapshotService SnapshotService { get; }

    public CreateLoanCaseHandler CreateHandler { get; }
    public SubmitLoanCaseHandler SubmitHandler { get; }
    public SendLoanCaseToCommitteeHandler SendToCommitteeHandler { get; }
    public ApproveLoanCaseHandler ApproveHandler { get; }
    public RejectLoanCaseHandler RejectHandler { get; }
    public CloseLoanCaseHandler CloseHandler { get; }
    public RestoreLoanCaseStateHandler RestoreHandler { get; }
}