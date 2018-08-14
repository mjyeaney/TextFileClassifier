Set-Location ..\Tests

dotnet test -l:"console;verbosity=normal" | Tee-Object ..\Scripts\tests.log

Set-Location ..\Scripts
