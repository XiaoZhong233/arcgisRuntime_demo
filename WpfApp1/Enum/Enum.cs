﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Enum
{
    public enum OperationType
    {
        无,
        画点,
        画线,
        画面,
        编辑顶点,
        选择,
        裁剪,
        缓冲区,
        分割,
        查询
    }

    public enum QueryGeoType
    {
        无,
        点,
        矩形
    }
}
