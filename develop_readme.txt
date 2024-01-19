
ipc は使用できなくなった dot net8より


このため、ipclogは削除


log出力まわり logmanager + log出力ライブラリより作り直しが必要

logmanagerは別プログラムとする。
　シナリオ解析+wav作成、aviutl側の顔画像合成サポートdll、画像合成プログラムよりllog出力を行うため

# log出力をどうするか、、、
fileに書き出し、共有ququeに登録する、、、
named pipeが使えるか？




# .vcporj設定
以下の設定をいれ、debugを出力先にし、net8.0-windowsがつかないようにする

    <BaseOutputPath>..\..\\</BaseOutputPath>
    <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
    <AppendRuntimeIdentifierToOutputPath>false</AppendRuntimeIdentifierToOutputPath>

<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <UseWindowsForms>true</UseWindowsForms>
    <ImplicitUsings>enable</ImplicitUsings>
    <BaseOutputPath>..\..\\</BaseOutputPath>
    <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
    <AppendRuntimeIdentifierToOutputPath>false</AppendRuntimeIdentifierToOutputPath>
  </PropertyGroup>

</Project>