# State Pattern Kata (.NET 10 Modular Monolith)

A production-grade kata demonstrating the State Pattern combined with Domain-Driven Design (DDD) principles, built using a Modular Monolith architecture in C# / .NET 10.

## 🎯 Kata Overview & Domain Context

This kata models a **Loan Case Application System (Dossier de Prêt)**. The goal is to enforce complex state transitions and business invariants dynamically using the State Pattern, preventing invalid domain operations without relying on bloated if/else or switch statements inside the main entity.

## 📋 Business Rules & State Workflow

A `LoanCase` goes through a strict life cycle with specific business rules governing each transition:

1. **Draft (DraftState):**
    - Initial state when a loan application is created.
    - The applicant details and requested loan amount can be modified.
    - *Transition:* Can be submitted for evaluation if all required information is valid.

2. **Submitted (SubmittedState):**
    - The loan application is locked for editing by the applicant.
    - Underwriting checks and automated risk assessments are triggered.
    - *Transitions:* Can be Approved by an underwriter or Rejected.

3. **Approved (ApprovedState):**
    - The loan is officially approved.
    - Loan agreement documents are generated for the customer.
    - *Transitions:* Can move to Activated once signed, or Cancelled if expired/refused.

4. **Activated (ActivatedState):**
    - The funds are disbursed, and repayment begins.
    - *Terminal State:* No further state transitions allowed.

5. **Rejected / Cancelled (RejectedState / CancelledState):**
    - *Terminal States:* Read-only states. No further edits or transitions are permitted.

## 🛠️ Engineering Standards

- **.NET 10:** Built on the latest modern, high-performance .NET runtime.
- **Central Package Management (CPM):** All NuGet package versions are managed globally via `Directory.Packages.props`.
- **Domain-Driven Design (DDD):** Rich domain models, encapsulation, and immutable Value Objects.
- **Code Formatting:** Clean, automated styling aligned with Csharpier and `.editorconfig`.

## ⚠️ Known Limitations & Technical Debt

### Missing transaction to guarantee use-case atomicity

Most use cases perform **two separate writes** that must succeed or fail together:

1. appending a new `LoanCaseStateSnapshot` (state history / audit trail);
2. updating the `LoanCase` aggregate in its repository.

These writes are currently executed sequentially, **outside of any transaction**:

```csharp
await new LoanCaseSnapshotService(snapshotRepository).ExecuteAsync(loanCase, ct);
await loanCaseRepository.UpdateAsync(loanCase, ct);
```

If the second call fails (database outage, optimistic concurrency conflict, network error, etc.), the snapshot has already been persisted while the aggregate keeps its previous state — or vice versa. The snapshot history no longer reflects the actual loan case, leaving the system **inconsistent**.

The use cases affected are `Create`, `Submit`, `SendToCommittee`, `Approve`, `Reject`, `Close` and `Restore`.

> The in-memory repositories used by the test suite mask this issue, because their writes never fail.

**Planned fix:** introduce a transactional boundary (Unit of Work / `IDbContextTransaction`) so that the snapshot append and the aggregate update are committed atomically. The snapshot stays append-only and versioned — only the persistence boundary changes.

**Acceptance criteria:**
- [ ] A shared transaction abstraction (e.g. `IUnitOfWork`) usable by every handler.
- [ ] Snapshot write and aggregate update committed within a single transaction.
- [ ] A test proving the rollback: if the update fails after the snapshot write, no snapshot remains persisted.

---

## 📖 Command Cheat Sheet & Explanations

Here is a complete breakdown of all the terminal commands used to build this architecture from scratch with a clean, flat directory structure (avoiding redundant intermediate technical folders).

### 1. Solution & Git Initialization
```bash
# Initialize a git repository to track version control
git init
```

### 2. Creating Domain and Test Libraries (Flattened Structure)
To create physical folders and projects directly matching their purpose without redundant nesting, we use the `dotnet new` command with explicit outputs:

```bash
# Create the Domain Class Library directly in its dedicated module path
dotnet new classlib -o src/Modules/LoanCase/LoanCase.Domain -n LoanCase.Domain

# Create the xUnit Test Project directly in its dedicated test path
dotnet new xunit -o src/Modules/LoanCase/LoanCase.Tests -n LoanCase.Tests
``

🔍 **Explanation of the Options:**
- `dotnet new`: Instructs the .NET CLI to generate a new project from a built-in template.
- `classlib / xunit`: Specifies the template type. `classlib` generates a pure C# library, while `xunit` generates a pre-configured unit testing project.
- `-o (or --output)`: Defines the exact physical directory on your disk where the project files will be created.
- `-n (or --name)`: Defines the internal project name, the `.csproj` file name, and the root namespace for the C# files.

### 3. Solution Integration & Project References
Once projects are created on the disk, they must be registered in the solution file (`.sln`) and linked if they depend on each other:

```bash
# Register the Domain project in the solution file
dotnet sln add src/Modules/LoanCase/LoanCase.Domain/LoanCase.Domain.csproj

# Register the Tests project in the solution file
dotnet sln add src/Modules/LoanCase/LoanCase.Tests/LoanCase.Tests.csproj

# Link the Test project to the Domain project so tests can access domain logic
dotnet add src/Modules/LoanCase/LoanCase.Tests/LoanCase.Tests.csproj reference src/Modules/LoanCase/LoanCase.Domain/LoanCase.Domain.csproj
```

🔍 **Explanation of the Options:**
- `dotnet sln add [path]`: Injects the project reference into the `.sln` file so that IDEs like JetBrains Rider can display and build it.
- `dotnet add [target] reference [dependency]`: Establishes a project-to-project reference, allowing the test suite to compile and test the domain classes.

### 4. Configuration Generation via CLI
```bash
# Automatically generate a standard .editorconfig file for C# code style
dotnet new editorconfig
```

---

## 🚀 Getting Started

1. Open the solution file `pattern-state-kata.sln` in JetBrains Rider or your preferred .NET IDE.
2. Restore NuGet dependencies:
   ```bash
   dotnet restore
   ```
3. Run the test suite to ensure everything is green:
   ```bash
   dotnet test