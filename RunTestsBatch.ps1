$PackagePath = "D:\Photon\Quantum\worktree\max-components-variable\quantum_unity\Photon-Quantum-Sdk_v3.0.0.Preview.0.unitypackage"
.\ImportPackageAndRunTest_2021.ps1 -PackagePath $PackagePath -Platform Android -AddPragmaMaxComponents512 -ScriptingBackend IL2CPP
.\ImportPackageAndRunTest_2021.ps1 -PackagePath $PackagePath -Platform Android -ScriptingBackend IL2CPP
.\ImportPackageAndRunTest_2021.ps1 -PackagePath $PackagePath -Platform StandaloneWindows64 -AddPragmaMaxComponents512 -ScriptingBackend IL2CPP
.\ImportPackageAndRunTest_2021.ps1 -PackagePath $PackagePath -Platform StandaloneWindows64 -ScriptingBackend IL2CPP

$PackagePath = "D:\Photon\Quantum\worktree\max-components-512\quantum_unity\Photon-Quantum-Sdk_v3.0.0.Preview.0.unitypackage"
.\ImportPackageAndRunTest_2021.ps1 -PackagePath $PackagePath -Platform Android -ScriptingBackend IL2CPP
.\ImportPackageAndRunTest_2021.ps1 -PackagePath $PackagePath -Platform StandaloneWindows64 -ScriptingBackend IL2CPP

$PackagePath = "D:\Photon\Quantum\quantum_unity\Photon-Quantum-Sdk_v3.0.0.Preview.0.unitypackage"
.\ImportPackageAndRunTest_2021.ps1 -PackagePath $PackagePath -Platform Android -ScriptingBackend IL2CPP -IsBaseline
.\ImportPackageAndRunTest_2021.ps1 -PackagePath $PackagePath -Platform StandaloneWindows64 -ScriptingBackend IL2CPP -IsBaseline

.\GenerateReport.ps1 -Platform StandaloneWindows64 -ScriptingBackend IL2CPP
.\GenerateReport.ps1 -Platform Android -ScriptingBackend IL2CPP