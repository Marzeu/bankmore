@echo off

echo ================================
echo Iniciando BankMore...
echo ================================

echo.
echo Subindo ContaCorrente API...
start cmd /k "cd /d src\ContaCorrente\ContaCorrente.API && dotnet run"

echo.
echo Subindo Transferencia API...
start cmd /k "cd /d src\Transferencia\Transferencia.API && dotnet run"

echo.
echo ================================
echo APIs iniciadas!
echo ================================

pause