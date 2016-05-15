Set-ExecutionPolicy Unrestricted -Scope Process -Force
Get-ChildItem –Path . –Include *databases* -Recurse | Remove-Item -Recurse -Force