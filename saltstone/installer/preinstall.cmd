@(
REN "%~f0" "%~nx0.wsf"
CScript.exe //NoLogo "%~f0.wsf" %*
REN "%~f0.wsf" "%~nx0"
GOTO :EOF
)


<package>

  <job id="yrdy">
    <script language="JScript">
       // ver 1.1

       var installed = false;

       var shell = new ActiveXObject("WScript.Shell");
       var fsys = WScript.CreateObject("Scripting.FileSystemObject");

       // check .NET 4.8 Installed?
       var checkdir = "C:/Windows/Microsoft.NET/Framework/v4.0.30319/"
       var checkfile = "Accessibility.dll";
       // dir check
       var ret = fsys.FolderExists(checkdir);
       if( ret === true ) {
         // file check
         var fullfile = checkdir + checkfile;
         ret = fsys.FileExists(fullfile)
         if( ret === true) {
           var fver = fsys.GetFileVersion(fullfile);
           // WScript.Echo("ver:" + fver);
           var i = fver.indexOf("4.8");
           if(i === 0) {
             // var ret = shell.Popup(".NET4.8�̓C���X�g�[���ς݂ł�",5,"Info",0);
             WScript.Echo(".NET 4.8 already installed");
             installed = true;
           }
         }
       }
// installed = false for debug
       if( installed === false)
       {
         var url = "//download.visualstudio.microsoft.com/download/pr/2d6bb6b2-226a-4baa-bdec-798822606ff1/9b7b8746971ed51a1770ae4293618187/ndp48-web.exe";
         var file = "ndp48-web";

         var ret = shell.Popup("URL:" + url + "\r\n�_�E�����[�h���Ď��s���܂����H",10,"�����m�F",33);
         if ( ret !== 2) {
           var request = WScript.CreateObject('MSXML2.XMLHTTP.6.0');

           WScript.Echo("URL:" + url);
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

           var ret = shell.Popup("exe:" + file + "�����s���܂����H",10,"�����m�F",33);
           if ( ret !== 2) {

             try
             {
               shell.Run(file + ".exe",1,true);
             } catch(e)
             {
               WScript.Echo(e.message);
             }
             shell.Run("cmd /C del /Q " + file + ".exe" ,0,true);
           }
         }

       }

       // �����t�H���_��zip��program�z���ɓW�J�H
       // �J�����g�ɓW�J -> exe���s yes or no
       var ret = shell.Popup("Saltstone�C���X�g�[�������s���܂����H",10,"�����m�F",33);
       if ( ret == 2) {
         WScript.Quit(0);
       }
       var curdir = fsys.GetParentFolderName(WScript.ScriptFullName)
       WScript.Echo("curd:" + curdir);
       checkfile = "/setup/SaltstoneSetup.exe";
       fullfile = curdir + checkfile
       WScript.Echo("fullf:" + fullfile);
       ret = fsys.FileExists(fullfile)
       if ( ret === 2) {
         WScript.Quit(0);
         WScript.Echo("file not found");
       }

       shell.Run(fullfile,1,true);



    </script>
  </job>
</package>