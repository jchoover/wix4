@setlocal
@pushd %~dp0

@set _C=Debug
:parse_args
@if /i "%1"=="release" set _C=Release
@if /i "%1"=="test" set RuntimeTestsEnabled=true
@if not "%1"=="" shift & goto parse_args

@if "%RuntimeTestsEnabled%"=="" echo Build integration tests %_C%
@if not "%RuntimeTestsEnabled%"=="" set _T=test&echo Run integration tests %_C%

@call burn\test_burn.cmd %_C% %_T%

@popd
@endlocal
