# DCIT 318 - Assignment 3

Five separate C# console applications, one per question. Each is its own project
folder with its own `.csproj`, so they build and run independently.

1. **Q1_FinanceManagementSystem** - records, interfaces, and a sealed class to
   process and apply financial transactions to a savings account.
2. **Q2_HealthcareSystem** - a generic repository plus a dictionary grouping
   prescriptions by patient.
3. **Q3_WarehouseInventorySystem** - a generic repository with custom exceptions
   for duplicate items, missing items, and invalid quantities.
4. **Q4_SchoolGradingSystem** - reads student records from a text file, validates
   them, and writes a graded summary report using custom exceptions.
5. **Q5_InventoryRecordSystem** - an immutable record type logged to and reloaded
   from a JSON file using a generic logger.

## Running each app

Requires the .NET SDK (8.0 or later).

```bash
cd Q1_FinanceManagementSystem
dotnet run

cd ../Q2_HealthcareSystem
dotnet run

cd ../Q3_WarehouseInventorySystem
dotnet run

cd ../Q4_SchoolGradingSystem
dotnet run

cd ../Q5_InventoryRecordSystem
dotnet run
```

Q4 and Q5 create small data files (`students.txt`/`report.txt` and
`inventory.json`) in their own project folder the first time they run, so no
extra setup is needed before running them.

## Git workflow

Same approach as Assignment 1: create the repo, clone it, copy in each
question's folder, and commit each one separately, e.g.:

```bash
git add Q1_FinanceManagementSystem .gitignore
git commit -m "Add Q1 Finance Management System"

git add Q2_HealthcareSystem
git commit -m "Add Q2 Healthcare System"

git add Q3_WarehouseInventorySystem
git commit -m "Add Q3 Warehouse Inventory System"

git add Q4_SchoolGradingSystem
git commit -m "Add Q4 School Grading System"

git add Q5_InventoryRecordSystem
git commit -m "Add Q5 Inventory Record System"

git add README.md
git commit -m "Add README"

git push origin main
```
