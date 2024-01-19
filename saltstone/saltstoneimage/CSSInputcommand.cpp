#include "CSSInputcommand.h"

int CSSInputcommand::getHeight(void)
{
  return height;
}
int CSSInputcommand::getWidth(void)
{
  return width;
}
std::vector<CSSPicturefile*>* CSSInputcommand::getPicturefiles(void)
{
  return picturefiles;
}




void CSSInputcommand::Dispose(void)
{
  if (picturefiles != nullptr) {
    picturefiles->clear();
    picturefiles = nullptr;
  }
  if (basedir != nullptr) {
    basedir->clear();
    basedir = nullptr;
  }
  if(outfile != nullptr)
  { 
    delete outfile;
    outfile = nullptr;
  }
  if (filename != nullptr)
  {
    filename->clear();
    filename = nullptr;
  }

}

bool CSSInputcommand::parse(const wchar_t* fname)
{
  debugmode = false;
  const std::locale empty_locale = std::locale::empty();
  typedef std::codecvt_utf8<wchar_t> converter_type;
  const converter_type* converter = new converter_type;
  const std::locale utf8_locale = std::locale(empty_locale, converter);
  // std::wifstream stream(L"test.txt");
  std::wifstream* ifs = new wifstream(fname);
  if (ifs == nullptr) {
    return false;
  }
  ifs->imbue(utf8_locale);
  std::wstring line;
  //  std::getline(ifs, line);
  // std::system("pause");
  // wchar_t linebuff[1000];
  wstring linebuff;
  //char linebuff2[1000];
  size_t i;
  bool pctliststart = false;
  picturefiles = new std::vector<CSSPicturefile*>();
  wstring* pfile;
  CSSPicturefile* pctbuff;
  while (true)
  {
    if (ifs->eof() == true)
    {
      break;
    }
    std::getline(*ifs, line);
    // ifs->getline(linebuff, 1000);
    // skip comment
    if (line.length() == 0)
    {
      continue;
    }
    i = line.compare(0, 2, L"//");
    if (i == 0) {
      continue;
    }

    if (pctliststart == true) {
      // multiplyでもそのまま文字を格納？
      // 別配列にする？ classを別に作る
      pctbuff = new CSSPicturefile();
      pctbuff->multiply = false;
      // alpha:0.25の指定　cmdの指定がないので、空白で区切れない
      // c# のようにspaceでsplitできればいいんだけど、、、

      // たぶんだが、wstringstreamに対して getlineが正常に実装されていないのでは？

      //wstringstream  wsst(line);
      //vector<std::wstring> cols;
      //wstring wsbuff;
      //char dlim;
      //dlim = ' ';
      //bool ret = getline(wsst, wsbuff, dlim);
      //while (ret == true) {
      //  cols.push_back(wsbuff);
      //}
      // 体\00.png [multiply] alpha:0.25
      i = line.find(L"alpha");
      pctbuff->alpha = 1;
      if (i != std::string::npos)
      {
        wstring wbuff = line.substr(i + 6);
        double d = std::stod(wbuff);
        pctbuff->alpha = d;
        line = line.substr(0, i - 1);
      }



      i = line.find(L"[multiply]");
      if (i != std::string::npos) {
        // multiply指定
        line = line.substr(0, i - 1);
        pctbuff->multiply = true;
      }
      pfile = new wstring(*basedir);
      pfile->append(L"\\");
      pfile->append(line);
      pctbuff->filename = pfile;

      picturefiles->push_back(pctbuff);
      // size_t i = picturefiles->size();
      pfile = nullptr;
      continue;
    }

    // debugmode
    i = line.compare(0, 5, L"debug");
    if (i == 0) {
      debugmode = true;
      continue;
    }

    // get out filename
    i = line.compare(0, 3, L"out");
    if (i == 0) {
      i = line.find(L"=");
      wstring sbuff = line.substr(i + 1);
      outfile = new wstring(sbuff);
      continue;
    }

    // get size
    i = line.compare(0, 4, L"size");
    if (i == 0) {
      i = line.find(L"=");
      wstring sbuff = line.substr(i + 1);
      i = sbuff.find(L"x");
      wstring sw = sbuff.substr(0, i);
      wstring sh = sbuff.substr(i + 1);
      width = std::stoi(sw);
      height = std::stoi(sh);
      continue;
    }

    // getbase
    i = line.compare(0, 4, L"base");
    if (i == 0) {
      i = line.find(L"=");
//      wstring sbuff =
      basedir = new wstring(line.substr(i + 1));
      continue;
    }
     
    // get picture list
    // picturelist
    i = line.compare(0, 11, L"picturelist");
    if (i == 0) {
      pctliststart = true;
      continue;
    }
  }

  ifs->close();

  // auto b = picturefiles->begin();
  // wstring xb(*b);

  return true;

}



