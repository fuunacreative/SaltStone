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


       var url = "//download.visualstudio.microsoft.com/download/pr/2d6bb6b2-226a-4baa-bdec-798822606ff1/9b7b8746971ed51a1770ae4293618187/ndp48-web.exe";
       var file = "ndp48-web";

       var shell = new ActiveXObject("WScript.Shell");
       var ret = shell.Popup("URL:" + url + "\r\nダウンロードして実行しますか？",10,"処理確認",33);
       if ( ret === 2) {
         WScript.Quit(0);
       }



    </script>
  </job>
</package>
