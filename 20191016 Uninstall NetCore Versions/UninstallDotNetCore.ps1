# get installed dotNet SDKs
$dnSDKs = dotnet --list-sdks

Write-Host "---------------------------"
Write-Host "Installed SDKs:"
foreach ($dnSDK in $dnSDKs) {
    Write-Host $dnSDK
}
Write-Host "---------------------------"

foreach ($dnSDK in $dnSDKs) {
    # get sdk information
    $dnSDKSplit = $dnSDK.Split('[', [System.StringSplitOptions]::RemoveEmptyEntries)
    $dnSDKVersion = $dnSDKSplit[0]
    $dnSDKPath = $dnSDKSplit[1].replace(']', '')

    Write-Host "  >> Version = " $dnSDKVersion
    Write-Host "  >> Path    = " $dnSDKPath

    # uninstall sdk, all but 3.0   
    if($dnSDKVersion.StartsWith("3.")) {
        Write-Host "  Keep Version = " $dnSDKVersion
    }
    else {
        Write-Host "  Uninstalling Version = " $dnSDKVersion
        $dnAppName = "Microsoft .NET Core SDK - " + $dnSDKVersion
                
        $app = Get-WmiObject -Class Win32_Product | Where-Object { 
            $_.Name -match $dnAppName
        }        
        Write-Host $app.Name 
        Write-Host $app.IdentifyingNumber
        # official not working 
        # $app.Uninstall()
        # test with Hanselman mode
        pushd $env:SYSTEMROOT\System32
        $app.identifyingnumber |% { Start-Process msiexec -wait -ArgumentList "/x $_" }
    }
}
