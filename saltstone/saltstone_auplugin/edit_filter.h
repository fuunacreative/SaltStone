#include "pch.h"
#include <string>

#pragma once

#ifndef EDIT_FILTER_H
#define EDIT_FILTER_H


#define	MID_EDIT_VIDEO_COPY		100
#define	MID_EDIT_AUDIO_COPY		101
#define	MID_EDIT_COPY			102
#define	MID_EDIT_PASTE			103
#define	MID_EDIT_DELETE			104
#define	MID_EDIT_INSERT			105
#define	MID_EDIT_FILE_INFO		106


void func_exfilter_info(void* editp, FILTER* fp);
std::string UTF8toSjis(std::string srcUTF8);

#endif
