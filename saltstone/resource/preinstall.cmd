@(
REN "%~f0" "%~nx0.wsf"
CScript.exe //NoLogo "%~f0.wsf" %*
REN "%~f0.wsf" "%~nx0"
GOTO :EOF
)


<package>

  <job id="yrdy">
    <script language="JScript">
       var url = "https://download.visualstudio.microsoft.com/download/pr/2d6bb6b2-226a-4baa-bdec-798822606ff1/9b7b8746971ed51a1770ae4293618187/ndp48-web.exe";
       var file = "ndp48-web.exe";

       // var shell = WScript.CreateObject("WScript.Shell");
       var shell = new ActiveXObject("WScript.Shell");
       var homed = shell.SpecialFolders("Desktop");
       // var homed = shell.ExpandEnvironmentStrings("%HOME%");
       // var colUserEnvVars = shell.Environment("SYSTEM");
       WScript.Echo("User-specific PATH Environment Variable");
//       WScript.Echo(colUserEnvVars("HOME"));
//       WScript.Echo(colUserEnvVars("HOME"));
       homed = homed.replace('Desktop','');
       WScript.Echo(homed);
       // request = WScript.CreateObject('MSXML2.ServerXMLHTTP');
       var request = WScript.CreateObject('MSXML2.XMLHTTP.6.0');

       request.open('GET', url, false); // not async
       request.send();
       if (request.status !== 200) {
         WScript.Echo("request failed");
       }
       WScript.Echo("Size: " + request.getResponseHeader("Content-Length") + " bytes");
       var cmd = "del /Q " + file;
       WScript.Echo(cmd);

       shell.Run("cmd /C del /Q " + file,0,true);

       stream = WScript.CreateObject('ADODB.Stream');
       stream.Open();
       stream.Type = 1; // adTypeBinary
       stream.Write(request.responseBody);
       stream.Position = 0; // rewind
       stream.SaveToFile(file, 1); // adSaveCreateNotExist
       stream.Close();
       WScript.Echo('Done');

       try
       {
         shell.Run(file,1,true);
       } catch(e)
       {
         WScript.Echo(e.message);
       }
       shell.Run("cmd /C del /Q " + file,0,true);

    </script>
  </job>
</package>
