﻿using GenshinPray.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinPray.Service
{
    public class PermPrayService : BasePrayService
    {
        protected override YSPrayRecord getPrayRecord(YSPrayRecord prayRecord, int floor180Surplus, int floor20Surplus)
        {
            throw new NotImplementedException();
        }

        protected override bool isUpItem(YSGoodsItem goodsItem)
        {
            throw new NotImplementedException();
        }
    }
}