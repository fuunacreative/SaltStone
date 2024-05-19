// pch.h: プリコンパイル済みヘッダー ファイルです。
// 次のファイルは、その後のビルドのビルド パフォーマンスを向上させるため 1 回だけコンパイルされます。
// コード補完や多くのコード参照機能などの IntelliSense パフォーマンスにも影響します。
// ただし、ここに一覧表示されているファイルは、ビルド間でいずれかが更新されると、すべてが再コンパイルされます。
// 頻繁に更新するファイルをここに追加しないでください。追加すると、パフォーマンス上の利点がなくなります。

#ifndef PCH_H
#define PCH_H

// #include <afxwin.h>
// #include <Windows.h>

// プリコンパイルするヘッダーをここに追加します

#include "framework.h"
#include "filter.h"
#include "edit_filter.h"
#include "exedit.h"
#include "obj.h"
#include "hex.h"
#include "util.h"
typedef unsigned int uint;

#endif //PCH_H
