@(
REN "%~f0" "%~nx0.wsf"
CScript.exe //NoLogo "%~f0.wsf" %*
REN "%~f0.wsf" "%~nx0"
GOTO :EOF
)


<package>

  <job id="yrdy">
    <script language="JScript">
       var url = "//download.visualstudio.microsoft.com/download/pr/2d6bb6b2-226a-4baa-bdec-798822606ff1/9b7b8746971ed51a1770ae4293618187/ndp48-web.exe";
       var file = "ndp48-web";

       var shell = new ActiveXObject("WScript.Shell");
       var homed = shell.SpecialFolders("Desktop");
       WScript.Echo("User-specific PATH Environment Variable");
       homed = homed.replace('Desktop','');
       WScript.Echo("current directory = " + homed);
       var request = WScript.CreateObject('MSXML2.XMLHTTP.6.0');

       request.open('GET', "https:" + url, false); // not async
       request.send();
       if (request.status !== 200) {
         WScript.Echo("request failed");
       }
       WScript.Echo("DownloadSize: " + request.getResponseHeader("Content-Length") + " bytes");
       var cmd = "del /Q " + file + ".exe";

       shell.Run("cmd /C del /Q " + file + ".exe",0,true);

       stream = WScript.CreateObject('ADODB.Stream');
       stream.Open();
       stream.Type = 1; // adTypeBinary
       stream.Write(request.responseBody);
       stream.Position = 0; // rewind
       stream.SaveToFile(file + ".exe", 1); // adSaveCreateNotExist
       stream.Close();
       WScript.Echo('Download end');

       try
       {
         shell.Run(file + ".exe",1,true);
       } catch(e)
       {
         WScript.Echo(e.message);
       }
       shell.Run("cmd /C del /Q " + file + ".exe" ,0,true);

    </script>
  </job>
</package>
